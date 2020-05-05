using System;
using System.Collections.Generic;

using CMS.Helpers;

using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;


public partial class CMSAdminControls_Basic_OrderByControl : FormEngineUserControl
{
    #region "Constants"

    // Control names
    private const string TXTCOLUMN_ID_PREFIX = "txtColumn";

    private const string DRPCOLUMN_ID_PREFIX = "drpColumn";

    private const string DRPDIRECTION_ID_PREFIX = "drpDirection";

    private const string LBLTHENBY_ID_PREFIX = "lblThenBy";

    private const string PNLORDERBY_ID_PREFIX = "pnlOrderBy";

    private const string SELECT_COLUMN = "##SELECTCOLUMN##";

    // Direction constants
    private const string ASC = "ASC";

    private const string DESC = "DESC";

    // JavaScript prefixes
    private const string SET_DIRECTION_TXT = "SetDirectionTxt";

    private const string SET_DIRECTION_DRP = "SetDirectionDrp";

    #endregion


    #region "Private variables"

    private List<ListItem> mColumns = new List<ListItem>();

    private readonly List<OrderByRow> orderByRows = new List<OrderByRow>();

    private IList<string> mValues;

    private IList<int> mControlIdentifiers;

    #endregion


    #region "Classes"

    /// <summary>
    /// Holds one 'ORDER BY' row.
    /// </summary>
    private class OrderByRow
    {
        /// <summary>
        /// Index of a row.
        /// </summary>
        public int Index
        {
            get;
            private set;
        }


        /// <summary>
        /// Label 'then by'.
        /// </summary>
        public LocalizedLabel ThenByLabel
        {
            get;
            private set;
        }


        /// <summary>
        /// Control for specifying column.
        /// </summary>
        public Control ColumnControl
        {
            get;
            private set;
        }


        /// <summary>
        /// Control for specifying sort direction.
        /// </summary>
        public CMSDropDownList DirectionDropDown
        {
            get;
            private set;
        }


        /// <summary>
        /// Wrapping panel.
        /// </summary>
        public Panel Panel
        {
            get;
            private set;
        }


        /// <summary>
        /// String representation of sorting column value.
        /// </summary>
        public string Column
        {
            get
            {
                // Get column name based on current column control
                if (ColumnControl.GetType() == typeof(TextBox))
                {
                    return ((TextBox)ColumnControl).Text.Trim();
                }
                
                if (ColumnControl.GetType() == typeof(CMSDropDownList))
                {
                    return ((CMSDropDownList)ColumnControl).SelectedValue.Trim();
                }
                
                return string.Empty;
            }
            set
            {
                if (ColumnControl.GetType() == typeof(TextBox))
                {
                    ((TextBox)ColumnControl).Text = value;
                }
                else if (ColumnControl.GetType() == typeof(CMSDropDownList))
                {
                     ((CMSDropDownList)ColumnControl).SelectedValue = value;
                }
            }
        }


        /// <summary>
        /// Default constructor for 'order by' row.
        /// </summary>
        /// <param name="index">Index of a row</param>
        /// <param name="thenByLabel">Label 'then by'</param>
        /// <param name="orderByControl">Control for specifying column</param>
        /// <param name="directionDropDown">Control for specifying sort direction</param>
        /// <param name="panel">Wrapping panel</param>
        public OrderByRow(int index, LocalizedLabel thenByLabel, Control orderByControl, CMSDropDownList directionDropDown, Panel panel)
        {
            Index = index;
            ThenByLabel = thenByLabel;
            ColumnControl = orderByControl;
            DirectionDropDown = directionDropDown;
            Panel = panel;
        }
    }

    #endregion


    #region "Enumerations"

