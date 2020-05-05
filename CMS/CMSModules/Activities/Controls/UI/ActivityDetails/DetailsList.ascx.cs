using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Activities_Controls_UI_ActivityDetails_DetailsList : CMSUserControl
{
    #region "Variables"

    private readonly Panel mTable = new Panel { CssClass = "form-horizontal" };
    private bool mHideEmptyValues = true;
    

    #endregion


    #region "Public properties"

    /// <summary>
    /// If true, empty values is hidden/ignored.
    /// </summary>
    public bool HideEmptyValues
    {
        get
        {
            return mHideEmptyValues;
        }
        set
        {
            mHideEmptyValues = value;
        }
    }


    /// <summary>
    /// Returns true if the details list is not empty.
    /// </summary>
    public bool IsDataLoaded
    {
        get
        {
            return (mTable.Controls.Count > 0);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        plcTable.Controls.Add(mTable);
    }


    /// <summary>
    /// Adds new row to the table.
    /// </summary>
    /// <param name="label">Label text (resource string)</param>
    /// <param name="value">Value</param>
    /// <param name="encode">Indicates whether the value should be encoded</param>
    public void AddRow(string label, string value, bool encode = true)
    {
        if (HideEmptyValues && String.IsNullOrEmpty(value))
        {
            return;
        }

        if (encode)
        {
            value = HTMLHelper.HTMLEncode(value);
        }

        var row = new Panel
        {
            CssClass = "form-group"
        };

        mTable.Controls.Add(row);

        AddLabelCell(row, label);
        AddValueCell(row, value);
    }


    private static void AddLabelCell(Panel row, string labelText)
    {

        var labelCell = new Panel
        {
            CssClass = "editing-form-label-cell"
        };

        row.Controls.Add(labelCell);

        var labelControl = new LocalizedLabel
        {
            CssClass = "control-label",
            ResourceString = labelText,
            DisplayColon = true
        };

        labelCell.Controls.Add(labelControl);
    }


    private static void AddValueCell(Panel row, string valueText)
    {
        var valueCell = new Panel
        {
            CssClass = "editing-form-value-cell"            
        };

        row.Controls.Add(valueCell);

        // A block wrap because of block content
        var valueControl = new Panel
        {
            CssClass = "form-control-text"            
        };

        // Content may contain block HTML tags
        var localizedContent = new LocalizedLiteral
        {
            Text = valueText
        };

        valueControl.Controls.Add(localizedContent);

        valueCell.Controls.Add(valueControl);
        
    }

    #endregion
}