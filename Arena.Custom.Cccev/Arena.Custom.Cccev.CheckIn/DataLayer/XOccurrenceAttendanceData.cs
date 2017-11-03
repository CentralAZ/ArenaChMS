using System;
using System.Collections;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Arena.DataLib;

/**********************************************************************
* Description:	TBD
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created:	TBD
*
* $Workfile: XOccurrenceAttendanceData.cs $
* $Revision: 5 $ 
* $Header: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/DataLayer/XOccurrenceAttendanceData.cs   5   2009-02-18 13:15:18-07:00   JasonO $
* 
* $Log: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/DataLayer/XOccurrenceAttendanceData.cs $
*  
*  Revision: 5   Date: 2009-02-18 20:15:18Z   User: JasonO 
*  
*  Revision: 4   Date: 2009-02-18 18:14:30Z   User: JasonO 
*  
*  Revision: 3   Date: 2009-01-27 20:51:08Z   User: JasonO 
*  
*  Revision: 2   Date: 2009-01-27 16:02:01Z   User: JasonO 
**********************************************************************/

namespace Arena.Custom.Cccev.CheckIn.DataLayer
{
    public class XOccurrenceAttendanceData : SqlData
    {
        public SqlDataReader GetOccurrenceAttendanceByStartDateAndPersonID(DateTime startDate, int personID)
        {
            ArrayList list = new ArrayList();
            list.Add(new SqlParameter("@StartDate", startDate));
            list.Add(new SqlParameter("@PersonID", personID));

            try
            {
                return this.ExecuteSqlDataReader("cust_cccev_ckin_sp_get_occurrence_attendance_byPersonIDAndStartDate", list);
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                list = null;
            }
        }
    }
}
