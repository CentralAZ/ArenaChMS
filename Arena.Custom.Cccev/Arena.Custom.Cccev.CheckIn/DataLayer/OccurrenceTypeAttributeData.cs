/**********************************************************************
* Description:  Holds the extended, custom properties for an Occurrence
*               Type (a.k.a. Attendance Type).
* Created By:   Nick Airdo
* Date Created: 11/12/2008	
*
* $Workfile: OccurrenceTypeAttributeData.cs $
* $Revision: 6 $ 
* $Header: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/DataLayer/OccurrenceTypeAttributeData.cs   6   2009-10-08 15:09:38-07:00   JasonO $
* 
* $Log: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/DataLayer/OccurrenceTypeAttributeData.cs $
*  
*  Revision: 6   Date: 2009-10-08 22:09:38Z   User: JasonO 
*  
*  Revision: 5   Date: 2009-09-23 22:38:02Z   User: JasonO 
*  
*  Revision: 4   Date: 2009-02-18 18:14:30Z   User: JasonO 
*  
*  Revision: 3   Date: 2009-02-17 00:52:54Z   User: nicka 
*  Changed OccurrenceTypeAttribute AbilityLevel property from column to 
*  skel_bone (multi value) 
*  
*  Revision: 2   Date: 2008-12-09 22:23:33Z   User: JasonO 
*  Removing min/max age and grade. 
*  
*  Revision: 1   Date: 2008-11-12 21:53:34Z   User: nicka 
**********************************************************************/
using System;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using Arena.DataLib;

namespace Arena.Custom.Cccev.CheckIn.DataLayer

{
	/// <summary>
	/// 
	/// </summary>
	public partial class OccurrenceTypeAttributeData : SqlData
	{
		/// <summary>
		/// Class constructor.
		/// </summary>
		public OccurrenceTypeAttributeData()
		{
		}

		/// <summary>
		/// Returns a <see cref="System.Data.SqlClient.SqlDataReader">SqlDataReader</see> object
		/// containing the OccurrenceTypeAttribute with the ID specified
		/// </summary>
		/// <param name="occurrenceTypeAttributeID">OccurrenceTypeAttribute ID</param>
		/// <returns><see cref="System.Data.SqlClient.SqlDataReader">SqlDataReader</see></returns>
		public SqlDataReader GetOccurrenceTypeAttributeByID(int occurrenceTypeAttributeId)
		{
			ArrayList lst = new ArrayList();

			lst.Add(new SqlParameter("@OccurrenceTypeAttributeId", occurrenceTypeAttributeId));

			try
			{
				return this.ExecuteSqlDataReader("cust_cccev_ckin_sp_get_occurrenceTypeAttributeByID", lst);
			}
			catch (SqlException ex)
			{
				throw ex;
			}
			finally
			{
				lst = null;
			}
		}
		
		public SqlDataReader GetOccurrenceTypeAttributeByOccurrenceTypeId(int occurrenceTypeId)
		{
			ArrayList lst = new ArrayList();

			lst.Add(new SqlParameter("@OccurrenceTypeId", occurrenceTypeId));

			try
			{
				return this.ExecuteSqlDataReader("cust_cccev_ckin_sp_get_occurrenceTypeAttributeByOccurrenceTypeId", lst);
			}
			catch (SqlException ex)
			{
				throw ex;
			}
			finally
			{
				lst = null;
			}
		}

		public DataTable GetOccurrenceTypeAttributeByOccurrenceTypeId_DT(int occurrenceTypeId)
		{
			ArrayList lst = new ArrayList();

			lst.Add(new SqlParameter("@OccurrenceTypeId", occurrenceTypeId));

			try
			{
				return this.ExecuteDataTable("cust_cccev_ckin_sp_get_occurrenceTypeAttributeByOccurrenceTypeId", lst);
			}
			catch (SqlException ex)
			{
				throw ex;
			}
			finally
			{
				lst = null;
			}
		}

		/// <summary>
		/// deletes a OccurrenceTypeAttribute record.
		/// </summary>
		/// <param name="roleID">The poll_id key to delete.</param>
		public void DeleteOccurrenceTypeAttribute(int occurrenceTypeAttributeId)
		{
			ArrayList lst = new ArrayList();

			lst.Add(new SqlParameter("@OccurrenceTypeAttributeId", occurrenceTypeAttributeId));

			try
			{
				this.ExecuteNonQuery("cust_cccev_ckin_sp_del_occurrenceTypeAttribute", lst);
			}
			catch (SqlException ex)
			{
				throw ex;
			}
			finally
			{
				lst = null;
			}

		}

        /// <summary>
		/// saves OccurrenceTypeAttribute record
		/// </summary>
		/// <returns><see cref="System.Data.SqlClient.SqlDataReader">SqlDataReader</see></returns>
		public int SaveOccurrenceTypeAttribute(int occurrenceTypeAttributeId, int occurrenceTypeId, bool isSpecialNeeds, string lastNameStartingLetter, 
            string lastNameEndingLetter, bool isRoomBalancing, string userId)
		{
			ArrayList lst = new ArrayList();
			
			lst.Add(new SqlParameter("@OccurrenceTypeAttributeId", occurrenceTypeAttributeId));
			lst.Add(new SqlParameter("@OccurrenceTypeId", occurrenceTypeId));
			lst.Add(new SqlParameter("@IsSpecialNeeds", isSpecialNeeds));
            lst.Add(new SqlParameter("@IsRoomBalancing", isRoomBalancing));
			lst.Add(new SqlParameter("@LastNameStartingLetter", lastNameStartingLetter));
			lst.Add(new SqlParameter("@LastNameEndingLetter", lastNameEndingLetter));
			lst.Add(new SqlParameter("@UserId", userId));
			
			SqlParameter paramOut = new SqlParameter();
			paramOut.ParameterName = "@ID";
			paramOut.Direction = ParameterDirection.Output;
			paramOut.SqlDbType = SqlDbType.Int;
			lst.Add(paramOut);
			
			try
			{
				this.ExecuteNonQuery("cust_cccev_ckin_sp_save_occurrenceTypeAttribute", lst);
				return (int)((SqlParameter)(lst[lst.Count - 1])).Value;
			}
			catch (SqlException ex)
			{
				if (ex.Number == 2627) //Unique Key Violation
					return -1;
				else
					throw ex;
			}
			finally
			{
				lst = null;
			}
		}
	}
}


