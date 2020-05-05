<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Widgets_Controls_WidgetSelector"  Codebehind="WidgetSelector.ascx.cs" %>
<%@ Register Src="~/CMSModules/Widgets/Controls/WidgetFlatSelector.ascx" TagName="WidgetFlatSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Widgets/Controls/WidgetTree.ascx" TagName="WidgetTree"
    TagPrefix="cms" %>
<table class="SelectorTable" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <div class="SelectorTree">
                <div class="TreePadding">
                    <cms:WidgetTree ID="treeElem" runat="server" SelectWidgets="false" UsePostBack="false"
                        ShowRecentlyUsed="true" EnableViewState="false" />
                </div>
            </div>
        </td>
        <td class="SelectorBorder">
            <div class="SelectorBorderGlue">
            </div>
        </td>
        <td class="ItemSelectorArea">
            <div class="ItemSelector">
                <cms:WidgetFlatSelector ID="flatElem" runat="server" />
            </div>
        </td>
    </tr>
</table>
