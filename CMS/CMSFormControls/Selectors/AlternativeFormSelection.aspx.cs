using System;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSFormControls_Selectors_AlternativeFormSelection : DesignerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Validate query params against hash value and check if control ID is not potential XSS threat
        string txtClientId = QueryHelper.GetString("txtelem", "");
        string lblClientId = QueryHelper.GetString("lblelem", "");
        Regex re = RegexHelper.GetRegex(@"[\w\d_$$]*");

        lblClass.AssociatedControlClientID = drpClass.DropDownSingleSelect.ClientID;

        if (!QueryHelper.ValidateHash("hash") || !re.IsMatch(txtClientId) || !re.IsMatch(lblClientId))
        {
            pnlUpdate.Visible = false;
            return;
        }

        if (!RequestHelper.IsPostBack() && drpClass.HasData)
        {
            // Try to preselect class from drop-down list
            string className = QueryHelper.GetString("classname", string.Empty);
            if (!String.IsNullOrEmpty(className))
            {
                var classInfo = DataClassInfoProvider.GetDataClassInfo(className);
                if (classInfo != null)
                {
                    drpClass.Value = classInfo.ClassID;
                }
            }

            // Load alternative forms for selected class
            LoadAltFormsList();
        }

        PageTitle.TitleText = GetString("altforms.selectaltform");
        SetSaveResourceString("general.select");

        SetJavascripts(txtClientId, lblClientId);
    }


    private void SetJavascripts(string txtClientId, string lblClientId)
    {
        var script = @"
function SelectCurrentAlternativeForm(txtClientId, lblClientId) {
    var lstAlternativeForms = document.getElementById('" + lstAlternativeForms.ClientID + @"');
    if (lstAlternativeForms.selectedIndex != -1) {
        wopener.SelectAltForm(lstAlternativeForms.options[lstAlternativeForms.selectedIndex].value, txtClientId, lblClientId);
        CloseDialog();
    }
    else {
        alert(document.getElementById('constNoSelection').value);
    }
}

function Cancel() {
    CloseDialog();
}";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AltFormSelection" + ClientID, script, true);

        SetSaveJavascript("SelectCurrentAlternativeForm('" + txtClientId + "','" + lblClientId + "'); return false;");
    }


    /// <summary>
    /// Fills alternative form list according to selection in class selector.
    /// </summary>
    private void LoadAltFormsList()
    {
        int formClassId = ValidationHelper.GetInteger(drpClass.Value, 0);
        var altForms = AlternativeFormInfoProvider.GetAlternativeForms()
                                    .Columns("FormName", "FormDisplayName", "FormClassID")
                                    .WhereEquals("FormClassID", formClassId)
                                    .OrderBy("FormName");

        lstAlternativeForms.Items.Clear();

        foreach (var alternativeForm in altForms)
        {
            if ((alternativeForm.FormDisplayName != String.Empty) && (alternativeForm.FormName != String.Empty))
            {
                lstAlternativeForms.Items.Add(new ListItem(ResHelper.LocalizeString(alternativeForm.FormDisplayName), alternativeForm.FullName));
            }
        }

        lstAlternativeForms.SelectedValue = null;
        lstAlternativeForms.DataBind();
    }
    

    protected void drpClass_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Load alternative forms for selected class
        LoadAltFormsList();
    }
}
