namespace ArenaWeb.UserControls.Custom.Cccev.Core
{
    using System;
    using System.Xml;
    using System.Xml.Xsl;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.Caching;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using Arena.Portal;
    using Arena.Portal.UI;
    using Arena.Exceptions;
    using Arena.SmallGroup;
    using Arena.Security;

    /// <summary>
    ///		Summary description for XmlTransformation.
    /// </summary>
    public partial class XmlFileTransform : PortalControl
    {
        #region Module Settings

        // Module Settings
        [FileSetting("XML File Path", "The path of the XML file.", true)]
        public string XMLSetting { get { return Setting("XML", "", true); } }

        [FileSetting("XSLT File Path", "The path of the XSLT file.", true)]
        public string XSLTSetting { get { return Setting("XSLT", "", true); } }

        [TextSetting("Param Name", "Optional name of parameters.", false)]
        public string ParamNameSetting { get { return Setting("ParamName", "", false); } }

        [TextSetting("User Name", "Optional user name for a remote XML file.", false)]
        public string UserNameSetting { get { return Setting("UserName", "", false); } }

        [TextSetting("Password", "Optional password for a remote XML file.", false)]
        public string PasswordSetting { get { return Setting("Password", "", false); } }

        [NumericSetting( "Time to Cache (minutes)", "Time in minutes to cache the XML file (if http). Use 0 for no caching. Default 1.", false )]
        public string TimeCacheMinutesSetting { get { return Setting( "TimeCacheMinutes", "1", false ); } }

        [BooleanSetting( "Use Error Panel", "Flag indicating whether to use the error panel to display errors; otherwise it will insert 'errors' into the XML document for display by your XSLT. Default true.", false, true )]
        public bool UseErrorPanelSetting { get { return Convert.ToBoolean( Setting( "UseErrorPanel", "true", false ) ); } }

        #endregion
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                if (ParamNameSetting != "")
                {
                    string ParamName = ParamNameSetting;
                    string QualifiedParamName = CurrentModule.ModuleInstanceID.ToString("0000") + ParamName;
                    string ParamValue = string.Empty;
                    if (Request.QueryString[ParamName] != null)
                        ParamValue = Request.QueryString[ParamName];
                    else if (Request.Form[ParamName] != null)
                        ParamValue = Request.Form[ParamName];
                    else if (Session[QualifiedParamName] != null)
                        ParamValue = (string)Session[QualifiedParamName];

                    Session[QualifiedParamName] = ParamValue;
                    XsltArgumentList xarg = new XsltArgumentList();
                    xarg.AddParam(ParamName, "", ParamValue);
                    xmlMain.TransformArgumentList = xarg;
                }

                string xmlSetting = XMLSetting;
                if (XMLSetting.Contains("{0}") && Request.QueryString["xml"] != null)
                    xmlSetting = string.Format(XMLSetting, Request.QueryString["xml"].ToString());

                if (xmlSetting.ToLower().StartsWith("http"))
                {
                    LoadRemoteXML( xmlSetting );
                }
                else
                    xmlMain.XmlFileURL = xmlSetting;

                foreach (XmlNode node in xmlMain.Document.DocumentElement.SelectNodes("//*[@pageid]"))
                    if (node.Attributes["pageid"] != null)
                    {
                        try
                        {
                            int pageID = Int32.Parse(node.Attributes["pageid"].Value);
                            PortalPage page = new PortalPage(pageID);
                            if (!page.Permissions.Allowed(OperationType.View, CurrentUser))
                                node.ParentNode.RemoveChild(node);
                        }
                        catch
                        {
                            xmlMain.Document.DocumentElement.RemoveChild(node);
                        }
                    }

                xmlMain.XslFileURL = XSLTSetting;
            }
            catch (System.Exception ex)
            {
                string errorMessage = "";
                Exception myEx = ex;
                while (myEx != null)
                {
                    errorMessage += myEx.Message + "<br/><br/>";
                    myEx = myEx.InnerException;
                }

                if ( UseErrorPanelSetting )
                {
                    pnlError.Controls.Clear();
                    pnlError.Controls.Add( new LiteralControl( errorMessage ) );
                    pnlError.Visible = true;
                    xmlMain.Visible = false;
                }
                else
                {   
                    // Stuff "errors" into the XmlDocument so the error can be handled by the Xslt
                    xmlMain.Document = new XmlDocument();
                    XmlNode rootNode = xmlMain.Document.CreateNode( XmlNodeType.Element, "errors", xmlMain.Document.NamespaceURI );
                    xmlMain.Document.AppendChild( rootNode );

                    // Build the node for the channel
                    XmlNode containerNode = xmlMain.Document.CreateNode( XmlNodeType.Element, "error", xmlMain.Document.NamespaceURI );
                    rootNode.AppendChild( containerNode );

                    XmlAttribute attribute = xmlMain.Document.CreateAttribute( "value" );
                    attribute.Value = ex.Message;
                    containerNode.Attributes.Append( attribute );
                    xmlMain.XslFileURL = XSLTSetting;
                }
            }
        }

        /// <summary>
        /// Loads the remote XML from cache if available otherwise fetches normally.
        /// Also if user and password is set, then fetches using the XmlTransform.LoadRemoteUrl method.
        /// </summary>
        /// <param name="xmlSetting"></param>
        private void LoadRemoteXML(string xmlSetting)
        {
            xmlMain.Document = null;
            string cacheKey = string.Empty;

            bool cacheEnabled = ( TimeCacheMinutesSetting.Equals( "0" ) ) ? false : true;

            if ( cacheEnabled )
            {
                cacheKey = String.Format( "ArenaWeb.UserControls.XmlFileTransform:Page:{0}:Module:{1}", CurrentPortalPage.PortalPageID, CurrentModule.ModuleInstanceID );
            }

            // fetch from cache if enabled, is in the cache, and a refreshcache param is not present
            if ( cacheEnabled && Cache[cacheKey] != null && Request.QueryString["refreshcache"] == null )
            {
                xmlMain.Document = (XmlDocument)Cache[cacheKey];
            }
            else
            {
                if ( UserNameSetting != "" && PasswordSetting != "" )
                {
                    xmlMain.LoadRemoteURL( xmlSetting, UserNameSetting, PasswordSetting );
                }
                else
                {
                    xmlMain.Document = new XmlDocument();
                    xmlMain.Document.Load( xmlSetting );
                }

                if ( cacheEnabled )
                {
                    Cache.Add( cacheKey, xmlMain.Document, null, DateTime.Now.AddMinutes( int.Parse( TimeCacheMinutesSetting ) ), Cache.NoSlidingExpiration, CacheItemPriority.High, null );
                }
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion
    }
}
