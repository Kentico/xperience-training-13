using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;

public partial class CMSModules_Content_Controls_WebPartToolbar : CMSAbstractPortalUserControl
{
    #region "Variables"

    /// <summary>
    /// CategorySelector - CMSDropDownList codes
    /// </summary>
    private const string CATEGORY_RECENTLY_USED = "##recentlyused##";
    private const string CATEGORY_UIWEBPARTS = "UIWebparts";
    private const string SPACE_REPLACEMENT = "__SPACE__";
    private const string CATEGORY_CHANGED_CODE = "wptcategorychanged";
    private const string SEARCH_TEXT_CODE = "wptsearchtext";

    private const int DEFAULT_WEBPART_COUNT = 30;

    private bool dataLoaded = false;
    private string mSelectedCategory = null;
    private CurrentUserInfo currentUser = null;
    protected bool uiCultureRTL = CultureHelper.IsUICultureRTL();
    private string imageHTML = null;

    protected string prefferedUICultureCode = string.Empty;

    protected string COOKIE_SELECTED_CATEGORY = CookieName.WebPartToolbarCategory;
    protected bool limitItems = true;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets or sets the selected web part category.
    /// </summary>
    private string SelectedCategory
    {
        get
        {
            if (mSelectedCategory == null)
            {
                // Get the selected category from the cookie
                mSelectedCategory = CookieHelper.GetValue(COOKIE_SELECTED_CATEGORY);

                if (String.IsNullOrEmpty(mSelectedCategory))
                {
                    string rootPath = categorySelector.RootPath;
                    if (String.IsNullOrEmpty(rootPath))
                    {
                        rootPath = "/";
                    }

                    WebPartCategoryInfo wpci = WebPartCategoryInfoProvider.GetWebPartCategoryInfoByCodeName(rootPath);
                    mSelectedCategory = (wpci != null) ? wpci.CategoryID.ToString() : CATEGORY_RECENTLY_USED;
                }
                else if (IsUITemplate() && (mSelectedCategory == CATEGORY_RECENTLY_USED))
                {
                    // Reset selected category if template type is UI Template and originally selected category was Recently used web parts
                    WebPartCategoryInfo wpci = WebPartCategoryInfoProvider.GetWebPartCategoryInfoByCodeName(CATEGORY_UIWEBPARTS);
                    if (wpci != null)
                    {
                        mSelectedCategory = wpci.CategoryID.ToString();
                    }
                }
            }

            return mSelectedCategory;
        }
        set
        {
            CookieHelper.SetValue(COOKIE_SELECTED_CATEGORY, value, "/", DateTime.Now.AddDays(30));
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        currentUser = MembershipContext.AuthenticatedUser;
        prefferedUICultureCode = currentUser.PreferredUICultureCode;

        var categoryWhere = new WhereCondition();

        if (IsUITemplate())
        {
            categorySelector.RootPath = CATEGORY_UIWEBPARTS;
            COOKIE_SELECTED_CATEGORY = CookieName.WebPartToolbarCategoryUI;
        }
        else
        {
            categoryWhere.WhereNotEquals("ObjectPath", CATEGORY_UIWEBPARTS).WhereNotStartsWith("ObjectPath", CATEGORY_UIWEBPARTS + "/");
        }

        // Display only top level categories
        categoryWhere.WhereLessThan("ObjectLevel", 2);
        categorySelector.WhereCondition = categoryWhere.ToString(true);

        base.OnInit(e);
    }


    /// <summary>
    /// Determines whether the current page template is UI template.
    /// </summary>
    private static bool IsUITemplate()
    {
        PageTemplateInfo pti = DocumentContext.CurrentPageInfo.DesignPageTemplateInfo;
        if (pti != null)
        {
            return (pti.PageTemplateType == PageTemplateTypeEnum.UI);
        }

        return false;
    }


    /// <summary>
    /// Load event handler.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Enable loading the web parts
        repItems.StopProcessing = false;

        // Setups the web part toolbar control
        SetupControl();

        if (RequestHelper.IsPostBack())
        {
            // Add the search text to the current url
            RequestContext.CurrentURL = URLHelper.AddParameterToUrl(RequestContext.CurrentURL, SEARCH_TEXT_CODE, HttpUtility.UrlEncode(txtSearch.Text));
        }

        // Handle & set hidden reload button
        btnLoadMore.Click += btnLoadMore_Click;
    }


    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (QueryHelper.Contains(SEARCH_TEXT_CODE))
        {
            // Get the search text from the current url
            txtSearch.Text = HttpUtility.UrlDecode(URLHelper.GetUrlParameter(RequestContext.CurrentURL, SEARCH_TEXT_CODE).Replace(SPACE_REPLACEMENT, " "));

            // Hide the scrollPanel when the search filter will be applied (skip this when changing displayed category)
            if (Request.Form[Page.postEventArgumentID] != CATEGORY_CHANGED_CODE)
            {
                scrollPanel.Style.Add("display", "none");
            }
        }

