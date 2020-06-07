using System;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.PortalEngine;
using CMS.SiteProvider;


public partial class CMSModules_PortalEngine_FormControls_PageTemplates_SelectPageTemplate : FormEngineUserControl
{
    #region "Variables"

    private PageTemplateInfo pti;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Enables or disables the control.
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
            btnClear.Enabled = value;
            txtTemplate.Enabled = value;
            btnSelect.Enabled = value;
        }
    }


    /// <summary>
    /// Root category name
    /// </summary>
    public String RootCategoryName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RootCategoryName"), String.Empty);
        }
        set
        {
            SetValue("RootCategoryName", value);
        }
    }


    /// <summary>
    /// Selected page template info object
    /// </summary>
    public PageTemplateInfo PageTemplateInfo
    {
        get
        {
            int templateId = ValidationHelper.GetInteger(hdnSelected.Value, 0);

            if ((pti != null) && (pti.PageTemplateId != templateId))
            {
                pti = null;
            }
            if (pti == null)
            {
                // Load by template ID in the hidden field
                pti = PageTemplateInfoProvider.GetPageTemplateInfo(templateId);
            }
            else if (templateId <= 0)
            {
                // Clear the page template info in case no value is selected
                pti = null;
            }

            return pti;
        }
    }


    /// <summary>
    /// Gets or sets page template ID.
    /// </summary>
    public int PageTemplateID
    {
        get
        {
            // Get the page template code name
            var ti = PageTemplateInfo;

            return ti?.PageTemplateId ?? 0;
        }
        set
        {
            // Get the page template
            pti = PageTemplateInfoProvider.GetPageTemplateInfo(value);

            // Update the selected ID
            UpdateSelectedID();
        }
    }


    /// <summary>
    /// Name of return column
    /// </summary>
    public String ReturnColumnName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ReturnColumnName"), String.Empty);
        }
        set
        {
            SetValue("ReturnColumnName", value);
        }
    }


    /// <summary>
    /// Gets or sets field value (Page template code name).
    /// </summary>
    public override object Value
    {
        get
        {
            // Get the page template code name
            var ti = PageTemplateInfo;
            if (ti == null)
            {
                return null;
            }

            if (ReturnColumnName.Equals("pagetemplateid", StringComparison.OrdinalIgnoreCase))
            {
                return ti.PageTemplateId;
            }

            return ti.CodeName;
        }
        set
        {
            if (!RequestHelper.IsPostBack() || (Form == null))
            {
                // Get the page template
                if (ReturnColumnName.Equals("pagetemplateid", StringComparison.OrdinalIgnoreCase) && (value is int))
                {
                    pti = PageTemplateInfoProvider.GetPageTemplateInfo((int)value);
                }
                else
                {
                    pti = PageTemplateInfoProvider.GetPageTemplateInfo((string)value);
                }

                // Update the selected ID
                UpdateSelectedID();
            }
        }
    }


    /// <summary>
    /// Updates the selected ID
    /// </summary>
    private void UpdateSelectedID()
    {
        hdnSelected.Value = pti?.PageTemplateId.ToString() ?? "";
    }


    /// <summary>
    /// Gets ClientID of the textbox with template.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return txtTemplate.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show site page templates only.
    /// </summary>
    public bool ShowOnlySiteTemplates
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowOnlySiteTemplates"), true);
        }
        set
        {
            SetValue("ShowOnlySiteTemplates", value);
        }
    }


    /// <summary>
    /// Root page template category ID
    /// </summary>
    public int RootCategoryID
    {
        get
        {
            return GetValue("RootCategoryID", 0);
        }
        set
        {
            SetValue("RootCategoryID", value);
        }
    }


    /// <summary>
    /// Indicates that adhoc page template was created and save is required.
    /// </summary>
    public bool Save
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Save"), false);
        }
        set
        {
            SetValue("Save", value);
        }
    }


    /// <summary>
    /// Indicates whether clear button should be displayed.
    /// </summary>
    public bool DisplayClearButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayClearButton"), true);
        }
        set
        {
            SetValue("DisplayClearButton", value);
        }
    }


    /// <summary>
    /// Indicates whether template was changed
    /// </summary>
    public bool TemplateChanged
    {
        get
        {
            return ValidationHelper.GetBoolean(hdnTemplateChanged.Value, false);
        }
        set
        {
            hdnTemplateChanged.Value = value.ToString();
            SetValue("TemplateChanged", value);
        }
    }


    /// <summary>
    /// Item GUID for storing AD-HOC templates (UI elements, nodes)
    /// </summary>
    public Guid ItemGuid
    {
        get
        {
            return ValidationHelper.GetGuid(GetValue("ItemGuid"), Guid.Empty);
        }
        set
        {
            SetValue("ItemGuid", value);
        }
    }


    /// <summary>
    /// First load selected category for tree selector
    /// </summary>
    public String TreeSelectedCategory
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TreeSelectedCategory"), String.Empty);
        }
        set
        {
            SetValue("TreeSelectedCategory", value);
        }
    }


    /// <summary>
    /// Item name for storing AD-HOC templates (UI elements, nodes)
    /// </summary>
    public String ItemName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemName"), String.Empty);
        }
        set
        {
            SetValue("ItemName", value);
        }
    }


    /// <summary>
    /// If true, template buttons (new, edit, adhoc are displayed)
    /// </summary>
    public bool ShowTemplateButtons
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowTemplateButtons"), false);
        }
        set
        {
            SetValue("ShowTemplateButtons", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page load event handler
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        pnlButtons.Visible = ShowTemplateButtons;
        btnClear.Visible = DisplayClearButton;

        var template = PageTemplateInfo;
        if (template != null)
        {
            int templateId = template.PageTemplateId;

            if (Enabled)
            {
                // Edit button
                string url = ApplicationUrlHelper.GetElementDialogUrl("cms.design", "PageTemplate.EditPageTemplate", templateId);

                btnEditTemplateProperties.OnClientClick = String.Format("modalDialog('{0}', 'Template edit', '95%', '95%'); return false;", url);
            }
        }
        else
        {
            pnlButtons.Visible = false;
        }

        SetButtonsOnClick();

        if (RequestHelper.IsPostBack() && (hdnSelected.Value == ""))
        {
            hdnSelected.Value = Request.Form[hdnSelected.UniqueID];
        }
    }


    /// <summary>
    /// Select button click
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        pti = null;
    }


    /// <summary>
    /// PreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        LoadData();

        base.OnPreRender(e);

        String treeSel = (TreeSelectedCategory != String.Empty) ? "&treeselectedcategory=" + TreeSelectedCategory : String.Empty;

        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "PageTemplateSelector", ScriptHelper.GetScript(
@"
function PTS_Select(selectorId, rootCategoryId, onlySiteTemplates) {

    var hid = document.getElementById(selectorId + '_hdnSelected');
    var url = '" + ResolveUrl("~/CMSModules/PortalEngine/UI/Layout/PageTemplateSelector.aspx") + @"?selectorid=' + selectorId + '" + treeSel + @"&selectedPageTemplateId=' + hid.value + '&rootcategoryid=' + rootCategoryId + '&onlysitetemplates=' + onlySiteTemplates;
    modalDialog(url, 'PageTemplateSelection', '90%', '85%'); 
    return false;
} 

function OnSelectPageTemplate(templateId, selectorId) { 
    PageTemplateChanged_UpdateSelector(templateId, selectorId);
}

function PageTemplateChanged_UpdateSelector(templateId, selectorId) {
    document.getElementById(selectorId + '_hdnTemplateChanged').value = 'true';
    var hid = document.getElementById(selectorId + '_hdnSelected');
    hid.value = templateId;
    window['PTS_' + selectorId]();
}

function PTS_Clear(selectorId) { 
    document.getElementById(selectorId + '_hdnTemplateChanged').value = 'true';
    document.getElementById(selectorId + '_txtTemplate').value = ''; 
    document.getElementById(selectorId + '_hdnSelected').value = 0; 
    return false;
}"
        ));

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), ClientID, ScriptHelper.GetScript(String.Format(
@"
function PTS_{0}() {{ 
    {1} 
}}
",
            ClientID,
            Page.ClientScript.GetPostBackEventReference(btnSelect, null)
        )));
    }


    /// <summary>
    /// Loads the control data.
    /// </summary>
    public void LoadData()
    {
        var ti = PageTemplateInfo;
        txtTemplate.Text = ti != null ? ResHelper.LocalizeString(ti.DisplayName) : "";
    }


    /// <summary>
    /// Setups action buttons on click.
    /// </summary>
    public void SetButtonsOnClick()
    {
        int id = RootCategoryID;
        if ((id == 0) && (RootCategoryName != String.Empty))
        {
            PageTemplateCategoryInfo ptci = PageTemplateCategoryInfoProvider.GetPageTemplateCategoryInfo(RootCategoryName);
            if (ptci != null)
            {
                id = ptci.CategoryId;
            }
        }

        btnSelect.OnClientClick = "return PTS_Select('" + ClientID + "', '" + id + "', '" + ShowOnlySiteTemplates + "');";
        btnClear.OnClientClick = "PTS_Clear('" + ClientID + "');";
    }


    protected void btnClear_clicked(object sender, EventArgs e)
    {
        btnEditTemplateProperties.Enabled = false;
        btnEditTemplateProperties.OnClientClick = String.Empty;
    }


    public override object GetValue(string propertyName)
    {
        if (string.Equals(propertyName, "templatechanged", StringComparison.OrdinalIgnoreCase))
        {
            return ValidationHelper.GetBoolean(hdnTemplateChanged.Value, false);
        }

        return base.GetValue(propertyName);
    }

    #endregion
}