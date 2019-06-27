<%@ Control Language="C#" CodeFile="CascadeTagPermissions.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.Security.CascadeTagPermissions" %>
<%@ Register TagPrefix="Arena" Namespace="Arena.Portal.UI" Assembly="Arena.Portal.UI" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<asp:ScriptManagerProxy ID="smpScripts" runat="server" />

<style type="text/css">
.ui-tabs .ui-tabs-hide {
     display: none;
}
.ui-widget input, .ui-widget select, .ui-widget textarea, .ui-widget button {
	font-size: 10px;
}
</style>


<asp:Panel ID="pnlCheckInstall" runat="server" Visible="false">
	<span class="smallText">One or more of the required stored procedures (<%= string.Join( ", ", sProcs ) %>) does not exist.
	<p>Would you like to download them from '<%= sProcURL %>'</p></span>
	<asp:button ID="btnDownload" runat="server" Text="Download" OnClick="btnDownload_Click" CssClass="smallText"/>
</asp:Panel>

<asp:Panel ID="pnlPerformInstall" runat="server" Visible="False">
	<asp:Label ID="lblStatus" runat="server" CssClass="errorText"></asp:Label>
	<br />
	<asp:TextBox ID="txtOutput"  CssClass="smallText" runat="server" Height="194px" TextMode="MultiLine" Width="750px"></asp:TextBox>
	<br />
	<asp:Button ID="btnInstall" runat="server" CssClass="smallText" onclick="btnInstall_Click" Text="Install" Visible="true"/>
	<asp:Button ID="btnContinue" runat="server" CssClass="smallText" onclick="btnContinue_Click" Text="Continue" Visible="false"/>
</asp:Panel>
<Arena:ModalPopup ID="mpSelectGroup" runat="server" CancelControlID="btnCancelGroupSelect" OkControlID="btnDone"
						Title="Select Group" DefaultFocusControlID="tvGroups">
	<Content>
		<div style="border:1px solid Gray; height:335px; overflow:auto; width:300px;">
			<ComponentArt:TreeView ID="tvGroups" DragAndDropEnabled="false" NodeEditingEnabled="false"
				KeyboardEnabled="true" CssClass="formItem" NodeCssClass="formItem" LineImageWidth="19"
				LineImageHeight="20" DefaultImageWidth="16" DefaultImageHeight="16" ItemSpacing="0"
				NodeLabelPadding="3" ParentNodeImageUrl="" LeafNodeImageUrl="" ShowLines="true"
				LineImagesFolderUrl="~/include/ComponentArt/TreeView/images/lines" EnableViewState="true"
				MultipleSelectEnabled="false" ClientSideOnNodeCheckChanged="groupCheckAll" runat="server">
			</ComponentArt:TreeView>
		</div>
	</Content>
	<Buttons>
		<asp:Button runat="server" ID="btnDone" CssClass="smallText" Text="Done" CausesValidation="False" />
		<asp:Button runat="server" ID="btnCancelGroupSelect" CssClass="smallText" Text="Cancel" CausesValidation="false" />
	</Buttons>
</Arena:ModalPopup>


<Arena:ModalPopup ID="mpSelectMetric" runat="server" CancelControlID="btnCancelMetricSelect" OkControlID="btnMetricDone"
						Title="Select Metric" DefaultFocusControlID="tvMetrics">
	<Content>
		<div style="border:1px solid Gray; height:335px; overflow:auto; width:300px;">
			<ComponentArt:TreeView ID="tvMetrics" DragAndDropEnabled="false" NodeEditingEnabled="false"
				KeyboardEnabled="true" CssClass="formItem" NodeCssClass="formItem" LineImageWidth="19"
				LineImageHeight="20" DefaultImageWidth="16" DefaultImageHeight="16" ItemSpacing="0"
				NodeLabelPadding="3" ParentNodeImageUrl="" LeafNodeImageUrl="" ShowLines="true"
				LineImagesFolderUrl="~/include/ComponentArt/TreeView/images/lines" EnableViewState="true"
				MultipleSelectEnabled="false" ClientSideOnNodeCheckChanged="metricCheckAll" runat="server">
			</ComponentArt:TreeView>
		</div>
	</Content>
	<Buttons>
		<asp:Button runat="server" ID="btnMetricDone" CssClass="smallText" Text="Done" CausesValidation="False" />
		<asp:Button runat="server" ID="btnCancelMetricSelect" CssClass="smallText" Text="Cancel" CausesValidation="false" />
	</Buttons>
