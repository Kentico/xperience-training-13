using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.ECOMMERCE, "Configuration.TaxClasses.Products")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_TaxClasses_TaxClass_Products : CMSTaxClassesPage
{
    private int editedSiteId;


    protected void Page_Load(object sender, EventArgs e)
    {
        var taxClassId = QueryHelper.GetInteger("objectid", 0);
        var taxClassInfoObj = TaxClassInfo.Provider.Get(taxClassId);
        EditedObject = taxClassInfoObj;

        if (taxClassInfoObj != null)
        {
            editedSiteId = taxClassInfoObj.TaxClassSiteID;

            // Check object's site id
            CheckEditedObjectSiteID(editedSiteId);
        
            uniGrid.WhereCondition = new WhereCondition().WhereEquals("SKUTaxClassID", taxClassId).ToString(true);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (editedSiteId != 0)
        {
            // Hide sitename column for site taxes
            uniGrid.NamedColumns["sitename"].Visible = false;
        }
    }
}