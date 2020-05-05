using System;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI;

using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;


[assembly: RegisterCustomClass("WidgetTreeControlExtender", typeof(WidgetTreeControlExtender))]

/// <summary>
/// Bad words list control extender
/// </summary>
public class WidgetTreeControlExtender : ControlExtender<UITreeView>
{
    public override void OnInit()
    {
        string postbackRef = ControlsHelper.GetPostBackEventReference(Control.Page, "##");
        // Script for new widget
        String script = @"function OnSelectWebPart(webpartId) {
if ((webpartId > 0) && (selectedItemId > 0) && (selectedItemType == 'category')) {" +
postbackRef.Replace("'##'", "'newwidget;' + webpartId + ';' + selectedItemId") + "}}";

        ScriptHelper.RegisterClientScriptBlock(Control, typeof(String), "RefreshScript", ScriptHelper.GetScript(script));

        if (HttpContext.Current != null)
        {
            String eventArgument = HttpContext.Current.Request[Page.postEventArgumentID];
            if (!String.IsNullOrEmpty(eventArgument) && eventArgument.StartsWithCSafe("newwidget", true))
            {
                CreateNewWidget(eventArgument);
            }
        }
    }


    /// <summary>
    /// Creates new widget
    /// </summary>
    /// <param name="eventArgument">Objecttype(widget or widgetcategory);objectid</param>
    public void CreateNewWidget(string eventArgument)
    {
        string[] values = eventArgument.Split(';');
        if ((values != null) && (values.Length > 2))
        {
            string action = values[0];
            int id = 0;
            int parentId = 0;

            switch (action)
            {
                case "newwidget":

                    id = ValidationHelper.GetInteger(values[1], 0);
                    parentId = ValidationHelper.GetInteger(values[2], 0);

                    // Create new widget of selected type
                    WidgetInfo wi = NewWidget(id, parentId);
                    if (wi != null)
                    {
                        URLHelper.Redirect(UIContextHelper.GetElementUrl("cms.widgets", "Development.Widgets", false, wi.WidgetID));
                    }

                    break;
            }
        }
    }


    /// <summary>
    /// Creates new widget with setting from parent webpart.
    /// </summary>
    /// <param name="parentWebpartId">ID of parent webpart</param>
    /// <param name="categoryId">ID of parent widget category</param>
    /// <returns>Created widget info</returns>
    private WidgetInfo NewWidget(int parentWebpartId, int categoryId)
    {
        WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(parentWebpartId);

        // Widget cannot be created from inherited webpart
        if ((wpi != null) && (wpi.WebPartParentID == 0))
        {
            // Set widget according to parent webpart
            WidgetInfo wi = new WidgetInfo();
            wi.WidgetName = FindUniqueWidgetName(wpi.WebPartName, 100);
            wi.WidgetDisplayName = wpi.WebPartDisplayName;
            wi.WidgetDescription = wpi.WebPartDescription;
            wi.WidgetDocumentation = wpi.WebPartDocumentation;
            wi.WidgetSkipInsertProperties = wpi.WebPartSkipInsertProperties;
            wi.WidgetIconClass = wpi.WebPartIconClass;

            wi.WidgetProperties = FormHelper.GetFormFieldsWithDefaultValue(wpi.WebPartProperties, "visible", "false");

            wi.WidgetWebPartID = parentWebpartId;
            wi.WidgetCategoryID = categoryId;

            // Save new widget to DB
            WidgetInfoProvider.SetWidgetInfo(wi);

            // Get thumbnail image from webpart
            DataSet ds = MetaFileInfoProvider.GetMetaFiles(wpi.WebPartID, WebPartInfo.OBJECT_TYPE, null, null, null, null, 1);

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                MetaFileInfo mfi = new MetaFileInfo(ds.Tables[0].Rows[0]);
                mfi.Generalized.EnsureBinaryData();
                mfi.MetaFileID = 0;
                mfi.MetaFileGUID = Guid.NewGuid();
                mfi.MetaFileObjectID = wi.WidgetID;
                mfi.MetaFileObjectType = WidgetInfo.OBJECT_TYPE;

                MetaFileInfoProvider.SetMetaFileInfo(mfi);
            }

            // Return ID of newly created widget
            return wi;
        }

        return null;
    }


    /// <summary>
    /// Finds unique widget name. Uses base prefix string with incrementing counter.
    /// </summary>
    /// <param name="basePrefix">New code name will start with this string</param>
    /// <param name="maxLength">Maximum length of unique name</param>
    /// <returns>Unique widget code name.</returns>
    private string FindUniqueWidgetName(string basePrefix, int maxLength)
    {
        int i = 0;
        string newName = null;

        string postfix = String.Empty;
        // Loop to get unique widget name
        do
        {
            newName = TextHelper.LimitLength(basePrefix, maxLength - postfix.Length, "") + postfix;
            i++;
            postfix = "_" + i;
        } while (WidgetInfoProvider.GetWidgetInfo(newName) != null);

        return newName;
    }
}