</Arena:ModalPopup>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
	<ContentTemplate>
		
	<asp:HiddenField id="hfSelectedTab" runat="server" />
	<asp:Panel ID="pnlPermissions" runat="server" Visible="False">
		<div ID="divMsg" runat="server" class="ui-state-highlight ui-corner-all" style="margin-top: 10px; padding: 10px 0.7em; max-height: 80px; overflow: auto">
			<asp:Label ID="lblPermissionStatus" runat="server" CssClass="errorText" Text="Select a Tag"></asp:Label>
		</div>
		<br />

		<div id="tabs" class="ui-tabs">
			<ul>
				<li><a href="#tags-tab"><span>Tags</span></a></li>
				<li><a href="#groups-tab"><span>Groups</span></a></li>
				<li><a href="#metrics-tab"><span>Metrics</span></a></li>
			</ul>
			
			<div id="tags-tab" class="ui-tabs-hide">
				<asp:Label ID="lblInstructions" runat="server" CssClass="smallText" Text="Select a parent <b>tag</b> and press the Cascade button to propogate the permissions down to all children."></asp:Label>
				<br /><br />
				<span class="formLabel">Parent Tag: </span><Arena:ProfilePicker ID="aProfilePicker" runat="server" CssClass="formItem"></Arena:ProfilePicker>
				<br /><br />
				<asp:Button ID="btnCopyTagPermissions" runat="server" CssClass="smallText" Text="Cascade Permissions" onclick="btnCopyTagPermissions_Click" />
			</div>
			
			<div id="groups-tab" class="ui-tabs-hide">
				
				<asp:Label ID="Label1" runat="server" CssClass="smallText" Text="Select a parent <b>group</b> and press the Cascade button to propogate the permissions down to all children."></asp:Label>
				<br /><br />
				<span class="formLabel">Parent Group: </span> <asp:Label ID="lblSelectedGroup" runat="server" CssClass="smallText" Text="(not set)"></asp:Label> <asp:Button runat="server" ID="btnSelectGroup" CssClass="smallButton" Text="..." OnClientClick="showModalGroupPicker(); return false;" CausesValidation="False" />
				<br /><br />
				<asp:Button ID="btnCopyGroupPermissions" runat="server" CssClass="smallText" Text="Cascade Permissions" onclick="btnCopyGroupPermissions_Click" />
			</div>
			
			<div id="metrics-tab" class="ui-tabs-hide">
				<asp:Label ID="Label2" runat="server" CssClass="smallText" Text="Select a parent <b>metric</b> and press the Cascade button to propogate the permissions down to all children."></asp:Label>
				<br /><br />
				<span class="formLabel">Parent Metric: </span> <asp:Label ID="lblSelectedMetric" runat="server" CssClass="smallText" Text="(not set)"></asp:Label> <asp:Button runat="server" ID="btnSelectMetric" CssClass="smallButton" Text="..." OnClientClick="showModalMetricPicker(); return false;" CausesValidation="False" />
				<br /><br />
				<asp:Button ID="btnCopyMetricPermissions" runat="server" CssClass="smallText" Text="Cascade Permissions" onclick="btnCopyMetricPermissions_Click" />
			</div>
		</div>
	</asp:Panel>
</ContentTemplate>
</asp:UpdatePanel>


<script type="text/javascript">

	Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function()
	{
		init();
	});

	function init()
	{
		$(document).ready(function()
		{
			$("#tabs").tabs();

			var selectedIndex = $("[id$=hfSelectedTab]").val();
			if (selectedIndex != "")
			{
				$("#tabs").tabs('select', parseInt(selectedIndex));
			}
		});

		var message = $get("<%= divMsg.ClientID %>");
		if (message && !$('div[id$=divMsg]').hasClass("errorText"))
		{
			window.setTimeout("fadeAndClear()", 5000);
		}
	}

	init();

	function groupCheckAll(node)
	{
		if (node.Checked == true)
		{
			//node.CheckAll();
			tvGroups.UnCheckAll();
			node.set_checked(true);
			$("[id$=lblSelectedGroup]").html(node.Text);
			$("[id$=lblSelectedGroup]").css("color", "black");
		}
		else
		{
			node.UnCheckAll();
			$("[id$=lblSelectedGroup]").html("(not set)");
			$("[id$=lblSelectedGroup]").css("color", "#A0A0A0");
		}
		node.ParentTreeView.Render();
		return true;
	}

	function metricCheckAll(node)
    {
        if (node.Checked == true)
        {
	        node.CheckAll();
	        tvMetrics.UnCheckAll();
	        node.set_checked(true);
	        $("[id$=lblSelectedMetric]").html(node.Text);
	        $("[id$=lblSelectedMetric]").css("color", "black");
	    }
	    else
        {
            node.UnCheckAll();
	        $("[id$=lblSelectedMetric]").html("(not set)");
	        $("[id$=lblSelectedMetric]").css("color", "#A0A0A0");
	    }
	    node.ParentTreeView.Render();
	    return true;
	}

	function showModalGroupPicker()
	{
	    var modal = $find('<%= mpSelectGroup.ClientID %>');
	    showModal(modal);
	}

	function showModalMetricPicker()
	{
	    var modal = $find('<%= mpSelectMetric.ClientID %>');
	    showModal(modal);
	}

	function showModal( modal )
	{        
		if (modal)
		{
			var selected = $("#tabs").tabs('option', 'selected');
			$("[id$=hfSelectedTab]").val(selected);
			modal.show();
		}
	}

	function fadeAndClear()
	{
		$('div[id$=divMsg]').fadeOut('slow');
	}
</script>
