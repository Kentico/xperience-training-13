using System;

using CMS.UIControls;

public partial class CMSInstall_Controls_LayoutPanels_Warning : CMSUserControl
{
    /// <summary>
    /// Gets client ID of warning label control.
    /// </summary>
    public string WarningLabelClientID
    {
        get
        {
            return lblWarning.ClientID;
        }
    }
}
