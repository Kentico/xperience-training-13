using System;
using System.Collections;
using System.Data;
using System.Linq;
using CMS.Base;
using CMS.CMSImportExport;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement("Licenses", "ExportListOfDomains")]
public partial class CMSModules_Licenses_Pages_License_Export_Domains : GlobalAdminPage
{
    #region "Page events"

    /// <summary>
    /// Page load event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup page title text and image
        PageTitle.TitleText = GetString("license.export");

        // Initialize breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("Licenses_License_New.Licenses"),
            RedirectUrl = UIContextHelper.GetElementUrl("Licenses", "Licenses", false),
        });
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("license.export"),
        });

        // Resource strings
        rfvFileName.ErrorMessage = GetString("license.export.filenameempty");

        // Default file name
        if (!RequestHelper.IsPostBack())
        {
            ShowInformation(GetString("license.export.info"));
            txtFileName.Text = ImportExportHelper.GenerateExportFileName("list_of_domains", ".txt");
        }
    }


    /// <summary>
    /// Button Ok click.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Validate file name
        string fileName = txtFileName.Text.Trim();
        string errorMessage = new Validator().NotEmpty(fileName, GetString("lincense.export.filenameempty")).IsFileName(fileName, GetString("license.export.notvalidfilename")).Result;
        if (!string.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
        }
        else
        {
            try
            {
                // Create writers
                string path = ImportExportHelper.GetSiteUtilsFolder() + "Export\\" + fileName;
                DirectoryHelper.EnsureDiskPath(path, SystemContext.WebApplicationPhysicalPath);

                using (FileStream file = FileStream.New(path, FileMode.Create))
                {
                    using (StreamWriter sw = StreamWriter.New(file))
                    {
                        ArrayList allRecords = new ArrayList();

                        // Get all sites
                        DataSet sites = SiteInfo.Provider.Get().Columns("SiteID", "SiteDomainName", "SitePresentationURL");
                        if (!DataHelper.DataSourceIsEmpty(sites))
                        {
                            foreach (DataRow dr in sites.Tables[0].Rows)
                            {
                                var domain = ValidationHelper.GetString(dr["SiteDomainName"], "");
                                AddRecord(allRecords, sw, domain);

                                var presentationUrl = ValidationHelper.GetString(dr["SitePresentationURL"], "");
                                AddRecord(allRecords, sw, presentationUrl);

                                // Add all domain aliases
                                var siteId = ValidationHelper.GetInteger(dr["SiteID"], 0);
                                var aliases = SiteDomainAliasInfoProvider.GetDomainAliases(siteId);
                                foreach (var alias in aliases)
                                {
                                    // Get domain
                                    var record = alias.SiteDomainAliasType == SiteDomainAliasTypeEnum.Administration ? alias.SiteDomainAliasName : alias.SiteDomainPresentationUrl;
                                    AddRecord(allRecords, sw, record);
                                }
                            }
                        }
                    }
                }

                // Output
                string url = ImportExportHelper.GetExportPackageUrl(fileName, SiteContext.CurrentSiteName);
                string downloadLink = (url != null) ? String.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", url, GetString("license.export.clicktodownload")) : "";
                string storageName = (StorageHelper.IsExternalStorage(path)) ? GetString("Export.StorageProviderName." + StorageHelper.GetStorageProvider(path).Name) : "";
                string relativePath = ImportExportHelper.GetSiteUtilsFolderRelativePath() + "Export/" + txtFileName.Text;

                ShowConfirmation(GetString("license.export.exported"));
                ShowInformation(String.Format(GetString("license.export.download"), storageName, relativePath, downloadLink));

                plcTextBox.Visible = false;                
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }


    private void AddRecord(ArrayList allRecords, StreamWriter sw, string domain)
    {
        if (string.IsNullOrEmpty(domain))
        {
            return;
        }

        domain = GetDomain(domain);
        // Add to file
        if (!allRecords.Contains(domain))
        {
            sw.WriteLine(domain);
            allRecords.Add(domain);
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Returns domain without www, protocol, port and application path.
    /// </summary>
    /// <param name="domain">String containing domain to be processed</param>
    private string GetDomain(string domain)
    {
        // Trim to domain
        domain = URLHelper.RemoveProtocol(domain);
        domain = URLHelper.RemoveWWW(domain);

        // Virtual directory
        int slash = domain.IndexOfCSafe('/');
        if (slash > -1)
        {
            domain = domain.Substring(0, slash);
        }

        domain = URLHelper.RemovePort(domain);

        return domain;
    }

    #endregion
}