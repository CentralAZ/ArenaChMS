using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Arena.DataLib;

namespace Arena.Custom.Cccev.CheckIn.DataLayer
{
    public class CccevSecurityCodeData : SqlData
    {
        public string GetNextSecurityCode()
        {
            string code = string.Empty;
            ArrayList list = new ArrayList();
            SqlParameter output = new SqlParameter("@FullCode", SqlDbType.Char, 6);
            output.Direction = ParameterDirection.Output;
            list.Add(output);

            try
            {
                this.ExecuteNonQuery("cust_cccev_ckin_sp_get_security_code", list);
                code = ((SqlParameter)list[list.Count - 1]).Value.ToString();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                list = null;
            }

            return code;
        }
    }
}
