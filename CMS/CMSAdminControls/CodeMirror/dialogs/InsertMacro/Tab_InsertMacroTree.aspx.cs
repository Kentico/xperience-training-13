using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_CodeMirror_dialogs_InsertMacro_Tab_InsertMacroTree : CMSModalPage
{
    private string mEditorName;
    private bool mIsMixedMode = true;
    private string mResolverSessionKey;

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get key to the session-stored resolver name
        mResolverSessionKey = QueryHelper.GetString("resolversessionkey", "");
        
        if (!mResolverSessionKey.StartsWithCSafe("ConditionBuilderResolver_"))
        {
            mResolverSessionKey = null;
        }

        macroTree.ResolverName = ValidationHelper.GetString(SessionHelper.GetValue(mResolverSessionKey), "");
        mEditorName = QueryHelper.GetString("editorname", "");
        mIsMixedMode = QueryHelper.GetBoolean("ismixedmode", true);

        PageTitle.TitleText = GetString("insertmacro.dialogtitle");

        SetSaveResourceString("dialogs.actions.insert");

        Save += OnSave;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        string script = @"
function InsertMacro(macro) {
    if ((macro != null) && (macro != ''))
    {
        var mixedmode = " + mIsMixedMode.ToString().ToLowerCSafe() + @";
        if (wopener != null)
        {
            if (mixedmode) { macro = '{% ' + macro + ' %}' };
            var editor = wopener[" + ScriptHelper.GetString(mEditorName) + @"];
            if (editor != null) {
                editor.replaceSelection(macro);
                editor.focus();
            }
        }
        return CloseDialog();
    }
}";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "InsertMacroScript", script, true);
    }


    /// <summary>
    /// Cleans-up the session on Save event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">EventArgs</param>
    private void OnSave(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(mResolverSessionKey))
        {
            SessionHelper.Remove(mResolverSessionKey);
        }
    }
}
