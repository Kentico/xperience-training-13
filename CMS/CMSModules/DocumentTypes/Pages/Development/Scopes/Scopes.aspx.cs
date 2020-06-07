using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


[EditedObject(DocumentTypeInfo.OBJECT_TYPE_DOCUMENTTYPE, "objectid")]
public partial class CMSModules_DocumentTypes_Pages_Development_Scopes_Scopes : GlobalAdminPage
{
    #region "Variables"

    private string currentValues = string.Empty;
    private int siteId = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current document type object.
    /// </summary>
    private DataClassInfo DocumentType
    {
        get
        {
            return (DataClassInfo)EditedObject;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.DisplaySiteSelectorPanel = true;

        siteId = !RequestHelper.IsPostBack() ? SiteContext.CurrentSiteID : ValidationHelper.GetInteger(selectSite.Value, UniSelector.US_ALL_RECORDS);

        selectSite.PostbackOnDropDownChange = true;
        selectSite.UniSelector.OnSelectionChanged += DropDownSingleSelect_SelectedIndexChanged;
        selectSite.UniSelector.WhereCondition = "SiteID IN (SELECT SiteID FROM CMS_ClassSite WHERE ClassID = " + DocumentType.ClassID + ")";
        selectSite.SiteID = siteId;
        selectSite.Reload(false);

        // Ensure correct site selection (doc type isn't assigned to current site fix)
        siteId = ValidationHelper.GetInteger(selectSite.Value, UniSelector.US_ALL_RECORDS);
        
        // Load current values for selected document type
        DataSet ds = DocumentTypeScopeInfoProvider.GetScopesForDocumentType(DocumentType.ClassID, siteId).Column("ScopeID");
        currentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "ScopeID"));

        if (!RequestHelper.IsPostBack())
        {
            usScopes.Value = currentValues;
        }

        if (siteId > 0)
        {
            usScopes.WhereCondition = "ScopeSiteID = " + siteId;
            usScopes.ButtonNewEnabled = true;
        }
        else
        {
            usScopes.ButtonNewEnabled = false;
            usScopes.UniGrid.OnBeforeDataReload += UniGrid_OnBeforeDataReload;
        }

        usScopes.DisabledAddButtonExplanationText = GetString("scopes.doctype.choosesite");
        usScopes.OnSelectionChanged += usScopes_OnSelectionChanged;
    }


    protected void UniGrid_OnBeforeDataReload()
    {
        if ((usScopes.UniGrid.GridView.Columns.Count > 0) && !usScopes.UniGrid.NamedColumns.ContainsKey("SiteName"))
        {
            // Add additional site column
            ExtendedBoundField field = new ExtendedBoundField();
            field.HeaderText = GetString("general.sitename");
            field.DataField = "ScopeSiteID";
            field.ExternalSourceName = "#sitename";
            field.ItemStyle.Wrap = false;
            field.HeaderStyle.Width = new Unit("100%");
            field.OnExternalDataBound += usScopes.UniGrid.RaiseExternalDataBound;

            usScopes.UniGrid.GridView.Columns.Add(field);
            usScopes.UniGrid.NamedColumns.Add("SiteName", field);

            // Clear width for item name column
            usScopes.UniGrid.GridView.Columns[1].HeaderStyle.Width = new Unit();
        }
    }


    protected void DropDownSingleSelect_SelectedIndexChanged(object sender, EventArgs e)
    {
        usScopes.Value = currentValues;
        usScopes.Reload(true);
    }


    protected void usScopes_OnSelectionChanged(object sender, EventArgs e)
    {
        // Remove old items
        string newValues = ValidationHelper.GetString(usScopes.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);

        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Remove document types from scope
                foreach (string item in newItems)
                {
                    DocumentTypeScopeClassInfo.Provider.Remove(ValidationHelper.GetInteger(item, 0), DocumentType.ClassID);
                }
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new document types to scope
                foreach (string item in newItems)
                {
                    DocumentTypeScopeClassInfo.Provider.Add(ValidationHelper.GetInteger(item, 0), DocumentType.ClassID);
                }
            }
        }
    }

    #endregion
}
