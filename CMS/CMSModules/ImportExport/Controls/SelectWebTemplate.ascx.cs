using System;
using System.Data;

using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_SelectWebTemplate : CMSUserControl
{
    /// <summary>
    /// Selected template ID.
    /// </summary>
    public int WebTemplateId
    {
        get
        {
            return ucSelector.SelectedId;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            ReloadData();
        }
    }


    public void ReloadData()
    {
        // Load the data
        DataSet templates = WebTemplateInfoProvider.GetWebTemplates(null, "WebTemplateOrder", 0, "WebTemplateID, WebTemplateDisplayName, WebTemplateDescription, WebTemplateFileName, WebTemplateThumbnailGUID, NULL AS WebTemplateIconClass" , true);

        ucSelector.DataSource = templates;
        ucSelector.IDColumn = "WebTemplateID";
        ucSelector.DisplayNameColumn = "WebTemplateDisplayName";
        ucSelector.DescriptionColumn = "WebTemplateDescription";
        ucSelector.ThumbnailGUIDColumn = "WebTemplateThumbnailGUID";
        ucSelector.IconClassColumn = "WebTemplateIconClass";

        if (ucSelector.SelectedId == 0)
        {
            if (!DataHelper.DataSourceIsEmpty(templates))
            {
                int firstTemplateId = ValidationHelper.GetInteger(templates.Tables[0].Rows[0]["WebTemplateID"], 0);
                ucSelector.SelectedId = firstTemplateId;
            }
        }

        ucSelector.DataBind();
    }


    /// <summary>
    /// Apply control settings.
    /// </summary>
    public bool ApplySettings()
    {
        if (WebTemplateId <= 0)
        {
            lblError.Text = GetString("TemplateSelection.SelectTemplate");
            return false;
        }

        return true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblError.Visible = (lblError.Text != "");
    }
}