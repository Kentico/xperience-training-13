using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using CMS.Base;
using CMS.DataEngine;
using CMS.DataEngine.CollectionExtensions;
using CMS.Helpers;

using System.Linq;

using CMS.Base.Web.UI;
using CMS.Taxonomy;
using CMS.UIControls;


public partial class CMSModules_TagGroups_Controls_TagSelectorDialog : CMSUserControl
{
    #region "Variables"

    private int groupId;
    private string textBoxId;
    private ISet<string> selectedTags;
    private Hashtable mDialogProperties;
    private string mDialogIdentifier;

    #endregion

    
    #region "Properties"

    /// <summary>
    /// Gets dialog identifier.
    /// </summary>
    private string DialogIdentifier
    {
        get
        {
            return mDialogIdentifier ?? (mDialogIdentifier = QueryHelper.GetString("params", null));
        }
    }


    /// <summary>
    /// Dialog properties
    /// </summary>
    private Hashtable DialogProperties
    {
        get
        {
            if (mDialogProperties != null)
            {
                return mDialogProperties;
            }

            var identifier = QueryHelper.GetString("params", null);
            if (!String.IsNullOrEmpty(identifier))
            {
                mDialogProperties = WindowHelper.GetItem(identifier) as Hashtable;
            }

            return mDialogProperties;
        }
    }


    /// <summary>
    /// List of the tags which are not saved in database yet.
    /// </summary>
    private ISet<string> UnsavedTags
    {
        get
        {
            return (ISet<string>)ViewState["UnsavedTags"];
        }
        set
        {
            ViewState["UnsavedTags"] = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        PrepareParameters();

        // Setup UniGrid
        gridElem.ZeroRowsText = GetString("tags.tagselector.noold");
        gridElem.GridView.ShowHeader = false;
        gridElem.OnBeforeDataReload += gridElem_OnBeforeDataReload;
        gridElem.OnAfterDataReload += gridElem_OnAfterDataReload;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
    }


    public void SaveTags()
    {
        var resultTags = UnsavedTags;

        var items = gridElem.SelectedItems;
        if (items.Count > 0)
        {
            var savedTagNames = TagInfo.Provider.Get()
                                .WhereIn("TagID", items)
                                .Column("TagName")
                                .OrderBy("TagName")
                                .GetListResult<String>();

            // Combine all selected tags (unsaved + saved)
            resultTags = resultTags.Union(savedTagNames, StringComparer.InvariantCultureIgnoreCase).ToHashSetCollection();
        }

        if (resultTags != null)
        {
            var result = resultTags.Select(t => t.Contains(" ") ? String.Format("\"{0}\"", t.Trim('"')) : t).Join(", ");

            // Update parameters
            UpdateParameters(result);

            ltlScript.Text = ScriptHelper.GetScript("wopener.TS_SetTagsToTextBox(" + ScriptHelper.GetString(textBoxId) + ", " + ScriptHelper.GetString(result) + "); CloseDialog();");
        }
        else
        {
            ltlScript.Text = ScriptHelper.GetScript("CloseDialog();");
        }
    }


    #region "UniGrid Events"

    protected void gridElem_OnBeforeDataReload()
    {
        // Filter records by tag group ID
        var where = new WhereCondition().WhereEquals("TagGroupID", groupId);
        if (!String.IsNullOrEmpty(gridElem.CompleteWhereCondition))
        {
            where.Where(gridElem.CompleteWhereCondition);
        }
        gridElem.WhereCondition = where.ToString(true);
    }


    protected void gridElem_OnAfterDataReload()
    {
        if (DataHelper.DataSourceIsEmpty(gridElem.GridView.DataSource))
        {
            return;
        }

        if (RequestHelper.IsPostBack())
        {
            return;
        }

        // Get tag IDs for given tag names
        var tags = TagInfo.Provider.Get()
                                    .WhereIn("TagName", selectedTags)
                                    .WhereEquals("TagGroupID", groupId)
                                    .Columns("TagID", "TagName")
                                    .Select(row => new Tuple<string, string>(row["TagID"].ToString(), row["TagName"].ToString())).ToList();

        UnsavedTags = selectedTags.Except(tags.Select(t => t.Item2)).ToHashSetCollection();

        gridElem.SelectedItems = tags.Select(t => t.Item1).ToList();
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (sourceName.Equals("tagname", StringComparison.OrdinalIgnoreCase))
        {
            DataRowView drv = (DataRowView)parameter;
            string tagName = ValidationHelper.GetString(drv["TagName"], "");
            string tagId = ValidationHelper.GetString(drv["TagID"], "");
            if ((tagName != "") && (tagName != tagId))
            {
                string tagCount = ValidationHelper.GetString(drv["TagCount"], "");
                string tagText = $"{HTMLHelper.HTMLEncode(tagName)} ({tagCount})";

                // Create link with onclick event which call onclick event of checkbox in the same row
                return $"<a href=\"#\" onclick=\"var c=$cmsj(this).parents('tr:first').find('input:checkbox'); c.prop('checked', !c.prop('checked')).get(0).onclick(); return false;\">{tagText}</a>";
            }
        }
        return "";
    }

    #endregion

    
    #region "Private methods"

    private void UpdateParameters(string tags)
    {
        DialogProperties["tags"] = tags;
        WindowHelper.Add(DialogIdentifier, DialogProperties);
    }


    private void PrepareParameters()
    {
        var props = DialogProperties;
        if (props == null)
        {
            return;
        }

        // Get group ID
        groupId = props["group"].ToInteger(0);

        // Get id of the base selector textbox
        textBoxId = props["textbox"].ToString(String.Empty);

        // Get selected tags
        var oldTags = props["tags"].ToString(String.Empty);
        selectedTags = TagHelper.GetTags(oldTags);
    }

    #endregion
}
