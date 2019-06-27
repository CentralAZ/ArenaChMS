/**********************************************************************
* Description:  Search Connector for MS Enterprise Search Server.
*				This is entirely based on the reference application found
*				at http://www.codeplex.com/MossSrchWs and the article at
*				http://msdn2.microsoft.com/en-us/library/bb852171.aspx
*
*               The search Query Schema is specified here:
*               http://msdn2.microsoft.com/en-us/library/ms563775.aspx
*
* Created By:   Nick Airdo @ Central Christian Church of the East Valley
* Date Created: 1/24/2008 11:02 PM
*
* $Workfile: SearchConnector.ascx.cs $
* $Revision: 6 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/SearchConnector.ascx.cs   6   2011-09-27 17:17:16-07:00   nicka $
*  
* $Log: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/SearchConnector.ascx.cs $
*  
*  Revision: 6   Date: 2011-09-28 00:17:16Z   User: nicka 
*  Updated to make generic and configurable.  Styling should be done via 
*  template stylesheet 
*  
*  Revision: 5   Date: 2010-01-27 22:49:28Z   User: JasonO 
*  Cleaning up. 
*  
*  Revision: 4   Date: 2009-04-03 00:24:10Z   User: JasonO 
*  
*  Revision: 3   Date: 2008-09-25 23:15:55Z   User: nicka 
*  Add module setting to allow custom URL for the search service 
*  
*  Revision: 2   Date: 2008-09-10 01:25:47Z   User: nicka 
*  use texbox value first if given over q in querystring 
*  
*  Revision: 1   Date: 2008-02-06 19:02:32Z   User: nicka 
*  Initial version of the search control 
***********************************************************************/

using System;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Arena.Custom.Cccev.WebUtils.QueryService;
using Arena.Custom.Cccev.WebUtils.Search;
using Arena.Portal;

namespace ArenaWeb.UserControls.Custom.Cccev.WebUtils
{ 
	public partial class SearchConnector : PortalControl
	{
		#region Module Settings

		[TextSetting( "Search Server URL", "The URL of your MS Search Service (eg, http://mss01/_vti_bin/search.asmx)", true ), Category( "WebService" )]
		public string SearchServerURLSetting { get { return Setting( "SearchServerURL", "", true ); } }

		[TextSetting( "WebService Account UserName", "The username to use in the Network Credentials for the WebService call.", true ), Category( "WebService Account" )]
		public string WebServiceUserNameSetting { get { return Setting( "WebServiceUserName", "", true ); } }

		[TextSetting( "WebService Account Password", "The password to use in the Network Credentials for the WebService call.", true ), Category( "WebService Account" )]
		public string WebServicePasswordSetting { get { return Setting( "WebServicePassword", "", true ); } }

		[TextSetting( "WebService Account Domain", "The domain to use in the Network Credentials for the WebService call.", true ), Category( "WebService Account" )]
		public string WebServiceDomainSetting { get { return Setting( "WebServiceDomain", "", true ); } }

		[NumericSetting( "Return Results Page Size", "The number of items to display on each page of the result set (default = 10).", false )]
		public string ReturnResultsPageSizeSetting { get { return Setting( "ReturnResultsPageSize", "10", false ); } }

		[TextSetting( "Search Button Image Path", "Relative path to image for search button above results.", false ), Category( "Styling" )]
		public string SearchImagePathSetting { get { return Setting( "SearchImagePath", "", false ); } }

		[TextSetting( "Search Button CSS Class", "CSS classname to use for the search button. Default 'searchBtn'", false ), Category( "Styling" )]
		public string SearchButtonCSSClassSetting { get { return Setting( "SearchButtonCSSClass", "searchBtn", false ); } }

		[TextSetting( "TextBox CSS Class", "CSS classname to use for the search textbox. Default 'search-box'", false ), Category( "Styling" )]
		public string TextBoxCSSClassSetting { get { return Setting( "TextBoxCSSClass", "search-box", false ); } }

		#endregion

		#region Private properties
		private System.Data.DataSet queryResults;
		private DateTime _startTime = DateTime.Now;
		private DateTime _endTime = DateTime.Now;
		#endregion

		protected void Page_Load( object sender, EventArgs e )
		{
			// I wish this worked...but alas Page/Response caching is not possible in Arena.
			//Page.Response.Cache.SetAllowResponseInBrowserHistory( true );
			//Page.Response.Cache.SetExpires( DateTime.Now.AddMinutes( 5 ) );
			//Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
			//Page.Response.Cache.SetValidUntilExpires( true );

			//base.Response.Cache.SetAllowResponseInBrowserHistory( true );
			//base.Response.Cache.SetExpires( DateTime.Now.AddMinutes( 5 ) );
			//base.Response.Cache.SetCacheability( HttpCacheability.ServerAndPrivate );
			//base.Response.Cache.SetValidUntilExpires( true );

			// If the user provided text in the textbox use it...
			if ( txtSearch.Text.Length > 0 ) 
			{
				dgSearchResults.CurrentPageIndex = 0;
				MSQueryService();
			}
			// otherwise look for a search term in the query string "q"
			else if ( Request.QueryString[ "q" ] != null )
			{
				txtSearch.Text = Request.QueryString[ "q" ];
				dgSearchResults.CurrentPageIndex = 0;
				MSQueryService();
			}
			else
			{
				divHeader.Visible = false;
			}

			if ( string.IsNullOrEmpty( SearchImagePathSetting ) )
			{
				ibGo.Style.Add( "display", "none" );
			}
			else
			{
				ibGo.ImageUrl = SearchImagePathSetting;
			}
			ibGo.CssClass = SearchButtonCSSClassSetting;
			txtSearch.CssClass = TextBoxCSSClassSetting;
			txtSearch.Focus();
		}

