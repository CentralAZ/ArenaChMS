/**********************************************************************
* Description:	TBD
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created:	TBD
*
* $Workfile: FlickrPhotoGallery.ascx.cs $
* $Revision: 4 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/FlickrPhotoGallery.ascx.cs   4   2010-02-17 09:52:47-07:00   JasonO $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/WebUtils/FlickrPhotoGallery.ascx.cs $
*  
*  Revision: 4   Date: 2010-02-17 16:52:47Z   User: JasonO 
*  Fixing jquery include issues. 
*  
*  Revision: 3   Date: 2010-01-27 22:49:28Z   User: JasonO 
*  Cleaning up. 
*  
*  Revision: 2   Date: 2009-04-15 20:42:07Z   User: nicka 
*  added retrieval of media info from Flickr 
*  
*  Revision: 1   Date: 2009-03-17 23:40:03Z   User: JasonO 
**********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Arena.Portal;
using Linq.Flickr;

/// NOTE: This module uses the LINQ to Flickr API found here: http://linqflickr.codeplex.com/
/// Additional API documentation can be found here: http://weblogs.asp.net/mehfuzh/archive/2007/10/28/new-linq-provider-for-flickr.aspx

namespace ArenaWeb.UserControls.Custom.Cccev.WebUtils
{
    public partial class FlickrPhotoGallery : PortalControl
    {
        [TextSetting("Flickr Photo Stream", "Username of the Flickr Photo Stream to be displayed.", false)]
        public string FlickrPhotoStreamSetting { get { return Setting("FlickrPhotoStream", "", false); } }

        [NumericSetting("Photo Count", "Number of Photos to be fetched from Flickr Photostream (defaults to 45 images).", false)]
        public string PhotoCountSetting { get { return Setting("PhotoCount", "45", false); } }

        List<Photo> photos;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                smpScripts.Scripts.Add(new ScriptReference(string.Format("~/{0}", BasePage.JQUERY_INCLUDE)));
                smpScripts.Scripts.Add(new ScriptReference("~/Templates/Cccev/liger/js/jquery.galleryScroll.1.4.5.pack.js"));
                smpScripts.Scripts.Add(new ScriptReference("~/Templates/Cccev/liger/js/main.js"));
            }

            if (FlickrPhotoStreamSetting.Trim() != string.Empty)
            {
                photos = GetPhotos();
                BuildAlbum();
            }
        }

        private List<Photo> GetPhotos()
        {
            FlickrContext fc = new FlickrContext();

            var photoStream = (from p in fc.Photos
							   where p.User == FlickrPhotoStreamSetting.Trim() && p.ViewMode == ViewMode.Public && p.PhotoSize == PhotoSize.Square && p.Extras == ( ExtrasOption.Media )
                               select p).Take(int.Parse(PhotoCountSetting));

            return photoStream.ToList();
        }

        private void BuildAlbum()
        {
            if (photos.Count > 0)
            {
                StringBuilder html = new StringBuilder();

                for (int i = 0; i < photos.Count; i += 9)
                {
                    if (i + 9 <= photos.Count)
                    {
                        html.Append(BuildScrollerItem(photos.GetRange(i, 9)));
                    }
                    else
                    {
                        int difference = (i + 9) - photos.Count;
                        List<Photo> photoList = photos.GetRange(i, 9 - difference);
                        html.Append(BuildScrollerItem(photoList));
                    }
                }

                phPhotoAlbum.Controls.Add(new LiteralControl(html.ToString()));
            }
        }

        private static string BuildScrollerItem(IEnumerable<Photo> list)
        {
            StringBuilder html = new StringBuilder();
            html.Append("\t\t\t\t\t<div class=\"scroller-item\">\n");
            html.Append("\t\t\t\t\t\t<div class=\"album-holder\">\n");
            html.Append("\t\t\t\t\t\t\t<ul>\n");

            foreach (Photo photo in list)
            {
                html.Append("\t\t\t\t\t\t\t\t<li>\n");
                html.AppendFormat("\t\t\t\t\t\t\t\t\t<a href=\"{0}\" target=\"_blank\">\n", photo.WebUrl);
                html.AppendFormat("\t\t\t\t\t\t\t\t\t\t<img src=\"{0}\" alt=\"{1}\" />\n", photo.Url, photo.Description);
                html.Append("\t\t\t\t\t\t\t\t\t</a>\n");
                html.Append("\t\t\t\t\t\t\t\t</li>\n");
            }

            html.Append("\t\t\t\t\t\t\t</ul>\n");
            html.Append("\t\t\t\t\t\t</div>\n");
            html.Append("\t\t\t\t\t</div>\n");
            return html.ToString();
        }
    }
}