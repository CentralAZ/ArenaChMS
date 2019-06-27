<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PromotionsViaWS.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.Web2.PromotionsViaWS" %>
<%@ Import Namespace="System.Linq" %>
<%@ Register TagPrefix="Arena" Namespace="Arena.Portal.UI" Assembly="Arena.Portal.UI" %>
<asp:ScriptManagerProxy ID="smpScripts" runat="server" />
<asp:PlaceHolder ID="phContent" runat="server"></asp:PlaceHolder>
<script type="text/javascript">

	/// Note: This module depends on Templates/Custom/CCCEV/Hasselhoff/campus-scripts.js for campus data.

	// This function will be injected by the module instance.
	$(function()
	{
		var campusID = campus != null ? campus.campusID : <%= DefaultCampusIDSetting %>;

		$("#<%= PlaceholderIDSetting %>" ).arenaPromotions({
			topicList:  "<%= TopicAreasSetting %>",
			areaFilter: "<%= AreaFilterSetting %>",
			campusID:   campusID,
			documentTypeID:         "<%= ThumbnailSetting %>",
			promotionDisplayPageID: <%= PromotionDisplayPageIDSetting %>,
			eventDetailPageID:      <%= EventDetailPageSetting %>,
			onSuccessCallback:      <%= OnSuccessCallbackSetting %>,
			updateOnCampusChange:   <%= Convert.ToString( UpdateOnCampusChangeSetting ).ToLower() %>,
			containerID:            "<%= ContainerIDSetting %>",
			maxItems:               <%= MaxItemsSetting %>,
			priorityLevel:          <%= PriorityLevelSetting %>,
			promotionTemplateID:    "<%= TemplateIDSetting %>"
		});
	});
	
	function defaultOnSuccessCallback(result)
	{
		// does nothing, simple placeholder.
	}
	
</script>