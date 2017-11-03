/**********************************************************************
* Description:  This class will offer the ability to do nothing with a print request.
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created: 4/19/2011
*
* $Workfile: NullPrintLabel.cs $
* $Revision: 1 $
* $Header: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/NullPrintLabel.cs   1   2011-04-19 14:19:48-07:00   JasonO $
*
* $Log: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/NullPrintLabel.cs $
*  
*  Revision: 1   Date: 2011-04-19 21:19:48Z   User: JasonO 
*  Adding null print label object to give the option of not printing anything 
*  during checkin. 
**********************************************************************/

using System.Collections.Generic;
using Arena.Computer;
using Arena.Core;

namespace Arena.Custom.Cccev.CheckIn.Entity
{
    /// <summary>
    /// This class will offer the ability to do nothing with a print request. 
    /// </summary>
    public class NullPrintLabel : IPrintLabel
    {
        /// <summary>
        /// Empty Print method to consume IPrintLabel contract, but do nothing with it.
        /// </summary>
        /// <param name="person"></param>
        /// <param name="occurrences"></param>
        /// <param name="attendance"></param>
        /// <param name="kiosk"></param>
        public void Print(FamilyMember person, IEnumerable<Occurrence> occurrences, OccurrenceAttendance attendance, ComputerSystem kiosk)
        {
        }
    }
}
