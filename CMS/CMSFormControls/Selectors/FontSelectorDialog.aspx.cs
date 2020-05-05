using System;
using System.Drawing;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("fontselector.title")]
public partial class CMSFormControls_Selectors_FontSelectorDialog : CMSModalPage
{
    #region "Variables"

    private string mHiddenFieldId;
    private string mFontTypeId;

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        
        Save += btnOk_Click;

        mHiddenFieldId = Request.QueryString["hiddenId"];
        mFontTypeId = Request.QueryString["fontTypeId"];
        
        if (!RequestHelper.IsPostBack())
        {
            FillFontStyleListBox();

            string fontType = WindowHelper.GetItem(mHiddenFieldId) as string;
            if (fontType != null)
            {
                string[] fontParams = fontType.Split(new [] { ';' });
                if (fontParams.Length == 5)
                {
                    txtFontType.Text = fontParams[0];

                    lstFontStyle.SelectedValue = fontParams[1].ToLowerCSafe();

                    txtFontSize.Text = fontParams[2];

                    chkUnderline.Checked = fontParams[3].EqualsCSafe("underline", true);
                    chkStrike.Checked = fontParams[4].EqualsCSafe("strikethrought", true);
                }
            }

            FillFontSizeListBox();
            FillFontTypeListBox();
        }
        else
        {
            txtFontSize.Text = lstFontSize.SelectedValue;
            txtFontType.Text = lstFontType.SelectedValue;
        }

        ListItem li = lstFontStyle.SelectedItem;
        if (li != null)
        {
            txtFontStyle.Text = li.Text;
        }

        // Setup sample text
        lblSampleText.Font.Name = txtFontType.Text;
        lblSampleText.Font.Size = FontUnit.Point(ValidationHelper.GetInteger(txtFontSize.Text, 11));
        lblSampleText.Font.Strikeout = chkStrike.Checked;
        lblSampleText.Font.Underline = chkUnderline.Checked;
        if (txtFontStyle.Text.ToLowerCSafe().Contains("bold"))
        {
            lblSampleText.Font.Bold = true;
        }
        if (txtFontStyle.Text.ToLowerCSafe().Contains("italic"))
        {
            lblSampleText.Font.Italic = true;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        RegisterScriptCode();
        SetSaveJavascript("return validateInputs();");

        WindowHelper.Remove(mHiddenFieldId);
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        if ((txtFontSize.Text != String.Empty) && (txtFontStyle.Text != String.Empty) && (txtFontType.Text != String.Empty))
        {
            FontStyle fs = new FontStyle();
            switch (lstFontStyle.SelectedValue.ToLowerCSafe())
            {
                case "bold":
                    fs = FontStyle.Bold;
                    break;

                case "italic":
                    fs = FontStyle.Italic;
                    break;

                case "bolditalic":
                    fs = FontStyle.Bold | FontStyle.Italic;
                    break;

                case "regular":
                    fs = FontStyle.Regular;
                    break;
            }

            if (chkUnderline.Checked)
            {
                fs |= FontStyle.Underline;
            }

            if (chkStrike.Checked)
            {
                fs |= FontStyle.Strikeout;
            }

            if (!FontExists(fs))
            {
                lblError.Visible = true;
                lblError.Text = GetString("fontselector.unsupportedfont");
                return;
            }

            string ret = String.Format("{0};{1};{2};{3};{4}", txtFontType.Text, lstFontStyle.SelectedValue, txtFontSize.Text,
                (chkUnderline.Checked) ? "underline" : String.Empty, chkStrike.Checked ? "strikethrought" : String.Empty);

            string submitScript = String.Format("wopener.setParameters('{0}',{1},{2}); CloseDialog();", ret, ScriptHelper.GetString(mHiddenFieldId), ScriptHelper.GetString(mFontTypeId));
            ScriptHelper.RegisterStartupScript(Page, typeof(String), "SubmitScript", submitScript, true);
        }
    }

    #endregion


    #region "Methods"

    private void FillFontSizeListBox()
    {
        for (int i = 8; i < 80; i++)
        {
            lstFontSize.Items.Add(i.ToString());
        }

        var li = lstFontSize.Items.FindByValue(txtFontSize.Text);
        if (li != null)
        {
            li.Selected = true;
        }
    }


