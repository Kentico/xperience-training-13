using System;

using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Class_AlternativeFormList : CMSUserControl
{
    #region "Variables"

    private int mFormClassID;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets class id.
    /// </summary>
    public int FormClassID
    {
        get
        {
            return mFormClassID;
        }
        set
        {
            mFormClassID = value;
        }
    }


    /// <summary>
    /// On edit event.
    /// </summary>
    public event OnEditDeleteActionEventHandler OnEdit;


    /// <summary>
    /// On delete event.
    /// </summary>
    public event OnEditDeleteActionEventHandler OnDelete;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        gridElem.GridName = "~/CMSModules/AdminControls/Controls/Class/AlternativeFormList.xml";
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.WhereCondition = " FormClassID=" + FormClassID.ToString();
        gridElem.IsLiveSite = IsLiveSite;
        gridElem.ZeroRowsText = GetString("general.nodatafound");
    }


    /// <summary>
    /// Grid action handling.
    /// </summary>
    private void gridElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "edit":
                if (OnEdit != null)
                {
                    OnEdit(this, actionArgument);
                }
                break;

            case "delete":
                if (OnDelete != null)
                {
                    OnDelete(this, actionArgument);
                }
                break;
        }
    }

    #endregion
}

/// <summary>
/// Action event handler.
/// </summary>
/// <param name="sender">Sender control</param>
/// <param name="actionArgument">Action parameter</param>
public delegate void OnEditDeleteActionEventHandler(object sender, object actionArgument);