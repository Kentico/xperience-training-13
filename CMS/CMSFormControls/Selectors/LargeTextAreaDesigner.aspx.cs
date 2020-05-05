using System;
using System.Collections;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.UIControls;


/// <summary>
/// Dialog page that extends LargeTextArea form control and provides syntax highlihgting and macros support.
/// </summary>
public partial class CMSFormControls_Selectors_LargeTextAreaDesigner : MessagePage
{
    protected Hashtable mParameters;

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
    /// ID of base editor control
    /// </summary>
    protected string EditorId
    {
        get
        {
            if (Parameters != null)
            {
                return ValidationHelper.GetString(Parameters["editorid"], string.Empty);
            }
            return string.Empty;
        }
    }
    
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!QueryHelper.ValidateHash("hash") || Parameters == null)
        {
            return;
        }

        txtText.Editor.Language = LanguageEnum.HTMLMixed;
        txtText.Editor.FullScreenParentElementID = "divContent";

        // Set window title and image
        PageTitle.TitleText = GetString("EditingFormControl.TitleText");
        // Set macro options using the querystring argument
        bool allowMacros = ValidationHelper.GetBoolean(Parameters["allowmacros"], true);
        txtText.Editor.ShowInsertMacro = allowMacros;
        if (allowMacros)
        {
            string resolverName = ValidationHelper.GetString(Parameters["resolvername"], string.Empty);
            MacroResolver resolver = MacroResolverStorage.GetRegisteredResolver(resolverName);
            txtText.Resolver = resolver;
        }

        // Register macro scripts
        RegisterModalPageScripts();
        RegisterEscScript();
    }

    #endregion
}