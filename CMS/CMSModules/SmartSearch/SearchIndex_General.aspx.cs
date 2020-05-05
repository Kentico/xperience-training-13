using System;

using CMS.Search;
using CMS.UIControls;

[EditedObject(SearchIndexInfo.OBJECT_TYPE, "indexId")]
public partial class CMSModules_SmartSearch_SearchIndex_General : GlobalAdminPage
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        var searchIndex = EditedObject as SearchIndexInfo;
        ucIndexInfo.SearchIndex = searchIndex;

        ucSearchIndexEdit.AsyncIndexTaskStarted += (sender, args) => ucIndexInfo.LoadData();
        ucSearchIndexEdit.OnSaved += UcSearchIndexEdit_OnSaved;

        if (searchIndex.IndexProvider.Equals(SearchIndexInfo.AZURE_SEARCH_PROVIDER, StringComparison.OrdinalIgnoreCase))
        {
            infoMessage.Message = GetString("srch.status.tooltip.azuredelay");
            infoMessage.Visible = true;
        }
    }


    private void UcSearchIndexEdit_OnSaved(object sender, EventArgs e)
    {
        ucIndexInfo.SearchIndex = EditedObject as SearchIndexInfo;
        ucIndexInfo.LoadData();
    }
}
