using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Xml;

using CMS.DocumentEngine.Web.UI;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;
using CMS.Base;
using CMS.DocumentEngine;

public partial class CMSWebParts_Text_editableimage : CMSAbstractEditableWebPart, IDialogControl
{
    #region "Public properties"

    /// <summary>
    /// Configuration of the dialog for inserting Images.
    /// </summary>
    public DialogConfiguration DialogConfig
    {
        get
        {
            return null;
        }
        set
        {
        }
    }


    /// <summary>
    /// Gets the url of the page which ensures editing of the web part's editable content in the On-Site editing mode.
    /// </summary>
    public override string EditPageUrl
    {
        get
        {
            return ucEditableImage.EditPageUrl;
        }
    }


    /// <summary>
    /// Gets the width of the edit dialog in the On-Site editing mode.
    /// </summary>
    public override string EditDialogWidth
    {
        get
        {
            return ucEditableImage.EditDialogWidth;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        this.Visible = true;

        // Do not hide for roles in edit or preview mode
        switch (ViewMode)
        {
            case ViewModeEnum.Edit:
            case ViewModeEnum.EditLive:
            case ViewModeEnum.EditDisabled:
            case ViewModeEnum.Design:
            case ViewModeEnum.DesignDisabled:
            case ViewModeEnum.EditNotCurrent:
            case ViewModeEnum.Preview:
                DisplayToRoles = "";
                break;
        }

        ucEditableImage.StopProcessing = StopProcessing;

        if (!StopProcessing)
        {
            ucEditableImage.ContentID = this.WebPartID;
            ucEditableImage.DataControl = this as ISimpleDataContainer;
            ucEditableImage.PageManager = PageManager;
            ucEditableImage.PagePlaceholder = PagePlaceholder;
            ucEditableImage.SetupControl();
        }
    }


    /// <summary>
    /// Overridden CreateChildControls method.
    /// </summary>
    protected override void CreateChildControls()
    {
        SetupControl();
        base.CreateChildControls();
    }


    /// <summary>
    /// Loads the control content.
    /// </summary>
    /// <param name="content">Content to load</param>
    /// <param name="forceReload">If true, the content is forced to reload</param>
    public override void LoadContent(string content, bool forceReload)
    {
        if (!StopProcessing)
        {
            ucEditableImage.LoadContent(content, forceReload);

            if (!string.IsNullOrEmpty(ucEditableImage.DefaultImage))
            {
                // Default image defined => content is not empty
                EmptyContent = false;
            }
        }
    }


    /// <summary>
    /// Gets the current control content.
    /// </summary>
    public override string GetContent()
    {
        if (!StopProcessing)
        {
            EnsureChildControls();

            return ucEditableImage.GetContent();
        }
        return null;
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// OnPreRender event
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (!ViewMode.IsEditLive())
        {
            // Use the control visibility
            this.Visible = ucEditableImage.Visible;
        }

        base.OnPreRender(e);
    }

    #endregion
}