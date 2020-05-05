using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title(Text = "{%GetResourceString(QueryString.addnumberres)%}")]
public partial class CMSModules_BannerManagement_Tools_Banner_HitClickAddModal : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string script = 
@"
function isInteger(value) {
    if ((undefined === value) || (null === value) || (value == '') || (parseInt(value) != value)) {
        return false;
    }
    return value % 1 == 0;
}

function add(txtControlID, validationControlID, numberControlID)
{
    var valueToAdd = $cmsj('#' + txtControlID).val();

    if (!isInteger(valueToAdd))
    {
        $cmsj('#' + validationControlID).show();
        return false;
    }
    wopener.addNumberHitsClicks(numberControlID, valueToAdd);
    return CloseDialog();
}
";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "add", script, true);
        ScriptHelper.RegisterJQuery(Page);

        lblFormatError.Style.Add("display","none");

        lblNumberToAdd.ResourceString = HTMLHelper.HTMLEncode(QueryHelper.GetString("numbertoaddres", ""));

        txtNumberToAdd.Attributes["onkeyup"] = string.Format("$cmsj('#{0}').hide();", lblFormatError.ClientID);

        string numberControlID = QueryHelper.GetString("numbercontrolid", "");

        // function addNumberHitsClicks is defined in HitsClicksControl.ascx
        SetSaveJavascript(string.Format("return add('{0}', '{1}', '{2}')", txtNumberToAdd.ClientID, lblFormatError.ClientID, numberControlID));

        SetSaveResourceString("general.addandclose");
    }
}
