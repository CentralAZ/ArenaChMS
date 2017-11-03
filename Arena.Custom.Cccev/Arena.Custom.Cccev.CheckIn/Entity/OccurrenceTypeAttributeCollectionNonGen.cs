/**********************************************************************
* Description:  Holds the extended, custom properties for an Occurrence
*               Type (a.k.a. Attendance Type).
* Created By:   Nick Airdo
* Date Created: 11/12/2008	
*
* $Workfile: OccurrenceTypeAttributeCollectionNonGen.cs $
* $Revision: 2 $ 
* $Header: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/OccurrenceTypeAttributeCollectionNonGen.cs   2   2009-02-16 17:52:54-07:00   nicka $
* 
* $Log: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/OccurrenceTypeAttributeCollectionNonGen.cs $
*  
*  Revision: 2   Date: 2009-02-17 00:52:54Z   User: nicka 
*  Changed OccurrenceTypeAttribute AbilityLevel property from column to 
*  skel_bone (multi value) 
*  
*  Revision: 1   Date: 2008-11-12 21:53:34Z   User: nicka 
**********************************************************************/
using System;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using Arena.Core;
using Arena.Custom.Cccev.CheckIn.DataLayer;

namespace Arena.Custom.Cccev.CheckIn.Entity
{
	/// <summary>
	/// Summary description for OccurrenceTypeAttribute.
	/// </summary>
	public partial class OccurrenceTypeAttributeCollection : ArenaCollectionBase
	{
		public void GetOccurrenceTypeAttributeByGroupId( int groupID )
		{
			SqlDataReader reader = new OccurrenceTypeAttributeData().GetOccurrenceTypeAttributeByGroupId( groupID );
			while ( reader.Read() )
				this.Add( new OccurrenceTypeAttribute( reader ) );
			reader.Close();
		}
	}
}