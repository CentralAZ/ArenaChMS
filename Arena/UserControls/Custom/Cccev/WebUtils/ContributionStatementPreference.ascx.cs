/**********************************************************************
* Description:  Displays and or sets the user's statement preference
*				such as quarterly, yearly, none.
* Created By:	Nick Airdo @ Central Christian Church of the East Valley
* Date Created:	9/16/2010 08:40:59 PM
*
* $Workfile: ContributionStatementPreference.ascx.cs $
* $Revision: 1 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/ContributionStatementPreference.ascx.cs   1   2010-10-18 16:16:25-07:00   nicka $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/ContributionStatementPreference.ascx.cs $
*  
*  Revision: 1   Date: 2010-10-18 23:16:25Z   User: nicka 
*  Module for allowing public people from updating their contribution 
*  statement frequency. 
**********************************************************************/

using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Arena.Core;
using Arena.Portal;

namespace ArenaWeb.UserControls.Custom.Cccev.WebUtils
{
	public partial class ContributionStatementPreference : PortalControl
	{
		[ListFromSqlSetting( "Frequency Preference Attribute", "The person attribute that holds each person's frequency choice.  Note: The attribute must be of type lookup.", true, "",
			"SELECT [attribute_id], [attribute_name] FROM [core_attribute] WHERE [attribute_type] = 3 ORDER BY [attribute_name]" )]
		public string FrequencyPreferenceAttributeIDSetting { get { return Setting( "FrequencyPreferenceAttributeID", "", true ); } }

		protected void Page_Load( object sender, EventArgs e )
		{
			if ( ! IsPostBack )
			{
				smpScripts.Scripts.Add( new ScriptReference( string.Format( "~/{0}", BasePage.JQUERY_INCLUDE ) ) );

				if ( CurrentPerson != null )
				{
					BindOptionsToList();
				}
			}
		}

		/// <summary>
		/// Binds the possible frequency options to the RadioButtonList.
		/// </summary>
		private void BindOptionsToList()
		{
			PersonAttribute selectedFrequency = new PersonAttribute( CurrentPerson.PersonID, int.Parse( FrequencyPreferenceAttributeIDSetting ) );
			LookupCollection frequencies = new LookupType( int.Parse( selectedFrequency.TypeQualifier ) ).Values;
			foreach ( Lookup item in frequencies )
			{
				rblPreference.Items.Add( new ListItem( item.Qualifier8, item.LookupID.ToString() ) );
			}

			if ( selectedFrequency != null )
			{
				rblPreference.SelectedValue = selectedFrequency.IntValue.ToString();
			}
		}

		protected void rblPreference_CheckedChanged( object sender, EventArgs e )
		{
			pnlFail.Visible = false;
			pnlSuccess.Visible = false;
			try
			{
				PersonAttribute selectedFrequency = new PersonAttribute( CurrentPerson.PersonID, int.Parse( FrequencyPreferenceAttributeIDSetting ) );
				selectedFrequency.IntValue = int.Parse( rblPreference.SelectedValue );

				selectedFrequency.Save( CurrentOrganization.OrganizationID, CurrentUser.Identity.Name );
				pnlSuccess.Visible = true;
			}
			catch
			{
				pnlFail.Visible = true;
			}
		}
	}
}