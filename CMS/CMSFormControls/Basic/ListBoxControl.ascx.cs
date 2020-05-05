using System.Web.UI.WebControls;

using CMS.FormEngine;
using CMS.FormEngine.Web.UI;


public partial class CMSFormControls_Basic_ListBoxControl : ListFormControl
{
    private const string DEFAULT_CSS_CLASS = "ListBoxField";

    private bool mSelectionModeSet;
    

    protected override ListControl ListControl => listbox;
    

    protected override ListSelectionMode SelectionMode
    {
        get
        {
            if (!mSelectionModeSet)
            {
                listbox.SelectionMode = GetValue("allowmultiplechoices", true) ? ListSelectionMode.Multiple : ListSelectionMode.Single;

                mSelectionModeSet = true;
            }

            return listbox.SelectionMode;
        }
    }


    protected override string FormControlName => FormFieldControlName.LISTBOX;
    

    protected override string DefaultCssClass => DEFAULT_CSS_CLASS;
}