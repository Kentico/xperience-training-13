using System;
using System.Web.UI;

using CMS;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.Synchronization;


[assembly: RegisterCustomClass("EmailTemplateEditExtender", typeof(EmailTemplateEditExtender))]
/// <summary>
/// Email template UIForm extender
/// </summary>
public class EmailTemplateEditExtender : ControlExtender<UIForm>
{
    public override void OnInit()
    {
        Control.Page.PreRender += Page_PreRender;
    }


    private void Page_PreRender(object sender, EventArgs e)
    {
        if (!Control.IsInsertMode)
        {
            InitHeaderActions();
        }
    }


    private void InitHeaderActions()
    {
        EmailTemplateInfo emailTemplate = Control.EditedObject as EmailTemplateInfo;

        if ((emailTemplate != null) && (emailTemplate.TemplateID > 0))
        {
            Page page = Control.Page;

            ObjectEditMenu menu = (ObjectEditMenu)ControlsHelper.GetChildControl(page, typeof(ObjectEditMenu));
            if (menu != null)
            {
                RegisterEditPageHeaderActions(page, menu, emailTemplate);
            }
        }
    }


    /// <summary>
    /// Registers email template actions headers to given page and menu item
    /// </summary>
    /// <param name="page">Page where the action is registered</param>
    /// <param name="menu">Menu where the action is registered</param>
    /// <param name="emailTemplate">Email template for which actions are registered</param>
    public static void RegisterEditPageHeaderActions(Page page, ObjectEditMenu menu, EmailTemplateInfo emailTemplate)
    {
        if (page == null)
        {
            throw new ArgumentNullException("page");
        }

        if (menu == null)
        {
            throw new ArgumentNullException("menu");
        }

        if (emailTemplate == null)
        {
            throw new ArgumentNullException("emailTemplate");
        }

        ScriptHelper.RegisterDialogScript(page);

        RegisterSendDraft(menu, emailTemplate);
        RegisterAttachments(page, menu, emailTemplate);
    }


    private static void RegisterSendDraft(ObjectEditMenu menu, EmailTemplateInfo emailTemplate)
    {
        // Prepare draft dialog URL
        var draftDialogUrl = UrlResolver.ResolveUrl("~/CMSModules/EmailTemplates/Pages/SendDraft.aspx");
        draftDialogUrl = URLHelper.AddParameterToUrl(draftDialogUrl, "objectid", emailTemplate.TemplateID.ToString());

        var draftCaption = ResHelper.GetString("emailtemplates.senddraft");

        // Add send draft action
        menu.AddExtraAction(new HeaderAction
        {
            Text = draftCaption,
            Tooltip = draftCaption,
            OnClientClick = string.Format(@"if (modalDialog) {{modalDialog('{0}', 'SendDraft', '700', '325');}}", draftDialogUrl) + " return false;",
            Enabled = true,
            ButtonStyle = ButtonStyle.Default
        });
    }


    private static void RegisterAttachments(Page page, ObjectEditMenu menu, EmailTemplateInfo emailTemplate)
    {
        const string ATTACHMENTS_ACTION_CLASS = "attachments-header-action";

        // Register attachments count update module
        ScriptHelper.RegisterModule(page, "CMS/AttachmentsCountUpdater", new
        {
            Selector = "." + ATTACHMENTS_ACTION_CLASS,
            Text = ResHelper.GetString("general.attachments")
        });

        // Prepare metafile dialog URL
        var metaFileDialogUrl = UrlResolver.ResolveUrl(@"~/CMSModules/AdminControls/Controls/MetaFiles/MetaFileDialog.aspx");
        var query = string.Format("?objectid={0}&objecttype={1}&siteid={2}", emailTemplate.TemplateID, EmailTemplateInfo.OBJECT_TYPE, emailTemplate.TemplateSiteID);
        metaFileDialogUrl += string.Format("{0}&category={1}&hash={2}", query, ObjectAttachmentsCategories.TEMPLATE, QueryHelper.GetHash(query));

        var attachCount = GetAttachmentsCount(emailTemplate);
        var attachmentsCaption = ResHelper.GetString("general.attachments");

        // Add attachments action
        menu.AddExtraAction(new HeaderAction
        {
            Text = attachmentsCaption + (attachCount > 0 ? " (" + attachCount + ")" : string.Empty),
            Tooltip = attachmentsCaption,
            OnClientClick = string.Format(@"if (modalDialog) {{modalDialog('{0}', 'Attachments', '700', '500');}}", metaFileDialogUrl) + " return false;",
            Enabled = !SynchronizationHelper.UseCheckinCheckout || emailTemplate.Generalized.IsCheckedOutByUser(MembershipContext.AuthenticatedUser),
            CssClass = ATTACHMENTS_ACTION_CLASS,
            ButtonStyle = ButtonStyle.Default
        });
    }


    private static int GetAttachmentsCount(EmailTemplateInfo emailTemplate)
    {
        var objectQuery = MetaFileInfo.Provider.Get();
        objectQuery.WhereCondition = MetaFileInfoProvider.GetWhereCondition(emailTemplate.TemplateID, EmailTemplateInfo.OBJECT_TYPE, ObjectAttachmentsCategories.TEMPLATE);
        objectQuery.Columns(MetaFileInfo.TYPEINFO.IDColumn);

        var siteId = emailTemplate.TemplateSiteID;
        if (siteId > 0)
        {
            objectQuery.OnSite(siteId);
        }
        else
        {
            objectQuery.WhereNull(MetaFileInfo.TYPEINFO.SiteIDColumn);
        }

        return objectQuery.Count;
    }
}
