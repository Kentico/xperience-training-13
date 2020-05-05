using System;

using CMS.Forums;
using CMS.Forums.Web.UI;


public partial class CMSModules_Forums_Controls_Layouts_Tree_Forums : ForumViewer
{
    /// <summary>
    /// Load data.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        ReloadData();

        base.OnLoad(e);
    }


    /// <summary>
    /// Reloads the data of the forum control.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        ForumContext.GroupID = GroupID;

        // Create where condition reflecting permissions
        string where = "";
        if (HideForumForUnauthorized)
        {
            where = ForumInfoProvider.CombineSecurityWhereCondition("(ForumOpen = 1) AND (ForumGroupID = " + GroupID + ")", CommunityGroupID);
        }
        else
        {
            where = "(ForumOpen = 1) AND (ForumGroupID = " + GroupID + ")";
        }

        listForums.OuterData = ForumContext.CurrentGroup;
        listForums.DataSource = ForumInfoProvider.GetForums().Where(where).OrderBy("ForumOrder ASC, ForumName ASC");
        listForums.DataBind();
    }
}