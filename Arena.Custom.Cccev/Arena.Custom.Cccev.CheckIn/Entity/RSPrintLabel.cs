/**********************************************************************
* Description:  Print Provider for Reporting Services
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created: 9/15/2009
*
* $Workfile: RSPrintLabel.cs $
* $Revision: 4 $
* $Header: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/RSPrintLabel.cs   4   2013-04-02 16:14:51-07:00   nicka $
*
* $Log: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/RSPrintLabel.cs $
*  
*  Revision: 4   Date: 2013-04-02 23:14:51Z   User: nicka 
*  Daniel Hazelbaker's Rev 77 change: Work around a bug in the way the 
*  ReportPrinter.PrintReports(jobs) method works. It uses impersonation which 
*  seems to cause some problems with printing. 
*  
*  Revision: 3   Date: 2010-09-23 20:53:58Z   User: JasonO 
*  Implementing changes suggested by HDC. 
*  
*  Revision: 2   Date: 2009-10-09 00:07:46Z   User: JasonO 
*  
*  Revision: 1   Date: 2009-09-15 23:38:45Z   User: JasonO 
*  Adding Reporting Services Provider. 
**********************************************************************/

using System.Collections.Generic;
using Arena.CheckIn;
using Arena.Computer;
using Arena.Core;
using Arena.Reporting;

namespace Arena.Custom.Cccev.CheckIn.Entity
{
    internal class RSPrintLabel : IPrintLabel
    {
        public void Print(FamilyMember person, IEnumerable<Occurrence> occurrences, OccurrenceAttendance attendance, ComputerSystem system)
        {
            // Don't new up Kiosk w/o empty constructor, might be too costly
            Kiosk kiosk = new Kiosk() { System = system };
            var jobs = kiosk.GetPrintJobs(attendance);
            ReportPrinter printer = new ReportPrinter();
			// Work around a bug in the way the ReportPrinter.PrintReports(jobs) method works...
			// ...it uses impersonation which seems to cause some problems with printing on Win 2008 R2 SP1.
			foreach (ReportPrintJob job in jobs)
			{
				printer.PrintReport(job.PrinterName, job.ReportPath, job.Copies, job.Landscape, job.Parameters, job.PrinterName);
			}
        }
    }
}
