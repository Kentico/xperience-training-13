using System;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.EmailEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;

public partial class CMSModules_EmailTemplates_FormControls_EmailTemplateTypeSelector : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the enabled state.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return drpEmailTemplateType.Enabled;
        }
        set
        {
            drpEmailTemplateType.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return drpEmailTemplateType.SelectedValue;
        }
        set
        {
            EnsureItems();
            drpEmailTemplateType.SelectedValue = ValidationHelper.GetString(value, string.Empty);
        }
    }


    /// <summary>
    /// Drop down control
    /// </summary>
    public CMSDropDownList DropDown
    {
        get
        {
            return drpEmailTemplateType;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Fill the e-mail template type enumeration
            EnsureItems();
        }
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void EnsureItems()
    {
        if (drpEmailTemplateType.Items.Count == 0)
        {
            bool isGlobalTemplate = UIContext.SiteID <= 0;

            drpEmailTemplateType.Items.AddRange(
                EmailTemplateTypeRegister.Current.GetTemplateTypes()
                    .Where(type => isGlobalTemplate || !type.IsGlobalOnly)
                    .Select(it => new ListItem(ResHelper.GetString(it.DisplayNameResourceString), it.Name))
                    .OrderBy(it => it.Text)
                    .ToArray()
                );
        }
    }

    #endregion
}