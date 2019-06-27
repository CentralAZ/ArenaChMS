<%@ Control Language="C#" ClassName="SimpleVideoPlayer" Inherits="Arena.Portal.PortalControl" %>
<%@ Import Namespace="Arena.Custom.Cccev.DataUtils" %>
<%@ Import Namespace="Arena.Feed" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Arena.Portal" %>

<script runat="server">
    
    [NumericSetting("Height", "How tall do you want the player to be? (Defaults to 360px).", false)]
    public string HeightSetting { get { return Setting("Height", "360", false); } }

    [NumericSetting("Width", "How wide do you want the player to be? (Defaults to 640px).", false)]
    public string WidthSetting { get { return Setting("Width", "640", false); } }

    [TextSetting("Image Path", "Relative path to image to be displayed over HTML5 player or if Flash is not supported.", true)]
    public string ImagePathSetting { get { return Setting("ImagePath", "", true); } }

    [PageSetting("Podcast Channel Topic Page", "Page listing topics of a channel feed.", true)]
    public string TopicPageSetting { get { return Setting("TopicPage", "", true); } }
    
    public void Page_Load(object sender, EventArgs e)
    {
        BasePage.AddJavascriptInclude(Page, BasePage.JQUERY_INCLUDE);
    }
</script>

<asp:ScriptManagerProxy ID="smpScripts" runat="server" />

<script type="text/javascript">
    $(function ()
    {
        VideoJS.setup();
    });
</script>
<%
    Item item = null;
    int id;
    string itemID = Request.QueryString["item"] ?? "-1";
    
    if (int.TryParse(itemID, out id))
    {
        item = new Item(id);
    }
    
    if (item != null && item.ItemId != Constants.NULL_INT && item.PublishDate < DateTime.Now && item.Active)
    {
        var video = item.ItemFormats.FirstOrDefault(f => f.MimeType.ToLower() == "video/mp4" && f.PublicFormat);
        var audio = item.ItemFormats.FirstOrDefault(f => f.MimeType.ToLower() == "audio/mpeg" && f.PublicFormat);
        var embed = item.ItemFormats.FirstOrDefault(f => f.MimeType.ToLower() == "text/html" && f.PublicFormat);
        
        if (embed != null)
        { %>
        <h1><%= item.Title %></h1>
        <div class="video-player">
            <iframe src="<%= embed.EnclosureUrl %>" height="<%= HeightSetting %>" width="<%= WidthSetting %>" border="0"></iframe>
        </div>
        <p style="padding-top: 10px"><a href="default.aspx?page=<%= TopicPageSetting %>&channel=<%= item.ChannelId %>" title="Back to Topic List">Back To Topic List</a></p>
     <% }
        else if (video != null)
        {
            smpScripts.Scripts.Add(new ScriptReference("~/UserControls/Custom/Cccev/Web2/js/video.js"));
            BasePage.AddCssLink(Page, "UserControls/Custom/Cccev/Web2/css/video-js.css");
    %>
    <h1><%= item.Title %></h1>
    <div class="video-js-box video-player">
        <video class="video-js" width="<%= WidthSetting %>" height="<%= HeightSetting %>" poster="<%= ImagePathSetting %>" controls preload>
            <source src="<%= video.EnclosureUrl %>" type='video/mp4; codecs="avc1.42E01E, mp4a.40.2"'>

            <object class="vjs-flash-fallback" width="<%= WidthSetting %>" height="<%= HeightSetting %>" type="application/x-shockwave-flash" data="usercontrols/custom/cccev/web2/misc/flowplayer/flowplayer-3.2.5.swf">
            <param name="movie" value="usercontrols/custom/cccev/web2/misc/flowplayer/flowplayer-3.2.5.swf" />
            <param name="allowfullscreen" value="true" />
            <param name="flashvars" value='config={"clip":{"url":"<%= video.EnclosureUrl %>","autoPlay":false,"autoBuffering":true}}' />
            <param name="wmode" value="transparent" />
            <img src="<%= ImagePathSetting %>" width="<%= WidthSetting %>" height="<%= HeightSetting %>" alt="Poster Image"
                title="No video playback capabilities." />
            </object>
        </video>
    </div>
    <p style="padding-top: 10px"><a href="default.aspx?page=<%= TopicPageSetting %>&channel=<%= item.ChannelId %>" title="Back to Topic List">Back To Topic List</a></p>
     <% }
        else
        {
            Response.Redirect(audio.EnclosureUrl);
        }
    }
%>