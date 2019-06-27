<%@ Control Language="C#" CodeFile="FacebookConnectAccount.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.Web.FacebookConnectAccount" %>
<% if (CurrentPerson != null && !connectedToFacebook && !hasOptedOut) { %>
<input type="hidden" name="ihState" id="ihState" value="<%= state %>" runat="server" />
<div id="fb-root"></div>
<script type="text/javascript">
    $(function () {
        CentralAZ.Facebook.Helpers.Authentication.config = {
            appID: '<%= FacebookAppIDSetting %>',
            state: '<%= state %>'
        };

        CentralAZ.Facebook.Helpers.Authentication.initConnect({ name: '<%= CurrentPerson.FullName %>' });
    });
</script>
<% } %>