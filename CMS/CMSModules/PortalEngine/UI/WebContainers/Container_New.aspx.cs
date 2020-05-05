using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement(ModuleName.DESIGN, "NewWebPartContainer")]
public partial class CMSModules_PortalEngine_UI_WebContainers_Container_New : CMSModalDesignPage
{
    #region "Constants"

    /// <summary>
    /// Short link to help page.
    /// </summary>
    private const string HELP_TOPIC_LINK = "webparts_containers";

    #endregion


    #region "Variables"

    private bool mDialogMode;

    #endregion


    #region "Methods"

    protected override void OnPreInit(EventArgs e)
    {
        RequireSite = false;

        // Page has been opened from administration
        mDialogMode = QueryHelper.GetBoolean("dialog", false);

        // Initialize the master page
        if (mDialogMode)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master";
        }
        else
        {
            //Only global admin has access to the regular page
            CheckGlobalAdministrator();
        }

        // Must be called after the master page file is set
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        txtContainerCSS.FullScreenParentElementID = txtContainerText.FullScreenParentElementID = "divContent";

        if (mDialogMode)
        {
            SetDialogMode();
            btnOk.Visible = false;
        }
        else
        {
            InitBreadcrumbs();

            SiteInfo currentSite = SiteContext.CurrentSite;
            if (currentSite != null)
            {
                chkAssign.Text = HTMLHelper.HTMLEncode(currentSite.DisplayName);
                plcAssign.Visible = true;
            }
        }

        PageTitle.TitleText = GetString("Container_Edit.NewHeaderCaption");
        rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvCodeName.ErrorMessage = GetString("general.requirescodename");

        plcCssLink.Visible = String.IsNullOrEmpty(txtContainerCSS.Text);

        if (!RequestHelper.IsPostBack())
        {
            txtContainerText.Text = "<div>\n  " + WebPartContainerInfoProvider.WP_CHAR + "\n</div>";
        }
    }

    private void SaveContainer()
    {
        try
        {
            // Validate user input
            string errorMessage = new Validator()
                .NotEmpty(txtContainerDisplayName.Text, rfvDisplayName.ErrorMessage)
                .NotEmpty(txtContainerName.Text, rfvCodeName.ErrorMessage)
                .IsCodeName(txtContainerName.Text, GetString("General.InvalidCodeName"))
                .Result;

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ShowError(errorMessage);
                return;
            }

            // Parse the container text
            string text = txtContainerText.Text;
            string after = "";

            int wpIndex = text.IndexOf(WebPartContainerInfoProvider.WP_CHAR, StringComparison.Ordinal);
            if (wpIndex >= 0)
            {
                after = text.Substring(wpIndex + 1).Replace(WebPartContainerInfoProvider.WP_CHAR, "");
                text = text.Substring(0, wpIndex);
            }

            WebPartContainerInfo webPartContainerObj = new WebPartContainerInfo()
            {
                ContainerTextBefore = text,
                ContainerTextAfter = after,
                ContainerCSS = txtContainerCSS.Text,
                ContainerName = txtContainerName.Text.Trim(),
                ContainerDisplayName = txtContainerDisplayName.Text.Trim()
            };

            // Check for duplicity
            if (WebPartContainerInfoProvider.GetWebPartContainerInfo(webPartContainerObj.ContainerName) != null)
            {
                ShowError(GetString("Container_Edit.UniqueError"));
                return;
            }

            WebPartContainerInfoProvider.SetWebPartContainerInfo(webPartContainerObj);
            UIContext.EditedObject = webPartContainerObj;
            CMSObjectManager.CheckOutNewObject(Page);

            if (mDialogMode)
            {
                ProcessDialog(webPartContainerObj, true);
            }
            else
            {
                ProcessPage(webPartContainerObj);
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        SaveContainer();
    }


    private void InitBreadcrumbs()
    {
        // Breadcrumbs initialization        		
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("Container_Edit.ItemListLink"),
            RedirectUrl = UIContextHelper.GetElementUrl("cms.design", "Development.WebContainers", false)
        });

        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("Container_Edit.NewItemCaption")
        });
    }


    private void SetDialogMode()
    {
        Save += (o, ea) => SaveContainer();

        CurrentMaster.PanelTitleActions.Visible = false;

        // When in modal dialog, the window scrolls to bottom, so this hack will scroll it back to top
        string scrollScript = "var scrollerDiv = document.getElementById('divContent'); if (scrollerDiv != null) setTimeout(function() { scrollerDiv.scrollTop = 0; }, 500);";
        ScriptHelper.RegisterStartupScript(this, GetType(), "ScrollTop", scrollScript, true);

        // Setup help
        PageTitle.HelpTopicName = HELP_TOPIC_LINK;
    }


    private void ProcessPage(WebPartContainerInfo webPartContainer)
    {
        if (chkAssign.Visible && chkAssign.Checked)
        {
            WebPartContainerSiteInfoProvider.AddContainerToSite(webPartContainer, SiteContext.CurrentSite);
        }

        UIElementInfo ui = UIElementInfoProvider.GetUIElementInfo("cms.Design", "EditWebPartContainer");
        if (ui != null)
        {
            String url = UIContextHelper.GetElementUrl(ui, false);
            url = URLHelper.AddParameterToUrl(url, "objectID", webPartContainer.ContainerID.ToString());
            URLHelper.Redirect(url);
        }
    }


    private void ProcessDialog(WebPartContainerInfo webPartContainer, bool closeOnSave)
    {
        WebPartContainerSiteInfoProvider.AddContainerToSite(webPartContainer, SiteContext.CurrentSite);

        ScriptHelper.RegisterWOpenerScript(this);
        string returnHandler = QueryHelper.GetControlClientId("returnhandler", String.Empty);
        string script = $"if (wopener && wopener.{returnHandler}) {{ wopener.{returnHandler}('{webPartContainer.ContainerName}'); }}";

        // Redirects to edit window or simply closes the current window
        if (closeOnSave)
        {
            script += "CloseDialog();";
        }
        else
        {
            UIElementInfo ui = UIElementInfoProvider.GetUIElementInfo("cms.Design", "EditWebPartContainer");
            if (ui != null)
            {
                // Create base URL for element
                String url = UIContextHelper.GetElementUrl(ui, UIContext);

                // Append object ID
                url = URLHelper.AddParameterToUrl(url, "objectid", webPartContainer.ContainerID.ToString());

                // Append actual query string parameters
                url = URLHelper.AddParameterToUrl(url, "returnhandler", QueryHelper.GetString("returnhandler", String.Empty));
                url = URLHelper.AddParameterToUrl(url, "dialog", "1");
                url = URLHelper.AddParameterToUrl(url, "aliaspath", QueryHelper.GetString("aliaspath", String.Empty));
                url = URLHelper.AddParameterToUrl(url, "instanceguid", QueryHelper.GetString("instanceguid", String.Empty));

                // Append dialog hash
                url = ApplicationUrlHelper.AppendDialogHash(url);

                script += "window.location.replace('" + ResolveUrl(url) + "');";
            }
        }

        ScriptHelper.RegisterStartupScript(this, GetType(), "UpdateSelector", script, true);
    }

    #endregion
}