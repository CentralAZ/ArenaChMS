using System;
using System.Data.SqlClient;
using Arena.Core;
using Arena.Custom.Cccev.CheckIn.DataLayer;
using Arena.Enums;

/**********************************************************************
* Description:	TBD
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created:	TBD
*
* $Workfile: OccurrenceAttendanceExtension.cs $
* $Revision: 4 $ 
* $Header: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/OccurrenceAttendanceExtension.cs   4   2009-09-09 11:06:39-07:00   JasonO $
* 
* $Log: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/OccurrenceAttendanceExtension.cs $
*  
*  Revision: 4   Date: 2009-09-09 18:06:39Z   User: JasonO 
*  
*  Revision: 3   Date: 2009-01-27 16:01:42Z   User: JasonO 
**********************************************************************/

namespace Arena.Custom.Cccev.CheckIn.Entity
{
    public static class OccurrenceAttendanceExtension
    {
        /// <summary>
        /// Attempts to load an OccurrenceAttendance by start time and person ID.
        /// </summary>
        /// <param name="oa">OccurrenceAttendance object this extension method will be used on (and return value).</param>
        /// <param name="startDate">occurrence start time</param>
        /// <param name="personID">person id</param>
        /// <returns>Returns null if nothing comes back from the database, otherwise returns a populated 
        /// <see cref"Arena.Core.OccurrenceAttendance">OccurrenceAttendance</see>.</returns>
        public static OccurrenceAttendance LoadOccurrenceAttendanceByStartDateAndPersonID(this OccurrenceAttendance oa, DateTime startDate, int personID)
        {
            SqlDataReader reader = new XOccurrenceAttendanceData().GetOccurrenceAttendanceByStartDateAndPersonID(startDate, personID);

            if (reader.Read())
            {
                oa = LoadOccurrenceAttendance(reader);

                if (oa.OccurrenceAttendanceID.Equals(-1))
                {
                    oa = null;
                }
            }
            else
            {
                oa = null;
            }

            reader.Close();
            return oa;
        }

        private static OccurrenceAttendance LoadOccurrenceAttendance(SqlDataReader reader)
        {
            OccurrenceAttendance oa = new OccurrenceAttendance();

            if (!reader.IsDBNull(reader.GetOrdinal("occurrence_attendance_id")))
                oa.OccurrenceAttendanceID = (int)reader["occurrence_attendance_id"];

            if (!reader.IsDBNull(reader.GetOrdinal("occurrence_id")))
                oa.OccurrenceID = (int)reader["occurrence_id"];

            if (!reader.IsDBNull(reader.GetOrdinal("person_id")))
                oa.PersonID = (int)reader["person_id"];

            oa.SecurityCode = reader["security_code"].ToString();

            if (!reader.IsDBNull(reader.GetOrdinal("attended")))
                oa.Attended = (bool)reader["attended"];

            if (!reader.IsDBNull(reader.GetOrdinal("check_in_time")))
                oa.CheckInTime = (DateTime)reader["check_in_time"];

            if (!reader.IsDBNull(reader.GetOrdinal("check_out_time")))
                oa.CheckOutTime = (DateTime)reader["check_out_time"];

            oa.Notes = reader["notes"].ToString();
            oa.Pager = reader["pager"].ToString();

            if (!reader.IsDBNull(reader.GetOrdinal("type")))
            {
                string type = reader["type"].ToString();
                oa.Type = (OccurrenceAttendanceType)Enum.Parse(typeof(OccurrenceAttendanceType), type);
            }

            return oa;
        }
    }
}
