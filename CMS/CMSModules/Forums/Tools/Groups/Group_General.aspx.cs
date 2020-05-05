using System;

using CMS.Core;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.FORUMS, "ForumGroupEditTab_General")]
public partial class CMSModules_Forums_Tools_Groups_Group_General : CMSForumsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int groupID = QueryHelper.GetInteger("objectid", 0);
        ForumContext.CheckSite(groupID, 0, 0);

        groupEdit.GroupID = groupID;
        groupEdit.IsLiveSite = false;
    }
}