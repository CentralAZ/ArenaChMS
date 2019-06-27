<%@ Control Language="C#" CodeFile="CampusPicker.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.Web.CampusPicker" %>
<asp:ScriptManagerProxy ID="smpScripts" runat="server" />
<div class="campuses">
<% if (!string.IsNullOrEmpty(HeadingSetting))
   { %>
   <h1><%= HeadingSetting %></h1>
<% } %>
<% if (shouldWrapInList)
   { %>
       <ul id="campus-picker">
    <% foreach (var c in campuses)
       {
       var id = "campus_" + c.Qualifier; %>
       <li id="<%= id %>">
           <input type="radio" id="rb_<%= id %>" name="campus" class="campus" data-id="<%= c.Qualifier %>" />
           <label for="rb_<%= id %>"><%= c.Value %></label>
       </li>
    <% } %>
    </ul>
<% }
   else
   { %>
    <div id="campus-picker">
     <% foreach (var c in campuses)
        { 
            var id = "campus_" + c.Qualifier; %>
            <input type="radio" id="rb_<%= id %>" name="campus" class="campus" data-id="<%= c.Qualifier %>" />
            <label for="rb_<%= id %>"><%= c.Value %></label>
     <% } %>
     </div>
<% } %>
</div>
<asp:Literal ID="lCustomScripts" runat="server" Visible="false" />