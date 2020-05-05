using System;

using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_NewSiteType : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Returns true if the template should be selected.
    /// </summary>
    public bool SelectTemplate
    {
        get
        {
            return radTemplate.Checked;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        radBlank.Text = GetString("NewSite_ChooseSite.NewSite");
        radTemplate.Text = GetString("NewSite_ChooseSite.TemplateSite");
    }
}