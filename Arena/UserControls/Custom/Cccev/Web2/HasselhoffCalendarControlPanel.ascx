<%@ Control Language="C#" ClassName="ArenaWeb.UserControls.Custom.Cccev.Web2.HasselhoffCalendarControlPanel" Inherits="Arena.Portal.PortalControl" %>
<%@ Import Namespace="Arena.Core" %>
<%@ Import Namespace="Arena.Portal" %>

<script runat="server">
    [ListFromSqlSetting("Primary Topic Area", "Set this to tie calendar to a specific topic or ministry.", false, "",
        "SELECT l.lookup_id, l.lookup_value FROM core_lookup l INNER JOIN core_lookup_type lt ON lt.lookup_type_id = l.lookup_type_id AND lt.guid = '1FE55E22-F67C-46BA-A6AE-35FD112AFD6D' WHERE active = 1 ORDER BY lookup_order", ListSelectionMode.Multiple )]
    public string TopicAreaSetting { get { return Setting("TopicArea", "", false); } }

    [TextSetting( "Calendar Prepend Title", "Name to prepend to calendar title when a topic area is specified. Default 'Ministry'.", false )]
    public string CalendarTitleSetting { get { return Setting( "CalendarTitle", "Ministry", false ); } }
    
    private void Page_Load(object sender, EventArgs e)
    {
        BasePage.AddCssLink(Page, "~/UserControls/Custom/Cccev/Web2/css/jquery-ui-1.8.2.custom.css");
        BasePage.AddCssLink(Page, "~/UserControls/Custom/Cccev/Web2/css/calendar.css");
    }
</script>

<asp:ScriptManagerProxy ID="smpScripts" runat="server">
    <Scripts>
        <asp:ScriptReference Path="~/UserControls/Custom/Cccev/Web2/js/jquery-ui-1.8.2.custom.min.js" />
        <asp:ScriptReference Path="~/UserControls/Custom/Cccev/Web2/js/eventCalendar-controls.min.js" />
    </Scripts>
</asp:ScriptManagerProxy>

<% if (TopicAreaSetting.Trim() != string.Empty)
   {
       %> <h2 class="multi-line-heading"><%= string.Format( "{0}<br />", CalendarTitleSetting )%>Calendar</h2> <%
   }
   else
   {
       %> <h2 class="content-heading">Calendar</h2> <%
   }
%>
<div class="control-wrap">
    
    <div id="slider-wrap">
        <h3>Date Range</h3>
        <div id="date-slider"></div>
        <div class="range">
            <span class="min"></span>
            <span class="max"></span>
        </div>
    </div>

    <h3>Keywords</h3>
    <asp:TextBox ID="tbKeywords" runat="server" CssClass="calendar-search" />

    <h3>View</h3>
    <ul id="calendar-views">
        <li><input type="radio" id="rbCalendar" name="calendar-view" checked="checked" rel="calendar" /><label for="rbCalendar">Calendar</label></li>
        <li><input type="radio" id="rbList" name="calendar-view" rel="list" /><label for="rbList">List</label></li>
        <li><input type="radio" id="rbCloud" name="calendar-view" rel="cloud" /><label for="rbCloud">Tag Cloud</label></li>
    </ul>
    <input type="hidden" id="ihTopicAreas" name="ihTopicAreas" value="<%= TopicAreaSetting %>" />
</div>