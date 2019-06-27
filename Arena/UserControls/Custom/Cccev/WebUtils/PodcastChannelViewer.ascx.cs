/**********************************************************************
* Description:	Displays Podcast Channels.
* Created By:	DallonF
* Date Created:	3/18/2009 5:11:53 PM
*
* $Workfile: PodcastChannelViewer.ascx.cs $
* $Revision: 9 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/PodcastChannelViewer.ascx.cs   9   2011-05-20 16:24:30-07:00   nicka $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/PodcastChannelViewer.ascx.cs $
*  
*  Revision: 9   Date: 2011-05-20 23:24:30Z   User: nicka 
*  Fixed a topic sorting bug, added ability to include link to play latest 
*  item in Channel Viewer, and added some basic caching. 
*  
*  Revision: 8   Date: 2010-11-24 22:55:15Z   User: nicka 
*  Added Content Category module setting and now filters using that setting. 
*  
*  Revision: 7   Date: 2010-06-23 23:39:51Z   User: nicka 
*  Changed tabslide js script path to be a module setting. 
*  
*  Revision: 6   Date: 2010-02-17 16:52:47Z   User: JasonO 
*  Fixing jquery include issues. 
*  
*  Revision: 5   Date: 2010-01-27 22:49:28Z   User: JasonO 
*  Cleaning up. 
*  
*  Revision: 4   Date: 2009-04-16 18:48:36Z   User: JasonO 
*  Removing un-needed dependencies. 
*  
*  Revision: 3   Date: 2009-03-25 20:05:15Z   User: DallonF 
*  Fixed &amp; issue 
*  
*  Revision: 2   Date: 2009-03-25 00:16:01Z   User: JasonO 
*  adding comments 
*  
*  Revision: 1   Date: 2009-03-23 23:28:24Z   User: DallonF 
**********************************************************************/

using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using System.Web.UI;

using Arena.Custom.Cccev.DataUtils;
using Arena.Portal;
using Arena.Feed;
using Arena.DataLayer.Feed;

namespace ArenaWeb.UserControls.Custom.Cccev.WebUtils
{

	public partial class PodcastChannelViewer : PortalControl
	{
		#region Module Settings
		[PageSetting("Player Page", "The page that contains a media player", false)]
        public string PlayerPageSetting { get { return Setting("PlayerPage", "-1", false); } }

		[NumericSetting( "Time to Cache (minutes)", "Time in minutes to cache contents.  Default 3.", false )]
		public string TimeCacheMinutesSetting { get { return Setting( "TimeCacheMinutes", "3", false ); } }

		[ListFromSqlSetting("Video Channel Format", "The Channel Format for the Video feed", false, "-1",
            "SELECT format_id, title FROM feed_format ORDER BY title")]
        public int VideoFormatSetting { get { return int.Parse(Setting("VideoFormat", "-1", false)); } }

        [ListFromSqlSetting("Audio Channel Format", "The Channel Format for the Audio feed", false, "-1",
            "SELECT format_id, title FROM feed_format ORDER BY title")]
        public int AudioFormatSetting { get { return int.Parse(Setting("AudioFormat", "-1", false)); } }

        [ListFromSqlSetting("Flash Video Channel Format", "The Channel Format for the Flash Video feed", false, "-1",
            "SELECT format_id, title FROM feed_format ORDER BY title")]
        public int FlashFormatSetting { get { return int.Parse(Setting("FlashFormat", "-1", false)); } }

        //TODO: Make SmartPageSetting.
        [PageSetting("Channel Details Page", "The page to go to when a channel is selected", true)]
        public int DetailsPageSetting { get { return int.Parse(Setting("DetailsPage", "-1", true)); } }

        [NumericSetting("Channel Width", "The width, in pixels, of a single channel icon.", false)]
        public int PageWidthSetting { get { return int.Parse(Setting("PageWidth", "717", false)); } }

