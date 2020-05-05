using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.OutputFilter;
using CMS.PortalEngine.Web.UI;

/// <summary>
/// Cache items form control
/// </summary>
public partial class CMSFormControls_System_CacheItems : FormEngineUserControl
{
    #region "Variables"

    private string mValue = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets enabled state
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            txtItems.Enabled = value;
            btnSelect.Enabled = value;
        }
    }


    /// <summary>
    /// Type of items to show
    /// </summary>
    public String CacheItemsType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CacheItemsType"), "output");
        }
        set
        {
            SetValue("CacheItemsType", value);
        }
    }


    /// <summary>
    /// Gets or sets the field value
    /// </summary>
    public override object Value
    {
        get
        {
            // Get values from list if enabled
            if (pnlList.Visible)
            {
                StringBuilder sb = new StringBuilder();
                foreach (ListItem li in chkList.Items)
                {
                    sb.Append(li.Value, ',', li.Selected, ';');
                }
                return sb.ToString().TrimEnd(';');
            }

            // Otherwise get original value
            return mValue;
        }
        set
        {
            mValue = Convert.ToString(value);
        }
    }


    /// <summary>
    /// Indicates whether control shows output cache items or partial cache items.
    /// </summary>
    private bool OutputCache
    {
        get
        {
            return CacheItemsType.ToLowerCSafe() == "output";
        }

    }

    #endregion


    #region "Methods"

    /// <summary>
    /// OnLoad handler. Ensures button click handler and error message.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        // Display error if values are loaded from web.config
        if (!String.IsNullOrEmpty(Service.Resolve<IAppSettingsService>()[OutputCache ? "CMSOutputCacheItems" : "CMSPartialCacheItems"]))
        {
            lblError.Visible = true;
            lblError.Text = GetString("cacheitems.appvalues");
        }

        // Handle click event
        btnSelect.Click += btnSelect_Click;

        // Load text values
        if (!RequestHelper.IsPostBack())
        {
            txtItems.Text = OutputCache ? OutputHelper.CacheItems : PartialCacheItemsProvider.GetEnabledCacheItems();
        }

        base.OnLoad(e);
    }


    /// <summary>
    /// Process button click
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        // Ensure visibility
        pnlItems.Visible = false;
        pnlList.Visible = true;

        // Fill items
        FillItems();
    }


    /// <summary>
    /// Fills check box list with current values
    /// </summary>
    private void FillItems()
    {
        var items = OutputCache ? GetOutputCacheItemNames() : PartialCacheItemsProvider.GetCacheItemNames();

        foreach (var item in items)
        {
            ListItem li = new ListItem(item.Key, item.Key);
            chkList.Items.Add(li);
            li.Selected = item.Value;
        }
    }


    private Dictionary<String, bool> GetOutputCacheItemNames()
    {
        return CacheHelper.GetCombinedCacheItems(SettingsKeyInfoProvider.GetValue("CMSOutputCacheItems"), OutputHelper.AvailableCacheItemNames);
    }

    #endregion
}