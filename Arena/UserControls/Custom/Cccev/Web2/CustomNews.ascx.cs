/**********************************************************************
* Description:	Custom News from Promotions
* Created By:	Nick Airdo @ Central Christian Church of the East Valley
* Date Created:	5/5/2010 2:01:03 PM
*
* $Workfile: CustomNews.ascx.cs $
* $Revision: 6 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Web2/CustomNews.ascx.cs   6   2010-08-17 11:54:59-07:00   nicka $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Web2/CustomNews.ascx.cs $
*  
*  Revision: 6   Date: 2010-08-17 18:54:59Z   User: nicka 
*  
*  Revision: 5   Date: 2010-08-02 23:33:42Z   User: nicka 
*  Added support for passing an event details page ID. 
*  
*  Revision: 4   Date: 2010-06-01 17:11:43Z   User: JasonO 
*  Updating custom news to latest 
*  
*  Revision: 3   Date: 2010-05-26 21:24:16Z   User: JasonO 
*  Adding support for setting preferred news topics 
*  
*  Revision: 2   Date: 2010-05-25 17:52:06Z   User: JasonO 
*  Adding stub elements/functionality for custom topics 
*  
*  Revision: 1   Date: 2010-05-25 00:24:52Z   User: nicka 
*  Working DRAFT of custom news and events; binds to these jQuery events: 
*  CAMPUS_UPDATED, CAMPUS_UPDATING, TOPICS_UPDATED, TOPICS_UPDATING and 
*  USER_LOGGED_IN 
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
	public partial class CustomNews : PortalControl
	{
		#region Module Settings
		string _eventDisplayPageIDSetting;
		[SmartPageSetting("Event Display Page", "The page that should be used to display the event details.", "UserControls/Content/eventdetails.ascx", RelatedModuleLocation.Beneath)]
		public string EventDisplayPageIDSetting { get { return _eventDisplayPageIDSetting; } set { _eventDisplayPageIDSetting = value; } }

		string _promotionDisplayPageIDSetting;
		[SmartPageSetting( "Promotion Display Page", "The page that should be used to display the promotion details.", "UserControls/Content/PromotionDisplayDetails.ascx", RelatedModuleLocation.Beneath )]
		public string PromotionDisplayPageIDSetting { get { return _promotionDisplayPageIDSetting; } set { _promotionDisplayPageIDSetting = value; } }

		[ListFromSqlSetting("Default CampusID", "The default CampusID in which to show promotions", false, "-1",
		"SELECT l.lookup_id, l.lookup_value FROM core_lookup l INNER JOIN core_lookup_type lt ON lt.lookup_type_id = l.lookup_type_id AND lt.guid = 'FB0AC12E-630C-4792-BF9E-32442D7FEA62' WHERE active = 1 ORDER BY lookup_order",
		ListSelectionMode.Single)]
		public string DefaultCampusIDSetting { get { return Setting("DefaultCampusID", "-1", false); } }

		[ListFromSqlSetting("Default Topic Areas", "The default Topic Areas in which to show promotions", false, "",
		"SELECT l.lookup_id, l.lookup_value FROM core_lookup l INNER JOIN core_lookup_type lt ON lt.lookup_type_id = l.lookup_type_id AND lt.guid = '1FE55E22-F67C-46BA-A6AE-35FD112AFD6D' WHERE active = 1 ORDER BY lookup_order",
		ListSelectionMode.Multiple)]
		public string DefaultTopicAreasSetting { get { return Setting("DefaultTopicAreas", "", false); } }

		#endregion

		#region Event Handlers

		private void Page_Load(object sender, EventArgs e)
		{
			InsertTemplates();
		}

		/// <summary>
		/// Responsible for injecting the jQuery templates into the header.
		/// </summary>
		private void InsertTemplates()
		{
			if (!Page.Header.Controls.OfType<LiteralControl>().Any(c => c.Text.Contains("id=\"news-template\"")))
			{
				string template = @"
<script type=""text/html"" id=""news-template"">
<li class=""item"">
	<h3><a href=""{%= detailsUrl %}"">{%= title %}</a></h3>
	<h4>{%= topic %}</h4>
	{%= summary %}
</li>
</script>";
				Page.Header.Controls.Add(new LiteralControl(template));
			}
		}

		#endregion

		protected IEnumerable<Lookup> GetAllTopics()
		{
			bool isPublic;
			var topics = new LookupType(SystemLookupType.TopicArea);
			return topics.Values.Where(t => bool.TryParse(t.Qualifier2, out isPublic) && t.Active).OrderBy(t => t.Order);
		}

		protected IEnumerable<Lookup> GetTopicPreferences()
		{
			IEnumerable<int> lookupIDs = null;

			if (Request.IsAuthenticated)
			{
				var attribute = new Arena.Core.Attribute(SystemGuids.WEB_PREFS_NEWS_TOPICS_ATTRIBUTE);
				var pa = new PersonAttribute(CurrentPerson.PersonID, attribute.AttributeId);

				if (!string.IsNullOrEmpty(pa.StringValue))
				{
					lookupIDs = pa.StringValue.SplitAndConvertTo<int>(new[] { ',' }, Convert.ToInt32);
				}
			}
			else
			{
				var cookie = Request.Cookies["Cccev.Web.Settings"] != null
								 ? Request.Cookies["Cccev.Web.Settings"].Value
								 : Constants.NULL_STRING;

				var array = Server.UrlDecode(cookie).Split(new[] { "|||" }, StringSplitOptions.None);

				if (!string.IsNullOrEmpty(array.Last().Trim()))
				{
					lookupIDs = array.Last().SplitAndConvertTo<int>(new[] { ',' }, Convert.ToInt32);
				}
			}

			return lookupIDs != null ? (from s in lookupIDs select new Lookup(s)).ToList() : new List<Lookup>();
		}
	}
}