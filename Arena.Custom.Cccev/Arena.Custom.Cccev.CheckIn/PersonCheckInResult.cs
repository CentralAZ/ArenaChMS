/**********************************************************************
* Description:  ViewModel to represent checkin result
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created: 11/2/2010
*
* $Workfile: PersonCheckInResult.cs $
* $Revision: 1 $
* $Header: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/PersonCheckInResult.cs   1   2010-11-03 15:22:06-07:00   JasonO $
*
* $Log: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/PersonCheckInResult.cs $
*  
*  Revision: 1   Date: 2010-11-03 22:22:06Z   User: JasonO 
*  Refactoring to bring more data regarding results of check in process out to 
*  UI level. 
**********************************************************************/

using System.Collections.Generic;
using Arena.Core;

namespace Arena.Custom.Cccev.CheckIn
{
	public class PersonCheckInResult
	{
		public FamilyMember FamilyMember { get; set; }
		public OccurrenceAttendance Attendance { get; set; }
		public IList<CheckInResult> CheckInResults { get; set; }
		public int SessionID { get; set; }
		public bool IsPrintSuccessful { get; set; }
	}

	public class CheckInResult
	{
		public int PersonID { get; set; }
		public Occurrence Occurrence { get; set; }
		public bool IsCheckInSuccessful { get; set; }
	}
}
