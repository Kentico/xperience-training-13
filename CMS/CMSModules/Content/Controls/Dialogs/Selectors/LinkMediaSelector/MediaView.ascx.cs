using System;
using System.Text;
using System.Linq;

using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Selectors_LinkMediaSelector_MediaView : ContentMediaView
{
    #region "Public properties"

    /// <summary>
    /// Inner media view control
    /// </summary>
    public override ContentInnerMediaView InnerMediaControl
    {
        get
        {
            return innermedia;
        }
    }
    
    #endregion
    

    #region "Methods"

    /// <summary>
    /// Ensures no item is selected.
    /// </summary>
    public void ResetSearch()
    {
        dialogSearch.ResetSearch();
    }


    /// <summary>
    /// Ensures first page is displayed in the control displaying the content.
    /// </summary>
    public void ResetPageIndex()
    {
        innermedia.ResetPageIndex();
    }


    /// <summary>
    /// Ensure no item is selected in list view.
    /// </summary>
    public void ResetListSelection()
    {
        innermedia.ResetListSelection();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // If processing the request should not continue
        if (StopProcessing)
        {
            Visible = false;
        }
        else
        {
            Visible = true;

            // Initialize controls
            SetupControls();
        }
    }
    

    /// <summary>
    /// Loads data from data source property.
    /// </summary>
    private void ReloadData()
    {
        innermedia.Reload(true);
    }


    /// <summary>
    /// Loads control's content.
    /// </summary>
    public void Reload()
    {
        // Initialize controls
        SetupControls();
        ReloadData();
    }


    /// <summary>
    /// Displays listing info message.
    /// </summary>
    /// <param name="infoMsg">Info message to display</param>
    public void DisplayListingInfo(string infoMsg)
    {
        if (!string.IsNullOrEmpty(infoMsg))
        {
            plcListingInfo.Visible = true;
            lblListingInfo.Text = infoMsg;
        }
    }

    #endregion
}