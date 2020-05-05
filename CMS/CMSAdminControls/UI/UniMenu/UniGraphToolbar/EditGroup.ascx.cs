using System;

using CMS.UIControls;


public partial class CMSAdminControls_UI_UniMenu_UniGraphToolbar_EditGroup : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Handles value sent to control.
    /// </summary>
    public string JsGraphObject
    {
        get
        {
            return (string)GetValue("JsGraphObject");
        }
        set
        {
            SetValue("JsGraphObject", value);
        }
    }


    /// <summary>
    /// Pattern to be searched.
    /// </summary>
    public string Search
    {
        get
        {
            return txtSearch.Text;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// OnLoad event handler.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Add water mark text 
        txtSearch.WatermarkText = GetString("unigraphToolbar.searchTooltip");
    }

    #endregion
}