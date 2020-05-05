using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.SalesForce;

using MappingEditorItem = CMSModules_ContactManagement_Controls_UI_SalesForce_MappingEditorItem;

/// <summary>
/// Displays a complete mapping of CMS objects to SalesForce entities, and allows the user to edit it.
/// </summary>
public partial class CMSModules_ContactManagement_Pages_Tools_SalesForce_MappingEditor : CMSSalesForceDialogPage
{
    #region "Constants"

    /// <summary>
    /// Short link to help topic page.
    /// </summary>
    private const string HELP_TOPIC_LINK = "salesforce_integration_config";

    #endregion


    #region "Private members"

    private FormInfo mFormInfo;
    private AttributeValueConverterFactory mAttributeValueConverterFactory;
    private IEnumerable<EntityAttributeModel> mAttributeModels;

    private string mEntityModelName;
    private string mSourceMappingHiddenFieldClientId;
    private string mSourceMappingPanelClientId;
    private OrganizationCredentials mCredentials;
    private Mapping mSourceMapping;

    private Dictionary<string, MappingEditorItem> mMappingEditorItems;

    #endregion


    #region "Protected properties"

    protected EntityModel EntityModel
    {
        get
        {
            EntityModel model = ViewState["EntityModel"] as EntityModel;
            if (model == null)
            {
                model = GetEntityModel("Lead");
                ViewState["EntityModel"] = model;
            }

            return model;
        }
        set
        {
            ViewState["EntityModel"] = value;
        }
    }

    protected FormInfo FormInfo
    {
        get
        {
            if (mFormInfo == null)
            {
                ContactFormInfoProvider provider = new ContactFormInfoProvider();
                mFormInfo = provider.GetFormInfo();
            }

            return mFormInfo;
        }
    }

    protected AttributeValueConverterFactory AttributeValueConverterFactory
    {
        get
        {
            if (mAttributeValueConverterFactory == null)
            {
                mAttributeValueConverterFactory = new AttributeValueConverterFactory();
            }

            return mAttributeValueConverterFactory;
        }
    }

    protected IEnumerable<EntityAttributeModel> AttributeModels
    {
        get
        {
            if (mAttributeModels == null)
            {
                mAttributeModels = EntityModel.AttributeModels.Where(x => x.IsCreatable && x.IsUpdateable && !x.IsAutoNumber && !x.IsCalculated && !x.IsExternalId && !x.IsNameField && !x.IsParentName);
            }

            return mAttributeModels;
        }
    }

    protected Dictionary<string, MappingEditorItem> MappingItems
    {
        get
        {
            if (mMappingEditorItems == null)
            {
                mMappingEditorItems = new Dictionary<string, MappingEditorItem>();
            }

            return mMappingEditorItems;
        }
    }

    protected string SourceMappingHiddenFieldClientId
    {
        get
        {
            return mSourceMappingHiddenFieldClientId;
        }
    }

    protected string SourceMappingPanelClientId
    {
        get
        {
            return mSourceMappingPanelClientId;
        }
    }

    protected OrganizationCredentials Credentials
    {
        get
        {
            return mCredentials;
        }
    }

    protected Mapping SourceMapping
    {
        get
        {
            return mSourceMapping;
        }
    }

    #endregion


