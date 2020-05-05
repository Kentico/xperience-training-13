using System;
using System.Text.RegularExpressions;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.Base.Web.UI;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.PortalEngine.Web.UI;


[assembly: RegisterCustomClass("MacroRuleEditExtender", typeof(MacroRuleEditExtender))]

/// <summary>
/// Macro rule UIForm extender
/// </summary>
public class MacroRuleEditExtender : ControlExtender<UIForm>
{
    #region "Methods and event handlers"

    public override void OnInit()
    {
        Control.OnBeforeSave += EditForm_OnBeforeSave;
    }


    protected void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        MacroRuleInfo info = Control.EditedObject as MacroRuleInfo;
        if (info != null)
        {
            // Generate automatic fields when present in UserText
            FormEngineUserControl control = Control.FieldControls["MacroRuleText"];
            if (control != null)
            {
                string userText = ValidationHelper.GetString(control.Value, String.Empty);
                if (!string.IsNullOrEmpty(userText))
                {
                    Regex regex = RegexHelper.GetRegex("\\{[-_a-zA-Z0-9]*\\}");
                    MatchCollection match = regex.Matches(userText);
                    if (match.Count > 0)
                    {
                        FormInfo fi = new FormInfo(info.MacroRuleParameters);
                        foreach (Match m in match)
                        {
                            foreach (Capture c in m.Captures)
                            {
                                string name = c.Value.Substring(1, c.Value.Length - 2).ToLowerCSafe();
                                FormFieldInfo ffi = fi.GetFormField(name);
                                if (ffi == null)
                                {
                                    ffi = new FormFieldInfo();
                                    ffi.Name = name;
                                    ffi.DataType = FieldDataType.Text;
                                    ffi.Size = 100;
                                    ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "select operation");
                                    ffi.AllowEmpty = true;
                                    switch (name)
                                    {
                                        case "_is":
                                            ffi.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, ";is");
                                            ffi.SetControlName("MacroNegationOperator");
                                            ffi.Settings["EditText"] = "false";
                                            ffi.Settings["Options"] = ";is\r\n!;is not";
                                            break;

                                        case "_was":
                                            ffi.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, ";was");
                                            ffi.SetControlName("MacroNegationOperator");
                                            ffi.Settings["EditText"] = "false";
                                            ffi.Settings["Options"] = ";was\r\n!;was not";
                                            break;

                                        case "_will":
                                            ffi.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, ";will");
                                            ffi.SetControlName("MacroNegationOperator");
                                            ffi.Settings["EditText"] = "false";
                                            ffi.Settings["Options"] = ";will\r\n!;will not";
                                            break;

                                        case "_has":
                                            ffi.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, ";has");
                                            ffi.SetControlName("MacroNegationOperator");
                                            ffi.Settings["EditText"] = "false";
                                            ffi.Settings["Options"] = ";has\r\n!;does not have";
                                            break;

                                        case "_perfectum":
                                            ffi.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, ";has");
                                            ffi.SetControlName("MacroNegationOperator");
                                            ffi.Settings["EditText"] = "false";
                                            ffi.Settings["Options"] = ";has\r\n!;has not";
                                            break;

                                        case "_any":
                                            ffi.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, "false;any");
                                            ffi.SetControlName("macro_any-all_bool_selector");
                                            ffi.Settings["EditText"] = "false";
                                            ffi.Settings["Options"] = "false;any\r\ntrue;all";
                                            break;

                                        default:
                                            ffi.Size = 1000;
                                            ffi.SetControlName(FormFieldControlName.TEXTBOX);
                                            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "enter text");
                                            break;
                                    }

                                    fi.AddFormItem(ffi);
                                }
                            }
                        }
                        info.MacroRuleParameters = fi.GetXmlDefinition();
                    }
                }
            }
        }

        Control.EditedObject.SetValue("MacroRuleIsCustom", !SystemContext.DevelopmentMode);
    }

    #endregion
}