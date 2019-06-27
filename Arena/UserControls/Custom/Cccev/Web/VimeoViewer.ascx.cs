/**********************************************************************
* Description:	Module based on Jason's BackboneVimeo project.
*				https://github.com/JasonOffutt/BackboneVimeo
* Created By:	Nick Airdo @ Central Christian Church (Arizona)
*				Jason Offutt @ Central Christian Church (Arizona)
* Date Created:	10/26/2011 10:53:35 AM
*
* $Workfile: VimeoViewer.ascx.cs $
* $Revision: 2 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Web/VimeoViewer.ascx.cs   2   2011-11-21 17:16:14-07:00   JasonO $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Web/VimeoViewer.ascx.cs $
*  
*  Revision: 2   Date: 2011-11-22 00:16:14Z   User: JasonO 
*  Functionality tweaks and css tweaks for Facebook login and connect. 
*  
*  Revision: 1   Date: 2011-11-03 21:08:47Z   User: nicka 
*  initial working draft 
**********************************************************************/
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Arena.Portal;

namespace ArenaWeb.UserControls.Custom.Cccev.Web
{
	public partial class VimeoViewer : PortalControl
	{
		#region Module Settings

		[CustomListSetting( "* Instructions", "This module depends on centralaz.media.js, jQuery 1.6, mustache.js, backbone.js, underscore.js.",
			false, "", new string[] { "" }, new string[] { "1" } )]
		public string InstructionsSetting { get { return Setting( "Instructions", "", false ); } }

		[TextSetting( "CSS Path", "Path to a desired .css stylesheet. Default '~/UserControls/Custom/Cccev/Web/css/vimeoviewer.css'", false )]
		public string CSSPathSetting { get { return Setting( "CSSPath", "~/UserControls/Custom/Cccev/Web/css/vimeoviewer.css", false ); } }

		[TextSetting( "Vimeo Account", "Name of your Vimeo account (example: 'centralaz' as seen in http://vimeo.com/centralaz/)", true )]
		public string VimeoAccountSetting { get { return Setting( "VimeoAccount", "", true ); } }

		#endregion

		#region Event Handlers

		private void Page_Load(object sender, EventArgs e)
		{
			BasePage.AddCssLink( Page, CSSPathSetting );

			// jQuery MUST been loaded prior to these next few items..
            BasePage.AddJavascriptInclude(Page, "include/scripts/custom/cccev/lib/mustache.min.js");
            BasePage.AddJavascriptInclude(Page, "include/scripts/custom/cccev/lib/underscore.min.js");
            BasePage.AddJavascriptInclude(Page, "include/scripts/custom/cccev/lib/backbone.min.js");
			BasePage.AddJavascriptInclude( Page, "UserControls/Custom/Cccev/Web/Scripts/centralaz.media.js" );
		}

		#endregion
	}
}