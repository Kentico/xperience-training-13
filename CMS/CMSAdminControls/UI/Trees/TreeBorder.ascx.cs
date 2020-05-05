using System;

using CMS.UIControls;


public partial class CMSAdminControls_UI_Trees_TreeBorder : CMSUserControl
{
    #region "Properties"

    protected bool mAllowMouseResizing = true;


    /// <summary>
    /// Resizer control.
    /// </summary>
    public CMSUserControl FrameResizer
    {
        get
        {
            return frmResizer;
        }
    }


    /// <summary>
    /// Frameset minimized size.
    /// </summary>
    public string MinSize
    {
        get
        {
            return frmResizer.MinSize;
        }
        set
        {
            frmResizer.MinSize = value;
        }
    }


    /// <summary>
    /// Frameset name.
    /// </summary>
    public string FramesetName
    {
        get
        {
            return frmResizer.FramesetName;
        }
        set
        {
            frmResizer.FramesetName = value;
        }
    }


    /// <summary>
    /// If true, resizing with mouse is enabled.
    /// </summary>
    public bool AllowMouseResizing
    {
        get
        {
            return mAllowMouseResizing;
        }
        set
        {
            mAllowMouseResizing = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (AllowMouseResizing && FrameResizer.Visible)
        {
            pnlBorder.Attributes.Add("onmousemove", "InitFrameResizer(this); return false;");
            pnlBorder.Attributes.Add("unselectable", "on");
        }
        else
        {
            pnlBorder.Style.Add("cursor", "default");
        }
    }

    #endregion
}