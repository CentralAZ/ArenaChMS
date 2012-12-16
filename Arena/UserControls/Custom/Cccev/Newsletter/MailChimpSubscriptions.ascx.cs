/**********************************************************************
* Description:	Displays subscribe/subscribed information for each
*				newsletter configured in the MailChimpLists lookup.
*				
*				This module depends on:
*				 - MailChimp 1.1 API DLL (MailChimp_v1_1.dll)
*				 - CookComputing.XmlRpcV2.dll
*				 - A lookup type which contains all the MailChimp Lists
*				   with these values and qualifiers.
*					* Lookup value: the Name of the List/Newsletter
*					* Qualifier 1: MailChimp ListID
*					* Qualifier 2: corresponding Arena NewsletterID (int)
*					* Qualifier 8: URL of a background image URL to use when
*					  displaying the newsletter and status for the person.
* 
* Created By:	Nick Airdo
* Date Created: 10/15/2009 10:39:41 AM
*
* $Workfile: MailChimpSubscriptions.ascx.cs $
* $Revision: 5 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Newsletter/MailChimpSubscriptions.ascx.cs   5   2012-08-08 10:33:38-07:00   nicka $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Newsletter/MailChimpSubscriptions.ascx.cs $
*  
*  Revision: 5   Date: 2012-08-08 17:33:38Z   User: nicka 
*  Removed dependency on DataUtils.dll for simplicity sake. 
*  
*  Revision: 4   Date: 2011-07-16 06:18:47Z   User: nicka 
*  For Version 1.1.0 to support MailChimp "Groups" (aka INTERESTS). 
*  
*  Revision: 3   Date: 2010-08-02 22:07:28Z   User: nicka 
*  fix bad jquery reference 
*  
*  Revision: 2   Date: 2009-10-26 18:15:48Z   User: nicka 
*  Final version 
*  
*  Revision: 1   Date: 2009-10-26 17:49:07Z   User: nicka 
*  First draft includes ItemDataBound handler if wanting to bind to Arena 
*  newsletter. 
**********************************************************************/

using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Arena.Content;
using Arena.Core;
using Arena.Portal;

using MailChimp;
using CookComputing.XmlRpc;

namespace ArenaWeb.UserControls.Custom.Cccev.Newsletter
{
	public static class Constants
	{
		public static readonly DateTime NULL_DATE = DateTime.Parse( "1/1/1900" );
	}

	public static class MailChimpErrorCodes
	{
		public const int List_NotSubscribed = 215;
		public const int Email_NotExists = 232;
		public const int Email_NotSubscribed = 233;
		public const int List_MergeFieldRequired = 250;
		public const int List_InterestGroups_NotEnabled = 211;
	}

	#region Helper Class
	public partial class MailChimpLookup
	{
		#region Private Members
		private string imageUrl = string.Empty;
		private string listID = string.Empty;
		private string name = string.Empty;
		private int newsletterID = -1;

		#endregion

		#region Public Properties
		public string ImageUrl
		{
			get { return imageUrl; }
			set { imageUrl = value; }
		}

