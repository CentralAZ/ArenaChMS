<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ContributionStatementPreference.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.WebUtils.ContributionStatementPreference" %>

<asp:ScriptManagerProxy ID="smpScripts" runat="server" />
<style type="text/css">
.info, .success, .warning, .error, .validation {
	border: 1px solid;
	margin: 10px 0px;
	padding:15px 10px 15px 50px;
	background-repeat: no-repeat;
	background-position: 10px center;
	width: 50%;
}
.info {
	color: #00529B;
	background-color: #BDE5F8;
	background-image: url('images/custom/cccev/knob_info.png');
}
.success {
	color: #4F8A10;
	background-color: #DFF2BF;
	background-image:url('images/custom/cccev/knob_success.png');
}
.warning {
	color: #9F6000;
	background-color: #FEEFB3;
	background-image: url('images/custom/cccev/knob_warning.png');
}
.error {
	color: #D8000C;
	background-color: #FFBABA;
	background-image: url('images/custom/cccev/knob_error.png');
}
</style>

<asp:UpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Always">
	<ContentTemplate>
		<asp:Panel runat="server" ID="pnlPreferences">
			<asp:RadioButtonList ID="rblPreference" runat="server" OnSelectedIndexChanged="rblPreference_CheckedChanged" AutoPostBack="true"/>
		</asp:Panel>
		<asp:Panel runat="server" ID="pnlSuccess" Visible="false"><div class="success">Your preference was saved.</div></asp:Panel>
		<asp:Panel runat="server" ID="pnlFail" Visible="false"><div class="error">Oops.  Something went wrong and your preference was not saved. If it happens again please let us know.</div></asp:Panel>
	</ContentTemplate>
</asp:UpdatePanel>

<script language="javascript" type="text/javascript">
	$(document).ready(function ()
	{
		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function ()
		{
			csp_initEffects();
		});
	});

	function csp_initEffects()
	{
		var message = $get("<%= pnlSuccess.ClientID %>");

		if (message)
		{
			window.setTimeout("csp_fadeAndClear()", 5000);
		}
	}

	function csp_fadeAndClear()
	{
		$('[id$=pnlSuccess]').fadeOut('slow');
	}
</script>