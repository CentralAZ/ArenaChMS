<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShowNextSiblingDetails.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.WebUtils.ShowNextSiblingDetails" %>
<asp:Panel ID="pnlPasscode" runat="server" DefaultButton="btnVerify">
<span class="smallText">Passcode:</span>
    <asp:TextBox ID="txtPasscode" runat="server" TextMode="Password"></asp:TextBox>
    <asp:Button CssClass="smallText" ID="btnVerify" runat="server" Text="Verify" 
        onclick="btnVerify_Click" /><br />
</asp:Panel>
<asp:Panel ID="pnlContent" runat="server" CssClass="content">
    <asp:PlaceHolder ID="phContent" runat="server"></asp:PlaceHolder>
</asp:Panel>