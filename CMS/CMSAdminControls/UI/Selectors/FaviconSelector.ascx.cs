using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_UI_Selectors_FaviconSelector : FormEngineUserControl
{
    private bool mEnabled = true;


    /// <summary>
    /// Gets or sets if value can be changed.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;

            // Sets all FaviconSelector components as Enabled or Disabled depending on value
            pnlSingleFileSelector.Enabled = value;
            pnlMultipleFileSelector.Enabled = value;
            radSingleFavicon.Enabled = value;
            radMultipleFavicons.Enabled = value;
        }
    }


    /// <summary>
    /// Path to selected favicon.
    /// </summary>
    public override object Value
    {
        get
        {
            return radSingleFavicon.Checked ? fsSingleFavicon.Value : fsMultipleFavicon.Value;
        }
        set
        {
            var path = value as string;

            // Set setting controls accordingly to the selected path
            fsSingleFavicon.Value = path;
            fsMultipleFavicon.Value = path;

            // Sets radio buttons accordingly to the type of selected path
            var isDirectory = !String.IsNullOrWhiteSpace(path) && FileHelper.DirectoryExists(path);
            radMultipleFavicons.Checked = isDirectory;
            radSingleFavicon.Checked = !isDirectory;
        }
    }


    public override bool IsValid()
    {
        return radSingleFavicon.Checked ? fsSingleFavicon.IsValid() : fsMultipleFavicon.IsValid();
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        fsSingleFavicon.AllowedExtensions = String.Join(";", FaviconMarkupBuilder.AllowedExtensions);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Set the proper file selector visible
        pnlSingleFileSelector.Visible = radSingleFavicon.Checked;
        pnlMultipleFileSelector.Visible = radMultipleFavicons.Checked;
    }


    protected void component_Changed(object sender, EventArgs e)
    {
        RaiseOnChanged();
    }
}