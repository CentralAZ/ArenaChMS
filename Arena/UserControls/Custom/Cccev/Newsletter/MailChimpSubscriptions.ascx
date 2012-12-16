<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MailChimpSubscriptions.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.Newsletter.MailChimpSubscriptions" %>
<%@ Register TagPrefix="Arena" Namespace="Arena.Portal.UI" Assembly="Arena.Portal.UI" %>
<link href="UserControls/Custom/Cccev/Newsletter/css/mailchimpsubscriptions.css" type="text/css" rel="stylesheet"/>    
<asp:ScriptManagerProxy ID="smpScripts" runat="server">
    <Scripts>
        <asp:ScriptReference Path="~/UserControls/Custom/Cccev/Newsletter/js/effects.js" />
    </Scripts>
</asp:ScriptManagerProxy>
<script type="text/javascript">
    var requestManager = Sys.WebForms.PageRequestManager.getInstance();

    requestManager.add_endRequest(function()
    {
    	initEffectEvents();
    });
</script>
<asp:UpdatePanel ID="upSubscriptions" runat="server" UpdateMode="Always">
<ContentTemplate>
<div class="CccevMailChimp">
<ul>
<asp:Repeater ID="dlLists" runat="server" OnItemDataBound="dlLists_ItemDataBound" OnItemCommand="dlLists_ItemCommand">
	<ItemTemplate>
		<li runat="server" id="liItem">
		<div class="itemContainer">
			<asp:Image runat="server" id="imgBG" ImageUrl="~/UserControls/Custom/Cccev/Newsletter/images/default_list_bg.png" />
			<asp:ImageButton ID="btnSubscribe" class="buttonSubscribe" runat="server" ImageUrl="~/UserControls/Custom/Cccev/Newsletter/images/null.gif" Visible="false" />
			<asp:ImageButton ID="btnUnsubscribe" class="buttonUnsubscribe" runat="server" ImageUrl="~/UserControls/Custom/Cccev/Newsletter/images/null.gif" Visible="false" />
			<span><asp:Literal runat="server" ID="liListName"></asp:Literal></span>
		</div>
		<asp:CheckBoxList ID="cblGroups" runat="server" CellPadding="0" CellSpacing="0" Visible="false"></asp:CheckBoxList>
		<asp:LinkButton ID="lbSaveGroups" class="buttonSaveGroups" runat="server" Text="save"></asp:LinkButton>
		</li>
	</ItemTemplate>
</asp:Repeater>
</ul>
<span runat="server" class="fadingMessage" id="lblMsg" visible="false"></span>
</div>
</ContentTemplate>
</asp:UpdatePanel>