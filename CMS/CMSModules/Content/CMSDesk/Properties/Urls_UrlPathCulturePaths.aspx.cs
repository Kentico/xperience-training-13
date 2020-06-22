using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Routing;
using CMS.DocumentEngine.Routing.Internal;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Content_CMSDesk_Properties_Urls_UrlPathCulturePaths : CMSPropertiesPage
{
    private Dictionary<string, string> mSitePresentationUrls = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
    private IList<string> pageCultures;
    private int editedUrlPathId;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        ScriptHelper.RegisterModule(this, "CMS/Clamp", new
        {
            elementWithTextSelector = ".static-textpanel"
        });
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        SetHeaderActions();

        ScriptHelper.RegisterLoader(Page);
        ScriptHelper.RegisterEditScript(Page, false);

        SetGridWhereCondition();
        LoadExistingPageCultures();
        SetBreadcrumbs();
    }


    private void SetBreadcrumbs()
    {
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("content.ui.propertiesurls"),
            RedirectUrl = GetBackButtonUrl(),
            Target = "propedit"
        });

        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("content.ui.properties.pageurlpaths.breadcrumbs")
        });
    }


    private void SetGridWhereCondition()
    {
        gridUrlsPaths.WhereCondition = new WhereCondition()
                    .WhereEquals("PageUrlPathNodeID", NodeID)
                    .ToString(true);
    }


    private void SetHeaderActions()
    {
        menu.ActionsList.Add(new HeaderAction
        {
            ButtonStyle = ButtonStyle.Default,
            RedirectUrl = GetBackButtonUrl(),
            Text = GetString("general.back")
        });
    }


    private void LoadExistingPageCultures()
    {
        pageCultures = Node.GetTranslatedCultures();
    }


    private string GetBackButtonUrl()
    {
        return $"~/CMSModules/Content/CMSDesk/Properties/Urls.aspx?nodeid={NodeID}";
    }


    private bool CanEditPageUrlPath()
    {
        return MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) != AuthorizationResultEnum.Denied;
    }


    private static string GetUrlPathSlug(string urlPath)
    {
        return new PageUrlPath(urlPath).Slug;
    }


    private void ShowCollisionErrorMessage(IEnumerable<CollisionData> collisions)
    {
        foreach (var message in PageRoutingUIHelper.GetCollisionErrorMessages(collisions, Node.NodeSiteID))
        {
            AddError(message);
        }
    }


    private void UpdateUrlPathFromInline(InlineEditingTextBox inlineEditingTextBox, string urlPathCulture)
    {
        var newSlug = inlineEditingTextBox.Text;

        if (String.IsNullOrWhiteSpace(newSlug))
        {
            ShowError(GetString("general.requiresvalue"));
            return;
        }

        if (new PageUrlPathSlugUpdater(Node, urlPathCulture).TryUpdate(newSlug, out var collisions))
        {
            DocumentManager.ClearContentChanged();
            gridUrlsPaths.ReloadData();
            ShowChangesSaved();

            if (!urlPathCulture.Equals(LocalizationContext.PreferredCultureCode, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
            UpdateLiveSiteButtonUrl();
        }
        else
        {
            ShowCollisionErrorMessage(collisions);
            inlineEditingTextBox.ErrorText = ResHelper.GetString("content.ui.properties.pageurlpaths.inlineedit.conflict");
        }
    }


    private void UpdateLiveSiteButtonUrl()
    {
        var url = DocumentURLProvider.GetAbsoluteUrl(Node);
        ScriptHelper.RegisterSetLiveSiteURL(this, url);
    }


    private static object GetLabelControl(string completeUrl)
    {
        var label = new Label
        {
            Text = completeUrl,
            CssClass = "static-textpanel"
        };
        var container = new Panel
        {
            ToolTip = completeUrl,
            CssClass = "inline-editing-row-container",
        };
        container.Controls.Add(label);

        return container;
    }


    private InlineEditingTextBox GetInlineEditingControl(DataRowView dataRowView, string urlPath, string completeUrl)
    {
        var urlPathId = DataHelper.GetIntValue(dataRowView.Row, "PageUrlPathID");
        var urlPathCulture = DataHelper.GetStringValue(dataRowView.Row, "PageUrlPathCulture");

        var inlineEditingTextBox = new InlineEditingTextBox
        {
            Text = GetUrlPathSlug(urlPath),
            FormattedText = completeUrl,
            MaxLength = PageUrlPath.SLUG_LENGTH,
            AdditionalCssClass = "inline-editing-textbox-wide inline-editing-row-container",
            EnableEditingOnTextClick = false,
            ToolTip = completeUrl
        };

        inlineEditingTextBox.Update += (s, e) => UpdateUrlPathFromInline(inlineEditingTextBox, urlPathCulture);

        if (urlPathId == editedUrlPathId)
        {
            inlineEditingTextBox.Load += (s, e) => inlineEditingTextBox.SwitchToEditMode();
        }

        return inlineEditingTextBox;
    }


    protected object gridUrlPaths_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerInvariant())
        {
            case "openlivesite":
                {
                    if (pageCultures == null)
                    {
                        break;
                    }

                    var row = (DataRowView)((GridViewRow)parameter).DataItem;

                    var urlPath = DataHelper.GetStringValue(row.Row, "PageUrlPathUrlPath");
                    var urlPathCulture = DataHelper.GetStringValue(row.Row, "PageUrlPathCulture");

                    PageRoutingHelper.EnsurePathFormat(urlPath, Node.NodeSiteID, out var formattedPath);
                    var completeUrl = $"{GetSitePresentationUrl(urlPathCulture)}{formattedPath}";
                    var escapedUrl = Uri.EscapeUriString(completeUrl);

                    var eyeButton = (CMSGridActionButton)sender;
                    eyeButton.OnClientClick = $"window.open(\"{escapedUrl}\"); return false;";

                    var existsCultureVersion = pageCultures.Contains(urlPathCulture);
                    eyeButton.Enabled = existsCultureVersion;
                    eyeButton.ToolTip = existsCultureVersion
                        ? ResHelper.GetString("content.ui.properties.pageurlpaths.openlivesite")
                        : ResHelper.GetString("content.ui.properties.pageurlpaths.nonexistentcultureversion");
                }

                break;

            case "edit":

                if (!CanEditPageUrlPath())
                {
                    var editButton = (CMSGridActionButton)sender;
                    editButton.Enabled = false;
                }

                break;


            case "urlpath":
                {
                    var row = (DataRowView)parameter;
                    var urlPath = DataHelper.GetStringValue(row.Row, "PageUrlPathUrlPath");
                    var urlPathCulture = DataHelper.GetStringValue(row.Row, "PageUrlPathCulture");

                    PageRoutingHelper.EnsurePathFormat(urlPath, Node.NodeSiteID, out var formattedPath);
                    var completeUrl = HTMLHelper.HTMLEncode($"{GetSitePresentationUrl(urlPathCulture)}{formattedPath}");

                    // Ensure correct values for unigrid export
                    if (sender == null)
                    {
                        return completeUrl;
                    }

                    if (!CanEditPageUrlPath())
                    {
                        return GetLabelControl(completeUrl);
                    }

                    return GetInlineEditingControl(row, urlPath, completeUrl);
                }
        }

        return parameter;
    }


    protected void gridUrlPaths_OnAction(string actionName, object actionArgument)
    {
        if (!CanEditPageUrlPath())
        {
            return;
        }

        if (actionName.Equals("edit", StringComparison.InvariantCultureIgnoreCase))
        {
            int urlPathId = ValidationHelper.GetInteger(actionArgument, 0);
            editedUrlPathId = urlPathId;
        }
    }


    private string GetSitePresentationUrl(string cultureCode)
    {
        if (!mSitePresentationUrls.ContainsKey(cultureCode))
        {
            mSitePresentationUrls[cultureCode] = DocumentURLProvider.GetPresentationUrl(Node.NodeSiteID, cultureCode) + "/";
        }

        return mSitePresentationUrls[cultureCode];
    }
}