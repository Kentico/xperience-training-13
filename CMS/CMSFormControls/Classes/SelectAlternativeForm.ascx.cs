using System;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;


public partial class CMSFormControls_Classes_SelectAlternativeForm : FormEngineUserControl, ICallbackEventHandler
{
    private string _callbackArg = string.Empty;

    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            txtName.Enabled = value;
            btnSelect.Enabled = value;
        }
    }


    ///<summary>Gets or sets field value.</summary>
    public override object Value
    {
        get
        {
            return txtName.Text;
        }
        set
        {
            txtName.Text = (string)value;
        }
    }


    /// <summary>
    /// Gets ClientID of the textbox with transformation name.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return txtName.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets Class name.
    /// </summary>
    public string ClassName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["className"], string.Empty);
        }
        set
        {
            ViewState["className"] = (object)value;
        }
    }

    #endregion


    #region "CallBackHandling"

    public void RaiseCallbackEvent(string eventArgument)
    {
        _callbackArg = eventArgument;
    }


    public string GetCallbackResult()
    {
        return Validate(_callbackArg, true);
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        string argument = "document.getElementById('" + txtName.ClientID + "').value";
        string clientCallback = "CheckAlternativeForm";
        string CallbackRef = Page.ClientScript.GetCallbackEventReference(this, argument, clientCallback, "'" + lblStatus.ClientID + "'");
        txtName.Attributes["onchange"] = String.Format("javascript:{0}", CallbackRef);

        btnSelect.Text = GetString("general.select");
        btnSelect.OnClientClick = "SelectAltFormDialog_" + ClientID + "(); return false;";
        btnClear.Text = GetString("general.clear");
        btnClear.OnClientClick = string.Format("US_SetVal('{0}', ''); return false;", txtName.ClientID);
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        string classNames = string.Empty;
        string javaScript = string.Empty;
        string classNameParam = string.Empty;

        if ((Form != null) && (Form.Data != null))
        {
            object classNameObj = Form.Data.GetValue("ClassNames");
            if (classNameObj != null)
            {
                classNames = classNameObj.ToString();
            }
        }

        string url = "~/CMSFormControls/Selectors/AlternativeFormSelection.aspx?lblElem=" + lblStatus.ClientID + "&txtElem=" + txtName.ClientID;

        // Preselect class
        if (!string.IsNullOrEmpty(ClassName))
        {
            classNameParam = ClassName;
        }
        else if (!string.IsNullOrEmpty(classNames))
        {
            string[] splitClassNames = classNames.Split(';');
            classNameParam = splitClassNames[0];
        }

        if (!string.IsNullOrEmpty(classNameParam))
        {
            url += "&classname=" + classNameParam;
        }

        url += "&hash=" + QueryHelper.GetHash(url, false);
        javaScript = "function  SelectAltFormDialog_" + ClientID + "(){modalDialog('" + ResolveUrl(url) + "','AltFormSelection', 680, 310); return false;}";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "SelectDialog_" + ClientID, ScriptHelper.GetScript(javaScript));

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "SelectAlternativeForm", ScriptHelper.GetScript(
            "function SelectAltForm(formName,txtClientID,lblClientID){if((lblClientID != '') && (txtClientID != '')) { document.getElementById(txtClientID).value = formName;document.getElementById(lblClientID).innerHTML='';} return false;} " +
            "function CheckAlternativeForm(result, context){document.getElementById(context).innerHTML = result; return false; } "
                                                                                                  ));

        ScriptHelper.RegisterDialogScript(Page);
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        string ValidationResult = Validate(txtName.Text, false);
        if (ValidationResult == string.Empty)
        {
            return true;
        }
        else
        {
            ValidationError = ValidationResult;
            return false;
        }
    }


    private string Validate(string value, bool allowPreselect)
    {
        if (!string.IsNullOrEmpty(value))
        {
            // If alternative form name contains macro or is not full name, it is always valid
            if (MacroProcessor.ContainsMacro(value) || !ValidationHelper.IsFullName(value))
            {
                return string.Empty;
            }

            // Try to get alternative form object
            AlternativeFormInfo afi = AlternativeFormInfoProvider.GetAlternativeFormInfo(value);
            if (afi == null)
            {
                if (allowPreselect)
                {
                    // Alternative form does not exist
                    DataClassInfo di = DataClassInfoProvider.GetDataClassInfo(value);
                    if ((di == null) && (value != string.Empty))
                    {
                        return GetString("altform.selectaltform.notexist").Replace("%%code%%", value);
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
                else
                {
                    return GetString("altforms.selectaltform.formnotexist").Replace("%%code%%", value);
                }
            }
        }

        return string.Empty;
    }
}