using System;
using System.Linq;

using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.ECOMMERCE, "E-commerce")]
public partial class CMSModules_Ecommerce_Pages_Development_DocumentTypes_DocumentType_Edit_Ecommerce : GlobalAdminPage
{
    #region "Variables"

    private int docTypeId;
    private DataClassInfo dci;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        docTypeId = QueryHelper.GetInteger("objectid", 0);
        dci = DataClassInfoProvider.GetDataClassInfo(docTypeId);

        // Get class fields
        if (dci == null)
        {
            return;
        }

        var fi = FormHelper.GetFormInfo(dci.ClassName, false);
        fi.GetFields(true, true);

        if (!RequestHelper.IsPostBack())
        {
            // Set checkboxes
            chkIsProduct.Checked = dci.ClassIsProduct;
            chkIsProductSection.Checked = dci.ClassIsProductSection;

            // Select specified department
            if (string.IsNullOrEmpty(dci.ClassSKUDefaultDepartmentName))
            {
                departmentElem.SelectedID = dci.ClassSKUDefaultDepartmentID;
            }
            else
            {
                departmentElem.SelectedName = dci.ClassSKUDefaultDepartmentName;
            }

            // Select default product type
            productTypeElem.Value = dci.ClassSKUDefaultProductType;
        }

        pnlDefaultSelection.Visible = chkIsProduct.Checked;
    }

    #endregion


    #region "Events"

    protected void btnOK_Click(object sender, EventArgs e)
    {
        dci = DataClassInfoProvider.GetDataClassInfo(docTypeId);
        if (dci != null)
        {
            // Set checkboxes                
            dci.ClassIsProductSection = chkIsProductSection.Checked;
            dci.ClassIsProduct = chkIsProduct.Checked;

            if (dci.ClassIsProduct)
            {
                dci.ClassSKUDefaultDepartmentName = departmentElem.SelectedName;
                dci.ClassSKUDefaultProductType = (string)productTypeElem.Value;
            }

            try
            {
                // Save changes to database
                DataClassInfoProvider.SetDataClassInfo(dci);
                ShowChangesSaved();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }

    #endregion
}