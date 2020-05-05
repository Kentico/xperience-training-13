using System;

using CMS.FormEngine;
using CMS.FormEngine.Web.UI;


public partial class CMSModules_FormControls_FormControls_PresetFormControlSelector : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return formControlSelector.Value;
        }
        set
        {
            formControlSelector.Value = value;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        formControlSelector.ReturnColumnName = FormUserControlInfo.TYPEINFO.CodeNameColumn;
    }

    #endregion
}
