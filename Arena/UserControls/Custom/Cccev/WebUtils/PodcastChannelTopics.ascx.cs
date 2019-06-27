/**********************************************************************
* Description:	Shows series and messages within a podcast channel
* Created By:   DallonF 
* Date Created:	3/23/2009 4:29:45 PM
*
* $Workfile: PodcastChannelTopics.ascx.cs $
* $Revision: 10 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/PodcastChannelTopics.ascx.cs   10   2011-05-20 16:24:30-07:00   nicka $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/PodcastChannelTopics.ascx.cs $
*  
*  Revision: 10   Date: 2011-05-20 23:24:30Z   User: nicka 
*  Fixed a topic sorting bug, added ability to include link to play latest 
*  item in Channel Viewer, and added some basic caching. 
*  
*  Revision: 9   Date: 2010-12-08 22:46:46Z   User: nicka 
*  Corrected the sort order of the topics and use the Arena utility to change 
*  CRLF to BR. 
*  
*  Revision: 8   Date: 2010-02-17 16:52:47Z   User: JasonO 
*  Fixing jquery include issues. 
*  
*  Revision: 7   Date: 2010-01-27 22:49:28Z   User: JasonO 
*  Cleaning up. 
*  
*  Revision: 6   Date: 2009-11-16 21:31:52Z   User: JasonO 
*  Adding ability to sort topic items and display unpublished items. 
*  
*  Revision: 5   Date: 2009-04-16 18:37:15Z   User: JasonO 
*  Correcting the topic sorting order and removing un-needed dependencies. 
*  
*  Revision: 4   Date: 2009-03-25 20:59:18Z   User: DallonF 
*  Added "Back To Channels Page" link 
*  
*  Revision: 3   Date: 2009-03-24 21:27:33Z   User: JasonO 
*  
*  Revision: 1   Date: 2009-03-24 01:05:37Z   User: DallonF 
*  First version 
**********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Arena.Custom.Cccev.DataUtils;
using Arena.Portal;
using Arena.Feed;

namespace ArenaWeb.UserControls.Custom.Cccev.WebUtils
{
    public partial class PodcastChannelTopics : PortalControl
    {
        protected Channel CurrentChannel;

        [PageSetting("Player Page", "The page that contains a media player", true)]
        public string PlayerPageSetting { get { return Setting("PlayerPage", "-1", true); } }

        [PageSetting("Channels Page", "The page that contains the Channel Viewer", true)]
        public string ChannelsPageSetting { get { return Setting("ChannelsPage", "-1", true); } }

        [BooleanSetting("Show Unpublished Topic Items", "Show unpublished podcast topic items? (Defaults to true).", false, true)]
        public string ShowUnpublishedTopicItemsSetting { get { return Setting("ShowUnpublishedTopicItems", "true", false); } }

        [CustomListSetting("Topic Item Sort Order", "Order podcast topic items should be sorted in.", false, "", 
            new[] { "Ascending", "Descending" }, new[] { "asc", "desc"})]
        public string TopicItemSortOrderSetting { get { return Setting("TopicItemSortOrder", "", false); } }

        private const string DESCENDING = "desc";

        #region Event Handlers

        private void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
				BasePage.AddJavascriptInclude( Page, BasePage.JQUERY_INCLUDE );
                smpScripts.Scripts.Add(new ScriptReference("~/Templates/Cccev/liger/js/jquery.galleryScroll.1.4.5.pack.js"));
                smpScripts.Scripts.Add(new ScriptReference("~/Templates/Cccev/liger/js/main.js"));
            }

            int channelId = int.Parse(Request.QueryString["channel"]);

            CurrentChannel = new Channel(channelId);

            if (channelId == Constants.NULL_INT || CurrentChannel.ChannelId == Constants.NULL_INT)
            {
                throw new ArgumentException("Channel ID is invalid!");
            }
        }

        #endregion

        public IEnumerable<Topic> GetSeries()
        {
            //return CurrentChannel.Topics.Where(t => t.Active).OrderByDescending(t => t.Items.First().PublishDate);
			// Changed to sort by the latest (desc), active, item in the topic items list:
			return CurrentChannel.Topics.Where( t => t.Active ).OrderByDescending( 
				t => t.Items.Where( i => i.Active ).OrderByDescending( 
						i => i.PublishDate ).First().PublishDate
					);
        }

        public IEnumerable<Item> GetMessages(Topic series)
        {
            var items = series.Items.Where(t => t.Active);

            if (TopicItemSortOrderSetting == DESCENDING)
            {
                return items.OrderByDescending(i => i.PublishDate);
            }

            return items.OrderBy(i => i.PublishDate);
        }

        public string GetImage(Topic series)
        {
            return String.Format("CachedBlob.aspx?guid={0}", series.Image.GUID);
        }

        public string GetPlayerLink(Item message)
        {
            return String.Format("Default.aspx?page={0}&item={1}", PlayerPageSetting, message.ItemId);
        }

    }
}