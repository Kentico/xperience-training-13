using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSFormControls_Layouts_TransformationCode : FormEngineUserControl, IPostBackEventHandler
{
    #region "Constants"

    /// <summary>
    /// Short link to help page regarding transformation methods.
    /// </summary>
    protected const string HELP_TOPIC_TRANSFORMATION_METHODS_LINK = "transformation_methods_ref";

    #endregion


    #region "Variables"

    private TransformationInfo transformationInfo;

    #endregion


    #region "Properties"

    /// <summary>
    /// Value of code control.
    /// </summary>
    public override object Value
    {
        get
        {
            return TransformationCode;
        }
        set
        {
            if (TransformationType == TransformationTypeEnum.Html)
            {
                tbWysiwyg.ResolvedValue = ValidationHelper.GetString(value, String.Empty);
            }
            else
            {
                txtCode.Text = ValidationHelper.GetString(value, String.Empty);
            }
        }
    }


    /// <summary>
    /// Name of the edited transformation
    /// </summary>
    public String TransformationName
    {
        get;
        set;
    }


    /// <summary>
    /// Transformation's class name
    /// </summary>
    public String ClassName
    {
        get;
        set;
    }


    /// <summary>
    /// Transformation's class ID
    /// </summary>
    public int ClassID
    {
        get;
        set;
    }


    /// <summary>
    /// Property returning transformation code (based on transformation type)
    /// </summary>
    public string TransformationCode => txtCode.Visible ? txtCode.Text : tbWysiwyg.ResolvedValue;


    /// <summary>
    /// Returns transformation type
    /// </summary>
    public TransformationTypeEnum TransformationType
    {
        get
        {
            return EnumStringRepresentationExtensions.ToEnum<TransformationTypeEnum>(drpType.SelectedValue.ToLowerCSafe());
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


    /// <summary>
    /// Returns true if object is checked out or use checkin/out is not used 
    /// </summary>
    public bool IsChecked
    {
        get
        {
            CMSObjectManager om = CMSObjectManager.GetCurrent(Page);
            if (om != null)
            {
                return om.IsObjectChecked();
            }

            return false;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        // Currently does not support loading other values explicitly
    }


    /// <summary>
    /// Returns additional object data
    /// </summary>
    public override object[,] GetOtherValues()
    {
        object[,] values = new object[3, 2];
        values[0, 0] = "TransformationCode";
        values[0, 1] = TransformationCode;
        values[1, 0] = "TransformationType";

        String type = (drpType.SelectedValue == null ? TransformationTypeEnum.Text.ToStringRepresentation() : drpType.SelectedValue.ToLowerCSafe());

        values[1, 1] = type;

        return values;
    }


    protected override void OnPreRender(EventArgs e)
    {
        string script = @"
function GenerateDefaultCode(type){
" + ControlsHelper.GetPostBackEventReference(this, "#").Replace("'#'", "type") + @"
}";
        ScriptHelper.RegisterStartupScript(this, typeof(string), "TransformationCodeGenerate", ScriptHelper.GetScript(script));

        txtCode.ReadOnly = !Enabled;
        tbWysiwyg.Enabled = Enabled;

        // Check whether virtual objects are allowed
        if (!SettingsKeyInfoProvider.VirtualObjectsAllowed)
        {
            ShowWarning(GetString("VirtualPathProvider.NotRunning"));
        }

        base.OnPreRender(e);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        transformationInfo = UIContext.EditedObject as TransformationInfo;

        if (!RequestHelper.IsPostBack())
        {
            DropDownListInit();

            if (transformationInfo != null)
            {
                // Fills form with transformation information
                drpType.SelectedValue = transformationInfo.TransformationType.ToStringRepresentation();

                if (transformationInfo.TransformationType == TransformationTypeEnum.Html)
                {
                    tbWysiwyg.ResolvedValue = transformationInfo.TransformationCode;
                    tbWysiwyg.Visible = true;
                }
                else
                {
                    txtCode.Text = transformationInfo.TransformationCode;
                    txtCode.Visible = true;
                }
            }
            else
            {
                tbWysiwyg.Visible = true;
            }
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        CMSMasterPage currentMaster = Page.Master as CMSMasterPage;

        if (FullscreenMode)
        {
            txtCode.TopOffset = 40;
        }

        // Check master page
        if (currentMaster == null)
        {
            throw new Exception("Page using this control must have CMSMasterPage master page.");
        }

        txtCode.Editor.Width = new Unit("99%");
        txtCode.Editor.Height = new Unit("300px");
        txtCode.NamespaceUsings = new List<string> { "Transformation" };

        // transformation.{classid}.{isascx}
        string resolverName = "transformation." + ClassID + "." + "false";

        txtCode.ResolverName = resolverName;
        tbWysiwyg.ResolverName = resolverName;

        SetEditor();
    }


    private void GenerateDefaultTransformation()
    {
        if (String.IsNullOrEmpty(ClassName))
        {
            ClassName = DataClassInfoProvider.GetClassName(ClassID);
        }

        var code = TransformationInfoProvider.GenerateTransformationCode(ClassName);

        // Writes the result to the text box
        if (TransformationType == TransformationTypeEnum.Html)
        {
            txtCode.Visible = false;
            tbWysiwyg.Visible = true;
            tbWysiwyg.ResolvedValue = code;
        }
        else
        {
            tbWysiwyg.Visible = false;
            txtCode.Visible = true;
            txtCode.Text = code;
        }
    }


    /// <summary>
    /// Sets code editor based on transformation type
    /// </summary>
    private void SetEditor()
    {
        txtCode.Editor.Enabled = true;
        txtCode.Editor.Language = LanguageEnum.HTMLMixed;
    }


    protected void drpTransformationType_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Get the current code
        string code = TransformationCode;

        code = code.Replace("<%@", "{%").Replace("<%#", "{%").Replace("<%=", "{%").Replace("<%", "{%").Replace("%>", "%}");

        // Move the content if necessary
        if (TransformationType == TransformationTypeEnum.Html)
        {
            // Move from text to WYSIWYG
            if (txtCode.Visible)
            {
                tbWysiwyg.ResolvedValue = code;
                tbWysiwyg.Visible = true;

                txtCode.Text = string.Empty;
                txtCode.Visible = false;
            }
        }
        else
        {
            // Move from WYSIWYG to text
            if (tbWysiwyg.Visible)
            {
                code = HttpUtility.HtmlDecode(code);

                txtCode.Text = code;
                txtCode.Visible = true;

                tbWysiwyg.ResolvedValue = string.Empty;
                tbWysiwyg.Visible = false;
            }
            else
            {
                txtCode.Text = code;
            }
        }

        SetEditor();
    }


    /// <summary>
    /// Initializes dropdown lists.
    /// </summary>
    private void DropDownListInit()
    {
        drpType.Items.Add(new ListItem(TransformationTypeEnum.Text.ToLocalizedString("TransformationType"), TransformationTypeEnum.Text.ToStringRepresentation()));
        drpType.Items.Add(new ListItem(TransformationTypeEnum.Html.ToLocalizedString("TransformationType"), TransformationTypeEnum.Html.ToStringRepresentation()));
    }

    #endregion


    #region "IPostBackEventHandler members"

    /// <summary>
    ///  Process postback action
    /// </summary>
    void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
    {
        GenerateDefaultTransformation();
    }

    #endregion
}