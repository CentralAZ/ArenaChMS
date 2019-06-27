<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SearchConnector.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.WebUtils.SearchConnector" %>

<asp:Panel id="pnlQuickSearch" runat="server" DefaultButton="ibGo">
    <asp:ImageButton ID="ibGo" runat="server" OnClick="btnGo_Click" />
    <asp:TextBox ID="txtSearch" runat="server" />
</asp:Panel>

<div class="searchResults">
	<div id="divHeader" runat="server" class="searchResultSummary"></div>
	<div id="divNoResults" runat="server" class="searchResultNone" visible="false">
		No results matching your search were found.
		<ul>
		   <li>Check your spelling. Are the words in your query spelled correctly?</li>
		   <li>Try using synonyms. Maybe what you're looking for uses slightly different words.</li>
		   <li>Make your search more general. Try more general terms in place of specific ones.</li>
		</ul>
	</div>

	<div id="divErrorMessage" runat="server" class="searchResultError" visible="false">Unable to perform search at this time</div>
	
	<asp:DataGrid ID="dgSearchResults" runat="server" AlternatingItemStyle-CssClass="alt-row" BorderWidth="0" GridLines="none" AutoGenerateColumns="False" AllowPaging="true" PageSize="10" OnPageIndexChanged="dgSearchResults_PageIndexChanged" Width="100%">
		<Columns> 
			<asp:TemplateColumn> 
				<ItemTemplate> 
					<div class="searchResult">
						<div class="searchResultTitle"><a href="<%# DataBinder.Eval(Container.DataItem, "Path") %>"><%# HighlightKeywords( DataBinder.Eval( Container.DataItem, "Title" ) )%></a></div>
						<div class="searchResultDescription"><%# HighlightKeywords( DataBinder.Eval( Container.DataItem, "HitHighlightedSummary" ) ) %></div>
						<div class="searchResultMeta">
							<span class="searchResultLink"><a href="<%# DataBinder.Eval(Container.DataItem, "Path") %>"><%# DataBinder.Eval(Container.DataItem, "Path") %></a></span> - <%# FormatSize( DataBinder.Eval(Container.DataItem, "Size")) %>
						</div>
					</div>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
		<PagerStyle HorizontalAlign="Center" Mode="NumericPages" CssClass="searchResultsPager" />
	</asp:DataGrid>
</div>