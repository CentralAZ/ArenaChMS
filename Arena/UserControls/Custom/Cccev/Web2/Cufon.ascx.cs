/**********************************************************************
* Description:	Fast text replacement with canvas and VML - no Flash or images required.
* 
*				Based 100% on the Cufon http://github.com/sorccu/cufon
*				
* Created By:	Nick Airdo @ Central Christian Church (Cccev)
* Date Created:	2/23/2010 6:32:36 AM
*
* $Workfile: Cufon.ascx.cs $
* $Revision: 3 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Web2/Cufon.ascx.cs   3   2010-06-02 16:35:51-07:00   JasonO $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Web2/Cufon.ascx.cs $
*  
*  Revision: 3   Date: 2010-06-02 23:35:51Z   User: JasonO 
*  Bug Fix: Cufon wouldn't replace fonts with more complicated CSS selectors 
*  in IE7 or below. It depends on an existing selector framework (like 
*  jQuery's) or will use what's available in browser. In IE7 or below, it has 
*  to rely on document.getElementByTagName(), thus causing problems with 
*  dynamic font replacement via CSS selector. Injecting script includes via 
*  ScriptManagerProxy to ensure they're included after jQuery. 
*  
*  Revision: 2   Date: 2010-05-20 19:57:08Z   User: nicka 
*  Added jQuery Bind feature 
*  
*  Revision: 1   Date: 2010-02-23 15:58:15Z   User: nicka 
*  initial version 
**********************************************************************/
using System.Web.UI;

using Arena.Portal;
using System.Text;

namespace ArenaWeb.UserControls.Custom.Cccev.Web2
{
	public partial class Cufon : PortalControl
	{
		#region Module Settings
		[TextSetting( "Cufon-yui.js Path", "Path to the Cufon-yui.js script. Default 'UserControls/Custom/Cccev/Web2/js/cufon-yui.js'.", false )]
		public string CufonScriptPathSetting { get { return Setting( "CufonScriptPath", "UserControls/Custom/Cccev/Web2/js/cufon-yui.js", false ); } }

		[TextSetting( "Font Script Path", "Path to the *.font.js script. Default 'UserControls/Custom/Cccev/Web2/js/DistrictThin_400.font.js'.", false )]
		public string FontScriptPathSetting { get { return Setting( "FontScriptPath", "UserControls/Custom/Cccev/Web2/js/DistrictThin_400.font.js", false ); } }

		[TextSetting( "Cufon.replace() Chain", "Chain of elements to font replace. Default: ('h1')('h2')('h3')('h4')('h5')('#nav-menu')", false )]
		public string CufonReplaceChainSetting { get { return Setting( "CufonReplaceChain", "('h1')('h2')('h3')('h4')('h5')('#nav-menu')", false ); } }

        [TextSetting("Cufon Bind Event Names", "Space deliminated list of event pool event names. Ex: CAMPUS_UPDATED USER_LOGGED_IN (see http://api.jquery.com/bind/)", false)]
        public string CufonBindEventNamesSetting { get { return Setting("CufonBindEventNames", "", false); } }

		#endregion Module Settings

		#region Event Handlers

		private void Page_Load(object sender, System.EventArgs e)
		{
			smpScripts.Scripts.Add(new ScriptReference(string.Format("~/{0}", CufonScriptPathSetting)));
            smpScripts.Scripts.Add(new ScriptReference(string.Format("~/{0}", FontScriptPathSetting)));

            if (CufonBindEventNamesSetting != string.Empty)
            {
               RegisterClientScripts();
            }
		}
		
		#endregion

        private void RegisterClientScripts()
        {
            StringBuilder sbScript = new StringBuilder();
            sbScript.Append("\n<script type=\"text/javascript\">\n");
            sbScript.Append("function CuFonReplace() {\n");
            sbScript.AppendFormat("   Cufon.replace{1};\n", CufonBindEventNamesSetting, CufonReplaceChainSetting);
            sbScript.Append("}\n");
            sbScript.AppendFormat("$(document).bind('{0}', CuFonReplace );\n", CufonBindEventNamesSetting, CufonReplaceChainSetting);
            sbScript.Append("$(document).ready( CuFonReplace );\n");
            sbScript.Append("</script>\n");
            phCuFonScript.Controls.Add(new LiteralControl(sbScript.ToString()));
        }
	}
}