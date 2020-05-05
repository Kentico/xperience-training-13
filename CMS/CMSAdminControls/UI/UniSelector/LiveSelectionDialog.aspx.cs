using System;
using System.Collections;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_UI_UniSelector_LiveSelectionDialog : CMSLiveModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash", "selectedvalue"))
        {
            selectionDialog.LocalizeItems = QueryHelper.GetBoolean("localize", true);

            string identifier = QueryHelper.GetString("params", null);
            Hashtable parameters = (Hashtable)WindowHelper.GetItem(identifier);

            if (parameters != null)
            {
                // Load resource prefix
                string resourcePrefix = ValidationHelper.GetString(parameters["ResourcePrefix"], "general");

                // Set the page title
                string titleText = GetString(resourcePrefix + ".selectitem|general.selectitem");

                PageTitle.TitleText = titleText;
                Page.Title = titleText;

                // Cancel button
                btnCancel.ResourceString = "general.cancel";

                SelectionModeEnum selectionMode = (SelectionModeEnum)parameters["SelectionMode"];

                // Show and set up the OK and the Cancel button if needed
                switch (selectionMode)
                {
                    case SelectionModeEnum.Multiple:
                    case SelectionModeEnum.MultipleTextBox:
                    case SelectionModeEnum.MultipleButton:
                        // Ok button
                        btnOk.Visible = true;
                        btnOk.ResourceString = "general.ok";
                        btnOk.Attributes.Add("onclick", "return US_Submit();");
                        // Cancel button
                        btnCancel.Attributes.Add("onclick", "return US_Cancel();");
                        break;

                    default:
                        // Ok button
                        btnOk.Visible = false;
                        // Cancel button
                        btnCancel.Attributes.Add("onclick", "return CloseDialog();");
                        break;
                }
            }
            else
            {
                URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext", true));
            }
        }
        else
        {
            URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext", true));
        }
    }
}