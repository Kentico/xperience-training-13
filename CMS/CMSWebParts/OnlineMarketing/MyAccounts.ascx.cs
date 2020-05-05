using System;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_OnlineMarketing_MyAccounts : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets list of visible fields (columns).
    /// </summary>
    public string VisibleFields
    {
        get
        {
            return ValidationHelper.GetString(GetValue("VisibleFields"), "");
        }
        set
        {
            SetValue("VisibleFields", value);
        }
    }


    /// <summary>
    /// Gets or sets page size.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), 0);
        }
        set
        {
            SetValue("PageSize", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    protected void SetupControl()
    {
        // Check permissions
        var currentUser = MembershipContext.AuthenticatedUser;
        if (!AuthorizationHelper.AuthorizedReadContact(false))
        {
            lblInfo.Visible = true;
            lblInfo.Text = GetString("om.myaccounts.notallowedtoreadaccounts");
            return;
        }

        gridElem.Visible = true;
        gridElem.WhereCondition = "AccountOwnerUserID=" + currentUser.UserID;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.Pager.DefaultPageSize = PageSize;

        ScriptHelper.RegisterDialogScript(Page);

        SetVisibleColumns();
    }


    /// <summary>
    /// Hide unwanted columns.
    /// </summary>
    protected void SetVisibleColumns()
    {
        string visibleCols = "|" + VisibleFields.Trim('|') + "|";
        // Hide unwanted columns
        for (int i = gridElem.GridColumns.Columns.Count - 1; i >= 0; i--)
        {
            string colName;
            if (!String.IsNullOrEmpty(colName = gridElem.GridColumns.Columns[i].Name))
            {
                if (visibleCols.IndexOf("|" + colName + "|", StringComparison.Ordinal) == -1)
                {
                    gridElem.GridColumns.Columns[i].Visible = false;
                    gridElem.GridColumns.Columns[i].Filter = null;
                }
                else
                {
                    gridElem.GridColumns.Columns[i].Visible = true;
                }
            }
        }
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerInvariant())
        {
            case "edit":
                var btn = ((CMSGridActionButton)sender);
                // Ensure accountID parameter value;
                var objectID = ValidationHelper.GetInteger(btn.CommandArgument, 0);
                // Account detail URL
                string accountURL = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.CONTACTMANAGEMENT, "EditAccount", objectID);
                // Add modal dialog script to onClick action
                btn.OnClientClick = ScriptHelper.GetModalDialogScript(accountURL, "AccountDetail");
                break;

            case "primary":
                DataRowView drv = (DataRowView)parameter;
                int contactId = ValidationHelper.GetInteger(drv["AccountPrimaryContactID"], 0);
                string fullName = ValidationHelper.GetString(drv["PrimaryContactFullName"], null);

                string contactURL = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.CONTACTMANAGEMENT, "EditContact", contactId);
                // Add modal dialog script to onClick action
                var script = ScriptHelper.GetModalDialogScript(contactURL, "ContactDetail");
                return "<a href=\"#\" onclick=\"" + script + "\">" + HTMLHelper.HTMLEncode(fullName) + "</a>";

            case "website":
                string url = ValidationHelper.GetString(parameter, null);
                if (url != null)
                {
                    return "<a target=\"_blank\" href=\"" + HTMLHelper.HTMLEncode(url) + "\" \">" + HTMLHelper.HTMLEncode(url) + "</a>";
                }

                return GetString("general.na");
        }

        return parameter;
    }

    #endregion
}
