using System;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;


public partial class CMSModules_WebAnalytics_Controls_UI_Conversion_Edit : CMSAdminEditControl
{
    private string mOldConversionName = String.Empty;


    /// <summary>
    /// UIForm control used for editing objects properties.
    /// </summary>
    public UIForm UIFormControl
    {
        get
        {
            return EditForm;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            EditForm.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            EditForm.IsLiveSite = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        EditForm.OnBeforeSave += EditForm_OnBeforeSave;
        EditForm.OnAfterSave += EditForm_OnAfterSave;
        var modalDialog = QueryHelper.GetBoolean("modaldialog", false);

        if (modalDialog)
        {
            EditForm.SubmitButton.Visible = false;
            EditForm.RedirectUrlAfterCreate = "";
        }
        else
        {
            var editUrl = UIContextHelper.GetElementUrl("CMS.WebAnalytics", "ConversionProperties");
            editUrl = URLHelper.AddParameterToUrl(editUrl, "objectId", "{%EditedObject.ID%}");
            editUrl = URLHelper.AddParameterToUrl(editUrl, "saved", "1");
            editUrl = URLHelper.AddParameterToUrl(editUrl, "displayTitle", "0");
            EditForm.RedirectUrlAfterCreate = editUrl;
        }
    }


    private void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        var ci = EditForm.EditedObject as ConversionInfo;
        // If code name has changed (on existing object) => Rename all analytics statistics data.
        if ((ci != null) && (ci.ConversionName != mOldConversionName) && (mOldConversionName != String.Empty))
        {
            ConversionInfoProvider.RenameConversionStatistics(mOldConversionName, ci.ConversionName, SiteContext.CurrentSiteID);
        }
    }


    private void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        var ci = EditForm.EditedObject as ConversionInfo;
        if (ci != null)
        {
            ci.ConversionSiteID = SiteContext.CurrentSiteID;
            mOldConversionName = ci.ConversionName;
        }
    }


    /// <summary>
    /// Saves the data
    /// </summary>
    /// <param name="redirect">If true, use server redirect after successful save</param>
    public bool Save(bool redirect)
    {
        var selectorID = QueryHelper.GetString("selectorID", String.Empty);

        var ret = EditForm.SaveData("");

        // If saved - redirect with ConversionID parameter
        if (ret && redirect)
        {
            var ci = (ConversionInfo)EditForm.EditedObject;
            if (ci != null)
            {
                URLHelper.Redirect(UrlResolver.ResolveUrl("edit.aspx?conversionid=" + ci.ConversionID + "&saved=1&modaldialog=true&selectorID=" + selectorID));
            }
        }

        return ret;
    }
}