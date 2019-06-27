<%@ Control Language="C#" ClassName="ArenaWeb.UserControls.Custom.Cccev.Web2.HasselhoffEventCloud" Inherits="Arena.Portal.PortalControl" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Arena.Portal" %>

<asp:ScriptManagerProxy ID="smpScripts" runat="server">
    <Scripts>
        <asp:ScriptReference Path="~/Include/scripts/jquery.hoverIntent.min.js" />
        <asp:ScriptReference Path="~/UserControls/Custom/Cccev/Web2/js/fullcalendar.mod.min.js" />
        <asp:ScriptReference Path="~/UserControls/Custom/Cccev/Web2/js/eventcalendar.min.js" />
    </Scripts>
</asp:ScriptManagerProxy>

<script runat="server">
    
    [PageSetting("Event Detail Page", "Reference to Event Details page.", true)]
    public string EventDetailPageSetting { get { return Setting("EventDetailPage", "", true); } }
    
    private void Page_Load(object sender, EventArgs e)
    {
        if (!Page.Header.Controls.OfType<LiteralControl>().Any(c => c.Text.Contains("id=\"cloud-item\"")))
        {
            StringBuilder template = new StringBuilder();
            template.AppendLine("<script type=\"text/html\" id=\"cloud-item\">");
            template.AppendLine("<li class=\"tag\">");
            template.AppendLine("<a href=\"{%= url %}\" class=\"{%= className %}\">{%= title %}</a>");
            template.AppendLine("<div class=\"tag-description\"><span class=\"bubble\"><span class=\"time\">{%= time %}</span>{%= description %}</span></div>");
            template.AppendLine("</li>");
            template.AppendLine("</scr" + "ipt>");
            Page.Header.Controls.Add(new LiteralControl(template.ToString()));
        }
    }
    
</script>

<input type="hidden" id="ihCloudEventDetails" value="<%= EventDetailPageSetting %>" />
<div id="event-cloud" class="event-view">
    <p>Listed below are upcoming events at Central Christian Church, <span class="campus"></span>. The larger the event name, the closer to today’s date it takes place. 
    Use the tools to the left to filter by keyword or to narrow or widen your search.</p>
    <ul id="cloud">
    </ul>
</div>
<div id="cloud-overlay"></div>