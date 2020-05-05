using System;
using System.Runtime.InteropServices;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Guid("8EF3FF11-429C-4438-B922-A0884CED307F")]
public partial class CMSModules_ContactManagement_Controls_UI_Contact_Edit : CMSAdminEditControl
{
    #region "Properties"

    /// <summary>
    /// Event that fires after saving the form.
    /// </summary>
    public event EventHandler OnAfterSave
    {
        add
        {
            EditForm.OnAfterSave += value;
        }
        remove
        {
            EditForm.OnAfterSave -= value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        
        EditForm.OnAfterDataLoad += EditForm_OnAfterDataLoad;
        EditForm.OnBeforeSave += EditForm_OnBeforeSave;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        InitHeaderActions();
        InitEditFormsRedirectURLs();
    }


    #endregion


    #region "UIform events"

    protected void EditForm_OnCreate(object sender, EventArgs e)
    {
        EditForm.AlternativeFormName = IsFullContact() ? "ContactEdit" : "ContactEditSimple";
    }


    /// <summary>
    /// OnAfterDataLoad event handler.
    /// </summary>
    protected void EditForm_OnAfterDataLoad(object sender, EventArgs e)
    {
        if (IsFullContact())
        {
            DistributeParams();
        }
    }


    /// <summary>
    /// OnBeforeSave event handler.
    /// </summary>
    private void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        RepairNoneSelectorValues();
        ChangeContactLastName();
        SetAdministrationFlag();
    }


    private void ChangeContactLastName()
    {
        if (ValidationHelper.GetString(EditForm.Data["ContactLastName"], "") == "")
        {
            EditForm.Data["ContactLastName"] = ContactHelper.ANONYMOUS + DateTime.Now.ToString(ContactHelper.ANONYMOUS_CONTACT_LASTNAME_DATE_PATTERN);
        }
    }


    private void SetAdministrationFlag()
    {
        var contact = (ContactInfo)EditForm.EditedObject;
        contact.ContactCreatedInAdministration = true;
    }


    private void RepairNoneSelectorValues()
    {
        // Repairs (none) selector value
        // UniSelector returns '0' if (none) is selected, but 'null' is required
        string[] fieldNames =
        {
            "ContactCountryID",
            "ContactStatusID"
        };
        foreach (string fieldName in fieldNames)
        {
            if (ValidationHelper.GetInteger(EditForm.Data[fieldName], 0) <= 0)
            {
                EditForm.Data[fieldName] = null;
            }
        }
    }


    /// <summary>
    /// OnAfterSave event handler.
    /// </summary>
    protected void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        ContactInfo contact = (ContactInfo)EditForm.EditedObject;

        // Refresh breadcrumbs
        ScriptHelper.RefreshTabHeader(Page, contact.ContactDescriptiveName);
        
    }

    #endregion


    #region "Methods"

    private void InitHeaderActions()
    {
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }

    private void InitEditFormsRedirectURLs()
    {
        var afterCreateUrl = UIContextHelper.GetElementUrl(ModuleName.CONTACTMANAGEMENT, "EditContact");
        afterCreateUrl = URLHelper.AddParameterToUrl(afterCreateUrl, "objectid", "{%EditedObject.ID%}");
        afterCreateUrl = URLHelper.AddParameterToUrl(afterCreateUrl, "displaytitle", "0");
        afterCreateUrl = URLHelper.AddParameterToUrl(afterCreateUrl, "saved", "1");
        
        var afterSaveUrl = UIContextHelper.GetElementUrl(ModuleName.CONTACTMANAGEMENT, "ContactProfile");
        afterSaveUrl = URLHelper.AddParameterToUrl(afterSaveUrl, "objectid", "{%EditedObject.ID%}");
        afterSaveUrl = URLHelper.AddParameterToUrl(afterSaveUrl, "contactid", "{%EditedObject.ID%}");

        EditForm.RedirectUrlAfterSave = afterSaveUrl;
        EditForm.RedirectUrlAfterCreate = afterCreateUrl;
    }
    

    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        // Check permission
        AuthorizationHelper.AuthorizedModifyContact(true);

        switch (e.CommandName.ToLowerCSafe())
        {
            // Save contact
            case "save":
                EditForm.SaveData(null);
                break;
        }
    }


    /// <summary>
    /// Distributes parameters to form controls.
    /// </summary>
    private void DistributeParams(object sender = null, EventArgs eventArgs = null)
    {
        if (EditForm.FieldControls == null)
        {
            // Try to call that later, if controls are not initialized yet
            EditForm.Load += DistributeParams;
            return;
        }

        // UserSelector
        SetControl("contactowneruserid", ctrl => ctrl.SetValue("wherecondition", "UserName NOT LIKE N'public'"));

        // CampaignSelector
        SetControl("contactcampaign", ctrl => ctrl.SetValue("nonerecordvalue", string.Empty));
    }


    /// <summary>
    /// Performs an action on found control.
    /// </summary>
    private void SetControl(string controlName, Action<FormEngineUserControl> action)
    {
        var control = EditForm.FieldControls[controlName];
        if (control != null)
        {
            action(control);
        }
    }

    #endregion


    private bool IsFullContact()
    {
        return ObjectFactory<ILicenseService>.StaticSingleton().IsFeatureAvailable(FeatureEnum.FullContactManagement);
    }
}