<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VimeoViewer.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.Web.VimeoViewer" %>
<div id="centralaz-vimeo-main" class="clearfix"></div>
<script type="text/javascript">
	/// Note: This module depends on UserControls/Custom/Cccev/Web/Scripts/centralaz.media.js, backbone.js, mustache.js
	$(function () {
		var mediaApp = new CentralAZ.Media.Runner("<%= VimeoAccountSetting %>");
		mediaApp.init();
	});
</script>