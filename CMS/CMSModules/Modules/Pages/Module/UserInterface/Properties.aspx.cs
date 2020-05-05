using System;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.PortalEngine;
using CMS.UIControls;


[UIElement(ModuleName.CMS, "Modules.UserInterface.Properties")]
public partial class CMSModules_Modules_Pages_Module_UserInterface_Properties : GlobalAdminPage
{
    #region "Variables"

    UIElementInfo mUIElementInfo;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup info/error message placeholder
        if (MessagesPlaceHolder != null)
        {
            MessagesPlaceHolder.UseRelativePlaceHolder = false;
            form.EnsureMessagesPlaceholder(MessagesPlaceHolder);
        }

        // Load UI element and element's page template (if any)
        mUIElementInfo = UIElementInfoProvider.GetUIElementInfo(QueryHelper.GetInteger("elementID", 0));

        if (mUIElementInfo != null)
        {
            PageTemplateInfo pti = null;
            if (mUIElementInfo.ElementType == UIElementTypeEnum.PageTemplate)
            {
                pti = PageTemplateInfoProvider.GetPageTemplateInfo(mUIElementInfo.ElementPageTemplateID);
            }

            form.Enabled = ((SystemContext.DevelopmentMode || mUIElementInfo.ElementIsCustom) && (!UIElementInfoProvider.AllowEditOnlyCurrentModule || (mUIElementInfo.ElementResourceID == QueryHelper.GetInteger("moduleId", 0))));

            // Create form info
            FormInfo fi = (pti != null) ? pti.PageTemplatePropertiesForm : PortalFormHelper.GetUIElementDefaultPropertiesForm(UIElementPropertiesPosition.Both);
            form.FormInformation = fi;

            // Load data row from properties
            DataRow dr = fi.GetDataRow();
            fi.LoadDefaultValues(dr);
            if ((mUIElementInfo == null) || !mUIElementInfo.IsApplication)
            {
                fi.RemoveFormField("DescriptionLink");
            }

            XmlData customData = new XmlData();

            // Load element properties
            XmlData data = new XmlData();
            data.AllowMacros = true;
            data.LoadData(mUIElementInfo.ElementProperties);

            form.MacroTable = data.MacroTable;

            // Fill template datarow with element's properties. If template does not contain such column, add it to custom
            foreach (String col in data.ColumnNames)
            {
                if (col.StartsWith(UIContextData.CATEGORYNAMEPREFIX, StringComparison.Ordinal))
                {
                    if (!RequestHelper.IsPostBack())
                    {
                        String catName = col.Substring(UIContextData.CATEGORYNAMEPREFIX.Length);
                        FormCategoryInfo fci = fi.GetFormCategory(catName);
                        if (fci != null)
                        {
                            fci.SetPropertyValue(FormCategoryPropertyEnum.CollapsedByDefault, ValidationHelper.GetBoolean(data[col], false).ToString().ToLowerInvariant());
                        }
                    }
                }
                else
                {
                    if (dr.Table.Columns.Contains(col))
                    {
                        DataHelper.SetDataRowValue(dr, col, data[col]);
                    }
                    else
                    {
                        customData[col] = data[col];
                    }
                }
            }

            dr["CustomProperties"] = customData.GetData();

            form.DataRow = dr;
            form.LoadData(dr);
            form.FormInformation = fi;

            // Master page style for margin
            CurrentMaster.PanelContent.CssClass = "WebpartProperties PageContent";

            form.OnAfterSave += form_OnAfterSave;
        }
        else
        {
            EditedObject = null;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        ScriptHelper.RegisterEditScript(Page, false);
        ScriptHelper.RegisterJQuery(Page);

        // Register progress script
        ScriptHelper.RegisterLoader(Page);

        base.OnPreRender(e);
    }


    void form_OnAfterSave(object sender, EventArgs e)
    {
        if (mUIElementInfo != null)
        {
            XmlData data = new XmlData();
            data.AllowMacros = true;

            // Store basic form data to XMLData structure
            DataRow drActual = form.DataRow;
            foreach (DataColumn dc in drActual.Table.Columns)
            {
                if ((dc.ColumnName != "CustomProperties") && (drActual[dc.ColumnName] != DBNull.Value) && (drActual[dc.ColumnName].ToString() != ""))
                {
                    data[dc.ColumnName] = drActual[dc.ColumnName];
                }

                // Append values from macro table
                object o = form.MacroTable[dc.ColumnName.ToLowerCSafe()];
                if (o != null)
                {
                    data[dc.ColumnName] = o;
                }
            }

            // Store category changes
            var categories = form.FormInformation.GetCategoryNames();
            foreach (string category in categories)
            {
                FormCategoryInfo fci = form.FormInformation.GetFormCategory(category);
                if (ValidationHelper.GetBoolean(fci.GetPropertyValue(FormCategoryPropertyEnum.Collapsible, form.ContextResolver), false))
                {
                    bool collapsedByDefault = ValidationHelper.GetBoolean(fci.GetPropertyValue(FormCategoryPropertyEnum.CollapsedByDefault, form.ContextResolver), false);
                    bool collapsed = form.IsCategoryCollapsed(category);

                    if (collapsed != collapsedByDefault)
                    {
                        data[UIContextData.CATEGORYNAMEPREFIX + category] = collapsed;
                    }
                }
            }

            data.MacroTable = form.MacroTable;

            // Add custom data
            XmlData customData = new XmlData();
            customData.LoadData(ValidationHelper.GetString(drActual["CustomProperties"], String.Empty));

            // Add (replace) default properties with custom data
            foreach (String col in customData.ColumnNames)
            {
                data[col] = customData[col];
            }

            mUIElementInfo.ElementProperties = data.GetData();
            mUIElementInfo.Update();
            ShowChangesSaved();
        }
    }

    #endregion
}