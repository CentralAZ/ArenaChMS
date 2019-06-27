<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PodcastChannelViewer.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.WebUtils.PodcastChannelViewer" %>
<%@ Register TagPrefix="Arena" Namespace="Arena.Portal.UI" Assembly="Arena.Portal.UI" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Arena.Feed" %>

<asp:ScriptManagerProxy ID="smpScripts" runat="server" />

<div class="columns-holder-area-tv">
	<div id="gallery">
		<a class="link-prev" href="#">link-prev</a>
		<div class="list">
	<%                  
            var channels = GetChannels();
	%>   
			<ul style='margin-left: 0'>
            <%	    
                foreach (var channel in channels)
                {   
            %> 
                    <li>
					    <div class="tv-holder">
							<a href="<%= GetPlayerPageForLatestItem( channel ) %>" title="play latest">
							    <img alt="<%= channel.Title %>" src='<%= GetImage(channel) %>'/>
							</a>
					    </div>
					    <div class="text-box">
						    <p class="title"><%= channel.Title %></p>
						<%
							string verb = "Listen";
                            if (channel.ChannelFormats.Any(f => f.FormatId == VideoFormatSetting)) 
                            {
								verb = "Watch";
							} 
                        %>
                                <strong><a href='<%= GetDetailsLink(channel.ChannelId, FlashFormatSetting) %>'><%= verb %> Online</a></strong>
                        <%
                            if (channel.ChannelFormats.Any(f => f.FormatId == VideoFormatSetting))
                            {
                        %>
                                <span><a href='<%= channel.FeedPublicUrl( string.Empty, channel.ChannelFormats.Find( f => f.FormatId == VideoFormatSetting ) ) %>'>Subscribe to Video</a></span> 
                        <%
                            }
                            if (channel.ChannelFormats.Any(f => f.FormatId == AudioFormatSetting))
                            {
                        %>
                               <span><a href='<%= channel.FeedPublicUrl( string.Empty, channel.ChannelFormats.Find( f => f.FormatId == AudioFormatSetting ) ) %>'>Subscribe to Audio</a></span> 
                        <%
                            }
                        %>					    
					    </div>
				    </li>
            <%
                }      
		    %>
			</ul>
		</div>
		<a class="link-next" href="#">link-next</a>
	</div>
	<div class="title-box">
		<ul>
	<%
        foreach (var channel in channels)
        {
    %>
            <li><a href="#" class="<%= channels.First().ChannelId == channel.ChannelId ? "active" : "" %>"><%= channel.SubTitle %></a></li>
    <%
        }   
	%>
		</ul>
	</div>
</div>