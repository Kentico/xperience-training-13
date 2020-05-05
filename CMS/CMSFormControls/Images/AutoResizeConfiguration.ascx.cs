using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;


/// <summary>
/// This form control must be used with name 'autoresize' only. Another 3 blank form controls must be used in class to make it working properly. 
/// Blank form controls must have names: 'autoresize_width', 'autoresize_height', 'autoresize_maxsidesize', 'autoresize_hashtable'.
/// </summary>
public partial class CMSFormControls_Images_AutoResizeConfiguration : FormEngineUserControl, ICallbackEventHandler
{
    #region "Variables"

    private readonly XmlDocument xmlValue = new XmlDocument();
    protected string dimensions = string.Empty;
    private bool? mAutoresizeHashTable;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Indicates if values are saved in settings hash table or in xml format in value.
    /// </summary>
    private bool AutoresizeHashTable
    {
        get
        {
            if ((mAutoresizeHashTable == null) && (Form != null))
            {
                if (ContainsColumn("autoresize_hashtable"))
                {
                    object val = Form.GetDataValue("autoresize_hashtable");
                    if (val != null)
                    {
                        mAutoresizeHashTable = ValidationHelper.GetBoolean(val, false);
                    }
                }

                if (Form.FormInformation != null)
                {
                    FormFieldInfo ffi = Form.FormInformation.GetFormField("autoresize_hashtable");
                    if (ffi != null)
                    {
                        mAutoresizeHashTable = ValidationHelper.GetBoolean(ffi.DefaultValue, false);
                    }
                }
            }

            if (mAutoresizeHashTable == null)
            {
                mAutoresizeHashTable = false;
            }

            return mAutoresizeHashTable.Value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates if control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return drpSettings.Enabled;
        }
        set
        {
            base.Enabled = value;
            txtWidth.Enabled = value;
            txtHeight.Enabled = value;
            txtMax.Enabled = value;
            drpSettings.Enabled = value;
        }
    }


    /// <summary>
    /// DropDownList selected item value. Possible options: custom - for custom size, noresize - do not resize, (nothing) - use site settings.
    /// </summary>
    public override object Value
    {
        get
        {
            // Return data form hashtable or XML data
            return AutoresizeHashTable ? drpSettings.SelectedValue : UpdateConfiguration(xmlValue);
        }
        set
        {
            EnsureChildControls();

            string strValue = ValidationHelper.GetString(value, string.Empty);
            
            if (AutoresizeHashTable)
            {
                // Provided data are not in XML format
                drpSettings.SelectedValue = strValue;

                LoadOtherValues();
            }
            else
            {
                // Try to load data from XML
                try
                {
                    xmlValue.LoadXml(strValue);
                    LoadConfiguration(xmlValue);
                }
                catch
                {
                }
            }
        }
    }

    #endregion


    #region "Methods"

    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        if (!StopProcessing)
        {
            // Load drop-down list
            if ((!RequestHelper.IsPostBack()) || (drpSettings.Items.Count == 0))
            {
                drpSettings.Items.Add(new ListItem("dialogs.resize.donotresize", "noresize"));
                drpSettings.Items.Add(new ListItem("dialogs.resize.usesitesettings", string.Empty));
                drpSettings.Items.Add(new ListItem("dialogs.resize.usecustomsettings", "custom"));
            }
        }
        else
        {
            Visible = false;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (base.Enabled)
        {
            // Register scripts
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AutoResize_EnableDisableForm", GetScriptEnableDisableForm());
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AutoResize_ReceiveDimensions", GetScriptReceiveDimensions());
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AutoResize_LoadSiteSettings", ScriptHelper.GetScript("function GetDimensions(txtWidthID, txtHeightID, txtMaxID){ return " + Page.ClientScript.GetCallbackEventReference(this, "txtWidthID + ';' + txtHeightID + ';' + txtMaxID", "ReceiveDimensions", null) + " } \n"));

            drpSettings.Attributes.Add("onchange", GetEnableDisableFormDefinition());

            ScriptHelper.RegisterStartupScript(this, typeof(string), "EnableDisableFields", ScriptHelper.GetScript(GetEnableDisableFormDefinition()));
        }
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        // Try to load data from settings
        if (ContainsColumn("autoresize_width") && ContainsColumn("autoresize_height") && ContainsColumn("autoresize_maxsidesize"))
        {
            int width = ValidationHelper.GetInteger(Form.Data["autoresize_width"], 0);
            int height = ValidationHelper.GetInteger(Form.Data["autoresize_height"], 0);
            int maxSideSize = ValidationHelper.GetInteger(Form.Data["autoresize_maxsidesize"], 0);

            LoadConfiguration(drpSettings.SelectedValue, width, height, maxSideSize);
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        bool isValid = true;

        if (drpSettings.SelectedValue == "custom")
        {
            // Check if all required fields are defined
            if (AutoresizeHashTable)
            {
                if (!ContainsColumn("autoresize_width"))
                {
                    ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "autoresize_width", GetString("templatedesigner.fieldtypes.integer"));
                    isValid = false;
                }
                if (!ContainsColumn("autoresize_height"))
                {
                    ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "autoresize_height", GetString("templatedesigner.fieldtypes.integer"));
                    isValid = false;
                }
                if (!ContainsColumn("autoresize_maxsidesize"))
                {
                    ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "autoresize_maxsidesize", GetString("templatedesigner.fieldtypes.integer"));
                    isValid = false;
                }
            }

