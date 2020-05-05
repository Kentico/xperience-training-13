using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;


[EditedObject(ConversionInfo.OBJECT_TYPE, "conversionid")]
[CheckLicence(FeatureEnum.CampaignAndConversions)]
[UIElement(ModuleName.WEBANALYTICS, "Conversions.General", false, true)]
public partial class CMSModules_WebAnalytics_Pages_Tools_Conversion_Edit : CMSDeskPage
{
    private bool mModalDialog;

    // Help variable for set info label of UI form
    private string mInfoText = String.Empty;


    protected void Page_PreInit(object sender, EventArgs e)
    {
        mModalDialog = QueryHelper.GetBoolean("modalDialog", false);
        if (mModalDialog)
        {
            // Display in selector dialog
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master";

            var master = CurrentMaster as ICMSModalMasterPage;
            if (master != null)
            {
                master.Save += (s, ea) => editElem.Save(true);
                master.ShowSaveAndCloseButton();
            }
        }
        IsDialog = mModalDialog;
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        var ci = EditedObject as ConversionInfo;
        
        var conversionName = QueryHelper.GetString("conversionName", String.Empty);
        if (conversionName != String.Empty)
        {
            // Try to check dialog mode
            conversionName = conversionName.Trim(';');
            ci = ConversionInfoProvider.GetConversionInfo(conversionName, SiteContext.CurrentSiteName);
        }
        
        // Test whether conversion is in current site, if not - test if user is authorized for conversion's site
        if (ci != null)
        {
            if (!ci.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
            {
                RedirectToAccessDenied(ci.TypeInfo.ModuleName, "Read");
            }
        }

        if ((conversionName != String.Empty) && (ci == null))
        {
            // Set warning text
            mInfoText = String.Format(GetString("conversion.editedobjectnotexits"), HTMLHelper.HTMLEncode(conversionName));

            // Create new conversion info based on conversion name
            ci = new ConversionInfo();
            ci.ConversionName = conversionName;
            ci.ConversionDisplayName = conversionName;
        }

       
        if (mModalDialog)
        {
            if (ci != null)
            {
                PageTitle.TitleText = GetString("analytics.conversion");
            }
            else
            {
                PageTitle.TitleText = GetString("conversion.conversion.new");
            }
        }

        if (ci != null)
        {
            EditedObject = ci;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        var isNew = (EditedObject == null) || (((ConversionInfo)EditedObject).ConversionID <= 0);
        if (isNew && !mModalDialog)
        {
            // Set the title
            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem { Text = GetString("conversion.conversion.list"), RedirectUrl = "~/CMSModules/WebAnalytics/Pages/Tools/Conversion/List.aspx" });
            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem { Text = GetString("conversion.conversion.new") });
        }

        // Set info label
        editElem.UIFormControl.ShowInformation(mInfoText);
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (QueryHelper.GetBoolean("saved", false) && !RequestHelper.IsPostBack())
        {
            UpdateUniSelector(true);
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Adds scripts to update parent's uniselector (used in modal dialogs)
    /// </summary>
    /// <param name="closeOnSave">If true, window close JS is added</param>
    private void UpdateUniSelector(bool closeOnSave)
    {
        var selector = HTMLHelper.HTMLEncode(QueryHelper.GetString("selectorid", String.Empty));
        var conversion = editElem.UIFormControl.EditedObject as ConversionInfo;
        if (!String.IsNullOrEmpty(selector) && (conversion != null))
        {
            ScriptHelper.RegisterWOpenerScript(this);
            // Add selector refresh
            var script =
                String.Format(@"if (wopener) {{ wopener.US_SelectNewValue_{0}('{1}'); }}", ValidationHelper.GetControlClientId(selector, ""), conversion.ConversionName);

            if (closeOnSave)
            {
                script += "CloseDialog();";
            }

            ScriptHelper.RegisterStartupScript(this, GetType(), "UpdateSelector", script, true);
        }
    }
}
