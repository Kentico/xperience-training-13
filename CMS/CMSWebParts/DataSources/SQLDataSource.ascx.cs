using System;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_DataSources_SQLDataSource : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gest or sest the cache item name.
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
            srcSQL.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, srcSQL.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            srcSQL.CacheDependencies = value;
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
            srcSQL.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the server authentication mode.
    /// </summary>
    public int AuthenticationMode
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("AuthenticationMode"), 0);
        }
        set
        {
            SetValue("AuthenticationMode", value);
            SQLServerAuthenticationModeEnum mode = (value == 0) ? SQLServerAuthenticationModeEnum.SQLServerAuthentication : SQLServerAuthenticationModeEnum.WindowsAuthentication;
            srcSQL.AuthenticationMode = mode;
        }
    }


    /// <summary>
    /// Gets or sets query text.
    /// </summary>
    public string QueryText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("QueryText"), null);
        }
        set
        {
            SetValue("QueryText", value);
            srcSQL.QueryText = value;
        }
    }


    /// <summary>
    /// Gets or sets query type. (Standard query or stored procedure.).
    /// </summary>
    public int QueryType
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("QueryType"), 0);
        }
        set
        {
            SetValue("QueryType", value);
            QueryTypeEnum type = (value == 0) ? QueryTypeEnum.SQLQuery : QueryTypeEnum.StoredProcedure;
            srcSQL.QueryType = type;
        }
    }


    /// <summary>
    /// Gets or sets complete connection string.
    /// </summary>
    public string ConnectionString
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ConnectionString"), null);
        }
        set
        {
            SetValue("ConnectionString", value);
            srcSQL.ConnectionString = value;
        }
    }


    /// <summary>
    /// Gets or sets database name.
    /// </summary>
    public string DatabaseName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DatabaseName"), null);
        }
        set
        {
            SetValue("DatabaseName", value);
            srcSQL.DatabaseName = value;
        }
    }


    /// <summary>
    /// Gets or sets database server name.
    /// </summary>
    public string ServerName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ServerName"), null);
        }
        set
        {
            SetValue("ServerName", value);
            srcSQL.ServerName = value;
        }
    }


    /// <summary>
    /// Gets or sets user name.
    /// </summary>
    public string UserName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UserName"), null);
        }
        set
        {
            SetValue("UserName", value);
            srcSQL.UserName = value;
        }
    }


    /// <summary>
    /// Gets or sets password.
    /// </summary>
    public string Password
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Password"), null);
        }
        set
        {
            SetValue("Password", value);
            srcSQL.Password = value;
        }
    }


    /// <summary>
    /// Gets or sets connection language.
    /// </summary>
    public string Language
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Language"), "English");
        }
        set
        {
            SetValue("Language", value);
            srcSQL.Language = value;
        }
    }


    /// <summary>
    /// Gets or sets connection timeout (240 seconds by default).
    /// </summary>
    public int Timeout
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Timeout"), srcSQL.Timeout);
        }
        set
        {
            SetValue("Timeout", value);
            srcSQL.Timeout = value;
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            // Document properties
            srcSQL.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);
            srcSQL.CacheItemName = CacheItemName;
            srcSQL.CacheDependencies = CacheDependencies;
            srcSQL.CacheMinutes = CacheMinutes;

            srcSQL.AuthenticationMode = (AuthenticationMode == 0) ? SQLServerAuthenticationModeEnum.SQLServerAuthentication : SQLServerAuthenticationModeEnum.WindowsAuthentication;
            srcSQL.ServerName = ServerName;
            srcSQL.Password = EncryptionHelper.DecryptData(Password);
            srcSQL.UserName = UserName;
            srcSQL.DatabaseName = DatabaseName;
            srcSQL.QueryType = (QueryType == 0) ? QueryTypeEnum.SQLQuery : QueryTypeEnum.StoredProcedure;
            srcSQL.QueryText = QueryText;
            srcSQL.ConnectionString = ConnectionString;
            srcSQL.Language = Language;
            srcSQL.Timeout = Timeout;
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcSQL.ClearCache();
    }
}