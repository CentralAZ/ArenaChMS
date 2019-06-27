using System;
using System.Security.Cryptography;
using System.Text;
using Arena.Core;
using Arena.Custom.Cccev.FrameworkUtils.FrameworkConstants;
using Arena.Portal;

namespace ArenaWeb.UserControls.Custom.Cccev.Web
{
    public partial class FacebookConnectAccount : PortalControl
    {
        [TextSetting("Facebook App ID", "Your church's Facebook App ID", true)]
        public string FacebookAppIDSetting { get { return Setting("FacebookAppID", "", true); } }

        protected string state;
        protected bool hasOptedOut = true;
        protected bool connectedToFacebook = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentPerson != null)
            {
                LoadFbAttributes();
            }
            
            if (!connectedToFacebook && !hasOptedOut)
            {
                BasePage.AddJavascriptInclude(Page, "include/scripts/custom/cccev/lib/mustache.min.js");
                BasePage.AddJavascriptInclude(Page, "include/scripts/custom/cccev/lib/underscore.min.js");
                BasePage.AddJavascriptInclude(Page, "include/scripts/custom/cccev/lib/backbone.min.js");
                BasePage.AddJavascriptInclude(Page, "usercontrols/custom/cccev/web/scripts/centralaz.web.min.js");
                BasePage.AddJavascriptInclude(Page, "usercontrols/custom/cccev/web/scripts/centralaz.facebook.min.js");
                BasePage.AddCssLink(Page, "usercontrols/custom/cccev/web/css/forms.css");

                GetState();
                ihState.Value = state;
            }
        }

        private void LoadFbAttributes()
        {
            const string OPT_OUT_KEY = "CentralAZ.Web.FacebookAuth.OptOut";
            const string CONNECT_KEY = "CentralAZ.Web.FacebookAuth.AccountIsConnected";

            //if (Session[OPT_OUT_KEY] != null || Session[CONNECT_KEY] != null)
            //{
            //    hasOptedOut = Convert.ToBoolean(Session[OPT_OUT_KEY]);
            //    connectedToFacebook = Convert.ToBoolean(Session[CONNECT_KEY]);
            //    return;
            //}

            var optOutAttribute = new Arena.Core.Attribute(SystemGuids.FACEBOOK_OPT_OUT_ATTRIBUTE);
            var optedOut = new PersonAttribute(CurrentPerson.PersonID, optOutAttribute.AttributeId);
            hasOptedOut = Convert.ToBoolean(optedOut.IntValue);
            //Session[OPT_OUT_KEY] = hasOptedOut;

            var facebookIdAttribute = new Arena.Core.Attribute(SystemGuids.FACEBOOK_USER_ID_ATTRIBUTE);
            var facebookID = new PersonAttribute(CurrentPerson.PersonID, facebookIdAttribute.AttributeId);
            connectedToFacebook = !string.IsNullOrEmpty(facebookID.StringValue);
            //Session[CONNECT_KEY] = connectedToFacebook;
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
    }
}