/**********************************************************************
* Description:	Determines if the currently logged in person can view
*				"The Person" (http://community.arenachms.com/forums/permalink/14069/14069/ShowThread.aspx#14069)
* Created By:	Nick Airdo @ Central Christian Church of the East Valley
* Date Created:	9/6/2011 09:11:59 AM
*
* $Workfile: $
* $Revision: $ 
* $Header: $
* 
* $Log: $
**********************************************************************/
using System;

using Arena.Portal;
using Arena.Core;

namespace ArenaWeb.UserControls.Custom.Cccev.Security
{
	public partial class AllowedToView : PortalControl
	{
		private Person person;

		#region Module Settings
		[BooleanSetting("Check Family?", "Checks if the currently logged in person is a family member of 'The Person'. (default true)", true, true)]
		public bool CheckFamilySetting { get { return bool.Parse( Setting( "CheckFamily", "true", true ) ); } }
        
		[BooleanSetting("Throw Security Exception?", "If true, throws a security exception the currently logged in person is not able to view 'The Person' using the enabled security checks. (default true)", false, true)]
		public bool ThrowSecurityExceptionSetting { get { return bool.Parse( Setting( "ThrowSecurityException", "true", false ) ); } }

		[PageSetting( "Unauthorized to View Page", "The page to show if the currently logged in person is not authorized to view 'The Person'. (default not set)", false )]
		public string UnauthorizedPageSetting { get { return Setting( "UnauthorizedPage", "", false ); } }

		#endregion

		#region Event Handlers

		protected void Page_Load( object sender, EventArgs e )
		{
			if ( ! IsPostBack )
			{
				bool pass = true;

				LoadThePerson();

				// Custom Developers: this is the pattern...
				if ( pass && CheckFamilySetting )
				{
					pass = IsFamilyMember();
				}

				// Custom Developers: add any additional "pass" checks here...
				// YOUR CODE GOES ABOVE THIS BLOCK
				// (leave this block here right before the final pass check)

				// This is the final check.  If it did not pass, then redirect or throw a security exception.
				if ( ! pass )
				{
					ExceptionOrRedirect();
				}
			}
		}

		private void ExceptionOrRedirect()
		{
			if ( ThrowSecurityExceptionSetting )
			{
				throw ( new UnauthorizedAccessException( string.Format( "{0} ({1}) is not allowed to view {2} ({3})", CurrentPerson.FullName, CurrentPerson.PersonID, person.FullName, person.PersonID) ) );
			}
			else if ( ! string.IsNullOrEmpty( UnauthorizedPageSetting ) )
			{
				Response.Redirect( string.Format( "~/default.aspx?page={0}", UnauthorizedPageSetting ) );
			}
			else
			{
				Response.Redirect( "~/default.aspx");
			}
		}
		
		/// <summary>
		/// Returns false if currently logged in person is not a family member of "The Person".
		/// </summary>
		/// <returns>false if not a family member</returns>
		private bool IsFamilyMember()
		{
			bool retval = false;
			foreach ( FamilyMember fm in person.Family().FamilyMembers )
			{
				if ( CurrentPerson.PersonID == fm.PersonID )
				{
					return true;
				}
			}

			return retval;
		}

		
		#endregion

		#region DO NOT TOUCH -- this mirror's Arena's "LoadPerson" logic

		private void LoadThePerson()
		{
			if (ArenaContext.Current.SelectedPerson != null)
			{
				this.person = ArenaContext.Current.SelectedPerson;
			}

			if (this.person == null)
			{
				string[] allKeys = base.Request.QueryString.AllKeys;
				string[] array = allKeys;
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					string a;
					if ((a = text.ToUpper()) != null)
					{
						if (!(a == "PERSON"))
						{
							if (!(a == "GUID"))
							{
								goto IL_D5;
							}
						}
						else
						{
							try
							{
								this.person = new Person(int.Parse(base.Request.QueryString.Get(text)));
								goto IL_D5;
							}
							catch
							{
								goto IL_D5;
							}
						}
						try
						{
							this.person = new Person(new Guid(base.Request.QueryString.Get(text)));
						}
						catch
						{
						}
					}
					IL_D5:;
				}

				if (this.person == null || this.person.PersonID == -1)
				{
					throw new ModuleException( base.CurrentPortalPage, base.CurrentModule, "This module requires a person in the ArenaContext.Current.SelectedPerson, a numeric person ID, or valid person GUID." );
				}
			}
		}

		#endregion


	} // end class

} // end namespace