    private void FillFontTypeListBox()
    {
        foreach (FontFamily fontName in FontFamily.Families)
        {
            lstFontType.Items.Add(fontName.Name);
        }

        var li = lstFontType.Items.FindByValue(txtFontType.Text, true);
        if (li != null)
        {
            li.Selected = true;
        }
    }


    private void FillFontStyleListBox()
    {
        lstFontStyle.Items.Add(new ListItem(GetString("fontselector.regular"), "regular"));
        lstFontStyle.Items.Add(new ListItem(GetString("fontselector.bold"), "bold"));
        lstFontStyle.Items.Add(new ListItem(GetString("fontselector.italic"), "italic"));
        lstFontStyle.Items.Add(new ListItem(GetString("fontselector.bolditalic"), "bolditalic"));
    }


    private bool FontExists(FontStyle fontStyle)
    {
        try
        {
            new Font(txtFontType.Text, ValidationHelper.GetInteger(txtFontSize.Text, 10), fontStyle);
            return true;
        }
        catch
        {
            return false;
        }
    }


    private void RegisterScriptCode()
    {
        ScriptHelper.RegisterWOpenerScript(Page);

        string script = @"
var initDone = false;
var sampleTextInput, fontSizeInput, fontTypeInput, fontStyleInput, fontSizeList;

function initialize() {
    if (!initDone) {
        sampleTextInput = document.getElementById('" + lblSampleText.ClientID + @"');
        fontSizeInput = document.getElementById('" + txtFontSize.ClientID + @"');
        fontTypeInput = document.getElementById('" + txtFontType.ClientID + @"');
        fontStyleInput = document.getElementById('" + txtFontStyle.ClientID + @"');
        fontSizeList = document.getElementById('" + lstFontSize.ClientID + @"');

        initDone = true;
    }
}

function fontSizeChange(val) {
    initialize();

    fontSizeInput.value = val;
    sampleTextInput.style.fontSize = val + 'pt';
}

function fontTypeChange(val) {
    initialize();

    fontTypeInput.value = val;
    sampleTextInput.style.fontFamily = val;
}

function fontStyleChange(index,val) {
    initialize();

    fontStyleInput.value = val;

    switch (index) {
        case (0):
        {
            sampleTextInput.style.fontStyle = 'normal';
            sampleTextInput.style.fontWeight = 'normal';
            break;
        }
        case (1):
        {
            sampleTextInput.style.fontWeight = 'bold';
            sampleTextInput.style.fontStyle = 'normal';
            break;
        }
        case (2):
        {
            sampleTextInput.style.fontStyle = 'italic';
            sampleTextInput.style.fontWeight = 'normal';
            break;
        }
        default:
        {
            sampleTextInput.style.fontStyle = 'Italic';
            sampleTextInput.style.fontWeight = 'Bold';
            break;
        }
    }
}

function fontDecorationChange() {
    initialize();

    var checkedUnderline = document.getElementById('" + chkUnderline.ClientID + @"').checked;
    var checkedStrike = document.getElementById('" + chkStrike.ClientID + @"').checked;

    if (checkedUnderline && checkedStrike) {
        sampleTextInput.style.textDecoration = 'underline line-through';
    }
    else if (checkedUnderline) {
        sampleTextInput.style.textDecoration = 'underline';
    }
    else if (checkedStrike) {
        sampleTextInput.style.textDecoration = 'line-through';
    }
    else {
        sampleTextInput.style.textDecoration = 'none';
    }
}

function validateInputs() {
    initialize();

    var size = fontSizeInput.value;
    var type = fontTypeInput.value;
    var style = fontStyleInput.value;

    if (size == '' || type  == '' || style == '' ) {
        alert(" + ScriptHelper.GetLocalizedString("fontselector.notallvaluesfilled") + @");
        return false;
    }
    return true;
}

function sizeManualUpdate(tbInput) {
    initialize();

    var val = fontSizeInput.value;
    if (val != parseInt(val) || val == 0) {
        alert (" + ScriptHelper.GetLocalizedString("fontselector.wrongsize") + @");
        fontSizeInput.value = fontSizeList.options[fontSizeList.selectedIndex].value;
    }

    for (var i = 0; i < fontSizeList.length; ++i) {
        if (fontSizeList[i].value == val) {
            fontSizeList[i].selected = true;
        }
    }
    sampleTextInput.style.fontSize = val + 'px';
}";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(String), "FontSelectorDialogScripts", script, true);
    }

    #endregion
}