		public string ListID
		{
			get { return listID; }
			set { listID = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public int NewsletterID
		{
			get { return newsletterID; }
			set { newsletterID = value; }
		}
		#endregion

		#region Constructors
		public MailChimpLookup()
        {
			
        }

		public MailChimpLookup(Lookup item)
        {
			newsletterID = int.Parse( item.Qualifier2 );
			listID = item.Qualifier;
			imageUrl = item.Qualifier8;
			name = item.Value;
        }

		#endregion
	}
	#endregion

	public partial class MailChimpSubscriptions : PortalControl
	{
		#region Module Settings
		[TextSetting( "MailChimp API Key", "The API Key you got from MailChimp by going here: http://admin.mailchimp.com/account/api-key-popup or http://us1.admin.mailchimp.com/account/api/", true )]
		public string MailChimpAPIKeySetting { get { return Setting( "MailChimpAPIKey", "", true ); } }

		[TextSetting( "MailChimp List Lookup GUID", "The GUID of the Cccev MailChimp Lists lookup. Default D5AA90DA-2DE6-4E66-A966-DB6133ED18EC", true )]
		public string MailChimpLookupGUIDSetting { get { return Setting( "MailChimpLookupGUID", "D5AA90DA-2DE6-4E66-A966-DB6133ED18EC", true ); } }

		[BooleanSetting( "Enable Interest Groups", "Enables the ability to see and edit a person's MailChimp interest groups within a list.", false, false ), Category( "Interest Groups" )]
		public bool EnableMCGroupsSetting { get { return Boolean.Parse( Setting( "EnableMCGroups", "false", false )) ; } }

		[BooleanSetting( "Set Maximum Columns?", "Sets whether to use a maximum number of columns or rows. (true = columns; false = rows)", false, true ), Category( "Interest Groups" )]
		public bool MaxColumnsSetting { get { return Boolean.Parse( Setting( "MaxColumns", "true", false ) ); } }

		[NumericSetting( "Number of Rows/Cols", "Number of rows/cols to use when creating the table of interest groups. (Default 2).", false ), Category( "Interest Groups" )]
		public int NumRowsColsSetting { get { return int.Parse( Setting( "NumCols", "2", false ) ); } }

		#endregion

		#region Private Property
		protected DateTime NULL_DATE = DateTime.Parse( "1/1/1900" );
		private ApiWrapper api;
		private Person person = null;
		#endregion

		#region Event Handlers

		protected void Page_Init( object sender, System.EventArgs e )
		{
			InitAPI();

		}

		private void Page_Load(object sender, System.EventArgs e)
		{			
			lblMsg.InnerHtml = "";
			smpScripts.Scripts.Add( new ScriptReference( string.Format( "~/{0}", BasePage.JQUERY_INCLUDE ) ) );
			if ( !IsPostBack )
			{
				BindLists();
			}
		}

		private void BindLists()
		{
			PersonEmailCollection emails = GetPerson().Emails;
			if ( emails.Count > 0 )
			{
				LookupCollection mailChimpLists = new LookupCollection( new Guid( MailChimpLookupGUIDSetting ) );
				dlLists.DataSource = mailChimpLists;
				dlLists.DataBind();
			}
		}

		/// <summary>
		/// Used when binding the lists to the MailChimp actual subscribers.  It will use the
		/// API to dynamically query each potential subscriber against each configured list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void dlLists_ItemDataBound( object sender, RepeaterItemEventArgs e )
		{
			if ( e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem )
			{
				MailChimpLookup item = new MailChimpLookup( e.Item.DataItem as Lookup );

				if ( item.ImageUrl != string.Empty )
				{
					Image image = e.Item.FindControl( "imgBG" ) as Image;
					image.ImageUrl = item.ImageUrl;
				}
				else
				{
					Literal itemName = e.Item.FindControl( "liListName" ) as Literal;
					itemName.Text = item.Name;
				}

				ImageButton btnSubscribe = e.Item.FindControl( "btnSubscribe" ) as ImageButton;
				btnSubscribe.CommandName = "Subscribe";
				btnSubscribe.CommandArgument = item.ListID;

				ImageButton btnUnsubscribe = e.Item.FindControl( "btnUnsubscribe" ) as ImageButton;
				btnUnsubscribe.CommandName = "Unsubscribe";
				btnUnsubscribe.CommandArgument = item.ListID;

				LinkButton lbSaveGroups = e.Item.FindControl( "lbSaveGroups" ) as LinkButton;
				lbSaveGroups.CommandName = "SaveGroups";
				lbSaveGroups.CommandArgument = item.ListID;

				CheckBoxList cblGroups = e.Item.FindControl( "cblGroups" ) as CheckBoxList;

				try
				{
					// Create a checkbox for each Interest group if MC Groups is enabled.
					if ( EnableMCGroupsSetting )
					{
						lbSaveGroups.Visible = false;
						try
						{
							MCInterestGroups mcGroups = api.listInterestGroups( item.ListID );
							cblGroups.Visible = true;
							cblGroups.RepeatColumns = ( MaxColumnsSetting ) ? NumRowsColsSetting : ( mcGroups.groups.Count() + NumRowsColsSetting - 1 ) / NumRowsColsSetting;
							cblGroups.DataSource = mcGroups.groups;
							cblGroups.DataBind();
						}
						catch ( XmlRpcFaultException ex )
						{
							if ( ex.FaultCode != MailChimpErrorCodes.List_InterestGroups_NotEnabled )
							{
								AddMsg( string.Format( " {0}: {1} (err:{2})", item.Name, ex.FaultString, ex.FaultCode ) );
							}
						}
					}

					// Try to determine if the email address is subscribed/unsubscribed from the MailChimp list
					MCMemberInfo info = api.listMemberInfo( item.ListID, GetPerson().Emails.FirstActive );

					// Set the user's groups if "MailChimp groups" is enabled.
					if ( EnableMCGroupsSetting )
					{
						lbSaveGroups.Visible = true;
						// now check the interests that are selected for this user
						string interests = (from m in info.merges where m.tag == "INTERESTS" select m).FirstOrDefault().val;
						if ( !string.IsNullOrEmpty( interests ) )
						{
							foreach ( ListItem interestCheckbox in cblGroups.Items )
							{
								if ( interests.Contains( interestCheckbox.Value ) )
								{
									interestCheckbox.Selected = true;
								}
							}
						}
					}

					string hoverInfo = string.Format( " {0}: {1} on {2} EST/EDT", item.Name, info.status, info.timestamp );
					if ( "subscribed".Equals( info.status ) )
					{
						btnUnsubscribe.ToolTip = hoverInfo;
						btnUnsubscribe.Visible = true;
					}
					else
					{
						btnSubscribe.ToolTip = hoverInfo;
						btnSubscribe.Visible = true;
					}
				}
				catch ( XmlRpcFaultException ex )
				{
					if ( ex.FaultCode == MailChimpErrorCodes.Email_NotExists )
					{
						btnSubscribe.Visible = true;
					}
					else if ( ex.FaultCode == MailChimpErrorCodes.List_NotSubscribed )
					{
						btnSubscribe.ToolTip = "Person no longer subscribed.";
						btnSubscribe.Visible = true;
					}
					else
					{
						AddMsg( string.Format( " {0}: {1} (err:{2})", item.Name, ex.FaultString, ex.FaultCode ) );
					}
				}
			}
		}

		/// <summary>
		/// Event handler for when user actually clicks action button.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void dlLists_ItemCommand( object source, RepeaterCommandEventArgs e )
		{
			string listID = e.CommandArgument.ToString();
			ImageButton btnSubscribe = e.Item.FindControl( "btnSubscribe" ) as ImageButton;
			ImageButton btnUnsubscribe = e.Item.FindControl( "btnUnsubscribe" ) as ImageButton;

			int newsletterID = GetNewsletterIDFromMailChimpListID( listID );

			if ( e.CommandName == "Subscribe" )
			{
				CheckBoxList cblGroups = e.Item.FindControl( "cblGroups" ) as CheckBoxList;

				Subscribe( GetPerson(), listID, newsletterID, cblGroups.Items );
				btnSubscribe.Visible = false;
				btnUnsubscribe.Visible = true;
				btnUnsubscribe.CssClass = "buttonPending";
				AddMsg( "Subscription pending user opt-in." );
			}
			else if ( e.CommandName == "Unsubscribe" )
			{
				Unsubscribe( GetPerson(), listID, newsletterID );
				btnSubscribe.Visible = true;
				btnUnsubscribe.Visible = false;
				AddMsg( "Unsubscribed from list" );
			}
			else if ( e.CommandName == "SaveGroups" )
			{
				LinkButton lbSaveGroups = e.Item.FindControl( "lbSaveGroups" ) as LinkButton;
				CheckBoxList cblGroups = e.Item.FindControl( "cblGroups" ) as CheckBoxList;

				if ( SaveGroups( GetPerson(), listID, newsletterID, cblGroups.Items ) )
				{
					AddMsg( "Saved Interest Groups" );
					lbSaveGroups.Style.Add( "display", "none" );
				}
			}
		}

		/// <summary>
		/// Utility method to fetch an Arena newsletter which corresponds to the given MailChimp List.
		/// </summary>
		/// <param name="listID">List ID of a MailChimp list</param>
		/// <returns>newsletterID of a corresponding Arena newsletter; otherwise returns -1.</returns>
		private int GetNewsletterIDFromMailChimpListID( string listID )
		{
			int newsletterID = -1;
			LookupCollection mailChimpLists = new LookupCollection( new Guid( MailChimpLookupGUIDSetting ) );
			var query = ( from lookup in mailChimpLists.OfType<Lookup>()
						  where lookup.Qualifier == listID
						  select lookup ).FirstOrDefault();

			if ( query != null )
			{
				newsletterID = int.Parse( query.Qualifier2 );
			}
			return newsletterID;
		}

		/// <summary>
		/// Method used to unsubscribe a person from the MailChimp List (and Arena newsletter).
		/// </summary>
		/// <param name="person">person to subscribe</param>
		/// <param name="listID">MailChimp list ID</param>
		/// <param name="newsletterID">Arena's newsletter ID</param>
		private void Unsubscribe( Person person, string listID, int newsletterID )
		{
			api.listUnsubscribe( listID, person.Emails.FirstActive );

			NewsletterSubscription subscription = FetchOrCreateNewsletterSubscription( person, newsletterID );
			subscription.Status = false;
			subscription.Save( CurrentUser.Identity.Name );
		}

		/// <summary>
		/// Method used to subscribe a person to the MailChimp List (and Arena newsletter).
		/// </summary>
		/// <param name="person">person to subscribe</param>
		/// <param name="listID">MailChimp list ID</param>
		/// <param name="newsletterID">Arena's newsletter ID</param>
		private void Subscribe( Person person, string listID, int newsletterID, ListItemCollection listItemCollection )
		{
			MCMergeVar[] merges = new MCMergeVar[5];
			BuildMergeVars( person, merges, listItemCollection );

			if ( api.listSubscribe( listID, person.Emails.FirstActive, merges ) )
			{
				// Track the subscription locally too
				NewsletterSubscription subscription = FetchOrCreateNewsletterSubscription( person, newsletterID );
				subscription.Status = true;
				subscription.Save( CurrentUser.Identity.Name );
			}
			else
			{
				AddMsg( "An error prevented the person from being subscribed to the list." );
			}
		}

		/// <summary>
		/// Save the person's interest groups.  This already assumes the person is Subscribed.
		/// </summary>
		/// <param name="person"></param>
		/// <param name="listID"></param>
		/// <param name="newsletterID"></param>
		/// <param name="listItemCollection"></param>
		private bool SaveGroups( Person person, string listID, int newsletterID, ListItemCollection listItemCollection )
		{
			MCMergeVar[] merges = new MCMergeVar[5];
			BuildMergeVars( person, merges, listItemCollection );

			if ( ! api.listUpdateMember( listID, person.Emails.FirstActive, merges, "", true ) )
			{
				AddMsg( "An error occurred while trying to save the person's interests." );
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// A small helper method to create the basic merge varibles from a person (FNAME, LNAME, BIRTHDATE, GENDER).
		/// </summary>
		/// <param name="person"></param>
		/// <param name="merges"></param>
		private static void BuildMergeVars( Person person, MCMergeVar[] merges, ListItemCollection listItemCollection)
		{
			merges[0].tag = "FNAME";
			merges[0].val = person.FirstName;
			merges[1].tag = "LNAME";
			merges[1].val = person.LastName;

			if ( person.BirthDate != Constants.NULL_DATE )
			{
				merges[2].tag = "BIRTHDATE";
				merges[2].val = person.BirthDate.ToShortDateString();
			}
			merges[3].tag = "GENDER";
			merges[3].val = person.Gender.ToString();

			merges[4].tag = "INTERESTS";

			// Turn the selected items in the checkbox into a comma delimited list of interests (ex: "A,B,C")
			var values =  listItemCollection.Cast<ListItem>().Where( n => n.Selected ).Select( n => n.Value ).ToArray();
			merges[4].val = string.Join( ",", values );

		}

		/// <summary>
		/// Fetches an existing newsletter subscription for the given person and Arena
		/// newsletter ID or creates a new one if it did not already exist.
		/// </summary>
		/// <param name="person">person for who the subscription is for</param>
		/// <param name="newsletterID">Arena newsletter ID of the newsletter subscription</param>
		/// <returns>An Arena NewsletterSubscription for the given args</returns>
		private NewsletterSubscription FetchOrCreateNewsletterSubscription( Person person, int newsletterID )
		{
			NewsletterSubscription subscription;
			NewsletterSubscriptionCollection subscriptionsArena = new NewsletterSubscriptionCollection();
			subscriptionsArena.LoadNewsletterSubscriptionsByEmail( person.Emails.FirstActive );

			var query =
				( from existingSubscription in subscriptionsArena.OfType<NewsletterSubscription>()
				  where existingSubscription.NewsletterId == newsletterID
				  select existingSubscription ).ToList();

			if ( query.Count > 0 )
			{
				subscription = new NewsletterSubscription( query.First().SubscriptionId );
			}
			else
			{
				subscription = new NewsletterSubscription();
				subscription.PersonId = person.PersonID;
				subscription.NewsletterId = newsletterID;
				subscription.Email = person.Emails.FirstActive;
				subscription.FirstName = person.FirstName;
				subscription.LastName = person.LastName;
				subscription.Gender = person.Gender;
			}

			return subscription;
		}
		#endregion

		/// <summary>
		/// A helper method to get a person object using what is in the query string.
		/// Candidate for shared library.
		/// </summary>
		/// <returns></returns>
		private Person GetPerson()
		{
			if ( person == null )
			{
				if ( Request.Params[ "guid" ] != null )
				{
					person = new Person( new Guid( Request.Params[ "guid" ] ) );
				}
				else if ( Request.Params[ "person" ] != null )
				{
					person = new Person( int.Parse( Request.Params[ "person" ] ) );
				}
				else
				{
					person = new Person();
				}
			}
			return person;
		}

		/// <summary>
		/// Method to include text to the user message output.
		/// </summary>
		/// <param name="msg"></param>
		private void AddMsg( string msg )
		{
			lblMsg.Visible = true;
			if ( lblMsg.InnerHtml.Length > 0 )
			{
				lblMsg.InnerHtml += "<br/>";
			}
			lblMsg.InnerHtml += msg;
		}

		private void InitAPI()
		{
			api = new ApiWrapper();
			api.setCurrentApiKey( MailChimpAPIKeySetting );
		}
	}
}