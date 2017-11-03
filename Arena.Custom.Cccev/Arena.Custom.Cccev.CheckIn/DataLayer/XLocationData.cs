/**********************************************************************
* Description:  Extensions on Arena.DataLayer.Organization.LocationData functionality
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created: 9/22/2009
*
* $Workfile: XLocationData.cs $
* $Revision: 1 $
* $Header: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/DataLayer/XLocationData.cs   1   2009-09-23 15:38:01-07:00   JasonO $
*
* $Log: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/DataLayer/XLocationData.cs $
*  
*  Revision: 1   Date: 2009-09-23 22:38:01Z   User: JasonO 
**********************************************************************/

using System;
using System.Collections;
using System.Data.SqlClient;
using Arena.DataLib;

namespace Arena.Custom.Cccev.CheckIn.DataLayer
{
    public class XLocationData : SqlData
    {
        public int GetLocationHeadCountByDate(int locationID, DateTime startDate)
        {
            ArrayList list = new ArrayList();
            list.Add(new SqlParameter("@LocationID", locationID));
            list.Add(new SqlParameter("@StartDate", startDate));

            try
            {
                return (int) this.ExecuteScalar("cust_cccev_ckin_sp_get_location_head_count_by_date", list);
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
