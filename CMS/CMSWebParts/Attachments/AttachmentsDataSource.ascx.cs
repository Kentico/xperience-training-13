using System;

using CMS.Helpers;
using CMS.MacroEngine;
using CMS.PortalEngine.Web.UI;
using CMS.DocumentEngine;
using CMS.PortalEngine;

public partial class CMSWebParts_Attachments_AttachmentsDataSource : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets WHERE condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WhereCondition"), null);
        }
        set
        {
            SetValue("WhereCondition", value);
            srcAttach.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the SELECT part of the query.
    /// </summary>
    public string Columns
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Columns"), null);
        }
        set
        {
            SetValue("Columns", value);
            srcAttach.SelectedColumns = value;
        }
    }


    /// <summary>
    /// Gets or sets ORDER BY condition.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), null);
        }
        set
        {
            SetValue("OrderBy", value);
            srcAttach.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets top N selected documents.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), -1);
        }
        set
        {
            SetValue("SelectTopN", value);
            srcAttach.TopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the source filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), null);
        }
        set
        {
            SetValue("FilterName", value);
            srcAttach.SourceFilterName = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache item name.
    /// </summary>
    public override string CacheItemName
    {
        get
        {
            return base.CacheItemName;
        }
        set
        {
            base.CacheItemName = value;
            srcAttach.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, srcAttach.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            srcAttach.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache minutes.
    /// </summary>
    public override int CacheMinutes
    {
        get
        {
            return base.CacheMinutes;
        }
        set
        {
            base.CacheMinutes = value;
            srcAttach.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Group GUID (document field GUID) of the grouped attachments.
    /// </summary>
    public Guid AttachmentGroupGUID
    {
        get
        {
            string guidAndText = ValidationHelper.GetString(GetValue("AttachmentGroupGUID"), string.Empty);
            string[] values = guidAndText.Split('|');
            return (values.Length >= 1) ? ValidationHelper.GetGuid(values[0], Guid.Empty) : Guid.Empty;
        }
        set
        {
            SetValue("AttachmentGroupGUID", value);
            srcAttach.AttachmentGroupGUID = value;
        }
    }


    /// <summary>
    /// Culture code, such as en-us.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CultureCode"), null);
        }
        set
        {
            SetValue("CultureCode", value);
            srcAttach.CultureCode = value;
        }
    }


    /// <summary>
    /// Indicates if the document should be selected eventually from the default culture.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), true);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            srcAttach.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the alias path.
    /// </summary>
    public string Path
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Path"), string.Empty);
        }
        set
        {
            SetValue("Path", value);
            srcAttach.Path = value;
        }
    }


    /// <summary>
    /// Allows you to specify whether to check permissions of the current user. If the value is 'false' (default value) no permissions are checked. Otherwise, only nodes for which the user has read permission are displayed.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), false);
        }
        set
        {
            SetValue("CheckPermissions", value);
            srcAttach.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Load pages individually.
    /// </summary>
    public bool LoadPagesIndividually
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("LoadPagesIndividually"), false);
        }
        set
        {
            SetValue("LoadPagesIndividually", value);
        }
    }

    #endregion


    #region "Stop processing"

    /// <summary>
    /// Returns true if the control processing should be stopped.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            srcAttach.StopProcessing = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Initializes control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            if (string.IsNullOrEmpty(Path))
            {
                Path = DocumentContext.CurrentAliasPath;
            }
            srcAttach.Path = TreePathUtils.EnsureSingleNodePath(MacroResolver.ResolveCurrentPath(Path));
            srcAttach.OrderBy = OrderBy;
            srcAttach.TopN = SelectTopN;
            srcAttach.WhereCondition = WhereCondition;
            srcAttach.SelectedColumns = Columns;
            srcAttach.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);
            srcAttach.SourceFilterName = FilterName;
            srcAttach.GetBinary = false;
            srcAttach.AttachmentGroupGUID = AttachmentGroupGUID;
            if (string.IsNullOrEmpty(CultureCode))
            {
                srcAttach.CultureCode = DocumentContext.CurrentDocumentCulture.CultureCode;
            }
            else
            {
                srcAttach.CultureCode = CultureCode;
            }
            srcAttach.CombineWithDefaultCulture = CombineWithDefaultCulture;
            srcAttach.CheckPermissions = CheckPermissions;
            srcAttach.CacheItemName = CacheItemName;
            srcAttach.CacheMinutes = CacheMinutes;
            srcAttach.CacheDependencies = CacheDependencies;
            srcAttach.LoadPagesIndividually = LoadPagesIndividually;
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcAttach.ClearCache();
    }

    #endregion
}