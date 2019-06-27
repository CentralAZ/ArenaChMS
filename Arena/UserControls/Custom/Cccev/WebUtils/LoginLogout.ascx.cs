/**********************************************************************
 * Description:	A custom login logout control that implements the return
 *              to previous page behavior.
 * Created By:   Nick Airdo @ Central Christian Church of the East Valley
 * Date Created: 04/09/2008 19:00
 *
 * $Workfile: LoginLogout.ascx.cs $
 * $Revision: 9 $ 
 * $Header: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/LoginLogout.ascx.cs   9   2010-02-17 09:52:47-07:00   JasonO $
 *  
 * $Log: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/LoginLogout.ascx.cs $
*  
*  Revision: 9   Date: 2010-02-17 16:52:47Z   User: JasonO 
*  Fixing jquery include issues. 
*  
*  Revision: 8   Date: 2009-09-09 18:54:40Z   User: nicka 
*  added switch to control whether or not to use popup login form. 
*  
*  Revision: 7   Date: 2009-04-08 22:11:52Z   User: JasonO 
*  
*  Revision: 6   Date: 2009-04-08 22:11:04Z   User: JasonO 
*  
*  Revision: 5   Date: 2009-03-31 01:32:38Z   User: nicka 
*  Corrected PostBackUrl 
*  
*  Revision: 4   Date: 2009-03-25 23:13:46Z   User: DallonF 
*  Shows Register link 
*  
*  Revision: 3   Date: 2009-03-25 21:27:15Z   User: nicka 
*  Liger beta 0.5 
*  
*  Revision: 2   Date: 2008-09-08 23:22:36Z   User: nicka 
*  change DIV to SPAN 
*  
*  Revision: 1   Date: 2008-04-09 19:41:19Z   User: nicka 
*  First version of a custom LoginLogout module 
 **********************************************************************/
using System;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using Arena.Portal;

namespace ArenaWeb.UserControls.Custom.Cccev.WebUtils
{
    public partial class LoginLogout : PortalControl
    {

        #region Module Setting
        [PageSetting("Register Page", "Page to use when registering a new account", false)]
        public string RegisterPageSetting
        {
            get { return base.Setting("RegisterPage", "", false); }
        }

        [BooleanSetting("Return to Current Page", "Set to true to return to the current page if a RedirectURL is not specified.", false, false)]
        public bool ReturnToCurrentPageSetting
        {
            get { return Boolean.Parse(base.Setting("ReturnToCurrentPage", "false", false)); }
        }

		[BooleanSetting( "Show Liger Popup", "Set to true to show the liger popup (User Login module).", false, false )]
		public bool ShowLigerPopupSetting
		{
			get { return Boolean.Parse( base.Setting( "ShowLigerPopup", "false", false ) ); }
		}

        [CssSetting("Css Class", "Css Class of the Login/Logout text.", false)]
        public string CssClassSetting
        {
            get
            {
                return base.Setting("CssClass", "", false);
            }
        }

        [ImageSetting("Login Image Hover", "Hover image to show when user is not logged in.", false)]
        public string LoginImageHoverSetting
        {
            get
            {
                return base.Setting("LoginImageHover", "", false);
            }
        }

        [ImageSetting("Login Image", "Image to show when user is not logged in.", false)]
        public string LoginImageSetting
        {
            get
            {
                return base.Setting("LoginImage", "", false);
            }
        }

        [TextSetting("Login Text", "Text to display when user is not logged in.", false)]
        public string LoginTextSetting
        {
            get
            {
                return base.Setting("LoginText", "", false);
            }
        }

        [ImageSetting("Logout Image Hover", "Hover image to display when user is logged in.", false)]
        public string LogoutImageHoverSetting
        {
            get
            {
                return base.Setting("LogoutImageHover", "", false);
            }
        }

        [ImageSetting("Logout Image", "Image to display when user is logged in.", false)]
        public string LogoutImageSetting
        {
            get
            {
                return base.Setting("LogoutImage", "", false);
            }
        }

        [TextSetting("Logout Text", "Text to display when user is logged in.", false)]
        public string LogoutTextSetting
        {
            get
            {
                return base.Setting("LogoutText", "", false);
            }
        }

        [ImageSetting("Register Image Hover", "Hover image to show when user is not logged in.", false)]
        public string RegisterImageHoverSetting
        {
            get
            {
                return base.Setting("RegisterImageHover", "", false);
            }
        }

        [ImageSetting("Register Image", "Image to show when user is not logged in.", false)]
        public string RegisterImageSetting
        {
            get
            {
                return base.Setting("RegisterImage", "", false);
            }
        }

