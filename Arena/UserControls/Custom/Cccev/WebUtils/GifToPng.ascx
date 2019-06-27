<%@ Control Language="C#" Inherits="Arena.Portal.PortalControl" %>
<%@ Import Namespace="Arena.Portal" %>

<script type="text/C#" runat="server">
        [TextSetting("Files To Replace", "List of file names to replace extensions (separate by comma ',').", true)]
        public string FilesToReplaceSetting { get { return Setting("FilesToReplace", "", true); } }
</script>

<asp:ScriptManagerProxy ID="smpScripts" runat="server" />

<script type="text/javascript">
    var array;
    var prmGifs = Sys.WebForms.PageRequestManager.getInstance();

    $(document).ready(function()
    {
        initArray();
        replaceGifs();
    });

    prmGifs.add_endRequest(function()
    {
        initArray();
        replaceGifs();
    });

    function initArray()
    {
        var names = '<%= FilesToReplaceSetting %>';
        array = names.split(',');
    }
    
    function replaceGifs()
    {
        for (var i = 0; i < array.length; i++)
        {
            var oldName = array[i];
            var newName = oldName.replace('.gif', '.png');

            $("img[src$='" + oldName + "'], input[src$='" + oldName + "']").each(function()
            {
                $(this).attr('src', $(this).attr('src').replace(oldName, newName));
            });
        }
    }
</script>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            smpScripts.Scripts.Add(new ScriptReference(string.Format("~/{0}", BasePage.JQUERY_INCLUDE)));
        }
    }
</script>