            // Validate width
            if (!string.IsNullOrEmpty(txtWidth.Text.Trim()))
            {
                if (ValidationHelper.GetInteger(txtWidth.Text.Trim(), 0) <= 0)
                {
                    isValid = false;
                }
            }

            // Validate height
            if (isValid && !string.IsNullOrEmpty(txtHeight.Text.Trim()))
            {
                if (ValidationHelper.GetInteger(txtHeight.Text.Trim(), 0) <= 0)
                {
                    isValid = false;
                }
            }

            // Validate max side size
            if (isValid && !string.IsNullOrEmpty(txtMax.Text.Trim()))
            {
                if (ValidationHelper.GetInteger(txtMax.Text.Trim(), 0) <= 0)
                {
                    isValid = false;
                }
            }

            if (!isValid)
            {
                // Display error message                   
                ValidationError += GetString("dialogs.resize.wrongformat");
            }
        }

        return isValid;
    }


    /// <summary>
    /// Returns other values related to this form control.
    /// </summary>
    /// <returns>Returns an array where first dimension is attribute name and the second dimension is its value.</returns>
    public override object[,] GetOtherValues()
    {
        // Save custom settings
        if (AutoresizeHashTable && (Value.ToString() == "custom"))
        {
            // Set properties names
            object[,] values = new object[3, 2];
            values[0, 0] = "autoresize_width";
            values[0, 1] = ValidationHelper.GetInteger(txtWidth.Text.Trim(), 0);
            values[1, 0] = "autoresize_height";
            values[1, 1] = ValidationHelper.GetInteger(txtHeight.Text.Trim(), 0);
            values[2, 0] = "autoresize_maxsidesize";
            values[2, 1] = ValidationHelper.GetInteger(txtMax.Text.Trim(), 0);

            return values;
        }
        return null;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Prepare javaScript for dimensions.
    /// </summary>
    private string GetScriptReceiveDimensions()
    {
        const string script = @"
function ReceiveDimensions(rValue, context){
    var dimensions = rValue.split(';');

    var width = (dimensions[0] > 0) ? dimensions[0]: '';
    var txtWidth = document.getElementById(dimensions[3]);
    if (txtWidth != null){
        txtWidth.value = width;
    }

    var height = (dimensions[1] > 0) ? dimensions[1]: '';
    var txtHeight = document.getElementById(dimensions[4]);
    if (txtHeight != null){
        txtHeight.value = height;
    }

    var max = (dimensions[2] > 0) ? dimensions[2]: '';
    var txtMax = document.getElementById(dimensions[5]);
    if (txtMax != null){
        txtMax.value = max;
    }

    txtWidth.disabled=true;
    txtHeight.disabled=true;
    txtMax.disabled=true;
}";
        return ScriptHelper.GetScript(script);
    }


    /// <summary>
    /// Prepare JavaScript for disabling form.
    /// </summary>
    /// <returns>Script that enables/disables form</returns>
    private string GetScriptEnableDisableForm()
    {
        const string script = @"
function EnableDisableForm(drpSettingsID, txtWidthID, txtHeightID, txtMaxID){
    var drpSettings = document.getElementById(drpSettingsID);
    var txtWidth = document.getElementById(txtWidthID);
    var txtHeight = document.getElementById(txtHeightID);
    var txtMax = document.getElementById(txtMaxID);

    if ((drpSettings != null) && (txtWidth != null) && (txtHeight != null) && (txtMax != null)){
        var disabled = (drpSettings.value != 'custom');
        txtWidth.disabled = disabled;
        txtHeight.disabled = disabled;
        txtMax.disabled = disabled;
        if (drpSettings.value == ''){
            GetDimensions(txtWidthID, txtHeightID, txtMaxID);
        }
        else if (drpSettings.value == 'noresize'){
            txtWidth.value = ''; txtHeight.value = ''; txtMax.value = '';
        }
    }
}";
        return ScriptHelper.GetScript(script);
    }


    /// <summary>
    /// Prepare control IDs.
    /// </summary>
    private string GetEnableDisableFormDefinition()
    {
        return string.Format("EnableDisableForm('{0}', '{1}', '{2}', '{3}');",
                             drpSettings.ClientID,
                             txtWidth.ClientID,
                             txtHeight.ClientID,
                             txtMax.ClientID);
    }


    /// <summary>
    /// Sets inner controls according to the parameters and their values included in configuration collection. Parameters collection will be passed from Field editor.
    /// </summary>    
    private void LoadConfiguration(string autoresize, int width, int height, int maxSideSize)
    {
        switch (autoresize.ToLowerCSafe())
        {
            case "noresize":
                width = 0;
                height = 0;
                maxSideSize = 0;
                break;

            // Use custom settings
            case "custom":
                break;

            // Use site settings
            default:
                string siteName = SiteContext.CurrentSiteName;
                width = ImageHelper.GetAutoResizeToWidth(siteName);
                height = ImageHelper.GetAutoResizeToHeight(siteName);
                maxSideSize = ImageHelper.GetAutoResizeToMaxSideSize(siteName);
                break;
        }

        SetDimension(txtWidth, width);
        SetDimension(txtHeight, height);
        SetDimension(txtMax, maxSideSize);
    }


    /// <summary>
    /// Sets inner controls according to the parameters and their values included in configuration collection.
    /// </summary>
    /// <param name="config">Parameters collection</param>
    private void LoadConfiguration(XmlDocument config)
    {
        int width = ValidationHelper.GetInteger(config["AutoResize"]["autoresize_width"].InnerText, 0);
        int height = ValidationHelper.GetInteger(config["AutoResize"]["autoresize_height"].InnerText, 0);
        int maxSideSize = ValidationHelper.GetInteger(config["AutoResize"]["autoresize_maxsidesize"].InnerText, 0);
        string autoresize = ValidationHelper.GetString(config["AutoResize"]["autoresize"].InnerText, string.Empty);

        drpSettings.SelectedValue = autoresize;
        LoadConfiguration(autoresize, width, height, maxSideSize);
    }


    /// <summary>
    /// Updates parameters collection of parameters and values according to the values of the inner controls.
    /// </summary>
    /// <param name="config">Parameters collection</param>
    private string UpdateConfiguration(XmlDocument config)
    {
        // Create proper XML structure
        config.LoadXml("<AutoResize><autoresize /><autoresize_width /><autoresize_height /><autoresize_maxsidesize /></AutoResize>");

        XmlNode nodeAutoresize = config.SelectSingleNode("AutoResize/autoresize");
        XmlNode nodeAutoresizeWidth = config.SelectSingleNode("AutoResize/autoresize_width");
        XmlNode nodeAutoresizeHeight = config.SelectSingleNode("AutoResize/autoresize_height");
        XmlNode nodeAutoresizeMax = config.SelectSingleNode("AutoResize/autoresize_maxsidesize");

        // Save custom settings
        if (drpSettings.SelectedValue == "custom")
        {
            nodeAutoresize.InnerText = "custom";

            if (!string.IsNullOrEmpty(txtWidth.Text.Trim()))
            {
                nodeAutoresizeWidth.InnerText = txtWidth.Text.Trim();
            }

            if (!string.IsNullOrEmpty(txtHeight.Text.Trim()))
            {
                nodeAutoresizeHeight.InnerText = txtHeight.Text.Trim();
            }

            if (!string.IsNullOrEmpty(txtMax.Text.Trim()))
            {
                nodeAutoresizeMax.InnerText = txtMax.Text.Trim();
            }
        }
        // Save no resize settings
        else if (drpSettings.SelectedValue == "noresize")
        {
            nodeAutoresize.InnerText = "noresize";
        }

        return config.ToFormattedXmlString(true);
    }


    /// <summary>
    /// Sets specified dimension to the specified text box.
    /// </summary>
    /// <param name="txt">Textbox the dimensions should be set to</param>
    /// <param name="dimension">Dimension to be set</param>
    private void SetDimension(TextBox txt, int dimension)
    {
        txt.Text = dimension > 0 ? dimension.ToString() : string.Empty;
    }

    #endregion


    #region "Callback handling"

    public string GetCallbackResult()
    {
        return dimensions;
    }


    public void RaiseCallbackEvent(string eventArgument)
    {
        // Get site settings
        string siteName = SiteContext.CurrentSiteName;
        int width = ImageHelper.GetAutoResizeToWidth(siteName);
        int height = ImageHelper.GetAutoResizeToHeight(siteName);
        int max = ImageHelper.GetAutoResizeToMaxSideSize(siteName);

        string[] IDs = eventArgument.Split(';');

        // Returns site settings back to the client
        dimensions = string.Format("{0};{1};{2};{3};{4};{5}", width, height, max, IDs[0], IDs[1], IDs[2]);
    }

    #endregion
}