/**********************************************************************
* Description:	Displays a breadcrumb trail.
* Created By:   Nick Airdo @ Central Christian Church of the East Valley
* Date Created:	08/26/2008
*
* $Workfile: PortalPageBreadCrumb.ascx.cs $
* $Revision: 1 $ 
* $Header: /Arena/Arena/Inetpub/wwwroot/Arena/UserControls/Custom/Cccev/WebUtils/PortalPageBreadCrumb.ascx.cs   1   2008-08-27 08:45:46-07:00   nicka $
* 
* $Log: /Arena/Arena/Inetpub/wwwroot/Arena/UserControls/Custom/Cccev/WebUtils/PortalPageBreadCrumb.ascx.cs $
*  
*  Revision: 1   Date: 2008-08-27 15:45:46Z   User: nicka 
**********************************************************************/
using System;
using System.Web.UI;

using Arena.Portal;

namespace ArenaWeb.UserControls.Custom.WebUtils
{
	public partial class PortalPageBreadCrumb : PortalControl
	{
		[NumericSetting( "Remove Levels", "The number of base levels to remove from the breadcrumb trail", false )]
		public int RemoveLevelsSetting { get { return int.Parse( Setting( "RemoveLevels", "0", false )); } }

		[TextModuleSetting( "Breadcrumb Class", "the CSS classname to use in the breadcrumbs", false )]
		public string BreadcrumbClassSetting { get { return Setting( "BreadcrumbClass", "", false ); } }

		[TextModuleSetting( "Breadcrumb Separator", "the separator to use in the breadcrumbs", false )]
		public string BreadcrumbSeparatorSetting { get { return Setting( "BreadcrumbSeparator", "", false ); } }

		protected void Page_Load( object sender, EventArgs e )
		{
			if ( ! IsPostBack )
			{
				GetBreadCrumbs();
			}
		}

		public void GetBreadCrumbs()
		{
			phBreadCrumbs.Controls.Clear();
			BreadCrumbs breadcrumbs = base.CurrentPortalPage.BreadCrumbs;
			if ( BreadcrumbClassSetting != "" )
			{
				breadcrumbs.CssClass = BreadcrumbClassSetting;
			}

			if ( BreadcrumbSeparatorSetting != "" )
			{
				breadcrumbs.Separator = BreadcrumbSeparatorSetting;
			}

			// Remove the number of levels set by the administrator
			if ( RemoveLevelsSetting != 0 )
			{
				int levels = breadcrumbs.Count - RemoveLevelsSetting;
				for ( int i = breadcrumbs.Count; i > levels; i-- )
				{
					breadcrumbs.RemoveAt( i - 1 );
				}
			}

			phBreadCrumbs.Controls.Add( new LiteralControl( breadcrumbs.ToString() ) );
		}
	}
}