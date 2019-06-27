/**********************************************************************
* Description:  Search Form used to pass search string to the 
*               Search Connector.
*
* Created By:   Nick Airdo @ Central Christian Church of the East Valley
* Date Created: 7/08/2008 09:08 PM
*
* $Workfile: SearchForm.ascx.cs $
* $Revision: 2 $ 
* $Header: /Arena/Arena/Inetpub/wwwroot/Arena/UserControls/Custom/Cccev/WebUtils/SearchForm.ascx.cs   2   2008-09-09 18:24:58-07:00   nicka $
*  
* $Log: /Arena/Arena/Inetpub/wwwroot/Arena/UserControls/Custom/Cccev/WebUtils/SearchForm.ascx.cs $
*  
*  Revision: 2   Date: 2008-09-10 01:24:58Z   User: nicka 
*  change button to NOT use submit behavior (conflict with enter key/postback) 
*  
*  Revision: 1   Date: 2008-09-09 00:09:57Z   User: nicka 
*  initial version 1.0 
***********************************************************************/
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
using System.Net;
using System.Text.RegularExpressions;

using Arena.Portal;
using Arena.Custom.Cccev.WebUtils.Search;

namespace ArenaWeb.UserControls.Custom.Cccev.WebUtils
{
	public partial class SearchForm : PortalControl
	{
		string _searchResultsPageID = string.Empty;

		#region Module Settings
		[SmartPageSetting("Search Results Page", "Page with a SearchConnector module used to show search results.", "UserControls/Custom/Cccev/WebUtils/SearchConnector.ascx", RelatedModuleLocation.Beside)]
		public string SearchResultsPageIDSetting
		{
			get { return _searchResultsPageID; }
			set { _searchResultsPageID = value; }
		}

		[TextSetting("Button Text", "The text to display on the button.  Default is 'Search'.", false)]
		public string ButtonTextSetting { get { return Setting("ButtonText", "", false); } }

		[TextSetting( "Button CSS Class", "The CSS classname to use on the button.  Default is 'smallText'.", false )]
		public string ButtonCssClassSetting { get { return Setting( "ButtonCSSClass", "", false ); } }

		[BooleanSetting("ShowSearchButton", "Flag indicating whether or not the search button should be displayed.", false, true)]
		public bool ShowSearchButtonSetting { get { return Convert.ToBoolean( Setting( "ShowSearchButton", "true", false ) ); } }

		#endregion

		protected void Page_Load( object sender, EventArgs e )
		{
			if ( IsPostBack && txtString.Text.Length > 0 )
			{
				DoSearch();
			}
			else
			{
				if ( ShowSearchButtonSetting )
				{
					if ( !ButtonTextSetting.Equals( "" ) )
						btnQuickSearch.Text = ButtonTextSetting;

					if ( !ButtonCssClassSetting.Equals( "" ) )
						btnQuickSearch.CssClass = ButtonCssClassSetting;
				}
				else
				{
					btnQuickSearch.Visible = false;
				}
			}
		}

		protected void btnQuickSearch_Click( object sender, EventArgs e )
		{
			DoSearch();
		}

		private void DoSearch()
		{
			Response.Redirect( Request.Url.AbsolutePath + "?Page=" + SearchResultsPageIDSetting + "&q=" + txtString.Text );
		}
}
}
