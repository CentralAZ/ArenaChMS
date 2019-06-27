/**********************************************************************
* Description:  Basic login functionality via Facebook's OAuth 2.0 API.
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created: 8/29/2011
*
* $Workfile: FacebookLogIn.ascx.cs $
* $Revision: 12 $
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Web/FacebookLogIn.ascx.cs   12   2011-12-05 12:04:14-07:00   nicka $
*
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Web/FacebookLogIn.ascx.cs $
*  
*  Revision: 12   Date: 2011-12-05 19:04:14Z   User: nicka 
*  Added "requestUrl" to the GetRedirectPath to resolve bug whereby users are 
*  not getting redirected back to the page they were on when they clicked 
*  log-in. 
*  
*  Revision: 11   Date: 2011-11-22 22:19:27Z   User: JasonO 
*  Tweaks to facebook functionality. Adding ability to auto-connect to 
*  facebook account when logging in through facebook for the first time. 
*  
*  Revision: 10   Date: 2011-11-22 00:16:14Z   User: JasonO 
*  Functionality tweaks and css tweaks for Facebook login and connect. 
*  
*  Revision: 9   Date: 2011-11-02 19:13:01Z   User: JasonO 
*  Removing dependency on jQuery.tmpl plugin as it's no longer supported. 
*  Refactored to use mustache.js instead. 
*  
*  Revision: 8   Date: 2011-11-02 18:06:28Z   User: JasonO 
*  First working iteration of Facebook Authentication and Facebook Account 
*  Connection. 
*  
*  Revision: 7   Date: 2011-10-20 23:07:30Z   User: JasonO 
*  Tweaks to facebook login/registration 
*  
*  Revision: 6   Date: 2011-10-20 00:22:31Z   User: JasonO 
*  Tweaks to Facebook login process. Re-namespacing javascript 
*  
*  Revision: 5   Date: 2011-10-18 23:36:39Z   User: JasonO 
*  Tweaking folder structure. 
*  
*  Revision: 4   Date: 2011-09-29 16:37:46Z   User: JasonO 
*  Adding latest jQuery version to Facebook Auth module. 
*  
*  Revision: 3   Date: 2011-09-28 21:57:46Z   User: JasonO 
*  Updating directory structure to reflect standards for 
*  backbone.js/coffeescript and testing with jasmine. 
*  
*  Revision: 2   Date: 2011-09-14 22:22:32Z   User: JasonO 
*  Adding tests for Facebook Authorization feature. 
*  
*  Revision: 1   Date: 2011-09-01 22:45:30Z   User: JasonO 
*  Adding/refinding Facebook authentication functionality. 
**********************************************************************/

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using Arena.Core;
using Arena.Custom.Cccev.FrameworkUtils.FrameworkConstants;
using Arena.DataLayer.Core;
using Arena.Enums;
using Arena.Portal;
using Arena.Security;
using Facebook;

namespace ArenaWeb.UserControls.Custom.Cccev.Web
{
    public partial class FacebookLogIn : PortalControl
    {
        [PageSetting("User Profile Page", "Optional page to redirect user to after they log in.", false)]
        public string UserProfilePageSetting { get { return Setting("UserProfilePage", "", false); } }

        [TextSetting("Facebook App ID", "Your church's Facebook App ID", true)]
        public string FacebookAppIDSetting { get { return Setting("FacebookAppID", "", true); } }

        [PageSetting("Forgot Password Page", "Page containing module to retrieve/reset your password.", true)]
        public string ForgotPasswordPageSetting { get { return Setting("ForgotPasswordPage", "", true); } }

        [PageSetting("Forgot Username Page", "Page containing module to retrieve forgotten username.", true)]
        public string ForgotUsernamePageSetting { get { return Setting("ForgotUsernamePage", "", true); } }

        protected string state;
        private const string CREATED_BY = "FacebookLogin";

        protected void Page_Init(object sender, EventArgs e)
        {
            btnRegister.Click += Register_Click;
            btnLogin.Click += btnLogin_Click;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblLoginError.Visible = false;
                lblSignupError.Visible = false;
            }

            BasePage.AddJavascriptInclude(Page, "include/scripts/custom/cccev/lib/mustache.min.js");
            BasePage.AddJavascriptInclude(Page, "include/scripts/custom/cccev/lib/underscore.min.js");
            BasePage.AddJavascriptInclude(Page, "include/scripts/custom/cccev/lib/backbone.min.js");
            BasePage.AddJavascriptInclude(Page, "usercontrols/custom/cccev/web/scripts/centralaz.web.min.js");
            BasePage.AddJavascriptInclude(Page, "usercontrols/custom/cccev/web/scripts/centralaz.facebook.min.js");
            BasePage.AddCssLink(Page, "usercontrols/custom/cccev/web/css/forms.css");

            GetState();
            ihState.Value = state;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = HttpContext.Current.Request.UserHostAddress;
            }

            var username = tbUsername.Text;
            var personID = PortalLogin.Authenticate(username, tbPassword.Text, ipAddress, CurrentOrganization.OrganizationID);
            
