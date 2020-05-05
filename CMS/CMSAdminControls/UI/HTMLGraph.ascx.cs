using System;
using System.Collections;
using System.Data;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_UI_HTMLGraph : CMSUserControl
{
    // css styles
    private const string CSS_GRAPHBAR = "GraphBar";
    private const string CSS_GRAPHTEXT = "GraphText";
    private const string CSS_GRAPHBARSELBOX = "GraphBarSelectedBox";
    private const string CSS_GRAPHBARBOX = "GraphBarBox";

    // dataset columns
    private const string COL_URL = "url";
    private const string COL_VALUE = "value";
    private const string COL_TEXT = "text";
    private const string COL_TOOLTIP = "tooltip";
    private const string COL_SPAN = "span";
    private const string COL_URL2 = "url2";

    private DataSet mGraphData = new DataSet(); // Graph data in multiple tables
    private ArrayList mMaxValues = new ArrayList(); // Max value from dataset (1 value for 1 table)
    private ArrayList mMaxBarHeight = new ArrayList(); // Max bar height in pixels
    private ArrayList mCSSStyle = new ArrayList();
    private ArrayList mSelected = new ArrayList(); // Selected bar (1 value for 1 table)


    protected void Page_Load(object sender, EventArgs e)
    {
        ltlScript.Text = CreateScript();
    }


    /// <summary>
    /// ID of the selected bar.
    /// </summary>
    private string SelectedBarID
    {
        get
        {
            return ValidationHelper.GetString(ViewState["SelectedBarID"], "");
        }
        set
        {
            ViewState["SelectedBarID"] = value;
        }
    }


    /// <summary>
    /// Clears all graph datasets.
    /// </summary>
    public void ClearAllGraphs()
    {
        mGraphData.Clear();
        mMaxValues.Clear();
        mMaxBarHeight.Clear();
        mCSSStyle.Clear();
        mSelected.Clear();
    }


    /// <summary>
    /// Appends new graph dataset.
    /// </summary>
    public void AddGraph()
    {
        AddGraph(100, String.Empty);
    }


    /// <summary>
    /// Appends new graph dataset with CSS style.
    /// </summary>
    /// <param name="maxBarHeight">Bar height in pixels</param>
    /// <param name="cssStyle">Bar style</param>
    public void AddGraph(int maxBarHeight, string cssStyle)
    {
        mGraphData.Tables.Add();
        int lasttable = mGraphData.Tables.Count - 1;
        mGraphData.Tables[lasttable].Columns.Add(COL_VALUE);
        mGraphData.Tables[lasttable].Columns.Add(COL_TEXT);
        mGraphData.Tables[lasttable].Columns.Add(COL_TOOLTIP);
        mGraphData.Tables[lasttable].Columns.Add(COL_URL);
        mGraphData.Tables[lasttable].Columns.Add(COL_URL2);
        mGraphData.Tables[lasttable].Columns.Add(COL_SPAN, typeof(int));

        mMaxValues.Add(-1);
        mMaxBarHeight.Add(maxBarHeight);
        mCSSStyle.Add(cssStyle);
        mSelected.Add(-1);
    }


    /// <summary>
    /// Adds new data to last graph dataset.
    /// </summary>
    /// <param name="value">Current value</param>
    /// <param name="text">Text</param>
    /// <param name="tooltip">Tooltip</param>
    /// <param name="url">URL</param>
    /// <param name="span">Column span</param>
    public void AddGraphData(int value, string text, string tooltip, string url, string url2, int span)
    {
        if (mGraphData.Tables.Count == 0)
        {
            AddGraph();
        }
        int lasttable = mGraphData.Tables.Count - 1;
        mGraphData.Tables[lasttable].Rows.Add(value, ValidationHelper.GetString(text, ""),
                                              ValidationHelper.GetString(tooltip, ""),
                                              ValidationHelper.GetString(url, ""),
                                              ValidationHelper.GetString(url2, ""),
                                              span);

        if ((int)mMaxValues[lasttable] < value)
        {
            mMaxValues[lasttable] = value;
        }
    }


    /// <summary>
    /// Selects one graph bar.
    /// </summary>
    /// <param name="tableIndex">Graph index (usually 0)</param>
    /// <param name="barIndex">Bar index</param>
    /// <param name="select">State</param>
    public void SelectBar(int tableIndex, int barIndex, bool select)
    {
        if ((tableIndex < 0) || (barIndex < 0))
        {
            return;
        }
        if ((tableIndex < mGraphData.Tables.Count) && (barIndex < mGraphData.Tables[tableIndex].Rows.Count))
        {
            mSelected[tableIndex] = select ? barIndex : -1;
        }
    }


    /// <summary>
    /// Renders all graphs.
    /// </summary>
    public void RenderGraph()
    {
        int ratio, maxratio, barIndex, maxValIndex = 0;
        string colspan, tooltip, cssClass, barId;
        string strout = "";

        strout += "\n <table style=\"width:100%\" cellpadding=\"0\" cellspacing=\"1\" class=\"GraphTable\">";
        foreach (DataTable dt in mGraphData.Tables)
        {
            cssClass = "";
            if (mCSSStyle[maxValIndex].ToString().Length > 0)
            {
                cssClass = " class =\"" + mCSSStyle[maxValIndex].ToString() + "\" ";
            }

            maxratio = (int)mMaxBarHeight[maxValIndex];
            strout += "\n <tr" + cssClass + ">";
            barIndex = 0;
            foreach (DataRow dr in dt.Rows)
            {
                colspan = "";
                if (ValidationHelper.GetInteger(dr[COL_SPAN], 0) > 1)
                {
                    colspan = "colspan=" + dr[COL_SPAN];
                }
                strout += "\n <td " + colspan + " style=\"text-align: center;\">";

                int value = ValidationHelper.GetInteger(dr[COL_VALUE].ToString(), 0);
                ratio = value;
                bool isMax = (ratio == (int)mMaxValues[maxValIndex]);
                //Response.Write(((int)mMaxValues[maxValIndex]) + " ");
                if (isMax && (ratio > 0))
                {
                    ratio = maxratio - 3;
                }
                else
                {
                    ratio = (maxratio * ratio) / ((int)mMaxValues[maxValIndex] + 1);
                }

                tooltip = "";
                if (dr[COL_TOOLTIP].ToString().Length > 0)
                {
                    tooltip = "title=\"" + dr[COL_TOOLTIP].ToString() + "\" ";
                }

                // Create unique bar ID based on graph ID and table indexes
                barId = ID + "_" + barIndex + "_" + maxValIndex;

                if ((int)mSelected[maxValIndex] == barIndex)
                {
                    cssClass = CSS_GRAPHBARSELBOX;
                    SelectedBarID = barId;
                    //                    barId = " id=\"" + this.ID + "_" + GRAPHBAR_SEL_ID + "\" ";
                }
                else
                {
                    cssClass = CSS_GRAPHBARBOX;
                    //                    barId = "";
                }

                strout += "\n  <div  id=\"" + barId + "\" class=\"" + cssClass + "\" " + tooltip + "onclick=\"" + ID + "_RenderStyle(this); " + ID;
                strout += "_ShowDesktopContent('" + dr[COL_URL].ToString() + "', '" + dr[COL_URL2].ToString() + "')\" style=\"cursor: pointer;\">";
                if (!isMax || (value == 0))
                {
                    strout += "\n      <div style=\"height: " + (maxratio - ratio - 3) + "px;\"></div>";
                }
                strout += "\n      <div class=\"" + CSS_GRAPHBAR;
                strout += "\" style=\"height:" + (ratio + 1) + "px; line-height: 0px;\" >&nbsp;</div>";
                strout += "\n      <div class=\"" + CSS_GRAPHTEXT + "\" style=\"width: 20px;\">" + dr[COL_TEXT] + "</div>";
                strout += "\n  </div>";
                strout += "\n </td>";
                barIndex++;
            }
            strout += "\n </tr>";

            maxValIndex++;
        }
        strout += "\n </table>";
        pnlGraph.Controls.Add(new LiteralControl(strout));
    }


    /// <summary>
    /// Creates helper script.
    /// </summary>
    /// <returns>Script</returns>
    public string CreateScript()
    {
        string strout = "";
        strout += "\n var " + ID + "_oldItem = document.getElementById(\"" + SelectedBarID + "\");";
        strout += "\n function " + ID + "_RenderStyle(sender)";
        strout += "\n {";
        strout += "\n     if (" + ID + "_oldItem) {";
        strout += "\n         " + ID + "_oldItem.className = \"" + CSS_GRAPHBARBOX + "\";";
        strout += "\n         }";
        strout += "\n     sender.className = \"" + CSS_GRAPHBARSELBOX + "\"; ";
        strout += "\n     " + ID + "_oldItem = sender;";
        strout += "\n     " + ID + "_OnSelectBar(sender);";

        strout += "\n }";

        return ScriptHelper.GetScript(strout);
    }
}
