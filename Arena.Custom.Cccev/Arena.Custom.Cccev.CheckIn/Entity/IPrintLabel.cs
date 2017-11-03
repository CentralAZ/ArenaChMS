/**********************************************************************
* Description:	TBD
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created:	TBD
*
* $Workfile: IPrintLabel.cs $
* $Revision: 8 $ 
* $Header: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/IPrintLabel.cs   8   2010-01-20 15:42:48-07:00   JasonO $
* 
* $Log: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/IPrintLabel.cs $
*  
*  Revision: 8   Date: 2010-01-20 22:42:48Z   User: JasonO 
*  Adding support for declaring print-provider at the module level. 
*  
*  Revision: 7   Date: 2009-09-15 23:38:17Z   User: JasonO 
*  Implementing R# recommendations. 
*  
*  Revision: 6   Date: 2009-06-18 17:43:42Z   User: nicka 
*  Changes to handle new IPrintLabel that requires kiosk as discussed here: 
*  http://checkinwizard.codeplex.com/Thread/View.aspx?ThreadId=57675 
*  
*  Revision: 5   Date: 2009-02-24 18:29:49Z   User: JasonO 
*  Fixing typo. 
*  
*  Revision: 4   Date: 2009-02-24 18:18:51Z   User: JasonO 
*  Updating org setting keys for provider class Luids. 
*  
*  Revision: 3   Date: 2009-01-06 17:35:06Z   User: JasonO 
*  
*  Revision: 2   Date: 2009-01-06 16:34:25Z   User: JasonO 
*  
*  Revision: 1   Date: 2009-01-06 16:12:00Z   User: JasonO 
**********************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using Arena.Core;
using Arena.Computer;
using Arena.Exceptions;

namespace Arena.Custom.Cccev.CheckIn.Entity
{
    public interface IPrintLabel
    {
        void Print(FamilyMember person, IEnumerable<Occurrence> occurrences, OccurrenceAttendance attendance, ComputerSystem kiosk);
    }

    public static class PrintLabelHelper
    {
        public static IPrintLabel GetPrintLabelClass(int printLabelSystemID)
        {
            Lookup printLabelSystem = new Lookup(printLabelSystemID);
            Assembly assembly = Assembly.Load(printLabelSystem.Qualifier2);

            if (assembly == null)
            {
                return null;
            }

            Type type = assembly.GetType(printLabelSystem.Qualifier8) ?? 
                assembly.GetType(printLabelSystem.Qualifier2 + "." + printLabelSystem.Qualifier8);

            if (type == null)
            {
                throw new ArenaApplicationException(string.Format("Could not find '{0}' class in '{1}' assembly.", printLabelSystem.Qualifier2, printLabelSystem.Qualifier8));
            }

            return (IPrintLabel)Activator.CreateInstance(type);
        }
    }
}
