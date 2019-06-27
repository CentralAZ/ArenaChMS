<%@ Control Language="C#" AutoEventWireup="true" CodeFile="QuickSearch.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.WebUtils.QuickSearch" %>

<asp:ScriptManagerProxy ID="smpScripts" runat="server" />

<asp:Panel id="pnlQuickSearch" runat="server" DefaultButton="ibQuickSearch" class="subscribe">
    <asp:ImageButton ID="ibQuickSearch" runat="server" CssClass="searchBtn"  ImageUrl="/arena/Templates/Cccev/liger/images/btn-search.gif" ValidationGroup="quickSearch" TabIndex="999" />
    <div><asp:TextBox ID="tbQuickSearch" runat="server" Text="SEARCH" ValidationGroup="quickSearch" /></div>
</asp:Panel>