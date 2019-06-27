<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
        xmlns:msxsl="urn:schemas-microsoft-com:xslt"
        xmlns:cs="urn:cs"
		xmlns:dc="http://purl.org/dc/elements/1.1/"
 >

<xsl:output method="html" indent="yes"/>
<msxsl:script language="c#" implements-prefix="cs">
	<![CDATA[

  // This XSLT is for the Central AZ Wordpress Blog 
  // http://www.centralaz.com/blog/feed
  // and transforms the last three posts of XML into a nicely
  // formatted HTML block for viewing on the website.
  // Since we don't get the author's email via the RSS feed,
  // I'm keeping track of the author's name and the hash to
  // their gravatar in this file. :( Yeah I know, that kinda
  // stinks but it's not worth the time to write some custom
  // Arena wordpress module to do this another way.
  
  // http://0.gravatar.com/avatar/e2f96e60845862c26ce139dcc2da198d?s=48&d=&r=PG
  // https://secure.gravatar.com/avatar/e2f96e60845862c26ce139dcc2da198d?s=48&d=&r=PG
	public static System.Collections.Generic.Dictionary<string, string> gravatars = new System.Collections.Generic.Dictionary<string, string>();

	public static void InitGravatars()
	{
		gravatars.Add("Default", "49d57c559bdf07ce8c20cb5e8cfb1089");
		gravatars.Add("Cal Jernigan", "beaf231577da75debf683562888f5c2f");
		gravatars.Add("Worship Team", "7246beb6d312f1927a1dbf667dc289bb");
		gravatars.Add("Campus Pastors", "568c7ca26439d057f509d536ec392137");
		gravatars.Add("Jeremy Jernigan", "e2f96e60845862c26ce139dcc2da198d");
		gravatars.Add("Student Team", "65795cbef6807ffff25c83a950694ea9");
		gravatars.Add("Paul Carpenter", "4f0f1cfa224234066956b299f36141d6");
	}
	
	public static string GetGravatar(string authorName)
	{
		string hash = ( gravatars.ContainsKey(authorName) ) ? gravatars[ authorName ] : gravatars[ "Default" ];
		string url = String.Format( "http://0.gravatar.com/avatar/{0}?s=48&d=&r=PG", hash );
		return url;
	}
	  
	public static string TruncateText( string text, int maxLength, string append )
	{
		text = Regex.Replace(text, @"&nbsp;", " ", RegexOptions.IgnoreCase);
		text = Regex.Replace(text, @"(\<(/?[^\>]+)\>)", "", RegexOptions.IgnoreCase);
		if ( text.Length > maxLength )
		{
			text = string.Format("{0}{1}", text.Substring( 0, maxLength - 1), append );
		}
		return text;
	}

	public static string RelativeDate( string pubDate )
	{  	
    	try
    	{
				DateTime d = DateTime.Parse( pubDate );
				DateTime now = DateTime.Now;
				TimeSpan timeSince = now - d;
				
				double inSeconds = timeSince.TotalSeconds;
				double inMinutes = timeSince.TotalMinutes;
				double inHours = timeSince.TotalHours;
				double inDays = timeSince.TotalDays;
				double inMonths = inDays / 30;
				double inYears = inDays / 365;
				
				if(Math.Round(inSeconds) == 1){
					return "1 second ago";
				}
				else if(inMinutes < 1.0){
					return Math.Floor(inSeconds) + " seconds ago";
				}
				else if(Math.Floor(inMinutes) == 1){
					return "1 minute ago";
				}
				else if(inHours < 1.0){
					return Math.Floor(inMinutes) + " minutes ago";
				}
				else if(Math.Floor(inHours) == 1){
					return "about an hour ago";
				}
				else if(inDays < 1.0){
					return Math.Floor(inHours) + " hours ago";
				}
				else if(Math.Floor(inDays) == 1){
					return "1 day ago";
				}
				else if(inMonths < 3 ){
					return Math.Floor(inDays) + " days ago";
				}
				else if(inMonths <= 12 ){
					return Math.Floor(inMonths) + " months ago ";
				}
				else if(Math.Floor(inYears) <= 1){
					return "1 year ago";
				}
				else
				{
					return Math.Floor(inYears) + " years ago";
				}
			}
			catch ( Exception )
			{
			}
			return "";
    }
 ]]>
 </msxsl:script>

	<xsl:template match="/">
		<xsl:value-of select="cs:InitGravatars()"/>
		<ul class="recentblog">
		<xsl:for-each select="/rss/channel/item">
		<xsl:if test="position() &lt; 4">
			<li>
			<img border="0" alt="author">
				<xsl:attribute name="src">
				<xsl:value-of select="cs:GetGravatar(dc:creator)"/>
				</xsl:attribute>
			</img>
			<a target="_blank">
				<xsl:attribute name="href">
				<xsl:value-of select="link"/>
				</xsl:attribute>
				<xsl:value-of select="title" />
			</a>
			<br/>
			<span>
				<xsl:value-of select="cs:RelativeDate(pubDate)" />
			</span>
			</li>
		</xsl:if>
		</xsl:for-each>
		</ul>
	</xsl:template>
</xsl:stylesheet>
