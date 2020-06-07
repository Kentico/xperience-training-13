using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Globalization;
using CMS.Helpers;

namespace CMSApp.CMSModules.ContactManagement.Filters
{
    public partial class CMSModules_ContactManagement_Filters_CountryFilter : FormEngineUserControl
    {
        /// <summary>
        /// Gets or sets field value.
        /// </summary>
        public override object Value
        {
            get
            {
                EnsureChildControls();
                return fltCountry.Value;
            }
            set
            {
                EnsureChildControls();
                fltCountry.Value = value;
            }
        }
        

        /// <summary>
        /// Specifies, column name, which holds country ID.
        /// </summary>
        public string CountryIDColumnName
        {
            get
            {
                return ValidationHelper.GetString(GetValue("CountryIDColumnName"), string.Empty);
            }
        }


        public override string GetWhereCondition()
        {
            var whereCondition = new WhereCondition();
            var originalQuery = fltCountry.WhereCondition;
            if (string.IsNullOrEmpty(originalQuery) || string.IsNullOrEmpty(CountryIDColumnName))
            {
                return string.Empty;
            }

            var countryIDs = CountryInfo.Provider.Get()
                                                .Where(originalQuery)
                                                .AsMaterializedList("CountryID");

            whereCondition.WhereIn(CountryIDColumnName, countryIDs);            
            if (fltCountry.FilterOperator == WhereBuilder.NOT_LIKE || fltCountry.FilterOperator == WhereBuilder.NOT_EQUAL)
            {
                whereCondition.Or().WhereNull(CountryIDColumnName);
            }

            return whereCondition.ToString(true);
        }


        /// <summary>
        /// Sets the given <paramref name="value"/> as the <see cref="Value"/> of current selector. In case <c>null</c> is passed, the filter is reseted to the default state.
        /// </summary>
        public override void LoadControlValue(object value)
        {
            base.LoadControlValue(value);

            if (value == null)
            {
                fltCountry.ResetFilter();                
            }
        }
    }
}