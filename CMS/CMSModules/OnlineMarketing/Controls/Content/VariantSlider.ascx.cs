using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;

public partial class CMSModules_OnlineMarketing_Controls_Content_VariantSlider : CMSAbstractPortalUserControl
{
    #region "Variables"

    /// <summary>
    /// Web part control of the variant slider.
    /// </summary>
    private CMSAbstractWebPart mWebPartControl = null;

    /// <summary>
    /// Web part zone control of the variant slider.
    /// </summary>
    private CMSWebPartZone mWebPartZoneControl = null;

    /// <summary>
    /// Indicates whether processing should be stopped.
    /// </summary>
    private bool stopProcessing = false;

    /// <summary>
    /// Indicates whether the variant slider is used for a zone, web part or widget.
    /// </summary>
    private VariantTypeEnum? mSliderMode = null;

    /// <summary>
    /// Gets the variant mode. Indicates whether there are MVT/ContentPersonalization/None variants active.
    /// </summary>
    private VariantModeEnum? mVariantMode = null;

    /// <summary>
    /// Indicates whether the variant slider is used in the RTL mode.
    /// </summary>
    private bool isRTL = (CultureHelper.IsPreferredCultureRTL());

    /// <summary>
    /// Unique code for the original web part/zone
    /// </summary>
    private string uniqueCode = string.Empty;

