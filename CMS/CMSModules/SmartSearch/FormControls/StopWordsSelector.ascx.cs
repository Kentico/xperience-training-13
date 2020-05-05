using System;

using CMS.Base;

using System.Linq;
using System.Web.UI.WebControls;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.Search;


/// <summary>
/// Represents a DDL selector form control which initializes its items based on the search index stop words files.
/// </summary>
public partial class CMSModules_SmartSearch_FormControls_StopWordsSelector : FormEngineUserControl
{
    /// <summary>
    /// Gets or sets the selected value
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureData();

            return drpStopWords.SelectedValue;
        }
        set
        {
            EnsureData();

            var stringValue = ValidationHelper.GetString(value, null);
            drpStopWords.SelectedValue = stringValue;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        EnsureData();
    }


    private void EnsureData()
    {
        if (drpStopWords.Items.Count == 0)
        {
            LoadData();
        }
    }


    private void LoadData()
    {
        // Get stop words directory
        var stopWordsPath = SearchIndexInfo.IndexPathPrefix + "_StopWords\\";
        if (!Directory.Exists(stopWordsPath))
        {
            return;
        }

        // Clear existing items
        drpStopWords.Items.Clear();

        // Add default item
        drpStopWords.Items.Add(new ListItem(GetString("general.defaultchoice"), string.Empty));

        // Get stop words files
        var fileNames = Directory.GetFiles(stopWordsPath, "*.txt");
        if (fileNames == null)
        {
            return;
        }

        // Get list items
        var listItems = fileNames.Select(name =>
        {
            var lastSlashIndex = name.LastIndexOfCSafe('\\');
            if (lastSlashIndex > -1)
            {
                name = name.Substring(lastSlashIndex + 1);
            }

            name = name.Substring(0, name.LastIndexOfCSafe('.'));

            return new ListItem(GetString("srch.stopwords." + name), name);
        });

        // Add list items to the DDL
        drpStopWords.Items.AddRange(listItems.ToArray());
    }
}