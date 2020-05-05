using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Settings_Controls_SettingsGroupViewer : SettingsGroupViewerControl
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Get search parameters
        SearchText = QueryHelper.GetString("search", "").Trim();
        SearchDescription = QueryHelper.GetBoolean("description", false);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (SettingsCategoryInfo == null)
        {
            plcContent.Append(GetString("settings.keys.nocategoryselected"));
            StopProcessing = true;
            return;
        }

        ScriptHelper.RegisterTooltip(Page);
        ScriptHelper.RegisterBootstrapTooltip(Page, ".info-icon > i");

        // Loop through all the groups in the category
        int groupCount = 0;
        bool hasOnlyGlobalKeys = true;
        var groups = GetGroups(SettingsCategoryInfo.CategoryName);

        foreach (var group in groups)
        {
            // Get keys
            var keys = GetKeys(group.CategoryID).ToArray();

            // Skip empty group
            if (!keys.Any())
            {
                continue;
            }

            groupCount++;

            // Add category panel for the group
            var pnlGroup = GetCategoryPanel(group, groupCount);
            plcContent.Append(pnlGroup);

            // Loop through all the keys in the group
            int keyCount = 0;
            foreach (var keyInfo in keys)
            {
                // Increase key number for unique control identification
                keyCount++;

                // Update flag when non-global-only key exists
                if (!keyInfo.KeyIsGlobal)
                {
                    hasOnlyGlobalKeys = false;
                }

                // Create key item
                var keyItem = new SettingsKeyItem
                {
                    ParentCategoryPanel = pnlGroup,
                    KeyName = keyInfo.KeyName,
                    KeyType = keyInfo.KeyType,
                    ValidationRegexPattern = keyInfo.KeyValidation,
                    CategoryName = group.CategoryName,
                    ExplanationText = ResHelper.LocalizeString(keyInfo.KeyExplanationText)
                };

                var parameters = new SettingsKeyRenderEventArgs(keyItem);
                using (var h = SettingsKeyRender.StartEvent(parameters))
                {
                    Panel pnlRow = new Panel
                    {
                        CssClass = "form-group"
                    };
                    pnlGroup.Controls.Add(pnlRow);

                    // Add label cell to the beginning of the row
                    var pnlLabelCell = new Panel
                    {
                        CssClass = "editing-form-label-cell"
                    };
                    pnlRow.Controls.AddAt(0, pnlLabelCell);

                    // Continue with the value cell
                    pnlRow.Controls.Add(new LiteralControl(@"<div class=""editing-form-value-cell"">"));

                    // Create placeholder for the editing control that may end up in an update panel
                    var pnlValueCell = new Panel
                    {
                        CssClass = "settings-group-inline keep-white-space-fixed"
                    };
                    var pnlValue = new Panel
                    {
                        CssClass = "editing-form-control-nested-control keep-white-space-fixed"
                    };
                    pnlValueCell.Controls.Add(pnlValue);
                    var pnlIcons = new Panel
                    {
                        CssClass = "settings-info-group keep-white-space-fixed"
                    };
                    pnlValueCell.Controls.Add(pnlIcons);

                    // Don't show help icon when not provided. (Help icon will be shown if macro resolution results in an empty string.)
                    if (!String.IsNullOrWhiteSpace(keyInfo.KeyDescription))
                    {
                        Label helpIcon = UIHelper.GetIcon("icon-question-circle", ResHelper.LocalizeString(keyInfo.KeyDescription));
                        pnlIcons.Controls.Add(helpIcon);
                    }

                    CMSCheckBox chkInherit = null;
                    if (SiteID > 0)
                    {
                        // Wrap in update panel for inherit checkbox postback
                        var pnlValueUpdate = new UpdatePanel
                        {
                            ID = $"pnlValueUpdate{groupCount}_{keyCount}",
                            UpdateMode = UpdatePanelUpdateMode.Conditional,
                        };
                        pnlRow.Controls.Add(pnlValueUpdate);

                        // Add inherit checkbox
                        chkInherit = GetInheritCheckBox(groupCount, keyCount);
                        keyItem.InheritCheckBox = chkInherit;

                        pnlValueUpdate.ContentTemplateContainer.Controls.Add(chkInherit);

                        pnlValueUpdate.ContentTemplateContainer.Controls.Add(pnlValueCell);
                    }
                    else
                    {
                        pnlRow.Controls.Add(pnlValueCell);

                        // Add "current site does not inherit the global value" warning for global settings
                        if (SiteContext.CurrentSite != null)
                        {
                            var isCurrentSiteValueInherited = SettingsKeyInfoProvider.IsValueInherited(keyInfo.KeyName, SiteContext.CurrentSiteID);
                            if (!isCurrentSiteValueInherited)
                            {
                                string inheritWarningText = String.Format(GetString("settings.currentsitedoesnotinherit"), ResHelper.LocalizeString(SiteContext.CurrentSite.DisplayName));
                                Label inheritWarningImage = UIHelper.GetIcon("icon-exclamation-triangle warning-icon", HTMLHelper.HTMLEncode(inheritWarningText));

                                pnlIcons.Controls.Add(inheritWarningImage);
                            }
                        }
                    }

                    // Add explanation text
                    if (!String.IsNullOrWhiteSpace(keyItem.ExplanationText))
                    {
                        Panel pnlExplanationText = new Panel
                        {
                            CssClass = "explanation-text-settings"
                        };
                        LocalizedLiteral explanationText = new LocalizedLiteral
                        {
                            Text = keyItem.ExplanationText
                        };
                        pnlExplanationText.Controls.Add(explanationText);
                        pnlRow.Controls.Add(pnlExplanationText);
                    }

                    pnlRow.Controls.Add(new LiteralControl("</div>"));

                    // Get current values
                    keyItem.KeyIsInherited = SettingsKeyInfoProvider.IsValueInherited(keyInfo.KeyName, SiteID);
                    keyItem.KeyValue = SettingsKeyInfoProvider.GetValue(keyInfo.KeyName, SiteID);

                    // Get value
                    string keyValue;
                    bool isInherited;
                    if (RequestHelper.IsPostBack() && (chkInherit != null))
                    {
                        isInherited = Request.Form[chkInherit.UniqueID] != null;
                        keyValue = isInherited ? SettingsKeyInfoProvider.GetValue(keyInfo.KeyName) : SettingsKeyInfoProvider.GetValue(keyInfo.KeyName, SiteID);
                    }
                    else
                    {
                        isInherited = keyItem.KeyIsInherited;
                        keyValue = keyItem.KeyValue;

                        // Set the inherit checkbox state
                        if (!RequestHelper.IsPostBack() && chkInherit != null)
                        {
                            chkInherit.Checked = isInherited;
                        }
                    }

                    // Add value editing control
                    var enabled = !isInherited;
                    FormEngineUserControl control = GetFormEngineUserControl(keyInfo, groupCount, keyCount);
                    if (control != null)
                    {
                        // Add form engine value editing control
                        control.Value = keyValue;
                        pnlValue.Controls.Add(control);

                        // Set form control enabled value, does not work when moved before plcControl.Controls.Add(control)
                        control.Enabled = enabled;

                        keyItem.ValueControl = control;

                        if (chkInherit != null)
                        {
                            chkInherit.CheckedChanged += (sender, args) =>
                            {
                                control.Value = keyValue;
                            };
                        }
                    }
                    else
                    {
                        // Add simple value editing control
                        switch (keyInfo.KeyType.ToLowerInvariant())
                        {
                            case "boolean":
                                // Add checkbox value editing control
                                var @checked = ValidationHelper.GetBoolean(keyValue, false);
                                CMSCheckBox chkValue = GetValueCheckBox(groupCount, keyCount, @checked, enabled);
                                pnlValue.Controls.Add(chkValue);

                                keyItem.ValueControl = chkValue;

                                if (chkInherit != null)
                                {
                                    chkInherit.CheckedChanged += (sender, args) =>
                                    {
                                        chkValue.Checked = @checked;
                                    };
                                }
                                break;

                            case "longtext":
                                // Add text area value editing control
                                var longText = keyValue;
                                var txtValueTextArea = GetValueTextArea(groupCount, keyCount, longText, enabled);
                                if (txtValueTextArea != null)
                                {
                                    // Text area control was loaded successfully
                                    pnlValue.Controls.Add(txtValueTextArea);
                                    keyItem.ValueControl = txtValueTextArea;
                                    if (chkInherit != null)
                                    {
                                        chkInherit.CheckedChanged += (sender, args) =>
                                        {
                                            txtValueTextArea.Text = longText;
                                        };
                                    }
                                }
                                else
                                {
                                    // Text area control was not loaded successfully
                                    var errorLabel = new FormControlError
                                    {
                                        ErrorTitle = "[Error loading the editing control, check the event log for more details]",
                                    };
                                    pnlValue.Controls.Add(errorLabel);
                                }
                                break;

                            default:
                                // Add textbox value editing control
                                var text = keyValue;
                                TextBox txtValue = GetValueTextBox(groupCount, keyCount, text, enabled);
                                pnlValue.Controls.Add(txtValue);

                                keyItem.ValueControl = txtValue;

                                if (chkInherit != null)
                                {
                                    chkInherit.CheckedChanged += (sender, args) =>
                                    {
                                        txtValue.Text = text;
                                    };
                                }
                                break;
                        }
                    }

                    // Add label to the label cell when associated control has been resolved
                    pnlLabelCell.Controls.Add(GetLabel(keyInfo, keyItem.ValueControl, groupCount, keyCount));

                    // Add error label if KeyType is integer or validation expression defined or FormControl is used
                    if ((keyInfo.KeyType == "int") || (keyInfo.KeyType == "double") || (keyItem.ValidationRegexPattern != null) || (control != null))
                    {
                        Label lblError = GetLabelError(groupCount, keyCount);
                        pnlIcons.Controls.Add(lblError);
                        keyItem.ErrorLabel = lblError;
                    }

                    h.FinishEvent();
                }

                KeyItems.Add(keyItem);
            }
        }

        // Show info message when other than global-only global keys are displayed
        if ((SiteID <= 0) && (CategoryID > 0) && !hasOnlyGlobalKeys && AllowGlobalInfoMessage)
        {
            ShowInformation(GetString("settings.keys.globalsettingsnote"));
        }

        // Display export and reset links only if some groups were found.
        if (groupCount > 0)
        {
            // Add reset link if required
            if (!RequestHelper.IsPostBack() && QueryHelper.GetInteger("resettodefault", 0) == 1)
            {
                ShowInformation(GetString("Settings-Keys.ValuesWereResetToDefault"));
            }
        }
        else
        {
            // Hide "These settings are global ..." message if no setting found in this group
            if (!string.IsNullOrEmpty(SearchText))
            {
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "SettingsGroupViewer_DisableHeaderActions", ScriptHelper.GetScript("DisableHeaderActions();"));

                lblNoData.Visible = true;
            }
        }
    }
}