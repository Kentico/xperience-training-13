using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_DocTypeSelection : CMSAbstractNewDocumentControl
{
    #region "Variables"

    private BaseInfo mScope;

    #endregion


    #region "Properties"

    /// <summary>
    /// The count of document types found.
    /// </summary>
    public int ClassesCount
    {
        get;
        private set;
    }


    /// <summary>
    /// Gets the best fitting document type scope if exist.
    /// </summary>
    private DocumentTypeScopeInfo Scope
    {
        get
        {
            return (DocumentTypeScopeInfo)InfoHelper.EnsureInfo(ref mScope, () => DocumentTypeScopeInfoProvider.GetScopeInfo(ParentNode));
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup unigrid
        gridClasses.GridView.ShowHeader = false;
        gridClasses.GridView.BorderWidth = 0;
        gridClasses.OnExternalDataBound += gridClasses_OnExternalDataBound;
        gridClasses.OnBeforeDataReload += gridClasses_OnBeforeDataReload;
        gridClasses.OnAfterRetrieveData += gridClasses_OnAfterRetrieveData;

        if (Scope != null)
        {
            // Initialize control by scope settings
            AllowNewLink &= Scope.ScopeAllowLinks;
        }

        lblNewLink.Text = GetString("content.ui.linkexistingdoc");
        lnkNewLink.NavigateUrl = "javascript:modalDialog('" + GetLinkDialogUrl(ParentNodeID) + "', 'contentselectnode', '90%', '85%')";
        
        if (ParentNode != null)
        {
            AllowNewLink &= !ParentNode.IsLink;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        gridClasses.GridView.Columns[1].Visible = false;
        gridClasses.GridView.GridLines = GridLines.None;

        headDocumentTypeSelection.Text = Caption;
        headDocumentTypeSelection.Level = HeadingLevel;

        mainCaptionDocumentTypeSelection.Text = MainCaption;
        mainCaptionDocumentTypeSelection.Level = 3;

        // Show/hide new linked document panel
        if (!AllowNewLink)
        {
            plcNewLinkNew.Visible = false;
        }
    }

    #endregion


    #region "Methods"

    protected void gridClasses_OnBeforeDataReload()
    {
        if (ParentNode != null)
        {
            var currentUser = MembershipContext.AuthenticatedUser;

            // Check permission to create new document
            if (currentUser.IsAuthorizedToCreateNewDocument(ParentNode, null))
            {
                if (ParentNode.IsLink)
                {
                    plcDocumentTypeSelection.Visible = false;
                    ShowInformation(GetString("content.noallowedlinkchild"));
                    return;
                }

                // Apply document type scope
                string where = DocumentTypeScopeInfoProvider.GetScopeClassWhereCondition(Scope).ToString(true);

                if (!String.IsNullOrEmpty(gridClasses.CompleteWhereCondition))
                {
                    where = SqlHelper.AddWhereCondition(where, gridClasses.CompleteWhereCondition);
                }

                // Add extra where condition
                where = SqlHelper.AddWhereCondition(where, Where);

                var parentClassId = ValidationHelper.GetInteger(ParentNode.GetValue("NodeClassID"), 0);

                // Get the allowed child classes
                DataSet ds = AllowedChildClassInfoProvider.GetAllowedChildClasses(parentClassId, SiteContext.CurrentSiteID)
                    .Where(where)
                    .OrderBy("ClassID")
                    .TopN(gridClasses.TopN)
                    .Columns("ClassName", "ClassDisplayName", "ClassID", "ClassIconClass");

                // Check user permissions for "Create" permission
                bool hasNodeAllowCreate = (currentUser.IsAuthorizedPerTreeNode(ParentNode, NodePermissionsEnum.Create) == AuthorizationResultEnum.Allowed);
                bool isAuthorizedToCreateInContent = currentUser.IsAuthorizedPerResource("CMS.Content", "Create");

                // No data loaded yet
                ClassesCount = 0;

                // If dataSet is not empty
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    List<DataRow> rowsToRemove = new List<DataRow>();

                    DataTable table = ds.Tables[0];
                    table.DefaultView.Sort = "ClassDisplayName";

                    DataTable resultTable = table.DefaultView.ToTable();

                    for (int i = 0; i < resultTable.Rows.Count; ++i)
                    {
                        DataRow dr = resultTable.Rows[i];
                        string doc = DataHelper.GetStringValue(dr, "ClassName");

                        // Document type is not allowed, remove it from the data set (Extra check for 'CreateSpecific' permission)
                        if (!isAuthorizedToCreateInContent && !currentUser.IsAuthorizedPerClassName(doc, "Create") && (!currentUser.IsAuthorizedPerClassName(doc, "CreateSpecific") || !hasNodeAllowCreate))
                        {
                            rowsToRemove.Add(dr);
                        }
                    }

                    // Remove the document types
                    foreach (DataRow dr in rowsToRemove)
                    {
                        resultTable.Rows.Remove(dr);
                    }

                    if (!DataHelper.DataSourceIsEmpty(resultTable))
                    {
                        ClassesCount = resultTable.Rows.Count;

                        var classes = new DataSet();
                        classes.Tables.Add(resultTable);

                        gridClasses.DataSource = classes;
                    }
                    else
                    {
                        // Show message
                        SetInformationMessage(GetString(Scope != null ? "Content.ScopeApplied" : "Content.NoPermissions"));

                        gridClasses.Visible = false;

                        ClassesCount = -1;
                    }
                }
                else
                {
                    if (!gridClasses.FilterIsSet)
                    {
                        // Show message
                        SetInformationMessage(NoDataMessage);
                    }
                    else
                    {
                        gridClasses.ZeroRowsText = NoDataMessage;
                    }
                }
            }
            else
            {
                // Show message
                SetInformationMessage(GetString("Content.NoPermissions"));
            }
        }
    }


    protected DataSet gridClasses_OnAfterRetrieveData(DataSet ds)
    {
        // Check if there are more options
        if (RedirectWhenNoChoice
            && !AllowNewLink
            && !RequestHelper.IsPostBack()
            && !DataHelper.DataSourceIsEmpty(ds))
        {
            DataTable table = ds.Tables[0];
            if (table.Rows.Count == 1)
            {
                int classId = ValidationHelper.GetInteger(table.Rows[0]["ClassId"], 0);

                // Redirect when only one document type found
                if (!string.IsNullOrEmpty(SelectionUrl))
                {
                    string url = GetSelectionUrl(classId);
                    if (IsInDialog)
                    {
                        url = URLHelper.UpdateParameterInUrl(url, "reloadnewpage", "1");
                    }
                    URLHelper.Redirect(UrlResolver.ResolveUrl(url));
                }
            }
        }

        return ds;
    }


    protected object gridClasses_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (sourceName.ToLowerInvariant() == "classname")
        {
            DataRowView drv = (DataRowView)parameter;

            // Get properties
            string className = ValidationHelper.GetString(drv["ClassName"], String.Empty);
            string classDisplayName = ResHelper.LocalizeString(ValidationHelper.GetString(drv["ClassDisplayName"], String.Empty));
            int classId = ValidationHelper.GetInteger(drv["ClassId"], 0);
            string iconClass = ValidationHelper.GetString(drv["ClassIconClass"], String.Empty);

            string nameFormat = UIHelper.GetDocumentTypeIcon(Page, className, iconClass) + "{0}";

            // Append link if url specified
            if (!String.IsNullOrEmpty(SelectionUrl))
            {
                string url = GetSelectionUrl(classId);
                if (IsInDialog)
                {
                    url = URLHelper.UpdateParameterInUrl(url, "dialog", "1");
                    url = URLHelper.UpdateParameterInUrl(url, "reloadnewpage", "1");
                }

                // Prepare attributes
                string attrs = "";
                if (!String.IsNullOrEmpty(ClientTypeClick))
                {
                    attrs = $"onclick=\"{ClientTypeClick}\"";
                }

                nameFormat = String.Format("<a class=\"ContentNewClass cms-icon-link\" href=\"{0}\" {2}>{1}</a>", url, nameFormat, attrs);
            }

            // Format items to output
            return String.Format(nameFormat, HTMLHelper.HTMLEncode(classDisplayName));
        }

        return HTMLHelper.HTMLEncode(parameter.ToString());
    }


    private void SetInformationMessage(string message)
    {
        ShowInformation(message);

        mainCaptionDocumentTypeSelection.Visible = false;
        headDocumentTypeSelection.Visible = false;
        pnlFooter.Visible = false;
        pnlVerticalSeparator.Visible = false;
    }

    #endregion
}
