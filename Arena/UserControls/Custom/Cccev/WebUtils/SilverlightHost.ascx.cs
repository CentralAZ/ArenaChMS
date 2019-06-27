/**********************************************************************
* Description: Hosts a Silverlight control in Arena
* Created By: DallonF
* Date Created: 6/9/08
*
* $Workfile: SilverlightHost.ascx.cs $
* $Revision: 2 $
* $Header: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/SilverlightHost.ascx.cs   2   2009-06-21 14:27:12-07:00   nicka $
*
* $Log: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/SilverlightHost.ascx.cs $
*  
*  Revision: 2   Date: 2009-06-21 21:27:12Z   User: nicka 
*  updated for Silverlight2 
*  
*  Revision: 1   Date: 2008-06-12 02:40:21Z   User: DallonF 
*  Version 1. Hosts a Silverlight control in Arena. 
**********************************************************************/
using System;
using Arena.Portal;

namespace ArenaWeb.UserControls.Custom.Cccev.WebUtils
{
    public partial class SilverlightHost : PortalControl
    {
		[FileSetting( "XAP file", "The location of the XAP file that contains the Silverlight application you wish to display. Example: ClientBin/OrgID/Arena.Custom.Cccev.Silverlight.ExampleApp.xap", true )]
		public string XapFileSetting { get { return Setting( "XapFile", "", true ); } }

		[NumericSetting("Height", "The height, in pixels, of the application. Default: 300", false)]
		public string HeightSetting { get { return Setting("Height", "300", false); } }

		[NumericSetting("Width", "The width, in pixels, of the application. Default: 400", false)]
		public string WidthSetting { get { return Setting("Width", "400", false); } }

		[BooleanSetting("Show Errors", "Show runtime errors above plugin. Useful for debugging. ", false, false)]
		public string ShowErrorsSetting { get { return Setting("ShowErrors", "False", false); } }

		[TextSetting("Init Parameters", "Parameters to pass to the Silverlight application, comma delimited. Example:param1=value1,param2=value2", false)]
		public string InitParamsSetting { get { return Setting("InitParams", "", false); } }

    } 
}
