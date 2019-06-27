/**********************************************************************
* Description:	Dynamically inject a CSS file into the page header
* Created By:   Nick Airdo @ Central Christian Church of the East Valley
* Date Created:	04/07/2008
*
* $Workfile: InjectCss.ascx.cs $
* $Revision: 2 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/InjectCss.ascx.cs   2   2009-03-30 14:49:43-07:00   DallonF $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/InjectCss.ascx.cs $
*  
*  Revision: 2   Date: 2009-03-30 21:49:43Z   User: DallonF 
*  Injects CSS on postbacks 
*  
*  Revision: 1   Date: 2008-04-08 05:40:50Z   User: nicka 
*  first draft 
**********************************************************************/
using System;

using Arena.Portal;

namespace ArenaWeb.UserControls.Custom.Cccev.WebUtils
{
	public partial class InjectCss : PortalControl
	{
		[FileSetting( "Css File", "The CSS filename to inject into the page header", false )]
		public string CssFileSetting { get { return Setting( "CssFile", "", false ); } }

		protected void Page_Load( object sender, EventArgs e )
		{
            Utilities.AddCssLink( this.Page, CssFileSetting );
		}
	}

}