		/// <summary>
		/// Called when the user clicks the Go/Search button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnGo_Click( object sender, ImageClickEventArgs e )
		{
			if ( ! String.IsNullOrEmpty( txtSearch.Text ) )
			{
				MSQueryService();
			}
		}

		/// <summary>
		/// Called when changing the page on the datagrid's pager
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void dgSearchResults_PageIndexChanged( object source, DataGridPageChangedEventArgs e )
		{
			dgSearchResults.CurrentPageIndex = e.NewPageIndex;
			BindData( 1 );
		}

		#region Format Helper Methods
		/// <summary>
		/// Used to format the size (bytes) into KB.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public string FormatSize( object size )
		{
			string sSize = "0 KB";
			try
			{
				long lSize = (Int64)size / 1024;
				sSize = lSize + " KB";
			}
			catch
			{ }

			return sSize;
		}

		/// <summary>
		/// This will "bold" the search term in the search result fields.
		/// </summary>
		/// <param name="text">text from a search result field</param>
		/// <returns>the text with the search term bolded using HTML markup</returns>
		public string HighlightKeywords(object text)
		{
			string stringToSearch = (string)text;
			string replacedString = Regex.Replace( stringToSearch, txtSearch.Text, "<b>" + txtSearch.Text + "</b>", RegexOptions.IgnoreCase );
			return replacedString;
		}

		#endregion

		#region Private Methods

		private void MSQueryService()
		{
			BindData( 1 );
		}

		/// <summary>
		/// This will make a search request and bind the search results to the 
		/// table.  In theory the search request could be done in chunks
		/// starting at the given startAt variable, however we're just going to
		/// fetch the entire result set when we do the query since we don't have
		/// 100k+ resulting matches.
		/// </summary>
		/// <param name="startAt"></param>
		private void BindData( int startAt )
		{
			dgSearchResults.PageSize = Convert.ToInt32( ReturnResultsPageSizeSetting );

			try
			{
				string keywordString = txtSearch.Text;
				QueryRequest queryRequest = BuildQueryRequest( keywordString, true, startAt, null );
				QueryService queryService = new QueryService
				{
					Url = SearchServerURLSetting,
					Credentials = new NetworkCredential( WebServiceUserNameSetting, WebServicePasswordSetting, WebServiceDomainSetting )
				};

				queryResults = queryService.QueryEx( queryRequest.ToString() );
				_endTime = DateTime.Now;
				if ( queryResults.Tables[ 0 ].Rows.Count > 0 )
				{
					dgSearchResults.Visible = true;
					divNoResults.Visible = false;
					DisplayHeader( queryResults.Tables[ 0 ].Rows.Count );
					dgSearchResults.DataSource = queryResults.Tables[ 0 ];
					dgSearchResults.DataBind();
				}
				else
				{
					dgSearchResults.Visible = false;
					divHeader.Visible = false;
					divNoResults.Visible = true;
				}
				divErrorMessage.Visible = false;
			}
			catch
			{
				dgSearchResults.DataSource = null;
				dgSearchResults.DataBind();
				divHeader.Visible = false;
				dgSearchResults.Visible = false;
				divErrorMessage.Visible = true;
			}
		}

		private void DisplayHeader( int total )
		{
			divHeader.Visible = true;
			TimeSpan ts = _endTime.Subtract( _startTime );
			int startItem = dgSearchResults.CurrentPageIndex * dgSearchResults.PageSize + 1;
			int endItem = startItem + dgSearchResults.PageSize - 1;
			if ( endItem > total )
				endItem = total;
			divHeader.InnerHtml = "Results <b>" + startItem + "</b> - <b>" + endItem + "</b> of about <b>" + total + "</b>. (<b>" + ts.Seconds + "." + ts.Milliseconds + "</b> seconds)";
		}

		private static QueryRequest BuildQueryRequest( string text, bool isKeyword, int startAt, string target )
        {
			QueryRequest queryRequest = new QueryRequest
			{
				QueryText = text,
				QueryID = Guid.NewGuid()
			};

            // Decide what type of query we're doing
			queryRequest.QueryType = isKeyword ? QueryType.Keyword : QueryType.MsSql;

            // Set the number of results to return
			//queryRequest.Count = returnCount;
			queryRequest.StartAt = startAt;

            // Set the web service target, if needed
            if (string.IsNullOrEmpty( target ) == false)
            {
                queryRequest.Target = target;
            }

            // Set query options, as appropriate
            queryRequest.EnableStemming = true; // checkBoxEnableStemming.Checked;
            queryRequest.IgnoreAllNoiseQuery = true; //checkBoxIgnoreAllNoiseQuery.Checked;
            queryRequest.ImplicitAndBehavior = true; //checkBoxImplicitAndBehavior.Checked;
            queryRequest.TrimDuplicates = true; //checkBoxTrimDuplicates.Checked;
            queryRequest.IncludeRelevantResults = true; // checkBoxIncludeRelevantResults.Checked;
            queryRequest.IncludeHighConfidenceResults = true; // checkBoxIncludeHighConfidenceResults.Checked;
            queryRequest.IncludeSpecialTermResults = true; //checkBoxIncludeSpecialTermResults.Checked;

			return queryRequest;
		}
		#endregion
	}
}
