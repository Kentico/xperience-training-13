using System;
using System.Collections.Generic;

using CMS.Base;
using CMS.Base.Web.UI;

using System.Text;
using System.Web;
using System.Web.UI;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;
using CMS.Core;
using CMS.Helpers.Caching.Abstractions;

public partial class CMSModules_System_Debug_CacheItemsGrid : CMSUserControl, IUniPageable, IPostBackEventHandler
{
    #region "Properties"

    /// <summary>
    /// Event fired when some cache item is deleted.
    /// </summary>
    public event EventHandler OnItemDeleted;


    /// <summary>
    /// All cache items array.
    /// </summary>
    public IEnumerable<string> AllItems
    {
        get;
        set;
    }


    /// <summary>
    /// If true, grid shows the dummy items.
    /// </summary>
    public bool ShowDummyItems
    {
        get;
        set;
    }


    /// <summary>
    /// Returns the pager control.
    /// </summary>
    public UIPager PagerControl
    {
        get
        {
            return pagerItems;
        }
    }


    /// <summary>
    /// Returns total number of items.
    /// </summary>
    public int TotalItems
    {
        get;
        private set;
    }


    /// <summary>
    /// Returns total number of filtered items.
    /// </summary>
    private int TotalFilteredItems
    {
        get;
        set;
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        pagerItems.PagedControl = this;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Register delete script
        string script =
            "function DeleteCacheItem(key) { if (confirm(" + ScriptHelper.GetString(GetString("general.confirmdelete")) + ")) { document.getElementById('" + hdnKey.ClientID + "').value = key; " + Page.ClientScript.GetPostBackEventReference(this, "delete") + " } }\n" +
            "function Refresh() { " + Page.ClientScript.GetPostBackEventReference(this, "refresh") + " }\n" +
            "function Show(key) { var url = '" + ResolveUrl("System_ViewObject.aspx?source=cache&key=") + "' + key; modalDialog(url, 'CacheItem', '1000', '700');}" +
            "function Debug(key) { var url = '" + ResolveUrl("~/CMSAdminControls/UI/Macros/Dialogs/ObjectBrowser.aspx?mode=values&expr=") + "Debug(CacheItem(\"' + key + '\"))'; window.open(url); }";

        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DeleteCacheItem", ScriptHelper.GetScript(script));
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        pagerItems.UniPager.CurrentPage = 1;
    }


    public void ReloadData()
    {
        if (ShowDummyItems)
        {
            lblKey.Text = GetString("Administration-System.CacheInfoDummyKey");
            plcData.Visible = false;
        }
        else
        {
            lblKey.Text = GetString("Administration-System.CacheInfoKey");
            lblData.Text = GetString("Administration-System.CacheInfoData");
            lblExpiration.Text = GetString("Administration-System.CacheInfoExpiration");
            lblPriority.Text = GetString("Administration-System.CacheInfoPriority");

            plcData.Visible = true;
            plcContainer.Visible = CacheDebug.Settings.Enabled;
        }

        lblAction.Text = GetString("General.Action");

        // Build the table
        StringBuilder sb = new StringBuilder();

        // Prepare the indexes for paging
        int pageSize = pagerItems.CurrentPageSize;

        int startIndex = (pagerItems.CurrentPage - 1) * pageSize + 1;
        int endIndex = startIndex + pageSize;

        // Process all items
        int filteredCount = 0;
        int count = 0;
        bool all = (endIndex <= startIndex);

        var cacheAccessor = Service.Resolve<ICacheAccessor>();

        string search = txtFilter.Text;
        
        if (AllItems != null)
        {
            // Process dummy keys
            foreach (string key in AllItems)
            {
                count++;

                if (key.IndexOfCSafe(search, true) >= 0)
                {
                    if (!String.IsNullOrEmpty(key) || !ShowDummyItems)
                    {
                        // Process the key
                        object value = cacheAccessor.Get(key);
                        CacheItemContainer container = null;

                        // Handle the container
                        if (value is CacheItemContainer)
                        {
                            container = (CacheItemContainer)value;
                            value = container.Data;
                        }

                        if ((ShowDummyItems && value == CacheHelper.DUMMY_KEY) || (!ShowDummyItems && value != CacheHelper.DUMMY_KEY))
                        {
                            filteredCount++;

                            if (all || (filteredCount >= startIndex) && (filteredCount < endIndex))
                            {
                                RenderItem(sb, key, container, value, ShowDummyItems);
                            }
                        }
                    }
                }
            }

            TotalItems = count;
            TotalFilteredItems = filteredCount;

            lblInfo.Visible = (filteredCount <= 0);
            plcItems.Visible = (filteredCount > 0);
            pnlSearch.Visible = (count > 2);

            ltlCacheInfo.Text = sb.ToString();

            // Call page binding event
            if (OnPageBinding != null)
            {
                OnPageBinding(this, null);
            }
        }
    }


