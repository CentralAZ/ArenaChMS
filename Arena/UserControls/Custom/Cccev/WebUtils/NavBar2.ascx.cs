/**********************************************************************
 * Description: NavBar menu control that uses the Telerik Rad PanelBar.
 * Created By:   Nick Airdo @ Central Christian Church of the East Valley
 * Date Created: 1/11/2008 04:02 PM
 *
 * $Workfile: NavBar2.ascx.cs $
 * $Revision: 6 $ 
 * $Header: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/NavBar2.ascx.cs   6   2009-01-05 17:16:29-07:00   nicka $
 *  
 * $Log: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/NavBar2.ascx.cs $
*  
*  Revision: 6   Date: 2009-01-06 00:16:29Z   User: nicka 
*  
*  Revision: 5   Date: 2009-01-06 00:13:51Z   User: nicka 
*  SingleExpandedItem setting default true 
*  
*  Revision: 4   Date: 2009-01-05 23:59:20Z   User: nicka 
*  Added control of single vs muti expanded items 
*  
*  Revision: 3   Date: 2008-01-29 19:37:28Z   User: nicka 
*  Changed so that selected child group CSS is "groupExpanded" 
*  
*  Revision: 2   Date: 2008-01-23 05:28:28Z   User: nicka 
*  Added ability to persist navbar state via cookie. 
*  
*  Revision: 1   Date: 2008-01-11 23:06:13Z   User: nicka 
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
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Arena.Portal;
using Arena.Custom.Cccev.WebUtils;
using Telerik.WebControls;

namespace ArenaWeb.UserControls.Custom.WebUtils
{
	public partial class NavBar2 : PortalControl
	{
		private int pageID = -1;

		[PageSetting( "Root Page", "The page to display the child pages of. The default is the current page.", false )]
		public string RootPageIdSetting { get { return Setting( "RootPageId", "", false ); } }

		[TextSetting( "RadPanel Skin", "The skin to use for the RadPanel. The default is Cccev.", false )]
		public string RadPanelSkinSetting { get { return Setting( "RadPanelSkin", "Cccev", false ); } }

		[NumericSetting( "Width", "The width to use (default = 200).", false )]
		public string WidthSetting { get { return Setting( "Width", "200", false ); } }

		[NumericSetting( "ExpandAnimationDuration", "The speed of the effect animation duration (default = 100).", false )]
		public string ExpandAnimationDurationSetting { get { return Setting( "ExpandAnimationDuration", "100", false ); } }

		[NumericSetting( "CollapseAnimationDuration", "The speed of the effect animation duration (default = 100).", false )]
		public string CollapseAnimationDurationSetting { get { return Setting( "CollapseAnimationDuration", "100", false ); } }

		[BooleanSetting( "PersistStateInCookie", "Controls whether or not the panel bar should remember its state (from postback to postback) via a cookie.  Defaults is true.", false, true )]
		public bool PersistStateInCookieSetting	{ get { return Convert.ToBoolean( Setting( "BooleanValue", "true", false ) ); } }

		[BooleanSetting( "Single Expanded Item", "Only one item can be expanded at a time. Expanding another item collapses the previously expanded one.  Defaults is true.", false, true )]
		public bool SingleExpandedItemSetting { get { return Convert.ToBoolean( Setting( "BooleanValue", "true", false ) ); } }

		// Methods
		private void InitializeComponent()
		{
			RadPanelbar2.ItemDataBound += new RadPanelbarEventHandler( RadPanelbar2_ItemDataBound );
		}

		protected void Page_Load( object sender, EventArgs e )
		{
			if ( ! IsPostBack )
			{
				InitPanelBar();
				BindPanelBarToDataSource();
			}

			// Expand the selected page and its parent chain
			pageID = Convert.ToInt32( Request.Params[ "page" ] );
			RadPanelItem selected = RadPanelbar2.FindItemByValue( pageID.ToString() );
			if ( selected != null )
			{
				selected.Selected = true;
				selected.Expanded = true;
				selected.ChildGroupCssClass = "groupExpanded";
				selected.ExpandParentItems();
			}
		}

		/// <summary>
		/// Bind the panel bar to the DataSource and cache the DataSource in the user's
		/// session to improve performance.
		/// </summary>
		private void BindPanelBarToDataSource()
		{
			NavBarData navbar = new NavBarData();
			int personID = CurrentPerson != null ? CurrentPerson.PersonID : -1;

			DataTable table = null;

			string navSessionkey = "CccevNavBar$" + RootPageIdSetting + "$";
			string authKey = "CccevNavBarPersonID$";

			// don't get the table from the session if RefreshCache was requested
			if (Request.QueryString["refreshcache"] != null )
			{
				table = null;
			}
			else if ( Session[navSessionkey] != null && Session[authKey] != null && (int) Session[authKey] == personID )
			{
				table = (DataTable) Session[navSessionkey];
			}
			// fetch a fresh table if the table has been set to null
			if ( table == null )
			{
				table = navbar.GetNavBar_DT( Convert.ToInt32( RootPageIdSetting ), personID );
				Session[ navSessionkey ] = table;
				// store the authenticated personID for the next check in order to ensure the user's 
				// authentication has not changed since this DataSource was cached in their session. 
				Session[ authKey ] = personID;
			}

			RadPanelbar2.DataSource = table;
			RadPanelbar2.DataBind();
		}

		private void InitPanelBar()
		{
			RadPanelbar2.ItemDataBound += new RadPanelbarEventHandler( RadPanelbar2_ItemDataBound );
			
			RadPanelbar2.Skin = RadPanelSkinSetting;
			RadPanelbar2.Width = Convert.ToInt32( WidthSetting );

			if ( SingleExpandedItemSetting )
			{
				RadPanelbar2.ExpandMode = Telerik.WebControls.PanelbarExpandMode.SingleExpandedItem;
			}
			else
			{
				RadPanelbar2.ExpandMode = Telerik.WebControls.PanelbarExpandMode.MultipleExpandedItems;
			}
			
			RadPanelbar2.ExpandAnimation.Type = PanelbarAnimationType.OutSine;
			RadPanelbar2.ExpandAnimation.Duration = Convert.ToInt32( ExpandAnimationDurationSetting );
			RadPanelbar2.CollapseAnimation.Type = PanelbarAnimationType.InOutCubic;
			RadPanelbar2.CollapseAnimation.Duration = Convert.ToInt32( CollapseAnimationDurationSetting );
			RadPanelbar2.PersistStateInCookie = PersistStateInCookieSetting;

			RadPanelbar2.DataFieldID = "page_id";
			RadPanelbar2.DataFieldParentID = "parent_page_id";
			RadPanelbar2.DataTextField = "page_name";
			RadPanelbar2.DataValueField = "page_id";
		}

		protected void RadPanelbar2_ItemDataBound( object sender, RadPanelbarEventArgs e )
		{
			// Clear the .Text property if x is == ""
			object x = DataBinder.Eval( e.Item.DataItem, "page_name" );
			if ( "".Equals( x ) )
			{
				e.Item.Text = "";
			}
			else
			{
				e.Item.NavigateUrl = "~/Default.aspx?page=" + (int)DataBinder.Eval( e.Item.DataItem, "page_id" );
			}
			e.Item.ImagePosition = Telerik.WebControls.RadPanelItemImagePosition.Right;
		}

		protected void Page_PreRender( object sender, EventArgs e )
		{
			foreach ( RadPanelItem item in RadPanelbar2.Items )
			{
				if ( item.Items.Count > 0 )
				{
					item.ImageUrl = item.Expanded ? "~/UserControls/Custom/Cccev/WebUtils/images/expanded2.gif" : "~/UserControls/Custom/Cccev/WebUtils/images/collapsed2.gif";
					SetNodeChildrenImages( item );
				}
			}
		}

		protected void SetNodeChildrenImages( RadPanelItem node )
		{
			foreach ( RadPanelItem item in node.Items )
			{
				if ( item.Items.Count > 0 )
				{
					item.ImageUrl = item.Expanded ? "~/UserControls/Custom/Cccev/WebUtils/images/expanded2.gif" : "~/UserControls/Custom/Cccev/WebUtils/images/collapsed2.gif";
					SetNodeChildrenImages( item );
				}
			}
		}
	}
}
