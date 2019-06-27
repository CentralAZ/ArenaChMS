/// <reference path="/Include/scripts/jquery-1.3.2.vsdoc.js" />

var calendarParams = {
    start: new Date(),
    end: new Date(),
    keywords: '',
    topicIDs: '',
    campus: null
};

var currentView= new String();

$(function ()
{
    // Set initial date values
    var theStartDate = new Date();
    calendarParams.start = theStartDate;
    $(".min").text(getShortDateString(theStartDate.toDateString()));
    var theEndDate = new Date();
    theEndDate.setDate(theEndDate.getDate() + 90);
    calendarParams.end = theEndDate;
    $(".max").text(getShortDateString(theEndDate.toDateString()));
    calendarParams.campus = getCampusForCalendar();

    $("input[name='calendar-view']:first").attr("checked", "checked");
    $(".calendar-search").val("");

    $("#date-slider").slider({
        range: true,
        min: 0,
        max: 365,
        step: 7,
        values: [0, 90],
        slide: setRange
    });

    $(document).bind("slidechange", function (event, ui)
    {
        calendarParams.campus = getCampusForCalendar();
        $(document).trigger("CALENDAR_INFO_CHANGED", calendarParams);
        return false;
    });

    $(".calendar-search").change(function ()
    {
        calendarParams.campus = getCampusForCalendar();
        calendarParams.keywords = $(".calendar-search").val();
        $(document).trigger("CALENDAR_INFO_CHANGED", calendarParams);
        $(document).unbind("submit").submit(function () { return false; });
        return false;
    });

    $(".calendar-search").keypress(function (event)
    {
        if (event.which == 13)
        {
            $(".calendar-search").change();
        }

        return true;
    });

    $("input[name='calendar-view']").click(function ()
    {
        currentView = $(this).attr("rel");
        $(document).trigger("CALENDAR_VIEW_CHANGED");
        return true;
    });

    $(document).bind("CAMPUS_UPDATED", function ()
    {
        calendarParams.campus = getCampusForCalendar();
        $(document).trigger("CALENDAR_INFO_CHANGED", calendarParams);
        return false;
    });

    $(document).bind("CALENDAR_VIEW_CHANGED", function ()
    {
        if (currentView.toLowerCase() == "calendar")
        {
            $("#slider-wrap").slideUp();
        }
        else
        {
            $("#slider-wrap").slideDown();
        }

        return false;
    });
});

function setRange(event, ui)
{
    var startDate = new Date();
    startDate.setDate(startDate.getDate() + ui.values[0]);
    calendarParams.start = startDate;

    var endDate = new Date();
    endDate.setDate(endDate.getDate() + ui.values[1]);
    calendarParams.end = endDate;

    $(".min").text(getShortDateString(startDate.toDateString()));
    $(".max").text(getShortDateString(endDate.toDateString()));
}

function getShortDateString(dateString)
{
    var today = new Date();
    var date = new Date(Date.parse(dateString));

    if ((today.getMonth() == date.getMonth()) && (today.getDate() == date.getDate()))
    {
        return "Today";
    }

    return dateString.substring(dateString.indexOf(" ") + 1, dateString.length - 4);
}

function getCampusForCalendar()
{
    return campus != null ? campus : { campusID: -1, name: "All Campuses" };
}