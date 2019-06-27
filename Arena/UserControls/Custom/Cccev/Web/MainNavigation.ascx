<%@ Control Language="C#" ClassName="ArenaWeb.UserControls.Custom.Cccev.Web.MainNavigation" Inherits="Arena.Portal.PortalControl" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Arena.Portal" %>
<%
/**********************************************************************
* Description:	Main Navigation
* Created By:	Jason Offutt @ Central Christian Church of the East Valley
* Date Created:	10/19/2011 12:59:59 PM
*
* $Workfile: MainNavigation.ascx $
* $Revision: 7 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Web/MainNavigation.ascx   7   2011-11-17 15:23:01-07:00   nicka $
**********************************************************************/
%>
<script runat="server">
	[CustomListSetting( "* Instructions", "This module depends on UserControls/Custom/Cccev/Web/scripts/CentralAZ.web.min.js",
		false, "", new string[] { "" }, new string[] { "1" } )]
	public string InstructionsSetting { get { return Setting( "Instructions", "", false ); } }
	
    [PageSetting("Root Page", "Page to display root nav from.", true)]
    public string RootPageSetting { get { return Setting("RootPage", "", true); } }

    [BooleanSetting("Use Default CSS Document", "If checked, the CSS document bundled with this module will be included.", true, true)]
    public string UseDefaultCssSetting { get { return Setting("UseDefaultCss", "true", true); } }

	[TextSetting( "Skip Prefix", "Will prevent the link's title attribute value from being set if the page 'description' matches this supplied prefix.", false )]
	public string SkipPrefixSetting { get { return Setting( "SkipPrefix", "", false ); } }
	
    private PortalPage rootPage;
    private IEnumerable<PortalPage> topLevelNav;
	private string target;
	
    private void Page_Load(object sender, EventArgs e)
    {
        if (bool.Parse(UseDefaultCssSetting))
        {
            BasePage.AddCssLink(Page, "~/UserControls/Custom/Cccev/Web/css/mainnavigation.css");
        }

        BasePage.AddJavascriptInclude(Page, "UserControls/Custom/Cccev/Web/scripts/CentralAZ.web.min.js");
        rootPage = new PortalPage(int.Parse(RootPageSetting));
		target = (string.IsNullOrEmpty( CurrentPortalPage.Settings["Target"] )) ? "" : string.Format( "target=\"{0}\"", CurrentPortalPage.Settings["Target"]);
		
        topLevelNav = rootPage.ChildPages.Where(p => p.DisplayInNav);
    }

	private static string EscapeTitle( string value, string skipPrefix )
	{
		if ( string.IsNullOrEmpty( skipPrefix ) || ! value.StartsWith( skipPrefix ) )
		{
			return HttpUtility.HtmlEncode( ( ! string.IsNullOrEmpty( value ) ) ? value : "" );
		}
		else
		{
			return "";
		}
	}
</script>
<nav>
    <ul class="main-nav">
<% foreach (var page in topLevelNav) { %>
        <li>
            <a <%= target %> href="<%= page.NavigationUrl %>"><%= page.Title %></a>
            <% if (page.ChildPages.Any(p => p.DisplayInNav)) { %>
                <ul class="sub-nav">
                <% foreach (var header in page.ChildPages.Where(p => p.DisplayInNav)) { %>
                    <li>
                        <% if (header.ChildPages.Any(p => p.DisplayInNav)) { 
                               var navItems = header.ChildPages.Where(p => p.DisplayInNav); %>
							<h3><%= header.Title %></h3>
						    <ul class="three-col">
                            <% foreach (var navItem in navItems) { %>
                                <li><a <%= target %> href="<%= navItem.NavigationUrl %>" title="<%= EscapeTitle( navItem.Description, SkipPrefixSetting ) %>"><%= navItem.Title %></a></li>
                            <% } %>
                            </ul>
                        <% } else { %>
				            <a <%= target %> href="<%= header.NavigationUrl %>" title="<%= EscapeTitle( header.Description, SkipPrefixSetting ) %>"><%= header.Title%></a>
						<% } %>
                    </li>
                 <% } %>
                </ul>
            <% } %>
        </li>
<% } %>
    </ul>
</nav>
