using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ContactManagement_Controls_UI_ContactGroup_List : CMSAdminListControl
{
    #region "Variables"
   
    private string mWhereCondition = null;
    private bool? mModifyGroupPermission;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets permission for modifying a group.
    /// </summary>
    protected bool ModifyGroupPermission
    {
        get
        {
            return (bool)(mModifyGroupPermission ??
                          (mModifyGroupPermission = AuthorizationHelper.AuthorizedModifyContact(false)));
        }
    }


    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return gridElem;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Additional WHERE condition to filter data.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return mWhereCondition;
        }
        set
        {
            mWhereCondition = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        var editUrl = UIContextHelper.GetElementUrl(ModuleName.CONTACTMANAGEMENT, "EditContactGroup");
        editUrl = URLHelper.AddParameterToUrl(editUrl, "displayTitle", "false");
        editUrl = URLHelper.AddParameterToUrl(editUrl, "objectId", "{0}");

        // Setup unigrid
        gridElem.OnAction += gridElem_OnAction;
        gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, WhereCondition);
        gridElem.EditActionUrl = editUrl;
        gridElem.ZeroRowsText = GetString("om.contactgroup.notfound");
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        CMSGridActionButton btn = null;

        switch (sourceName.ToLowerCSafe())
        {
            case "delete":
                if (!ModifyGroupPermission)
                {
                    btn = (CMSGridActionButton)sender;
                    btn.Enabled = false;
                }
                break;
        }

        return null;
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Unigrid button clicked.
    /// </summary>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "delete")
        {
            int groupId = ValidationHelper.GetInteger(actionArgument, 0);
            ContactGroupInfo cgi = ContactGroupInfo.Provider.Get(groupId);

            // Check permission
            if ((cgi != null) && AuthorizationHelper.AuthorizedModifyContact(true))
            {
                // Delete contact group
                ContactGroupInfo.Provider.Delete(ContactGroupInfo.Provider.Get(groupId));
            }
        }
    }

    #endregion
}