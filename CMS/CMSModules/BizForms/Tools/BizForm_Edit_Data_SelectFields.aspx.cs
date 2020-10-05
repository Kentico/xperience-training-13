using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.OnlineForms;
using CMS.SiteProvider;
using CMS.UIControls;


[EditedObject(BizFormInfo.OBJECT_TYPE, "formId")]
[Security(Resource = "cms.form", Permission = "ReadData")]
[UIElement("cms.form", "Form")]
public partial class CMSModules_BizForms_Tools_BizForm_Edit_Data_SelectFields : CMSModalPage
{
    #region "Variables"

    private BizFormInfo formInfo = null;

    #endregion


    #region "Properties"

    protected BizFormInfo FormInfo
    {
        get
        {
            return formInfo ?? (formInfo = EditedObject as BizFormInfo);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (EditedObject == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        Save += btnOK_Click;

        var user = MembershipContext.AuthenticatedUser;

        // Check permissions for Forms application        
        if (!user.IsAuthorizedPerUIElement("cms.form", "Form"))
        {
            RedirectToUIElementAccessDenied("cms.form", "Form");
        }

        // Check 'ReadData' permission
        if (!user.IsAuthorizedPerResource("cms.form", "ReadData", SiteInfoProvider.GetSiteName(FormInfo.FormSiteID)))
        {
            RedirectToAccessDenied("cms.form", "ReadData");
        }

        // Check authorized roles for this form
        if (!FormInfo.IsFormAllowedForUser(MembershipContext.AuthenticatedUser.UserName, SiteContext.CurrentSiteName))
        {
            RedirectToAccessDenied(GetString("Bizforms.FormNotAllowedForUserRoles"));
        }

        List<String> columnNames = null;
        DataClassInfo dci = null;
        Hashtable reportFields = new Hashtable();
        FormInfo fi = null;

        // Initialize controls
        PageTitle.TitleText = GetString("BizForm_Edit_Data_SelectFields.Title");
        CurrentMaster.DisplayActionsPanel = true;

        HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("UniSelector.SelectAll"),
            OnClientClick = "ChangeFields(true); return false;",
            ButtonStyle = ButtonStyle.Default,
        });

        HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("UniSelector.DeselectAll"),
            OnClientClick = "ChangeFields(false); return false;",
            ButtonStyle = ButtonStyle.Default,
        });

        if (!RequestHelper.IsPostBack())
        {

            if (FormInfo != null)
            {
                // Get dataclass info
                dci = DataClassInfoProvider.GetDataClassInfo(FormInfo.FormClassID);

                if (dci != null)
                {
                    // Get columns names
                    fi = FormHelper.GetFormInfo(dci.ClassName, false);

                    columnNames = fi.GetColumnNames(false, i => i.System);
                }

                // Get report fields
                if (String.IsNullOrEmpty(FormInfo.FormReportFields))
                {
                    reportFields = LoadReportFields(columnNames);
                }
                else
                {
                    reportFields.Clear();

                    foreach (string field in FormInfo.FormReportFields.Split(';'))
                    {
                        // Add field key to hastable
                        reportFields[field] = null;
                    }
                }

                if (columnNames != null)
                {
                    FormFieldInfo ffi = null;
                    ListItem item = null;

                    MacroResolver resolver = MacroResolverStorage.GetRegisteredResolver(FormHelper.FORM_PREFIX + FormInfo.ClassName);

                    foreach (string name in columnNames)
                    {
                        ffi = fi.GetFormField(name);

                        // Add checkboxes to the list
                        item = new ListItem(HTMLHelper.HTMLEncode(ResHelper.LocalizeString(GetFieldCaption(ffi, name, resolver))), name);
                        if (reportFields.Contains(name))
                        {
                            // Select checkbox if field is reported
                            item.Selected = true;
                        }
                        chkListFields.Items.Add(item);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Button OK clicked.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        string reportFields = string.Empty;
        bool noItemSelected = (chkListFields.SelectedIndex == -1) ? true : false;

        foreach (ListItem item in chkListFields.Items)
        {
            // Display all fields
            if (noItemSelected)
            {
                reportFields += item.Value + ";";
            }
            // Display only selected fields
            else if (item.Selected)
            {
                reportFields += item.Value + ";";
            }
        }

        if (!string.IsNullOrEmpty(reportFields))
        {
            // Remove ending ';'
            reportFields = reportFields.TrimEnd(';');
        }

        // Save report fields
        FormInfo.FormReportFields = reportFields;
        BizFormInfo.Provider.Set(FormInfo);

        // Close dialog window
        ScriptHelper.RegisterStartupScript(this, typeof(string), "Forms_SelectFields", "CloseAndRefresh();", true);
    }


    /// <summary>
    /// Returns field caption of the specified column.
    /// </summary>
    /// <param name="ffi">Form field info</param>
    /// <param name="columnName">Column name</param>    
    protected string GetFieldCaption(FormFieldInfo ffi, string columnName, MacroResolver resolver)
    {
        string fieldCaption = string.Empty;

        // get field caption        
        if (ffi != null)
        {
            fieldCaption = ffi.GetDisplayName(resolver);
        }
        if (string.IsNullOrEmpty(fieldCaption))
        {
            fieldCaption = columnName;
        }

        return fieldCaption;
    }


    /// <summary>
    /// Returns report fields hashtable.
    /// </summary>
    protected Hashtable LoadReportFields(IEnumerable<string> columns)
    {
        Hashtable table = new Hashtable();

        if (columns != null)
        {
            foreach (string str in columns)
            {
                table.Add(str, null);
            }
        }
        return table;
    }

    #endregion
}
