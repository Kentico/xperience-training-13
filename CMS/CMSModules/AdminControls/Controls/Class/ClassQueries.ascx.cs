using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Class_ClassQueries : CMSUserControl
{
    #region "Private fields"

    private int mClassID = 0;
    private string mEditPageUrl = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// ID of the class to edit queries.
    /// </summary>
    public int ClassID
    {
        get
        {
            return mClassID;
        }
        set
        {
            mClassID = value;
        }
    }


    /// <summary>
    /// URL of the page holding the editing tasks.
    /// </summary>
    public string EditPageUrl
    {
        get
        {
            return mEditPageUrl;
        }
        set
        {
            mEditPageUrl = value;
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
            uniGrid.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets module identifier.
    /// </summary>
    public int ModuleID
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
        }
        else
        {
            // Initialize the controls
            uniGrid.OnAction += new OnActionEventHandler(uniGrid_OnAction);
            uniGrid.GridName = "~/CMSModules/AdminControls/Controls/Class/ClassQueries.xml";
            uniGrid.IsLiveSite = IsLiveSite;
            uniGrid.ZeroRowsText = GetString("general.nodatafound");

            // If the ClassID was specified
            if (ClassID > 0)
            {
                uniGrid.WhereCondition = "ClassID=" + ClassID;
            }
            else
            {
                // Otherwise hide the UniGrid to avoid unexpected behaviour
                uniGrid.Visible = false;
            }
        }
    }

    #endregion


    #region "UniGrid handling"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (CMSString.Equals(actionName, "edit", true))
        {
            RedirectToEditUrl(actionArgument);
        }
        else if (CMSString.Equals(actionName, "delete", true))
        {
            int queryId = ValidationHelper.GetInteger(actionArgument, -1);
            if (queryId > 0)
            {
                QueryInfo.Provider.Get(queryId)?.Delete();
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Redirects to page where user can edit a selected query.
    /// </summary>
    /// <param name="actionArgument">ID of the query selected in UniGrid</param>
    private void RedirectToEditUrl(object actionArgument)
    {
        string actionArg = ValidationHelper.GetString(actionArgument, string.Empty);
        if (actionArg == string.Empty)
        {
            return;
        }

        var editUrl = URLHelper.AddParameterToUrl(EditPageUrl, "objectid", actionArg);
        editUrl = URLHelper.AddParameterToUrl(editUrl, "parentobjectid", ClassID.ToString());
        editUrl = URLHelper.AddParameterToUrl(editUrl, "moduleid", ModuleID.ToString());

        URLHelper.Redirect(UrlResolver.ResolveUrl(editUrl));
    }

    #endregion
}