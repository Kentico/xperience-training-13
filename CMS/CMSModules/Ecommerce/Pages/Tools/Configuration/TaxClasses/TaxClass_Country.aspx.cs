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


public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_TaxClasses_TaxClass_Country : CMSTaxClassesPage
{
    #region "Variables"

    private int taxClassId;
    private readonly Hashtable changedTextBoxes = new Hashtable();

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UI element
        CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, IsMultiStoreConfiguration ? "Tools.Ecommerce.CountriesTaxClasses" : "Configuration.TaxClasses.Countries");
        
        taxClassId = QueryHelper.GetInteger("objectid", 0);

        TaxClassInfo taxClassObj = TaxClassInfo.Provider.Get(taxClassId);
        EditedObject = taxClassObj;
        // Check if configured tax class belongs to configured site
        if (taxClassObj != null)
        {
            CheckEditedObjectSiteID(taxClassObj.TaxClassSiteID);
            
        }

        GridViewCountries.Columns[0].HeaderText = GetString("TaxClass_Country.Country");
        GridViewCountries.Columns[1].HeaderText = GetString("TaxClass_Country.Value");

        var ds = GetCountriesAndTaxRates();
       
        // Bounded CountryID field
        GridViewCountries.Columns[2].Visible = true;
        GridViewCountries.DataSource = ds.Tables[0];
        GridViewCountries.DataBind();

        // After id is copied, the CountryID column it's not needed anymore
        GridViewCountries.Columns[2].Visible = false;

        // Set header actions
        CurrentMaster.HeaderActions.AddAction(new SaveAction());
        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }

    #endregion


    #region Methods

    protected void GridViewCountries_DataBound(object sender, EventArgs e)
    {
        foreach (GridViewRow row in GridViewCountries.Rows)
        {
            // Copy id from CountryID column to invisible label in last column
            Label id = new Label();
            id.Visible = false;
            id.Text = row.Cells[2].Text;
            row.Cells[2].Controls.Add(id);

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


    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "save":
                Save();
                break;
        }
    }


    /// <summary>
    /// Saves the taxes values.
    /// </summary>
    private void Save()
    {
        // Check permissions
        CheckConfigurationModification();

        string errorMessage = String.Empty;
        bool saved = false;

        // Loop through countries
        foreach (GridViewRow row in GridViewCountries.Rows)
        {
            Label lblCountryId = (Label)row.Cells[2].Controls[0];
            int countryId = ValidationHelper.GetInteger(lblCountryId.Text, 0);

            if (countryId > 0)
            {
                string countryName = ((Label)row.Cells[0].Controls[1]).Text;
                TextBox txtValue = (TextBox)row.Cells[1].Controls[1];

                if (changedTextBoxes[txtValue.ID] != null) 
                {
                    // Remove country tax information if tax value is empty
                    if (String.IsNullOrEmpty(txtValue.Text))
                    {
                        TaxClassCountryInfo.Provider.Remove(countryId, taxClassId);
                        saved = true;
                    }
                    // Update country tax information if tax value is not empty
                    else
                    {
                        var taxValue = ValidationHelper.GetDecimal(txtValue.Text, -1);

                        // Save tax value if valid
                        if (ValidationHelper.IsInRange(0, 100, taxValue))
                        {
                            TaxClassCountryInfo.Provider.Add(countryId, taxClassId, taxValue);
                            saved = true;
                        }
                        else
                        {
                            errorMessage += countryName + ", ";
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


    private DataSet GetCountriesAndTaxRates()
    {
        return new ObjectQuery<CountryInfo>()
            .Source(s => s.LeftJoin("COM_TaxClassCountry", "CMS_Country.CountryID", "COM_TaxClassCountry.CountryID",
            new WhereCondition("COM_TaxClassCountry.TaxClassID", QueryOperator.Equals, taxClassId)));
    }

    #endregion
}