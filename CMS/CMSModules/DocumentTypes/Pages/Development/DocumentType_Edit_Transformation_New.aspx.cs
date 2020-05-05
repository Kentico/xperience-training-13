using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


[Breadcrumbs]
[Breadcrumb(0, "documenttype_edit_transformation_edit.transformations", "DocumentType_Edit_Transformation_List.aspx?parentobjectid={?parentobjectid?}", null)]
[Breadcrumb(1, "documenttype_edit_transformation_edit.newtransformation")]
public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Transformation_New : CMSDesignPage
{
    #region "Constants"

    private const string HELP_TOPIC_LINK = "transformations_writing";

    #endregion


    #region "Properties"

    /// <summary>
    /// If true, control is used in site manager.
    /// </summary>
    public bool IsSiteManager
    {
        get
        {
            return filter.IsSiteManager;
        }
        set
        {
            filter.IsSiteManager = value;
        }
    }


    public bool DialogMode
    {
        get
        {
            return QueryHelper.GetBoolean("editonlycode", false);
        }
    }


    public bool IsCustomTable
    {
        get
        {
            return QueryHelper.GetBoolean("iscustomtable", false);
        }
    }


    public int ClassID
    {
        get
        {
            return QueryHelper.GetInteger("parentobjectid", 0);
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (DialogMode)
        {
            // Check hash
            if (!ValidationHelper.ValidateHash("?editonlycode=1", QueryHelper.GetString("hash", String.Empty), new HashSettings("")))
            {
                URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext"));
            }
            // Setup help
            PageTitle.HelpTopicName = HELP_TOPIC_LINK;
        }

        editElem.OnAfterSave += editElem_OnAfterSave;

        var transformation = editElem.EditedObject as TransformationInfo;
        if (transformation != null)
        {
            // Set parent identifier
            transformation.TransformationClassID = (ClassID > 0) ? ClassID : filter.ClassId;
        }

        ucTransfCode.ClassID = ClassID;
        if (ClassID <= 0)
        {
            // Class is not defined, display doc type filter to select under which class the transformation should be crated
            filter.SelectedValue = QueryHelper.GetString("selectedvalue", null);
            filter.FilterMode = TransformationInfo.OBJECT_TYPE;

            ucTransfCode.ClassID = filter.ClassId;

            CurrentMaster.DisplayControlsPanel = true;
        }
        else
        {
            CurrentMaster.DisplayControlsPanel = false;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        int classId = ClassID;
        if (classId <= 0)
        {
            classId = filter.ClassId;
        }

        var classInfo = DataClassInfoProvider.GetDataClassInfo(classId);
        if ((classInfo != null) && ((classInfo.ClassIsCustomTable) || (classInfo.ClassIsDocumentType && classInfo.ClassIsCoupledClass)))
        {

            // Generate default
            HeaderAction generate = new HeaderAction
            {
                Text = GetString("DocumentType_Edit_Transformation_Edit.ButtonDefault"),
                Tooltip = GetString("transformationtypecode.generatetooltip"),
                OnClientClick = "GenerateDefaultCode('default'); return false;"
            };

            if (CurrentMaster.ObjectEditMenu != null)
            {
                CurrentMaster.ObjectEditMenu.AddExtraAction(generate);
            }

            if (ucTransfCode.IsAscx)
            {
                HeaderAction atom = new HeaderAction
                {
                    Text = GetString("transformationtypecode.atom"),
                    OnClientClick = "GenerateDefaultCode('atom'); return false;",
                    Tooltip = GetString("transformationtypecode.atomtooltip")
                };

                HeaderAction xml = new HeaderAction
                {
                    Text = GetString("transformationtypecode.xml"),
                    Tooltip = GetString("transformationtypecode.xmltooltip"),
                    OnClientClick = "GenerateDefaultCode('xml'); return false;"
                };

                HeaderAction rss = new HeaderAction
                {
                    Text = GetString("transformationtypecode.rss"),
                    OnClientClick = "GenerateDefaultCode('rss'); return false;",
                    Tooltip = GetString("transformationtypecode.rsstooltip")
                };

                generate.AlternativeActions.Add(atom);
                generate.AlternativeActions.Add(rss);
                generate.AlternativeActions.Add(xml);
            }
        }

        base.OnPreRender(e);

        if (DialogMode)
        {
            PageBreadcrumbs.Items.Clear();
            PageTitle.TitleText = GetString("DocumentType_Edit_Transformation_Edit.NewTransformation");
        }
        else if (IsCustomTable)
        {
            SetBreadcrumb(0, GetString("DocumentType_Edit_Transformation_Edit.Transformations"), ResolveUrl("~/CMSModules/CustomTables/CustomTable_Edit_Transformation_List.aspx?parentobjectid=" + ClassID), null, null);
        }
    }


    protected void editElem_OnAfterSave(object sender, EventArgs e)
    {
        var ti = editElem.EditedObject as TransformationInfo;
        if (ti != null)
        {
            // If dialog mode redirect after, selector is updated
            var url = GetEditUrl(IsCustomTable ? "CMS.CustomTables" : "CMS.DocumentEngine", "EditTransformation", ti);


            URLHelper.Redirect(url);
        }
    }


    #region "Private methods"

    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    /// <param name="resourceName">Resource name</param>
    /// <param name="elementName">Element name</param>
    /// <param name="transformation">Transformation object info</param>
    private String GetEditUrl(string resourceName, string elementName, TransformationInfo transformation)
    {
        var uiChild = UIElementInfoProvider.GetUIElementInfo(resourceName, elementName);
        if (uiChild != null)
        {
            string url = String.Empty;
            string query = RequestContext.CurrentQueryString;

            if (!DialogMode)
            {
                url = UIContextHelper.GetElementUrl(uiChild, UIContext);
                url = URLHelper.AppendQuery(url, query);
            }
            else
            {
                url = ApplicationUrlHelper.GetElementDialogUrl(uiChild, 0, query);
            }


            return URLHelper.AppendQuery(url, "objectid=" + transformation.TransformationID);
        }

        return String.Empty;
    }

    #endregion
}