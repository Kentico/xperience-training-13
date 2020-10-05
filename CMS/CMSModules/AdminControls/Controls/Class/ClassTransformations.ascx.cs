using System;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Class_ClassTransformations : CMSUserControl
{
    #region "Private fields"

    private int mClassID = 0;
    private string mEditPageUrl = null;

    #endregion


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
    /// Gets or sets the class id.
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
    /// Gets or sets the module id.
    /// </summary>
    public int ModuleID
    {
        get;
        set;
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
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
        }
        else
        {
            // Check whether virtual objects are allowed
            if (!SettingsKeyInfoProvider.VirtualObjectsAllowed)
            {
                ShowWarning(GetString("VirtualPathProvider.NotRunning"));
            }

            // Initialize the controls
            uniGrid.OnAction += uniGrid_OnAction;
            uniGrid.OnExternalDataBound += uniGrid_OnExternalDataBound;
            uniGrid.GridName = "~/CMSModules/AdminControls/Controls/Class/ClassTransformations.xml";
            uniGrid.ZeroRowsText = GetString("general.nodatafound");
            uniGrid.IsLiveSite = IsLiveSite;

            // If the ClassID was specified
            if (ClassID > 0)
            {
                uniGrid.WhereCondition = "TransformationClassID=" + ClassID;
            }
            else
            {
                // Otherwise hide the UniGrid to avoid unexpected behavior
                uniGrid.Visible = false;
            }
        }
    }


    #region "GridView handling"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName.ToLowerCSafe() == "edit")
        {
            int transID = ValidationHelper.GetInteger(actionArgument, 0);
            if (transID != 0)
            {
                var ti = TransformationInfoProvider.GetTransformation(transID);
                if (ti != null)
                {
                    var url = EditPageUrl;

                    var query = String.Format(
                            "objectid={0}&displaytitle=false{1}",
                            transID,
                            (ModuleID > 0 ? "&moduleid=" + ModuleID : String.Empty)
                        );

                    url = URLHelper.AppendQuery(url, query);

                    URLHelper.Redirect(UrlResolver.ResolveUrl(url));
                }
            }
        }
        else if (actionName.ToLowerCSafe() == "delete")
        {
            int transformationId = ValidationHelper.GetInteger(actionArgument, -1);
            if (transformationId > 0)
            {
                TransformationInfoProvider.DeleteTransformation(transformationId);
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the controls on the page.
    /// </summary>
    private void SetupControl()
    {
    }


    protected object uniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "transformationtype":
                DataRowView dr = (DataRowView)parameter;
                string type = ValidationHelper.GetString(dr["TransformationType"], String.Empty);

                switch (type.ToLowerCSafe())
                {
                    case "text":
                        return "Text / XML";

                    case "html":
                        return "HTML";
                }
                break;
        }
        return parameter;
    }

    #endregion
}