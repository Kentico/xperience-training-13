using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Selectors_LinkMediaSelector_Search : CMSUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Initialize nested controls
            SetupControls();
        }
        else
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Makes search empty.
    /// </summary>
    public void ResetSearch()
    {
        txtSearchByName.Text = "";
    }


    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    private void SetupControls()
    {
        // Get JavaScript definition of method used to set hidden attributes
        string setSearchAction = String.Format(@"
function SetSearchAction(){{
    var searchTxt = document.getElementById('{0}');
    if(searchTxt!=null){{
        SetAction('search', searchTxt.value);
    }}
}}
function SetSearchFocus(){{
    var searchTxt = document.getElementById('{0}');
    if(searchTxt!=null){{
        searchTxt.blur();
        searchTxt.focus();
        searchTxt.value = searchTxt.value;
    }}
}}", txtSearchByName.ClientID);

        ScriptHelper.RegisterStartupScript(this, GetType(), "SearchScripts", setSearchAction, true);

        // Set search scripts
        txtSearchByName.Attributes["onkeydown"] = "var keynum; if(window.event){ keynum = event.keyCode; } else if(event.which){ keynum = event.which; } if(keynum == 13){ SetSearchAction(); RaiseHiddenPostBack(); return false; }";
        btnSearch.Attributes["onclick"] = "SetSearchAction(); RaiseHiddenPostBack(); return false;";
    }
}