    /// <summary>
    /// Determines which control to use when selecting columns
    /// </summary>
    public enum SelectorMode
    {
        TextBox,
        DropDownList
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Mode the control is currently working in.
    /// </summary>
    public SelectorMode Mode
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if delayed reload should be used.
    /// If True reload data must be called externally.
    /// </summary>
    public bool DelayedReload
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets columns. When columns are specified, control switches to dropdown list mode 
    /// allowing user to select from specified columns instead of typing them in.
    /// </summary>
    public List<ListItem> Columns
    {
        get
        {
            return mColumns;
        }
        set
        {
            mColumns = value;
        }
    }


    /// <summary>
    /// Array of column control indexes.
    /// </summary>
    private IList<int> ControlIdentifiers
    {
        get
        {
            return mControlIdentifiers;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            string returnValue = null;
            IEnumerable<OrderByRow> rows = from row in orderByRows
                                           where ((row.Column != SELECT_COLUMN) && !String.IsNullOrEmpty(row.Column))
                                           group row by row.Column
                                           into groupedRows
                                           let rowRecord = groupedRows.First()
                                           select rowRecord;
            // For all rows where column is specified and is distinct
            foreach (OrderByRow row in rows)
            {
                Panel panel = row.Panel;

                if (panel != null)
                {
                    string text = row.Column;
                    // Get direction dropdown list control
                    CMSDropDownList drpDirection = row.DirectionDropDown;
                    returnValue += text + " " + drpDirection.SelectedValue + ",";
                }
            }
            if (!string.IsNullOrEmpty(returnValue))
            {
                returnValue = returnValue.TrimEnd(',');
            }
            return returnValue;
        }
        set
        {
            string clauses = ValidationHelper.GetString(value, string.Empty);

            if (!string.IsNullOrEmpty(clauses))
            {
                Values = clauses.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                Values = null;
            }
        }
    }


    /// <summary>
    /// Array with values.
    /// </summary>
    private IList<string> Values
    {
        get
        {
            if (mValues == null)
            {
                return ValidationHelper.GetString(Value, string.Empty).Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
            return mValues;
        }
        set
        {
            mValues = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!DelayedReload)
        {
            ReloadData();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        
        // Make final adjustments of controls
        RefreshControls();
    }

    #endregion


    #region "Control events"

    protected void txtBox_TextChanged(object sender, EventArgs e)
    {
        // jQuery script for cursor position
        string script = "$cmsj('#" + ((TextBox)sender).ClientID + "').putCursorAtEnd()";
        ScriptHelper.RegisterStartupScript(this, typeof(string), "putCursorAtEnd", ScriptHelper.GetScript(script));
    }


    protected void drpOrderBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        CMSDropDownList drp = ((CMSDropDownList)sender);
        vals[drp.ID] = drp.SelectedValue;
    }


    private Dictionary<string, string> vals
    {
        get
        {
            Dictionary<string, string> x = (Dictionary<string, string>)ViewState["vals"];
            if (x == null)
            {
                x = new Dictionary<string, string>();
                ViewState["vals"] = x;
            }
            return x;
        }
    }

    #endregion


    #region "Methods"

    /// <summary> 
    /// Reloads control
    /// </summary>
    public void ReloadData()
    {
        SetupControl();
    }


    /// <summary>
    /// Add controls for saved values.
    /// </summary>
    private void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            orderByRows.Clear();
            plcOrderBy.Controls.Clear();
            mControlIdentifiers = Array.ConvertAll(hdnIndices.Value.Trim().Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries), StringToInt);
            hdnIndices.Value = string.Empty;
            int count;

            if (ControlIdentifiers.Count > 0)
            {
                count = ControlIdentifiers.Count;

                // Generate controls with used indexes
                for (int i = 0; i < count; i++)
                {
                    AddRow(ControlIdentifiers[i], (i != 0));
                }
            }
            else
            {
                // Reload controls based on Value property
                if (Values != null)
                {
                    count = Values.Count;

                    // Add controls with saved values
                    for (int i = 0; i < count; i++)
                    {
                        string line = Values[i].Trim();
                        string[] valuesArray = line.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (valuesArray.Length > 0)
                        {
                            string column = valuesArray[0];
                            string direction = (valuesArray.Length > 1) ? valuesArray[1] : ASC;
                            AddRow(i, column, direction, (i != 0));
                        }
                    }
                }
            }

