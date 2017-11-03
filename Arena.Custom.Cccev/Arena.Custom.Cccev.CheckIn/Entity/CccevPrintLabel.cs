/**********************************************************************
* Description:	Class that handles printing check-in labels for children
*               who check in to a class.
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created:	2009-01-06
*
* $Workfile: CccevPrintLabel.cs $
* $Revision: 19 $ 
* $Header: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/CccevPrintLabel.cs   19   2013-11-13 11:24:35-07:00   nicka $
* 
* $Log: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/CccevPrintLabel.cs $
*  
*  Revision: 19   Date: 2013-11-13 18:24:35Z   User: nicka 
*  Added new EpiPen flag/symbol that appears on attendance labels and nametags 
*  if the "Cccev.EpiPenReleaseAttributeID" org setting is tied to an attribute 
*  of type Document and a document is attached to the person. 
*  
*  Revision: 18   Date: 2013-04-29 22:37:35Z   User: nicka 
*  If both the Cccev.DisplayRoomNameOnNameTag and 
*  Cccev.UseTagNameInsteadOfRoomName organization settings are true, it will 
*  print the linked tag name (if any) in place of the room name on the 
*  name-tag label. 
*  
*  Revision: 17   Date: 2012-11-26 23:30:25Z   User: nicka 
*  Only try to print to kiosk (instead of location) if the kiosk has a printer 
*  (line 97). 
*  
*  Revision: 16   Date: 2010-01-19 23:17:48Z   User: JasonO 
*  
*  Revision: 15   Date: 2009-10-29 16:35:34Z   User: JasonO 
*  Adding enhanced support for at kiosk/at location printing via 
*  OccurrenceTypeReports. 
*  
*  Revision: 14   Date: 2009-10-27 17:16:46Z   User: JasonO 
*  
*  Revision: 13   Date: 2009-10-08 17:18:18Z   User: JasonO 
*  Merging/updating to make changes for 1.2 release. 
*  
*  Revision: 12   Date: 2009-09-16 15:32:00Z   User: JasonO 
*  
*  Revision: 11   Date: 2009-09-15 23:38:17Z   User: JasonO 
*  Implementing R# recommendations. 
*  
*  Revision: 10   Date: 2009-06-18 22:46:04Z   User: nicka 
*  
*  Revision: 9   Date: 2009-06-18 22:45:33Z   User: nicka 
*  DanielH|HDC patch 
*  
*  Revision: 8   Date: 2009-06-18 17:43:42Z   User: nicka 
*  Changes to handle new IPrintLabel that requires kiosk as discussed here: 
*  http://checkinwizard.codeplex.com/Thread/View.aspx?ThreadId=57675 
*  
*  Revision: 7   Date: 2009-03-05 15:36:26Z   User: nicka 
*  use nick name only if available else use first name 
*  
*  Revision: 6   Date: 2009-03-05 02:01:53Z   User: nicka 
*  Change label to use Nick name instead of First name 
*  
*  Revision: 5   Date: 2009-01-27 13:43:20Z   User: nicka 
*  change health notes to show until 1st grade 
*  
*  Revision: 4   Date: 2009-01-07 02:41:12Z   User: nicka 
*  Added logic to change Age Group based on age and or grade. Also only show 
*  Health Notes details if person under 2 years old. 
*  
*  Revision: 3   Date: 2009-01-07 00:21:01Z   User: JasonO 
*  
*  Revision: 2   Date: 2009-01-06 22:14:13Z   User: JasonO 
*  Sprint completion! 
*  
*  Revision: 1   Date: 2009-01-06 17:35:18Z   User: JasonO 
**********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Arena.Core;
using Arena.Computer;
using Arena.Organization;
using Arena.Custom.Cccev.DataUtils;

namespace Arena.Custom.Cccev.CheckIn.Entity
{
    internal class CccevPrintLabel : IPrintLabel
    {
        private readonly Organization.Organization organization;
        private CheckinLabel label;
        private Location location;

        public CccevPrintLabel()
        {
            organization = ArenaContext.Current.Organization;
        }

        /// <summary>
        /// IPrintLabel implementation to print out name tags.
        /// </summary>
        /// <param name="attendee"><see cref="Arena.Core.FamilyMember">FamilyMember</see> person attending</param>
        /// <param name="occurrences">IEnumerable<<see cref="Arena.Core.Occurrence">Occurrence</see> of services</param>
        /// <param name="attendance"><see cref="Arena.Core.OccurrenceAttendance">OccurrenceAttendance</see> attendance record</param>
		/// <param name="kiosk"><see cref="Arena.Computer.ComputerSystem">ComputerSystem</see> the family is standing at</param>
        void IPrintLabel.Print(FamilyMember attendee, IEnumerable<Occurrence> occurrences, OccurrenceAttendance attendance, ComputerSystem kiosk)
        {
            location = new Location(occurrences.First().LocationID);
            InitLabel(attendee, occurrences, attendance, location);

            OccurrenceTypeReportCollection reports = new OccurrenceTypeReportCollection(occurrences.First().OccurrenceTypeID);
            var report = reports.OfType<OccurrenceTypeReport>().FirstOrDefault();

			if ( ( report != null && report.UseDefaultPrinter && kiosk.Printer != null) || 
                location.Printer.PrinterName.Equals("[Kiosk]", StringComparison.CurrentCultureIgnoreCase))
            {
                label.PrintAllLabels(kiosk.Printer.PrinterName);
            }
            else
            {
                label.PrintAllLabels(location.Printer.PrinterName);
            }
        }

		/// <summary>
		/// Intialize a person's label set with the information from the given person, occurrence(s),
		/// and attendance record.
		/// </summary>
        /// <param name="attendee"><see cref="Arena.Core.Person">Person</see> attending occurrence</param>
        /// <param name="occurrences">Collection of <see cref="Arena.Core.Occurrence">Occurrences</see> the attendee can check into</param>
        /// <param name="attendance"><see cref="Arena.Core.OccurrenceAttendance">OccurrenceAttendance</see> to be updated with failure status if print job fails</param>
        /// <param name="loc"></param>
        private void InitLabel(Person attendee, IEnumerable<Occurrence> occurrences, OccurrenceAttendance attendance, Location loc)
        {
			// Change required for Extreme Week (formerly VBS) Request #7093
			// http://arena/arena/default.aspx?page=3989&parCurrentGroup=Assignments&assignmentID=7093
			string displayValue = loc.LocationName;
			var x = occurrences.First();
			int tagId = occurrences.First().OccurrenceType.SyncWithProfile;

			if ( tagId != Constants.NULL_INT )
			{
				bool useTagName = false;
				if ( bool.TryParse( organization.Settings["Cccev.UseTagNameInsteadOfRoomName"], out useTagName ) && useTagName )
				{
					displayValue = ProfileCache( tagId );
				}
			}

            label = new CheckinLabel
            {
                FirstName = attendee.NickName.Trim() != string.Empty ? attendee.NickName : attendee.FirstName, 
                LastName = attendee.LastName, 
                FullName = string.Format("{0} {1}", attendee.NickName, attendee.LastName), 
                SecurityToken = attendance.SecurityCode, 
                CheckInDate = attendance.CheckInTime, 
                RoomName = displayValue
            };

		    StringBuilder services = new StringBuilder();

            foreach (Occurrence o in occurrences)
            {
                if (services.Length > 0)
                    services.Append(", ");

                services.Append(o.StartTime.ToShortTimeString());
            }

            label.Services = services.ToString();
            label.BirthdayDate = attendee.BirthDate;
			SetAgeGroup( attendee );
            SetLabelFlags(attendee);
            LoadOrgSettings();
        }

		/// <summary>
		/// Fetches the tag name from cache for the given ID and/or puts it into cache if not there.
		/// </summary>
		/// <param name="profileId">The profileId</param>
		/// <returns>the name of the profile</returns>
		private string ProfileCache( int profileId )
		{
			string cachekey = string.Format( "Cccev.ProfileIdToName:{0}", profileId );
			var tagName = HttpRuntime.Cache.Get( cachekey ) as string;
			if ( tagName == null )
			{
				Profile profile = new Profile( profileId );
				tagName = profile.Name;
				HttpRuntime.Cache.Insert( cachekey, tagName, null, DateTime.UtcNow.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
			}

			return tagName;
		}

		/// <summary>
		/// Set these global label values based on the Org settings
		/// </summary>
        private void LoadOrgSettings()
        {
            label.AttendanceLabelTitle = organization.Settings["Cccev.AttendanceLabelTitle"];
            label.BirthdayImageFile = organization.Settings["Cccev.BirthdayImageFile"];
            label.Footer = organization.Settings["Cccev.ClaimCardFooter"];
            label.ClaimCardTitle = organization.Settings["Cccev.ClaimCardTitle"];
            label.HealthNotesTitle = organization.Settings["Cccev.HealthNotesTitle"];
            label.LogoImageFile = organization.Settings["Cccev.LogoImageFile"];
            label.ParentsInitialsTitle = organization.Settings["Cccev.ParentsInitialsTitle"];
            label.ServicesTitle = organization.Settings["Cccev.ServicesLabel"];
        }

		/// <summary>
		/// This method will set the label's flags and health notes for the given person.
		/// </summary>
		/// <param name="attendee"></param>
        private void SetLabelFlags(Person attendee)
        {
            PersonAttribute selfCheckOut = new PersonAttribute(attendee.PersonID, int.Parse(organization.Settings["Cccev.SelfCheckOutAttributeID"]));
            PersonAttribute legalNotes = new PersonAttribute(attendee.PersonID, int.Parse(organization.Settings["Cccev.LegalNotesAttributeID"]));
            PersonAttribute healthNotes = new PersonAttribute(attendee.PersonID, int.Parse(organization.Settings["Cccev.HealthNotesAttributeID"]));

			if ( !string.IsNullOrEmpty( organization.Settings["Cccev.EpiPenReleaseAttributeID"] ) )
			{
				PersonAttribute epiPenRelease = new PersonAttribute( attendee.PersonID, int.Parse( organization.Settings["Cccev.EpiPenReleaseAttributeID"] ) );
				label.EpiPenFlag = ( epiPenRelease.HasIntValue );
			}

            label.SelfCheckOutFlag = (selfCheckOut.IntValue.Equals(1));
            label.LegalNoteFlag = (!legalNotes.StringValue.Equals(string.Empty));

			if ( !healthNotes.StringValue.Equals( string.Empty ) )
			{
				label.HealthNoteFlag = true;
				// This was removed after speaking with Laurie (NA 1/26/2009)
				// Don't print health notes if child greater than 1st grade.
				//if ( !( attendee.GraduationDate > DateTime.Parse( "1/1/1900" )
				//    && Person.CalculateGradeLevel( attendee.GraduationDate, organization.GradePromotionDate ) >= 1 ) )
				//{
					label.HealthNotes = healthNotes.StringValue;
				//}
			}
			else
			{
				label.HealthNoteFlag = false;
			}
        }

		/// <summary>
		/// This sets the label's "Age Group" value to a specific text (as per Children's
		/// ministries) based on the person's age and or grade.
		/// Basically:
		///		* if the person is under the age of 2, display age in months;
		///     * if they are in grade school, display the grade
		///		* otherwise display age in years.
		/// </summary>
		/// <param name="attendee"></param>
		private void SetAgeGroup( Person attendee )
		{
			if ( attendee.Age < 2 )
			{
				label.AgeGroup = String.Format( "{0} months", DateUtils.GetAgeInMonths( attendee.BirthDate ) );
			}
			else
			{
			    int grade = Person.CalculateGradeLevel( attendee.GraduationDate, organization.GradePromotionDate );
			    label.AgeGroup = grade > -1 ? String.Format( "{0} grade", Person.GetGradeName( grade ) ) : String.Format( "{0} year olds", attendee.Age );
			}
		}
    }
}
