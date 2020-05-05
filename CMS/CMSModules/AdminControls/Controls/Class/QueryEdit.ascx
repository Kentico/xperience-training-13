<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_AdminControls_Controls_Class_QueryEdit"
     Codebehind="QueryEdit.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Filters/DocTypeFilter.ascx" TagName="DocTypeFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/SelectConnectionString.ascx" TagName="SelectString"
    TagPrefix="cms" %>

<script type="text/javascript">
    function ToggleHelp() {
        var helpTable = document.getElementById('<%=tblHelp.ClientID%>');
        helpTable.style.display = (helpTable.style.display === 'none') ? 'table' : 'none';
    }
</script>

<cms:ObjectCustomizationPanel runat="server" ID="pnlCustomization">
    <asp:PlaceHolder runat="server" ID="plcDocTypeFilter">
        <cms:DocTypeFilter runat="server" ID="filter" RenderTableTag="true" EnableViewState="true" />
    </asp:PlaceHolder>
    <asp:Panel ID="pnlContainer" runat="server">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblQueryName" EnableViewState="false" ResourceString="queryedit.queryname"
                        DisplayColon="true" AssociatedControlID="txtQueryName" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox runat="server" ID="txtQueryName" MaxLength="100" CssClass="form-control"
                        EnableViewState="true" />
                    <cms:CMSRequiredFieldValidator ID="RequiredFieldValidatorQueryName" runat="server"
                        EnableViewState="false" ControlToValidate="txtQueryName" Display="dynamic" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblQueryType" EnableViewState="false" ResourceString="queryedit.querytype"
                        DisplayColon="true" AssociatedControlID="rblQueryType" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSRadioButtonList runat="server" ID="rblQueryType" RepeatDirection="Horizontal"
                        UseResourceStrings="true" EnableViewState="false">
                        <asp:ListItem Selected="True" Value="SQLQuery" Text="queryedit.querytypetext" />
                        <asp:ListItem Value="StoredProcedure" Text="queryedit.querytypesp" />
                    </cms:CMSRadioButtonList>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTransaction" EnableViewState="false" ResourceString="queryedit.requirestransaction"
                        DisplayColon="true" AssociatedControlID="chbTransaction" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox runat="server" ID="chbTransaction" EnableViewState="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblQueryText" EnableViewState="false" ResourceString="queryedit.querytextvalue"
                        DisplayColon="true" AssociatedControlID="txtQueryText" />
                </div>
                <div class="editing-form-value-cell form-field-full-column-width">
                    <cms:LocalizedButton ID="btnGenerate" runat="server" ButtonStyle="Default" OnClick="btnGenerate_Click"
                        ResourceString="queryedit.generate" EnableViewState="false" Visible="false" />
                    <cms:ExtendedTextArea runat="server" ID="txtQueryText" EnableViewState="false" EditorMode="Advanced"
                        Language="SQL" Height="280px" Width="100%" />
                </div>
            </div>
            <asp:PlaceHolder runat="server" ID="plcConnectionString">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblConnString" EnableViewState="false" ResourceString="ConnectionString.Title"
                            DisplayColon="true" AssociatedControlID="ucSelectString" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:SelectString runat="server" ID="ucSelectString" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="form-group">
                <div class="editing-form-value-cell editing-form-value-cell-offset">
                    <div class="control-group-inline">
                        <cms:LocalizedLinkButton ID="lnkHelp" runat="server" OnClientClick="ToggleHelp(); return false;"
                            ResourceString="queryedit.helpheader" EnableViewState="false" />
                    </div>
                    <div class="control-group-inline-forced">
                        <asp:Table ID="tblHelp" runat="server" EnableViewState="false" CssClass="table table-hover" />
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
</cms:ObjectCustomizationPanel>