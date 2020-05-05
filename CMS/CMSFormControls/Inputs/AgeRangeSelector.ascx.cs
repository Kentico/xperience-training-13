using System;

using CMS.FormEngine.Web.UI;


public partial class CMSFormControls_Inputs_AgeRangeSelector : FormEngineUserControl
{
    public override object Value
    {
        get
        {
            return txtBetween.Text + ";" + txtDays.Text;
        }
        set
        {
            string str = value as String;
            if (str != null)
            {
                string[] strs = str.Split(';');
                if (strs.Length == 2)
                {
                    txtBetween.Text = strs[0];
                    txtDays.Text = strs[1];
                }
            }
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
    }
}