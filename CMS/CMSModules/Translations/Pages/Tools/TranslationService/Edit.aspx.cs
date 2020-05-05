using System;

using CMS.Helpers;
using CMS.TranslationServices;
using CMS.TranslationServices.Web.UI;
using CMS.UIControls;

// Edited object
[EditedObject(TranslationServiceInfo.OBJECT_TYPE, "serviceId")]

// Breadcrumbs
[Breadcrumbs()]
[Breadcrumb(0, "translationservice.translationservice.list", "~/CMSModules/Translations/Pages/Tools/TranslationService/List.aspx", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "translationservice.translationservice.new", NewObject = true)]

// Title
[Title(ResourceString = "translationservice.translationservice.edit", HelpTopic = "translationservices_edit", ExistingObject = true)]
[Title(ResourceString = "translationservice.translationservice.new", HelpTopic = "translationservices_edit", NewObject = true)]
public partial class CMSModules_Translations_Pages_Tools_TranslationService_Edit : CMSTranslationsPage
{
    #region "Variables"

    private int? mServiceId = null;

    #endregion

    #region "Properties"

    /// <summary>
    /// Gets edited service ID, or 0 if creating a new one.
    /// </summary>
    public int ServiceID
    {
        get
        {
            if (mServiceId == null)
            {
                mServiceId = QueryHelper.GetInteger("serviceid", 0);
            }

            return mServiceId.Value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreRender(EventArgs e)
    {
        string elementName = (ServiceID == 0) ? "Development.NewTranslationService" : "Development.EditTranslationService";
        var uiElement = new UIElementAttribute("CMS.TranslationServices", elementName);
        uiElement.Check(this);

        base.OnPreRender(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.ItemID = ServiceID;
    }

    #endregion
}
