<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FriendlyUrlMgmt.ascx.cs"
	Inherits="ArenaWeb.UserControls.Custom.Cccev.Core.FriendlyUrlMgmt" %>
<br />
<Arena:ArenaButton ID="btnShowHideFormDiv" runat="server" Text="Show Add/Edit Friendly URL Form"
	ToolTip="Toggle visibility of Add/Update Friendly URL Form" OnClick="btnShowHideFormDiv_Click" />
<br />
<br />
<div id="divAddEditRemoveForm" runat="server">
	<asp:Label ID="lblFriendlyUrlDdl" runat="server" Text="Friendly URL:" AssociatedControlID="ddlFriendlyUrls" />
	&nbsp;<asp:DropDownList ID="ddlFriendlyUrls" AutoPostBack="true" runat="server" AppendDataBoundItems="True"
		OnSelectedIndexChanged="ddlFriendlyUrls_SelectedIndexChanged">
	</asp:DropDownList>
	<asp:Label ID="lblOutput" runat="server"></asp:Label>
	<br />
	<hr />
	<table border="0" cellpadding="2" cellspacing="5">
		<tr>
			<td align="right">
				<asp:Label ID="Label1" runat="server" AssociatedControlID="FriendlyUrlName" Text="Friendly URL Name:"></asp:Label>
			</td>
			<td>
				<asp:TextBox ID="FriendlyUrlName" runat="server" Width="150px"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td align="right">
				<asp:Label ID="Label2" runat="server" AssociatedControlID="RedirectDestination" Text="Redirect Destination URL:"></asp:Label>
			</td>
			<td>
				<asp:TextBox ID="RedirectDestination" runat="server" Width="450px"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td>
				&nbsp;
			</td>
			<td>
				<asp:CheckBox ID="ExactDestination" runat="server" Checked="True" Text="Redirect all requests to exact destination (instead of relative to destination)" />
			</td>
		</tr>
		<tr>
			<td>
				&nbsp;
			</td>
			<td>
				<asp:CheckBox ID="ChildOnly" runat="server" Text="Only redirect requests to content in this directory (not subdirectories)" />
			</td>
		</tr>
		<tr>
			<td align="right">
				<asp:Label ID="Label3" runat="server" AssociatedControlID="ddlStatusCode" Text="Status Code:"></asp:Label>
			</td>
			<td>
				<asp:DropDownList ID="ddlStatusCode" runat="server">
					<asp:ListItem Value="Permanent">Permanent (301)</asp:ListItem>
					<asp:ListItem Value="Found">Found (302)</asp:ListItem>
					<asp:ListItem Value="Temporary">Temporary (307)</asp:ListItem>
				</asp:DropDownList>
				&nbsp;
			</td>
		</tr>
	</table>
	<br />
	<table border="0" cellpadding="5" cellspacing="10">
		<tr>
			<td>
				<asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
			</td>
			<td>
				<asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
			</td>
			<td>
				<asp:Button ID="btnDelete" runat="server" Text="Delete" OnClientClick="return confirm('Are you sure you want to delete the selected Friendly URL?')"
					OnClick="btnDelete_Click" />
			</td>
		</tr>
	</table>
	<br />
	<asp:Label ID="lblProcessReport" runat="server" Text=""></asp:Label>
</div>
<br />
<Arena:ArenaButton ID="btnFriendlyURLRequest2" Text="Create New Friendly URL Request"
	ToolTip="Only available from Central's internal network." runat="server" OnClick="btnFriendlyURLRequest_Click" />
<br />
<br />
<Arena:DataGrid ID="grdVirtualDirectoryList" runat="server" AutoGenerateColumns="False"
	DeleteEnabled="false" EnableModelValidation="True">
	<Columns>
		<asp:BoundColumn HeaderText="Friendly URL" DataField="FriendlyURL" HeaderStyle-Font-Bold="true" />
		<asp:HyperLinkColumn HeaderText="Destination" Target="_blank" DataTextField="Destination"
			DataNavigateUrlField="Destination" HeaderStyle-Font-Bold="true" DataNavigateUrlFormatString="{0}" />
		<asp:TemplateColumn HeaderText="Size(50-1000) - Get QR Code" HeaderStyle-Font-Bold="true" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
			<ItemTemplate>
				<asp:TextBox ID="txtQRCodeSize" runat="server" Width="50" />&nbsp;&nbsp;-&nbsp;&nbsp;<asp:Button
					CommandName="GetQRCode" ID="btnGetQRCode" Text="Get QR Code" runat="server" />
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</Arena:DataGrid>
<br />
<Arena:ArenaButton ID="btnFriendlyURLRequest" Text="Create New Friendly URL Request"
	ToolTip="Only available from Central's internal network." runat="server" OnClick="btnFriendlyURLRequest_Click" />
