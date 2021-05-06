using System;
using System.Linq;
using System.Threading.Tasks;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_System_Debug_System_DebugCacheItems : CMSDebugPage
{
    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (ShowLiveSiteData)
        {
            pnlHeaderActions.Parent.Controls.Clear();
        }
        else
        {
            btnClear.Text = GetString("Administration-System.btnClearCache");
        }

        await ReloadData();
    }


    /// <summary>
    /// Loads the data.
    /// </summary>
    protected async Task ReloadData()
    {
        var keyList = ShowLiveSiteData ? await new LiveSiteDebugProcessor().GetAllCacheItemsAsync() : CacheHelper.GetCacheItemRows();

        keyList = keyList?.OrderBy(g => g.Key);

        // Load the grids
        gridItems.AllItems = keyList;
        gridItems.ReloadData();

        gridDummy.AllItems = keyList;
        gridDummy.ReloadData();
    }


    protected async void btnClear_Click(object sender, EventArgs e)
    {
        CacheHelper.ClearCache();

        await ReloadData().ConfigureAwait(false);
    }
}
