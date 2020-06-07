using System;
using System.Web.UI;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSAdminControls_UI_UserPicture : CMSUserControl
{
    private int mHeight = 0;
    private int mWidth = 0;


    #region "Variables"

    public CMSAdminControls_UI_UserPicture()
    {
        RenderOuterDiv = false;
        AvatarID = 0;
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Keep aspect ratio.
    /// </summary>
    public bool KeepAspectRatio
    {
        get;
        set;
    }


    /// <summary>
    /// Max picture width.
    /// </summary>
    public int Width
    {
        get
        {
            return mWidth > 0 ? mWidth : SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarWidth"); ;
        }
        set
        {
            mWidth = value;
        }
    }


    /// <summary>
    /// Max picture height.
    /// </summary>
    public int Height
    {
        get
        {
            return mHeight > 0 ? mHeight : SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarHeight"); ;
        }
        set
        {
            mHeight = value;
        }
    }


    /// <summary>
    /// Enable/disable display picture
    /// </summary>
    public bool DisplayPicture { get; set; } = true;


    /// <summary>
    /// User ID.
    /// </summary>
    public int UserID { get; set; } = 0;


    /// <summary>
    /// Gets or sets avatar id.
    /// </summary>
    public int AvatarID
    {
        get;
        set;
    }


    /// <summary>
    /// Div tag is rendered around picture if true (default value = 'false').
    /// </summary>
    public bool RenderOuterDiv
    {
        get;
        set;
    }


    /// <summary>
    /// CSS class of outer div (default value = 'UserPicture').
    /// </summary>
    public string OuterDivCSSClass { get; set; } = "UserPicture";

    #endregion


    /// <summary>
    /// Sets image  url, width and height.
    /// </summary>
    protected void SetImage()
    {
        Visible = false;

        // Only if display picture is allowed
        if (DisplayPicture)
        {
            string imageUrl = ResolveUrl("~/getavatar/{0}/avatar");

            // Is user id set? => Get user info
            if (UserID > 0)
            {
                // Get user info
                UserInfo ui = UserInfo.Provider.Get(UserID);
                if (ui != null)
                {
                    AvatarID = ui.UserAvatarID;
                }
            }


            if (AvatarID > 0)
            {
                AvatarInfo ai = AvatarInfoProvider.GetAvatarInfoWithoutBinary(AvatarID);
                if (ai != null)
                {
                    imageUrl = String.Format(imageUrl, ai.AvatarGUID);
                    imageUrl = URLHelper.AppendQuery(imageUrl, "lastModified=" + SecurityHelper.GetSHA2Hash(ai.AvatarLastModified.ToString()));
                    Visible = true;
                }
            }


            // If item was found 
            if (Visible)
            {
                if (KeepAspectRatio)
                {
                    imageUrl += "&maxsidesize=" + (Width > Height ? Width : Height);
                }
                else
                {
                    imageUrl += "&width=" + Width + "&height=" + Height;
                }

                imageUrl = HTMLHelper.EncodeForHtmlAttribute(imageUrl);
                ltlImage.Text = "<img alt=\"" + GetString("general.avatar") + "\" src=\"" + imageUrl + "\" />";

                // Render outer div with specific CSS class
                if (RenderOuterDiv)
                {
                    ltlImage.Text = "<div class=\"" + OuterDivCSSClass + "\">" + ltlImage.Text + "</div>";
                }
            }
        }
    }


    /// <summary>
    /// Render.
    /// </summary>
    protected override void Render(HtmlTextWriter writer)
    {
        if (DisplayPicture)
        {
            SetImage();
        }
        else
        {
            Visible = false;
        }

        base.Render(writer);
    }

}