        [TextSetting("Register Text", "Text to display when user is not logged in.", false)]
        public string RegisterTextSetting
        {
            get
            {
                return base.Setting("RegisterText", "", false);
            }
        }

        [TextSetting("Redirect URL", "The URL to redirect user to after they have successfully logged in.", false)]
        public string RedirectURLSetting
        {
            get
            {
                return base.Setting("RedirectURL", "", false);
            }
        }
        #endregion

        // Methods
        private void ibLog_Click(object sender, ImageClickEventArgs e)
        {
            this.LogInOut();
        }

        private void lbLog_Click(object sender, EventArgs e)
        {
            this.LogInOut();
        }

        private void LogInOut()
        {
            if (!base.Request.IsAuthenticated)
            {
                string str = string.Empty;
                if (this.RedirectURLSetting != string.Empty)
                {
                    str = "&requestUrl=" + this.Page.Server.UrlEncode(this.RedirectURLSetting);
                }
                else if (ReturnToCurrentPageSetting)
                {
                    str = "&requestUrl=" + this.Page.Server.UrlEncode(Request.RawUrl);
                }
                base.Response.Redirect(string.Format("~/Default.aspx?page={0}{1}", base.CurrentPortal.LoginPageID.ToString(), str));
            }
            else
            {
                FormsAuthentication.SignOut();
                base.Response.Cookies["portalroles"].Value = null;
                base.Response.Cookies["portalroles"].Path = "/";
                base.Response.Redirect(base.Request.ApplicationPath);
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            this.lbLog.Click += new EventHandler(this.lbLog_Click);
            this.ibLog.Click += new ImageClickEventHandler(this.ibLog_Click);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                smpScripts.Scripts.Add(new ScriptReference(string.Format("~/{0}", BasePage.JQUERY_INCLUDE)));
                smpScripts.Scripts.Add(new ScriptReference(string.Format("~/Templates/Cccev/liger/js/popover.js")));
                smpScripts.Scripts.Add(new ScriptReference(string.Format("~/Templates/Cccev/liger/js/form.js")));
            }

			if ( ! ShowLigerPopupSetting )
			{
				lbLog.CssClass = "no-popup";
			}
            string loginTextSetting = string.Empty;
            string logoutImageSetting = string.Empty;
            string logoutImageHoverSetting = string.Empty;
            if (!base.Request.IsAuthenticated)
            {
                loginTextSetting = this.LoginTextSetting;
                logoutImageSetting = this.LogoutImageSetting;
                logoutImageHoverSetting = this.LogoutImageHoverSetting;

                if (!RegisterPageSetting.Equals(string.Empty))
                {
                    if (RegisterImageSetting != String.Empty)
                    {
                        lbReg.Visible = false;
                        ibReg.Visible = true;
                        ibReg.ImageUrl = "~/" + RegisterImageSetting;
                        ibReg.Attributes.Clear();
                        ibReg.PostBackUrl = "~/Default.aspx?page=" + RegisterPageSetting;
                        this.ibReg.Attributes.Add("onmouseout", string.Format("javascript:this.src='{0}'", RegisterImageSetting));
                        this.ibReg.Attributes.Add("onmouseover", string.Format("javascript:this.src='{0}'", RegisterImageHoverSetting));
                    }
                    else
                    {
                        lbReg.Visible = true;
                        ibReg.Visible = false;
                        lbReg.Text = RegisterTextSetting;
                        lbReg.PostBackUrl = "~/Default.aspx?page=" + RegisterPageSetting;
                    }
                }
                
            }
            else
            {
                loginTextSetting = this.LogoutTextSetting;
                logoutImageSetting = this.LoginImageSetting;
                logoutImageHoverSetting = this.LoginImageHoverSetting;
            }
            if (this.LoginImageSetting != string.Empty)
            {
                this.lbLog.Visible = false;
                this.ibLog.Visible = true;
                this.ibLog.ImageUrl = "~/" + logoutImageSetting;
                this.ibLog.Attributes.Clear();
                this.ibLog.Attributes.Add("onmouseout", string.Format("javascript:this.src='{0}'", logoutImageSetting));
                this.ibLog.Attributes.Add("onmouseover", string.Format("javascript:this.src='{0}'", logoutImageHoverSetting));
            }
            else
            {
                this.lbLog.Visible = true;
                this.ibLog.Visible = false;
                this.lbLog.Text = loginTextSetting;
            }
            if (this.CssClassSetting != string.Empty)
            {
                this.lbLog.CssClass = this.CssClassSetting;
            }
        }


    }
}
