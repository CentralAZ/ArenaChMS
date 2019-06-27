/**********************************************************************
* Description:	Renders standard SWFObject JavaScript code to embed a Flash Movie onto a page.
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created:	4/20/2009
*
* $Workfile: FlashImageRotator.ascx.cs $
* $Revision: 11 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/FlashImageRotator.ascx.cs   11   2010-01-27 15:49:28-07:00   JasonO $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/FlashImageRotator.ascx.cs $
*  
*  Revision: 11   Date: 2010-01-27 22:49:28Z   User: JasonO 
*  Cleaning up. 
*  
*  Revision: 10   Date: 2009-07-28 17:12:19Z   User: JasonO 
*  Adding header comment block. 
**********************************************************************/

using System;
using System.Text;
using System.Web.UI;
using Arena.Portal;

namespace ArenaWeb.UserControls.Custom.Cccev.WebUtils
{
    public partial class FlashImageRotator : PortalControl
    {
        [TextSetting("XML Document Path", "Relative path to the XML document that dictates what renders in the Flash player.", true)]
        public string XmlDocumentPathSetting { get { return Setting("XmlDocumentPath", "", true); } }

        [TextSetting("Flash Video Player URL", "URL pointing to the Flash movie player to be loaded. (Leave blank for default player).", false)]
        public string FlashVideoUrlSetting { get { return Setting("FlashVideoUrl", "UserControls/Custom/Cccev/WebUtils/FlashImageRotator.swf", false); } }

        [NumericSetting("Flash Movie Height", "Height (in px) of the Flash movie.", true)]
        public string FlashHeightSetting { get { return Setting("FlashHeight", "", true); } }

        [NumericSetting("Flash Movie Width", "Width (in px) of the Flash movie.", true)]
        public string FlashWidthSetting { get { return Setting("FlashWidth", "", true); } }

        [BooleanSetting("Link From Display", "Determines whether or not to navigate to item link by clicking on Flash Display (Defaults to false).", false, false)]
        public string LinkFromDisplaySetting { get { return Setting("LinkFromDisplay", "false", false); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                smpScripts.Scripts.Add(new ScriptReference("~/Templates/Cccev/liger/js/swfobject.js"));
                BuildSwfObject();
            }
        }

        private void BuildSwfObject()
        {
            StringBuilder swfobject = new StringBuilder();
            swfobject.Append("\n\t<script type=\"text/javascript\">\n");
            swfobject.AppendFormat("\t\tvar swf = new SWFObject('{0}', 'rotator', '{1}', '{2}', '7');\n", 
                Server.UrlEncode(FlashVideoUrlSetting), FlashWidthSetting, FlashHeightSetting);
            swfobject.Append("\t\tswf.addParam('allowfullscreen', 'false');\n");
            swfobject.Append("\t\tswf.addParam('wmode', 'transparent');\n");
            swfobject.Append("\t\tswf.addVariable('usefullscreen', 'false');\n");
            swfobject.AppendFormat("\t\tswf.addVariable('file', '{0}');\n", Server.UrlEncode(XmlDocumentPathSetting));
            swfobject.AppendFormat("\t\tswf.addVariable('width', '{0}');\n", FlashWidthSetting);
            swfobject.AppendFormat("\t\tswf.addVariable('height', '{0}');\n", FlashHeightSetting);
            swfobject.AppendFormat("\t\tswf.addVariable('linkfromdisplay', '{0}');\n", LinkFromDisplaySetting);
            swfobject.Append("\t\tswf.addVariable('shuffle', 'false');\n");
            swfobject.Append("\t\tswf.write('image_rotator');\n");
            swfobject.Append("\t</script>\n");
            phFlashPlayer.Controls.Add(new LiteralControl(swfobject.ToString()));
        }
    }
}