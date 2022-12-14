using System;
using System.Web;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement(ModuleName.CONTENT, "content")]
public partial class CMSModules_Content_CMSDesk_Default : CMSContentPage, ICallbackEventHandler
{
    #region "Variables & constants"

    private const string UI_LAYOUT_KEY = nameof(CMSModules_Content_CMSDesk_Default);


    private int? mResultNodeID;
    private string mResultMode;
    private readonly ContentUrlRetriever urlRetriever;

    #endregion


    #region "Private properties"

    private int ExpandNodeID
    {
        get
        {
            return QueryHelper.GetInteger("expandnodeid", 0);
        }
    }


    private int SelectedNodeID
    {
        get
        {
            return ValidationHelper.GetInteger(Request.Params["selectedNodeId"], 0);
        }
    }


    private string SelectedCulture
    {
        get
        {
            return ValidationHelper.GetString(Request.Params["selectedCulture"], LocalizationContext.PreferredCultureCode);
        }
    }


    private string SelectedMode
    {
        get
        {
            return ValidationHelper.GetString(Request.Params["selectedMode"], null);
        }
    }


    private TreeNode RootNode
    {
        get
        {
            // Root
            string baseDoc = "/";
            if (CurrentUser.UserStartingAliasPath != String.Empty)
            {
                // Change to user's root page
                baseDoc = CurrentUser.UserStartingAliasPath;
            }
            // Try to get culture-specific root node
            TreeNode rootNode = Tree.SelectSingleNode(SiteContext.CurrentSiteName, baseDoc, LocalizationContext.PreferredCultureCode, false, null, false);
            if (rootNode == null)
            {
                // Get root node
                rootNode = Tree.SelectSingleNode(SiteContext.CurrentSiteName, baseDoc, TreeProvider.ALL_CULTURES, false, null, false);
            }
            return rootNode;
        }
    }


    protected int ResultNodeID
    {
        get
        {
            if (mResultNodeID == null)
            {
                // Get ID from query string
                mResultNodeID = NodeID;
                if (mResultNodeID <= 0)
                {
                    // Get ID selected by user
                    mResultNodeID = SelectedNodeID;
                    // If no node specified, add the root node id
                    if ((mResultNodeID) <= 0 && (NodeID <= 0))
                    {
                        TreeNode rootNode = RootNode;
                        if (rootNode != null)
                        {
                            mResultNodeID = rootNode.NodeID;
                        }
                    }
                }
            }
            return mResultNodeID.Value;
        }
    }


    /// <summary>
    /// Resulting viewmode. Prefers user choice over query string setting.
    /// </summary>
    protected string ResultMode
    {
        get
        {
            return mResultMode ?? (mResultMode = SelectedMode ?? (Mode ?? "edit"));
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Constructor
    /// </summary>
    public CMSModules_Content_CMSDesk_Default()
    {
        urlRetriever = new ContentUrlRetriever(this, DocumentUIHelper.GetDocumentPageUrl);
    }


    protected override void OnPreInit(EventArgs e)
    {
        // Do not check document Read permission
        CheckDocPermissions = false;

        base.OnPreInit(e);

        // Do not include document manager to the controls collection
        EnsureDocumentManager = false;
        DocumentManager.RedirectForNonExistingDocument = false;
    }


    protected override void OnInit(EventArgs e)
    {
        layoutElem.OnResizeEndScript = ScriptHelper.GetLayoutResizeScript(contentcontrolpanel, this);
        layoutElem.MaxSize = "50%";

        EnsureScriptManager();

        if (!RequestHelper.IsPostBack() && !RequestHelper.IsCallback())
        {
            // Set the culture if specified in query string
            string culture = QueryHelper.GetString("culture", string.Empty);
            if (culture != string.Empty)
            {
                LocalizationContext.PreferredCultureCode = culture;
            }

            // Set the view mode if specified in query string
            PortalContext.UpdateViewMode(ViewModeEnum.Edit);

            var width = UILayoutHelper.GetLayoutWidth(UI_LAYOUT_KEY);
            if (width.HasValue)
            {
                contentcontrolpanel.Size = width.ToString();
            }
        }

        // Check (and ensure) the proper content culture
        CheckPreferredCulture();

        contentcontrolpanel.Values.AddRange(new[]
        {
            new UILayoutValue("NodeID", ResultNodeID),
            new UILayoutValue("ExpandNodeID", ExpandNodeID),
            new UILayoutValue("Culture", SelectedCulture),
            new UILayoutValue("SelectedMode", ResultMode)
        });

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register the scripts
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "defaultVariables", ScriptHelper.GetScript(@"var IsCMSDesk = true;"));
        ScriptHelper.RegisterScriptFile(Page, @"~/CMSModules/Content/CMSDesk/Content.js");

        var settings = new UIPageURLSettings
        {
            Mode = ResultMode,
            Action = Action,
            NodeID = ResultNodeID,
            Culture = LocalizationContext.PreferredCultureCode,
            IncludeLiveSiteURL = true
        };

        var urls = urlRetriever.GetRequestedUrl(settings).Split(new[] { ContentUrlRetriever.CALLBACK_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);

        // Ensure action URL
        contentview.Src = urls[0];

        if (urls.Length > 1)
        {
            // Set default live site URL in header link
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "SetDefaultLiveSiteURL", ScriptHelper.GetScript("SetLiveSiteURL('" + HttpUtility.JavaScriptStringEncode(urls[1]) + "');"));
        }
    }


    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
    {
        var parsed = eventArgument.Split(new[] { UILayoutHelper.DELIMITER });
        if (parsed.Length == 2 && String.Equals(UILayoutHelper.WIDTH_ARGUMENT, parsed[0], StringComparison.OrdinalIgnoreCase))
        {
            if (int.TryParse(parsed[1], out var width))
            {
                UILayoutHelper.SetLayoutWidth(UI_LAYOUT_KEY, width);
            }
        }
    }


    string ICallbackEventHandler.GetCallbackResult()
    {
        return null;
    }

    #endregion
}
