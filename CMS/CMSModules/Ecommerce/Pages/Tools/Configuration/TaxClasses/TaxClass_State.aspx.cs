using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Globalization;
using CMS.Helpers;


public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_TaxClasses_TaxClass_State : CMSTaxClassesPage
{
    #region "Variables"

    private int taxClassId;
    private int countryId;
    private readonly Hashtable changedTextBoxes = new Hashtable();

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UI element
        CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, IsMultiStoreConfiguration ? "Tools.Ecommerce.StatesTaxClasses" : "Configuration.TaxClasses.States");

        // Get tax class Id from URL
        taxClassId = QueryHelper.GetInteger("objectid", 0);

        TaxClassInfo taxClass = TaxClassInfo.Provider.Get(taxClassId);
        EditedObject = taxClass;
        // Check if configured tax class belongs to configured site
        if (taxClass != null)
        {
            // Check site id of tax class
            CheckEditedObjectSiteID(taxClass.TaxClassSiteID);
        }

        if (!RequestHelper.IsPostBack())
        {
            // Fill the drpCountry with countries which have some states or colonies
            drpCountry.DataSource = CountryInfoProvider.GetCountriesWithStates();
            drpCountry.DataValueField = "CountryID";
            drpCountry.DataTextField = "CountryDisplayName";
            drpCountry.DataBind();
        }
        // Set grid view properties
        gvStates.Columns[0].HeaderText = GetString("taxclass_state.gvstates.state");
        gvStates.Columns[1].HeaderText = GetString("Code");
        gvStates.Columns[2].HeaderText = GetString("taxclass_state.gvstates.value");
        gvStates.Columns[3].HeaderText = GetString("StateId");

        LoadGridViewData();

        // Set header actions
        CurrentMaster.HeaderActions.AddAction(new SaveAction());
        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        CurrentMaster.DisplaySiteSelectorPanel = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        // Hide code column
        gvStates.Columns[1].Visible = false;
    }

    #endregion


    #region "Methods"

    protected void drpCountry_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadGridViewData();
    }


    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "save":
                Save();
                break;
        }
    }


    protected void gvStates_DataBound(object sender, EventArgs e)
    {
        foreach (GridViewRow row in gvStates.Rows)
        {
            // Copy id from StateID column to invisible label in last column
            Label id = new Label();
            id.Visible = false;
            id.Text = row.Cells[3].Text;
            row.Cells[3].Controls.Add(id);

            // Set unique text box ID
            TextBox txtValue = ControlsHelper.GetChildControl(row, typeof(TextBox), "txtTaxValue") as TextBox;
            if (txtValue != null)
            {
                txtValue.ID = "txtTaxValue" + id.Text;
            }
        }
    }


    protected void txtTaxValue_Changed(object sender, EventArgs e)
    {
        changedTextBoxes[((TextBox)sender).ID] = sender;
    }


    private void LoadGridViewData()
    {
        // Binding StateID column
        gvStates.Columns[3].Visible = true;
        countryId = ValidationHelper.GetInteger(drpCountry.SelectedValue, 0);

        var ds = GetStatesAndTaxRates();
        gvStates.DataSource = ds.Tables[0];
        gvStates.DataBind();
        gvStates.Columns[3].Visible = false;
    }


    /// <summary>
    /// Saves values.
    /// </summary>
    private void Save()
    {
        // Check permissions
        CheckConfigurationModification();

        string errorMessage = String.Empty;
        bool saved = false;

        // Loop through states
        foreach (GridViewRow row in gvStates.Rows)
        {
            Label lblStateId = (Label)row.Cells[3].Controls[0];
            int stateId = ValidationHelper.GetInteger(lblStateId.Text, 0);

            if (stateId > 0)
            {
                string stateName = ((Label)row.Cells[0].Controls[1]).Text;
                TextBox txtValue = (TextBox)row.Cells[2].Controls[1];

                if (changedTextBoxes[txtValue.ID] != null)
                {
                    // Remove state tax information if tax value is empty
                    if (String.IsNullOrEmpty(txtValue.Text))
                    {
                        TaxClassStateInfo.Provider.Remove(taxClassId, stateId);
                        saved = true;
                    }
                    // Update state tax information if tax value is not empty
                    else
                    {
                        var taxValue = ValidationHelper.GetDecimal(txtValue.Text, -1);

                        // Save tax value if valid
                        if (ValidationHelper.IsInRange(0, 100, taxValue))
                        {
                            TaxClassStateInfo.Provider.Add(taxClassId, stateId, taxValue);
                            saved = true;
                        }
                        else
                        {
                            errorMessage += stateName + ", ";
                        }
                    }
                }
            }
        }

        // Error message
        if (!String.IsNullOrEmpty(errorMessage))
        {
            // Remove last comma
            if (errorMessage.EndsWithCSafe(", "))
            {
                errorMessage = errorMessage.Remove(errorMessage.Length - 2, 2);
            }

            // Show error message
            ShowError(String.Format("{0} - {1}", errorMessage, GetString("Com.Error.TaxValue")));
        }

        // Display info message if some records were saved
        if (String.IsNullOrEmpty(errorMessage) || saved)
        {
            // Show message
            ShowChangesSaved();
        }
    }


    private DataSet GetStatesAndTaxRates()
    {
        return new ObjectQuery<StateInfo>()
            .Source(s => s.LeftJoin("COM_TaxClassState", "CMS_State.StateID", "COM_TaxClassState.StateID",
                new WhereCondition("COM_TaxClassState.TaxClassID", QueryOperator.Equals, taxClassId)))
            .WhereEquals("CMS_State.CountryID", countryId);
    }

    #endregion
}