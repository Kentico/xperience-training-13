using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.UIControls;


[EditedObject(AlternativeFormInfo.OBJECT_TYPE, "altformid")]
public partial class CMSModules_Modules_Pages_Class_AlternativeForm_General : GlobalAdminPage
{
    #region "Properties"

    /// <summary>
    /// Indicates if module allows to edit this tab.
    /// </summary>
    private bool IsEditable
    {
        get
        {
            ResourceInfo resource = ResourceInfo.Provider.Get(QueryHelper.GetInteger("moduleid", 0));
            return ((resource != null) && resource.ResourceIsInDevelopment) || SystemContext.DevelopmentMode || AlternativeForm.FormIsCustom;
        }
    }


    /// <summary>
    /// Edited object.
    /// </summary>
    private AlternativeFormInfo AlternativeForm
    {
        get
        {
            return (AlternativeFormInfo)EditedObject;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        int classId = QueryHelper.GetInteger("classid", 0);

        if (!IsEditable)
        {
            altFormEdit.Enabled = false;
            HeaderActions.Enabled = false;
            ShowInformation(GetString("resource.installedresourcewarning"));
        }

        // Check if the 'Combine With User Settings' feature should be available
        if (classId > 0)
        {
            string className = DataClassInfoProvider.GetClassName(classId);
            if (className != null && (className.ToLowerCSafe().Trim() == UserInfo.OBJECT_TYPE.ToLowerCSafe()))
            {
                altFormEdit.ShowCombineUsersSettings = true;
            }
        }
    }

    #endregion
}