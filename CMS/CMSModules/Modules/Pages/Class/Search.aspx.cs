using System;

using CMS.Base;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.UIControls;


[EditedObject("cms.class", "classid")]
[UIElement(ModuleName.CMS, "Search")]
public partial class CMSModules_Modules_Pages_Class_Search : GlobalAdminPage
{
    #region "Private properties"

    /// <summary>
    /// Edited object.
    /// </summary>
    private DataClassInfo ClassInfo
    {
        get
        {
            return (DataClassInfo)EditedObject;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (ClassInfo != null)
        {
            string className = ClassInfo.ClassName.ToLowerInvariant();

            switch (className)
            {
                case UserSettingsInfo.OBJECT_TYPE:
                    ShowWarning(GetString("searchfields.usersettings"));
                    searchFields.Visible = false;
                    break;

                case PredefinedObjectType.SKU:
                    ShowWarning(GetString("systbl.search.warninfo"));
                    DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(PredefinedObjectType.DOCUMENT);
                    if (dci != null)
                    {
                        searchFields.ItemID = dci.ClassID;
                    }
                    searchFields.AdvancedMode = SystemContext.DevelopmentMode;
                    CurrentMaster.HeaderActions.AddAction(new SaveAction());
                    break;

                default:
                    searchFields.ItemID = ClassInfo.ClassID;
                    searchFields.LoadActualValues = true;
                    CurrentMaster.HeaderActions.AddAction(new SaveAction());
                    break;
            }

            // Initialize customization if needed
            ResourceInfo resource = ResourceInfo.Provider.Get(QueryHelper.GetInteger("moduleid", 0));
            if ((className != UserSettingsInfo.OBJECT_TYPE) && !SystemContext.DevelopmentMode && (resource != null) && !resource.ResourceIsInDevelopment)
            {
                pnlCustomization.MessagesPlaceHolder = MessagesPlaceHolder;
                pnlCustomization.Columns = new [] { "ClassSearchEnabled", "ClassSearchTitleColumn", "ClassSearchContentColumn", "ClassSearchImageColumn", "ClassSearchCreationDateColumn", "ClassSearchSettings" };
                pnlCustomization.HeaderActions = HeaderActions;
            }
            else
            {
                pnlCustomization.StopProcessing = true;
            }
        }
    }

    #endregion
}