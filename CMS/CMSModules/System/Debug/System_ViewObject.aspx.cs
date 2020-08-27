using System;
using System.Web.Caching;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.Helpers;
using CMS.Helpers.Caching.Abstractions;
using CMS.UIControls;


[Title("ViewObject.Title")]
public partial class CMSModules_System_Debug_System_ViewObject : CMSDebugPage
{
    #region "Variables"

    private string mKey;
    private string mSource;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.IsDialog = true;

        ScriptHelper.RegisterWOpenerScript(this);

        // Delete all action
        CurrentMaster.HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("general.delete"),
            OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("general.confirmdelete")) + ")) return false;",
            Tooltip = GetString("general.delete"),
            CommandName = "delete"
        });
        CurrentMaster.HeaderActions.ActionPerformed += actionsElem_ActionPerformed;

        gridDependencies.PagerControl.DefaultPageSize = 10;
        gridDependencies.OnItemDeleted += gridDependencies_OnItemDeleted;

        mSource = QueryHelper.GetString("source", "");
        mKey = QueryHelper.GetString("key", "");

        ReloadData();
    }


    protected void gridDependencies_OnItemDeleted(object sender, EventArgs e)
    {
        ReloadData(true);
    }


    /// <summary>
    /// Reloads the cache item view.
    /// </summary>
    /// <param name="objectDeleted">True if object was recently deleted</param>
    protected void ReloadData(bool objectDeleted = false)
    {
        object obj = null;
        pnlCacheItem.Visible = !objectDeleted;
        CurrentMaster.HeaderActions.Visible = !objectDeleted;

        switch (mSource.ToLowerCSafe())
        {
            case "cache":
                {
                    var cacheAccessor = Service.Resolve<ICacheAccessor>();
                    
                    // Get the item from cache
                    obj = cacheAccessor.Get(mKey);

                    // Take the object from the cache
                    if ((obj != null) && !objectDeleted)
                    {
                        if (obj is CacheItemContainer)
                        {
                            // Setup the advanced information
                            CacheItemContainer container = (CacheItemContainer)obj;
                            obj = container.Data;

                            // Get the inner value
                            obj = CacheHelper.GetInnerValue(obj);

                            ltlKey.Text = HTMLHelper.HTMLEncode(mKey);
                            ltlPriority.Text = container.Priority.ToString();
                            if (container.AbsoluteExpiration != Cache.NoAbsoluteExpiration)
                            {
                                ltlExpiration.Text = container.AbsoluteExpiration.ToString();
                            }
                            else
                            {
                                ltlExpiration.Text = container.SlidingExpiration.ToString();
                            }

                            if (container.Dependencies != null)
                            {
                                gridDependencies.AllItems = container.Dependencies.CacheKeys;
                                gridDependencies.ReloadData();
                            }

                            gridDependencies.Visible = gridDependencies.TotalItems > 0;
                            ltlDependencies.Visible = gridDependencies.TotalItems == 0;

                            pnlCacheItem.Visible = true;
                        }

                        pnlBody.Visible = true;
                    }
                    else if (objectDeleted)
                    {
                        ShowConfirmation(GetString("general.wasdeleted"), true);
                    }
                    else
                    {
                        ShowError(GetString("general.objectnotfound"));
                    }
                }
                break;
        }

        objElem.Object = obj;
    }


    private void actionsElem_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "delete":
                // Delete the item from the cache
                if (!string.IsNullOrEmpty(mKey))
                {
                    CacheHelper.Remove(mKey);
                    ReloadData(true);
                }
                break;
        }
    }

    #endregion
}
