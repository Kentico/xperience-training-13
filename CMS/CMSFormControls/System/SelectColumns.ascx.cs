using System;
using System.Text;
using System.Xml;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.SiteProvider;

public partial class CMSFormControls_System_SelectColumns : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            txtColumns.Enabled = value;
            btnDesign.Enabled = value;
        }
    }


    ///<summary>Gets or sets field value.</summary>
    public override object Value
    {
        get
        {
            return hdnSelectedColumns.Value;
        }
        set
        {
            hdnSelectedColumns.Value = (string)value;
        }
    }
    

    public override bool IsValid()
    {
        var value = hdnSelectedColumns.Value;

        if (String.IsNullOrEmpty(value))
        {
            return true;
        }

        try
        {
            // Check whether XML is not malformed
            LoadXmlDocument(new XmlDocument(), value);

            return true;
        }
        catch
        {
            return false;
        }
    }
    
    #endregion


    #region "Methods"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (Form != null)
        {
            bool IsQuery = true;
            bool IsClassNames = true;
            bool IsCustomTable = true;

            ScriptHelper.RegisterDialogScript(Page);

            btnDesign.OnClientClick = "OpenModalDialog('" + hdnSelectedColumns.ClientID + "','" + txtColumns.ClientID + "'); return false;";

            string script = @"
function SetValue(input, txtInput,hdnSelColId,hdnColId){
    document.getElementById(hdnSelColId).value = input;
    document.getElementById(hdnColId).value = txtInput;
    return false;
}
function GetClassNames(hdnColId)  {
    return document.getElementById(hdnColId).value;
}
function GetSelectedColumns(hdnSelColId) { 
    return document.getElementById(hdnSelColId).value;
}";

            // Try to find QueryName or ClassNames field

            object value;
            bool succ = Form.Data.TryGetValue("QueryName", out value);
            if (succ)
            {
                hdnProperties.Value = value.ToString();
                IsClassNames = false;
                IsCustomTable = false;
            }
            else
            {
                IsQuery = false;
            }

            // If it still can be custom table, try it
            if (IsCustomTable)
            {
                // Fake it as query
                succ = Form.Data.TryGetValue("CustomTable", out value);
                if (succ)
                {
                    hdnProperties.Value = value.ToString();
                    IsClassNames = false;
                    IsQuery = true;
                }
                else
                {
                    IsCustomTable = false;
                }
            }

            // If it still can be class names, try it
            if (IsClassNames)
            {
                value = String.Empty;
                succ = Form.Data.TryGetValue("ClassNames", out value);
                if (succ)
                {
                    hdnProperties.Value = value.ToString();
                }
                else
                {
                    IsClassNames = false;
                }
            }

            // if QueryName field was found
            if (IsQuery)
            {
                // if query name isnt empty
                if (!String.IsNullOrEmpty(hdnProperties.Value))
                {
                    // Custom tables uses selectall query by default
                    if (IsCustomTable)
                    {
                        hdnProperties.Value += ".selectall";
                    }

                    string properties = ScriptHelper.GetString(hdnProperties.Value, false);

                    script += "function OpenModalDialog(hdnSelColId, hdnColId) { modalDialog('" + ResolveUrl("~/CMSFormControls/Selectors/GridColumnDesigner.aspx") + "?queryname=" + properties + "&SelColId=' + hdnSelColId + '&ColId=' + hdnColId + '&hash=" + ValidationHelper.GetHashString("?queryname=" + properties + "&SelColId=" + hdnSelectedColumns.ClientID + "&ColId=" + txtColumns.ClientID, new HashSettings("")) + "' ,'GridColumnDesigner', 700, 560); return false;}\n";
                }
                else
                {
                    string message;

                    // Different message for query and custom table
                    if (IsCustomTable)
                    {
                        var classes = DataClassInfoProvider.GetClasses().Where("ClassIsCustomTable = 1 AND ClassID IN (SELECT ClassID FROM CMS_ClassSite WHERE SiteID = " + SiteContext.CurrentSiteID + ")");

                        message = classes.HasResults() ? GetString("SelectColumns.ApplyFirst") : GetString("SelectColumns.nocustomtablesavaible");
                    }
                    else
                    {
                        message = GetString("SelectColumns.EmptyQueryName");
                    }

                    script += "function OpenModalDialog(hdnSelColId, hdnColId) { alert('" + message + "'); return false;}\n";
                }
            }
            else if (IsClassNames)
            {
                script += "function OpenModalDialog(hdnSelColId, hdnColId) { modalDialog('" + ResolveUrl("~/CMSFormControls/Selectors/GridColumnDesigner.aspx") + "?classnames=" + ScriptHelper.GetString(hdnProperties.Value, false) + "&SelColId=' + hdnSelColId + '&ColId=' + hdnColId ,'GridColumnDesigner', 700, 560); return false;}\n";
            }
            else // Cant find QueryName or ClassNames or Custom table fiels 
            {
                script += "function OpenModalDialog(hdnSelColId, hdnColId) { alert(" + ScriptHelper.GetLocalizedString("SelectColumns.EmptyClassNamesAndQueryName") + ");}\n";
            }

            //Register JavaScript
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "SelectColumsGlobal", ScriptHelper.GetScript(script));

            btnDesign.Text = GetString("general.select");

            // Set to Textbox selected columns
            txtColumns.Text = ConvertXML(hdnSelectedColumns.Value);
        }
    }


    /// <summary>
    /// Convert XML to TextBox.
    /// </summary>
    /// <param name="mXML">XML document</param>
    public string ConvertXML(string mXML)
    {
        if (DataHelper.GetNotEmpty(mXML, "") == "")
        {
            return "";
        }

        StringBuilder mToReturn = new StringBuilder();
        XmlDocument mXMLDocument = new XmlDocument();

        try
        {
            LoadXmlDocument(mXMLDocument, mXML);
        }
        catch
        {
            return "";
        }

        if (mXMLDocument.DocumentElement == null)
        {
            return "";
        }

        XmlNodeList NodeList = mXMLDocument.DocumentElement.GetElementsByTagName("column");

        int i = 0;

        foreach (XmlNode node in NodeList)
        {
            if (i > 0)
            {
                mToReturn.Append(";");
            }

            mToReturn.Append(XmlHelper.GetXmlAttributeValue(node.Attributes["name"], ""));

            i++;
        }

        return mToReturn.ToString();
    }


    private static void LoadXmlDocument(XmlDocument document, string xml)
    {
        using (var stringReader = new StringReader(xml))
        using (var xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings()))
        {
            document.Load(xmlReader);
        }
    }

    #endregion
}