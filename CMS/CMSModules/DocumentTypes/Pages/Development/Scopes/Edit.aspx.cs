using System;
using System.Data;

using CMS.Base;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


[EditedObject(DocumentTypeScopeInfo.OBJECT_TYPE, "scopeid")]

[Breadcrumb(0, "template.scopes", "List.aspx?siteid={?siteid?}", "")]
[Breadcrumb(1, "template.scopes.new", NewObject = true)]
[Breadcrumb(2, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
public partial class CMSModules_DocumentTypes_Pages_Development_Scopes_Edit : GlobalAdminPage
{
    protected string currentValues = string.Empty;

    #region "Properties"

    /// <summary>
    /// Edited scope.
    /// </summary>
    private DocumentTypeScopeInfo Scope
    {
        get
        {
            return (DocumentTypeScopeInfo)EditedObject;
        }
    }


    /// <summary>
    /// Site identifier of edited scope.
    /// </summary>
    private int SiteID
    {
        get
        {
            return (Scope != null) && (Scope.ScopeID > 0) ? Scope.ScopeSiteID : QueryHelper.GetInteger("siteid", 0);
        }
    }

    #endregion


    #region "Page and controls events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        form.OnAfterDataLoad += form_OnAfterDataLoad;
    }


    /// <summary>
    /// Page load event.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Help topic
        form.OnAfterSave += form_OnAfterSave;
        form.OnBeforeValidate += form_OnBeforeValidate;
    }


    protected void form_OnAfterDataLoad(object sender, EventArgs e)
    {
        // Assign selected site name for filtering
        UniSelector usClasses = (UniSelector)form.FieldControls["ScopeClasses"];
        if (usClasses != null)
        {
            usClasses.ObjectSiteName = SiteInfoProvider.GetSiteName(SiteID);
        }

        // Load current bindings
        if ((Scope != null) && (Scope.ScopeID > 0))
        {
            DataSet ds = DocumentTypeScopeClassInfoProvider.GetScopeClasses("ScopeID = " + Scope.ScopeID, "ScopeID", 0, "ClassID");

            currentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "ClassID"));

            form.FieldControls["ScopeClasses"].Value = currentValues;
        }
    }


    protected void form_OnBeforeValidate(object sender, EventArgs e)
    {
        // Get single node path
        string path = TreePathUtils.EnsureSingleNodePath(form.FieldControls["ScopePath"].Value.ToString());

        // Ensure slash at the beginning
        if (!string.IsNullOrEmpty(path) && !path.StartsWithCSafe("/"))
        {
            path = "/" + path;
        }

        form.FieldControls["ScopePath"].Value = path;
    }


    /// <summary>
    /// OnBeforeDataLoad event handler.
    /// </summary>
    protected void form_OnBeforeDataLoad(object sender, EventArgs e)
    {
        form.ObjectSiteID = SiteID;
    }


    /// <summary>
    /// OnAfterSave event handler.
    /// </summary>
    protected void form_OnAfterSave(object sender, EventArgs e)
    {
        // Remove old items
        string newValues = ValidationHelper.GetString(form.FieldControls["ScopeClasses"].Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);

        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Add all new items to site
            foreach (string item in newItems)
            {
                DocumentTypeScopeClassInfo.Provider.Remove(Scope.ScopeID, ValidationHelper.GetInteger(item, 0));
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Add all new items to site
            foreach (string item in newItems)
            {
                DocumentTypeScopeClassInfo.Provider.Add(Scope.ScopeID, ValidationHelper.GetInteger(item, 0));
            }
        }
    }

    #endregion
}