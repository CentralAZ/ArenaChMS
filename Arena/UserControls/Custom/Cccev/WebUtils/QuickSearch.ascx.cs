/**********************************************************************
* Description:	TBD
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created:	TBD
*
* $Workfile: QuickSearch.ascx.cs $
* $Revision: 3 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/QuickSearch.ascx.cs   3   2010-01-27 15:49:28-07:00   JasonO $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/QuickSearch.ascx.cs $
*  
*  Revision: 3   Date: 2010-01-27 22:49:28Z   User: JasonO 
*  Cleaning up. 
*  
*  Revision: 2   Date: 2009-04-02 19:32:27Z   User: JasonO 
**********************************************************************/

using System;
using System.Web.UI;
using Arena.Portal;

namespace ArenaWeb.UserControls.Custom.Cccev.WebUtils
{
    public partial class QuickSearch : PortalControl
    {
        [PageSetting("Search Page", "Page ID of page that displays search results.", true)]
        public string SearchPageSetting { get { return Setting("SearchPage", "", true); } }

        protected void Page_Init(object sender, EventArgs e)
        {
            ibQuickSearch.Click += ibQuickSearch_Click;

            if (!Page.IsPostBack)
            {
                smpScripts.Scripts.Add(new ScriptReference("~/Templates/Cccev/liger/js/inputs.js"));
            }
        }

        private void ibQuickSearch_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect(string.Format("Default.aspx?page={0}&q={1}", SearchPageSetting, tbQuickSearch.Text));
        }
    }
}