            if (personID != -1)
            {
                var login = new Login(username);
                FormsAuthentication.SetAuthCookie(login.LoginID, false);
                HttpContext.Current.Response.Cookies["portalroles"].Value = string.Empty;

                try
                {
                    var person = new Person(personID);
                    var facebookUser = GetFacebookUser();
                    SavePersonAttribute(person, facebookUser["id"].ToString(), CurrentOrganization.OrganizationID);
                }
                catch (FacebookApiException ex)
                {
                    new ExceptionHistoryData().AddUpdate_Exception(ex, CurrentOrganization.OrganizationID,
                        "Cccev.Web", ArenaContext.Current.ServerUrl);
                }

                Redirect();
            }

            lblLoginError.Text = "Please enter a valid username and password.";
            lblLoginError.Visible = true;
        }

        private void Register_Click(object sender, EventArgs e)
        {
            if (state != ihState.Value)
            {
                // NOTE: If this is invalid, something's not right with the Facebook session...
                //lblSignupError.Text = "Oops, something wonky happened. Please reload the page and ";
                //lblSignupError.Visible = true;
                return;
            }

            try
            {
                var facebookUser = GetFacebookUser();
                var id = facebookUser["id"].ToString();
                
                string password = string.Empty;
                var person = PopulatePerson(facebookUser, CREATED_BY);
                person.Save(CurrentOrganization.OrganizationID, CREATED_BY, true);

                // Thanks Arena!
                var trash = person.Logins;
                var loginID = person.AddLogin(true, CREATED_BY, out password);

                // Add a PersonAttribute to their record with their Facebook ID.
                SavePersonAttribute(person, id, CurrentOrganization.OrganizationID);
                FormsAuthentication.SetAuthCookie(loginID, false);
                Response.Cookies["portalroles"].Value = string.Empty;
                Redirect();
            }
            catch (FacebookApiException ex)
            {
                new ExceptionHistoryData().AddUpdate_Exception(ex, CurrentOrganization.OrganizationID,
                    "Cccev.Web", ArenaContext.Current.ServerUrl);
            }
        }

        private IDictionary<string, object> GetFacebookUser()
        {
            var client = new FacebookClient(ihAccessToken.Value);
            return (IDictionary<string, object>)client.Get("me") ?? new Dictionary<string, object> { { "id", string.Empty } };
        }

        private static void SavePersonAttribute(Person person, string facebookID, int orgID)
        {
            var attribute = new Arena.Core.Attribute(SystemGuids.FACEBOOK_USER_ID_ATTRIBUTE);
            var facebookSetting = new PersonAttribute
            {
                PersonID = person.PersonID,
                AttributeId = attribute.AttributeId,
                StringValue = facebookID
            };

            facebookSetting.Save(orgID, CREATED_BY);
        }

        protected string GetRedirectPath()
        {
            string destination;

            if ( !string.IsNullOrEmpty( Request.QueryString["requestUrl"] ) )
			{
				destination = Request.QueryString["requestUrl"];
			}
			else if ( !string.IsNullOrEmpty( Request.QueryString["requestpage"] ) )
			{
				destination = Request.QueryString["requestpage"];
			}
			else if ( !string.IsNullOrEmpty( UserProfilePageSetting ) )
			{
				destination = string.Format( "default.aspx?page={0}", UserProfilePageSetting );
			}
			else
			{
				destination = "default.aspx";
			}

            return destination;
        }

        private void Redirect()
        {
            Response.Redirect(GetRedirectPath());
        }

        private void GetState()
        {
            if (Session["CentralAZ.Web.FacebookAuth.State"] == null)
            {
                var md5 = MD5.Create();
                var stateString = Guid.NewGuid().ToString();
                var input = Encoding.ASCII.GetBytes(stateString);
                var hash = md5.ComputeHash(input);
                var sb = new StringBuilder();

                for (var i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }

                state = sb.ToString();
                Session["CentralAZ.Web.FacebookAuth.State"] = state;
            }
            else
            {
                state = Session["CentralAZ.Web.FacebookAuth.State"].ToString();
            }
        }

        private Person PopulatePerson(IDictionary<string, object> result, string createdBy)
        {
            DateTime facebookBirthdate;
            DateTime userSuppliedBirthdate;
            var lookupID = CurrentOrganization.Settings["CentralAZ.Web.FacebookRegistration.MembershipStatus"];

            if (!DateTime.TryParse(result["birthday"].ToString(), out facebookBirthdate))
            {
                facebookBirthdate = new DateTime(1900, 1, 1);
            }

            if (!DateTime.TryParse(tbBirthdate.Text, out userSuppliedBirthdate))
            {
                userSuppliedBirthdate = new DateTime(1900, 1, 1);
            }

            var person = new Person
                             {
                                 FirstName = result["first_name"].ToString(),
                                 LastName = result["last_name"].ToString(),
                                 RecordStatus = RecordStatus.Pending,
                                 MemberStatus = new Lookup(int.Parse(lookupID)),
                                 BirthDate = (facebookBirthdate != userSuppliedBirthdate && userSuppliedBirthdate != new DateTime(1900, 1, 1))
                                         ? userSuppliedBirthdate
                                         : facebookBirthdate
                             };

            // Create new person object, and register an Arena login for them.
            person.Save(CurrentOrganization.OrganizationID, createdBy, false);
            var email = new PersonEmail
                            {
                                Active = true,
                                AllowBulkMail = true,
                                Email = result["email"].ToString(),
                                PersonId = person.PersonID
                            };

            person.Emails.Add(email);
            return person;
        }
    }
}