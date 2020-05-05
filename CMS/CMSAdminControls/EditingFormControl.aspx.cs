using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.UIControls;


public partial class CMSAdminControls_EditingFormControl : CMSModalPage
{
    #region "Variables"

    protected string selectorId = "";
    protected string controlPanelId = "";
    protected string selectorPanelId = "";
    protected Hashtable mParameters;
    private MacroResolver mMacroResolver = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Hashtable containing dialog parameters.
    /// </summary>
    protected Hashtable Parameters
    {
        get
        {
            if (mParameters == null)
            {
                string identifier = QueryHelper.GetString("params", null);
                mParameters = (Hashtable)WindowHelper.GetItem(identifier);
            }
            return mParameters;
        }
    }


    /// <summary>
    /// Default resolver.
    /// </summary>
    protected MacroResolver MacroResolver
    {
        get
        {
            if (mMacroResolver == null)
            {
                // Init resolver
                if (Parameters != null)
                {
                    string resolverName = ValidationHelper.GetString(Parameters["resolvername"], string.Empty);
                    mMacroResolver = MacroResolverStorage.GetRegisteredResolver(resolverName);
                    mMacroResolver.Settings.VirtualMode = true;
                }
            }

            return mMacroResolver;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!QueryHelper.ValidateHash("hash") || Parameters == null)
        {
            return;
        }       

        // Get  parameters
        selectorId = ValidationHelper.GetString(Parameters["selectorid"], string.Empty);
        controlPanelId = ValidationHelper.GetString(Parameters["controlpanelid"], string.Empty);
        selectorPanelId = ValidationHelper.GetString(Parameters["selectorpanelid"], string.Empty);

        // Initialize UI
        PageTitle.TitleText = GetString("EditingFormControl.TitleText");
        SetSaveJavascript("setValueToParent(" + ScriptHelper.GetString(selectorId) + ", " + ScriptHelper.GetString(controlPanelId) + ", " + ScriptHelper.GetString(selectorPanelId) + "); return CloseDialog();");

        ScriptHelper.RegisterClientScriptBlock(this.Page, typeof(string), "SetNestedControlValue", @"
function setValueToParent(selId, controlPanelId, selPanelId) {
    wopener.setNestedControlValue(selId, controlPanelId, trimNewLines(" + macroEditor.Editor.GetValueGetterCommand() + @"), selPanelId); 
}", true);



        macroEditor.Resolver = MacroResolver;
        macroEditor.MixedMode = true;
        macroEditor.Editor.Language = LanguageEnum.HTMLMixed;
        macroEditor.Editor.Width = new Unit("97%");
        macroEditor.Editor.Height = new Unit("310px");
        macroEditor.Editor.FullScreenParentElementID = "divContent";

        SetSaveResourceString("general.ok");

        ScriptHelper.RegisterDialogScript(Page);
    }


    protected override void Render(HtmlTextWriter writer)
    {
        base.Render(writer);
        writer.Write(ScriptHelper.GetScript(macroEditor.Editor.GetValueSetterCommand("wopener.getNestedControlValue('" + selectorId + "')")));
    }
}
