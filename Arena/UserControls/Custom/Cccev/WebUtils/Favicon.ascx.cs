/**********************************************************************
* Description:	Inject your Favicon into the Page Header
* Created By:	Nick Airdo
* Date Created:	4/17/2009 9:34:28 AM
*
* $Workfile: Favicon.ascx.cs $
* $Revision: 1 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/Favicon.ascx.cs   1   2009-04-17 10:21:08-07:00   nicka $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/Favicon.ascx.cs $
*  
*  Revision: 1   Date: 2009-04-17 17:21:08Z   User: nicka 
**********************************************************************/
using System.Web.UI;

using Arena.Portal;

namespace ArenaWeb.UserControls.Custom.Cccev.WebUtils
{
	public partial class Favicon : PortalControl
	{
		[TextSetting( "Favicon Relative Path", "URI of a favicon file (http://www.google.com/favicon.ico). Default favicon.ico.", true )]
		public string FaviconSetting { get { return Setting( "Favicon", "favicon.ico", true ); } }

		private void Page_Load(object sender, System.EventArgs e)
		{
			Page.Header.Controls.Add( new LiteralControl( string.Format("<link rel=\"shortcut icon\" type=\"image/vnd.microsoft.icon\" href=\"{0}\" />\n", FaviconSetting ) ) );
		}
	}
}