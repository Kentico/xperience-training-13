using System;
using CMS.DocumentEngine;
using CMS.Helpers;
using System.Text;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.TranslationServices;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_NewCultureVersion : CMSUserControl
{
    #region "Variables"

    private TreeNode mNode;
    private TreeProvider mTree;

    #endregion


    #region "Properties"


    /// <summary>
    /// Gets and sets the target culture code.
    /// </summary>
    public string RequiredCulture
    {
        get;
        set;
    }


    /// <summary>
    /// Gets a value that indicates if the page is loaded inside a split view.
    /// </summary>
    public bool IsInCompare
    {
        get
        {
            return QueryHelper.GetBoolean("compare", false);
        }
    }

    /// <summary>
    /// Indicates whether the page is displayed as dialog.
    /// </summary>
    public bool RequiresDialog
    {
        get;
        set;
    }


    /// <summary>
    /// Tree provider object.
    /// </summary>
    public TreeProvider Tree
    {
        get
        {
            return mTree ?? (mTree = new TreeProvider());
        }
        set
        {
            mTree = value;
        }
    }


    /// <summary>
    /// Gets and sets node ID of current document.
    /// </summary>
    public int NodeID
    {
        get;
        set;
    }


    /// <summary>
    /// Node instance
    /// </summary>
    private TreeNode Node
    {
        get
        {
            return mNode ?? (mNode = Tree.SelectSingleNode(NodeID, TreeProvider.ALL_CULTURES));
        }
    }


    /// <summary>
    /// Mode query parameter value.
    /// </summary>
    public string Mode
    {
        get;
        set;
    }

    #endregion


    #region "Life cycle"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        radCopy.Text = GetString("ContentNewCultureVersion.Copy");
        radEmpty.Text = GetString("ContentNewCultureVersion.Empty");
        radTranslate.Text = GetString("ContentNewCultureVersion.Translate");

        radCopy.Attributes.Add("onclick", "ShowSelection();");
        radEmpty.Attributes.Add("onclick", "ShowSelection()");
        radTranslate.Attributes.Add("onclick", "ShowSelection()");

        btnCreateDocument.Text = GetString("ContentNewCultureVersion.Create");
        btnTranslate.Text = GetString("ContentNewCultureVersion.TranslateButton");
        btnTranslate.Click += btnTranslate_Click;
        btnCreateDocument.Click += btnCreateDocument_Click;

        if ((NodeID <= 0) || (Node == null))
        {
            return;
        }

        // Fill in the existing culture versions
        bool translationAllowed = SettingsKeyInfoProvider.GetBoolValue(Node.NodeSiteName + ".CMSEnableTranslations") && LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.TranslationServices, ModuleName.TRANSLATIONSERVICES);
        if (translationAllowed)
        {
            var settings = new TranslationSettings();
            settings.TargetLanguages.Add(RequiredCulture);
            translationElem.TranslationSettings = settings;

            translationElem.NodeID = Node.NodeID;
        }
        else
        {
            translationElem.StopProcessing = true;
            plcTranslationServices.Visible = false;
        }

        if (IsAuthorizedToCreateNewDocument())
        {
            EnsureScripts();

            SiteInfo si = SiteInfo.Provider.Get(Node.NodeSiteID);
            if (si == null)
            {
                return;
            }

            TreeNode originalNode = Tree.GetOriginalNode(Node);
            copyCulturesElem.UniSelector.DisplayNameFormat = "{% CultureName %}{% if (CultureCode == \"" + CultureHelper.GetDefaultCultureCode(si.SiteName) + "\") { \" \" +\"" + GetString("general.defaultchoice") + "\" } %}";
            copyCulturesElem.AdditionalWhereCondition = "CultureCode IN (SELECT DocumentCulture FROM CMS_Document WHERE DocumentNodeID = " + originalNode.NodeID + ")";

            if (!MembershipContext.AuthenticatedUser.IsCultureAllowed(RequiredCulture, si.SiteName))
            {
                pnlNewVersion.Visible = false;
                headNewCultureVersion.Visible = false;
                ShowError(GetString("transman.notallowedcreate"));
            }
        }
        else
        {
            pnlNewVersion.Visible = false;
            headNewCultureVersion.Visible = false;
            ShowError(GetString("accessdenied.notallowedtocreatenewcultureversion"));
        }
    }


    private bool IsAuthorizedToCreateNewDocument()
    {
        return MembershipContext.AuthenticatedUser.IsAuthorizedToCreateNewDocument(Node.NodeParentID, Node.NodeClassName);
    }


    /// <summary>
    /// Checks if translation service is available.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide translations if no service is available
        if (!translationElem.AnyServiceAvailable)
        {
            plcTranslationServices.Visible = false;
        }
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// Creates new culture version of object.
    /// </summary>
    protected void btnCreateDocument_Click(object sender, EventArgs e)
    {
        if (radCopy.Checked)
        {
            string sourceCulture = copyCulturesElem.Value.ToString();
            TreeNode actualSourceNode = DocumentHelper.GetDocument(NodeID, sourceCulture, Tree);
            TreeNode sourceNode = actualSourceNode.IsLink ? DocumentHelper.GetDocument(Tree.GetOriginalNode(actualSourceNode), Tree) : actualSourceNode;

            if (sourceNode != null)
            {
                if (chkSaveBeforeEditing.Checked && (Node != null))
                {
                    // Create the version first
                    TreeNode newCulture = TreeNode.New(Node.ClassName);

                    // The 'DocumentABTestConfiguration' is excluded from copying node to another culture as A/B test is linked to the culture specific node
                    var excludedColumns = new[] { "DocumentABTestConfiguration" };

                    DocumentHelper.CopyNodeData(sourceNode, newCulture, new CopyNodeDataSettings(true, excludedColumns) { ResetChanges = true});

                    var settings = new NewCultureDocumentSettings(newCulture, RequiredCulture, Tree)
                    {
                        CopyAttachments = true,
                        CopyCategories = true,
                        ClearAttachmentFields = false
                    };

                    try
                    {
                        DocumentHelper.InsertNewCultureVersion(settings);
                    }
                    catch (Exception ex)
                    {
                        // Catch possible exceptions
                        LogAndShowError("Content", "NEWCULTUREVERSION", ex);
                        return;
                    }

                    // Make sure document is published when versioning without workflow is applied
                    var workflow = newCulture.GetWorkflow();
                    if ((workflow != null) && workflow.WorkflowAutoPublishChanges && !workflow.UseCheckInCheckOut(newCulture.NodeSiteName))
                    {
                        newCulture.MoveToPublishedStep();
                    }

                    // Refresh page
                    if (RequiresDialog)
                    {
                        string url = UrlResolver.ResolveUrl(DocumentURLProvider.GetUrl(newCulture) + "?" + URLHelper.LanguageParameterName + "=" + RequiredCulture);
                        ScriptHelper.RegisterStartupScript(this, typeof(string), "NewCultureRefreshAction", ScriptHelper.GetScript(" wopener.location = " + ScriptHelper.GetString(url) + "; CloseDialog();"));
                    }
                    else
                    {
                        ScriptHelper.RegisterStartupScript(this, typeof(string), "NewCultureRefreshAction", ScriptHelper.GetScript("if (FramesRefresh) { FramesRefresh(" + Node.NodeID + "); }"));
                    }
                }
                else
                {
                    var url = GetEditUrl(Node);
                    url = URLHelper.AddParameterToUrl(url, "sourcedocumentid", sourceNode.DocumentID.ToString());

                    if (RequiresDialog)
                    {
                        // Reload new page after save
                        url = URLHelper.AddParameterToUrl(url, "reloadnewpage", "true");
                    }

                    // Provide information about actual node
                    if (actualSourceNode.IsLink)
                    {
                        url = URLHelper.AddParameterToUrl(url, "sourcenodeid", actualSourceNode.NodeID.ToString());
                    }
                    URLHelper.ResponseRedirect(url);
                }
            }
            else
            {
                ShowError(GetString("transman.notallowedcreate"));
            }
        }
        else
        {
            var url = GetEditUrl(Node);

            if (RequiresDialog)
            {
                // Reload new page after save
                url = URLHelper.AddParameterToUrl(url, "reloadnewpage", "true");
            }

            URLHelper.ResponseRedirect(url);
        }
    }


    /// <summary>
    /// Translates object to new culture.
    /// </summary>
    protected void btnTranslate_Click(object sender, EventArgs e)
    {
        if (TranslationServiceHelper.IsAuthorizedToTranslateDocument(Node, MembershipContext.AuthenticatedUser))
        {
            try
            {
                // Submits the document to translation service
                string err = translationElem.SubmitToTranslation();
                if (string.IsNullOrEmpty(err))
                {
                    // Refresh page
                    string script;
                    if (RequiresDialog)
                    {
                        string url = UrlResolver.ResolveUrl(DocumentURLProvider.GetUrl(Node) + "?" + URLHelper.LanguageParameterName + "=" + RequiredCulture);
                        script = "window.top.location = " + ScriptHelper.GetString(url) + ";";
                    }
                    else
                    {
                        script = "if (FramesRefresh) { FramesRefresh(" + Node.NodeID + "); }";
                    }

                    ScriptHelper.RegisterStartupScript(this, typeof(string), "NewCultureRefreshAction", ScriptHelper.GetScript(script));
                }
                else
                {
                    ShowError(err);
                }
            }
            catch (Exception ex)
            {
                ShowError(GetString("ContentRequest.TranslationFailed"), ex.Message);
                TranslationServiceHelper.LogEvent(ex);
            }
        }
        else
        {
            RedirectToAccessDenied("CMS.Content", "SubmitForTranslation");
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates editing url based on object type (document, product, product section).
    /// </summary>
    /// <param name="currentNode">Edited node.</param>
    private string GetEditUrl(TreeNode currentNode)
    {
        string url;
        if (currentNode.HasSKU && ModuleEntryManager.IsModuleLoaded(ModuleName.ECOMMERCE))
        {
            url = "~/CMSModules/Ecommerce/Pages/Content/Product/Product_Edit_General.aspx";
        }
        else
        {
            url = "~/CMSModules/Content/CMSDesk/Edit/Edit.aspx";
        }

        url = URLHelper.AddParameterToUrl(url, "nodeid", NodeID.ToString());
        url = URLHelper.AddParameterToUrl(url, "action", "newculture");
        url = URLHelper.AddParameterToUrl(url, "mode", Mode);
        url = URLHelper.AddParameterToUrl(url, "parentculture", RequiredCulture);
        url = URLHelper.AddParameterToUrl(url, "culture", RequiredCulture);
        url = URLHelper.AddParameterToUrl(url, "parentnodeid", currentNode.NodeParentID.ToString());

        if (IsInCompare)
        {
            url = URLHelper.AddParameterToUrl(url, "compare", "1");
        }

        if (RequiresDialog)
        {
            url = URLHelper.AddParameterToUrl(url, "dialog", "1");
        }

        return url;
    }


    private void EnsureScripts()
    {
        var script = new StringBuilder();
        script.Append(
            @"
function ShowSelection() {
    var radCopyElem = document.getElementById('", radCopy.ClientID, @"');
    var divCulturesElem = document.getElementById('divCultures');
    divCulturesElem.style.display = radCopyElem.checked ? ""block"" : ""none"";

    var divTranslationsElem = document.getElementById('divTranslations');
    var divTranslateElem = document.getElementById('divTranslate');
    if ((divTranslationsElem != null) && (divTranslateElem != null)) {
        var useTranslations = document.getElementById('", radTranslate.ClientID, @"').checked;
        var divCreateElem = document.getElementById('divCreate');

        divTranslationsElem.style.display = useTranslations ? 'block' : 'none';
        divTranslateElem.style.display = useTranslations ? '' : 'none';
        divCreateElem.style.display = useTranslations ? 'none' : '';
    }
}

function FramesRefresh(selectNodeId) {
    parent.RefreshTree(selectNodeId, selectNodeId);
    parent.SelectNode(selectNodeId);
}
");

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "ShowSelection", script.ToString(), true);
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ShowSelectionStartup", "ShowSelection();", true);
    }

    #endregion
}
