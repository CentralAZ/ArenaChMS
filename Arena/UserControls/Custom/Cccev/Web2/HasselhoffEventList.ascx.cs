/**********************************************************************
* Description:  Displays a list of events based on the Calendar Control Panel
* Created By:	Nick Airdo @ Central Christian Church of the East Valley
* Date Created:	7/12/2010 12:59:59 PM
*
* $Workfile: HasselhoffEventList.ascx.cs $
* $Revision: 2 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Web2/HasselhoffEventList.ascx.cs   2   2010-07-26 08:36:40-07:00   nicka $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Web2/HasselhoffEventList.ascx.cs $
*  
*  Revision: 2   Date: 2010-07-26 15:36:40Z   User: nicka 
*  Added fadeIn/fadeOut support for List view. 
*  
*  Revision: 1   Date: 2010-07-22 23:38:11Z   User: nicka 
*  Added support for the Event List View module 
**********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Arena.Core;
using Arena.Custom.Cccev.DataUtils;
using Arena.Custom.Cccev.FrameworkUtils.FrameworkConstants;
using Arena.Portal;

namespace ArenaWeb.UserControls.Custom.Cccev.Web2
{
	public partial class HasselhoffEventList : PortalControl
	{
		#region Module Settings

		[TextSetting( "CSS Path", "Path to the stylesheet. Default '~/UserControls/Custom/Cccev/Web2/css/calendar.css'.", false )]
		public string CSSPathSetting { get { return Setting( "CSSPath", "~/UserControls/Custom/Cccev/Web2/css/calendar.css", false ); } }

		#endregion

		#region Event Handlers

		private void Page_Load(object sender, EventArgs e)
		{
			BasePage.AddCssLink( Page, CSSPathSetting );
			InsertTemplates();
		}

		/// <summary>
		/// Responsible for injecting the jQuery templates into the header.
		/// </summary>
		private void InsertTemplates()
		{
			if ( !Page.Header.Controls.OfType<LiteralControl>().Any( c => c.Text.Contains( "id=\"event-featured-list-template\"" ) ) )
			{
				string template = @"
<script type=""text/html"" id=""event-featured-list-template"">
<li class=""item"">
	<div class=""date"">{%= date %}<div class=""month"">{%= month %}</div></div>
	<div class=""photo""><a href=""{%= url %}""><img src=""{%= imageUrl %}"" /></a></div>
	<h3><a href=""{%= url %}"">{%= title %}</a></h3>
</li>
</script>

<script type=""text/html"" id=""event-list-template"">
<li class=""item"">
	<div class=""date"">{%= date %}<div class=""month"">{%= month %}</div></div>
	<h3><a href=""{%= url %}"">{%= title %}</a></h3>
	<h4>{%= title %}</h4>
	<p class=""summary"">{%= description %}</p>
</li>
</script>";
				Page.Header.Controls.Add(new LiteralControl(template));
			}
		}

		#endregion
	}
}