using System;
using System.Text.RegularExpressions;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Internal;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.UIControls;

[EditedObject(AlternativeUrlInfo.OBJECT_TYPE, "objectid")]
public partial class CMSModules_Content_CMSDesk_Properties_Urls_AlternativeUrlEdit : CMSPropertiesPage
{
    private const string RESOURCE_NAME = "CMS.Content";
    private const string PERMISSION_NAME = "ManageAlternativeURLs";
    private const string COMMAND_ARGUMENT_SAVE_ANOTHER = "saveandanother";

    private bool? mCanManageAlternativeURLs;
    private string mSitePresentationUrl;
    private AlternativeUrlInfo alternativeUrl;

    protected bool CanManageAlternativeURLs
    {
        get
        {
            if (!mCanManageAlternativeURLs.HasValue)
            {
                mCanManageAlternativeURLs = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(RESOURCE_NAME, PERMISSION_NAME);
            }

            return mCanManageAlternativeURLs.Value;
        }
    }


    private string SitePresentationUrl
    {
        get
        {
            return mSitePresentationUrl ?? (mSitePresentationUrl = DocumentURLProvider.GetPresentationUrl(Node.NodeSiteID, Node.DocumentCulture) + "/");
        }
    }


    protected override void OnInit(EventArgs e)
    {
        DocumentManager.RegisterEvents = false;
        DocumentManager.UseDocumentHelper = false;
        DocumentManager.CheckPermissions = false;

        base.OnInit(e);

        // Register the scripts
        ScriptHelper.RegisterLoader(Page);

        if (!CanManageAlternativeURLs)
        {
            RedirectToAccessDenied(RESOURCE_NAME, PERMISSION_NAME);
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        lblAltUrl.AssociatedControlClientID = txtAltUrl.TextBox.ClientID;
        txtAltUrl.PlaceholderText = TextHelper.LimitLength(SitePresentationUrl, 45, CutTextEnum.Start);

        menu.ActionsList.Add(new SaveAction());
        menu.ActionsList.Add(new SaveAction()
        {
            ButtonStyle = ButtonStyle.Default,
            Text = GetString("content.ui.properties.saveandaddanotherlternativeurl"),
            CommandArgument = COMMAND_ARGUMENT_SAVE_ANOTHER
        });
        menu.ActionsList.Add(new HeaderAction
        {
            ButtonStyle = ButtonStyle.Default,
            RedirectUrl = GetListingUrl(),
            Text = GetString("general.back"),
            OnClientClick = "if (CheckChanges && !CheckChanges()) {{ return false; }}"
        });

        // Register for action
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.SAVE, btnOk_Click);

        // Ensure script which allows redirecting to conflicting page
        ScriptHelper.RegisterEditScript(Page, false);

        alternativeUrl = GetAlternativeUrlInfo();

        // Initializes page breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("content.ui.propertiesurls"),
            RedirectUrl = GetListingUrl(),
            Target = "propedit"
        });

        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = alternativeUrl.AlternativeUrlID > 0 ? TextHelper.LimitLength(alternativeUrl.AlternativeUrlUrl.ToString(), 100, cutLocation: CutTextEnum.Middle) : GetString("content.ui.properties.addalternativeurl")
        });

        if (alternativeUrl.AlternativeUrlID == 0)
        {
            // Do not include type as breadcrumbs suffix
            UIHelper.SetBreadcrumbsSuffix("");
        }

        if (!RequestHelper.IsPostBack())
        {
            // Automatically display changes saved text
            if (QueryHelper.GetBoolean("saved", false))
            {
                ShowChangesSaved();
            }

            txtAltUrl.Text = alternativeUrl.AlternativeUrlUrl.NormalizedUrl;
        }
    }


    private string GetListingUrl()
    {
        return $"~/CMSModules/Content/CMSDesk/Properties/Urls.aspx?nodeid={NodeID}";
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        if (!CanManageAlternativeURLs)
        {
            RedirectToAccessDenied(RESOURCE_NAME, PERMISSION_NAME);
        }

        // Save alternative URL or display error message if error occurred
        var error = SaveData(alternativeUrl);
        if (String.IsNullOrEmpty(error))
        {
            var commandArgument = (e as System.Web.UI.WebControls.CommandEventArgs)?.CommandArgument as string;
            if (String.Equals(commandArgument, COMMAND_ARGUMENT_SAVE_ANOTHER, StringComparison.Ordinal))
            {
                var url = UrlResolver.ResolveUrl("~/CMSModules/Content/CMSDesk/Properties/Urls_AlternativeUrlEdit.aspx");
                url = URLHelper.AddParameterToUrl(url, "nodeid", NodeID.ToString());
                url = URLHelper.AddParameterToUrl(url, "saved", "1");
                URLHelper.Redirect(url);

            }
            URLHelper.Redirect(GetListingUrl());
        }
        else
        {
            ShowError(error);
        }
    }


    /// <summary>
    /// Returns <see cref="String.Empty"/> when <paramref name="alternativeUrl"/> was saved.
    /// If error occurs during saving the <paramref name="alternativeUrl"/>, corresponding error message is returned.
    /// </summary>
    private string SaveData(AlternativeUrlInfo alternativeUrl)
    {
        alternativeUrl.AlternativeUrlUrl = AlternativeUrlHelper.NormalizeAlternativeUrl(txtAltUrl.Text);

        if (String.IsNullOrWhiteSpace(alternativeUrl.AlternativeUrlUrl.NormalizedUrl))
        {
            return GetString("general.requiresvalue");
        }

        try
        {
            AlternativeUrlInfoProvider.SetInfoCheckForConflictingPage(alternativeUrl);
        }
        catch (InvalidAlternativeUrlException ex) when (ex.CollisionData != null)
        {
            var collisionData = ex.CollisionData;
            if (collisionData.CultureVersionExists)
            {
                var editDocumentLink = $"<a onclick=\"window.CMSContentManager.changed(false); EditDocument({collisionData.NodeID}, 'Properties.URLs', {ScriptHelper.GetString(collisionData.CultureCode)})\">{GetDocumentIdentification(collisionData.PageName, collisionData.CultureCode)}</a>";
                return String.Format(GetString("alternativeurl.isinconflictwithpage"), alternativeUrl.AlternativeUrlUrl, editDocumentLink);
            }
            else
            {
                var editDocumentLink = $"<a onclick=\"window.CMSContentManager.changed(false); EditDocument({collisionData.NodeID}, 'Properties.URLs|{PageRoutingUIHelper.SLUGS_LIST_SUBTAB_NAME}', {ScriptHelper.GetString(collisionData.ExistingCultureCode)})\">{collisionData.PageName}</a>";
                return String.Format(GetString("alternativeurl.isinconflictwithnottranslatedpage"), alternativeUrl.AlternativeUrlUrl, GetCultureShortName(collisionData.CultureCode), editDocumentLink);
            }
        }
        catch (InvalidAlternativeUrlException ex) when (ex.ConflictingAlternativeUrl != null)
        {
            var page = DocumentHelper.GetDocument(ex.ConflictingAlternativeUrl.AlternativeUrlDocumentID, new TreeProvider());
            var editDocumentLink = $"<a onclick=\"window.CMSContentManager.changed(false); EditDocument({page.NodeID}, 'Properties.URLs', {ScriptHelper.GetString(page.DocumentCulture)})\">{GetDocumentIdentification(page)}</a>";
            return String.Format(GetString("alternativeurl.isinconflictwithalternativeurl"), alternativeUrl.AlternativeUrlUrl, editDocumentLink);
        }
        catch (InvalidAlternativeUrlException ex) when (!String.IsNullOrEmpty(ex.ExcludedUrl) || ex.AlternativeUrl != null)
        {
            var errorMessage = SettingsKeyInfoProvider.GetValue("CMSAlternativeURLsErrorMessage", Node.NodeSiteID);
            return String.IsNullOrEmpty(errorMessage) ?
                GetString("settingskey.cmsalternativeurlsfallbackerrormessage") :
                ResHelper.LocalizeString(errorMessage);
        }
        catch (InvalidAlternativeUrlException)
        {
            return String.Format(GetString("general.mustbeunique"), TypeHelper.GetNiceObjectTypeName(AlternativeUrlInfo.OBJECT_TYPE));
        }
        catch (RegexMatchTimeoutException)
        {
            return String.Format(GetString("settingskey.cmsalternativeurlstimeouterrormessage"), alternativeUrl.AlternativeUrlUrl);
        }
        catch (Exception ex)
        {
            Service.Resolve<IEventLogService>().LogException("Alternative URLs", "EDIT", ex);
            return GetString("alternativeurl.generalerror");
        }

        DocumentManager.SaveDocument();

        txtAltUrl.Text = alternativeUrl.AlternativeUrlUrl.NormalizedUrl;

        return String.Empty;
    }


    private static string GetDocumentIdentification(TreeNode page)
    {
        return HTMLHelper.HTMLEncode(AlternativeUrlHelper.GetDocumentIdentification(page.DocumentName, page.DocumentCulture));
    }


    private static string GetDocumentIdentification(string pageName, string pageCulture)
    {
        return HTMLHelper.HTMLEncode(AlternativeUrlHelper.GetDocumentIdentification(pageName, pageCulture));
    }


    private static string GetCultureShortName(string cultureCode)
    {
        return $"{CultureInfoProvider.GetCultureInfoForCulture(cultureCode)?.CultureShortName ?? cultureCode}";
    }


    private AlternativeUrlInfo GetAlternativeUrlInfo()
    {
        if (QueryHelper.GetInteger("objectid", 0) > 0)
        {
            return EditedObject as AlternativeUrlInfo;
        }

        return new AlternativeUrlInfo
        {
            AlternativeUrlDocumentID = Node.DocumentID,
            AlternativeUrlSiteID = Node.NodeSiteID
        };
    }
}