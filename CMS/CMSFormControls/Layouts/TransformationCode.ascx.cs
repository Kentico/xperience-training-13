using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.UIControls;

using StreamWriter = CMS.IO.StreamWriter;
using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSFormControls_Layouts_TransformationCode : FormEngineUserControl, IPostBackEventHandler
{
    #region "Constants"

    /// <summary>
    /// Short link to help page regarding transformation methods.
    /// </summary>
    protected const string HELP_TOPIC_TRANSFORMATION_METHODS_LINK = "transformation_methods_ref";

    #endregion


    #region "Variables"

    private readonly int transformationID = QueryHelper.GetInteger("transformationid", 0);
    private TransformationInfo transformationInfo;
    private CurrentUserInfo user;
    private bool? mAscxEditAllowed;

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
    public String TransformationCode
    {
        get
        {
            if (IsAscx && !AscxEditAllowed)
            {
                // Ignore value from user input to avoid a forgery
                return transformationInfo.TransformationCode;
            }

            return (txtCode.Visible) ? txtCode.Text : tbWysiwyg.ResolvedValue;
        }
    }


    /// <summary>
    /// Returns true, if transformation type is ASCX
    /// </summary>
    public bool IsAscx
    {
        get
        {
            return (TransformationType == TransformationTypeEnum.Ascx);
        }
    }


    /// <summary>
    /// Returns transformation type
    /// </summary>
    public TransformationTypeEnum TransformationType
    {
        get
        {
            return drpType.SelectedValue.ToLowerCSafe().ToEnum<TransformationTypeEnum>();
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


    /// <summary>
    /// Returns true if currently authenticated user is authorized to edit ASCX code.
    /// </summary>
    private bool AscxEditAllowed
    {
        get
        {
            if (mAscxEditAllowed == null)
            {
                mAscxEditAllowed = user.IsAuthorizedPerResource("CMS.Design", "EditCode");
            }

            return mAscxEditAllowed.Value;
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

        String type = (drpType.SelectedValue == null ? TransformationTypeEnum.Ascx.ToStringRepresentation() : drpType.SelectedValue.ToLowerCSafe());

        // Ignore currently selected value for transformation type if the type equaled 'ASCX' originally and current user doesn't have permission to edit ASCX code.
        if (IsAscx && !AscxEditAllowed)
        {
            if ((transformationInfo != null) && (transformationID > 0) && (transformationInfo.TransformationType == TransformationTypeEnum.Ascx))
            {
                type = TransformationTypeEnum.Ascx.ToStringRepresentation();
            }
        }

        values[1, 1] = type;
        values[2, 0] = "TransformationCSS";
        values[2, 1] = txtCSS.Text;
        return values;
    }


    /// <summary>
    /// Checks whether XSLT transformation text is valid.
    /// </summary>
    /// <param name="xmlText">XML text</param>
    protected string XMLValidator(string xmlText)
    {
        // Creates memory stream from transformation text
        using (MemoryStream stream = new MemoryStream())
        using (StreamWriter writer = StreamWriter.New(stream))
        {
            writer.Write(xmlText);
            writer.Flush();
#pragma warning disable BH1014 // Do not use System.IO
            stream.Seek(0, SeekOrigin.Begin);
#pragma warning restore BH1014 // Do not use System.IO

            // New xml text reader from the stream
            using (XmlTextReader tr = new XmlTextReader(stream))
            {
                try
                {
                    // Need to read the data to validate
                    while (tr.Read())
                    {
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

                return string.Empty;
            }
        }
    }


    /// <summary>
    /// Initializes labels.
    /// </summary>
    private void LabelsInit()
    {
        // Initializes labels        
        string lang = DataHelper.GetNotEmpty(Service.Resolve<IAppSettingsService>()["CMSProgrammingLanguage"], "C#");
        lblDirectives.Text = string.Concat("&lt;%@ Control Language=\"", lang, "\" AutoEventWireup=\"true\" Inherits=\"CMS.DocumentEngine.Web.UI.CMSAbstractTransformation\" %&gt;<br />&lt;%@ Register TagPrefix=\"cc1\" Namespace=\"CMS.DocumentEngine.Web.UI\" Assembly=\"CMS.DocumentEngine.Web.UI\" %&gt;");
    }


    /// <summary>
    /// Returns true, if all entered values are valid
    /// </summary>
    public override bool IsValid()
    {
        if (TransformationType != TransformationTypeEnum.Xslt)
        {
            return true;
        }

        // Validates XSLT transformation text
        String result = XMLValidator(txtCode.Text);
        if (result != String.Empty)
        {
            ShowError(String.Format("{0}'{1}'", ScriptHelper.GetLocalizedString("DocumentType_Edit_Transformation_Edit.XSLTTransformationError"), result));
            return false;
        }

        return true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        string script = @"
function GenerateDefaultCode(type){
" + ControlsHelper.GetPostBackEventReference(this, "#").Replace("'#'", "type") + @"
}";
        ScriptHelper.RegisterStartupScript(this, typeof(string), "TransformationCodeGenerate", ScriptHelper.GetScript(script));

        pnlDirectives.Visible = IsAscx;
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
        user = MembershipContext.AuthenticatedUser;

        if (!RequestHelper.IsPostBack())
        {
            DropDownListInit();

            if (transformationInfo != null)
            {
                // Fills form with transformation information
                drpType.SelectedValue = transformationInfo.TransformationType.ToStringRepresentation();
                txtCSS.Text = transformationInfo.TransformationCSS;

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
                txtCode.Visible = true;
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

        LabelsInit();

        txtCode.Editor.Width = new Unit("99%");
        txtCode.Editor.Height = new Unit("300px");
        txtCode.NamespaceUsings = new List<string> { "Transformation" };

        // transformation.{classid}.{isascx}
        string resolverName = "transformation." + ClassID + "." + IsAscx;

        txtCode.ResolverName = resolverName;
        tbWysiwyg.ResolverName = resolverName;

        if (IsAscx)
        {
            DataClassInfo resolverClassInfo = DataClassInfoProvider.GetDataClassInfo(ClassID);
            if (resolverClassInfo != null)
            {
                if (resolverClassInfo.ClassIsCustomTable)
                {
                    txtCode.ASCXRootObject = CustomTableItem.New(resolverClassInfo.ClassName);
                }
                else if (resolverClassInfo.ClassIsDocumentType)
                {
                    txtCode.ASCXRootObject = TreeNode.New(resolverClassInfo.ClassName);
                }
                else
                {
                    txtCode.ASCXRootObject = ModuleManager.GetReadOnlyObjectByClassName(resolverClassInfo.ClassName);
                }
            }

            if (!RequestHelper.IsPostBack() && IsChecked)
            {
                ShowMessage();
            }
        }

        // Hide/Display CSS section
        plcCssLink.Visible = String.IsNullOrEmpty(txtCSS.Text.Trim());

        SetEditor();
    }


    private void GenerateDefaultTransformation(DefaultTransformationTypeEnum transformCode)
    {
        if (String.IsNullOrEmpty(ClassName))
        {
            ClassName = DataClassInfoProvider.GetClassName(ClassID);
        }

        var code = TransformationInfoProvider.GenerateTransformationCode(ClassName, TransformationType, transformCode);

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
        if (IsAscx)
        {
            txtCode.Editor.Language = LanguageEnum.ASPNET;

            // Check the edit code permission
            if (!AscxEditAllowed)
            {
                txtCode.Editor.Enabled = false;
            }
        }
        else
        {
            txtCode.Editor.Enabled = true;
            txtCode.Editor.Language = LanguageEnum.HTMLMixed;
        }
    }


    /// <summary>
    /// Display info message
    /// </summary>
    public void ShowMessage()
    {
        if (!IsAscx)
        {
            return;
        }

        // Check the edit code permission
        if (!AscxEditAllowed)
        {
            ShowWarning(GetString("EditCode.NotAllowed"));
        }
    }


    protected void drpTransformationType_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Get the current code
        string code = TransformationCode;

        switch (TransformationType)
        {
            case TransformationTypeEnum.Ascx:

                if (!AscxEditAllowed)
                {
                    // Ignore type change and reset transformation type selector
                    drpType.SelectedValue = transformationInfo.TransformationType.ToStringRepresentation();
                    ShowWarning(GetString("EditCode.NotAllowed"));
                    break;
                }

                // Convert to ASCX syntax
                code = MacroSecurityProcessor.RemoveSecurityParameters(code, false, null);
                code = code.Replace("{% Register", "<%@ Register").Replace("{%", "<%#").Replace("%}", "%>");
                ShowMessage();
                break;

            case TransformationTypeEnum.Xslt:
                // No transformation
                break;

            default:
                // Convert to macro syntax
                code = code.Replace("<%@", "{%").Replace("<%#", "{%").Replace("<%=", "{%").Replace("<%", "{%").Replace("%>", "%}");
                break;
        }

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
        drpType.Items.Add(new ListItem(TransformationTypeEnum.Ascx.ToLocalizedString("TransformationType"), TransformationTypeEnum.Ascx.ToStringRepresentation()));
        drpType.Items.Add(new ListItem(TransformationTypeEnum.Text.ToLocalizedString("TransformationType"), TransformationTypeEnum.Text.ToStringRepresentation()));
        drpType.Items.Add(new ListItem(TransformationTypeEnum.Html.ToLocalizedString("TransformationType"), TransformationTypeEnum.Html.ToStringRepresentation()));
        drpType.Items.Add(new ListItem(TransformationTypeEnum.Xslt.ToLocalizedString("TransformationType"), TransformationTypeEnum.Xslt.ToStringRepresentation()));
        drpType.Items.Add(new ListItem(TransformationTypeEnum.jQuery.ToLocalizedString("TransformationType"), TransformationTypeEnum.jQuery.ToStringRepresentation()));
    }

    #endregion


    #region "IPostBackEventHandler members"

    /// <summary>
    ///  Process postback action
    /// </summary>
    void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
    {
        switch (eventArgument.ToLowerCSafe())
        {
            case "xml":
                GenerateDefaultTransformation(DefaultTransformationTypeEnum.XML);
                break;
            case "atom":
                GenerateDefaultTransformation(DefaultTransformationTypeEnum.Atom);
                break;
            case "rss":
                GenerateDefaultTransformation(DefaultTransformationTypeEnum.RSS);
                break;
            default:
                GenerateDefaultTransformation(DefaultTransformationTypeEnum.Default);
                break;
        }
    }

    #endregion
}