using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_System_CodeNameWithPrefix : FormEngineUserControl
{

    #region "Public properties"

    /// <summary>
    /// Gets or sets string to separate prefix and class name
    /// </summary>
    public string Joiner
    {
        get
        {
            return GetValue("Joiner", ".");
        }
        set
        {
            SetValue("Joiner", value);
        }
    }


    /// <summary>
    /// Gets or sets predefined prefix.
    /// </summary>
    public string PredefinedPrefix
    {
        get
        {
            return GetValue("PredefinedPrefix", String.Empty);
        }
        set
        {
            SetValue("PredefinedPrefix", value);
        }
    }


    /// <summary>
    /// Indicates if prefix is editable
    /// </summary>
    public bool AllowEditPrefix
    {
        get
        {
            return GetValue("AllowEditPrefix", true);
        }
        set
        {
            SetValue("AllowEditPrefix", value);
        }
    }


    /// <summary>
    /// Indicates if additional info should be visible
    /// </summary>
    public bool ShowAdditionalInfo
    {
        get
        {
            return GetValue("ShowAdditionalInfo", true);
        }
        set
        {
            SetValue("ShowAdditionalInfo", value);
        }
    }


    /// <summary>
    /// Gets or sets control value
    /// </summary>
    public override object Value
    {
        get
        {
            string result = (AllowEditPrefix ? txtPrefix.Text : PredefinedPrefix) + Joiner + txtCodeName.Text;
            return (result != Joiner) ? result : String.Empty;
        }
        set
        {
            string codeName = ValidationHelper.GetString(value, String.Empty);
            if (!String.IsNullOrEmpty(codeName))
            {
                if (AllowEditPrefix)
                {
                    txtPrefix.Visible = true;
                    txtPrefix.Text = codeName.Substring(0, codeName.IndexOf(Joiner, StringComparison.Ordinal));
                }

                txtCodeName.Text = codeName.Substring(codeName.IndexOf(Joiner, StringComparison.Ordinal) + Joiner.Length);
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControls();
        }
    }


    /// <summary>
    /// Setups controls.
    /// </summary>
    private void SetupControls()
    {
        if (AllowEditPrefix)
        {
            txtPrefix.Visible = true;

            if (String.IsNullOrEmpty(txtPrefix.Text))
            {
                txtPrefix.Text = PredefinedPrefix;
            }
        }
        else
        {
            lblPrefix.Text = HTMLHelper.HTMLEncode(PredefinedPrefix);
            lblPrefix.Visible = true;
        }

        txtPrefix.Enabled = AllowEditPrefix;
        lblJoiner.Text = HTMLHelper.HTMLEncode(Joiner);

        if (ShowAdditionalInfo)
        {
            plcInfo.Visible = true;
            lblNamespace.ResourceString = "general.namespace";
            lblClass.ResourceString = "general.codename";
        }

        if ((FieldInfo != null) && (FieldInfo.AllowEmpty))
        {
            rfvCodeNameName.Visible = false;
            rfvCodeNameNamespace.Visible = false;
        }
        else
        {
            // Set up validation
            rfvCodeNameNamespace.ErrorMessage = GetString("sysdev.class_edit_gen.namespace");
            rfvCodeNameName.ErrorMessage = GetString("sysdev.class_edit_gen.name");
        }
    }


    /// <summary>
    /// Validates control input.
    /// </summary>
    public override bool IsValid()
    {
        string result = String.Empty;

        if (AllowEditPrefix)
        {
            result = new Validator().IsIdentifier(txtPrefix.Text.Trim(), GetString("general.NamespaceNameIdentifier")).Result + " ";
        }

        result += new Validator().IsIdentifier(txtCodeName.Text.Trim(), GetString("general.CodeNameIdentifier")).Result;

        if (!String.IsNullOrEmpty(result.Trim()))
        {
            ValidationError = result;
            return false;
        }

        return true;
    }

    #endregion
}