<%@ Control Language="C#" ClassName="ArenaWeb.UserControls.Custom.Cccev.WebUtils.InjectHtmlIfNotLoggedIn" Inherits="Arena.Portal.PortalControl" %>
<%@ Import Namespace="Arena.Portal"%>
<asp:PlaceHolder ID="phHtml" runat="server" />

<script runat="server">
/**********************************************************************
* Description:  A simple module to inject HTML (from the module's details)
*               only when the CurrentPerson is not logged in. 
* Created By:	Nick Airdo @ Central Christian Church of the East Valley
* Date Created:	8/10/1900 11:32:59 PM
*
* $Workfile: InjectHtmlIfNotLoggedIn.ascx $
* $Revision: 1 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/InjectHtmlIfNotLoggedIn.ascx   1   2010-08-11 09:23:17-07:00   nicka $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/InjectHtmlIfNotLoggedIn.ascx $
*  
*  Revision: 1   Date: 2010-08-11 16:23:17Z   User: nicka 
*  initial version 
**********************************************************************/
	protected void Page_Load( object sender, EventArgs e )
	{
		if ( CurrentPerson == null )
		{
			phHtml.Controls.Add( new LiteralControl( CurrentModule.Details ) );
		}
	}
</script>
