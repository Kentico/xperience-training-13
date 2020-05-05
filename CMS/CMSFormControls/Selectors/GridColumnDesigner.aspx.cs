using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml;

using CMS.Base;
using CMS.Helpers;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.IO;
using CMS.UIControls;


public partial class CMSFormControls_Selectors_GridColumnDesigner : DesignerPage
{
    #region "Variables"

    public string[,] tmpColumns;
    private string mSelColId;
    private string mColId;

    #endregion


    #region "Properties"

    /// <summary>
    /// Save count of loads page.
    /// </summary>
    private int FirstLoad
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["FirstLoad"], 0);
        }
        set
        {
            ViewState["FirstLoad"] = value;
        }
    }


    /// <summary>
    /// Changed columns in viewstate string.
    /// </summary>
    private string ViewColumns
    {
        get
        {
            return ValidationHelper.GetString(ViewState["ViewColumns"], "");
        }
        set
        {
            ViewState["ViewColumns"] = value;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(QueryHelper.GetString("queryname", string.Empty)) && !QueryHelper.ValidateHash("hash"))
        {
            URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext"));
        }
        else
        {
            int mTypeOfInput = 1;

            // Handle to ListBox postback
            ItemSelection1.OnPostBack += ItemSelection1_OnPostBack;
            ItemSelection1.OnMoveLeft += ItemSelection1_OnMoveLeft;

            // Initialize resource strings
            PageTitle.TitleText = GetString("GridColumnDesigner.Title");
            ItemSelection1.LeftColumnLabel.Text = GetString("ItemSelection.Avaliable");
            ItemSelection1.RightColumnLabel.Text = GetString("ItemSelection.Displayed");
            mSelColId = QueryHelper.GetString("SelColId", "");
            mColId = QueryHelper.GetString("ColId", "");

            // Set-up ColumnListBox
            ItemSelection1.RightColumListBox.SelectionMode = ListSelectionMode.Single;
            ItemSelection1.RightColumListBox.AutoPostBack = true;

            // Get classnames or queryname from querystring
            var classNames = QueryHelper.GetString("classnames", "");
            if (!string.IsNullOrEmpty(classNames))
            {
                hdnClassNames.Value = classNames;
                mTypeOfInput = 1;
            }

            var queryName = QueryHelper.GetString("queryname", "");
            if (!string.IsNullOrEmpty(queryName))
            {
                hdnClassNames.Value = queryName;
                mTypeOfInput = 2;
            }

            // Get data from viewstate
            tmpColumns = FromView(ViewColumns);

            // Load Columns names

            if (FirstLoad <= 1)
            {
                FirstLoad++;

                // Use dataClass or Query to get names of columns
                switch (mTypeOfInput)
                {
                    case 1:
                        LoadFromDataClass();
                        break;

                    case 2:
                        LoadFromQuery();
                        break;
                }

                // Read XML
                ReadXML(hdnSelectedColumns.Value);

                if (!string.IsNullOrEmpty(ViewColumns))
                {
                    radSelect.Checked = true;
                    radGenerate.Checked = false;
                }

                // Move selected columns
                if (tmpColumns != null)
                {
                    for (int i = tmpColumns.GetLowerBound(0); i <= tmpColumns.GetUpperBound(0); i++)
                    {
                        ListItem LI = ItemSelection1.LeftColumListBox.Items.FindByText(tmpColumns[i, 1]);
                        if (LI != null)
                        {
                            ItemSelection1.RightColumListBox.Items.Add(LI);
                            ItemSelection1.LeftColumListBox.Items.Remove(LI);
                        }
                    }
                }
            }

            if (!RequestHelper.IsPostBack())
            {
                // Reload page to get data from parent in javascript
                string script = String.Format(@"
document.getElementById('{0}').value = wopener.GetClassNames('{1}');
document.getElementById('{2}').value = wopener.GetSelectedColumns('{3}');
document.body.onload = function () {{ {4} }};
", hdnClassNames.ClientID, ScriptHelper.GetString(mColId, false), hdnSelectedColumns.ClientID, ScriptHelper.GetString(mSelColId, false), ControlsHelper.GetPostBackEventReference(ItemSelection1.RightColumListBox, ""));

                ScriptHelper.RegisterStartupScript(this, typeof(string), "LoadScript", script, true);
            }


            // Show or hide dialog
            ItemSelection1.Visible = !radGenerate.Checked;
            pnlProperties.Visible = !radGenerate.Checked;
        }
    }


    private void ItemSelection1_OnMoveLeft(object sender, CommandEventArgs e)
    {
        txtHeaderText.Text = String.Empty;
        chkDisplayAsLink.Checked = false;
    }


    /// <summary>
    /// On click OK button save changes.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (!ItemSelection1.Visible)
        {
            return;
        }

        ListItem listItem = ItemSelection1.RightColumListBox.SelectedItem;
        if (listItem != null)
        {
            SetInSelected(Convert.ToInt32(listItem.Value), listItem.Text, txtHeaderText.Text, chkDisplayAsLink.Checked.ToString());
            ShowConfirmation(GetString("grid.columnupdated"));
        }
        else
        {
            ShowWarning(GetString("grid.noselection"), persistent: false);
        }
    }


    /// <summary>
    /// On click to close button create XML and send to parent and close window.
    /// </summary>
    protected void btnClose_Click(object sender, EventArgs e)
    {
        if (radSelect.Checked)
        {
            CreateXML();
        }
        else
        {
            hdnSelectedColumns.Value = "<columns></columns>";
        }

        hdnTextClassNames.Value = ConvertXML(hdnSelectedColumns.Value);

        string script = String.Format(@"
var columnsElem = document.getElementById('{0}'),
    classNamesElem = document.getElementById('{1}');
wopener.SetValue(columnsElem.value, classNamesElem.value,'{2}','{3}');
CloseDialog();
", hdnSelectedColumns.ClientID, hdnTextClassNames.ClientID, ScriptHelper.GetString(mSelColId, false), ScriptHelper.GetString(mColId, false));

        ScriptHelper.RegisterStartupScript(this, typeof(string), "CloseScript", script, true);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Load Columns names into listbox.
    /// </summary>
    protected void LoadFromDataClass()
    {
        // Get selected classes from hiddenfield.
        ArrayList classesList = new ArrayList();
        ArrayList columnList = new ArrayList();
        if (!string.IsNullOrEmpty(hdnClassNames.Value))
        {
            classesList = new ArrayList(hdnClassNames.Value.Split(';'));
        }

        classesList.Add("CMS.Tree");
        classesList.Add("CMS.Document");

        // Fill columnList with column names from all classes.
        foreach (string className in classesList)
        {
            try
            {
                if (!String.IsNullOrEmpty(className))
                {
                    IDataClass dc = DataClassFactory.NewDataClass(className);
                    DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(className);
                    // Get columns only from couplet classes.
                    if (dci.ClassIsCoupledClass)
                    {
                        foreach (string columnName in dc.StructureInfo.ColumnNames)
                        {
                            columnList.Add(columnName);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        // Move columns from array list to string array and add indexes.
        string[,] columns = new string[columnList.Count, 2];
        int index = 0;
        foreach (string columnName in columnList)
        {
            columns[index, 0] = index.ToString();
            columns[index, 1] = columnName;
            index += 1;
        }

        ItemSelection1.LeftItems = columns;
        ItemSelection1.fill();
    }


    /// <summary>
    /// Load Columns names with Query.
    /// </summary>
    protected void LoadFromQuery()
    {
        string queryName = hdnClassNames.Value;

        var q = new DataQuery(queryName).TopN(1);
        if (!q.HasResults())
        {
            return;
        }

        DataView dv = q.Result.Tables[0].DefaultView;
        string[,] listItems = new string[dv.Table.Columns.Count, 2];

        int i = 0;
        foreach (DataColumn dc in dv.Table.Columns)
        {
            listItems[i, 0] = i.ToString();
            listItems[i, 1] = dc.ColumnName;
            i++;
        }

        ItemSelection1.LeftItems = listItems;
        ItemSelection1.fill();
    }


    /// <summary>
    /// Handle for Right Column List Box.
    /// </summary>
    private void ItemSelection1_OnPostBack(object sender, EventArgs e)
    {
        if (ItemSelection1.RightColumListBox.SelectedItem == null)
        {
            return;
        }

        int mIndex = SearchInSelected(Convert.ToInt32(ItemSelection1.RightColumListBox.SelectedItem.Value));

        if (mIndex >= 0)
        {
            // text
            txtHeaderText.Text = !string.IsNullOrEmpty(tmpColumns[mIndex, 2]) ? tmpColumns[mIndex, 2] : ItemSelection1.RightColumListBox.SelectedItem.Text;

            // checked
            if (!string.IsNullOrEmpty(tmpColumns[mIndex, 3]))
            {
                if (tmpColumns[mIndex, 3] != null && tmpColumns[mIndex, 3] == "link")
                {
                    chkDisplayAsLink.Checked = true;
                }
                else
                {
                    chkDisplayAsLink.Checked = false;
                }
            }
            else
            {
                chkDisplayAsLink.Checked = false;
            }
        }
        else
        {
            txtHeaderText.Text = ItemSelection1.RightColumListBox.SelectedItem.Text;
            chkDisplayAsLink.Checked = false;
        }
    }


    /// <summary>
    /// Search if selected item is in field.
    /// </summary>
    /// <param name="value">Value in Column list box</param>
    protected int SearchInSelected(int value)
    {
        if (tmpColumns == null)
        {
            return -1;
        }

        for (int i = tmpColumns.GetLowerBound(0); i <= tmpColumns.GetUpperBound(0); i++)
        {
            if ((!string.IsNullOrEmpty(tmpColumns[i, 0])) && (value == Convert.ToInt32(tmpColumns[i, 0])))
            {
                return i;
            }
        }

        return -1;
    }


    /// <summary>
    /// Sets attribute in selected item or insert new if attribute is not in.
    /// </summary>
    /// <param name="value">Value in Column list box</param>
    /// <param name="name">Name of column</param>
    /// <param name="headerText">Header text</param>
    /// <param name="type">Type (link|bound))</param>
    protected void SetInSelected(int value, string name, string headerText, string type)
    {
        if (SearchInSelected(value) >= 0)
        {
            tmpColumns[SearchInSelected(value), 2] = headerText;
            tmpColumns[SearchInSelected(value), 3] = Convert.ToBoolean(type) ? "link" : "bound";
        }
        else // Insert new
        {
            int to = (tmpColumns != null) ? (tmpColumns.GetUpperBound(0) + 1) : 0;

            string[,] field = new string[to + 1, 4];

            for (int i = 0; i < to; i++)
            {
                if (tmpColumns == null)
                {
                    continue;
                }

                field[i, 0] = tmpColumns[i, 0];
                field[i, 1] = tmpColumns[i, 1];
                field[i, 2] = tmpColumns[i, 2];
                field[i, 3] = tmpColumns[i, 3];
            }

            field[to, 0] = value.ToString();
            field[to, 1] = name;
            field[to, 2] = headerText;
            field[to, 3] = Convert.ToBoolean(type) ? "link" : "bound";

            tmpColumns = field;
        }

        ViewColumns = ToView(tmpColumns);
    }


    /// <summary>
    /// Read XML and set properties.
    /// </summary>
    /// <param name="xml">XML document</param>
    protected void ReadXML(string xml)
    {
        if (DataHelper.GetNotEmpty(xml, "") == "")
        {
            return;
        }

        XmlDocument xmlDocument = new XmlDocument();

        LoadDocument(xmlDocument, xml);

        if (xmlDocument.DocumentElement == null)
        {
            return;
        }

        XmlNodeList NodeList = xmlDocument.DocumentElement.GetElementsByTagName("column");

        string[,] attributes = new string[NodeList.Count, 4];

        int i = 0;

        foreach (XmlNode node in NodeList)
        {
            if (node.Attributes == null)
            {
                continue;
            }

            string value = "";
            for (int j = ItemSelection1.LeftItems.GetLowerBound(0); j <= ItemSelection1.LeftItems.GetUpperBound(0); j++)
            {
                if (ItemSelection1.LeftItems[j, 1] == XmlHelper.GetXmlAttributeValue(node.Attributes["name"], ""))
                {
                    value = ItemSelection1.LeftItems[j, 0];
                }
            }

            attributes[i, 0] = value;
            attributes[i, 1] = XmlHelper.GetXmlAttributeValue(node.Attributes["name"], "");
            attributes[i, 2] = XmlHelper.GetXmlAttributeValue(node.Attributes["header"], "");
            attributes[i, 3] = XmlHelper.GetXmlAttributeValue(node.Attributes["type"], "");
            i++;
        }

        tmpColumns = attributes;
        ViewColumns = ToView(tmpColumns);
    }
    

    /// <summary>
    /// Searches in changed columns by identifier.
    /// </summary>
    protected string[,] SearchInChangedColumnsById(int id)
    {
        if (tmpColumns == null)
        {
            return null;
        }

        string[,] returnArray = new string[1, 4];

        for (int i = tmpColumns.GetLowerBound(0); i <= tmpColumns.GetUpperBound(0); i++)
        {
            if (tmpColumns[i, 0] != id.ToString())
            {
                continue;
            }

            returnArray[0, 0] = tmpColumns[i, 0];
            returnArray[0, 1] = tmpColumns[i, 1];
            returnArray[0, 2] = tmpColumns[i, 2];
            returnArray[0, 3] = tmpColumns[i, 3];
            return returnArray;
        }

        return null;
    }


    /// <summary>
    /// Creates new XML document for columns.
    /// </summary>
    protected void CreateXML()
    {
        string[,] changed = null;

        XmlDocument xmlDoc = new XmlDocument();
        XmlNode xmlRoot = xmlDoc.CreateElement("columns");
        xmlDoc.AppendChild(xmlRoot);

        for (int i = 0; i < ItemSelection1.RightColumListBox.Items.Count; i++)
        {
            bool isChanged = false;

            if (SearchInSelected(Convert.ToInt32(ItemSelection1.RightColumListBox.Items[i].Value)) >= 0)
            {
                isChanged = true;

                changed = SearchInChangedColumnsById(Convert.ToInt32(ItemSelection1.RightColumListBox.Items[i].Value));

                if (changed == null)
                {
                    isChanged = false;
                }
            }

            XmlElement xmlColumn = xmlDoc.CreateElement("column");

            // Prepare attributes for "column" node
            var attributes = new Dictionary<string, string>
            {
                { "name", ItemSelection1.RightColumListBox.Items[i].Text },
                { "header", (isChanged ? changed[0, 2] : String.Empty) },
                { "type", ((isChanged) && (changed[0, 3] != null && changed[0, 3].EqualsCSafe("link", true)) ? "link" : "bound") }
            };

            xmlColumn.AddAttributes(attributes);

            xmlRoot.AppendChild(xmlColumn);
        }

        hdnSelectedColumns.Value = xmlDoc.ToFormattedXmlString(true);
    }


    /// <summary>
    /// Convert XML to TextBox.
    /// </summary>
    /// <param name="xml">XML document in string</param>
    protected static string ConvertXML(string xml)
    {
        if (DataHelper.GetNotEmpty(xml, String.Empty) == String.Empty)
        {
            return String.Empty;
        }

        XmlDocument xmlDocument = new XmlDocument();

        LoadDocument(xmlDocument, xml);

        if (xmlDocument.DocumentElement == null)
        {
            return String.Empty;
        }

        XmlNodeList nodeList = xmlDocument.DocumentElement.GetElementsByTagName("column");

        return String.Join(";", nodeList.Cast<XmlNode>().Select(node => node.Attributes != null ? XmlHelper.GetXmlAttributeValue(node.Attributes["name"], "") : String.Empty));
    }


    /// <summary>
    /// Convert 2D string array to viewstate string.
    /// </summary>
    protected static string ToView(string[,] input)
    {
        string toReturn = String.Empty;

        for (int i = 0; i <= input.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= input.GetUpperBound(1); j++)
            {
                toReturn += input[i, j];
                toReturn += "~";
            }

            toReturn += ";";
        }

        return toReturn;
    }


    /// <summary>
    /// Convert from Viewstate string to 2dimensional string array.
    /// </summary>
    protected static string[,] FromView(string input)
    {
        if (input == null)
        {
            return null;
        }

        string[] first = input.Split(';');

        string[,] toReturn = new string[first.GetUpperBound(0), 4];

        for (int i = 0; i < first.GetUpperBound(0); i++)
        {
            string[] second = first[i].Split('~');

            toReturn[i, 0] = second[0];
            toReturn[i, 1] = second[1];
            toReturn[i, 2] = second[2];
            toReturn[i, 3] = second[3];
        }

        return toReturn;
    }


    /// <summary>
    /// Loads XML string to <paramref name="xmlDocument"/>
    /// </summary>
    private static void LoadDocument(XmlDocument xmlDocument, string xml)
    {
        using (var stringReader = new StringReader(xml))
        {
            using (var xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings()))
            {
                xmlDocument.Load(xmlReader);
            }
        }
    }

    #endregion
}
