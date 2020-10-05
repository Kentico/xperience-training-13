using System;
using System.Data;
using System.Linq;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.Modules;
using CMS.Modules.NuGetPackages;
using CMS.UIControls;
using CMS.UIControls.UniGridConfig;


public partial class CMSModules_Modules_Controls_NuGetPackages_ExportModule : ExportModuleControl
{
    private ResourceInfo mResource;
    private readonly CMSButton mFooterButton = new CMSButton();
    private ModulePackageBuilder mPackageBuilder;


    /// <summary>
    /// Gets or set directly the ResourceInfo of which th module is created
    /// </summary>
    private ResourceInfo Resource
    {
        get
        {
            return mResource ?? (mResource = ResourceInfo.Provider.Get(ResourceID));
        }
    }


    /// <summary>
    /// Gets the Control to be put into the footer of the dialog. in this case it's CMSButton.
    /// </summary>
    public override Control FooterControl
    {
        get
        {
            return mFooterButton;
        }
    }


    /// <summary>
    /// Handles the loading and configuration of the control
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Resource == null)
        {
            RedirectToInformation("editedobject.notexists");

            return;
        }

        if (!SetUpPackageBuilder())
        {
            return;
        }

        SetupUniGrids();

        dataForm.SubmitButton.Visible = false;

        ModulePackageMetadata metadata = mPackageBuilder.GetModuleMetadata();

        var formInfo = CreateFormInfo();
        FillFormWithData(formInfo, metadata);

        SetUpFooterButton(metadata);
    }


    /// <summary>
    /// Creates and configures the package builder
    /// Returns true on success, false on failure
    /// </summary>
    private bool SetUpPackageBuilder()
    {
        try
        {
            mPackageBuilder = new ModulePackageBuilder(Resource);
        }
        catch (ArgumentException ex)
        {
            ShowError("cms.modules.installpackage.resourceerror");
            Service.Resolve<IEventLogService>().LogException("CreateInstallPackage", "SetUpPackageBuilder", ex);

            return false;
        }
        catch (Exception ex)
        {
            ShowError("cms.modules.installpackage.generalerror");
            Service.Resolve<IEventLogService>().LogException("CreateInstallPackage", "SetUpPackageBuilder", ex);

            return false;
        }

        return true;
    }


    /// <summary>
    /// Configures the footer button
    /// </summary>
    private void SetUpFooterButton(ModulePackageMetadata metadata)
    {
        var button = (CMSButton)FooterControl;
        button.Text = GetString("general.create");
        button.ButtonStyle = ButtonStyle.Primary;

        string fullFilePath = GetPackageFilePath(metadata);
        if (File.Exists(fullFilePath))
        {
            string errorMessage = ScriptHelper.GetString(String.Format(GetString("cms.modules.installpackage.exists"), fullFilePath));
            button.OnClientClick = String.Format("if(!confirm({0})){{return false;}}", errorMessage);
        }

        button.Click += (sender, e) => MoveToNextStep(fullFilePath);
    }


    /// <summary>
    /// Creates the installation package and if successful moves the control to step two
    /// </summary>
    /// <param name="fullFilePath">Full physical path to the resulting package file.</param>
    private void MoveToNextStep(string fullFilePath)
    {
        try
        {
            mPackageBuilder.BuildPackage(fullFilePath);
        }
        catch (Exception ex)
        {
            ShowError("cms.modules.installpackage.failed");
            Service.Resolve<IEventLogService>().LogException("CreateInstallPackage", "BuildPackage", ex);

            return;
        }

        FooterControl.Visible = false;
        plcStep1.Visible = false;
        plcStep2.Visible = true;

        string linkString = null;
        string unmappedFilePath = URLHelper.UnMapPath(fullFilePath);
        if (!unmappedFilePath.EqualsCSafe(fullFilePath))
        {
            linkString = String.Format("<a href=\"{0}\">{1}</a>", HTMLHelper.EncodeForHtmlAttribute(UrlResolver.ResolveUrl(unmappedFilePath)), HTMLHelper.HTMLEncode(fullFilePath));
        }
        ltlExportResult.Text = String.Format(GetString("cms.modules.installpackage.result"), linkString ?? fullFilePath);
    }


    /// <summary>
    /// Gets the path to the location of the package based on provided package <see cref="metadata"/>.
    /// </summary>
    /// <param name="metadata">Metadata of the package</param>
    private string GetPackageFilePath(ModulePackageMetadata metadata)
    {
        var packageName = String.Format("{0}.{1}.nupkg", metadata.Id, metadata.Version);

        return Path.EnsureSlashes(Path.Combine(ImportExportHelper.GetSiteUtilsFolder(), "Export", packageName));
    }


    /// <summary>
    /// Configures the grids displaying objects
    /// </summary>
    private void SetupUniGrids()
    {
        var pagerConfig = new UniGridPagerConfig()
        {
            DefaultPageSize = -1,
            DisplayPager = false,
        };

        gridFiles.OnDataReload += gridFiles_OnDataReload;
        gridFiles.PagerConfig = pagerConfig;

        gridObjects.OnDataReload += gridObjects_OnDataReload;
        gridObjects.PagerConfig = pagerConfig;
    }


    /// <summary>
    /// Populates the form with data to be displayed
    /// </summary>
    /// <param name="formInfo">FormInfo that will be used in the form</param>
    /// <param name="metadata">Metadata of the package</param>
    private void FillFormWithData(FormInfo formInfo, ModulePackageMetadata metadata)
    {
        DataRow row = formInfo.GetDataRow();
        row["moduleDisplayName"] = HTMLHelper.HTMLEncode(metadata.Title);
        row["moduleName"] = HTMLHelper.HTMLEncode(metadata.Id);
        row["moduleDescription"] = HTMLHelper.HTMLEncode(metadata.Description);
        row["moduleVersion"] = HTMLHelper.HTMLEncode(metadata.Version);
        row["moduleAuthor"] = HTMLHelper.HTMLEncode(metadata.Authors);

        dataForm.DataRow = row;
        dataForm.FormInformation = formInfo;
        dataForm.ReloadData();
    }


    /// <summary>
    /// Based on the TypeInformation gets and returns object's name
    /// </summary>
    /// <param name="row">A DataRow representign the object</param>
    /// <param name="objectTypeInfo">ObjectType of the object passed</param>
    private string GetName(DataRow row, ObjectTypeInfo objectTypeInfo)
    {
        string columnName = objectTypeInfo.DisplayNameColumn;
        if (String.IsNullOrEmpty(columnName))
        {
            columnName = objectTypeInfo.CodeNameColumn;
        }
        string result = row[columnName].ToString();
        if (String.IsNullOrEmpty(result))
        {
            return GetString("general.na");
        }

        return result;
    }


    /// <summary>
    /// Event handler which populates the grid of objects with data.
    /// </summary>
    /// <param name="completeWhere">Ignored</param>
    /// <param name="currentOrder">Ignored</param>
    /// <param name="currentTopN">Ignored</param>
    /// <param name="columns">Ignored</param>
    /// <param name="currentOffset">Ignored</param>
    /// <param name="currentPageSize">Ignored</param>
    /// <param name="totalRecords">Set to number of objects associated with the module</param>
    private DataSet gridObjects_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        var set = new DataSet();
        var table = new DataTable();
        set.Tables.Add(table);
        table.Columns.Add("ObjectType");
        table.Columns.Add("Name");
        try
        {
            foreach (string objectType in mPackageBuilder.IncludedObjectTypes)
            {
                // Prevent accessing foreach variable in closure
                string localObjectType = objectType;

                ObjectQuery objects = mPackageBuilder.GetModuleObjects(localObjectType);
                objects.ForEachRow(p => table.Rows.Add(GetString("objecttype." + localObjectType.Replace('.', '_')), GetName(p, objects.TypeInfo)));

            }
        }
        catch (Exception ex)
        {
            ShowError("cms.modules.installpackage.errorobjects");
            Service.Resolve<IEventLogService>().LogException("CreateInstallPackage", "GetModuleObjects", ex);
        }
        totalRecords = table.Rows.Count;

        return set;
    }


    /// <summary>
    /// Event handler which populates the grid of files with data.
    /// </summary>
    /// <param name="completeWhere">Ignored</param>
    /// <param name="currentOrder">Ignored</param>
    /// <param name="currentTopN">Ignored</param>
    /// <param name="columns">Ignored</param>
    /// <param name="currentOffset">Ignored</param>
    /// <param name="currentPageSize">Ignored</param>
    /// <param name="totalRecords">Set to number of files associated with the module</param>
    private DataSet gridFiles_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        var set = new DataSet();
        var table = new DataTable();
        set.Tables.Add(table);
        table.Columns.Add("Path");
        try
        {
            mPackageBuilder.GetModuleFiles().ToList().ForEach(p => table.Rows.Add(p));
        }
        catch (Exception ex)
        {
            ShowError("cms.modules.installpackage.errorfiles");
            Service.Resolve<IEventLogService>().LogException("CreateInstallPackage", "GetModuleFiles", ex);
        }
        totalRecords = table.Rows.Count;

        return set;
    }


    /// <summary>
    /// Shows the specified error message, optionally with a tooltip text and disables the footer button.
    /// </summary>
    /// <param name="text">Error message text</param>
    /// <param name="description">Additional description</param>
    /// <param name="tooltipText">Tooltip text</param>
    /// <param name="persistent">Indicates if the message is persistent</param>
    public override void ShowError(string text, string description = null, string tooltipText = null, bool persistent = true)
    {
        ((CMSButton)FooterControl).Enabled = false;
        base.ShowError(GetString(text), description, tooltipText, persistent);
    }


    /// <summary>
    /// Adds new field to the FormInfo.
    /// </summary>
    /// <param name="formInfo">FormInfo to which the field will be added</param>
    /// <param name="fieldName">Name of the field</param>
    /// <param name="resourceString">Resource string used for field label</param>
    private void AddField(FormInfo formInfo, string fieldName, string resourceString)
    {
        var field = new FormFieldInfo()
        {
            Name = fieldName,
            DataType = FieldDataType.Text,
            Caption = String.Format("{{${0}$}}", resourceString)
        };

        field.SetControlName(FormFieldControlName.LABEL);

        formInfo.AddFormItem(field);
    }


    /// <summary>
    /// Returns FormInfo with module information fields.
    /// </summary>
    private FormInfo CreateFormInfo()
    {
        var formInfo = new FormInfo();
        AddField(formInfo, "moduleDisplayName", "cms.modules.installpackage.title");
        AddField(formInfo, "moduleName", "cms.modules.installpackage.id");
        AddField(formInfo, "moduleDescription", "cms.modules.installpackage.description");
        AddField(formInfo, "moduleVersion", "cms.modules.installpackage.version");
        AddField(formInfo, "moduleAuthor", "cms.modules.installpackage.authors");

        return formInfo;
    }
}