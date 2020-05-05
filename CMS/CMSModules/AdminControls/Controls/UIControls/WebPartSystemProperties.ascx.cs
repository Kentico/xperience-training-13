using System;
using System.Xml;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;

#pragma warning disable BH3501 // Should inherit from some abstract CMS UI WebPart.
public partial class CMSModules_AdminControls_Controls_UIControls_WebPartSystemProperties : CMSUserControl
#pragma warning restore BH3501 // Should inherit from some abstract CMS UI WebPart.
{
    #region "Variables"

    String mDefaultValueColumName = String.Empty;
    BaseInfo mEditedObject;
    String mDefaultSet = String.Empty;
    String mWebPartProperties = "<form></form>";

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        mEditedObject = UIContext.EditedObject as BaseInfo;

        // If saved is found in query string
        if (!RequestHelper.IsPostBack() && (QueryHelper.GetInteger("saved", 0) == 1))
        {
            ShowChangesSaved();
        }

        string before;
        string after;

        string objectType = UIContextHelper.GetObjectType(UIContext);

        switch (objectType.ToLowerCSafe())
        {
            case "cms.webpart":
                mDefaultValueColumName = "WebPartDefaultValues";

                before = PortalFormHelper.GetWebPartProperties(WebPartTypeEnum.Standard, PropertiesPosition.Before);
                after = PortalFormHelper.GetWebPartProperties(WebPartTypeEnum.Standard, PropertiesPosition.After);

                mDefaultSet = FormHelper.CombineFormDefinitions(before, after);

                WebPartInfo wi = mEditedObject as WebPartInfo;

                // If inherited web part load parent properties
                if (wi.WebPartParentID > 0)
                {
                    WebPartInfo parentInfo = WebPartInfoProvider.GetWebPartInfo(wi.WebPartParentID);
                    if (parentInfo != null)
                    {
                        mWebPartProperties = FormHelper.MergeFormDefinitions(parentInfo.WebPartProperties, wi.WebPartProperties);
                    }
                }
                else
                {
                    mWebPartProperties = wi.WebPartProperties;
                }

                break;

            case "cms.widget":
                before = PortalFormHelper.LoadProperties("Widget", "Before.xml");
                after = PortalFormHelper.LoadProperties("Widget", "After.xml");

                mDefaultSet = FormHelper.CombineFormDefinitions(before, after);

                mDefaultValueColumName = "WidgetDefaultValues";
                WidgetInfo wii = mEditedObject as WidgetInfo;
                if (wii != null)
                {
                    WebPartInfo wiiWp = WebPartInfoProvider.GetWebPartInfo(wii.WidgetWebPartID);
                    if (wiiWp != null)
                    {
                        mWebPartProperties = FormHelper.MergeFormDefinitions(wiiWp.WebPartProperties, wii.WidgetProperties);
                    }
                }

                break;
        }

        // Get the web part info
        if (mEditedObject != null)
        {
            String defVal = ValidationHelper.GetString(mEditedObject.GetValue(mDefaultValueColumName), string.Empty);
            mDefaultSet = LoadDefaultValuesXML(mDefaultSet);

            fieldEditor.Mode = FieldEditorModeEnum.SystemWebPartProperties;
            fieldEditor.FormDefinition = FormHelper.MergeFormDefinitions(mDefaultSet, defVal);
            fieldEditor.OnAfterDefinitionUpdate += fieldEditor_OnAfterDefinitionUpdate;
            fieldEditor.OriginalFormDefinition = mDefaultSet;
            fieldEditor.WebPartId = mEditedObject.Generalized.ObjectID;
        }

        ScriptHelper.HideVerticalTabs(Page);
    }


    /// <summary>
    /// Load XML with default values (remove keys already overridden in properties tab).
    /// </summary>
    /// <param name="formDef">String XML definition of default values of webpart</param>
    private String LoadDefaultValuesXML(string formDef)
    {
        XmlDocument xmlDefault = new XmlDocument();

        // Test if there is any default properties set
        XmlDocument xmlProperties = new XmlDocument();
        xmlProperties.LoadXml(mWebPartProperties);

        // Load default system xml 
        xmlDefault.LoadXml(formDef);

        // Filter overridden properties - remove properties with same name as in system XML
        XmlNodeList defaultList = xmlDefault.SelectNodes(@"//field");
        foreach (XmlNode node in defaultList)
        {
            string columnName = node.Attributes["column"].Value;

            XmlNodeList propertiesList = xmlProperties.SelectNodes("//field[@column=\"" + columnName + "\"]");
            //This property already set in properties tab
            if (propertiesList.Count > 0)
            {
                node.ParentNode.RemoveChild(node);
            }
        }

        // Filter empty categories            
        XmlNodeList nodes = xmlDefault.DocumentElement.ChildNodes;
        for (int i = 0; i < nodes.Count; i++)
        {
            XmlNode node = nodes[i];
            if (node.Name.ToLowerCSafe() == "category")
            {
                // Find next category
                if (i < nodes.Count - 1)
                {
                    XmlNode nextNode = nodes[i + 1];
                    if (nextNode.Name.ToLowerCSafe() == "category")
                    {
                        // Delete actual category
                        node.ParentNode.RemoveChild(node);
                        i--;
                    }
                }
            }
        }

        // Remove WebPartControlID 
        XmlNode IDNode = xmlDefault.SelectSingleNode("//field[@column=\"WebPartControlID\"]");
        if (IDNode != null)
        {
            IDNode.ParentNode.RemoveChild(IDNode);
        }

        // Test if last category is not empty           
        nodes = xmlDefault.DocumentElement.ChildNodes;
        if (nodes.Count > 0)
        {
            XmlNode lastNode = nodes[nodes.Count - 1];
            if (lastNode.Name.ToLowerCSafe() == "category")
            {
                lastNode.ParentNode.RemoveChild(lastNode);
            }
        }
        return xmlDefault.OuterXml;
    }


    protected void fieldEditor_OnAfterDefinitionUpdate(object sender, EventArgs e)
    {
        String defVal = FormHelper.GetFormDefinitionDifference(mDefaultSet, fieldEditor.FormDefinition, true);
        mEditedObject.SetValue(mDefaultValueColumName, defVal);
        mEditedObject.Update();
    }

    #endregion
}