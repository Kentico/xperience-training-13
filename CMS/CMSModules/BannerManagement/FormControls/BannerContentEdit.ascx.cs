using System;

using CMS.BannerManagement;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSModules_BannerManagement_FormControls_BannerContentEdit : FormEngineUserControl
{
    #region "Private fields"

    /// <summary>
    /// Value obtained by setting Value property.
    /// </summary>
    private string providedValue;

    #endregion


    #region "Properties"

    private BannerTypeEnum? BannerTypeVS
    {
        set
        {
            ViewState["BannerType"] = value;
        }
        get
        {
            return ViewState["BannerType"] as BannerTypeEnum?;
        }
    }


    /// <summary>
    /// Type of banner. Controls will be displayed based on this field.
    /// </summary>
    public BannerTypeEnum BannerType
    {
        get
        {
            if (BannerTypeVS.HasValue)
            {
                return BannerTypeVS.Value;
            }

            return BannerTypeEnum.Plain;
        }
        set
        {
            if (BannerTypeVS.HasValue)
            {
                BannerTypeEnum previousBannerType = BannerTypeVS.Value;

                // Change from plain to HTML - preserve value
                if ((previousBannerType == BannerTypeEnum.Plain) && (value == BannerTypeEnum.HTML))
                {
                    providedValue = macroEditorPlain.Text;
                }
                // Change from HTML to plain - preserve value
                else if ((previousBannerType == BannerTypeEnum.HTML) && (value == BannerTypeEnum.Plain))
                {
                    providedValue = htmlBanner.ResolvedValue;
                }
            }

            BannerTypeVS = value;
        }
    }


    /// <summary>
    /// Value. Its format depends on banner type.
    /// </summary>
    public override object Value
    {
        get
        {
            // Get value based on BannerType
            switch (BannerType)
            {
                case BannerTypeEnum.HTML:
                    return htmlBanner.ResolvedValue;

                case BannerTypeEnum.Plain:
                    return macroEditorPlain.Text;

                case BannerTypeEnum.Image:
                    if (txtImage.Text == "")
                    {
                        return "";
                    }

                    BannerImageAttributes bannerImage = new BannerImageAttributes
                    {
                        Src = URLHelper.UnResolveUrl(txtImage.Text, SystemContext.ApplicationPath),
                        Title = txtImgTitle.Text,
                        Alt = txtImgAlt.Text,
                        Class = txtImgClass.Text,
                        Style = txtImgStyle.Text,
                    };

                    return BannerManagementHelper.SerializeBannerImageAttributesToString(bannerImage);
            }

            return "";
        }
        set
        {
            providedValue = value as string;
        }
    }


    public int BannerID
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        htmlBanner.RemoveButtons.Add("QuicklyInsertImage");
        htmlBanner.RemoveButtons.Add("Maximize");

        macroEditorPlain.Language = LanguageEnum.HTMLMixed;

        if (DependsOnAnotherField)
        {
            BannerType = (BannerTypeEnum)ValidationHelper.GetInteger(Form.GetFieldValue("BannerType"), (int)BannerTypeEnum.Image);
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        SetVisibility(BannerType);

        // Display appropriate controls and fill them with values.
        switch (BannerType)
        {
            case BannerTypeEnum.HTML:
                {
                    if (!string.IsNullOrEmpty(providedValue))
                    {
                        htmlBanner.ResolvedValue = providedValue;
                    }
                }
                break;
            case BannerTypeEnum.Plain:
                {
                    if (!string.IsNullOrEmpty(providedValue))
                    {
                        macroEditorPlain.Text = providedValue;
                    }
                }
                break;
            case BannerTypeEnum.Image:
                {
                    if (!string.IsNullOrEmpty(providedValue))
                    {
                        BannerImageAttributes bannerImage = BannerManagementHelper.DeserializeBannerImageAttributes(providedValue);

                        txtImage.Text = bannerImage.Src;
                        txtImgTitle.Text = bannerImage.Title;
                        txtImgAlt.Text = bannerImage.Alt;
                        txtImgClass.Text = bannerImage.Class;
                        txtImgStyle.Text = bannerImage.Style;
                    }
                }
                break;
        }
    }


    private void SetVisibility(BannerTypeEnum bannerType)
    {
        plcImage.Visible = bannerType == BannerTypeEnum.Image;
        plcPlain.Visible = bannerType == BannerTypeEnum.Plain;
        plcHtml.Visible = bannerType == BannerTypeEnum.HTML;
    }

    #endregion
}
