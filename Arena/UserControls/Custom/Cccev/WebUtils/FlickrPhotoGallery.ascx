<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FlickrPhotoGallery.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.WebUtils.FlickrPhotoGallery" %>

<asp:ScriptManagerProxy ID="smpScripts" runat="server" />
<div>
    <h2>Photo Album</h2>
    <div class="holder">
        <div class="album-wrapper">
            <div class="album">
                <div class="scroller">
                    <asp:PlaceHolder ID="phPhotoAlbum" runat="server" />
                </div>
            </div>
        </div>
        
        <div class="album-btm">
            <a class="right" href="#">right</a>
            <a class="left" href="#">left</a>
        </div>
    </div>
</div>