using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.Helpers;
using CMS.Helpers.Caching.Abstractions;
using CMS.Helpers.Internal;
using CMS.UIControls;

[Title("ViewObject.Title")]
public partial class CMSModules_System_Debug_System_ViewObject : CMSDebugPage
{
    private string mKey;
    private string mSource;


    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        PageTitle.IsDialog = true;

        ScriptHelper.RegisterWOpenerScript(this);

        // Delete all action
        if (!ShowLiveSiteData)
        {
            CurrentMaster.HeaderActions.AddAction(new HeaderAction
            {
                Text = GetString("general.delete"),
                OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("general.confirmdelete")) + ")) return false;",
                Tooltip = GetString("general.delete"),
                CommandName = "delete"
            });
            CurrentMaster.HeaderActions.ActionPerformed += actionsElem_ActionPerformed;

            gridDependencies.OnItemDeleted += gridDependencies_OnItemDeleted;
        }

        gridDependencies.PagerControl.DefaultPageSize = 10;
        gridDependencies.PagerControl.ShowPageSize = false;

        mSource = QueryHelper.GetString("source", "");
        mKey = QueryHelper.GetString("key", "");

        await ReloadData(false);
    }


    protected async void gridDependencies_OnItemDeleted(object sender, EventArgs e)
    {
        await ReloadData(true);
    }


    /// <summary>
    /// Reloads the cache item view.
    /// </summary>
    /// <param name="objectDeleted">True if the object was recently deleted.</param>
    private async Task ReloadData(bool objectDeleted = false)
    {
        object obj = null;

        switch (mSource.ToLowerInvariant())
        {
            case "cache":
                {
                    // Get the item from cache
                    obj = ShowLiveSiteData ? await new LiveSiteDebugProcessor().GetCacheItemAsync(Server.UrlEncode(mKey)) : Service.Resolve<ICacheAccessor>().Get(mKey);

                    // Take the object from the cache
                    if ((obj != null) && !objectDeleted)
                    {
                        // Setup the advanced information
                        if (obj is CacheItemContainer container)
                        {
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
                                gridDependencies.AllItems = container.Dependencies.CacheKeys?.Select(c => new CacheItemRowContainer(c, new DummyItem(), null));
                                gridDependencies.ReloadData();
                            }

                            gridDependencies.Visible = gridDependencies.TotalItems > 0;
                            ltlDependencies.Visible = gridDependencies.TotalItems == 0;

                            pnlCacheItem.Visible = true;
                        }

                        pnlBody.Visible = true;
                    }
                    else if (objectDeleted || obj == null)
                    {
                        pnlCacheItem.Visible = false;
                        CurrentMaster.HeaderActions.Visible = false;

                        if (objectDeleted)
                        {
                            ShowConfirmation(GetString("general.wasdeleted"), true);
                        }
                        else
                        {
                            ShowWarning(GetString("general.objectnotfound"));
                        }
                    }
                }
                break;
        }

        objElem.Object = obj;
    }


    private async void actionsElem_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "delete":
                // Delete the item from the cache
                if (!String.IsNullOrEmpty(mKey))
                {
                    CacheHelper.Remove(mKey);
                    await ReloadData(true);
                }
                break;
        }
    }
}
