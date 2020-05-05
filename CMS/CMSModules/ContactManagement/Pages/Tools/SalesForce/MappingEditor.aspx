<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="MappingEditor.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_SalesForce_MappingEditor" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" EnableEventValidation="false" Theme="Default" %>

<%@ Register TagPrefix="cms" TagName="SalesForceError" Src="~/CMSModules/ContactManagement/Controls/UI/SalesForce/Error.ascx" %>
<%@ Import Namespace="System.Linq" %>
<%@ Register TagPrefix="cms" TagName="Mapping" Src="~/CMSModules/ContactManagement/Controls/UI/SalesForce/Mapping.ascx" %>
<%@ Register TagPrefix="cms" TagName="MappingEditorItem" Src="~/CMSModules/ContactManagement/Controls/UI/SalesForce/MappingEditorItem.ascx" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="plcContent" runat="Server" EnableViewState="false">
    <asp:HiddenField ID="MappingHiddenField" runat="server" EnableViewState="false" />
    <asp:Panel ID="MappingPanel" runat="server" EnableViewState="false" CssClass="Hidden">
        <cms:Mapping ID="MappingControl" runat="server" EnableViewState="false" />
    </asp:Panel>
    <asp:Panel ID="MainPanel" runat="server" EnableViewState="false">
        <cms:SalesForceError ID="SalesForceError" runat="server" EnableViewState="false" MessagesEnabled="true" />
        <h4><%= HTMLHelper.HTMLEncode(GetString("sf.mapping.replicationattributes"))%></h4>
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" AssociatedControlID="ExternalAttributeDropDownListEx" ResourceString="sf.mapping.externalidentifierattribute" runat="server" DisplayColon="True"/>
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSDropDownList ID="ExternalAttributeDropDownListEx" runat="server" EnableViewState="false" />
                </div>
            </div>
        </div>
        <h4><%= HTMLHelper.HTMLEncode(GetString("sf.mapping.requiredattributes"))%></h4>
        <div class="form-horizontal">
            <asp:Repeater ID="RequiredAttributeRepeater" runat="server" EnableViewState="false">
                <ItemTemplate>
                    <cms:MappingEditorItem ID="MappingEditorItemControl" runat="server" EnableViewState="false" />
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <h4><%= HTMLHelper.HTMLEncode(GetString("sf.mapping.otherattributes"))%></h4>
        <div class="form-horizontal">
            <asp:Repeater ID="OtherAttributeRepeater" runat="server" EnableViewState="false">
                <ItemTemplate>
                    <cms:MappingEditorItem ID="MappingEditorItemControl" runat="server" EnableViewState="false" />
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </asp:Panel>
    <script type="text/javascript">

        $cmsj(document).ready(function () {
            var mappingField = document.getElementById('<%= MappingHiddenField.ClientID %>');
            var sourceMappingField = wopener.document.getElementById('<%= SourceMappingHiddenFieldClientId %>');
            if (mappingField != null && sourceMappingField != null && mappingField.value != null && mappingField.value != '') {
                $cmsj(sourceMappingField).val(mappingField.value);
                var panelElement = document.getElementById('<%= MappingPanel.ClientID %>');
                var sourcePanelElement = wopener.document.getElementById('<%= SourceMappingPanelClientId %>');
                if (panelElement != null && sourcePanelElement != null) {
                    var content = $cmsj(panelElement).html();
                    $cmsj(sourcePanelElement).html(content);
                }
                CloseDialog();
            }
            else {
                var fieldLabel = '<%= HTMLHelper.HTMLEncode(GetString("sf.sourcetype.field"))%>';
                var metaFieldLabel = '<%= HTMLHelper.HTMLEncode(GetString("sf.sourcetype.metafield"))%>';
                var picklistEntryLabel = '<%= HTMLHelper.HTMLEncode(GetString("sf.sourcetype.picklistentry"))%>';
                $cmsj("select.SourceDropDownList").each(function (index, select) {
                    $cmsj(select).find('option[value^="Field"]').wrapAll("<optgroup label='" + fieldLabel + "'>");
                    $cmsj(select).find('option[value^="MetaField"]').wrapAll("<optgroup label='" + metaFieldLabel + "'>");
                    $cmsj(select).find('option[value^="PicklistEntry"]').wrapAll("<optgroup label='" + picklistEntryLabel + "'>");

                    var html = $cmsj(select).html();
                    $cmsj(select).html(html);
                });
            }
        });

        $cmsj(document).ready(function () {
            var displayWarnings = function (comboElement) {
                comboElement = $cmsj(comboElement);
                comboElement.parent().parent().find("i").hide().filter(".Warning" + comboElement.val()).show();
            };
            var comboElements = $cmsj("[id*='SourceDropDownList']");
            comboElements.each(function (index, comboElement) {
                displayWarnings(comboElement);
            });
            comboElements.change(function (e) {
                displayWarnings(e.target);
            });
        });

    </script>
</asp:Content>