        // Add the javascript file and scripts into the page
        ScriptHelper.RegisterJQueryCookie(Page);
        ScriptHelper.RegisterScriptFile(Page, "DesignMode/WebPartToolbar.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/jquery/jquery-url.js");
        ScriptHelper.RegisterStartupScript(this, typeof(string), "wptPageScripts", ScriptHelper.GetScript(GetPageScripts() + @"
wptSetupSearch();
var WPTImgBaseSrc = '""" + ResolveUrl("~/CMSPages/GetMetaFile.aspx?maxsidesize=64&fileguid=") + @"';
"));

        // Adjust the page to accommodate the web part toolbar
        scrollPanel.IsRTL = uiCultureRTL;

        // Set the selected category
        if (!RequestHelper.IsPostBack())
        {
            categorySelector.DropDownListControl.SelectedValue = SelectedCategory;
        }
        else
        {
            SelectedCategory = categorySelector.DropDownListControl.SelectedValue;

            // Refresh scroll panel
            ScriptHelper.RegisterClientScriptBlock(pnlU, typeof(string), "wptRefreshScrollPanel", "$cmsj(document).ready(function () { wptInit(true); wptReloadScrollPanel(true); wptLoadWebpartImages(); });", true);
        }

        // Load the web part items according to the selected category and the search text condition
        LoadWebParts(false);

        // Register script for web part lazy load
        if (limitItems)
        {
            const string appearScript = @"
            $cmsj(document).ready(
            function () {
                $cmsj('.AppearElement').appear(
                    function () {
                        $cmsj(this).html(wpLoadingMoreString); 
                        DoToolbarPostBack(); 
                     });
            });";

            string postBackScript = @"
            function DoToolbarPostBack(){
                " + ControlsHelper.GetPostBackEventReference(btnLoadMore, "") + @"
            };";

            ScriptHelper.RegisterStartupScript(Page, typeof(string), "DoToolbarPostback", ScriptHelper.GetScript(appearScript + postBackScript));
        }

        bool isMinimized = ValidationHelper.GetBoolean(CookieHelper.GetValue(CookieName.WebPartToolbarMinimized), false);
        if (isMinimized)
        {
            // Renew the cookie
            CookieHelper.SetValue(CookieName.WebPartToolbarMinimized, "true", "/", DateTime.Now.AddDays(31), false);

            // Hide the expanded web part toolbar
            pnlMaximized.Style.Add("display", "none");
        }
        else
        {
            // Register the OnLoad javascript for the expanded bar
            ltrScript.Text = ScriptHelper.GetScript("function pageLoad(sender, args) { wptInit(true); }");

            // Hide the minimized toolbar
            pnlMinimized.Style.Add("display", "none");
        }

        ScriptHelper.RegisterJQueryAppear(Page);
    }


    /// <summary>
    /// Handles the ItemDataBound event of the repItems control.
    /// </summary>
    protected void repItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            object dataItem = e.Item.DataItem;
            if (dataItem.GetType() == typeof(DataRowView))
            {
                // Get data row
                DataRow dr = ((DataRowView)dataItem).Row;
                if (dr["ObjectType"].ToString() == "webpartcategory")
                {
                    string currentCategory = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Convert.ToString(dr["DisplayName"]), prefferedUICultureCode));
                    int currentLevel = ValidationHelper.GetInteger(dr["ObjectLevel"], -1);

                    // Ignore item template defined in markup
                    e.Item.Controls.Clear();

                    // Render only top level categories
                    if (currentLevel == 1)
                    {
                        Literal ltl = new Literal();
                        ltl.Text = @"<div class=""WPTCat""><h4>" + currentCategory + "</h4></div>";
                        e.Item.Controls.Add(ltl);
                    }
                }
                else
                {
                    // Setup the web part item
                    Panel wptFlatItem = ((Panel)e.Item.FindControl("i"));
                    if (wptFlatItem != null)
                    {
                        string toolTip = ResHelper.LocalizeString(Convert.ToString(dr["WebPartDescription"]), prefferedUICultureCode);
                        toolTip = String.IsNullOrEmpty(toolTip) ? GetString("webparttoolbar.nodescription") : toolTip;
                        wptFlatItem.ToolTip = "<div class='WPTTH'>" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Convert.ToString(dr["DisplayName"]), prefferedUICultureCode)) + "</div><div class='WPTTC'>" + HTMLHelper.HTMLEncode(toolTip) + "</div>";

