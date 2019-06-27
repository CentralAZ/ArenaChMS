<%@ Control Language="C#" CodeFile="LoginLogout.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.WebUtils.LoginLogout" %>
<asp:ScriptManagerProxy ID="smpScripts" runat="server" />

<script type="text/javascript">
    var requestManager = Sys.WebForms.PageRequestManager.getInstance();

    requestManager.add_endRequest(function() {
        initCustomForms();
        initPopovers();
    });
</script>

<%
    if (Request.IsAuthenticated)
    {
%>
        <li>
            <span class="nameWrap"><%= CurrentPerson.FullName %></span>
        </li>
<%
    }
%>
    <li>
        <span class="logWrap" runat="server" id="Span1">
            <asp:LinkButton ID="lbLog" runat="server" CausesValidation="False" Visible="False"
                CssClass="show-popup" />
            <asp:ImageButton ID="ibLog" runat="server" CausesValidation="False" Visible="False" />
        </span>
    </li>
<%
    if (!Request.IsAuthenticated && !RegisterPageSetting.Equals(string.Empty))
    {
%>
        <li><span class="logWrap" runat="server" id="regWrap">
            <asp:LinkButton ID="lbReg" runat="server" CausesValidation="False">Register</asp:LinkButton>
            <asp:ImageButton ID="ibReg" runat="server" CausesValidation="False" Visible="False">
            </asp:ImageButton>
        </span></li>
<%
    }
%>