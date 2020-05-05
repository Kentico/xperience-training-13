using System;
using System.Linq;

using CMS.Base;
using CMS.Core;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.OnlineForms;
using CMS.UIControls;

public partial class CMSAdminControls_UI_Development_Generators_ContentItemCodeGenerator : CodeGeneratorBase
{
    private DataClassInfo mDataClass;
    private string mFolderBasePath;

 
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (SystemContext.IsPrecompiledWebsite)
        {
            ShowCodeSaveDisabledMessage();

            ucSavePath.Enabled = false;
            btnSaveCode.Enabled = false;
        }
        else
        {
            btnSaveCode.Click += SaveCode;
        }

        try
        {
            mDataClass = GetDataClassFromContext();
            mFolderBasePath = string.Format("~/{0}/CMSClasses", SystemContext.IsWebApplicationProject ? "Old_App_Code" : "App_Code");

            if (!RequestHelper.IsPostBack())
            {

                if (ContentItemCodeGenerator.Internal.CanGenerateItemClass(mDataClass))
                {
                    txtItemCode.Text = ContentItemCodeGenerator.Internal.GenerateItemClass(mDataClass);
                }

                if (ContentItemCodeGenerator.Internal.CanGenerateItemProviderClass(mDataClass))
                {
                    txtProviderCode.Text = ContentItemCodeGenerator.Internal.GenerateItemProviderClass(mDataClass);
                }
                else
                {
                    pnlGeneratedCode.CssClass = null;
                    pnlItemCode.CssClass = null;
                    pnlProviderCode.Visible = false;
                }

                ucSavePath.Value = mFolderBasePath;
                if (Directory.Exists(mFolderBasePath))
                {
                    ucSavePath.DefaultPath = mFolderBasePath;
                }
            }
        }
        catch (Exception exception)
        {
            Service.Resolve<IEventLogService>().LogException("Content item code generator", "Initialize", exception);

            btnSaveCode.Enabled = false;

            var message = GetString("general.exception");
            ShowError(message);
        }
    }


    private void SaveCode(object sender, EventArgs e)
    {
        var dataClasses = Enumerable.Repeat(mDataClass, 1);
        var path = ValidationHelper.GetString(ucSavePath.Value, string.Empty);

        if (string.IsNullOrEmpty(path))
        {
            path = mFolderBasePath;
            ucSavePath.Value = path;
        }

        base.SaveCode(path, dataClasses);
    }


    private DataClassInfo GetDataClassFromContext()
    {
        var item = UIContext.EditedObject as BaseInfo;

        if (item == null)
        {
            throw new Exception("Context does not contain content item.");
        }

        var dataClass = FindDataClass(item);

        if (dataClass == null)
        {
            throw new Exception("Content item class was not found.");
        }

        return dataClass;
    }


    private DataClassInfo FindDataClass(BaseInfo item)
    {
        switch (item.TypeInfo.ObjectType)
        {
            case BizFormInfo.OBJECT_TYPE:
                var classId = item.GetIntegerValue("FormClassID", 0);
                return DataClassInfoProvider.GetDataClassInfo(classId);
            case DocumentTypeInfo.OBJECT_TYPE_DOCUMENTTYPE:
            case CustomTableInfo.OBJECT_TYPE_CUSTOMTABLE:
                var className = item.GetStringValue("ClassName", string.Empty);
                return DataClassInfoProvider.GetDataClassInfo(className);
        }

        return null;
    }
}
