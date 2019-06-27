using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Arena.Core;
using Arena.Custom.Cccev.FrameworkUtils.FrameworkConstants;
using Arena.Portal;

namespace ArenaWeb.UserControls.Custom.Cccev.Web
{
    public partial class CampusPicker : PortalControl
    {
        [CustomListSetting("* Instructions", "Any text entered into the details box will be rendered out to the DOM. This space can be used to execute custom JavaScripts.", 
            false, "", new string[]{}, new string[]{})]
        public string InstructionsSetting { get { return Setting("Instructions", "", false); } }

        [TextSetting("Custom Heading", "Text to appear as a heading for the campus picker.", false)]
        public string HeadingSetting { get { return Setting("Heading", "", false); } }

        [BooleanSetting("Wrap radio buttons in unordered list?", "Will wrap each radio button in the set in a &lt;ul&gt; structure.", false, true)]
        public string WrapInListSetting { get { return Setting("WrapInList", "true", false); } }

        [BooleanSetting("Include jQueryUI js and css files?", "Will inject the jQueryUI JavaScript and CSS files onto the page.", false, false)]
        public string IncludeJqueryUiSetting { get { return Setting("IncludeJqueryUi", "false", false); } }

        protected int currentCampusID;
        protected IEnumerable<Lookup> campuses;
        protected bool shouldWrapInList;
        protected bool injectJqueryUi;

        protected void Page_Load(object sender, EventArgs e)
        {
            currentCampusID = -1;
            shouldWrapInList = bool.Parse(WrapInListSetting);
            injectJqueryUi = bool.Parse(IncludeJqueryUiSetting);

            if (injectJqueryUi)
            {
                BasePage.AddCssLink(Page, "~/Templates/cccev/007/css/jquery-ui-1.8.16.custom.css");
                smpScripts.Scripts.Add(new ScriptReference("~/include/scripts/Custom/Cccev/lib/jquery-ui-1.8.13.min.js"));
            }

            if (CurrentPerson != null && CurrentPerson.PersonID != -1 && CurrentPerson.Campus != null)
            {
                currentCampusID = CurrentPerson.Campus.CampusId;
            }

            campuses = from l in new LookupType(SystemGuids.CAMPUS_LOOKUP_TYPE).Values
                       select l;

            if (!IsPostBack)
            {
                InjectCustomScripts();
            }
        }

        private void InjectCustomScripts()
        {
            if (!string.IsNullOrEmpty(CurrentModule.Details))
            {
                lCustomScripts.Text = CurrentModule.Details;
                lCustomScripts.Visible = true;
            }
        }
    }
}
