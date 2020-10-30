using System;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.OnlineMarketing;

public partial class CMSFormControls_Selectors_ConversionSelector : FormEngineUserControl
{
    public override object Value 
    {
        get
        {
            return drpConversions.SelectedValue;
        }
        set
        {
            drpConversions.SelectedValue = value as string;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!StopProcessing)
        {
            InitializeConversionSelector();
        }
    }


    /// <summary>
    /// Initializes selector with available conversion types ordered by the conversion display name.
    /// </summary>
    private void InitializeConversionSelector()
    {
        if (drpConversions.Items.Count == 0)
        {
            drpConversions.Items.Add(new ListItem(ResHelper.GetString("general.selectall"), string.Empty));

            var items = ABTestConversionDefinitionRegister.Instance.Items
                .Select(i => new ListItem(ResHelper.LocalizeString(i.ConversionDisplayName), i.ConversionName.ToLowerInvariant()))
                .OrderBy(i => i.Text)
                .ToArray();
            drpConversions.Items.AddRange(items);
        }
    }
}
