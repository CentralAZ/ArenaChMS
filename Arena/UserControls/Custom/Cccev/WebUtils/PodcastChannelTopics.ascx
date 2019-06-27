<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PodcastChannelTopics.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.WebUtils.PodcastChannelTopics" %>
<%@ Register TagPrefix="Arena" Namespace="Arena.Portal.UI" Assembly="Arena.Portal.UI" %>
<%@ Import Namespace="System.Linq" %>

<asp:ScriptManagerProxy ID="smpScripts" runat="server" />

<div class="columns-holder-area-tv">
	<div class="top-title">
		<a class="back-channel" href="<%= "Default.aspx?page=" + ChannelsPageSetting %>">Back to Message Series</a>
		<h1><%= CurrentChannel.Title %></h1>
		<h2>Message Series</h2>
	</div>
	<div class="top-mar" id="gallery">
		<a class="link-prev other" href="#">link-prev</a>
		<div class="list">
			<ul style="margin-left: 0px;">
	        <%
                var seriesList = GetSeries(); 
	            
                foreach (var series in seriesList)
                {
            %>
                    <li>
					    <div class="white-box-top">
						    <div class="white-box-btm">
							    <div class="white-box">
								    <h2><%= series.Title %></h2>
								    <div class="img-holder">
									    <img alt="<%= series.Title %>" src="<%= GetImage(series) %>"/>
								    </div>
								    <div class="text-holder">
									    <p><%= ArenaWeb.Utilities.replaceCRLF( series.Description ) %></p>
									    <h2>Messages In This Series:</h2>
									    <div class="links-holder">
									    <%
                                            var messages = GetMessages(series);
                    
                                            foreach (var msg in messages)
                                            {
                                                if (bool.Parse(ShowUnpublishedTopicItemsSetting) && msg.PublishDate > DateTime.Now)
                                                {
                                        %>
                                                    <span class="inactive-topic"><%= msg.Title %></span>
                                        <%
                                                }
                                                else
                                                {
                                        %>
                                                    <a href="<%= GetPlayerLink(msg) %>"><%= msg.Title %> <span class="date"><%= msg.PublishDate.ToShortDateString() %></span></a>
                                        <%
                                                }
                                            }
                                        %>
									    </div>
								    </div>
							    </div>
						    </div>
					    </div>
				    </li>
            <%
                }
	        %>
			</ul>
		</div>
		<a class="link-next other" href="#">link-next</a>
	</div>
	<div class="title-box tb-other"></div>
</div>