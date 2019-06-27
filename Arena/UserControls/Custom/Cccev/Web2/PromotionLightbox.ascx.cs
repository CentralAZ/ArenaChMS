/**********************************************************************
* Description:	TYPE THE DESCRIPTION OF YOUR MODULE HERE
* Created By:   Nick Airdo @ Central Christian Church of the East Valley
* Date Created:	2/23/2010 5:39:42 PM
*
* $Workfile: PromotionLightbox.ascx.cs $
* $Revision: 9 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Web2/PromotionLightbox.ascx.cs   9   2011-07-21 11:54:09-07:00   nicka $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Web2/PromotionLightbox.ascx.cs $
*  
*  Revision: 9   Date: 2011-07-21 18:54:09Z   User: nicka 
*  Added campus filtering 
*  
*  Revision: 8   Date: 2011-06-16 00:40:13Z   User: nicka 
*  make the heading tag levels configurable as a module setting. 
*  
*  Revision: 7   Date: 2011-06-08 18:09:33Z   User: nicka 
*  Added module setting to enable promotion grouping based on 'path' 
*  information in the promotion's title. 
*  
*  Revision: 6   Date: 2011-05-10 23:50:37Z   User: nicka 
*  added jquery inclusion 
*  
*  Revision: 5   Date: 2011-05-10 17:45:25Z   User: nicka 
*  added link to the ImageSettings document. 
*  
*  Revision: 4   Date: 2011-05-05 19:48:13Z   User: nicka 
*  Added Number of Columns module setting to control the style of the "last" 
*  column item of each row. 
*  
*  Revision: 3   Date: 2010-03-10 02:55:32Z   User: nicka 
*  Moved script inclusion from Page header to scriptmangerproxy 
*  
*  Revision: 2   Date: 2010-03-01 23:40:38Z   User: nicka 
*  remove unused usings 
*  
*  Revision: 1   Date: 2010-03-01 20:39:28Z   User: nicka 
*  final draft 
**********************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Arena.Document;
using Arena.Marketing;
using Arena.Portal;

namespace ArenaWeb.UserControls.Custom.Cccev.Web2
{
	public partial class PromotionLightbox : PortalControl
	{
		#region Module Settings
		[CustomListSetting( "* Instructions", "This module requires jquery and an included UserControls/Custom/Cccev/Web2/js/promotion_lightbox.js library which should have been distributed with this module.",
			false, "", new string[] { "" }, new string[] { "1" } )]
		public string InstructionsSetting { get { return Setting( "Instructions", "", false ); } }

		[CustomListSetting( "Area Filter", "Filter flag for Topic Areas.  If set to 'primary' only items whose Primary Ministry matches the Topic Area will be shown, etc.  Defaults to 'primary'.", false, "primary",
			new[] { "primary", "secondary", "both", "home" }, new[] { "primary", "secondary", "both", "home" } )]
		public string AreaFilterSetting { get { return Setting( "AreaFilter", "primary", false ); } }

		[ListFromSqlSetting( "Topic Areas", "The Topic Areas in which to show promotions", true, "",
			"SELECT l.lookup_id, l.lookup_value FROM core_lookup l INNER JOIN core_lookup_type lt ON lt.lookup_type_id = l.lookup_type_id AND lt.guid = '1FE55E22-F67C-46BA-A6AE-35FD112AFD6D' WHERE active = 1 ORDER BY lookup_order",
			ListSelectionMode.Multiple )]
		public string TopicAreasSetting { get { return Setting( "TopicAreas", "", true ); } }

		[ListFromSqlSetting( "Thumbnail Document Type", "The document type containing the thumbnail to display", false, "",
			"SELECT document_type_id, type_name FROM core_document_type" )]
		public string ThumbnailSetting { get { return Setting( "Thumbnail", "", false ); } }

		[NumericSetting( "Thumbnail Width", "The width of the thumbnail images Default 32.", false )]
		public string WidthSetting { get { return Setting( "Width", "32", false ); } }

		[NumericSetting( "Thumbnail Height", "The height of the thumbnail images. Default 32.", false )]
		public string HeightSetting { get { return Setting( "Height", "32", false ); } }

		private string ImageEffect = string.Empty;
		[TextSetting( "Image Effect", "The image effect to use when displaying a promotion image.  See <a href='http://community.arenachms.com/files/folders/documents/entry176.aspx'>Arena Image Documentation</a> for details. ", false )]
		public string ImageEffectSetting { get { return Setting( "ImageEffect", "", false ); } }

		[TextSetting( "Image Effect Settings", "The image effect settings to use when displaying a promotion image.", false )]
		public string ImageEffectDetailsSetting { get { return Setting( "ImageEffectDetails", "", false ); } }

		[NumericSetting( "Number Columns", "The number of columns to use when aligning items. Default 4.", false )]
		public int NumberColumnsSetting { get { return int.Parse( Setting( "NumberColumns", "4", false ) ); } }

		[NumericSetting( "Maximum Items", "The maximum items to display. Default 16.", false )]
		public string MaxItemsSetting { get { return Setting( "MaxItems", "16", false ); } }

		[NumericSetting( "Priority Level", "Priority level to constrain results by. Default is 99.", false )]
		public string PriorityLevelSetting { get { return Setting( "PriorityLevel", "99", false ); } }

		[BooleanSetting( "Priority Randomized", "Flag indicating whether to randomize the order of events based on their priority (weighted). Default false.", false, false )]
		public bool RandomizedSetting { get { return Convert.ToBoolean( Setting( "Randomized", "false", false ) ); } }

		[TextSetting( "Colorbox js Path", "Path to the jquery.colorbox.js script. Default '~/UserControls/Custom/Cccev/Web2/js/jquery.colorbox.js'.", false )]
		public string ColorboxScriptPathSetting { get { return Setting( "ColorboxScriptPath", "~/UserControls/Custom/Cccev/Web2/js/jquery.colorbox.js", false ); } }

		[TextSetting( "Colorbox CSS Path", "Path to the colorbox.css stylesheet. Default 'UserControls/Custom/Cccev/Web2/css/colorbox.css'.", false )]
		public string ColorboxCSSPathSetting { get { return Setting( "ColorboxCSSPath", "UserControls/Custom/Cccev/Web2/css/colorbox.css", false ); } }

		string _promotionDisplayPageIDSetting;
		[SmartPageSetting( "Promotion Display Page", "The page that should be used to display the promotion summary.", "UserControls/Content/PromotionDisplayDetails.ascx", RelatedModuleLocation.Beneath )]
		public string PromotionDisplayPageIDSetting	{ get { return _promotionDisplayPageIDSetting; } set { _promotionDisplayPageIDSetting = value; } }

		[BooleanSetting( "Enable Title-Path Grouping", "Flag indicating whether to split the promotion titles (on the '/' separator and group them accordingly.). Default false.", false, false )]
		public bool EnableTitlePathSplitSetting { get { return Convert.ToBoolean( Setting( "EnableTitlePathSplit", "false", false ) ); } }
		
		[NumericSetting( "Title-path Grouping Level", "The level to start at when the <h#> tags are used for title path grouping. Default 1.", false )]
		public int TitlePathLevelSetting { get { return int.Parse( Setting( "TitlePathLevel", "1", false ) ); } }

		[ListFromSqlSetting( "Use Configured Campus", "If 'Use Person Campus' is set to false, choosing one or more campuses here will filter promotions to include only those that are tied to these campuses and ones that have no campus specified.", false, "",
	"SELECT campus_id, name FROM orgn_campus ORDER BY name",
	ListSelectionMode.Multiple )]
		public string UseConfigCampusSetting { get { return Setting( "UseConfigCampus", "", false ); } }

		#endregion

		#region Event Handlers

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Add required stylesheets to the page
			Page.Header.Controls.Add( new LiteralControl( string.Format( "<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" href=\"{0}\" />\n", ColorboxCSSPathSetting ) ) );
			Page.Header.Controls.Add( new LiteralControl( "<!--[if IE 7]><style type=\"text/css\">#portfolio-nav{margin:65px 5px 18px 5px;}</style><![endif]-->\n" ) );
			//BasePage.AddJavascriptInclude( Page, BasePage.JQUERY_INCLUDE );

			// Add the required scripts to the page
			smpScripts.Scripts.Add( new ScriptReference( ColorboxScriptPathSetting ) );
			smpScripts.Scripts.Add( new ScriptReference( "~/UserControls/Custom/Cccev/Web2/js/promotion_lightbox.js" ) );

			// Set the image effects if any
			if ( ImageEffectSetting != string.Empty )
				ImageEffect = "&effect=" + ImageEffectSetting;

			if ( ImageEffectDetailsSetting != string.Empty )
				ImageEffect += "&effectSettings=" + ImageEffectDetailsSetting;
		}
		
		#endregion

		protected IEnumerable<PromotionRequest> GetPromotionRequests()
		{
			PromotionRequestCollection prc = new PromotionRequestCollection();
			prc.LoadCurrentWebRequests( TopicAreasSetting, AreaFilterSetting, -1, int.Parse( MaxItemsSetting ), false, -1 );
			Random randGen = new Random();

			var promotions = ( from p in prc.OfType<PromotionRequest>()
							   where p.Priority <= int.Parse( PriorityLevelSetting ) + 1
							   select p ).Select( p => new
							   {
								   Promotion = p,
								   Index = ( RandomizedSetting ) ? randGen.NextDouble() * p.Priority : p.Priority
							   } ).OrderBy( p => p.Index )
							  .Select( p => p.Promotion );

			// filter by campuses if UseConfigCampusSetting is not empty (feature for rev 13)
			if ( !string.IsNullOrEmpty( UseConfigCampusSetting ) )
			{
				var campuses = UseConfigCampusSetting.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries ).ToList();
				promotions = ( from p in promotions where p.Campus == null || p.Campus.CampusId == -1 || campuses.Contains( p.Campus.CampusId.ToString() ) select p );
			}

			return promotions.ToList();
		}

		protected string GetImageUrl( PromotionRequest promotion )
		{
			string guid = string.Empty;
			if ( ThumbnailSetting != string.Empty )
			{
				PromotionRequestDocument doc = promotion.Documents.GetFirstByType( int.Parse( ThumbnailSetting ) );
				if ( doc != null )
				{
					guid = doc.GUID.ToString();
				}
			}
			else
			{
				guid = promotion.WebSummaryImageBlob.GUID.ToString();
			}

			return String.Format( "CachedBlob.aspx?guid={0}&width={1}&height={2}{3}", guid, WidthSetting, HeightSetting, ImageEffect );
		}

		protected string GetDetailsUrl( PromotionRequest promotion )
		{
			if ( promotion.WebExternalLink != string.Empty )
			{
				return promotion.WebExternalLink;
			}
			else
			{
				return String.Format( "Default.aspx?page={0}&promotionId={1}", PromotionDisplayPageIDSetting, promotion.PromotionRequestID );
			}
		}
	}
}