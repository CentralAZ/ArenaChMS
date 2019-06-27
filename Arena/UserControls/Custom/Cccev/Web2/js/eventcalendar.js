/// <reference path="/Include/scripts/jquery-1.3.2.vsdoc.js" />
/// <reference path="fullcalendar.js" />
/// <reference path="/Include/scripts/jquery.hoverintent.min.js" />

/**********************************************************************
* Description:  JavaScript for event calendar views
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created: 7/12/2010
*
* $Workfile: eventcalendar.js $
* $Revision: 17 $
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Web2/js/eventcalendar.js   17   2011-04-05 15:46:09-07:00   JasonO $
*
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Web2/js/eventcalendar.js $
*  
*  Revision: 17   Date: 2011-04-05 22:46:09Z   User: JasonO 
*  Functionality updates for Glendale campus rollout and usability 
*  improvements. 
*  
*  Revision: 16   Date: 2010-08-05 00:09:35Z   User: nicka 
*  Condense events on list view and never show only 3 featured items. 
*  
*  Revision: 15   Date: 2010-08-03 23:56:55Z   User: JasonO 
*  Adding ability to define topic areas on calendar pages. 
*  
*  Revision: 14   Date: 2010-08-03 00:40:03Z   User: JasonO 
*  Bug Fix: In Webkit browsers, title attributes on links would overlay 
*  tooltips. 
*  Bug Fix: On first time visit to calendar page, campus would be null, 
*  causing an error to be thrown when attempting to load events. 
*  
*  Revision: 13   Date: 2010-08-02 22:39:07Z   User: JasonO 
*  Minifying calendar scripts 
*  
*  Revision: 12   Date: 2010-08-02 21:23:54Z   User: JasonO 
*  Adding support for hover intent 
*  
*  Revision: 11   Date: 2010-08-02 19:12:41Z   User: JasonO 
*  Adding support for tooltips on event cloud view. 
*  
*  Revision: 10   Date: 2010-08-02 17:17:52Z   User: JasonO 
*  Updating calendar controls and associated scripts/css so each control has 
*  its own overlay. 
*  
*  Revision: 9   Date: 2010-07-30 00:44:44Z   User: JasonO 
*  Bug Fix: When hitting back button, need to at least put view into valid 
*  state. TODO: add logic to dynamically set view. Need a clean entry point. 
*  
*  Revision: 8   Date: 2010-07-30 00:20:36Z   User: nicka 
*  event list overlay fade in and out in ajax thread 
*  
*  Revision: 7   Date: 2010-07-29 22:35:25Z   User: JasonO 
*  
*  Revision: 6   Date: 2010-07-29 18:31:32Z   User: JasonO 
*  Updating formatting of calendar view event time/summary and adding 
*  functionality to condense events in tag cloud view based on profile id. 
*  
*  Revision: 5   Date: 2010-07-26 21:44:12Z   User: JasonO 
*  Adding ability to dynamically construct event details page url on the 
*  server based on new module setting. 
*  
*  Revision: 4   Date: 2010-07-26 15:36:40Z   User: nicka 
*  Added fadeIn/fadeOut support for List view. 
*  
*  Revision: 3   Date: 2010-07-22 23:38:14Z   User: nicka 
*  Added support for the Event List View module 
*  
*  Revision: 2   Date: 2010-07-22 20:29:10Z   User: JasonO 
*  Adding more calendar view support 
*  
*  Revision: 1   Date: 2010-07-22 15:33:53Z   User: JasonO 
**********************************************************************/

var eventProfiles = [];
var currentDate = new Date();
//var fakeCampus = { campusID: -1 };

$(function ()
{
    $("#event-calendar").fadeIn("fast");
    calendarParams.campus = getCampusForCalendar();

    calendarParams.topicIDs = $("#ihTopicAreas").val().trim() != '' ? $("#ihTopicAreas").val() : '';
    initEventCalendar(calendarParams);

    $(document).bind("CALENDAR_INFO_CHANGED", function (e, data)
    {
        switch (currentView.toLowerCase().trim())
        {
            case "cloud":
                updateCloudView(e, data);
                break;
            case "list":
                updateEventListView(e, data);
                break;
            case "calendar":
            default:
                updateCalendarView(e, data);
                break;
        }

        return false;
    });

    $(document).bind("CALENDAR_VIEW_CHANGED", function ()
    {
        loadView();
        return false;
    });
});