            // Register JavaScripts
            ScriptHelper.RegisterJQuery(Page);
            // Register direction selectors
            StringBuilder sharedScript = new StringBuilder();
            sharedScript.Append(@"
function ", SET_DIRECTION_TXT, @"(txtColumnId)
{
    var txtColumn = $cmsj('#' + txtColumnId);
    var drpDirection = $cmsj(txtColumn).next();
    if(txtColumn.val() != '')
    {
        drpDirection.removeAttr('disabled');
    }
    else
    {
        drpDirection.attr('disabled', 'disabled');
    }
}
function ", SET_DIRECTION_DRP, @"(drpColumnId)
{
    var drpColumn = $cmsj('#' + drpColumnId);
    var drpDirection = $cmsj(drpColumn).next();
    if(drpColumn.val() != '", SELECT_COLUMN, @"')
    {
        drpDirection.removeAttr('disabled');
    }
    else
    {
        drpDirection.attr('disabled', 'disabled');
    }
}");
            // Register script for keeping focus and scroll position
            sharedScript.Append(@"
(function($) {
$.fn.putCursorAtEnd = function() {
return this.each(function() {
    $(this).focus();
    if (this.setSelectionRange) {
        var len = $(this).val().length * 2;
        this.setSelectionRange(len, len);
    }
    else {
        $(this).val($(this).val());
    }
    this.scrollTop = 999999;
});
};
})($cmsj);
window.orderBySelector = [];
");
            ScriptHelper.RegisterStartupScript(this, typeof(string), "orderBySelector", ScriptHelper.GetScript(sharedScript.ToString()));

            // Register function for automatic row addition
            StringBuilder controlScript = new StringBuilder();
            controlScript.Append(@"
$cmsj('#", pnlUpdate.ClientID, @"').find('input[id*=" + ((Mode == SelectorMode.TextBox) ? TXTCOLUMN_ID_PREFIX : DRPCOLUMN_ID_PREFIX) + @"]').each(function(){
$cmsj(this).keyup(function() {
    var parentID = $cmsj(this).parent('.orderBySelector').attr('id');
    clearTimeout(window.orderBySelector[parentID]);
    window.orderBySelector[parentID] = setTimeout('$cmsj(\'#' + this.id + '\').trigger(\'change\')',500);
})
.keydown(function() {
    var parentID = $cmsj(this).parent('.orderBySelector').attr('id');
    clearTimeout(window.orderBySelector[parentID]);
});
});");
            ScriptHelper.RegisterStartupScript(this, typeof(string), "orderBySelector" + ClientID, ScriptHelper.GetScript(controlScript.ToString()));
        }
    }


    /// <summary>
    /// Converts string to integer.
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <returns>Converted integer</returns>
    private int StringToInt(string value)
    {
        return ValidationHelper.GetInteger(value, 0);
    }


    /// <summary>
    /// Returns true if the returned value is valid.
    /// </summary>
    public override bool IsValid()
    {
        int i = 0;
        // Check errors in textbox
        foreach (OrderByRow row in orderByRows)
        {
            int index = row.Index;
            Panel panel = row.Panel;

            string text = row.Column;

            bool isLastRow = (orderByRows.IndexOf(row) == orderByRows.Count - 1);

            // Check whether entered value is an identifier
            if (!ValidationHelper.IsIdentifier(text) && !isLastRow)
            {
                LocalizedLabel lblError = new LocalizedLabel
                {
                    ResourceString = "orderbycontrol.notidentifier",
                    CssClass = "ErrorLabel",
                    EnableViewState = false,
                    ID = "lblError" + index
                };
                panel.Controls.Add(lblError);
                i++;
            }
        }

        return (i == 0);
    }


    /// <summary>
    /// Gets actual controls values for return value.
    /// </summary>
    private void RefreshControls()
    {
        List<OrderByRow> rowsToRemove = new List<OrderByRow>();

        // For all rows
        for (int index = 0; index < orderByRows.Count; index++)
        {
            OrderByRow row = orderByRows[index];
            Panel panel = row.Panel;

            if (panel != null)
            {
                string text = row.Column;

                // If value is specified
                if (String.IsNullOrEmpty(text) || (text == SELECT_COLUMN))
                {
                    bool isFirstRow = orderByRows.IndexOf(row) == 0;
                    int nextRowIndex = index + 1;
                    // If there are more rows
                    if (isFirstRow && (orderByRows.Count > nextRowIndex))
                    {
                        OrderByRow nextRow = orderByRows[nextRowIndex];
                        Panel nextPanel = nextRow.Panel;
                        if (nextPanel != null)
                        {
                            // Remove 'then by' label
                            LocalizedLabel label = nextRow.ThenByLabel;
                            nextPanel.Controls.Remove(label);
                        }
                    }

                    // Remove row from controls collection
                    plcOrderBy.Controls.Remove(panel);
                    // Remove its index
                    hdnIndices.Value = hdnIndices.Value.Replace(row.Index + ";", string.Empty);
                    // Remove reference to the row
                    rowsToRemove.Add(row);
                }
            }
        }

        // Remove previously selected (empty) rows
        foreach (OrderByRow row in rowsToRemove)
        {
            orderByRows.Remove(row);
        }

        // Get first free row index
        int newRowIndex = 0;
        while (orderByRows.FirstOrDefault(row => row.Index == newRowIndex) != null)
        {
            newRowIndex++;
        }
        // If other clauses can be specified
        if (!((Mode == SelectorMode.DropDownList) && (orderByRows.Count >= Columns.Count)))
        {
            // Add new empty line
            AddRow(newRowIndex, (orderByRows.Count != 0));
        }
    }


    /// <summary>
    /// Adds textbox/dropdown list determining sort column, dropdown list determining sort direction and 'then by' label
    /// </summary>
    /// <param name="i">Index of a control</param>
    /// <param name="addThenBy">Whether to add 'then by' label</param>
    private void AddRow(int i, bool addThenBy)
    {
        AddRow(i, string.Empty, string.Empty, addThenBy);
    }


    /// <summary>
    /// Clones array of list items.
    /// </summary>
    /// <param name="items">List item array</param>
    /// <returns>Copied array of list items</returns>
    public ListItem[] CloneItems(ListItem[] items)
    {
        ListItem[] newItems = null;
        if (items != null)
        {
            newItems = new ListItem[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                ListItem item = items[i];
                newItems[i] = new ListItem(item.Text, item.Value);
            }
        }
        return newItems;
    }


    /// <summary>
    /// Adds textbox/dropdown list determining sort column, dropdown list determining sort direction and 'then by' label
    /// </summary>
    /// <param name="i">Index of a control</param>
    /// <param name="sortColumn">Sort column</param>
    /// <param name="sortDirection">Sort direction</param>
    /// <param name="addThenBy">Whether to add 'then by' label</param>
    private void AddRow(int i, string sortColumn, string sortDirection, bool addThenBy)
    {
        hdnIndices.Value += i + ";";

        Panel pnlOrderBy = new Panel
        {
            ID = PNLORDERBY_ID_PREFIX + i
        };

        LocalizedLabel lblThenBy = null;
        if (addThenBy)
        {
            // Add 'Then by' label 
            lblThenBy = new LocalizedLabel
            {
                ResourceString = "orderbycontrol.thenby",
                CssClass = "ThenBy",
                EnableViewState = false,
                ID = LBLTHENBY_ID_PREFIX + i
            };
        }

        // Add dropdown list for setting direction
        CMSDropDownList drpDirection = new CMSDropDownList
        {
            ID = DRPDIRECTION_ID_PREFIX + i,
            CssClass = "ShortDropDownList",
            AutoPostBack = true,
            Enabled = Enabled
        };
        drpDirection.Items.Add(new ListItem(GetString("general.ascending"), ASC));
        drpDirection.Items.Add(new ListItem(GetString("general.descending"), DESC));
        if (!String.IsNullOrEmpty(sortDirection))
        {
            drpDirection.SelectedValue = sortDirection;
        }

        Control orderByControl = null;
        switch (Mode)
        {
                // Add textbox for column name
            case SelectorMode.TextBox:
                CMSTextBox txtColumn = new CMSTextBox
                {
                    ID = TXTCOLUMN_ID_PREFIX + i,
                    AutoPostBack = true,
                    Enabled = Enabled
                };
                txtColumn.TextChanged += txtBox_TextChanged;

                if (!String.IsNullOrEmpty(sortColumn))
                {
                    // Set sorting column
                    txtColumn.Text = sortColumn;
                }
                orderByControl = txtColumn;
                break;

                // Add dropdown list for column selection
            case SelectorMode.DropDownList:
                CMSDropDownList drpColumn = new CMSDropDownList
                {
                    ID = DRPCOLUMN_ID_PREFIX + i,
                    CssClass = "ColumnDropDown",
                    AutoPostBack = true,
                    Enabled = Enabled
                };
                drpColumn.SelectedIndexChanged += drpOrderBy_SelectedIndexChanged;
                drpColumn.Items.Add(new ListItem(GetString("orderbycontrol.selectcolumn"), SELECT_COLUMN));
                drpColumn.Items.AddRange(CloneItems(Columns.ToArray()));
                if (!String.IsNullOrEmpty(sortColumn))
                {
                    // Set sorting column
                    drpColumn.SelectedValue = sortColumn;
                }
                orderByControl = drpColumn;
                break;
        }

        // Add controls to panel
        if (lblThenBy != null)
        {
            pnlOrderBy.Controls.Add(WrapControl(lblThenBy));
        }
        if (orderByControl != null)
        {
            pnlOrderBy.Controls.Add(WrapControl(orderByControl));
        }
        pnlOrderBy.Controls.Add(WrapControl(drpDirection));

        // Add panel to place holder
        plcOrderBy.Controls.Add(pnlOrderBy);

        if (Enabled)
        {
            // Setup enable/disable script for direction dropdown list
            if (orderByControl is TextBox)
            {
                ((TextBox)orderByControl).Attributes.Add("onkeyup", SET_DIRECTION_TXT + "('" + orderByControl.ClientID + "')");
                ScriptHelper.RegisterStartupScript(this, typeof (string), "setEnabledTxt" + orderByControl.ClientID, ScriptHelper.GetScript("$cmsj(document).ready(function() {" + SET_DIRECTION_TXT + "('" + orderByControl.ClientID + "');})"));
            }
            else
            {
                ScriptHelper.RegisterStartupScript(this, typeof (string), "setEnabledDrp" + orderByControl.ClientID, ScriptHelper.GetScript("$cmsj(document).ready(function() {" + SET_DIRECTION_DRP + "('" + orderByControl.ClientID + "');})"));
            }
        }

        // Add row to collection
        orderByRows.Add(new OrderByRow(i, lblThenBy, orderByControl, drpDirection, pnlOrderBy));
    }


    /// <summary>
    /// Wraps the given control in panel.
    /// </summary>
    /// <param name="control">Control that will be wrapped.</param>
    private Control WrapControl(Control control)
    {
        Panel pnl = new Panel
        {
            ID = "pnlWrp" + control.ID,
            CssClass = "control-group-inline"
        };
        pnl.Controls.Add(control);

        return pnl;
    }

    #endregion
}
