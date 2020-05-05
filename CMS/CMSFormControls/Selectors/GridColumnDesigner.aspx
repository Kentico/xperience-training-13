<%@ Page Language="C#" ValidateRequest="false" AutoEventWireup="true"
    Inherits="CMSFormControls_Selectors_GridColumnDesigner" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" Title="Grid column designer" CodeBehind="GridColumnDesigner.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/Selectors/ItemSelection.ascx" TagName="ItemSelection"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="hdnSelectedColumns" runat="server" />
            <asp:HiddenField ID="hdnClassNames" runat="server" />
            <asp:HiddenField ID="hdnColumns" runat="server" />
            <asp:HiddenField ID="hdnTextClassNames" runat="server" />
            <asp:Panel runat="server" ID="pnlBody">
                <table style="width: 100%;">
                    <tr>
                        <td>
                            <cms:CMSRadioButton ID="radGenerate" runat="server" Checked="True" GroupName="GenerateSelect"
                                AutoPostBack="True" ResourceString="GridColumnDesigner.Generate" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cms:CMSRadioButton ID="radSelect" runat="server" GroupName="GenerateSelect" AutoPostBack="True" ResourceString="GridColumnDesigner.Select" />
                        </td>
                    </tr>
                </table>

                <cms:ItemSelection ID="ItemSelection1" runat="server" Visible="false" />

                <asp:Panel ID="pnlProperties" runat="server" Visible="false">
                    <br />
                    <cms:LocalizedLabel ID="lblProperties" runat="server" Font-Bold="true" ResourceString="GridColumnDesigner.Properties" EnableViewState="false" />
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblHeaderText" runat="server" AssociatedControlID="txtHeaderText" ResourceString="GridColumnDesigner.Headertext" EnableViewState="false" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox ID="txtHeaderText" runat="server" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblDisplayAsLink" runat="server" AssociatedControlID="chkDisplayAsLink" ResourceString="GridColumnDesigner.DisplayAsLink" EnableViewState="false" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSCheckBox ID="chkDisplayAsLink" runat="server" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-value-cell editing-form-value-cell-offset">
                                <cms:LocalizedButton ID="btnOk" runat="server" Text="OK" ButtonStyle="Primary" OnClick="btnOK_Click" ResourceString="general.ok" EnableViewState="false" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
<asp:Content ID="plcFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnClose" runat="server" ButtonStyle="Primary"
            OnClick="btnClose_Click" ResourceString="general.saveandclose" EnableViewState="false" />
    </div>
</asp:Content>
