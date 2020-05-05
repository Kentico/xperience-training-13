using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

public partial class CMSWebParts_UserContributions_ContributionList : CMSAbstractWebPart
{
    #region "Document properties"

    /// <summary>
    /// Gets or sets the class names (document types) separated with semicolon, which should be displayed.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("Classnames"), list.ClassNames), list.ClassNames);
        }
        set
        {
            SetValue("ClassNames", value);
            list.ClassNames = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the default language version of the document 
    /// should be displayed if the document is not translated to the current language.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), list.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            list.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture version of the displayed content.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CultureCode"), list.CultureCode), list.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            list.CultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the maximum nesting level. It specifies the number of sub-levels in the content tree 
    /// that should be included in the displayed content.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), list.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            list.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the ORDER BY part of the SELECT query.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("OrderBy"), list.OrderBy), list.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            list.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the path to the document.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), null);
        }
        set
        {
            SetValue("Path", value);
            list.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether to show only published documents.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), list.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            list.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Gets or sets the codename of the site from which you want to display the content.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SiteName"), list.SiteName), list.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            list.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the WHERE part of the SELECT query.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WhereCondition"), list.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            list.WhereCondition = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether the list of documents should be displayed.
    /// </summary>
    public bool DisplayList
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayList"), list.DisplayList);
        }
        set
        {
            SetValue("DisplayList", value);
            list.DisplayList = value;
        }
    }


    /// <summary>
    /// Gets or sets the path for new created documents.
    /// </summary>
    public string NewDocumentPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NewDocumentPath"), list.NewDocumentPath);
        }
        set
        {
            SetValue("NewDocumentPath", value);
            list.NewDocumentPath = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether inserting new document is allowed.
    /// </summary>
    public bool AllowInsert
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowInsert"), list.AllowInsert);
        }
        set
        {
            SetValue("AllowInsert", value);
            list.AllowInsert = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether editing document is allowed.
    /// </summary>
    public bool AllowEdit
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowEdit"), list.AllowEdit);
        }
        set
        {
            SetValue("AllowEdit", value);
            list.AllowEdit = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether deleting document is allowed.
    /// </summary>
    public bool AllowDelete
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowDelete"), list.AllowDelete);
        }
        set
        {
            SetValue("AllowDelete", value);
            list.AllowDelete = value;
        }
    }


    /// <summary>
    /// Gets or sets the group of users which can work with the documents.
    /// </summary>
    public UserContributionAllowUserEnum AllowUsers
    {
        get
        {
            object value = GetValue("AllowUsers");

            if (value == null)
            {
                return UserContributionAllowUserEnum.DocumentOwner;
            }
            else if (ValidationHelper.IsInteger(value))
            {
                return (UserContributionAllowUserEnum)(ValidationHelper.GetInteger(value, 2));
            }
            else
            {
                return (UserContributionAllowUserEnum)(value);
            }
        }
        set
        {
            SetValue("AllowUsers", value);
            list.AllowUsers = value;
        }
    }


    /// <summary>
    /// Gets or sets the page template the new items are assigned to.
    /// </summary>
    public string NewItemPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NewItemPageTemplate"), list.NewItemPageTemplate);
        }
        set
        {
            SetValue("NewItemPageTemplate", value);
            list.NewItemPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the type of the child documents that are allowed to be created.
    /// </summary>
    public string AllowedChildClasses
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AllowedChildClasses"), list.AllowedChildClasses);
        }
        set
        {
            SetValue("AllowedChildClasses", value);
            list.AllowedChildClasses = value;
        }
    }


    /// <summary>
    /// Gets or sets alternative form name.
    /// </summary>
    public string AlternativeFormName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AlternativeFormName"), list.AlternativeFormName);
        }
        set
        {
            SetValue("AlternativeFormName", value);
            list.AlternativeFormName = value;
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed after validation failed.
    /// </summary>
    public string ValidationErrorMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ValidationErrorMessage"), list.ValidationErrorMessage);
        }
        set
        {
            SetValue("ValidationErrorMessage", value);
            list.ValidationErrorMessage = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the permissions are checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), list.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            list.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates if document type permissions are required to create new document.
    /// </summary>
    public bool CheckDocPermissionsForInsert
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckDocPermissionsForInsert"), list.CheckDocPermissionsForInsert);
        }
        set
        {
            SetValue("CheckDocPermissionsForInsert", value);
            list.CheckDocPermissionsForInsert = value;
        }
    }


    /// <summary>
    /// Gets or sets new item button label.
    /// </summary>
    public string NewItemButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("NewItemButtonText"), list.NewItemButtonText);
        }
        set
        {
            SetValue("NewItemButtonText", value);
            list.NewItemButtonText = value;
        }
    }


    /// <summary>
    /// Gets or sets List button label.
    /// </summary>
    public string ListButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ListButtonText"), list.ListButtonText);
        }
        set
        {
            SetValue("ListButtonText", value);
            list.ListButtonText = value;
        }
    }


    /// <summary>
    /// Indicates whether activity is logged.
    /// </summary>
    public bool LogActivity
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("LogActivity"), false);
        }
        set
        {
            SetValue("LogActivity", value);
            list.LogActivity = value;
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
        AttachEvents();
    }


    /// <summary>
    /// Reloads data for partial caching.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        AttachEvents();
    }


    /// <summary>
    /// Event registration.
    /// </summary>
    protected void AttachEvents()
    {
        list.EditForm.OnAfterApprove += EditForm_OnAfterChange;
        list.EditForm.OnAfterReject += EditForm_OnAfterChange;
        list.EditForm.OnAfterDelete += EditForm_OnAfterChange;
        list.EditForm.CMSForm.OnAfterSave += CMSForm_OnAfterSave;
        list.OnAfterDelete += EditForm_OnAfterChange;
    }


    /// <summary>
    /// EditForm after change event handler.
    /// </summary>
    private void EditForm_OnAfterChange(object sender, EventArgs e)
    {
        CMSForm_OnAfterSave(sender, e);
    }


    /// <summary>
    /// CMSForm after save event handler.
    /// </summary>
    private void CMSForm_OnAfterSave(object sender, EventArgs e)
    {
        if (!StandAlone)
        {
            // Reload data after saving the document
            PagePlaceholder.ClearCache();
            PagePlaceholder.ReloadData();
        }
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            list.StopProcessing = true; // Do nothing
        }
        else
        {
            list.ControlContext = ControlContext;
            list.AllowEdit = AllowEdit;
            list.AllowInsert = AllowInsert;
            list.AllowDelete = AllowDelete;
            list.CheckPermissions = CheckPermissions;
            list.CheckDocPermissionsForInsert = CheckDocPermissionsForInsert;
            list.AllowedChildClasses = AllowedChildClasses;
            list.NewItemPageTemplate = NewItemPageTemplate;
            list.AllowUsers = AllowUsers;
            list.WhereCondition = WhereCondition;
            list.SiteName = SiteName;
            list.SelectOnlyPublished = SelectOnlyPublished;
            list.Path = Path;
            list.OrderBy = OrderBy;
            list.MaxRelativeLevel = MaxRelativeLevel;
            list.CultureCode = CultureCode;
            list.CombineWithDefaultCulture = CombineWithDefaultCulture;
            list.ClassNames = ClassNames;
            list.DisplayList = DisplayList;
            list.NewDocumentPath = NewDocumentPath;
            list.AlternativeFormName = AlternativeFormName;
            list.ValidationErrorMessage = ValidationErrorMessage;
            list.NewItemButtonText = NewItemButtonText;
            list.ListButtonText = ListButtonText;
            list.LogActivity = LogActivity;
            list.ComponentName = WebPartID;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (list.StopProcessing)
        {
            // Hide control if stop processing is set
            list.Visible = false;
        }

        base.OnPreRender(e);
    }
}