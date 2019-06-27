/**********************************************************************
* Description:	Displays a "waiting list" signup button if the event
*				registration is active, but full.
* Created By:   Nick Airdo @ Central Christian Church of the East Valley
* Date Created:	11/05/2009
*
* $Workfile: WaitingListButton.ascx.cs $
* $Revision: 3 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/WaitingListButton.ascx.cs   3   2010-08-16 19:19:19-07:00   nicka $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/WaitingListButton.ascx.cs $
*  
*  Revision: 3   Date: 2010-08-17 02:19:19Z   User: nicka 
*  Bug fix: sometimes an event details uses a profileId instead of eventId in 
*  the querystring. 
*  
*  Revision: 2   Date: 2009-11-05 23:09:22Z   User: nicka 
*  
*  Revision: 1   Date: 2009-11-05 20:06:43Z   User: nicka 
*  First draft 
**********************************************************************/
using System;
using Arena.Core;
using Arena.Event;
using Arena.Portal;

namespace ArenaWeb.UserControls.Custom.Cccev.WebUtils
{
	public partial class WaitingListButton : PortalControl
	{
		[TextSetting( "Button Text", "Text to use on waiting list button. Default 'Event Full - Waiting List Sign-up'.", false )]
		public string ButtonTextSetting { get { return Setting( "ButtonText", "", false ); } }

		[PageSetting( "Registration Page", "The page to use for users to register for this tag.", true )]
		public string RegistrationPageSetting { get { return Setting( "RegistrationPage", "", true ); } }

		private void Page_Load( object sender, EventArgs e )
		{
			if ( ! IsPostBack )
			{
				int eventID = -1;
				if ( Request.QueryString["eventId"] != null )
				{
					eventID = int.Parse( Request.QueryString["eventId"] );
				}
				else if ( Request.QueryString["profileId"] != null )
				{
					eventID = int.Parse( Request.QueryString["profileId"] );
				}

				if ( eventID != -1 )
				{ 
					ProcessEvent( eventID );
				}
			}
		}

		/// <summary>
		/// Decide whether to show the Waiting List button or not for the given event ID.
		/// If so, uses the first child called "Waiting List" or sibling to the event if one exists and is active.
		/// </summary>
		/// <param name="eventID">the event tag/profile ID</param>
		private void ProcessEvent( int eventID )
		{
			EventProfile profile = new Arena.Event.EventProfile( eventID );

			if ( profile.RegistrationActive && profile.RegistrationLimited &&
				profile.ProfileActiveMemberCount >= profile.RegistrationMaximumIndividuals )
			{
				Profile containsWaitingListProfile = ( profile.ChildProfiles.Count > 0 ) ?  containsWaitingListProfile = profile : containsWaitingListProfile = new Arena.Event.EventProfile( profile.ParentProfileID );
				Profile waitingListProfile = containsWaitingListProfile.ChildProfiles.FindByName( "Waiting List" );

				if ( waitingListProfile != null && waitingListProfile.Active )
				{
					hlWaitingList.Visible = true;
					hlWaitingList.Text = ( ButtonTextSetting != string.Empty ) ? ButtonTextSetting : profile.DescriptionLabel + " Full - Waiting List Sign-up";
					hlWaitingList.NavigateUrl = string.Format( "~/default.aspx?page={0}&event={1}", RegistrationPageSetting, waitingListProfile.ProfileID );
				}
			}
		}
	}
}