    #region "Life-cycle methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterTooltip(Page);
        PageTitle.TitleText = GetString("sf.mapping.leadreplication.title");
        PageTitle.HelpTopicName = HELP_TOPIC_LINK;
        RequiredAttributeRepeater.ItemDataBound += new RepeaterItemEventHandler(AttributeRepeater_ItemDataBound);
        OtherAttributeRepeater.ItemDataBound += new RepeaterItemEventHandler(AttributeRepeater_ItemDataBound);
        Save += ConfirmButton_Click;
        try
        {
            if (RequestHelper.IsPostBack() && Page.Request.Params[postEventSourceID] == "RefreshButton")
            {
                EntityModel = null;
            }
            RestoreParameters();
            InitializeExternalAttributeDropDownList();
            InitializeAttributeRepeaters();
        }
        catch (Exception exception)
        {
            HandleError(exception);
        }
    }

    #endregion


    #region "Control event methods"

    protected void ConfirmButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (ValidateInput())
            {
                Mapping mapping = CreateMappingFromInput();
                MappingSerializer serializer = new MappingSerializer();
                MappingHiddenField.Value = serializer.SerializeMapping(mapping);
                MappingControl.Mapping = mapping;
                string parametersIdentifier = QueryHelper.GetString("pid", null);
                Hashtable parameters = WindowHelper.GetItem(parametersIdentifier) as Hashtable;
                parameters["Mapping"] = MappingHiddenField.Value;
                WindowHelper.Add(parametersIdentifier, parameters);
            }
        }
        catch (Exception exception)
        {
            HandleError(exception);
        }
    }

    protected void AttributeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        EntityAttributeModel attributeModel = e.Item.DataItem as EntityAttributeModel;
        MappingEditorItem control = e.Item.FindControl("MappingEditorItemControl") as MappingEditorItem;
        control.EntityModel = EntityModel;
        control.EntityAttributeModel = attributeModel;
        control.FormInfo = FormInfo;
        control.ConverterFactory = AttributeValueConverterFactory;
        control.SourceMappingItem = SourceMapping.GetItem(attributeModel.Name);
        control.Initialize();
        MappingItems.Add(attributeModel.Name, control);
    }

    #endregion


    #region "Private methods"

    private void InitializeAttributeRepeaters()
    {
        IEnumerable<EntityAttributeModel> requiredAttributeModels = AttributeModels.Where(x => !x.IsNullable && !x.HasDefaultValue);
        IEnumerable<EntityAttributeModel> otherAttributeModels = AttributeModels.Where(x => x.IsNullable || x.HasDefaultValue);
        InitializeAttributeRepeater(RequiredAttributeRepeater, requiredAttributeModels);
        InitializeAttributeRepeater(OtherAttributeRepeater, otherAttributeModels);
    }

    private void InitializeAttributeRepeater(Repeater control, IEnumerable<EntityAttributeModel> attributeModels)
    {
        control.DataSource = attributeModels;
        control.DataBind();
    }

    private void InitializeExternalAttributeDropDownList()
    {
        foreach (EntityAttributeModel attributeModel in EntityModel.AttributeModels.Where(x => x.Type == EntityAttributeValueType.String && x.Length == 32 && x.IsCreatable && x.IsUpdateable && x.IsCustom && x.IsExternalId))
        {
            ListItem item = new ListItem
            {
                Text = attributeModel.Label,
                Value = attributeModel.Name
            };
            ExternalAttributeDropDownListEx.Items.Add(item);
        }
        ExternalAttributeDropDownListEx.SelectedValue = SourceMapping.ExternalIdentifierAttributeName;
    }

    private Mapping CreateMappingFromInput()
    {
        ListItem externalItem = ExternalAttributeDropDownListEx.SelectedItem;
        Mapping mapping = new Mapping
        {
            ExternalIdentifierAttributeName = (externalItem != null ? externalItem.Value : String.Empty),
            ExternalIdentifierAttributeLabel = (externalItem != null ? externalItem.Text : String.Empty)
        };
        foreach (MappingEditorItem control in MappingItems.Values)
        {
            MappingItem item = control.MappingItem;
            if (item != null)
            {
                mapping.Add(item);
            }
        }

        return mapping;
    }

    private bool ValidateInput()
    {
        List<string> errors = new List<string>();
        if (String.IsNullOrEmpty(ExternalAttributeDropDownListEx.SelectedValue))
        {
            errors.Add(GetString("sf.mapping.noexternalattributeselected"));
        }
        foreach (EntityAttributeModel attributeModel in AttributeModels.Where(x => !x.IsNullable && !x.HasDefaultValue))
        {
            MappingEditorItem control = null;
            if (MappingItems.TryGetValue(attributeModel.Name, out control))
            {
                if (control.MappingItem == null)
                {
                    string error = String.Format(GetString("sf.mapping.requiredattributemappingmissing"), attributeModel.Label);
                    errors.Add(error);
                }
            }
        }
        if (errors.Count > 0)
        {
            SalesForceError.Report(GetString("sf.mapping.validationerror"), errors);
            return false;
        }

        return true;
    }

    private void RestoreParameters()
    {
        // Validate parameters
        if (!QueryHelper.ValidateHash("hash"))
        {
            throw new Exception("[SalesForceMappingEditorPage.RestoreParameters]: Invalid query hash.");
        }
        Hashtable parameters = WindowHelper.GetItem(QueryHelper.GetString("pid", null)) as Hashtable;
        if (parameters == null)
        {
            throw new Exception("[SalesForceMappingEditorPage.RestoreParameters]: The dialog page parameters are missing, the session might have been lost.");
        }

        // Restore parameters
        mEntityModelName = ValidationHelper.GetString(parameters["EntityModelName"], null);
        mSourceMappingHiddenFieldClientId = ValidationHelper.GetString(parameters["MappingHiddenFieldClientId"], null);
        mSourceMappingPanelClientId = ValidationHelper.GetString(parameters["MappingPanelClientId"], null);

        // Restore mapping
        string content = ValidationHelper.GetString(parameters["Mapping"], null);
        if (String.IsNullOrEmpty(content))
        {
            mSourceMapping = new Mapping();
        }
        else
        {
            MappingSerializer serializer = new MappingSerializer();
            mSourceMapping = serializer.DeserializeMapping(content);
        }

        // Restore credentials
        content = ValidationHelper.GetString(parameters["Credentials"], null);
        if (String.IsNullOrEmpty(content))
        {
            throw new Exception("[SalesForceMappingEditorPage.RestoreParameters]: The dialog page parameters are corrupted.");
        }
        mCredentials = OrganizationCredentials.Deserialize(EncryptionHelper.DecryptData(content).TrimEnd('\0'));
    }

    private EntityModel GetEntityModel(string name)
    {
        ISessionProvider sessionProvider = CreateSessionProvider();
        Session session = sessionProvider.CreateSession();
        SalesForceClient client = new SalesForceClient(session);
        
        return client.DescribeEntity(name);
    }

    private ISessionProvider CreateSessionProvider()
    {
        return new RefreshTokenSessionProvider
        {
            ClientId = Credentials.ClientId,
            ClientSecret = Credentials.ClientSecret,
            RefreshToken = Credentials.RefreshToken
        };
    }

    private void HandleError(Exception exception)
    {
        SalesForceError.Report(exception);
        Service.Resolve<IEventLogService>().LogException("Salesforce.com Connector", "MappingEditorPage", exception);
    }

    #endregion
}