                        // Set the web part id
                        wptFlatItem.Attributes.Add("data-webpartid", Convert.ToString(dr["ObjectID"]));

                        // Ensure that when start dragging then a copy of the original web part item will be created
                        wptFlatItem.Attributes.Add("data-dragkeepcopy", "1");
                        wptFlatItem.Attributes.Add("onmouseover", "wptToggle(this, true);");
                        wptFlatItem.Attributes.Add("onmouseout", "wptToggle(this, false);");

                        // Skip the insert properties dialog when the web part allows this behavior
                        if (ValidationHelper.GetBoolean(dr["WebPartSkipInsertProperties"], false))
                        {
                            wptFlatItem.Attributes.Add("data-skipdialog", "1");
                        }
                    }

                    // Insert parent category names into HTML comment to be able filter web parts by category name
                    Literal ltlComm = ((Literal)e.Item.FindControl("ltlCategorytComment"));
                    if (ltlComm != null)
                    {
                        ltlComm.Text = "<!-- " + HTMLHelper.EncodeForHtmlComment(GetParentCategories(dr)) + " -->";
                    }

                    // Build the web part image html
                    Literal ltrImage = ((Literal)e.Item.FindControl("ltrImage"));
                    if (ltrImage != null)
                    {
                        imageHTML = PortalHelper.GetIconHtml(
                            thumbnailGuid: ValidationHelper.GetGuid(dr["ThumbnailGUID"], Guid.Empty),
                            iconClass: ValidationHelper.GetString(dr["IconClass"], PortalHelper.DefaultWebPartIconClass));

                        ltrImage.Text = imageHTML;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Reloads the web part list.
    /// </summary>
    /// <param name="forceLoad">if set to <c>true</c>, reload the control even if the control has been already loaded</param>
    protected void LoadWebParts(bool forceLoad)
    {
        if (!dataLoaded || forceLoad)
        {
            var repeaterWhere = new WhereCondition();

            /* The order is category driven => first level category display name is used for all nodes incl. sub-nodes */
            string categoryOrder = @"
(SELECT CMS_WebPartCategory.CategoryDisplayName FROM CMS_WebPartCategory 
WHERE CMS_WebPartCategory.CategoryPath = (CASE WHEN (CHARINDEX('/', ObjectPath, 0) > 0) AND (CHARINDEX('/', ObjectPath, CHARINDEX('/', ObjectPath, 0) + 1) = 0) 
THEN ObjectPath 
ELSE SUBSTRING(ObjectPath, 0, LEN(ObjectPath) - (LEN(ObjectPath) - CHARINDEX('/', ObjectPath, CHARINDEX('/', ObjectPath, 0) + 1)))
END))
";

            // Set query repeater
            repItems.SelectedColumns = " ObjectID, DisplayName, ObjectType, ParentID, ThumbnailGUID, IconClass, ObjectLevel, WebPartDescription, WebPartSkipInsertProperties";
            repItems.OrderBy = categoryOrder + ", ObjectType  DESC, DisplayName";

            // Setup the where condition
            if (SelectedCategory == CATEGORY_RECENTLY_USED)
            {
                // Recently used category
                RenderRecentlyUsedWebParts(true);
            }
            else
            {
                // Specific web part category
                int selectedCategoryId = ValidationHelper.GetInteger(SelectedCategory, 0);
                if (selectedCategoryId > 0)
                {
                    WebPartCategoryInfo categoryInfo = WebPartCategoryInfoProvider.GetWebPartCategoryInfoById(selectedCategoryId);
                    if (categoryInfo != null)
                    {
                        string firstLevelCategoryPath = String.Empty;

                        // Select also all subcategories (using "/%")
                        string categoryPath = categoryInfo.CategoryPath;
                        if (!categoryPath.EndsWith("/", StringComparison.Ordinal))
                        {
                            categoryPath += "/";
                        }

                        // Do not limit items if not root category is selected
                        if (!categoryInfo.CategoryPath.EqualsCSafe("/"))
                        {
                            limitItems = false;
                        }

                        // Get all web parts for the selected category and its subcategories
                        if (categoryPath.EqualsCSafe("/"))
                        {
                            repeaterWhere.Where(repItems.WhereCondition).Where(w => w
                                .WhereEquals("ObjectType", "webpart")
                                .Or()
                                .WhereEquals("ObjectLevel", 1)
                            ).Where(w => w
                                .WhereEquals("ParentID", selectedCategoryId)
                                .Or()
                                .WhereIn("ParentID", WebPartCategoryInfoProvider.GetCategories().WhereStartsWith("CategoryPath", categoryPath))
                            );

                            // Set caching for query repeater
                            repItems.ForceCacheMinutes = true;
                            repItems.CacheMinutes = 24 * 60;
                            repItems.CacheDependencies = "cms.webpart|all\ncms.webpartcategory|all";

                            // Show Recently used category
                            RenderRecentlyUsedWebParts(false);
                        }
                        else
                        {
                            // Prepare where condition -- the part that restricts web parts
                            repeaterWhere.WhereEquals("ObjectType", "webpart")
                                .Where(w => w
                                    .WhereEquals("ParentID", selectedCategoryId)
                                    .Or()
                                    .WhereIn("ParentID", WebPartCategoryInfoProvider.GetCategories().WhereStartsWith("CategoryPath", categoryPath))
                                );

                            // Get first level category path
                            firstLevelCategoryPath = categoryPath.Substring(0, categoryPath.IndexOf('/', 2));

                            var selectedCategoryWhere = new WhereCondition();

                            // Distinguish special categories
                            if (categoryPath.StartsWithCSafe(CATEGORY_UIWEBPARTS, true))
                            {
                                if (!categoryPath.EqualsCSafe(firstLevelCategoryPath + "/", true))
                                {
                                    // Currently selected category is one of subcategories
                                    string specialCategoryPath = firstLevelCategoryPath;
                                    firstLevelCategoryPath = categoryPath.Substring(CATEGORY_UIWEBPARTS.Length + 1).TrimEnd('/');
                                    selectedCategoryWhere.WhereEquals("ObjectPath", specialCategoryPath + "/" + firstLevelCategoryPath);
                                }
                                else
                                {
                                    // Currently selected category is root category
                                    selectedCategoryWhere.WhereStartsWith("ObjectPath", firstLevelCategoryPath);
                                }
                            }
                            else
                            {
                                // All web part category
                                selectedCategoryWhere.WhereEquals("ObjectPath", firstLevelCategoryPath);
                            }

                            repeaterWhere.Or().Where(w => w
                                .WhereEquals("ObjectType", "webpartcategory")
                                .WhereEquals("ObjectLevel", 1)
                                .Where(selectedCategoryWhere)
                            );

                            // Set caching for query repeater
                            repItems.CacheMinutes = 0;
                            repItems.ForceCacheMinutes = true;
                        }
                    }
                }

                repItems.WhereCondition = repeaterWhere.ToString(true);

                // Limit items if required
                if (limitItems)
                {
                    repItems.SelectTopN = DEFAULT_WEBPART_COUNT;
                }

                repItems.ReloadData(false);
                repItems.DataBind();
            }

            dataLoaded = true;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    // Setups the web part toolbar control.
    /// </summary>
    private void SetupControl()
    {
        // Search text box
        txtSearch.Attributes.Add("type", "search");
        txtSearch.Attributes.Add("placeholder", GetString("webparttoolbar.search", prefferedUICultureCode));

        // Load the web part category selector
        categorySelector.ReloadData(false);

        // Add the "Recently used" item to the category selector in all cases except UI templates
        if (!IsUITemplate())
        {
            ListItem listItem = new ListItem();
            listItem.Text = ResHelper.GetString("webparts.recentlyusedshort", prefferedUICultureCode);
            listItem.Value = CATEGORY_RECENTLY_USED;
            categorySelector.DropDownListControl.Items.Insert(0, listItem);
        }

        categorySelector.DropDownListControl.CssClass = "WPTCategories form-control";

        // Ensure that when selected item is changed then the web part repeater update panel will be also updated
        categorySelector.DropDownListControl.Attributes.Add("onchange", "wptCategoryChanged();");

        // Setup the handlers
        repItems.ItemDataBound += new RepeaterItemEventHandler(repItems_ItemDataBound);

        // Setup the toolbar web part list drag-and-drop extender
        ddExtender.TargetControlID = scrollPanel.ScrollAreaContainer.ID;
        ddExtender.DropCueID = pnlCue.ID;
        ddExtender.DragItemClass = "WPTSelectorEnvelope";
        ddExtender.DragItemHandleClass = "WPTHandle";
        ddExtender.OnClientDrop = "";
        ddExtender.OnClientBeforeDrop = "wptListOnBeforeDrop";

        // Setup the drag-and-drop extender for highlighted web parts as well.
        // This solves the issue when changing categories (using the drop down) and you highlight a web part before the init script for drag-and-drop is run.
        ddExtenderHovered.TargetControlID = scrollPanel.ScrollAreaContainer.ID;
        ddExtenderHovered.DropCueID = pnlCue.ID;
        ddExtenderHovered.DragItemClass = "WPTSelectorEnvelopeHover";
        ddExtenderHovered.DragItemHandleClass = "WPTHandle";
        ddExtenderHovered.OnClientDrop = "";
        ddExtenderHovered.OnClientBeforeDrop = "wptListOnBeforeDrop";

        // Hide the compulsory drag-and-drop panel Cue, it is not being used
        pnlCue.Style.Add("display", "none");
        pnlCue.Style.Add("position", "absolute");

        pnlLoader.Controls.Add(new LiteralControl(ScriptHelper.GetLoaderInlineHtml()));
    }


    /// <summary>
    /// Gets the page javascripts.
    /// </summary>
    private string GetPageScripts()
    {
        // Generate toolbar scripts
        StringBuilder sb = new StringBuilder();
        sb.Append(
@"var wptIsMinimizedCookie = '", CookieName.WebPartToolbarMinimized, @"';
var wptIsRTL = ", (uiCultureRTL ? "true" : "false"), @";

var wpLoadingMoreString = " + ScriptHelper.GetLocalizedString("general.loading") + @";

function wptSetupSearch()
{
    $cmsj('#", txtSearch.ClientID, @"')
        .keypress(function (e) {
            window.clearTimeout(wptFilterWebPartsTimer);
            return wptProceedSpecialKeys(this, e);
        })
        .keyup(function (e) {
            var ret = wptProceedSpecialKeys(this, e);
            wptFilterWebParts(this);
            return ret;
        });
}

function wptFilterWebParts(txtBoxElem) {
    window.clearTimeout(wptFilterWebPartsTimer);
    wptFilterWebPartsTimer = window.setTimeout(function () {
        var searchText = txtBoxElem.value.toLowerCase();
        UpdateRefreshPageUrlParam('", SEARCH_TEXT_CODE, @"', searchText.replace(/ /g,'", SPACE_REPLACEMENT, @"'));
        wptFilter(searchText);
        wptInit(true);
        wptReloadScrollPanel(true);
    }, 200);
}

function wptFilterAfterPostBack(){
    wptFilter(document.getElementById('", txtSearch.ClientID, @"').value);
    wptReloadScrollPanel(true);
}

function wptReloadScrollPanel(forceReload) {
    scrollPanelInit('", scrollPanel.ClientID, @"', forceReload);
}

function wptCategoryChanged() {
    $cmsj('#" + txtSearch.ClientID + @"').val('');
    // Remove all tooltip temporary nodes
    $cmsj('.WPTTT').remove(); ",
    ControlsHelper.GetPostBackEventReference(hdnUpdater, CATEGORY_CHANGED_CODE), @";
}
");

        if (QueryHelper.Contains(SEARCH_TEXT_CODE))
        {
            // Filter the web parts if the search text is set
            sb.Append("$cmsj(document).ready(function () { wptFilterWebParts(document.getElementById('", txtSearch.ClientID, @"')); })");
        }

        return sb.ToString();
    }


    /// <summary>
    /// Load all items
    /// </summary>
    private void btnLoadMore_Click(object sender, EventArgs e)
    {
        limitItems = false;
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "WPTBLoadMoreFilter", ScriptHelper.GetScript("wptReload= true; $cmsj(document).ready(function() { spScrollSetPosition('" + scrollPanel.ClientID + "');  wptFilterAfterPostBack();  window.setTimeout(function(){ wptReload= false; }, 1000) });"));
    }


    /// <summary>
    /// Traverses over parent categories and returns all as string.
    /// </summary>
    /// <param name="dr">Data row containing the web part data</param>
    private string GetParentCategories(DataRow dr)
    {
        StringBuilder parents = new StringBuilder();

        // Get web part parent ID
        int parentId = ValidationHelper.GetInteger(dr["ParentID"], 0);

        // Traverse over categories
        while (parentId > 0)
        {
            WebPartCategoryInfo wpci = WebPartCategoryInfoProvider.GetWebPartCategoryInfoById(parentId);
            if ((wpci != null) && !wpci.CategoryPath.EqualsCSafe("/"))
            {
                parents.Insert(0, wpci.CategoryDisplayName + " ");
                parentId = wpci.CategoryParentID;
            }
            else
            {
                break;
            }
        }

        return parents.ToString();
    }


    /// <summary>
    /// Generates Recently used category to the toolbar.
    /// </summary>
    /// <param name="allowFiltering">If true the the Recently used category doesn't take a part in filtering</param>
    private void RenderRecentlyUsedWebParts(bool allowFiltering)
    {
        StringBuilder result = new StringBuilder();
        List<WebPartInfo> wpList = new List<WebPartInfo>();

        // Get recently used web parts from user settings
        string[] webParts = Array.Empty<string>();
        foreach (string webPartName in webParts)
        {
            WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(webPartName);
            if (wpi != null)
            {
                wpList.Add(wpi);
            }
        }

        // Sort web parts
        wpList = wpList.OrderBy(x => ResHelper.LocalizeString(x.WebPartDisplayName, prefferedUICultureCode)).ToList();

        // Create the category
        if (webParts.Length > 0)
        {
            string categoryName = HTMLHelper.HTMLEncode(ResHelper.GetString("webparts.recentlyusedshort", prefferedUICultureCode));
            result.Append(@"<div class=""WPTCat""><h4>");
            result.Append(categoryName);
            if (!allowFiltering)
            {
                result.Append(@"<!--__NOFILTER__-->");
            }
            result.Append(@"</h4></div>");
        }

        foreach (WebPartInfo wp in wpList)
        {
            // Selector envelope
            Panel pnlEnvelope = new Panel();
            pnlEnvelope.CssClass = "WPTSelectorEnvelope";
            pnlEnvelope.ToolTip = @"<div class=""WPTTH"">" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(wp.WebPartDisplayName, prefferedUICultureCode)) + @"</div><div class=""WPTTC"">" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(wp.WebPartDescription, prefferedUICultureCode)) + @"</div>";

            // Set the web part id
            pnlEnvelope.Attributes.Add("data-webpartid", Convert.ToString(wp.WebPartID));
            pnlEnvelope.ID = "wpt_env_" + wp.WebPartID;

            // Ensure that when start dragging then a copy of the original web part item will be created
            pnlEnvelope.Attributes.Add("data-dragkeepcopy", "1");
            pnlEnvelope.Attributes.Add("onmouseover", "wptToggle(this, true);");
            pnlEnvelope.Attributes.Add("onmouseout", "wptToggle(this, false);");

            // Skip the insert properties dialog when the web part allows this behavior
            if (wp.WebPartSkipInsertProperties)
            {
                pnlEnvelope.Attributes.Add("data-skipdialog", "1");
            }

            // Handle
            Panel pnlHandle = new Panel();
            pnlHandle.CssClass = "WPTHandle";
            pnlEnvelope.Controls.Add(pnlHandle);
            pnlHandle.ID = "wpt_handle_" + wp.WebPartID;

            // Thumbnail image
            Literal ltlImage = new Literal();
            imageHTML = PortalHelper.GetIconHtml(
                thumbnailGuid: wp.WebPartThumbnailGUID,
                iconClass: wp.WebPartIconClass ?? PortalHelper.DefaultWebPartIconClass);
            ltlImage.Text = imageHTML;

            pnlHandle.Controls.Add(ltlImage);

            // Item text
            Literal ltlItemTxt = new Literal();
            ltlItemTxt.Text = @"<div>" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(wp.WebPartDisplayName, prefferedUICultureCode)) + "</div>";
            pnlHandle.Controls.Add(ltlItemTxt);

            // Get rendered code of web part item
            HtmlTextWriter writer = new HtmlTextWriter(new StringWriter(result));
            pnlEnvelope.RenderControl(writer);
        }

        ltlRecentlyUsedWebParts.Text = result.ToString();
    }

    #endregion
}


