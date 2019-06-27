<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PodcastVideoPlayer.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.WebUtils.PodcastVideoPlayer" %>

<asp:ScriptManagerProxy ID="smpScripts" runat="server" />

<script type="text/javascript">
    $(document).ready(function()
    {
        $('div.flash_player').css('z-index', '1');
    });
</script>

<div class="columns-holder-area-tv">
    <div class="top-title">
        <a class="back" href="<%= GetSeriesPageUrl() %>">Back to Message Series</a>
        <h1><%= GetServiceType() %></h1>
        <h2><%= GetSermonSeriesName() %></h2>
    </div>
    <div class="player-holder">
        <div id="flash_player" class="player">
            <asp:Literal ID="lError" runat="server" Visible="false" />
            <asp:PlaceHolder ID="phSwfObject" runat="server" />
        </div>
        <div class="description"><%= GetDescription() %></div>
        <a class="email" href="<%= GetMailto() %>">Email This Message</a>
        <% if ( DonateLinkSetting != string.Empty ) { %>
        <a class="donate" href="<%= DonateLinkSetting %>">Give Online</a>
        <% } %>
        <% if ( EnableFacebookLikeSetting ) { %>
        <div><iframe src="http://www.facebook.com/widgets/like.php?href=<%= GetItemPageUrl() %>" scrolling="no" frameborder="0" class="facebook-like"></iframe></div>
        <% } %>
    </div>
    <div class="title-box otherm"></div>
</div>