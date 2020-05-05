using System;
using System.Collections.Generic;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;


public partial class CMSWebParts_Text_editabletext : CMSAbstractEditableWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets the url of the page which ensures editing of the web part's editable content in the On-Site editing mode.
    /// </summary>
    public override string EditPageUrl
    {
        get
        {
            return ucEditableText.EditPageUrl;
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
                SetValue("DisplayToRoles", String.Empty);
                break;
        }

        ucEditableText.StopProcessing = StopProcessing;

        if (!StopProcessing)
        {
            ucEditableText.ContentID = WebPartID;
            ucEditableText.DataControl = this;
            ucEditableText.PageManager = PageManager;
            ucEditableText.PagePlaceholder = PagePlaceholder;
            ucEditableText.Title = HTMLHelper.HTMLEncode(String.Format("{0} ({1})", GetString("general.richtexteditor"), String.IsNullOrEmpty(WebPartTitle) ? WebPartID : WebPartTitle));
            ucEditableText.SetupControl();
        }
    }


    /// <summary>
    /// Overridden CreateChildControls method.
    /// </summary>
    protected override void CreateChildControls()
    {
        SetupControl();
        base.CreateChildControls();

        switch (ViewMode)
        {
            case ViewModeEnum.Edit:
            case ViewModeEnum.EditLive:
            case ViewModeEnum.EditDisabled:
                ucEditableText.UseInlineMode = true;
                break;
        }
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
            ucEditableText.LoadContent(content, forceReload);

            if (!string.IsNullOrEmpty(ucEditableText.DefaultText))
            {
                // Default image defined => content is not empty
                EmptyContent = false;
            }
        }
    }


    /// <summary>
    /// Returns true if entered data is valid. If data is invalid, it returns false and displays an error message.
    /// </summary>
    public override bool IsValid()
    {
        return ucEditableText.IsValid();
    }


    /// <summary>
    /// Gets the current control content.
    /// </summary>
    public override string GetContent()
    {
        if (!StopProcessing)
        {
            return ucEditableText.GetContent();
        }

        return null;
    }


    /// <summary>
    /// Returns the array list of the field IDs (Client IDs of the inner controls) that should be spell checked.
    /// </summary>
    public override List<string> GetSpellCheckFields()
    {
        return ucEditableText.GetSpellCheckFields();
    }


    /// <summary>
    /// Try initialize control
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        SetupControl();
        base.OnInit(e);
    }

    #endregion
}