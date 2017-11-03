/**********************************************************************
* Description:  ViewModel to represent a person requesting to check into a class
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created: 11/2/2010
*
* $Workfile: PersonCheckInRequest.cs $
* $Revision: 1 $
* $Header: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/PersonCheckInRequest.cs   1   2010-11-03 15:22:06-07:00   JasonO $
*
* $Log: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/PersonCheckInRequest.cs $
*  
*  Revision: 1   Date: 2010-11-03 22:22:06Z   User: JasonO 
*  Refactoring to bring more data regarding results of check in process out to 
*  UI level. 
**********************************************************************/

using System.Collections.Generic;
using Arena.Core;

namespace Arena.Custom.Cccev.CheckIn
{
	public class PersonCheckInRequest
	{
		public int PersonID { get; set; }
		public FamilyMember FamilyMember { get; set; }
		public IList<Occurrence> Occurrences { get; set; }
	}
}
