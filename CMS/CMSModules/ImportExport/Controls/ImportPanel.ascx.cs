using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_ImportPanel : CMSUserControl
{
    #region "Variables"

    // Position of the tree scrollbar
    protected string mScrollPosition = "0";

    // Additional settings control
    protected ImportExportControl settingsControl = null;

    private SiteImportSettings mSettings;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return pnlMessages;
        }
    }


    /// <summary>
    /// Import settings.
    /// </summary>
    public SiteImportSettings Settings
    {
        get
        {
            return mSettings;
        }
        set
        {
            mSettings = value;
            gvObjects.Settings = value;
            //gvTasks.Settings = value;
        }
    }


    /// <summary>
    /// Selected  node value.
    /// </summary>
    public string SelectedNodeValue
    {
        get
        {
            return ValidationHelper.GetString(ViewState["SelectedNodeValue"], ObjectHelper.GROUP_OBJECTS);
        }
        set
        {
            ViewState["SelectedNodeValue"] = value;
        }
    }


    /// <summary>
    /// If true, node is site node.
    /// </summary>
    public bool SiteNode
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["SiteNode"], false);
        }
        set
        {
            ViewState["SiteNode"] = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            if (!RequestHelper.IsCallback())
            {
                objectTree.TreeView.SelectedNodeChanged += treeElem_SelectedNodeChanged;
                objectTree.OnBeforeCreateNode += treeElem_BeforeCreateNode;
                objectTree.TreeView.ExpandImageToolTip = GetString("contenttree.expand");
                objectTree.TreeView.CollapseImageToolTip = GetString("contenttree.collapse");
                objectTree.TreeView.NodeStyle.CssClass = "ContentTreeItem";
                objectTree.TreeView.SelectedNodeStyle.CssClass = "ContentTreeSelectedItem";

                objectTree.NodeTextTemplate = "<span class=\"Name\">##NODENAME##</span>";
                objectTree.SelectedNodeTextTemplate = "<span class=\"Name\">##NODENAME##</span>";
                objectTree.ValueTextTemplate = "##OBJECTTYPE##";

                objectTree.PreselectObjectType = SelectedNodeValue;
                objectTree.IsPreselectedObjectTypeSiteObject = SiteNode;

                if (Settings != null)
                {
                    if (SelectedNodeValue != CMS.DocumentEngine.TreeNode.OBJECT_TYPE)
                    {
                        // Bind grid view
                        gvObjects.Visible = true;
                        gvObjects.ObjectType = SelectedNodeValue;
                        gvObjects.Settings = Settings;
                        gvObjects.SiteObject = SiteNode;

                        gvObjects.ButtonPressed += gridView_ActionPerformed;
                        gvObjects.OnPageChanged += gridView_ActionPerformed;

                        gvTasks.Visible = true;
                        gvTasks.ObjectType = SelectedNodeValue;
                        gvTasks.Settings = Settings;
                        gvTasks.SiteObject = SiteNode;

                        gvTasks.ButtonPressed += gridView_ActionPerformed;
                        gvTasks.OnPageChanged += gridView_ActionPerformed;
                    }
                    else
                    {
                        gvObjects.Visible = false;
                        gvTasks.Visible = false;
                    }
                }

                // Reload data
                ReloadData(false); 
            }

            // Load settings control
            LoadSettingsControl();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsCallback())
        {
            if (Settings != null)
            {
                if (SelectedNodeValue != CMS.DocumentEngine.TreeNode.OBJECT_TYPE)
                {
                    DataSet ds = ImportProvider.LoadObjects(Settings, SelectedNodeValue, SiteNode, true);

                    // Bind grid view
                    gvObjects.Visible = true;
                    gvObjects.ObjectType = SelectedNodeValue;
                    gvObjects.Settings = Settings;
                    gvObjects.SiteObject = SiteNode;

                    gvObjects.DataSource = ds;
                    gvObjects.Bind();

                    gvTasks.Visible = true;
                    gvTasks.ObjectType = SelectedNodeValue;
                    gvTasks.Settings = Settings;
                    gvTasks.SiteObject = SiteNode;

                    gvTasks.DataSource = ds;
                    gvTasks.Bind();
                }
                else
                {
                    gvObjects.Visible = false;
                    gvTasks.Visible = false;
                }

                // Reload settings control
                if (settingsControl != null)
                {
                    settingsControl.ReloadData();
                }
            }

            // Save scrollbar position
            mScrollPosition = ValidationHelper.GetString(Page.Request.Params["hdnDivScrollBar"], "0");
        }
    }

    #endregion


    #region "Methods"

    private void LoadSettingsControl()
    {
        try
        {
            if (Settings != null)
            {
                plcControl.Controls.Clear();
                plcControl.Visible = false;
                settingsControl = null;

                if (!string.IsNullOrEmpty(SelectedNodeValue))
                {
                    string virtualPath = SiteNode ? ImportSettingsControlsRegister.GetSiteSettingsControl(SelectedNodeValue) : ImportSettingsControlsRegister.GetSettingsControl(SelectedNodeValue);

                    if (virtualPath != null)
                    {
                        // Load control
                        settingsControl = (ImportExportControl)Page.LoadUserControl(virtualPath);
                        settingsControl.EnableViewState = true;
                        settingsControl.ID = "settingControl";
                        settingsControl.Settings = Settings;

                        if (settingsControl.Visible)
                        {
                            plcControl.Controls.Add(settingsControl);
                            plcControl.Visible = true;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowError("[ImportPanel.LoadSettingsControl]: Error loading settings control for object type '" + SelectedNodeValue + "'. " + EventLogProvider.GetExceptionLogMessage(ex));
        }
    }


    public void ReloadData(bool forceReload)
    {
        if (Settings != null)
        {
            // Genearate items of the tree
            GenerateTreeItems(forceReload);

            if (!objectTree.ContainsObjectType(SelectedNodeValue))
            {
                SelectedNodeValue = "##OBJECTS##";
            }

            if (forceReload)
            {
                gvObjects.Settings = Settings;
                gvObjects.Bind();
                LoadSettingsControl();
            }
        }
    }


    public bool ApplySettings()
    {
        // Save last selection
        SaveSelection();

        // Check if any object is selected
        if (Settings.IsEmptySelection())
        {
            ShowError(GetString("ImportPanel.NoObjectSelected"));
            return false;
        }

        return true;
    }


    private void SaveSelection()
    {
        // Save additional settings
        if (settingsControl != null)
        {
            settingsControl.SaveSettings();
        }
    }


    /// <summary>
    /// Handles pagination and button click events in grid views.
    /// </summary>
    protected void gridView_ActionPerformed(object sender, EventArgs e)
    {
        SaveSelection();
    }
    

    // Handle node selection
    protected void treeElem_SelectedNodeChanged(object sender, EventArgs e)
    {
        // Save selected objects
        SaveSelection();

        SelectedNodeValue = objectTree.TreeView.SelectedValue;
        SiteNode = IsSiteNode(objectTree.TreeView.SelectedNode);

        // Load settings control
        LoadSettingsControl();

        // Reset the pagers
        gvObjects.PagerControl.Reset();
        gvTasks.PagerControl.Reset();
    }


    /// <summary>
    /// Checks if the node can be created.
    /// </summary>
    /// <param name="node">Tree node</param>
    protected bool treeElem_BeforeCreateNode(ObjectTypeTreeNode node)
    {
        string objectType = node.ObjectType;
        if (string.IsNullOrEmpty(objectType))
        {
            return true;
        }

        return Settings.IsIncluded(objectType, node.Site);
    }


    /// <summary>
    /// Returns true if the node is site node.
    /// </summary>
    /// <param name="node">Node</param>
    public bool IsSiteNode(TreeNode node)
    {
        if ((node == null) || (node.Parent == null))
        {
            return false;
        }
        else if (node.Value == "##SITE##")
        {
            return true;
        }
        else
        {
            return IsSiteNode(node.Parent);
        }
    }


    /// <summary>
    /// Genearate items of the tree.
    /// </summary>
    /// <param name="forceReload">If true, tree is forced to reload</param>
    private void GenerateTreeItems(bool forceReload)
    {
        if ((Settings == null) || ((objectTree.TreeView.Nodes.Count > 0) && !forceReload))
        {
            return;
        }

        // Display site objects only if site id is set
        if (Settings.TemporaryFilesCreated)
        {
            int siteId = 0;
            if (Settings.SiteIsIncluded)
            {
                siteId = 1;
            }

            objectTree.RootNode = ImportExportHelper.ImportObjectTree;
            objectTree.SiteID = siteId;
            objectTree.ReloadData();
        }
    }


    /// <summary>
    /// Returns true if any of the types is included in the package.
    /// </summary>
    public bool IsAnyIncluded(string objectTypes, bool siteObjects)
    {
        var types = TypeHelper.GetTypes(objectTypes);
        foreach (string type in types)
        {
            if (Settings.IsIncluded(type, siteObjects))
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}