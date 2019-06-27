/**********************************************************************
* Description:	Promotions via WebServices via jquery-arena-promotions.js
* Created By:	Nick Airdo @ Central Christian Church of the East Valley
* Date Created:	8/19/2010 10:38:03 AM
*
* $Workfile: PromotionsViaWS.ascx.cs $
* $Revision: 2 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Web2/PromotionsViaWS.ascx.cs   2   2010-08-30 16:37:05-07:00   nicka $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Web2/PromotionsViaWS.ascx.cs $
*  
*  Revision: 2   Date: 2010-08-30 23:37:05Z   User: nicka 
*  Total rewrite to work 100% via client ajax webservice calls. 
*  
*  Revision: 1   Date: 2010-08-26 21:45:25Z   User: nicka 
*  working draft but still has hardcoded IDs in it. 
*  
*  Working DRAFT of custom news and events; binds to these jQuery events: 
*  CAMPUS_UPDATED, CAMPUS_UPDATING, and USER_LOGGED_IN 
**********************************************************************/
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Arena.Portal;

namespace ArenaWeb.UserControls.Custom.Cccev.Web2
{
	public partial class PromotionsViaWS : PortalControl
	{
		#region Module Settings

		[CustomListSetting( "* Instructions", "Put your jquery template script (type=text/html) in the module details at the very bottom and put your placeholder markup into the Placeholder field.  The Template Script (module details) will be inserted into the document head.  See <a href='http://ejohn.org/blog/javascript-micro-templating/'>this for info on jQuery Templating</a>.",
			false, "", new string[] { "" }, new string[] { "1" } )]
		public string InstructionsSetting { get { return Setting( "Instructions", "", false ); } }

		[CustomListSetting( "Area Filter", "Filter flag for Topic Areas.  If set to 'primary' only items whose Primary Ministry matches the Topic Area will be shown, etc.  Defaults to 'primary'.", false, "primary",
			new[] { "primary", "secondary", "both", "home" }, new[] { "primary", "secondary", "both", "home" } )]
		public string AreaFilterSetting { get { return Setting( "AreaFilter", "primary", false ); } }

		[ListFromSqlSetting( "Default Topic Areas", "The Topic Areas in which to show promotions.", true, "",
			"SELECT l.lookup_id, l.lookup_value FROM core_lookup l INNER JOIN core_lookup_type lt ON lt.lookup_type_id = l.lookup_type_id AND lt.guid = '1FE55E22-F67C-46BA-A6AE-35FD112AFD6D' WHERE active = 1 ORDER BY lookup_order",
			ListSelectionMode.Multiple )]
		public string TopicAreasSetting { get { return Setting( "TopicAreas", "", true ); } }

		[ListFromSqlSetting( "Thumbnail Document Type", "The document type of the thumbnail to display. Use only to override the default web promotion image", false, "-1",
			"SELECT document_type_id, type_name FROM core_document_type" )]
		public string ThumbnailSetting { get { return Setting( "Thumbnail", "-1", false ); } }

		[TextSetting( "Template ID", "ID to use for the jQuery templating.  Ex: #user_tmpl. See <a href='http://ejohn.org/blog/javascript-micro-templating/'>http://ejohn.org/blog/javascript-micro-templating/</a>", true )]
		public string TemplateIDSetting { get { return Setting( "TemplateID", "", true ); } }

		[TextSetting( "HTML Placeholder HTML", "This is the HTML markup which includes a spot where the template data will be injected.", true )]
		public string PlaceholderHTMLSetting { get { return Setting( "PlaceholderHTML", "", true ); } }

		[TextSetting( "HTML Container ID", "The ID of the container element in your HTML markup to hide if there are no promotions to display.", false )]
		public string ContainerIDSetting { get { return Setting( "ContainerID", "", false ); } }

		[TextSetting( "HTML Placeholder ID", "The ID of exact element in your HTML markup where the template is to be injected.", true )]
		public string PlaceholderIDSetting { get { return Setting( "PlaceholderID", "", true ); } }

		[TextSetting( "js Path", "Path to any needed .js script or plugin you use to animate/manipulate the final markup. Ex: ~/Templates/Custom/blah/blah.js", false )]
		public string ScriptPathSetting { get { return Setting( "ScriptPath", "", false ); } }

		[TextSetting( "On Success Callback", "A javascript callback which will be called after the promotions are fetched and rendered. It takes one argument of result (the return results of the ajax ws call).  You can use this to perform some action after the results rendered such as invoke another plugin like featureShow to animate the promotions.", false )]
		public string OnSuccessCallbackSetting { get { return Setting( "OnSuccessCallback", "defaultOnSuccessCallback", false ); } }

		[NumericSetting( "Maximum Items", "The maximum items to display. Default 16.", false )]
		public string MaxItemsSetting { get { return Setting( "MaxItems", "16", false ); } }

		[NumericSetting( "Priority Level", "Priority level to constrain results by. Default is 99.", false )]
		public string PriorityLevelSetting { get { return Setting( "PriorityLevel", "99", false ); } }

		[BooleanSetting( "Update On Campus Change", "Flag indicating if the promotions should be reloaded after a campus change event (CAMPUS_UPDATED). Default true.", true, true )]
		public bool UpdateOnCampusChangeSetting { get { return Convert.ToBoolean( Setting( "UpdateOnCampusChange", "true", true ) ); } }

		[PageSetting( "Event Detail Page", "The page that should be used to display event details.", true )]
		public string EventDetailPageSetting { get { return Setting( "EventDetailPage", "", true ); } }

		[PageSetting( "Promotion Display Page", "The page that should be used to display the promotion summary.", true )]
		public string PromotionDisplayPageIDSetting { get { return Setting( "PromotionDisplayPageID", "", true ); } }

		[ListFromSqlSetting( "Default CampusID", "The default CampusID in which to show promotions", false, "-1",
			"SELECT l.lookup_id, l.lookup_value FROM core_lookup l INNER JOIN core_lookup_type lt ON lt.lookup_type_id = l.lookup_type_id AND lt.guid = 'FB0AC12E-630C-4792-BF9E-32442D7FEA62' WHERE active = 1 ORDER BY lookup_order",
			ListSelectionMode.Single )]
		public string DefaultCampusIDSetting { get { return Setting( "DefaultCampusID", "-1", false ); } }

		#endregion

		#region Event Handlers

		private void Page_Load(object sender, EventArgs e)
		{
			InsertTemplates();

			if ( ScriptPathSetting != string.Empty )
			{
				smpScripts.Scripts.Add( new ScriptReference( ScriptPathSetting ) );
				smpScripts.Scripts.Add( new ScriptReference( string.Format( "~/{0}", "UserControls/Custom/Cccev/Web2/js/jquery-arena-promotions.js" ) ) );
			}

			phContent.Controls.Add( new LiteralControl(	PlaceholderHTMLSetting ) );
		}

		/// <summary>
		/// Responsible for injecting the jQuery templates into the header.
		/// </summary>
		private void InsertTemplates()
		{
			if ( ! Page.Header.Controls.OfType<LiteralControl>().Any( c => c.Text.Contains( TemplateIDSetting ) ) )
			{
				Page.Header.Controls.Add( new LiteralControl( CurrentModule.Details ) );
			}
		}

		#endregion
	}
}