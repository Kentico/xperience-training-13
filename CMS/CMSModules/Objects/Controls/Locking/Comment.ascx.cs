using System;

using CMS.Helpers;

using System.Text;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Objects_Controls_Locking_Comment : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Action
    /// </summary>
    protected string Action
    {
        get
        {
            return QueryHelper.GetString("acname", null);
        }
    }


    /// <summary>
    /// Menu ID
    /// </summary>
    protected string MenuID
    {
        get
        {
            return QueryHelper.GetString("menuid", null);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions
        if (objectManager.IsActionAllowed(Action))
        {
            RegisterActionScript();
        }
        else
        {
            Visible = false;
        }
    }


    private void RegisterActionScript()
    {
        // Get js functions
        string checkinStr = "CheckIn_" + ValidationHelper.GetIdentifier(MenuID);
        string consStr = "CheckConsistency_" + ValidationHelper.GetIdentifier(MenuID);
        
        StringBuilder sb = new StringBuilder();
        sb.Append(@"
function ProcessAction(action) { 
    var comment = document.getElementById('", txtComment.ClientID, @"').value;
    switch(action) {
        case '", DocumentComponentEvents.CHECKIN, @"':
            if(wopener.", checkinStr, @") { wopener.", checkinStr, @"(comment); } else { ", consStr, @"(); }
        break;
    }
}"
);
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "action", sb.ToString(), true);
    }

    #endregion
}