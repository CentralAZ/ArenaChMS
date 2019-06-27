<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NavBar2.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.WebUtils.NavBar2" %>
<%@ Register TagPrefix="radP" Namespace="Telerik.WebControls" Assembly="RadPanelbar.Net2" %>
<script type="text/javascript">
function OnLoad(sender, args)
{
   var panelbar = <%= RadPanelbar2.ClientID %>;
   for (var i = 0; i < panelbar.AllItems.length; i++)
   {
       var panelItem = panelbar.AllItems[i];
       if (panelItem.ImageElement)
       {
           panelItem.ImageElement.AssociatedItem = panelItem;
           panelItem.ImageElement.onclick = function (e)
           {
               if (!e) e = window.event;
               if (this.AssociatedItem.Expanded)
               {
					this.AssociatedItem.ImageUrl = "UserControls/Custom/Cccev/WebUtils/images/collapsed2.gif";
					this.AssociatedItem.Collapse();
               }
               else
               {
					this.AssociatedItem.ImageUrl = "UserControls/Custom/Cccev/WebUtils/images/expanded2.gif";
					this.AssociatedItem.Expand();
               }
               e.cancelBubble = true;
               if (e.stopPropagation)
               {
                   e.stopPropagation();
               }
               return false;
           }
       }
   }
}
</script>

<radP:RadPanelbar ID="RadPanelbar2" runat="server" OnClientLoad="OnLoad">

</radP:RadPanelbar>