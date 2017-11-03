/**********************************************************************
* Description:
* Created By:	
* Date Created:	
*
* $Workfile: OccurrenceTypeAttributeCollection.cs $
* $Revision: 1 $ 
* $Header: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/OccurrenceTypeAttributeCollection.cs   1   2008-11-12 14:53:34-07:00   nicka $
* 
* $Log: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/OccurrenceTypeAttributeCollection.cs $
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
	[Serializable]
	public partial class OccurrenceTypeAttributeCollection : ArenaCollectionBase
	{
		#region Class Indexers

		public new OccurrenceTypeAttribute this [int index]
		{
			get
			{
				if (this.List.Count > 0)
				{
					return (OccurrenceTypeAttribute)this.List[index];
				}
				else
				{
					return null;
				}
			}
			set
			{
				this.List[index] = value;
			}
		}

		#endregion

		#region Constructors

		public OccurrenceTypeAttributeCollection()
		{
		}

		public OccurrenceTypeAttributeCollection(int occurrenceTypeId)
		{
			SqlDataReader reader = new OccurrenceTypeAttributeData().GetOccurrenceTypeAttributeByOccurrenceTypeId(occurrenceTypeId);
			while (reader.Read())
				this.Add(new OccurrenceTypeAttribute(reader));
			reader.Close();
		}

		#endregion

		#region Public Methods

		public void Add(OccurrenceTypeAttribute item)
		{
			this.List.Add(item);
		}

		public void Insert(int index, OccurrenceTypeAttribute item)
		{
			this.List.Insert(index, item);
		}

		#endregion
	}
}
