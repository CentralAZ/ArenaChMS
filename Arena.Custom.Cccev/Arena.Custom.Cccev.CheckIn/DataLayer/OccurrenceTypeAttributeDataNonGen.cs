/**********************************************************************
* Description:
* Created By:	
* Date Created:	
*
* $Workfile: OccurrenceTypeAttributeDataNonGen.cs $
* $Revision: 2 $ 
* $Header: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/DataLayer/OccurrenceTypeAttributeDataNonGen.cs   2   2009-02-18 11:14:30-07:00   JasonO $
* 
* $Log: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/DataLayer/OccurrenceTypeAttributeDataNonGen.cs $
*  
*  Revision: 2   Date: 2009-02-18 18:14:30Z   User: JasonO 
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
		public SqlDataReader GetOccurrenceTypeAttributeByGroupId( int groupId )
		{
			ArrayList lst = new ArrayList();

			lst.Add( new SqlParameter( "@GroupId", groupId ) );

			try
			{
				return this.ExecuteSqlDataReader( "cust_cccev_ckin_sp_get_occurrenceTypeAttributeByGroupId", lst );
			}
			catch ( SqlException ex )
			{
				throw ex;
			}
			finally
			{
				lst = null;
			}
		}

		public DataTable GetOccurrenceTypeAttributeByGroupId_DT( int groupId )
		{
			ArrayList lst = new ArrayList();

			lst.Add( new SqlParameter( "@GroupId", groupId ) );

			try
			{
				return this.ExecuteDataTable( "cust_cccev_ckin_sp_get_occurrenceTypeAttributeByGroupId", lst );
			}
			catch ( SqlException ex )
			{
				throw ex;
			}
			finally
			{
				lst = null;
			}
		}
	}
}


