using System;

using CMS.Base;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.FormEngine.Web.UI;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_UIControls_NewHeaderAction : CMSAbstractUIWebpart
{
    #region "Properties"

    /// <summary>
    /// Target element
    /// </summary>
    public string NewElement
    {
        get
        {
            return GetStringContextValue("NewElement");
        }
        set
        {
            SetValue("NewElement", value);
        }
    }


    /// <summary>
    /// Action target URL
    /// </summary>
    public String TargetUrl
    {
        get
        {
            return GetStringContextValue("TargetUrl");
        }
        set
        {
            SetValue("TargetUrl", value);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        String text = String.Empty;
        String url = TargetUrl;
        bool enabled = true;
        String onClick = String.Empty;

        // If new element is set, load it directly, otherwise use first element with prefix 'new' in its name.
        UIElementInfo uiNew = String.IsNullOrEmpty(NewElement) ? UIContext.UIElement.GetNewElement() :
            UIElementInfoProvider.GetUIElementInfo(UIContext.UIElement.ElementResourceID, NewElement);

        bool openInDialog = false;
        String dialogWidth = null;
        String dialogHeight = null;

        if (uiNew != null)
        {
            UIContextData data = new UIContextData();
            data.LoadData(uiNew.ElementProperties);
            text = UIElementInfoProvider.GetElementCaption(uiNew);
            enabled = UIContextHelper.CheckElementVisibilityCondition(uiNew);
            url = ContextResolver.ResolveMacros(UIContextHelper.GetElementUrl(uiNew, UIContext));

            openInDialog = data["OpenInDialog"].ToBoolean(false);
            dialogWidth = data["DialogWidth"].ToString(null);
            dialogHeight = data["DialogHeight"].ToString(null);

            // Set on-click for JavaScript type
            if (uiNew.ElementType == UIElementTypeEnum.Javascript)
            {
                onClick = url;
                url = String.Empty;
            }
            else
            {
                // For URL append dialog hash if needed
                url = UIContextHelper.AppendDialogHash(UIContext, url);
            }
        }

        // If url is non empty add action
        if (((url != String.Empty) || (onClick != String.Empty)) && (HeaderActions != null))
        {
            HeaderActions.AddAction(new HeaderAction()
            {
                Text = GetString(text),
                RedirectUrl = url,
                Enabled = enabled,
                OnClientClick = onClick,
                OpenInDialog = openInDialog,
                DialogWidth = dialogWidth,
                DialogHeight = dialogHeight
            });
        }
    }

    #endregion
}