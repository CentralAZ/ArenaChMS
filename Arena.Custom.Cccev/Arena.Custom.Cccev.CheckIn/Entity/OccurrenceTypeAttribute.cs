/**********************************************************************
* Description:  Holds the extended, custom properties for an Occurrence
*               Type (a.k.a. Attendance Type).
* Created By:   Nick Airdo
* Date Created: 11/12/2008	
*
* $Workfile: OccurrenceTypeAttribute.cs $
* $Revision: 4 $ 
* $Header: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/OccurrenceTypeAttribute.cs   4   2009-09-23 15:38:02-07:00   JasonO $
* 
* $Log: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/OccurrenceTypeAttribute.cs $
*  
*  Revision: 4   Date: 2009-09-23 22:38:02Z   User: JasonO 
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
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using Arena.Core;
using Arena.Utility;
using Arena.Custom.Cccev.CheckIn.DataLayer;
using Arena.DataLayer.Skeleton;
using Arena.DataLayer.Core;

namespace Arena.Custom.Cccev.CheckIn.Entity
{
	/// <summary>
	/// Summary description for OccurrenceTypeAttribute.
	/// </summary>
	[Serializable]
	public class OccurrenceTypeAttribute : ArenaObjectBase
	{
		#region Private Members
		private Lookup luAbilityLevelEntityType;
		private int _occurrenceTypeAttributeId = -1; 
		private int _occurrenceTypeId = -1; 
		private bool _isSpecialNeeds = false; 
		private string _lastNameStartingLetter = string.Empty; 
		private string _lastNameEndingLetter = string.Empty; 
		private List<int> _abilityLevelLookupIDs = null;
        private bool _isRoomBalancing = false;
		private DateTime _dateCreated = DateTime.MinValue; 
		private DateTime _dateModified = DateTime.MinValue; 
		private string _createdBy = string.Empty; 
		private string _modifiedBy = string.Empty;

		#endregion

		#region Public Properties

		public static readonly Guid AbilityLevelEntityTypeGUID = new Guid( "B8903B32-3200-41E3-8C08-66C372A38D12" );

		public int OccurrenceTypeAttributeId
		{
			get{return _occurrenceTypeAttributeId;}
			set{_occurrenceTypeAttributeId = value;}
		}

		public int OccurrenceTypeId
		{
			get{return _occurrenceTypeId;}
			set{_occurrenceTypeId = value;}
		}

		public bool IsSpecialNeeds
		{
			get{return _isSpecialNeeds;}
			set{_isSpecialNeeds = value;}
		}

		public string LastNameStartingLetter
		{
			get{return _lastNameStartingLetter;}
			set{_lastNameStartingLetter = value;}
		}

		public string LastNameEndingLetter
		{
			get{return _lastNameEndingLetter;}
			set{_lastNameEndingLetter = value;}
		}

		public List<int> AbilityLevelLookupIDs
		{
			get
			{
				if ( _abilityLevelLookupIDs == null )
				{
					_abilityLevelLookupIDs = new List<int>();
					foreach ( Bone bone in new BoneCollection( _occurrenceTypeAttributeId, luAbilityLevelEntityType ) )
					{
						_abilityLevelLookupIDs.Add( bone.Topic.LookupID );
					}
				}
				return _abilityLevelLookupIDs;
			}
			set
			{
				_abilityLevelLookupIDs = value;
			}
		}

	    public bool IsRoomBalancing
	    {
            get { return _isRoomBalancing; }
            set { _isRoomBalancing = value; }
	    }

        public DateTime DateCreated
		{
			get{return _dateCreated;}
			set{_dateCreated = value;}
		}

		public DateTime DateModified
		{
			get{return _dateModified;}
			set{_dateModified = value;}
		}

		public string CreatedBy
		{
			get{return _createdBy;}
			set{_createdBy = value;}
		}

		public string ModifiedBy
		{
			get{return _modifiedBy;}
			set{_modifiedBy = value;}
		}


		#endregion

		#region Public Methods
		public void Save(string userId)
		{
			SaveOccurrenceTypeAttribute(userId);	
		}

		public static void Delete(int occurrenceTypeAttributeId)
		{
			// Delete the related Ability Levels stored in the Skeleton Bones
			DeleteAbilityLevels( occurrenceTypeAttributeId );
			new OccurrenceTypeAttributeData().DeleteOccurrenceTypeAttribute(occurrenceTypeAttributeId);
		}
		
		public void Delete()
		{
			// Delete the related Ability Levels stored in the Skeleton Bones
			DeleteAbilityLevels( _occurrenceTypeAttributeId, luAbilityLevelEntityType);

			// delete record
			OccurrenceTypeAttributeData occurrencetypeattributeData = new OccurrenceTypeAttributeData();
			occurrencetypeattributeData.DeleteOccurrenceTypeAttribute(_occurrenceTypeAttributeId);
												
			_occurrenceTypeAttributeId = -1;
		}
		
		#endregion

		#region Private Methods

		/// <summary>
		/// Delete the related Ability Levels stored in the Skeleton Bones
		/// </summary>
		/// <param name="occurrenceTypeAttributeId"></param>
		private static void DeleteAbilityLevels( int occurrenceTypeAttributeId )
		{
			Lookup entityType = new Lookup( AbilityLevelEntityTypeGUID, true );
			DeleteAbilityLevels( occurrenceTypeAttributeId, entityType );
		}

		/// <summary>
		/// Delete the related Ability Levels stored in the Skeleton Bones
		/// </summary>
		/// <param name="occurrenceTypeAttributeId"></param>
		private static void DeleteAbilityLevels( int occurrenceTypeAttributeId, Lookup entityType )
		{
			// Delete the related Ability Levels stored in the Skeleton Bones
			try
			{
				new SkeletonData().DeleteBonesByEntity( occurrenceTypeAttributeId, entityType.LookupID );
			}
			catch ( Exception ex )
			{
				try
				{
					// If SQL exception is generated while trying to delete the SkeletonData, just log it
					new ExceptionHistoryData().AddUpdate_Exception( ex, ArenaContext.Current.Organization.OrganizationID,
							ArenaContext.Current.User.Identity.Name, ArenaContext.Current.ServerUrl );
				}
				catch { }
			}
		}

		private void SaveOccurrenceTypeAttribute(string userId)
		{
			_occurrenceTypeAttributeId = new OccurrenceTypeAttributeData().SaveOccurrenceTypeAttribute(
				_occurrenceTypeAttributeId,
				_occurrenceTypeId,
				_isSpecialNeeds,
				_lastNameStartingLetter,
				_lastNameEndingLetter,
                _isRoomBalancing,
				userId);

			new SkeletonData().DeleteBonesByEntity( _occurrenceTypeAttributeId, luAbilityLevelEntityType.LookupID );
			foreach ( int lookupID in AbilityLevelLookupIDs )
			{
				Bone bone = new Bone( lookupID, luAbilityLevelEntityType, _occurrenceTypeAttributeId );
				bone.Save();
			}
		}

		private void LoadOccurrenceTypeAttribute(SqlDataReader reader)
		{
			if (!reader.IsDBNull(reader.GetOrdinal("occurrence_type_attribute_id")))
				_occurrenceTypeAttributeId = (int)reader["occurrence_type_attribute_id"];

			if (!reader.IsDBNull(reader.GetOrdinal("occurrence_type_id")))
				_occurrenceTypeId = (int)reader["occurrence_type_id"];

			if (reader.IsDBNull(reader.GetOrdinal("is_special_needs")))
				_isSpecialNeeds = false;
			else
				_isSpecialNeeds = (bool)reader["is_special_needs"];

			_lastNameStartingLetter  = reader["last_name_starting_letter"].ToString();

			_lastNameEndingLetter  = reader["last_name_ending_letter"].ToString();

            if (reader.IsDBNull(reader.GetOrdinal("is_room_balancing")))
                _isRoomBalancing = false;
            else
                _isRoomBalancing = (bool)reader["is_room_balancing"];

			if (!reader.IsDBNull(reader.GetOrdinal("date_created")))
				_dateCreated = (DateTime)reader["date_created"];

			if (!reader.IsDBNull(reader.GetOrdinal("date_modified")))
				_dateModified = (DateTime)reader["date_modified"];

			_createdBy  = reader["created_by"].ToString();

			_modifiedBy  = reader["modified_by"].ToString();

			luAbilityLevelEntityType = new Lookup( AbilityLevelEntityTypeGUID, true );
		}


		#endregion
		
		#region Constructors
		
		public OccurrenceTypeAttribute()
		{
			luAbilityLevelEntityType = new Lookup( AbilityLevelEntityTypeGUID, true );
		}

		public OccurrenceTypeAttribute(int occurrenceTypeAttributeId)
		{
			SqlDataReader reader = new OccurrenceTypeAttributeData().GetOccurrenceTypeAttributeByID(occurrenceTypeAttributeId);
			if (reader.Read())
				LoadOccurrenceTypeAttribute(reader);
			reader.Close();
		}

		public OccurrenceTypeAttribute(SqlDataReader reader)
		{
			LoadOccurrenceTypeAttribute(reader);
		}

		#endregion
	}
}





