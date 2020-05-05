using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.SalesForce;


/// <summary>
/// Displays a mapping of CMS objects to SalesForce entities.
/// </summary>
public partial class CMSModules_ContactManagement_Controls_UI_SalesForce_Mapping : AbstractUserControl
{
    #region "Private members"

    private Mapping mMapping;

    private FormInfo mFormInfo;
    
    private bool mEnabled = true;

    #endregion

    #region "Public properties"

    /// <summary>
    /// Gets or sets the mapping to display.
    /// </summary>
    public Mapping Mapping
    {
        get
        {
            return mMapping;
        }
        set
        {
            mMapping = value;
        }
    }

    /// <summary>
    /// Gets or sets the value indicating whether the control is enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
        }
    }

    #endregion

    #region "Protected properties"

    protected FormInfo FormInfo
    {
        get
        {
            if (mFormInfo == null)
            {
                ContactFormInfoProvider provider = new ContactFormInfoProvider();
                mFormInfo = provider.GetFormInfo();
            }

            return mFormInfo;
        }
    }

    #endregion

    #region "Life-cycle methods"

    protected override void OnPreRender(EventArgs e)
    {
        if (mMapping == null)
        {
            Visible = false;
        }
        else
        {
            ItemRepeater.DataSource = mMapping.Items;
            ItemRepeater.DataBind();
            if (String.IsNullOrEmpty(mMapping.ExternalIdentifierAttributeName))
            {
                MessageControl.InnerHtml = GetString("sf.noexternalidentifierattribute");
                MessageControl.Attributes.Add("class", "Red");
                MessageControl.Visible = true;
            }
            else
            {
                MessageControl.InnerHtml = String.Format("{0}: {1}", GetString("sf.mapping.externalidentifierattribute"), HTMLHelper.HTMLEncode(mMapping.ExternalIdentifierAttributeLabel));
                MessageControl.Visible = true;
            }
            if (!Enabled)
            {
                ContainerControl.Attributes.Add("class", "Gray");
            }
        }
    }

    #endregion

    #region "Protected methods"

    protected string FormatSourceType(MappingItem item)
    {
        switch (item.SourceType)
        {
            case MappingItemSourceTypeEnum.Field:
                return GetString("sf.sourcetype.field");
            case MappingItemSourceTypeEnum.MetaField:
                return GetString("sf.sourcetype.metafield");
            case MappingItemSourceTypeEnum.PicklistEntry:
                return GetString("sf.sourcetype.picklistentry");
        }

        return String.Empty;
    }

    protected string FormatSourceLabel(MappingItem item)
    {
        return item.SourceLabel;
    }

    #endregion
}