    /// <summary>
    /// Renders the particular cache item.
    /// </summary>
    protected void RenderItem(StringBuilder sb, string key, CacheItemContainer container, object value, bool dummy)
    {
        sb.Append("<tr><td class=\"unigrid-actions\">");

        // Get the action
        sb.Append(GetViewAction(key));
        sb.Append(GetDeleteAction(key));
        sb.Append(GetDebugAction(key));

        sb.Append("</td><td><span title=\"" + HTMLHelper.HTMLEncode(key) + "\">");
        string keyTag = TextHelper.LimitLength(key, 100);
        sb.Append(HTMLHelper.HTMLEncode(keyTag));
        sb.Append("</span></td>");

        if (!dummy)
        {
            sb.Append("<td>");

            // Render the value
            if ((value != null) && (value != DBNull.Value))
            {
                sb.Append(HttpUtility.HtmlEncode(DataHelper.GetObjectString(value, 100)));
            }
            else
            {
                sb.Append("null");
            }

            sb.Append("</td>");

            if (CacheDebug.Settings.Enabled)
            {
                // Expiration
                sb.Append("<td>");
                if (container != null)
                {
                    if (container.AbsoluteExpiration != CacheConstants.NoAbsoluteExpiration)
                    {
                        sb.Append(container.AbsoluteExpiration);
                    }
                    else
                    {
                        sb.Append(container.SlidingExpiration);
                    }
                }
                sb.Append("</td>");

                // Expiration
                sb.Append("<td>");
                if (container != null)
                {
                    sb.Append(container.Priority);
                }
                sb.Append("</td>");
            }
        }

        sb.Append("</tr>");
    }


    /// <summary>
    /// Gets the debug action.
    /// </summary>
    /// <param name="key">Cache key</param>
    private string GetDebugAction(string key)
    {
        var button = new CMSGridActionButton
        {
            IconCssClass = "icon-bug",
            ToolTip = GetString("General.Debug"),
            OnClientClick = String.Format("Debug('{0}'); return false;", Server.UrlEncode(ScriptHelper.GetString(key, false)))
        };

        return button.GetRenderedHTML();
    }



    /// <summary>
    /// Gets the view action.
    /// </summary>
    /// <param name="key">Cache key</param>
    private string GetViewAction(string key)
    {
        var button = new CMSGridActionButton
        {
            IconCssClass = "icon-eye",
            IconStyle = GridIconStyle.Allow,
            ToolTip = GetString("General.View"),
            OnClientClick = String.Format("Show('{0}'); return false;", Server.UrlEncode(key))
        };

        return button.GetRenderedHTML();
    }


    /// <summary>
    /// Gets the delete action.
    /// </summary>
    /// <param name="key">Cache key</param>
    protected string GetDeleteAction(string key)
    {
        var button = new CMSGridActionButton
        {
            IconCssClass = "icon-bin",
            IconStyle = GridIconStyle.Critical,
            ToolTip = GetString("General.Delete"),
            OnClientClick = String.Format("DeleteCacheItem({0}); return false;", ScriptHelper.GetString(key))
        };

        return button.GetRenderedHTML();
    }


    #region "IUniPageable Members"

    /// <summary>
    /// Pager data item object.
    /// </summary>
    public object PagerDataItem
    {
        get
        {
            return null;
        }
        set
        {
        }
    }


    /// <summary>
    /// Pager control.
    /// </summary>
    public UniPager UniPagerControl
    {
        get;
        set;
    }


    /// <summary>
    /// Occurs when the control bind data.
    /// </summary>
    public event EventHandler<EventArgs> OnPageBinding;


    /// <summary>
    /// Occurs when the pager change the page and current mode is postback => reload data
    /// </summary>
    public event EventHandler<EventArgs> OnPageChanged;


    /// <summary>
    /// Evokes control databind.
    /// </summary>
    public void ReBind()
    {
        if (OnPageChanged != null)
        {
            OnPageChanged(this, null);
        }

        ReloadData();
    }


    /// <summary>
    /// Gets or sets the number of result. Enables proceed "fake" datasets, where number 
    /// of results in the dataset is not correspondent to the real number of results
    /// This property must be equal -1 if should be disabled
    /// </summary>
    public int PagerForceNumberOfResults
    {
        get
        {
            return TotalFilteredItems;
        }
        set
        {
        }
    }

    #endregion


    #region "IPostBackEventHandler Members"

    /// <summary>
    /// Processes the postback.
    /// </summary>
    /// <param name="eventArgument">Event argument</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        // Delete the item
        string key = hdnKey.Value;
        if (!String.IsNullOrEmpty(key))
        {
            CacheHelper.Remove(key);

            // Raise the OnItemDeleted event
            if (OnItemDeleted != null)
            {
                OnItemDeleted(this, null);
            }
        }
    }

    #endregion
}