        [TextSetting( "tabSlide js Path", "Path to the tabSlide .js script such as ~/Templates/Custom/blah/jquery.tabSlide.js", false )]
        public string ScriptPathSetting { get { return Setting( "ScriptPath", "", false ); } }

		[LookupSetting( "Content Category", "The content category to use.", false, "624B9F73-7882-4F4B-AD62-2B67AA0090B9" )]
		public int ContentCategorySetting { get { return int.Parse( Setting( "ContentCategory", "414", false )); } }

		#endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if ( ScriptPathSetting != string.Empty )
            {
                smpScripts.Scripts.Add( new ScriptReference( ScriptPathSetting ) );
            }
        }

		public string GetDetailsLink(int channelId, int formatId)
        {
            return string.Format("Default.aspx?page={0}&channel={1}&format={2}", DetailsPageSetting, channelId, formatId);
        }

        public string GetRssLink(int channelId, int formatId)
        {
            return string.Format("rss.aspx?c={0}&f={1}", channelId, formatId);
        }

        public string GetImage(Channel channel)
        {
            return string.Format("CachedBlob.aspx?guid={0}", channel.Image.GUID);
        }

        public IEnumerable<Channel> GetChannels()
        {
			IEnumerable<Channel> channels = null;
			string cacheKey = "Cccev.WebUtils.PodcastChannelViewer.Channels";
			// get from the cache if available and if refreshcache is not present
			if ( Request.QueryString["refreshcache"] == null && Cache[cacheKey] != null )
			{
				channels = (IEnumerable<Channel>)Cache[cacheKey];
			}

			if ( channels == null )
			{
				channels = new ChannelData().GetChannelList( Constants.NULL_STRING, true ).AsEnumerable()
					.Select( r => new Channel( (int)r["channel_id"] ) ).Where( c => c.ContentCategory.LookupID == ContentCategorySetting );
				Cache.Add( cacheKey, channels, null, DateTime.Now.AddMinutes( int.Parse( TimeCacheMinutesSetting ) ), Cache.NoSlidingExpiration, CacheItemPriority.High, null );
			}

			return channels;
        }

        public int GetCurrentChannel(IEnumerable<Channel> channels)
        {
            if (int.Parse(Request.QueryString["channel"]) != -1)
            {
                return int.Parse(Request.QueryString["channel"]);
            }
            
            return channels.First().ChannelId;
        }

	    public int GetIndexOfCurrentChannel(IEnumerable<Channel> channels)
        {
            int count = 0;
            int currentId = GetCurrentChannel(channels);
            foreach (var channel in channels)
            {
                if (channel.ChannelId == currentId)
                {
                    break;
                }

                count++;
            }
            return count;
        }

		/// <summary>
		/// Uses the cache to get/store the link to the latest channel's topic's item.
		/// </summary>
		/// <param name="channel">A feed Channel</param>
		/// <returns>URL for the page of the player for the latest item (or "#" if PlayerPageSetting is not set)</returns>
		public string GetPlayerPageForLatestItem( Channel channel )
		{
			if ( PlayerPageSetting == "-1" )
			{
				return "#";
			}

			string cacheKey = String.Format( "Cccev.WebUtils.PodcastChannelViewer.Channel:{0}", channel.ChannelId );
			string url = null;

			// get from the cache if available and if refreshcache is not present
			if ( Request.QueryString["refreshcache"] == null && Cache[cacheKey] != null )
			{
				url = (string)Cache[cacheKey];
			}

			if ( url == null && PlayerPageSetting != "-1" )
			{
				Item item = channel.Items.OrderByDescending( i => i.PublishDate ).FirstOrDefault();
				if ( item != null )
				{
					url = String.Format( "Default.aspx?page={0}&item={1}", PlayerPageSetting, item.ItemId );
					Cache.Add( cacheKey, url, null, DateTime.Now.AddMinutes( int.Parse( TimeCacheMinutesSetting ) ), Cache.NoSlidingExpiration, CacheItemPriority.High, null );
				}
			}
			
			return url;
		}
	}
}