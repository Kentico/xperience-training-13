using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Synchronization;
using CMS.UIControls;


public partial class CMSModules_Integration_Controls_UI_Connectors_List : CMSAdminListControl
{
    #region "Properties"

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

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "status":
                CMSGridActionButton img = sender as CMSGridActionButton;
                if (img != null)
                {
                    DataRowView drv = UniGridFunctions.GetDataRowView(img.Parent as DataControlFieldCell);
                    if (drv != null)
                    {
                        string connectorName = ValidationHelper.GetString(drv["ConnectorName"], string.Empty);
                        bool connectorEnabled = ValidationHelper.GetBoolean(drv["ConnectorEnabled"], false);

                        if (connectorEnabled && (IntegrationHelper.GetConnector(connectorName) == null))
                        {
                            img.ToolTip = GetString("integration.loadconnectorfailed");
                        }
                        else
                        {
                            img.Visible = false;
                        }
                    }
                    if (img.Visible)
                    {
                        img.OnClientClick = "return false;";
                        img.Style.Add(HtmlTextWriterStyle.Cursor, "default");
                    }
                }
                break;
        }
        return parameter;
    }

    #endregion
}