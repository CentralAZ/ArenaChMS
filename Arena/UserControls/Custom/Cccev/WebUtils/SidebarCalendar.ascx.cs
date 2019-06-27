/**********************************************************************
* Description:	Caledar formatted for website sidebar that displays Arena Events given a parent EventTag ID
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created:	4/20/2009
*
* $Workfile: SidebarCalendar.ascx.cs $
* $Revision: 22 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/SidebarCalendar.ascx.cs   22   2010-07-21 14:21:57-07:00   JasonO $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/SidebarCalendar.ascx.cs $
*  
*  Revision: 22   Date: 2010-07-21 21:21:57Z   User: JasonO 
*  
*  Revision: 21   Date: 2010-02-17 16:52:47Z   User: JasonO 
*  Fixing jquery include issues. 
*  
*  Revision: 20   Date: 2010-01-27 22:49:28Z   User: JasonO 
*  Cleaning up. 
*  
*  Revision: 19   Date: 2009-07-28 17:10:11Z   User: JasonO 
*  Refactoring code. 
*  
*  Revision: 18   Date: 2009-04-20 22:27:54Z   User: JasonO 
*  Adding look ahead on page load. 
*  
**********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using Arena.Core;
using Arena.Custom.Cccev.DataUtils;
using Arena.Custom.Cccev.FrameworkUtils.Entity;
using Arena.Event;
using Arena.Portal;

namespace ArenaWeb.UserControls.Custom.Cccev.WebUtils
{
    public partial class SidebarCalendar : PortalControl
    {
        #region Module Settings

        [NumericSetting("Parent Event Pofile ID", "Event Profile ID of parent Event Tag.", true)]
        public string ParentEventProfileIDSetting { get { return Setting("ParentEventProfileID", "", true); } }

        [ListFromSqlSetting("Topic Areas", "Collection of topic areas to display events from.", true, "",
            "SELECT [lookup_id], [lookup_value] FROM [core_lookup] WHERE [lookup_type_id] = 69 ORDER BY [lookup_value]", 
            ListSelectionMode.Multiple)]
        public string TopicAreasSetting { get { return Setting("TopicAreas", "", true); } }

        [PageSetting("Event Details Page", "Page ID of event details page.", true)]
        public string EventDetailsPageSetting { get { return Setting("EventDetailsPage", "", true); } }

        [NumericSetting("Event Look Ahead", "Number of days to look in advance for events to be displayed on the calendar during page load (default is 30).", false)]
        public string EventDayLookAheadSetting { get { return Setting("EventDayLookAhead", "30", false); } }

        #endregion

        #region Private Variables

        private List<EventProfile> events;
        private Dictionary<int, EventProfile> eventHash;
        private List<EventProfileViewModel> cccevEvents;

        #endregion

        #region Page Events

        protected void Page_Init(object sender, EventArgs e)
        {
            calDates.SelectionChanged += calDates_SelectionChanged;
            calDates.DayRender += calDates_DayRender;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            eventHash = new Dictionary<int, EventProfile>();
            cccevEvents = new List<EventProfileViewModel>();
            Page.Header.Controls.Add(new LiteralControl("\t\t<link rel=\"stylesheet\" type=\"text/css\" href=\"/arena/Templates/Cccev/liger/css/calendar.css\" />\n"));

            if (!Page.IsPostBack)
            {
                smpScripts.Scripts.Add(new ScriptReference(string.Format("~/{0}", BasePage.JQUERY_INCLUDE)));
                smpScripts.Scripts.Add(new ScriptReference("~/Templates/Cccev/liger/js/jquery.galleryScroll.1.4.5.pack.js"));
                smpScripts.Scripts.Add(new ScriptReference("~/Templates/Cccev/liger/js/main.js"));
                smpScripts.Scripts.Add(new ScriptReference("~/Templates/Cccev/liger/js/popover.js"));
                smpScripts.Scripts.Add(new ScriptReference("~/Templates/Cccev/liger/js/calendar.js"));

                InitCalendar();
                BuildEvents(LoadNextActiveEvents(DateTime.Now).ToList());
            }
        }

        #endregion

        #region Calendar Events

        private void calDates_DayRender(object sender, DayRenderEventArgs e)
        {
            List<EventProfile> currentEvents = GetCurrentDayEvents(e.Day.Date).ToList();

            if (currentEvents.Count > 0)
            {
                e.Cell.Style.Add("font-weight", "bold");
            }
        }

        private void calDates_SelectionChanged(object sender, EventArgs e)
        {
            List<EventProfile> currentEvents = GetCurrentDayEvents(calDates.SelectedDate).ToList();
            SetTitleDate(calDates.SelectedDate);
            SetEventMonthYear(calDates.SelectedDate);
            BuildEvents(currentEvents);
        }

        #endregion

        #region Private Methods

        private void InitCalendar()
        {
            if (calDates.VisibleDate == DateTime.MinValue)
            {
                calDates.VisibleDate = DateTime.Now;
            }

            if (calDates.SelectedDate == DateTime.MinValue)
            {
                calDates.SelectedDate = DateTime.Now;
            }

            SetTitleDate(calDates.SelectedDate);
            SetEventMonthYear(calDates.SelectedDate);
        }

        private void LoadEvents()
        {
            int month = calDates.VisibleDate.Month;
            int year = calDates.VisibleDate.Year;
            int dayCount = DateTime.DaysInMonth(year, month);
            DateTime monthStart = new DateTime(year, month, 1);
            DateTime monthEnd = new DateTime(year, month, dayCount);
            EventProfileCollection epc = new EventProfileCollection();
            cccevEvents = epc.LoadEventProfilesByTopicMonthAndParentID(int.Parse(ParentEventProfileIDSetting), monthStart.AddDays(-8), monthEnd.AddDays(8), TopicAreasSetting);
            events = epc.OfType<EventProfile>().ToList();
            
            foreach (EventProfile e in events)
            {
                if (!eventHash.ContainsKey(e.ProfileID))
                {
                    eventHash.Add(e.ProfileID, e);
                }
            }

            CacheEvents();
        }

        private IEnumerable<EventProfile> LoadNextActiveEvents(DateTime currentDate)
        {
            IEnumerable<EventProfile> currentDayEvents = GetCurrentDayEvents(currentDate);

            if (currentDayEvents.Count() == 0)
            {
                for (int i = 1; i < int.Parse(EventDayLookAheadSetting); i++)
                {
                    IEnumerable<EventProfile> nextDayEvents = GetCurrentDayEvents(currentDate.AddDays(i));

                    if (nextDayEvents.Count() > 0)
                    {
                        currentDayEvents = nextDayEvents;
                        break;
                    }
                }
            }

            return currentDayEvents;
        }

        private string GetCacheKey(DateTime date)
        {
            return string.Format("cccev.liger.events-{0}-{1}_{2}_{3}", date.Month, date.Year, ParentEventProfileIDSetting, TopicAreasSetting);
        }

        private void CacheEvents()
        {
            string key = GetCacheKey(calDates.VisibleDate);
            CacheItemRemovedCallback onRemove = CacheRemovedCallback;
            Cache.Add(key, events, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration, CacheItemPriority.High, onRemove);
            Cache.Add(key + "cccevEvents", cccevEvents, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration, CacheItemPriority.High, onRemove);
            Cache.Add(key + "eventHash", eventHash, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration, CacheItemPriority.High, onRemove);
        }

        private static void CacheRemovedCallback(string k, object v, CacheItemRemovedReason r)
        {
        }

        private IEnumerable<EventProfile> GetCurrentDayEvents(DateTime currentDate)
        {
            string key = GetCacheKey(calDates.VisibleDate);

            if (Cache[key] != null)
            {
                events = (List<EventProfile>)Cache[key];
                cccevEvents = (List<EventProfileViewModel>)Cache[key + "cccevEvents"];
                eventHash = (Dictionary<int, EventProfile>)Cache[key + "eventHash"];
            }
            else
            {
                if (events == null)
                {
                    LoadEvents();
                }
            }

            var currentDayEvents = (from ce in cccevEvents
                                    where ce.OccurrenceStart.Date == currentDate.Date
                                    select ce).OrderBy(ce => ce.OccurrenceStart);

            foreach (EventProfileViewModel e in currentDayEvents)
            {
                EventProfile p = eventHash[e.ProfileID];
                EventProfile newProfile = new EventProfile
                                              {
                                                  ProfileID = p.ProfileID, 
                                                  Name = p.Name, 
                                                  Start = e.OccurrenceStart, 
                                                  ForiegnKey = e.OccurrenceID.ToString()
                                              };

                if (p.LocationId != -1)
                {
                    newProfile.Location = p.Location;
                }
                else
                {
                    Arena.Organization.Location location = new Arena.Organization.Location
                                                               {
                                                                   BuildingName = new Occurrence(e.OccurrenceID).Location
                                                               };
                    newProfile.Location = location; 
                }

                yield return newProfile;
            }
        }

        private void SetTitleDate(DateTime selected)
        {
            lblDay.Text = selected.DayOfWeek.ToString().Substring(0, 3).ToUpper() + ".";
            string date = selected.Day.ToString();

            if (date.Length < 2)
            {
                date = date.Insert(0, "0");
            }

            lblDate.Text = date;
        }

        private void SetEventMonthYear(DateTime currentDate)
        {
            lblEventMonthYear.Text = currentDate.ToString("MMMM yyyy");
        }

        private void BuildEvents(IList<EventProfile> currentEvents)
        {
            StringBuilder html = new StringBuilder();

            if (currentEvents.Count > 0)
            {
                if (currentEvents.Count > 1)
                {
                    BuildSlideControls();
                }

                for (int i = 0; i < currentEvents.Count; i++)
                {
                    string location;

                    // This hack needs to be removed once Arena adds Campuses into Locations
                    if (currentEvents[i].Location.LocationId != Constants.NULL_INT && currentEvents[i].Location.BuildingName.IndexOf('-') > Constants.NULL_INT)
                    {
                        location = currentEvents[i].Location.BuildingName.Substring(0, currentEvents[i].Location.BuildingName.IndexOf('-'));
                    }
                    else
                    {
                        location = currentEvents[i].Location.BuildingName;
                    }

                    html.Append("\t\t\t\t\t\t<li>\n");
                    html.Append("\t\t\t\t\t\t\t<div class=\"box\">\n");
                    html.AppendFormat("\t\t\t\t\t\t\t\t<p class=\"white\">{0}</p>\n", currentEvents[i].Start.ToString("MMMM dd hh:mm tt"));
                    html.AppendFormat("\t\t\t\t\t\t\t\t<p class=\"yellow\">Event: {0}</p>\n", currentEvents[i].Name);
                    html.AppendFormat("\t\t\t\t\t\t\t\t<p class=\"orange\">Location: {0}</p>\n", location);
                    html.Append("\t\t\t\t\t\t\t</div>\n");
                    html.Append("\t\t\t\t\t\t\t<div class=\"btn-box\">\n");
                    html.AppendFormat("\t\t\t\t\t\t\t\t<a href=\"default.aspx?page={0}&occurrenceId={1}&profileId={2}\" class=\"right-mar\">Learn More</a>\n",
                        EventDetailsPageSetting, currentEvents[i].ForiegnKey, currentEvents[i].ProfileID);
                    html.AppendFormat("\t\t\t\t\t\t\t\t<a class=\"email-event\" id=\"{0}\" href=\"#\">Send Invite</a>\n", 
                        currentEvents[i].ProfileID + "_" + currentEvents[i].ForiegnKey);
                    html.Append("\t\t\t\t\t\t\t</div>\n");
                    html.Append("\t\t\t\t\t\t</li>\n");
                }
            }
            else
            {
                html.Append("\t\t\t\t\t\t<li>\n");
                html.Append("\t\t\t\t\t\t\t<div class=\"box\">\n");
                html.AppendFormat("\t\t\t\t\t\t\t\t<p class=\"white\">No events found on {0}.</p>\n", calDates.SelectedDate.ToString("MMMM dd"));
                html.Append("\t\t\t\t\t\t\t\t<p class=\"yellow\">&nbsp;</p>\n");
                html.Append("\t\t\t\t\t\t\t\t<p class=\"orange\">&nbsp;</p>\n");
                html.Append("\t\t\t\t\t\t\t</div>\n");
                html.Append("\t\t\t\t\t\t\t<div class=\"btn-box\">\n");
                html.Append("\t\t\t\t\t\t\t</div>\n");
            }

            phEvents.Controls.Add(new LiteralControl(html.ToString()));
        }

        private void BuildSlideControls()
        {
            StringBuilder html = new StringBuilder();
            html.Append("\t\t\t\t\t\t<div class=\"links-holder\">\n");
            html.Append("\t\t\t\t\t\t\t<a class=\"right\" href=\"#\">right</a>\n");
            html.Append("\t\t\t\t\t\t\t<a class=\"left\" href=\"#\">left</a>\n");
            html.Append("\t\t\t\t\t\t</div>\n");
            phSlideControls.Controls.Add(new LiteralControl(html.ToString()));
        }

        #endregion
    }
}