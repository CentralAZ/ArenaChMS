using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;

using Arena.Core;
using Arena.Custom.Cccev.CheckIn.DataLayer;

namespace Arena.Custom.Cccev.CheckIn.Entity
{
    public static class OccurrenceCollectionExtension
    {
        public static void LoadOccurrencesBySystemIDAndDateRange(this OccurrenceCollection oc, int systemID, DateTime startDate, DateTime endDate)
        {
            SqlDataReader reader = new XOccurrenceData().GetOccurrencesBySystemIDAndDateRange(systemID, startDate, endDate);

            while (reader.Read())
            {
                oc.Add(new Occurrence((int)reader["occurrence_id"]));
            }

            reader.Close();
        }
    }
}