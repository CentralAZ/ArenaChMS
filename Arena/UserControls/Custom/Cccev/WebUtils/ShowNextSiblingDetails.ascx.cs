/**********************************************************************
* Description:	Shows the contents (module details) of the next module
*				only if the person supplies the correct passcode.
*				
*				As requested by the Outreach ministry to create a simple 
*				obscurity layer for content, this module will show the
*				NEXT sibling module's module details if the configured
*				pass-code is supplied -- circumventing view security on that
*				next module (by design).  It expects the next module to be a
*				HTML type module which stores its content in the module details.
*				
*				WARNING: This module circumvents the view permissions of
*						 the next sibling module.  This is on purpose.
*				
* Created By:   Nick Airdo @ Central Christian Church of the East Valley
* Date Created:	9/10/2010
*
* $Workfile: ShowNextSiblingDetails.ascx.cs $
* $Revision: 2 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/ShowNextSiblingDetails.ascx.cs   2   2010-09-10 16:11:26-07:00   nicka $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/ShowNextSiblingDetails.ascx.cs $
*  
*  Revision: 2   Date: 2010-09-10 23:11:26Z   User: nicka 
*  bug fix: Decode html before adding to page.  Panel now uses "content" css 
*  class. 
*  
*  Revision: 1   Date: 2010-09-10 16:52:09Z   User: nicka 
*  Requested by Global Outreach; see description for full details. 
**********************************************************************/
using System;
using System.Web.UI;

using Arena.Portal;

namespace ArenaWeb.UserControls.Custom.Cccev.WebUtils
{
	public partial class ShowNextSiblingDetails : PortalControl
	{
		[TextSetting("Passcode", "A simple passcode which when provided by the user will cause the next sibling's module details to be displayed regardless of its permissions.", true)]
		public string PasscodeSetting { get { return Setting("Passcode", "", true); } }

		protected void Page_Load( object sender, EventArgs e )
		{

		}

		protected void btnVerify_Click( object sender, EventArgs e )
		{
			if ( txtPasscode.Text == PasscodeSetting )
			{
				bool isNext = false;
				ModuleInstance protectedModule = null;
				foreach ( ModuleInstance m in CurrentPortalPage.Modules)
				{
					if ( isNext )
					{
						protectedModule = m;
						break;
					}
					else if ( m.ModuleInstanceID == CurrentModule.ModuleInstanceID )
					{
						isNext = true;
					}
				}

				if ( protectedModule != null )
				{
					phContent.Controls.Clear();
					phContent.Controls.Add( new LiteralControl( Server.HtmlDecode( protectedModule.Details ) ) );
					pnlPasscode.Visible = false;
				}
			}
		}
	}
}