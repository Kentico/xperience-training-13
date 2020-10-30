using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


/// <summary>
/// This form control is used in layout UI to edit page layouts code.
/// </summary>
public partial class CMSFormControls_Layouts_PageLayoutCode : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets Page layout code.
    /// </summary>
    public override object Value
    {
        get
        {
            return tbLayoutCode.Text;
        }
        set
        {
            tbLayoutCode.Text = ValidationHelper.GetString(value, "");
        }
    }


    /// <summary>
    /// Name of the column with layout code.
    /// </summary>
    public string CodeColumn
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CodeColumn"), "");
        }
        set
        {
            SetValue("CodeColumn", value);
        }
    }


    /// <summary>
    /// Name of the column with layout type.
    /// </summary>
    public string TypeColumn
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TypeColumn"), "");
        }
        set
        {
            SetValue("TypeColumn", value);
        }
    }


    /// <summary>
    /// Returns ExtendedArea object for code editing.
    /// </summary>
    public ExtendedTextArea Editor
    {
        get
        {
            return tbLayoutCode.Editor;
        }
    }


    /// <summary>
    /// Enables or disables the control
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return drpType.Enabled;
        }
        set
        {
            drpElements.Enabled = value;
            btn.Enabled = value;
            drpType.Enabled = value;
            tbLayoutCode.ReadOnly = !value;
        }
    }


    /// <summary>
    /// Determines whether the code is in the fullscreen mode.
    /// </summary>
    public bool FullscreenMode
    {
        get;
        set;
    }

    #endregion


    #region "Page events

    public override object[,] GetOtherValues()
    {
        object[,] values = new object[2, 2];

        values[0, 0] = CodeColumn;
        values[0, 1] = tbLayoutCode.Text;
        values[1, 0] = TypeColumn;
        values[1, 1] = drpType.SelectedValue == null ? "ascx" : drpType.SelectedValue.ToLowerCSafe();

        return values;
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        string lang = DataHelper.GetNotEmpty(Service.Resolve<IAppSettingsService>()["CMSProgrammingLanguage"], "C#");
        ltlDirectives.Text = "&lt;%@ Control Language=\"" + lang + "\" Inherits=\"CMS.PortalEngine.Web.UI.CMSAbstractLayout\" %&gt;<br />&lt;%@ Register Assembly=\"CMS.PortalEngine.Web.UI\" Namespace=\"CMS.PortalEngine.Web.UI\" TagPrefix=\"cms\" %&gt;";

        var items = drpType.Items;
        if (items.Count == 0)
        {
            items.Add(new ListItem(GetString("TransformationType.ascx"), "ascx"));
            items.Add(new ListItem(TransformationTypeEnum.Html.ToLocalizedString("TransformationType"), TransformationTypeEnum.Html.ToStringRepresentation()));
        }
    }


    /// <summary>
    /// Ensures insert zone element items
    /// </summary>
    protected void InitZoneElements()
    {
        var items = drpElements.Items;

        items.Clear();

        // ASCX
        if (drpType.SelectedIndex == 0)
        {
            items.Add(new ListItem(GetString("PageLayout.ConditionalElement"), "cl"));
            items.Add(new ListItem(GetString("PageLayout.ZoneElement"), "wpz"));
            drpElements.SelectedIndex = 1;
        }
        // HTML
        else
        {
            items.Add(new ListItem(GetString("PageLayout.ZoneElement"), "wpzhtml"));
        }
    }


    protected void InitZoneElementsScript()
    {
        // Insert element script
        string script = @"
function InsertLayoutElement()
{
    var type = document.getElementById('" + drpElements.ClientID + @"').value;
    var cedit = " + tbLayoutCode.Editor.EditorID + @";
    var elem = '<cms:CMSWebPartZone ZoneID=""#"" runat=""server"" />';
    var idDefault = 'ZoneA'; 
    
    switch(type)
    {
        case 'wpzhtml':
            elem = '{^WebPartZone|(id)#^}';
            break;
        
        case 'cl':
            elem = '<cms:CMSConditionalLayout runat=""server"" ID=""#"" ></cms:CMSConditionalLayout>';
            idDefault = 'ConditionLayout';
            break;    

    }
    
    cedit.replaceSelection(elem.replace('#',idDefault)); 
    cedit.focus();
}
";
        ScriptHelper.RegisterStartupScript(this, typeof(string), "LayoutCodeInsertElement", ScriptHelper.GetScript(script));
        btn.OnClientClick = "InsertLayoutElement();return false;";
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!RequestHelper.IsPostBack())
        {
            LoadOtherValues();
        }

        if (FullscreenMode)
        {
            tbLayoutCode.TopOffset = 40;
        }

        InitZoneElements();
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        string type = ValidationHelper.GetString(Form.GetFieldValue(TypeColumn), "ascx").ToLowerCSafe();

        drpType.SelectedIndex = (type == "html") ? 1 : 0;

        tbLayoutCode.Text = ValidationHelper.GetString(Form.GetFieldValue(CodeColumn), "");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        bool isAscx = (drpType.SelectedValue.ToLowerCSafe() == "ascx");

        // Setup the information and code type
        if (isAscx)
        {
            tbLayoutCode.Editor.Language = LanguageEnum.ASPNET;
            tbLayoutCode.UseAutoComplete = false;
        }
        else
        {
            tbLayoutCode.Editor.Language = LanguageEnum.HTMLMixed;
            tbLayoutCode.UseAutoComplete = true;
        }

        plcDirectives.Visible = isAscx;

        InitZoneElementsScript();
    }

    #endregion
}