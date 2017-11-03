/**********************************************************************
* Description:  Print Label Proxy Provider.  Communicates back to 
*				another server (via WebServices) to handle printing.
*				
* Created By:   Nick Airdo @ Central Christian Church, Arizona (Cccev)
* Date Created: 6/25/2012
*
* $Workfile: CccevPrintLabelProxy.cs $
* $Revision: 1 $
* $Header: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/CccevPrintLabelProxy.cs   1   2012-06-27 16:01:08-07:00   nicka $
*
* $Log: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/CccevPrintLabelProxy.cs $
*  
*  Revision: 1   Date: 2012-06-27 23:01:08Z   User: nicka 
*  initial working version that uses org setting to determine location of 
*  WebService to send print data for printing. 
**********************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

using Arena.Computer;
using Arena.Core;

namespace Arena.Custom.Cccev.CheckIn.Entity
{
	internal class CccevPrintLabelProxy : IPrintLabel
	{
		public void Print( FamilyMember person, IEnumerable<Occurrence> occurrences, OccurrenceAttendance attendance, ComputerSystem kiosk )
		{
			// Send the item to the real server...
			Organization.Organization organization = ArenaContext.Current.Organization;
			var proxyURI = organization.Settings["Cccev.PrintLabelURI"];
			if ( string.IsNullOrEmpty( proxyURI ) )
			{
				throw new ApplicationException( "Unable to request print label services.  Required Org Setting (Cccev.PrintLabelURI) must contain the URL to the print service." );
			}

			string data = string.Format( "personID={0}&occurrenceIDs={1}&attendanceID={2}&kioskID={3}",
				person.PersonID, OccurrenceIDsToCSV( occurrences ), attendance.OccurrenceAttendanceID, kiosk.SystemId );
			proxyURI = string.Format( "{0}?{1}", proxyURI, data );

			HttpWebRequest webRequest = WebRequest.Create( proxyURI ) as HttpWebRequest;

			using ( HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse )
			{
				if ( response == null || response.StatusCode != HttpStatusCode.OK )
				{
					throw new ApplicationException( "There was a problem with the Print Label service." );
				}

				var stream = response.GetResponseStream();
				var xr = XmlReader.Create( stream );
				var xdoc = XDocument.Load( xr );
				bool isSuccess = bool.Parse( xdoc.Root.Value );
				if ( ! isSuccess )
				{
					throw new ApplicationException( "There was a problem with the Print Label service." );
				}
			}
		}

		private static string OccurrenceIDsToCSV( IEnumerable<Occurrence> occurrences )
		{
			return string.Join( ",", occurrences.Select( x => x.OccurrenceID.ToString() ).ToArray() );
		}

		private static string SerializeToString( object obj )
		{
			XmlSerializer serializer = new XmlSerializer( obj.GetType() );

			using ( StringWriter writer = new StringWriter() )
			{
				serializer.Serialize( writer, obj );

				return writer.ToString();
			}
		}

	}
}
