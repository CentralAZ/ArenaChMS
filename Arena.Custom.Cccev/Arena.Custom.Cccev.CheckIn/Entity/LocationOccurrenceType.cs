using System.Configuration;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using Arena.DataLib;

namespace Arena.Custom.Cccev.CheckIn.Entity
{
    [Table(Name = "orgn_location_occurrence_type")]
    public class LocationOccurrenceType
    {
        private int locationID;
        private int occurrenceTypeID;
        private int locationOrder;

        [Column(Name = "location_id")]
        public int LocationID
        {
            get { return locationID; }
            set { locationID = value; }
        }

        [Column(Name = "occurrence_type_id")]
        public int OccurrenceTypeID
        {
            get { return occurrenceTypeID; }
            set { occurrenceTypeID = value; }
        }

        [Column(Name = "location_order")]
        public int LocationOrder
        {
            get { return locationOrder; }
            set { locationOrder = value; }
        }

        public LocationOccurrenceType()
        {
            locationID = DataUtils.Constants.NULL_INT;
            occurrenceTypeID = DataUtils.Constants.NULL_INT;
            locationOrder = DataUtils.Constants.NULL_INT;
        }

        public static LocationOccurrenceType LoadByLocationAndOccurrenceType(int locationID, int occurrenceTypeID)
        {
            using (var context = new LocationOccurrenceTypeContext(new SqlDbConnection().
                GetArenaConnectionString(ConfigurationManager.ConnectionStrings["Arena"].ToString())))
            {
                return (from lot in context.GetTable<LocationOccurrenceType>()
                        where lot.LocationID == locationID && lot.OccurrenceTypeID == occurrenceTypeID
                        select new LocationOccurrenceType
                                   {
                                       LocationID = lot.LocationID,
                                       OccurrenceTypeID = lot.OccurrenceTypeID,
                                       LocationOrder = lot.LocationOrder
                                   }).Single();
            }
        }
    }

    public class LocationOccurrenceTypeContext : DataContext
    {
        public LocationOccurrenceTypeContext(string connection) : base(connection)
        {
        }
    }
}
