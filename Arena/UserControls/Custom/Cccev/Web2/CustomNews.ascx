<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CustomNews.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.Web2.CustomNews" %>
<%@ Import Namespace="System.Linq" %>
<%@ Register TagPrefix="Arena" Namespace="Arena.Portal.UI" Assembly="Arena.Portal.UI" %>

<script type="text/javascript">
    /// Note: This depends on Templates/Custom/CCCEV/Hasselhoff/campus-scripts.js
	var news = null;

	$(function()
	{
        showNews();

        $("#saveNews").click(function ()
        {
            $("#topics").fadeOut("fast");

            var topicString = "";
            $("#topic-list .selected").each(function ()
            {
                if (topicString.length > 0)
                {
                    topicString += ",";
                }

                topicString += $(this).attr("rel");
            });

            saveTopics(topicString);
            return false;
        });

        $(document).bind("USER_LOGGED_IN CAMPUS_UPDATED TOPICS_UPDATED", function ()
        {
            news = null;
            showNews();
            return false;
        });

        $(document).bind("CAMPUS_UPDATING TOPICS_UPDATING", function ()
        {
            $("#news-events").fadeTo("fast", "0.01");
            return false;
        });
	});
    
    function showNews()
    {
        var campusID = campus != null ? campus.campusID : <%= DefaultCampusIDSetting %>;

        if ( topicList === null || topicList.toString().trim() === "" )
        {
            topicList = "<%= DefaultTopicAreasSetting %>";
        }

        if (news === null)
        {
	        loadNews(campusID, topicList, <%= PromotionDisplayPageIDSetting %>,  <%= EventDisplayPageIDSetting %>, onNewsLoadSuccess, onNewsLoadError);
        }
        else
        {
            renderNewsInfo();
        }
    }

    function onNewsLoadSuccess(result)
    {
        if (result.d.length > 0)
        {
            news = result.d;
        }
        else
        {
            loadBlankNews("There isn't any news to show you. Please modify your news selection.");
        }

        renderNewsInfo();
        return false;
    }

    function onNewsLoadError(result, errorText, thrownError)
    {
        loadBlankNews( "The news not available at this time.")
        return false;
    }

	function renderNewsInfo()
	{
		$("#news-events").empty();
        $("#news-events").fadeTo("fast", "1.0");
		$("#news-template").render(news).appendTo("#news-events");
		return false;
	}

    function loadBlankNews(message)
    {
        news = new Array();
        news[0] = {
            id: -1,
            topic: "Central News",
            title: "Uh Oh!",
            summary: message,
            imageUrl: "#",
            detailsUrl: "#"
        };
    }
</script>

            <h2 class="content-heading">
                News &amp; Events 
                <a href="#" title="Customize what you see here" id="customize">
                    <img src="Templates/CCCEV/Hasselhoff/img/customize-button.png" alt="Customize what you see here" />
                </a>
            </h2>
            <ul id="news-events">
            </ul>
            <div id="topics">
                <div class="aside">
                    <h2>Your Interests</h2>
                    <p>Select the ministries at Central that interest you the most to customize your homepage news feed. Save to close window.</p>
                    <a href="#" class="button" id="saveNews">Save</a>
                </div>
                <ul id="topic-list">
                <%
                    var topics = GetAllTopics();
                    var attributes = GetTopicPreferences();

                    foreach (var topic in topics) 
                    {
                        var theTopic = topic;
                        if (attributes.Any(t => t.LookupID == theTopic.LookupID))
                        { 
                            %> <li><a href="#" class="selected" rel="<%= theTopic.LookupID %>"><%= theTopic.Value%></a></li> <%
                        }
                        else
                        {
                            %> <li><a href="#" rel="<%= theTopic.LookupID %>"><%= theTopic.Value%></a></li> <%
                        }
                    } %>
                </ul>
            </div>