    /// <summary>
    /// Preferred UI culture for localized strings.
    /// </summary>
    private string mUICulture = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates whether the slider is used as a web part slider or a zone slider.
    /// </summary>
    public VariantTypeEnum SliderMode
    {
        get
        {
            if (mSliderMode == null)
            {
                // Check whether the slider is used for a webpart or for a zone
                Control baseControl = GetBaseControl(this);
                if (baseControl is CMSAbstractWebPart)
                {
                    if (((CMSAbstractWebPart)baseControl).IsWidget)
                    {
                        mSliderMode = VariantTypeEnum.Widget;
                    }
                    else
                    {
                        mSliderMode = VariantTypeEnum.WebPart;
                    }
                }
                else
                {
                    mSliderMode = VariantTypeEnum.Zone;
                }
            }

            return mSliderMode.Value;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets the web part control of the variant slider.
    /// </summary>
    private CMSAbstractWebPart WebPartControl
    {
        get
        {
            if (mWebPartControl == null)
            {
                mWebPartControl = GetBaseControl(Parent) as CMSAbstractWebPart;
            }

            return mWebPartControl;
        }
    }


    /// <summary>
    /// Gets the web part zone control of the variant slider.
    /// </summary>
    private CMSWebPartZone WebPartZoneControl
    {
        get
        {
            if (mWebPartZoneControl == null)
            {
                mWebPartZoneControl = GetBaseControl(Parent) as CMSWebPartZone;
            }

            return mWebPartZoneControl;
        }
    }


    /// <summary>
    /// Gets the variant mode. Indicates whether there are MVT/ContentPersonalization/None variants active.
    /// </summary>
    private VariantModeEnum VariantMode
    {
        get
        {
            if (mVariantMode == null)
            {
                switch (SliderMode)
                {
                    case VariantTypeEnum.WebPart:
                    case VariantTypeEnum.Widget:
                        if (WebPartControl.PartInstance != null)
                        {
                            // Get the variant mode from the webPartInstance
                            mVariantMode = WebPartControl.PartInstance.VariantMode;
                        }
                        break;

                    default:
                        if (WebPartZoneControl.ZoneInstance != null)
                        {
                            // Get the variant mode from the zoneInstance
                            mVariantMode = WebPartZoneControl.ZoneInstance.VariantMode;
                        }
                        break;
                }
            }

            return mVariantMode.Value;
        }
    }


    /// <summary>
    /// Checks permissions (depends on variant mode).
    /// </summary>
    /// <param name="permissionName">Name of permission to test</param>
    private bool CheckPermissions(string permissionName)
    {
        var cui = MembershipContext.AuthenticatedUser;
        switch (VariantMode)
        {
            case VariantModeEnum.MVT:
                return cui.IsAuthorizedPerResource("cms.mvtest", permissionName);

            case VariantModeEnum.ContentPersonalization:
                return cui.IsAuthorizedPerResource("cms.contentpersonalization", permissionName);

            case VariantModeEnum.Conflicted:
            case VariantModeEnum.None:
                return cui.IsAuthorizedPerResource("cms.mvtest", permissionName) || cui.IsAuthorizedPerResource("cms.contentpersonalization", permissionName);
        }

        return true;
    }


    /// <summary>
    /// Gets the UI culture for localized strings.
    /// </summary>
    private string UICulture
    {
        get
        {
            if (string.IsNullOrEmpty(mUICulture))
            {
                mUICulture = CultureHelper.GetPreferredUICultureCode();
            }
            return mUICulture;
        }
    }

    #endregion


    #region "Page methods"

    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Check permissions
        if ((!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Design", "Design") && (SliderMode != VariantTypeEnum.Widget))
            || !CheckPermissions("Read"))
        {
            stopProcessing = true;
            pnlVariations.Visible = false;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize the uniqueCode variable
        switch (SliderMode)
        {
            case VariantTypeEnum.Zone:
                // Zone
                uniqueCode = "Variant_Zone_" + WebPartZoneControl.ZoneInstance.ZoneID;
                break;

            case VariantTypeEnum.WebPart:
            case VariantTypeEnum.Widget:
                // Web part
                uniqueCode = "Variant_WP_" + WebPartControl.PartInstance.InstanceGUID.ToString("N");
                break;
        }

        // Process the postback manually (this must be done manually because when moving web parts between the zones,
        // the control ID of the moved web part is not changed in DOM and is not handled properly by the system)
        if (RequestHelper.IsPostBack())
        {
            string argument = Request[Page.postEventArgumentID];
            if (!string.IsNullOrEmpty(argument))
            {
                string[] parts = argument.Split(new char[] { ':' });
                if (parts.Length == 3)
                {
                    // Get uniqueCode, action argument, current variant ID
                    string argUniqueCode = parts[0];
                    string argArgument = parts[1];
                    int argVariantId = ValidationHelper.GetInteger(parts[2], 0);

                    // Process the post back event if the event was raised by this control
                    if (argUniqueCode == uniqueCode)
                    {
                        RaisePostBackEvent(argArgument, argUniqueCode, argVariantId);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (stopProcessing)
        {
            Visible = false;
            return;
        }

        // Check permissions
        if (!CheckPermissions("Manage"))
        {
            // Hide add/remove variant buttons when the Manage permission is not allowed
            plcRemoveVariant.Visible = false;
            plcAddVariant.Visible = false;
        }

        // Hide buttons in the template edit in the site manager
        if (DocumentContext.CurrentDocument == null)
        {
            plcRemoveVariant.Visible = false;
            plcAddVariant.Visible = false;
            plcVariantList.Visible = false;
        }

        // Get the sum of all variants
        int totalVariants = 0;

        switch (SliderMode)
        {
            case VariantTypeEnum.Zone:
                // Zone
                if ((WebPartZoneControl != null)
                    && (WebPartZoneControl.HasVariants)
                    && (WebPartZoneControl.ZoneInstance != null)
                    && (WebPartZoneControl.ZoneInstance.ZoneInstanceVariants != null))
                {
                    totalVariants = WebPartZoneControl.ZoneInstance.ZoneInstanceVariants.Count;
                }
                break;

            case VariantTypeEnum.WebPart:
            case VariantTypeEnum.Widget:
                // Web part
                // Widget
                if ((WebPartControl != null)
                    && (WebPartControl.HasVariants)
                    && (WebPartControl.PartInstance != null)
                    && (WebPartControl.PartInstance.PartInstanceVariants != null))
                {
                    totalVariants = WebPartControl.PartInstance.PartInstanceVariants.Count;
                }
                break;
        }

        // Increase by 1 to include the original webpart
        totalVariants++;

        // Reset the slider state (correct variant for the current combination is chosen by javascript in window.onload)
        txtSlider.Text = "1";
        if (isRTL)
        {
            // Reverse position index when in RTL
            txtSlider.Text = totalVariants.ToString();
        }

        // Change the slider CSS class if used for widgets
        if ((WebPartControl != null)
            && WebPartControl.IsWidget)
        {
            pnlVariations.CssClass = "WidgetVariantSlider";
        }

        // Setup the variant slider extender
        sliderExtender.Minimum = 1;
        sliderExtender.Maximum = totalVariants;
        sliderExtender.Steps = totalVariants;
        sliderExtender.HandleImageUrl = GetImageUrl("Design/Controls/VariantSlider/slider.png");

        if (isRTL)
        {
            // RTL culture - set the javascript variable
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "VariantSliderRTL", ScriptHelper.GetScript("variantSliderIsRTL = true;"));
        }

        // Change the arrows design for MVT
        if ((VariantMode == VariantModeEnum.MVT) || !PortalContext.ContentPersonalizationEnabled)
        {
            //pnlLeft.CssClass = "SliderLeftMVT";
            //pnlRight.CssClass = "SliderRightMVT";
        }

        CssRegistration.RegisterBootstrap(Page);

        sliderExtender.HandleCssClass = "slider-horizontal-handle";
        sliderExtender.RailCssClass = "slider-horizontal-rail";
        lblTotal.Text = totalVariants.ToString();

        txtSlider.Attributes.Add("onchange", "$cmsj('#" + hdnSliderPosition.ClientID + "').change();");
        txtSlider.Style.Add("display", "none");

        // Prepare the parameters
        string zoneId = string.Empty;

        PageInfo pi = PagePlaceholder.PageInfo;

        string aliasPath = pi.NodeAliasPath;
        string webPartName = string.Empty;
        string instanceGuidString = string.Empty;

        switch (SliderMode)
        {
            case VariantTypeEnum.Zone:
                // Zone
                zoneId = WebPartZoneControl.ZoneInstance.ZoneID;
                break;

            case VariantTypeEnum.WebPart:
            case VariantTypeEnum.Widget:
                // Web part
                zoneId = WebPartControl.PartInstance.ParentZone.ZoneID;
                instanceGuidString = WebPartControl.InstanceGUID.ToString();
                webPartName = WebPartControl.PartInstance.ControlID;
                break;
        }

        // Setup tooltips
        pnlLeft.ToolTip = ResHelper.GetString("variantslider.btnleft", UICulture);
        pnlRight.ToolTip = ResHelper.GetString("variantslider.btnright", UICulture);
        imgRemoveVariant.ToolTip = ResHelper.GetString("variantslider.btnremove", UICulture);
        imgVariantList.ToolTip = ResHelper.GetString("variantslider.btnlist", UICulture);

        // Setup default behaviour - no action executed
        imgRemoveVariantDisabled.Attributes.Add("onclick", "return false;");
        imgVariantList.Attributes.Add("onclick", "return false;");

        // Cancel propagation of the double-click event (skips opening the web part properties dialog)
        pnlVariations.Attributes.Add("ondblclick", "CancelEventPropagation(event)");

        // Hidden field used for changing the slider position. The position of the slider is stored also here because of the usage of the slider arrows.
        hdnSliderPosition.Style.Add("display", "none");
        hdnSliderPosition.Attributes.Add("onchange", "OnHiddenChanged(this, document.getElementById('" + lblPart.ClientID + "'), '" + uniqueCode + "', '" + sliderExtender.BehaviorID + @"' );");

        String zoneIdPar = (WebPartControl != null) ? "GetActualZoneId('wp_" + WebPartControl.InstanceGUID.ToString("N") + "')" : "'" + zoneId + "'";
        string dialogParams = zoneIdPar + ", '" + webPartName + "', '" + aliasPath + "', '" + instanceGuidString + "', 0, '" + VariantTypeFunctions.GetVariantTypeString(SliderMode) + "'";

        // Allow edit actions
        if (totalVariants == 1)
        {
            if (SliderMode == VariantTypeEnum.Widget)
            {
                plcRemoveVariant.Visible = false;
                plcVariantList.Visible = false;
                plcSliderPanel.Visible = false;
                var cui = MembershipContext.AuthenticatedUser;
                bool manageMVT = cui.IsAuthorizedPerResource("cms.mvtest", "manage") && cui.IsAuthorizedPerResource("cms.mvtest", "read");
                bool manageCP = cui.IsAuthorizedPerResource("cms.contentpersonalization", "manage") && cui.IsAuthorizedPerResource("cms.contentpersonalization", "read");

                if (PortalContext.MVTVariantsEnabled && PortalContext.ContentPersonalizationEnabled && manageMVT && manageCP)
                {
                    pnlAddVariantWrapper.Attributes.Add("onclick", "OpenMenuAddWidgetVariant(this, '" + WebPartControl.ShortClientID + "'); return false;");
                    imgAddVariant.ToolTip = ResHelper.GetString("variantslider.btnadd", UICulture);

                    // Script for opening a new variant dialog window and activating widget border to prevent to widget border from hiding
                    // when the user moves his mouse to the 'add widget' context menu.
                    string script = @"
function OpenMenuAddWidgetVariant(menuPositionEl, targetId) {
    currentContextMenuId = targetId;
    ContextMenu('addWidgetVariantMenu', menuPositionEl, webPartLocation[targetId + '_container'], true);
    AutoPostitionContextMenu('addWidgetVariantMenu');
}";
                    ScriptHelper.RegisterStartupScript(this, typeof(string), "OpenMenuAddWidgetVariantScript", ScriptHelper.GetScript(script));
                }
                else
                {
                    if (PortalContext.MVTVariantsEnabled && manageMVT)
                    {
                        imgAddVariant.Attributes.Add("onclick", "AddMVTVariant(" + dialogParams + "); return false;");
                        imgAddVariant.ToolTip = ResHelper.GetString("variantslider.btnaddmvt", UICulture);
                    }
                    else if (PortalContext.ContentPersonalizationEnabled && manageCP)
                    {
                        imgAddVariant.Attributes.Add("onclick","AddPersonalizationVariant(" + dialogParams + "); return false;");
                        imgAddVariant.ToolTip = ResHelper.GetString("variantslider.btnaddpesronalization", UICulture);
                    }
                }
            }
        }
        else
        {
            if (VariantMode == VariantModeEnum.MVT)
            {
                imgAddVariant.Attributes.Add("onclick","AddMVTVariant(" + dialogParams + "); return false;");
                imgAddVariant.ToolTip = ResHelper.GetString("variantslider.btnaddmvt", UICulture);
            }
            else
            {
                imgAddVariant.Attributes.Add("onclick","AddPersonalizationVariant(" + dialogParams + "); return false;");
                imgAddVariant.ToolTip = ResHelper.GetString("variantslider.btnaddpesronalization", UICulture);
            }
        }

        if ((totalVariants > 1) || (SliderMode == VariantTypeEnum.Widget))
        {
            // Register only for full postback or first page load
            if (!RequestHelper.IsAsyncPostback())
            {
                if ((VariantMode == VariantModeEnum.MVT))
                {
                    // Activate the variant list button fot MVT
                    imgVariantList.Attributes.Add("onclick", "ListMVTVariants(" + dialogParams + ", '" + uniqueCode + "'); return false;");
                }
                else if (VariantMode == VariantModeEnum.ContentPersonalization)
                {
                    // Activate the variant list button for Content personalization
                    imgVariantList.Attributes.Add("onclick", "ListPersonalizationVariants(" + dialogParams + ", '" + uniqueCode + "'); return false;");
                }

                // Assign the onclick event for he Remove variant button
                imgRemoveVariant.Attributes.Add("onclick", "RemoveVariantPostBack_" + uniqueCode + @"(); return false");

                // Register Remove variant script
                string removeVariantScript = @"
                function RemoveVariantPostBack_" + uniqueCode + @"() {
                    if (confirm(" + ScriptHelper.GetLocalizedString("variantslider.removeconfirm") + @")) {" +
                        DocumentManager.GetAllowSubmitScript() + @"
                        var postBackCode = '" + uniqueCode + ":remove:' + GetCurrentVariantId('" + uniqueCode + @"');
                        SetVariant('" + uniqueCode + @"', 0);"
                                             + ControlsHelper.GetPostBackEventReference(this, "#").Replace("'#'", "postBackCode") + @";
                    }
                }";

                ScriptHelper.RegisterStartupScript(Page, typeof(string), "removeVariantScript_" + uniqueCode, ScriptHelper.GetScript(removeVariantScript));

                int step = 1;
                if (isRTL)
                {
                    // Reverse step direction
                    step = -1;
                }
                // Assign the onclick events for the slider arrows
                pnlLeft.Attributes.Add("onclick", "OnSliderChanged(event, '" + hdnSliderPosition.ClientID + "', " + totalVariants + ", " + step * (-1) + ");");
                pnlRight.Attributes.Add("onclick", "OnSliderChanged(event, '" + hdnSliderPosition.ClientID + "', " + totalVariants + ", " + step + ");");

                // Get all variants GUIDs
                List<string> variantIDsArray = new List<string>();
                List<string> variantControlIDsArray = new List<string>();
                List<string> divIDsArray = new List<string>();

                switch (SliderMode)
                {
                    case VariantTypeEnum.Zone:
                        // Zone
                        if ((WebPartZoneControl != null)
                            && (WebPartZoneControl.ZoneInstance != null)
                            && (WebPartZoneControl.ZoneInstance.ZoneInstanceVariants != null))
                        {
                            // Fill the variant IDs array
                            variantIDsArray = WebPartZoneControl.ZoneInstance.ZoneInstanceVariants.Select(zone => zone.VariantID.ToString()).ToList<string>();
                            // First item is the original zone (variantid=0)
                            variantIDsArray.Insert(0, "0");

                            // Fill the variant control IDs array
                            variantControlIDsArray = WebPartZoneControl.ZoneInstance.ZoneInstanceVariants.Select(zone => "\"" + (!string.IsNullOrEmpty(ValidationHelper.GetString(zone.Properties["zonetitle"], string.Empty)) ? HTMLHelper.HTMLEncode(zone.Properties["zonetitle"].ToString()) : zone.ZoneID) + "\"").ToList<string>();
                            // First item is the original web part/widget
                            variantControlIDsArray.Insert(0, "\"" + (!string.IsNullOrEmpty(WebPartZoneControl.ZoneTitle) ? HTMLHelper.HTMLEncode(WebPartZoneControl.ZoneTitle) : WebPartZoneControl.ZoneInstance.ZoneID) + "\"");

                            // Fill the DIV tag IDs array
                            divIDsArray = WebPartZoneControl.ZoneInstance.ZoneInstanceVariants.Select(zone => "\"Variant_" + VariantModeFunctions.GetVariantModeString(zone.VariantMode) + "_" + zone.VariantID.ToString() + "\"").ToList<string>();
                            // First item is the original web part
                            divIDsArray.Insert(0, "\"" + uniqueCode + "\"");
                        }
                        break;

                    case VariantTypeEnum.WebPart:
                    case VariantTypeEnum.Widget:
                        // Web part or widget
                        if ((WebPartControl != null)
                            && (WebPartControl.PartInstance != null)
                            && (WebPartControl.PartInstance.PartInstanceVariants != null))
                        {
                            // Fill the variant IDs array
                            variantIDsArray = WebPartControl.PartInstance.PartInstanceVariants.Select(webpart => webpart.VariantID.ToString()).ToList<string>();
                            // First item is the original web part/widget (variantid=0)
                            variantIDsArray.Insert(0, "0");

                            // Fill the variant control IDs array
                            variantControlIDsArray = WebPartControl.PartInstance.PartInstanceVariants.Select(webpart => "\"" + (!string.IsNullOrEmpty(ValidationHelper.GetString(webpart.Properties["webparttitle"], string.Empty)) ? HTMLHelper.HTMLEncode(webpart.Properties["webparttitle"].ToString()) : webpart.ControlID) + "\"").ToList<string>();
                            // First item is the original web part/widget
                            variantControlIDsArray.Insert(0, "\"" + (!string.IsNullOrEmpty(ValidationHelper.GetString(WebPartControl.PartInstance.Properties["webparttitle"], string.Empty)) ? HTMLHelper.HTMLEncode(WebPartControl.PartInstance.Properties["webparttitle"].ToString()) : WebPartControl.PartInstance.ControlID) + "\"");

                            // Fill the DIV tag IDs array
                            divIDsArray = WebPartControl.PartInstance.PartInstanceVariants.Select(webpart => "\"Variant_" + VariantModeFunctions.GetVariantModeString(webpart.VariantMode) + "_" + webpart.VariantID + "\"").ToList<string>();
                            // First item is the original web part/widget
                            divIDsArray.Insert(0, "\"" + uniqueCode + "\"");
                        }
                        break;
                }

                // Create a javascript arrays:
                // Fill the following javascript array: itemCodesArray.push([variantIDs], [divIDs], actualSliderPosition, totalVariants, variantSliderId, sliderElement, hiddenElem_SliderPosition, zoneId, webPartInstanceGuid)
                StringBuilder sb = new StringBuilder();
                sb.Append("itemIDs = [");
                sb.Append(String.Join(",", variantIDsArray.ToArray()));
                sb.Append("]; divIDs = [");
                sb.Append(String.Join(",", divIDsArray.ToArray()));
                sb.Append("]; itemControlIDs = [");
                sb.Append(String.Join(",", variantControlIDsArray.ToArray()));
                sb.Append("];");
                sb.Append("itemCodes = [itemIDs, divIDs, itemControlIDs, 1, "); // 0, 1, 2, 3 (see the details in the 'variants.js' file)
                sb.Append(totalVariants); // 4
                sb.Append(", \"");
                sb.Append(pnlVariations.ClientID); // 5
                sb.Append("\", \"");
                sb.Append(sliderExtender.ClientID); // 6
                sb.Append("_handleImage\", \"");
                sb.Append(hdnSliderPosition.ClientID); // 7
                sb.Append("\", \"");
                if (SliderMode == VariantTypeEnum.Zone) // 8
                {
                    sb.Append(WebPartZoneControl.TitleLabel.ClientID);
                }
                else
                {
                    // Display label only for web parts (editor widgets have title hidden)
                    if (WebPartControl.TitleLabel != null)
                    {
                        sb.Append(WebPartControl.TitleLabel.ClientID);
                    }
                }
                sb.Append("\", \"");
                sb.Append(zoneId); // 9
                sb.Append("\", \"");
                if (SliderMode != VariantTypeEnum.Zone) // 10
                {
                    sb.Append(instanceGuidString);
                }
                sb.Append("\", \"");
                sb.Append(VariantModeFunctions.GetVariantModeString(VariantMode)); // 11
                sb.Append("\", \"");
                sb.Append(pnlVariations.ClientID); // 12
                sb.Append("\"]; itemCodesAssociativeArray[\"");
                sb.Append(uniqueCode);
                sb.Append("\"] = itemCodes;");

                ScriptHelper.RegisterStartupScript(Page, typeof(string), sliderExtender.ClientID + "_InitScript", sb.ToString(), true);
            }
        }
        else
        {
            Visible = false;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Gets the parent control which is the web part or zone control.
    /// </summary>
    /// <param name="ctrl">The control</param>
    private Control GetBaseControl(Control ctrl)
    {
        if (ctrl != null)
        {
            if ((ctrl is CMSWebPartZone)
                || (ctrl is CMSAbstractWebPart))
            {
                return ctrl;
            }

            return GetBaseControl(ctrl.Parent);
        }

        return null;
    }

    #endregion


    #region "IPostBackEventHandler Members"


    /// <summary>
    /// Raises event postback event.
    /// </summary>
    /// <param name="eventArgument">Event argument</param>
    /// <param name="uniqueCode">Unique code</param>
    /// <param name="variantId">Variant ID</param>
    public void RaisePostBackEvent(string eventArgument, string uniqueCode, int variantId)
    {
        // Check permissions
        if (!CheckPermissions("Manage")
            || stopProcessing)
        {
            return;
        }

        // Get the argument
        string arg = eventArgument.ToLowerCSafe();

        if (arg == "remove")
        {
            // Remove variant action
            int documentId = 0;

            if (VariantMode == VariantModeEnum.MVT)
            {
                // Is MVT zone => remove the MVT variant
                MVTVariantInfo variantObj = MVTVariantInfoProvider.GetMVTVariantInfo(variantId);
                if (variantObj != null)
                {
                    // Delete the variant
                    MVTVariantInfoProvider.DeleteMVTVariantInfo(variantObj);
                    documentId = variantObj.MVTVariantDocumentID;
                }
            }
            else if (VariantMode == VariantModeEnum.ContentPersonalization)
            {
                // Is Content personalization zone => remove the Content personalization variant
                ContentPersonalizationVariantInfo variantObj = ContentPersonalizationVariantInfoProvider.GetContentPersonalizationVariant(variantId);
                if (variantObj != null)
                {
                    // Delete the variant
                    ContentPersonalizationVariantInfoProvider.DeleteContentPersonalizationVariant(variantObj);
                    documentId = variantObj.VariantDocumentID;
                }
            }
            else
            {
                return;
            }

            // Log widget variant synchronization
            if (documentId > 0)
            {
                TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
                TreeNode node = DocumentHelper.GetDocument(documentId, tree);
                DocumentSynchronizationHelper.LogDocumentChange(node, TaskTypeEnum.UpdateDocument, tree);
            }

            switch (SliderMode)
            {
                // Remove zone variant
                case VariantTypeEnum.Zone:
                    if ((WebPartZoneControl != null) && (WebPartZoneControl.ZoneInstance != null))
                    {
                        // Remove the variant from WebPartZoneControl.ZoneInstance.ZoneInstanceVariants.
                        // It is necessary to remove the variants from the PartInstanceVariants list manually because the PageLoad method has already run
                        // and the PartInstanceVariants list was populated with the old values.
                        WebPartZoneControl.RemoveVariantFromCache(variantId);
                    }
                    break;

                // Remove web part or widget variant
                case VariantTypeEnum.WebPart:
                case VariantTypeEnum.Widget:
                    if ((WebPartControl != null) && (WebPartControl.PartInstance != null))
                    {
                        // Remove the variant from WebPartControls.PartInstance.PartInstanceVariants.
                        // It is necessary to remove the variants from the PartInstanceVariants list manually because the PageLoad method has already run
                        // and the PartInstanceVariants list was populated with the old values.
                        WebPartControl.RemoveVariantFromCache(variantId);

                        // If there are no other variants present, set the VariantMode to None to allow refreshing the add variant buttons.
                        if ((WebPartControl.PartInstance.PartInstanceVariants == null) || (WebPartControl.PartInstance.PartInstanceVariants.Count == 0))
                        {
                            mVariantMode = VariantModeEnum.None;
                        }
                    }
                    break;

                default:
                    break;
            }

            // Refresh the variant slider position => choose the last variant
            ltrScript.Text = ScriptHelper.GetScript(
                @"cpVariantSliderPositionElem = GetCPVariantSliderPositionElem();
                UpdateVariantPosition('" + uniqueCode + @"', '-1');
                SaveSlidersConfiguration();");
        }
    }

    #endregion
}
