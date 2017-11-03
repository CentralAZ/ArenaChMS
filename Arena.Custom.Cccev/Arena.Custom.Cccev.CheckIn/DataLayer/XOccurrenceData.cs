using System;
using System.Collections;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Arena.DataLib;

namespace Arena.Custom.Cccev.CheckIn.DataLayer
{
    public class XOccurrenceData : SqlData
    {
        public SqlDataReader GetOccurrencesBySystemIDAndDateRange(int systemID, DateTime startDate, DateTime endDate)
        {
            ArrayList list = new ArrayList();
            list.Add(new SqlParameter("@SystemID", systemID));
            list.Add(new SqlParameter("@StartDate", startDate));
            list.Add(new SqlParameter("@EndDate", endDate));

            try
            {
                return this.ExecuteSqlDataReader("cust_cccev_ckin_sp_get_occurrencesBySystemIDAndDateRange", list);
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
