using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Forums_Controls_ThreadMove : ForumViewer
{
    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets the current thread id if ForumCOntext.CurrentThread is not available e.g. in tree layout.
    /// </summary>
    public int CurrentThread
    {
        get
        {
            return QueryHelper.GetInteger("moveto", SelectedThreadID);
        }
    }


    /// <summary>
    /// Gets or sets the thread id.
    /// </summary>
    public int SelectedThreadID
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectedThreadID"), 0);
        }
        set
        {
            SetValue("SelectedThreadID", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether controls is in administration mode.
    /// </summary>
    public bool AdminMode
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AdminMode"), false);
        }
        set
        {
            SetValue("AdminMode", value);
        }
    }

    #endregion


    /// <summary>
    /// Occurs when thread  is moved.
    /// </summary>
    public event EventHandler TopicMoved;


    protected void Page_Load(object sender, EventArgs e)
    {
        if ((ForumContext.CurrentMode == ForumMode.TopicMove) || (AdminMode))
        {
            CopyValuesFromParent(this);

            btnMove.Click += btnMove_Click;

            if (!RequestHelper.IsPostBack())
            {
                LoadMoveTopicDropdown();
            }
            else
            {
                // Display message if no forum exists
                if (drpMoveToForum.Items.Count == 0)
                {
                    plcMoveInner.Visible = false;
                    ShowInformation(GetString("Forums.NoForumToMoveIn"));
                }
            }
        }
    }


    /// <summary>
    /// Topic move action handler.
    /// </summary>
    protected void btnMove_Click(object sender, EventArgs e)
    {
        int forumId = ValidationHelper.GetInteger(drpMoveToForum.SelectedValue, 0);
        if (forumId > 0)
        {
            ForumPostInfo fpi = ForumContext.CurrentThread;
            if ((fpi == null) && (CurrentThread > 0))
            {
                fpi = ForumPostInfoProvider.GetForumPostInfo(CurrentThread);
            }

            if (fpi != null)
            {
                // Move the thread
                ForumPostInfoProvider.MoveThread(fpi, forumId);

                plcMoveInner.Visible = false;

                // Generate back button
                ltlMoveBack.Text = GetLink(null, GetString("general.back"), "ActionLink", ForumActionType.Thread);

                string targetForumName = drpMoveToForum.SelectedItem.Text.TrimStart(' ');
                ForumInfo fi = ForumInfoProvider.GetForumInfo(forumId);
                if (fi != null)
                {
                    targetForumName = HTMLHelper.HTMLEncode(fi.ForumDisplayName);
                }

                // Display info
                ShowConfirmation(String.Format(GetString("forum.thread.topicmoved"), targetForumName));

                SetValue("TopicMoved", true);

                TopicMoved?.Invoke(this, null);
            }
        }
        else
        {
            ShowError(GetString("forum.thread.movetopic.selectforum"));
        }
    }


    /// <summary>
    /// Loads the forums DropDownList for topic move.
    /// </summary>
    private void LoadMoveTopicDropdown()
    {
        if (drpMoveToForum.Items.Count > 0)
        {
            return;
        }

        ForumPostInfo fpi = ForumContext.CurrentThread;
        if ((fpi == null) && (CurrentThread > 0))
        {
            fpi = ForumPostInfoProvider.GetForumPostInfo(CurrentThread);
        }

        if (fpi != null)
        {
            bool isOk = AdminMode || ((ForumContext.CurrentForum != null) && (ForumContext.CurrentForum.ForumID == fpi.PostForumID));
            if (isOk)
            {
                var currentForumId = fpi.PostForumID;

                ForumInfo fi = ForumInfoProvider.GetForumInfo(currentForumId);
                if (fi != null)
                {
                    ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(fi.ForumGroupID);
                    if (fgi != null)
                    {
                        var whereCondition = new WhereCondition().WhereNotStartsWith("GroupName", "AdHoc").WhereEquals("GroupSiteID", SiteID);

                        if (fgi.GroupGroupID > 0)
                        {
                            whereCondition.WhereEquals("GroupGroupID", fgi.GroupGroupID);
                        }
                        else
                        {
                            whereCondition.WhereNull("GroupGroupID");
                        }

                        DataSet dsGroups = ForumGroupInfoProvider.GetForumGroups().Where(whereCondition).OrderBy("GroupDisplayName").Columns("GroupID, GroupDisplayName");

                        if (!DataHelper.DataSourceIsEmpty(dsGroups))
                        {
                            Hashtable forums = new Hashtable();

                            // Get all forums for selected groups
                            var groupWhereCondition = new WhereCondition().WhereIn("ForumGroupID", new ObjectQuery<ForumGroupInfo>().Where(whereCondition).Column("GroupID"));
                            DataSet dsForums = ForumInfoProvider.GetForums()
                                                                .WhereEquals("ForumOpen", 1)
                                                                .WhereNotEquals("ForumID", currentForumId)
                                                                .Where(groupWhereCondition)
                                                                .OrderBy("ForumDisplayName")
                                                                .Columns("ForumID, ForumDisplayName, ForumGroupID")
                                                                .TypedResult;

                            if (!DataHelper.DataSourceIsEmpty(dsForums))
                            {
                                // Load forums into hash table
                                foreach (DataRow drForum in dsForums.Tables[0].Rows)
                                {
                                    int groupId = Convert.ToInt32(drForum["ForumGroupID"]);
                                    List<string[]> forumNames = forums[groupId] as List<string[]> ?? new List<string[]>();

                                    forumNames.Add(new[] { Convert.ToString(drForum["ForumDisplayName"]), Convert.ToString(drForum["ForumID"]) });
                                    forums[groupId] = forumNames;
                                }
                            }

                            foreach (DataRow dr in dsGroups.Tables[0].Rows)
                            {
                                int groupId = Convert.ToInt32(dr["GroupId"]);

                                List<string[]> forumNames = forums[groupId] as List<string[]>;
                                if (forumNames != null)
                                {
                                    // Add forum group item if some forum
                                    ListItem li = new ListItem(Convert.ToString(dr["GroupDisplayName"]), "0");
                                    li.Attributes.Add("disabled", "disabled");
                                    drpMoveToForum.Items.Add(li);

                                    // Add forum items
                                    foreach (string[] forum in forumNames)
                                    {
                                        // Add forum to DDL
                                        drpMoveToForum.Items.Add(new ListItem(" \xA0\xA0\xA0\xA0 " + forum[0], forum[1]));
                                    }
                                }
                            }


                            // Display message if no forum exists
                            if (drpMoveToForum.Items.Count == 0)
                            {
                                plcMoveInner.Visible = false;
                                ShowInformation(GetString("Forums.NoForumToMoveIn"));
                            }
                        }
                    }
                }
            }
        }
    }
}