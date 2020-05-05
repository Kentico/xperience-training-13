using System;

using CMS.Helpers;

using System.Text;

using CMS.Base.Web.UI;
using CMS.UIControls;


[Title(Text = "Object browser")]
public partial class CMSAdminControls_UI_Macros_Dialogs_ObjectBrowser : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        editorElem.Editor.ShowToolbar = false;
        editorElem.MixedMode = false;
        editorElem.Editor.DynamicHeight = true;

        treeElem.VirtualMode = false;
        treeElem.DisplayValues = false;
        treeElem.DisplayObjectInRoot = true;
        
        // Load expression from query string
        if (!RequestHelper.IsPostBack())
        {
            var expr = QueryHelper.GetString("expr", "");
            if (!String.IsNullOrEmpty(expr))
            {
                editorElem.Text = expr;
            }

            var mode = QueryHelper.GetString("mode", "");
            switch (mode)
            {
                case "normal":
                    radNormalMode.Checked = true;
                    break;

                case "values":
                    radValues.Checked = true;
                    treeElem.DisplayValues = true;
                    break;

                default:
                    radVirtual.Checked = true;
                    treeElem.VirtualMode = true;
                    break;
            }
        }
        else
        {
            treeElem.VirtualMode = radVirtual.Checked;
            treeElem.DisplayValues = radValues.Checked;
        }

        treeElem.ContextResolver.Settings.CheckSecurity = false;

        treeElem.MacroExpression = editorElem.Text;
        treeElem.OnNodeClickHandler = "SelectItem";
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        string macro = editorElem.Text;

        txtOutput.Text = treeElem.ContextResolver.ResolveMacros("{%" + macro + "%}");

        StringBuilder sb = new StringBuilder();

        // Conditional addition of the dot operator (if not using the indexer parentheses) is handled with the replacement
        sb.Append(
@"
function SelectItem(macro) {
    var item = ('", ScriptHelper.GetString(macro, false), @".' + macro).replace('.[', '[');
    ", editorElem.Editor.EditorID, @".setValue(item);
}
");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "SelectItem", ScriptHelper.GetScript(sb.ToString()));
    }


    protected void btnClear_Click(object sender, EventArgs e)
    {
        editorElem.Text = "CMSContext.Current";
        treeElem.MacroExpression = editorElem.Text;
    }
}
