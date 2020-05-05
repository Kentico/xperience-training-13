using System;
using System.Collections;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement("CMS.CustomTables", "CustomTables", false, false)]
public partial class CMSModules_CustomTables_Tools_CustomTable_Data_SelectFields : CMSCustomTablesModalPage
{
    #region "Variables"

    protected int customTableId = 0;
    private DataClassInfo dci = null;

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        Save += btnOK_Click;
        RequireSite = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get custom table id from url
        customTableId = QueryHelper.GetInteger("customtableid", 0);

        dci = DataClassInfoProvider.GetDataClassInfo(customTableId);
        
        // Set edited object
        EditedObject = dci;

        // If class exists and is custom table
        if ((dci != null) && dci.ClassIsCustomTable)
        {
            // Ensure that object belongs to current site or user has access to site manager 
            if (!CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin) && (dci.AssignedSites[SiteContext.CurrentSiteName] == null))
            {
                RedirectToInformation(GetString("general.notassigned"));
            }

            PageTitle.TitleText = GetString("customtable.data.selectdisplayedfields");
            CurrentMaster.DisplayActionsPanel = true;

            // Check 'Read' permission
            if (!dci.CheckPermissions(PermissionsEnum.Read, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
            {
                ShowError(String.Format(GetString("customtable.permissiondenied.read"), dci.ClassName));
                plcContent.Visible = false;
                return;
            }

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
                Hashtable reportFields = new Hashtable();

                // Get report fields
                if (!String.IsNullOrEmpty(dci.ClassShowColumns))
                {
                    reportFields.Clear();

                    foreach (string field in dci.ClassShowColumns.Split(';'))
                    {
                        // Add field key to hastable
                        reportFields[field] = null;
                    }
                }

                // Get columns names
                FormInfo fi = FormHelper.GetFormInfo(dci.ClassName, false);
                var columnNames = fi.GetColumnNames(false);

                if (columnNames != null)
                {
                    MacroResolver resolver = MacroResolverStorage.GetRegisteredResolver(FormHelper.FORM_PREFIX + dci.ClassName);
                    FormFieldInfo ffi;
                    ListItem item;
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
        else
        {
            ShowError(GetString("customtable.notcustomtable"));
            CurrentMaster.FooterContainer.Visible = false;
        }
    }

    #endregion


    #region "Button handling"

    /// <summary>
    /// Button OK clicked.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (dci != null)
        {
            string reportFields = null;
            bool itemSelected = (chkListFields.SelectedIndex != -1);

            if (itemSelected)
            {
                foreach (ListItem item in chkListFields.Items)
                {
                    // Display only selected fields
                    if (item.Selected)
                    {
                        reportFields += item.Value + ";";
                    }
                }
            }

            if (!string.IsNullOrEmpty(reportFields))
            {
                // Remove ending ';'
                reportFields = reportFields.TrimEnd(';');
            }


            // Save report fields
            if (CMSString.Compare(dci.ClassShowColumns, reportFields, true) != 0)
            {
                dci.ClassShowColumns = reportFields;
                DataClassInfoProvider.SetDataClassInfo(dci);
            }

            // Close dialog window
            ScriptHelper.RegisterStartupScript(this, typeof(string), "CustomTable_SelectFields", "CloseAndRefresh();", true);
        }
    }

    #endregion


    #region "Protected methods"

    /// <summary>
    /// Returns field caption of the specified column.
    /// </summary>
    /// <param name="ffi">Form field info</param>
    /// <param name="columnName">Column name</param>
    /// <param name="resolver">Macro resolver</param>
    protected string GetFieldCaption(FormFieldInfo ffi, string columnName, MacroResolver resolver)
    {
        // Get field caption     
        string caption = ffi.GetPropertyValue(FormFieldPropertyEnum.FieldCaption, resolver);
        return ((ffi == null) || (caption == string.Empty)) ? columnName : caption;
    }

    #endregion
}
