<%@ Control Language="C#" ClassName="InjectJavaScriptInclude" Inherits="Arena.Portal.PortalControl" %>
<%@ Import Namespace="Arena.Portal"%>

<asp:ScriptManagerProxy ID="smpScripts" runat="server" />
<asp:PlaceHolder ID="phScript" runat="server" />

<script runat="server">
    [TextSetting("Client Script Path", "Relative path to JavaScript file to be injected (ie: folder/script.js).", false)]
    public string ScriptPathSetting { get { return Setting("ScriptPath", "", false); } }

    [CustomListSetting("DOM Location", "Location in DOM to inject the JavaScript include.", false, "Head", 
        new[] { "Head", "Body - Top", "Body - Current Module Location" }, new[] { "head", "body", "current" }, ListSelectionMode.Single)]
    public string DOMLocationSetting { get { return Setting("DOMLocation", "Head", false); } }
    
    private void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(ScriptPathSetting))
        {
            switch (DOMLocationSetting)
            {
                case "head":
                    BasePage.AddJavascriptInclude(Page, ScriptPathSetting);
                    break;
                case "body":
                    smpScripts.Scripts.Add(new ScriptReference(string.Format("~/{0}", ScriptPathSetting)));
                    break;
                case "current":
                    phScript.Controls.Add(new LiteralControl(string.Format("<script src=\"{0}\" type=\"text/javascript\"></{1}", 
                        ScriptPathSetting, "script>")));
                    break;
                default:
                    break;
            }
        }
    }
</script>
