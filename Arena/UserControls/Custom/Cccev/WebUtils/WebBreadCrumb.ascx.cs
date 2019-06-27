/**********************************************************************
* Description:	Displays a breadcrumb trail.
* Created By:   Nick Airdo @ Central Christian Church of the East Valley
* Date Created:	11/21/2007
*
* $Workfile: WebBreadCrumb.ascx.cs $
* $Revision: 1 $ 
* $Header: /Arena/Arena/Inetpub/wwwroot/Arena/UserControls/Custom/Cccev/WebUtils/WebBreadCrumb.ascx.cs   1   2007-11-28 16:23:33-07:00   nicka $
* 
* $Log: /Arena/Arena/Inetpub/wwwroot/Arena/UserControls/Custom/Cccev/WebUtils/WebBreadCrumb.ascx.cs $
*  
*  Revision: 1   Date: 2007-11-28 23:23:33Z   User: nicka 
*  initial version 
**********************************************************************/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;

using Arena.Portal;
using Arena.Custom.Cccev.WebUtils;

namespace ArenaWeb.UserControls.Custom.WebUtils
{
	public partial class WebBreadCrumb : PortalControl
	{

		[PageModuleSetting( "Root Page", "The root page to start the breadcrumb trail from.", true )]
		public string RootPageIdSetting { get { return Setting( "RootPageId", "", true ); } }

		[TextModuleSetting( "Breadcrumb Class", "the CSS classname to use in the breadcrumbs", false )]
		public string BreadcrumbClassSetting { get { return Setting( "BreadcrumbClass", "", false ); } }

		[TextModuleSetting( "Breadcrumb Separator", "the separator to use in the breadcrumbs", false )]
		public string BreadcrumbSeparatorSetting { get { return Setting( "BreadcrumbSeparator", "", false ); } }

		[TextModuleSetting( "Breadcrumb Separator Class", "the CSS classname to use for the separator", false )]
		public string BreadcrumbSeparatorClassSetting { get { return Setting( "BreadcrumbSeparatorClass", "", false ); } }

		protected void Page_Load( object sender, EventArgs e )
		{
			if ( ! IsPostBack )
			{
				GetBreadCrumbs();
			}
		}

		public void GetBreadCrumbs()
		{
			BreadcrumbData breadcrumbData = new BreadcrumbData();
			StringBuilder sb = new StringBuilder();
			DataTable table = breadcrumbData.GetBreadcrumbs_DT( this.CurrentPortalPage.PortalPageID, Convert.ToInt32( this.RootPageIdSetting ) ).DataSet.Tables[0];

			// since the data starts from the current page, we'll reverse it by traversing
			// the set in reverse.
			for ( int i = table.Rows.Count -1 ; i >= 0 ; --i )
			{
				sb.Append( MakeUrl( (string)table.Rows[i]["page_name"], (int)table.Rows[i]["page_id"] ));
				if ( i != 0 )
					sb.Append( "<span class='" + BreadcrumbSeparatorClassSetting + "'>" + BreadcrumbSeparatorSetting + "</span>" );
			}

			this.lblBreadcrumb.Text = sb.ToString();
		}

		public string MakeUrl( string name, int pageId  )
		{
			return ( "<a class='" + BreadcrumbClassSetting + "' href='" + base.Request.Url.AbsolutePath + "?Page=" + pageId + "'>" + name + "</a>" );
		}
	}
}