using System;
using System.Data;

using CMS.CustomTables;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Class_ClassSites : CMSUserControl
{
    private int mClassId;
    private string mTitleString;
    private bool mCheckLicense = true;
    private string currentValues = String.Empty;


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether license should be chcecked.
    /// </summary>
    public bool CheckLicense
    {
        get
        {
            return mCheckLicense;
        }
        set
        {
            mCheckLicense = value;
        }
    }


    /// <summary>
    /// Gets or sets class id.
    /// </summary>
    public int ClassId
    {
        get
        {
            return mClassId;
        }
        set
        {
            mClassId = value;
        }
    }


    /// <summary>
    /// Gets or sets the title at the top of the page.
    /// </summary>
    public string TitleString
    {
        get
        {
            return mTitleString;
        }
        set
        {
            mTitleString = value;
        }
    }

    #endregion


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (ClassId > 0)
        {
            // initializes labels
            headTitle.Text = TitleString;

            // Get the user sites
            currentValues = GetClassSites();

            if (!RequestHelper.IsPostBack())
            {
                usSites.Value = currentValues;
            }

            usSites.OnSelectionChanged += usSites_OnSelectionChanged;
        }
        else
        {
            // Hide control if not properly set
            Visible = false;
        }
    }


    /// <summary>
    /// Returns string with class sites.
    /// </summary>    
    private string GetClassSites()
    {
        DataSet ds = ClassSiteInfo.Provider.Get().WhereEquals("ClassID", ClassId).Column("SiteID");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            return TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "SiteID"));
        }

        return String.Empty;
    }


    /// <summary>
    /// Handles site selector selection change event.
    /// </summary>
    protected void usSites_OnSelectionChanged(object sender, EventArgs e)
    {
        SaveSites();
    }


    protected void SaveSites()
    {
        // Remove old items
        string newValues = ValidationHelper.GetString(usSites.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items to site
                foreach (string item in newItems)
                {
                    int siteId = ValidationHelper.GetInteger(item, 0);

                    ClassSiteInfo.Provider.Remove(ClassId, siteId);
                }
            }
        }


        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            bool falseValues = false;

            // Add all new items to site
            foreach (string item in newItems)
            {
                int siteId = ValidationHelper.GetInteger(item, 0);

                SiteInfo si = SiteInfo.Provider.Get(siteId);
                if (si != null)
                {
                    // Check license
                    if (CheckLicense && !CustomTableItemProvider.LicenseVersionCheck(si.DomainName, ObjectActionEnum.Insert))
                    {
                        if (ClassSiteInfo.Provider.Get(ClassId, siteId) == null)
                        {
                            // Show error message
                            ShowError(GetString("LicenseVersion.CustomTables"));
                            falseValues = true;
                            continue;
                        }
                    }

                    try
                    {
                        ClassSiteInfo.Provider.Add(ClassId, siteId);
                    }
                    catch (Exception ex)
                    {
                        // Show error message
                        ShowError(ex.Message);

                        return;
                    }
                }
            }

            // If some of sites could not be assigned reload selector value
            if (falseValues)
            {
                usSites.Value = GetClassSites();
                usSites.Reload(true);
            }
        }

        if (CheckLicense)
        {
            CustomTableItemProvider.ClearLicensesCount(true);
        }

        // Show message
        ShowChangesSaved();
    }
}