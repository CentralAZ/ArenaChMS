<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SearchForm.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.WebUtils.SearchForm" %>
<asp:TextBox ID="txtString" CssClass="smallText" runat="server"></asp:TextBox>
<asp:Button
	ID="btnQuickSearch" runat="server" CssClass="smallText" Text="Search" 
	onclick="btnQuickSearch_Click" UseSubmitBehavior="False" />