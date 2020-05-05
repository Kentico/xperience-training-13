using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_Controls_UI_Contact_MappingDialog : CMSUserControl
{
    #region "Variables"

    private Hashtable customControls = null;
    private string mClassName = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {

            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Indicates if control is enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return pnlGeneral.Enabled;
        }
        set
        {
            pnlGeneral.Enabled = value;
            pnlCustom.Enabled = value;
            pnlPersonal.Enabled = value;
            pnlAddress.Enabled = value;
            chkOverwrite.Enabled = value;
        }
    }


    /// <summary>
    /// Name of a class that should be mapped to the contact.
    /// </summary>
    private string ClassName
    {
        get
        {
            if (string.IsNullOrEmpty(mClassName))
            {
                mClassName = ValidationHelper.GetString(GetValue("classname"), string.Empty);
            }
            return mClassName;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns the value of the given property.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    public override object GetValue(string propertyName)
    {
        switch (propertyName.ToLowerCSafe())
        {
            case "mappingdefinition":
                return GetMappingDefinition();

            case "allowoverwrite":
                return chkOverwrite.Checked;

            default:
                return base.GetValue(propertyName);
        }
    }


    /// <summary>
    /// Sets the property value of the control, setting the value affects only local property value.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <param name="value">Value</param>
    public override bool SetValue(string propertyName, object value)
    {
        switch (propertyName.ToLowerCSafe())
        {
            // Set the ability to overwrite contact info
            case "allowoverwrite":
                chkOverwrite.Checked = ValidationHelper.GetBoolean(value, false);
                break;

            case "enabled":
                Enabled = ValidationHelper.GetBoolean(value, true);
                break;

            default:
                base.SetValue(propertyName, value);
                break;
        }

        return true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Enabled)
        {
            ShowInformation(GetString("class.contactmapping"));
        }

        // Check if 'om.contact' class has any custom fields
        FormInfo contactFormInfo = FormHelper.GetFormInfo("om.contact", false);
        if (contactFormInfo != null)
        {
            var fields = contactFormInfo.GetFormElements(true, false, true);
            if (fields.Any())
            {
                pnlCustom.Visible = true;
                // Add contact's custom fields to the dialog
                AddCustomFields(fields);
            }
        }

        if (!RequestHelper.IsPostBack())
        {
            // Initialize controls
            InitializeControls();
        }
    }


    /// <summary>
    /// Initializes controls on the page.
    /// </summary>
    protected void InitializeControls()
    {
        DataClassInfo classInfo = DataClassInfoProvider.GetDataClassInfo(ClassName);
        if (classInfo == null)
        {
            return;
        }

        // Set class names
        fldAddress1.ClassName = ClassName;
        fldBirthday.ClassName = ClassName;
        fldBusinessPhone.ClassName = ClassName;
        fldCity.ClassName = ClassName;
        fldCompanyName.ClassName = ClassName;
        fldCountry.ClassName = ClassName;
        fldEmail.ClassName = ClassName;
        fldFirstName.ClassName = ClassName;
        fldGender.ClassName = ClassName;
        fldJobTitle.ClassName = ClassName;
        fldLastName.ClassName = ClassName;
        fldMiddleName.ClassName = ClassName;
        fldMobilePhone.ClassName = ClassName;
        fldState.ClassName = ClassName;
        fldZip.ClassName = ClassName;

        if (!string.IsNullOrEmpty(classInfo.ClassContactMapping))
        {
            // Prepare form info based on mapping data
            FormInfo mapInfo = new FormInfo(classInfo.ClassContactMapping);
            if (mapInfo.ItemsList.Count > 0)
            {
                FormEngineUserControl customControl = null;
                // Get all mapped fields
                var fields = mapInfo.GetFields(true, true);

                // Name property contains a column of contact object
                // and MappedToField property contains form field mapped to the contact column
                foreach (FormFieldInfo ffi in fields)
                {
                    // Set mapping values...
                    switch (ffi.Name.ToLowerCSafe())
                    {
                        case "contactaddress1":
                            // ... Address1
                            fldAddress1.Value = ffi.MappedToField;
                            break;
                        case "contactbirthday":
                            // ... birthday
                            fldBirthday.Value = ffi.MappedToField;
                            break;
                        case "contactbusinessphone":
                            // ... business phone
                            fldBusinessPhone.Value = ffi.MappedToField;
                            break;
                        case "contactcity":
                            // ... city
                            fldCity.Value = ffi.MappedToField;
                            break;
                        case "contactcountryid":
                            // ... country
                            fldCountry.Value = ffi.MappedToField;
                            break;
                        case "contactcompanyname":
                            // ... company name
                            fldCompanyName.Value = ffi.MappedToField;
                            break;
                        case "contactemail":
                            // ... email
                            fldEmail.Value = ffi.MappedToField;
                            break;
                        case "contactfirstname":
                            // ... first name
                            fldFirstName.Value = ffi.MappedToField;
                            break;
                        case "contactgender":
                            // ... gender
                            fldGender.Value = ffi.MappedToField;
                            break;
                        case "contactjobtitle":
                            // ... job title
                            fldJobTitle.Value = ffi.MappedToField;
                            break;
                        case "contactlastname":
                            // ... last name
                            fldLastName.Value = ffi.MappedToField;
                            break;
                        case "contactmiddlename":
                            // ... middle name
                            fldMiddleName.Value = ffi.MappedToField;
                            break;
                        case "contactmobilephone":
                            // ... mobile phone
                            fldMobilePhone.Value = ffi.MappedToField;
                            break;
                        case "contactstateid":
                            // ... state
                            fldState.Value = ffi.MappedToField;
                            break;
                        case "contactzip":
                            // ... ZIP
                            fldZip.Value = ffi.MappedToField;
                            break;
                        default:
                            // ... contact's custom fields
                            if (customControls != null)
                            {
                                customControl = (FormEngineUserControl)customControls[ffi.Name];
                                if (customControl != null)
                                {
                                    customControl.Value = ffi.MappedToField;
                                }
                            }
                            break;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Adds contact's custom fields to the dialog.
    /// </summary>
    /// <param name="fields">Array list with custom fields (FormFieldInfo)</param>
    protected void AddCustomFields(IEnumerable<IDataDefinitionItem> fields)
    {
        CMSModules_AdminControls_Controls_Class_ClassFields fieldControl = null;

        // Initialize hashtable that will store controls for custom fields
        customControls = new Hashtable();

        // Add form to the placeholder 'Custom'
        Panel form = new Panel { CssClass = "form-horizontal" };
        plcCustom.Controls.Add(form);

        FormFieldInfo field;

        foreach (IDataDefinitionItem item in fields)
        {
            if (!(item is FormFieldInfo))
            {
                continue;
            }
            field = item as FormFieldInfo;

            Panel formGroup = new Panel { CssClass = "form-group" };
            form.Controls.Add(formGroup);

            // Create label div
            Panel labelPanel = new Panel { CssClass = "editing-form-label-cell" };
            formGroup.Controls.Add(labelPanel);

            // Create label
            LocalizedLabel label = new LocalizedLabel();
            label.Text = ResHelper.LocalizeString(field.GetDisplayName(MacroContext.CurrentResolver));
            label.EnableViewState = false;
            label.DisplayColon = true;
            label.CssClass = "control-label";
            labelPanel.Controls.Add(label);

            // Create value div
            Panel valuePanel = new Panel { CssClass = "editing-form-value-cell" };
            formGroup.Controls.Add(valuePanel);

            // Create control
            fieldControl = (CMSModules_AdminControls_Controls_Class_ClassFields)Page.LoadUserControl("~/CMSModules/AdminControls/Controls/Class/ClassFields.ascx");
            fieldControl.ID = "fld" + field.Name;
            fieldControl.ClassName = ClassName;
            fieldControl.FieldDataType = field.DataType;
            valuePanel.Controls.Add(fieldControl);

            // Store the control to hashtable
            customControls.Add(field.Name, fieldControl);
        }
    }


    /// <summary>
    /// Returns contact mapping definition.
    /// </summary>
    protected string GetMappingDefinition()
    {
        StringBuilder sb = new StringBuilder();
        string pattern = "<field column=\"{0}\" mappedtofield=\"{1}\" />";

        // Get mapped fields...
        if (!string.IsNullOrEmpty(fldAddress1.Value.ToString()))
        {
            // ... address1
            sb.AppendFormat(pattern, "ContactAddress1", fldAddress1.Value.ToString());
        }
        if (!string.IsNullOrEmpty(fldBirthday.Value.ToString()))
        {
            // ... birthday
            sb.AppendFormat(pattern, "ContactBirthday", fldBirthday.Value.ToString());
        }
        if (!string.IsNullOrEmpty(fldBusinessPhone.Value.ToString()))
        {
            // ... business phone
            sb.AppendFormat(pattern, "ContactBusinessPhone", fldBusinessPhone.Value.ToString());
        }
        if (!string.IsNullOrEmpty(fldCity.Value.ToString()))
        {
            // ... city
            sb.AppendFormat(pattern, "ContactCity", fldCity.Value.ToString());
        }
        if (!string.IsNullOrEmpty(fldCountry.Value.ToString()))
        {
            // ... country
            sb.AppendFormat(pattern, "ContactCountryID", fldCountry.Value.ToString());
        }
        if (!string.IsNullOrEmpty(fldCompanyName.Value.ToString()))
        {
            // ... company name
            sb.AppendFormat(pattern, "ContactCompanyName", fldCompanyName.Value.ToString());
        }
        if (!string.IsNullOrEmpty(fldEmail.Value.ToString()))
        {
            // ... email
            sb.AppendFormat(pattern, "ContactEmail", fldEmail.Value.ToString());
        }
        if (!string.IsNullOrEmpty(fldFirstName.Value.ToString()))
        {
            // ... first name
            sb.AppendFormat(pattern, "ContactFirstName", fldFirstName.Value.ToString());
        }
        if (!string.IsNullOrEmpty(fldGender.Value.ToString()))
        {
            // ... gender
            sb.AppendFormat(pattern, "ContactGender", fldGender.Value.ToString());
        }
        if (!string.IsNullOrEmpty(fldJobTitle.Value.ToString()))
        {
            // ... job title
            sb.AppendFormat(pattern, "ContactJobTitle", fldJobTitle.Value.ToString());
        }
        if (!string.IsNullOrEmpty(fldLastName.Value.ToString()))
        {
            // ... last name
            sb.AppendFormat(pattern, "ContactLastName", fldLastName.Value.ToString());
        }
        if (!string.IsNullOrEmpty(fldMiddleName.Value.ToString()))
        {
            // ... middle name
            sb.AppendFormat(pattern, "ContactMiddleName", fldMiddleName.Value.ToString());
        }
        if (!string.IsNullOrEmpty(fldMobilePhone.Value.ToString()))
        {
            // ... mobile phone
            sb.AppendFormat(pattern, "ContactMobilePhone", fldMobilePhone.Value.ToString());
        }
        if (!string.IsNullOrEmpty(fldState.Value.ToString()))
        {
            // ... state
            sb.AppendFormat(pattern, "ContactStateID", fldState.Value.ToString());
        }
        if (!string.IsNullOrEmpty(fldZip.Value.ToString()))
        {
            // ... ZIP
            sb.AppendFormat(pattern, "ContactZIP", fldZip.Value.ToString());
        }
        if (customControls != null)
        {
            // ... contact's custom fields
            FormEngineUserControl control = null;
            foreach (DictionaryEntry entry in customControls)
            {
                control = (FormEngineUserControl)entry.Value;
                if ((control != null) && (!string.IsNullOrEmpty(control.Value.ToString())))
                {
                    sb.AppendFormat(pattern, entry.Key, control.Value.ToString());
                }
            }
        }

        if (sb.Length > 0)
        {
            // Surround the mapping definition with 'form' element
            sb.Insert(0, "<form>");
            sb.Append("</form>");
        }

        return sb.ToString();
    }

    #endregion
}