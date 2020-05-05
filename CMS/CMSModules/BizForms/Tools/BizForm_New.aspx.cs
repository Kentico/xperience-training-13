using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineForms;
using CMS.OnlineForms.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


[Title("BizForm_New.HeaderCaption")]
[UIElement("CMS.Form", "Form.AddForm")]
public partial class CMSModules_BizForms_Tools_BizForm_New : CMSBizFormPage
{
    private string mFormTablePrefix;


    /// <summary>
    /// Returns prefix for bizform table name.
    /// </summary>
    private string FormTablePrefix
    {
        get
        {
            if (string.IsNullOrEmpty(mFormTablePrefix))
            {
                mFormTablePrefix = BizFormHelper.GetFormTablePrefix(SiteContext.CurrentSiteName);
            }

            return mFormTablePrefix;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check 'CreateForm' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "CreateForm"))
        {
            RedirectToAccessDenied("cms.form", "CreateForm");
        }

        // Validator initializations
        rfvFormDisplayName.ErrorMessage = GetString("BizForm_Edit.ErrorEmptyFormDispalyName");
        rfvFormName.ErrorMessage = GetString("BizForm_Edit.ErrorEmptyFormName");
        rfvTableName.ErrorMessage = GetString("BizForm_Edit.ErrorEmptyFormTableName");

        // Control initializations
        lblFormDisplayName.Text = GetString("BizForm_Edit.FormDisplayNameLabel");
        lblFormName.Text = GetString("BizForm_Edit.FormNameLabel");
        lblTableName.Text = GetString("BizForm_Edit.TableNameLabel");
        lblPrefix.Text = FormTablePrefix + "&nbsp;";
        // Remove prefix length from maximum allowed length of table name
        txtTableName.MaxLength = 100 - FormTablePrefix.Length;
        btnOk.Text = GetString("General.OK");

        // Page title control initialization
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("BizForm_Edit.ItemListLink"),
            RedirectUrl = "~/CMSModules/BizForms/Tools/BizForm_List.aspx"
        });

        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("BizForm_Edit.NewItemCaption")
        });
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (!BizFormInfoProvider.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.BizForms, ObjectActionEnum.Insert))
        {
            ShowError(GetString("LicenseVersion.BizForm"));
            return;
        }

        string formDisplayName = txtFormDisplayName.Text.Trim();
        string formName = txtFormName.Text.Trim();
        string tableName = txtTableName.Text.Trim();

        string errorMessage = new Validator().NotEmpty(formDisplayName, rfvFormDisplayName.ErrorMessage).
                                              NotEmpty(formName, rfvFormName.ErrorMessage).
                                              NotEmpty(tableName, rfvTableName.ErrorMessage).
                                              IsIdentifier(formName, GetString("bizform_edit.errorformnameinidentifierformat")).
                                              IsIdentifier(tableName, GetString("BizForm_Edit.ErrorFormTableNameInIdentifierFormat")).Result;

        if (!String.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return;
        }

        try
        {
            var bizFormObj = BizFormHelper.Create(formDisplayName, formName, tableName, SiteContext.CurrentSite);

            // Redirect to Form builder tab
            string url = UIContextHelper.GetElementUrl("CMS.Form", "Forms.Properties", false, bizFormObj.FormID);
            url = URLHelper.AddParameterToUrl(url, "tabname", "Forms.FormBuilderMVC");
            URLHelper.Redirect(url);
        }
        catch (BizFormTableNameNotUniqueException ex)
        {
            ShowError(string.Format(GetString("bizform_edit.errortableexists"), ex.TableName));
        }
        catch(BizFormException ex)
        {
            Service.Resolve<IEventLogService>().LogException("BIZFORM_NEW", EventType.ERROR, ex);
            ShowError(ex.InnerException?.Message ?? ex.Message);
        }
    }
}