function loadView()
{
	clearViews();

	switch (currentView.toLowerCase().trim())
	{
		case "calendar":
			$("#event-calendar").fullCalendar("destroy");
			$("#event-calendar").fadeIn("fast");
			initEventCalendar(calendarParams);
			break;
		case "cloud":
			initEventCloud(calendarParams);
			$("#event-cloud").fadeIn("fast");
			break;
		case "list":
			initEventList(calendarParams);
			$("#event-list-wrapper").fadeIn("fast");
			break;
	}
}

function clearViews()
{
    $("html, body").animate({ scrollTop: 0 }, 1000, "easeOutCubic");
	getOverlay(currentView).css({ height: $(".event-view:visible").outerHeight() + 30 + "px" }).fadeIn("fast");
	$(".event-view").fadeOut("fast");
}

function getOverlay(view)
{
    switch (view.trim())
    {
        case "cloud":
            return $("#cloud-overlay");
        case "list":
            return $("#event-list-overlay");
        case "calendar":
        default:
            return $("#calendar-overlay");
    }
}

function toolTipOver(element, event, className)
{
    var div = element.children(className);

    if (div.is(":hidden"))
    {
        var offset = element.offset();
        div.css({
            left: event.pageX - offset.left + 20 + "px",
            top: event.pageY - offset.top + "px"
        }).fadeIn("fast");
    }
}

function toolTipOut(element, className)
{
    var div = element.children(className);

    if (div.is(":visible"))
    {
        element.children(className).fadeOut("fast");
    }
}

function toolTipMove(element, event, className)
{
    var div = element.children(className);

    if (div.is(":visible"))
    {
        var offset = element.offset();
        div.css({
            left: event.pageX - offset.left + 20 + "px",
            top: event.pageY - offset.top + "px"
        })
    }
}

function getEventTimeString(time)
{
    var timeString = time.toTimeString();

    var timeParts = timeString.split(':');
    var hours = timeParts[0];
    var minutes = timeParts[1];
    var isAM = true;

    if (parseInt(hours) > 12)
    {
        isAM = false;
        hours = hours - 12;

    }

    if (hours.length > 1 && hours[0] == 0)
    {
        hours = hours[1];
    }

    var theTime = hours + ":" + minutes;
    theTime += isAM ? " AM" : " PM";
    return theTime;
}

// Calendar //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function initEventCalendar(params)
{
	$("#event-calendar").fullCalendar({
		viewDisplay: bindCalendarViewEvents,
		events: function (start, end, callback)
		{
			var pageID = $("#ihCalendarEventDetails").val();
			//alert("start: " + start.toDateString() + "\nend: " + end.toDateString() + "\nkeywords: " + params.keywords + "\ncampusID: " + params.campus.campusID);
			postAsyncJson("webservices/custom/cccev/web2/eventsservice.asmx/GetEventList",
					'{ "start":"' + start.toDateString() + '", "end":"' + end.toDateString() + '", "keywords":"' + params.keywords + '", "topicIDs":"' + params.topicIDs + '", "campusID":' + params.campus.campusID + ', "pageID": ' + pageID + ' }',
					function (result)
					{
						eventProfiles = result.d;
						callback(eventProfiles);
						bindCalendarViewEvents();
						return false;
					},
					function (result, errorText, thrownError)
					{
						//alert(result.responseText);
						return false;
					});
		},
		loading: function (isLoading, view)
		{
			if (isLoading)
			{
				$("#calendar-overlay").css({ height: $("#event-calendar").outerHeight() + 20 + "px" }).fadeIn("fast");
			}
			else
			{
				$("#calendar-overlay").fadeOut("fast");
			}

			return false;
		}
	});

	if (currentDate.getMonth() != new Date().getMonth())
	{
		// will not render current month's events if called on page load
		$("#event-calendar").fullCalendar('gotoDate', currentDate);
	}
}

function updateCalendarView(e, data)
{
	var view = $("#event-calendar").fullCalendar("getView");
	var sDate = view.start;

	if (sDate.getFullYear < 1900)
	{
		sDate = new Date(sDate.getFullYear() + 1900, sDate.getMonth(), sDate.getDate());
	}

	currentDate = sDate;

	$("#event-calendar").fullCalendar("destroy");
	initEventCalendar(data);
	return false;
}

function bindCalendarViewEvents(view)
{
	$(document).trigger("CONTENT_RENDERED");

	$(".fc-event").hoverIntent(
	function (event)
	{
	    toolTipOver($(this), event, ".fc-event-description");
	    return false;
	},
	function ()
	{
	    toolTipOut($(this), ".fc-event-description");
	    return false;
	})
	.mousemove(function (event)
	{
	    toolTipMove($(this), event, ".fc-event-description");
	    return false;
	});
}

