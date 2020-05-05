using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Objects_Controls_Locking_ObjectEditPanel : CMSUserControl, IObjectEditPanel
{
    #region "Properties"

    /// <summary>
    /// Messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return pnlMessagePlaceholder;
        }
    }


    /// <summary>
    /// Header actions of the object edit menu.
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            return editMenuElem.HeaderActions;
        }
    }


    /// <summary>
    /// Gets the object edit menu control.
    /// </summary>
    public ObjectEditMenu ObjectEditMenu
    {
        get
        {
            return editMenuElem;
        }
    }


    /// <summary>
    /// Gets the object edit menu control.
    /// </summary>
    public IObjectEditMenu AbstractObjectEditMenu
    {
        get
        {
            return editMenuElem;
        }
    }


    /// <summary>
    /// Gets the object manager control used in the object edit menu.
    /// </summary>
    public CMSObjectManager ObjectManager
    {
        get
        {
            return editMenuElem.ObjectManager;
        }
    }


    /// <summary>
    /// Gets the object manager control used in the object edit menu.
    /// </summary>
    public ICMSObjectManager AbstractObjectManager
    {
        get
        {
            return ObjectManager;
        }
    }


    /// <summary>
    /// Returns panel in which the ObjectEditMenu is wrapped.
    /// </summary>
    public Panel MenuPanel
    {
        get
        {
            return pnlMenu;
        }
    }


    /// <summary>
    /// Gets the inner page title control.
    /// </summary>
    public PageTitle Title
    {
        get
        {
            return titleElem;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates if the control is used in the preview mode.
    /// </summary>
    public bool PreviewMode
    {
        get
        {
            return editMenuElem.PreviewMode;
        }
        set
        {
            editMenuElem.PreviewMode = value;
        }
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Get master page if present
        ICMSMasterPage master = Page.Master as ICMSMasterPage;

        // Connect header actions of current master page
        if (master != null)
        {
            master.HeaderActions = HeaderActions;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        var displayBreadCrumbs = titleElem.Breadcrumbs.Count > 0;

        titleElem.StopProcessing = !displayBreadCrumbs;
        titleElem.Visible = displayBreadCrumbs;
    }
}