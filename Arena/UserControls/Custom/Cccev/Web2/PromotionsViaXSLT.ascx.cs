/**********************************************************************
* Description:	Displays Promotions via the configured XSLT
* Created By:   Nick Airdo @ Central Christian Church of the East Valley
* Date Created:	3/2/2010 7:39:22 PM
*
* $Workfile: PromotionsViaXSLT.ascx.cs $
* $Revision: 13 $ 
* $Header: 
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Web2/PromotionsViaXSLT.ascx.cs $
*  
*  Revision: 13   Date: 2011-07-08 15:56:46Z   User: nicka 
*  Added ability to filter by selected campus(es) and also added the 
*  promotions "details" in the item XML. 
*  
*  Revision: 12   Date: 2010-10-18 18:13:58Z   User: nicka 
*  Return empty string if there is no image associated with the promotion 
*  (from Jon E). 
*  
*  Revision: 11   Date: 2010-10-04 18:40:17Z   User: nicka 
*  Bug fix: cache key now uses campus value 
*  
*  Revision: 10   Date: 2010-10-04 17:50:13Z   User: nicka 
*  Added link to Arena Image Documentation. 
*  
*  Revision: 9   Date: 2010-08-02 23:33:42Z   User: nicka 
*  Added support for passing an event details page ID. 
*  
*  Revision: 8   Date: 2010-07-02 04:04:14Z   User: nicka 
*  Fixed the logic bug I just introduced a few hours ago - argh!!! 
*  
*  Revision: 7   Date: 2010-07-01 22:22:51Z   User: nicka 
*  Tweak logic a bit (for clarity) 
*  
*  Revision: 6   Date: 2010-05-07 01:46:23Z   User: nicka 
*  Added person campus and events only flag capability. 
*  
*  Revision: 5   Date: 2010-04-24 14:50:33Z   User: nicka 
*  bug fix with script includes 
*  
*  Revision: 4   Date: 2010-04-14 16:19:02Z   User: nicka 
*  Bug fix.  Cache key did not include Module Instance ID so there was cache 
*  interference. 
*  
*  Revision: 3   Date: 2010-03-10 01:52:21Z   User: nicka 
*  Now uses promotion's External Link if it has one. 
*  
*  Revision: 2   Date: 2010-03-04 16:22:58Z   User: nicka 
*  added caching 
*  
*  Revision: 1   Date: 2010-03-04 15:53:02Z   User: nicka 
*  initial draft 
**********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Arena.Document;
using Arena.Marketing;
using Arena.Portal;
using System.Xml;
using System.Web.Caching;
using Arena.Core;

namespace ArenaWeb.UserControls.Custom.Cccev.Web2
{
	public partial class PromotionsViaXSLT : PortalControl
	{
		#region Module Settings
		[CustomListSetting( "Area Filter", "Filter flag for Topic Areas.  If set to 'primary' only items whose Primary Ministry matches the Topic Area will be shown, etc.  Defaults to 'primary'.", false, "primary",
			new[] { "primary", "secondary", "both", "home" }, new[] { "primary", "secondary", "both", "home" } )]
		public string AreaFilterSetting { get { return Setting( "AreaFilter", "primary", false ); } }

		[ListFromSqlSetting( "Topic Areas", "The Topic Areas in which to show promotions", true, "",
			"SELECT l.lookup_id, l.lookup_value FROM core_lookup l INNER JOIN core_lookup_type lt ON lt.lookup_type_id = l.lookup_type_id AND lt.guid = '1FE55E22-F67C-46BA-A6AE-35FD112AFD6D' WHERE active = 1 ORDER BY lookup_order",
			ListSelectionMode.Multiple )]
		public string TopicAreasSetting { get { return Setting( "TopicAreas", "", true ); } }

		[ListFromSqlSetting( "Thumbnail Document Type", "The document type containing the thumbnail to display. Use only to override the default web promotion image", false, "",
			"SELECT document_type_id, type_name FROM core_document_type" )]
		public string ThumbnailSetting { get { return Setting( "Thumbnail", "", false ); } }

		[NumericSetting( "Thumbnail Width", "The width of the thumbnail images Default 32.", false )]
		public string WidthSetting { get { return Setting( "Width", "32", false ); } }

		[NumericSetting( "Thumbnail Height", "The height of the thumbnail images. Default 32.", false )]
		public string HeightSetting { get { return Setting( "Height", "32", false ); } }

		[NumericSetting( "Maximum Items", "The maximum items to display. Default 16.", false )]
		public string MaxItemsSetting { get { return Setting( "MaxItems", "16", false ); } }

		[NumericSetting( "Priority Level", "Priority level to constrain results by. Default is 99.", false )]
		public string PriorityLevelSetting { get { return Setting( "PriorityLevel", "99", false ); } }

		[BooleanSetting( "Priority Randomized", "Flag indicating whether to randomize the order of events based on their priority (weighted). Default false.", false, false )]
		public bool RandomizedSetting { get { return Convert.ToBoolean( Setting( "Randomized", "false", false ) ); } }

		[TextSetting( "js Path", "Path to the any needed .js script such as ~/Templates/Custom/blah/blah.js", false )]
		public string ScriptPathSetting { get { return Setting( "ScriptPath", "", false ); } }

		[TextSetting( "CSS Path", "Path to the any desired .css stylesheet.", false )]
		public string CSSPathSetting { get { return Setting( "CSSPath", "", false ); } }

		string _promotionDisplayPageIDSetting;
		[SmartPageSetting( "Promotion Display Page", "The page that should be used to display the promotion summary.", "UserControls/Content/PromotionDisplayDetails.ascx", RelatedModuleLocation.Beneath )]
		public string PromotionDisplayPageIDSetting	{ get { return _promotionDisplayPageIDSetting; } set { _promotionDisplayPageIDSetting = value; } }

		[PageSetting( "Event Detail Page", "The page that should be used to display event details. Default 4222.", false, 4222)]
		public string EventDetailPageSetting { get { return Setting( "EventDetailPage", "4222", false ); } }

		[TextSetting( "XsltUrl", "The path to the XSLT file to use. Default '~/UserControls/Custom/Cccev/Web2/xslt/simple_promotions.xslt')", false )]
		public string XsltUrlSetting { get { return Setting( "XsltUrl", "~/UserControls/Custom/Cccev/Web2/xslt/simple_promotions.xslt", false ); } }

		private string ImageEffect = string.Empty;
		[TextSetting( "Image Effect", "The image effect to use when displaying a promotion image.  See <a href='http://community.arenachms.com/files/folders/documents/entry176.aspx'>Arena Image Documentation</a> for details. ", false )]
		public string ImageEffectSetting { get { return Setting( "ImageEffect", "", false ); } }

		[TextSetting( "Image Effect Settings", "The image effect settings to use when displaying a promotion image.", false )]
		public string ImageEffectDetailsSetting { get { return Setting( "ImageEffectDetails", "", false ); } }

		[NumericSetting( "Time to Cache (minutes)", "Time in minutes to cache the contents.  Default 3.", false )]
		public string TimeCacheMinutesSetting { get { return Setting( "TimeCacheMinutes", "3", false ); } }

		[BooleanSetting( "Events Only", "Flag indicating if only promotions tied to an event should be displayed. Default false.", false, false )]
		public bool EventsOnlySetting { get { return Convert.ToBoolean( Setting( "EventsOnly", "false", false ) ); } }

		[BooleanSetting( "Use Person's Campus", "Flag indicating whether to use the authenticated person's campus value if available. Default false.", false, false )]
		public bool UsePersonCampusSetting { get { return Convert.ToBoolean( Setting( "UsePersonCampus", "false", false ) ); } }

		[ListFromSqlSetting( "Use Configured Campus", "If 'Use Person Campus' is set to false, choosing one or more campuses here will filter promotions to include only those that are tied to these campuses and ones that have no campus specified.", false, "",
			"SELECT campus_id, name FROM orgn_campus ORDER BY name",
			ListSelectionMode.Multiple )]
		public string UseConfigCampusSetting { get { return Setting( "UseConfigCampus", "", false ); } }

		#endregion

		#region Event Handlers

		protected void Page_Load( object sender, EventArgs e )
		{
			// Add required stylesheet and script to the page
			if ( CSSPathSetting != string.Empty )
			{
				Page.Header.Controls.Add( new LiteralControl( string.Format( "<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" href=\"{0}\" />\n", CSSPathSetting ) ) );
			}

			if ( ScriptPathSetting != string.Empty )
			{
				smpScripts.Scripts.Add( new ScriptReference( ScriptPathSetting ) );
			}

			// Set the image effects if any
			if ( ImageEffectSetting != string.Empty )
				ImageEffect = "&effect=" + ImageEffectSetting;

			if ( ImageEffectDetailsSetting != string.Empty )
				ImageEffect += "&effectSettings=" + ImageEffectDetailsSetting;

			xmlTransform.Document = BuildXMLForPromotions();
			xmlTransform.XslFileURL = XsltUrlSetting;
		}

		#endregion

		private XmlDocument BuildXMLForPromotions()
		{
			// Create an empty XML doc to hold our xml output
			XmlDocument document = null;
			
			string campuses = "-1";
			if ( UsePersonCampusSetting && ArenaContext.Current.Person != null )
			{
				campuses = ArenaContext.Current.Person.Campus.CampusId.ToString();
			}
			else if ( !string.IsNullOrEmpty( UseConfigCampusSetting ) )
			{
				campuses = UseConfigCampusSetting;
			}

			string cacheKey = String.Format( "Cccev.Web2.PromotionsViaXSLT.Page:{0}:Module:{1}:Campuses:{2}", CurrentPortalPage.PortalPageID, CurrentModule.ModuleInstanceID, campuses );

			// get from the cache if available and if refreshcache is not present
			if ( Request.QueryString["refreshcache"] == null && Cache[cacheKey] != null )
			{
				document = (XmlDocument)Cache[ cacheKey ];
			}

			if ( document == null )
			{
				document = new XmlDocument();
				XmlNode rootNode = document.CreateNode( XmlNodeType.Element, "arenapromotions", document.NamespaceURI );
				document.AppendChild( rootNode );

				// Build the node for the channel
				XmlNode containerNode = document.CreateNode( XmlNodeType.Element, "container", document.NamespaceURI );
				rootNode.AppendChild( containerNode );

				foreach ( PromotionRequest item in GetPromotionRequests() )
				{
					XmlNode itemNode = document.CreateNode( XmlNodeType.Element, "item", document.NamespaceURI );
					containerNode.AppendChild( itemNode );

					XmlAttribute itemAttrib = document.CreateAttribute( "tmp" );

					SetNodeAttribute( document, itemNode, itemAttrib, "id", item.PromotionRequestID.ToString() );
					SetNodeAttribute( document, itemNode, itemAttrib, "topic", item.TopicArea.Value );
					SetNodeAttribute( document, itemNode, itemAttrib, "title", item.Title );
					SetNodeAttribute( document, itemNode, itemAttrib, "summary", item.WebSummary );
					SetNodeAttribute( document, itemNode, itemAttrib, "details", item.WebText );
					SetNodeAttribute( document, itemNode, itemAttrib, "imageUrl", GetImageUrl( item ) );
					SetNodeAttribute( document, itemNode, itemAttrib, "detailsUrl", GetDetailsUrl( item ) );
				}

				Cache.Add( cacheKey, document, null, DateTime.Now.AddMinutes( int.Parse( TimeCacheMinutesSetting ) ), Cache.NoSlidingExpiration, CacheItemPriority.High, null );
			}

			return document;
		}

		private void SetNodeAttribute( XmlDocument document, XmlNode node, XmlAttribute attrib, string attribName, string attribValue )
		{
			attrib = document.CreateAttribute( attribName );
			attrib.Value = attribValue;
			node.Attributes.Append( attrib );
		}

		protected IEnumerable<PromotionRequest> GetPromotionRequests()
		{
			int campusID = -1;
			if ( UsePersonCampusSetting && ArenaContext.Current.Person != null )
			{
				campusID = ArenaContext.Current.Person.Campus.CampusId;
			}

			PromotionRequestCollection prc = new PromotionRequestCollection();
			prc.LoadCurrentWebRequests( TopicAreasSetting, AreaFilterSetting, campusID, int.Parse( MaxItemsSetting ), EventsOnlySetting, -1 );
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
			if ( ! string.IsNullOrEmpty( UseConfigCampusSetting ) )
			{
				var campuses = UseConfigCampusSetting.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries ).ToList();
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

			if ( guid != string.Empty )
			{
				return string.Format( "CachedBlob.aspx?guid={0}&width={1}&height={2}{3}", guid, WidthSetting, HeightSetting, ImageEffect );
			}
			else
			{
				return string.Empty;
			}
		}

		protected string GetDetailsUrl( PromotionRequest promotion )
		{
			if ( promotion.WebExternalLink != string.Empty )
			{
				return promotion.WebExternalLink;
			}
			else if ( promotion.EventID != -1 )
			{
				return string.Format( "default.aspx?page={0}&eventId={1}", EventDetailPageSetting, promotion.EventID ); 
			}
			else
			{
				return String.Format( "default.aspx?page={0}&promotionId={1}", PromotionDisplayPageIDSetting, promotion.PromotionRequestID );
			}
		}
	}
}