// Tag Cloud //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function getCondensedEvents(profiles)
{
	var condensedEvents = [];

	for (var i = 0; i < profiles.length; i++)
	{
		var hasMatch = false;
		var event = profiles[i];

		for (var j = 0; j < condensedEvents.length; j++)
		{
			if (event.id == condensedEvents[j].id)
			{
				hasMatch = true;
				break;
			}
		}

		if (!hasMatch)
		{
		    var date = new Date(event.start);
		    event.time = getMonth(date) + " " + getDayOfMonth(date) + ", " + getEventTimeString(date);
			condensedEvents.push(event);
		}
	}

	return condensedEvents;
}

function initEventCloud(params)
{
	var pageID = $("#ihCloudEventDetails").val();
	postAsyncJson("webservices/custom/cccev/web2/eventsservice.asmx/GetAlphabeticalEventList",
		'{ "start":"' + params.start.toDateString() + '", "end":"' + params.end.toDateString() + '", "keywords":"' + params.keywords + '", "topicIDs":"' + params.topicIDs + '", "campusID":' + params.campus.campusID + ', "pageID": ' + pageID + ' }',
		function (result)
		{
		    eventProfiles = result.d;
		    var condensed = getCondensedEvents(eventProfiles);
		    $("#event-cloud ul").children().remove();
		    $("#cloud-item").render(condensed).appendTo("#event-cloud ul");
		    $("#event-cloud .campus").html(calendarParams.campus.name);
		    bindCloudEvents();
		    $("#cloud-overlay").fadeOut("fast");
		    return false;
		},
		function (result, errorText, thrownError)
		{
		    alert(result.responseText);
		    return false;
		});
}

function updateCloudView(e, data)
{
    $("#cloud-overlay").fadeIn("fast");
	initEventCloud(data);
	return false;
}

function bindCloudEvents()
{
    $(".tag").hoverIntent(
	function (event)
	{
	    toolTipOver($(this), event, ".tag-description");
	    return false;
	},
	function ()
	{
	    toolTipOut($(this), ".tag-description");
	    return false;
	})
	.mousemove(function (event)
	{
	    toolTipMove($(this), event, ".tag-description");
	    return false;
	});
}

// Event List //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

var MONTHS = new Array("Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec");

function initEventList(params)
{
	$("#event-list-overlay").height($("#event-list-overlay").parent().height() + 200);
	$("#event-list-overlay").fadeIn("fast");

	var pageID = $("#ihListEventDetails").val();
	postAsyncJson("webservices/custom/cccev/web2/eventsservice.asmx/GetEventList",
	'{ "start":"' + params.start.toDateString() + '", "end":"' + params.end.toDateString() + '", "keywords":"' + params.keywords + '", "topicIDs":"' + params.topicIDs + '", "campusID":' + params.campus.campusID + ', "pageID": ' + pageID + ' }',
	function (result)
	{
		eventProfiles = result.d;
		renderEventList(eventProfiles);
		//$("#calendar-overlay").fadeOut("fast");
		$("#event-list-overlay").fadeOut("fast");
		return false;
	},
	function (result, errorText, thrownError)
	{
		alert(result.responseText);
		return false;
	});
}

function updateEventListView(e, data)
{
	initEventList(data);
	
	return false;
}

function renderEventList(eventProfiles)
{
	$("#event-featured-list").children().remove();
	$("#event-list").children().remove();

	if (eventProfiles === undefined || eventProfiles.length == 0)
	{
		return false;
	}

	// Post process json data to match what template needs.
	// Here we will use the "start" item and from it create a "date" and "month"
	for (var i = 0; i < eventProfiles.length; i++)
	{
		eventProfiles[i].date = getDayOfMonth(new Date(eventProfiles[i].start));
		eventProfiles[i].month = getMonth(new Date(eventProfiles[i].start));
	}

	// For featured items, only show the first occurrence of the event
	var featuredEvents = getCondensedEvents(eventProfiles);
	var maxItems = 4;
	if (featuredEvents.length < maxItems)
	{
		maxItems = featuredEvents.length;
	}
	// ...also never show only 3 items.
	maxItems = (maxItems == 3) ? 2 : maxItems;

	featuredEvents = featuredEvents.slice(0, maxItems);

	// Render featured items
	$("#event-featured-list-template").render(featuredEvents).appendTo("#event-featured-list");

	// Render normal items
	$("#event-list-template").render(eventProfiles).appendTo("#event-list");
	return false;
}

function getDayOfMonth(aDate)
{
	var day = aDate.getDate();
	if (day < 10)
	{
		day = "0" + day;
	}
	return day;
}

function getMonth(aDate)
{
	return MONTHS[aDate.getMonth()];
}