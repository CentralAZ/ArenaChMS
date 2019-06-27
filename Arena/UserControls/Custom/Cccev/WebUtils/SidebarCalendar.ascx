<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SidebarCalendar.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.WebUtils.SidebarCalendar" %>

<asp:ScriptManagerProxy ID="smpScripts" runat="server" />

<script type="text/javascript">
    var requestManager = Sys.WebForms.PageRequestManager.getInstance();

    requestManager.add_beginRequest(function()
    {
        if (calClick)
        {
            showLoaders();
        }
    });

    requestManager.add_endRequest(function()
    {
        initPopovers();
        initEvents();
        initCalendar();
        hideLoaders();
    });
</script>

<div class="calendar">
    <asp:UpdatePanel ID="upDate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
        
            <h2>Calendar</h2>
            <div class="date-holder">
                <asp:Label ID="lblDay" runat="server" CssClass="title-day" />
                <asp:Label ID="lblDate" runat="server" CssClass="title-date" />
            </div>
        
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="calDates" EventName="SelectionChanged" />
        </Triggers>
    </asp:UpdatePanel>
    
    <asp:UpdatePanel ID="upCalendar" runat="server" UpdateMode="Always">
        <ContentTemplate>
            
            <div class="calendar-overlay"></div>
            <div class="calendar-holder">
                <asp:Calendar ID="calDates" runat="server" DayNameFormat="FirstLetter" SelectionMode="Day" 
                    TitleStyle-BackColor="#BBD1CA" NextPrevStyle-ForeColor="#DFE89F">
                    <DayStyle CssClass="calendar-day" />
                    <TodayDayStyle CssClass="calendar-today" />
                    <SelectedDayStyle CssClass="calendar-selected" />
                    <OtherMonthDayStyle CssClass="calendar-last-month" />
                    <DayHeaderStyle CssClass="calendar-day-header" />
                    <NextPrevStyle CssClass="calendar-next-prev" />
                    <TitleStyle CssClass="calendar-title" />
                </asp:Calendar>
                <div class="calendar-footer"></div>
            </div>
            
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <asp:UpdatePanel ID="upEvents" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            
            <h2>Events</h2>
            <div class="events-overlay"></div>
            <div class="events">
                <div class="top-box">
                    <asp:Label ID="lblEventMonthYear" runat="server" />
                    <asp:PlaceHolder ID="phSlideControls" runat="server" />
                </div>
                <div class="cover">
                    <ul>
                        <asp:PlaceHolder ID="phEvents" runat="server" />
                    </ul>
                </div>
            </div>
            
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="calDates" EventName="SelectionChanged" />
        </Triggers>
    </asp:UpdatePanel>
</div>
