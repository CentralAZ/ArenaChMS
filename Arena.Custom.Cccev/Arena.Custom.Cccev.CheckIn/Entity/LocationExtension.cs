/**********************************************************************
* Description:  Extensions on the Arena.Organization.Location object
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created: 9/22/2009
*
* $Workfile: LocationExtension.cs $
* $Revision: 1 $
* $Header: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/LocationExtension.cs   1   2009-09-23 15:38:01-07:00   JasonO $
*
* $Log: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/LocationExtension.cs $
*  
*  Revision: 1   Date: 2009-09-23 22:38:01Z   User: JasonO 
**********************************************************************/

using System;
using Arena.Custom.Cccev.CheckIn.DataLayer;
using Arena.Organization;

namespace Arena.Custom.Cccev.CheckIn.Entity
{
    public static class LocationExtension
    {
        public static int GetHeadCountByDate(this Location location, DateTime startDate)
        {
            return new XLocationData().GetLocationHeadCountByDate(location.LocationId, startDate);
        }
    }
}
