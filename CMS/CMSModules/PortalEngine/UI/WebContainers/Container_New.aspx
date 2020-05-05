<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_WebContainers_Container_New"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" ValidateRequest="false"
    Title="New container"  Codebehind="Container_New.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <asp:Panel ID="pnlContainer" runat="server">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblContainerDisplayName" ResourceString="general.displayname"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:LocalizableTextBox ID="txtContainerDisplayName" runat="server"
                        MaxLength="200" />
                    <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtContainerDisplayName:cntrlContainer:textbox" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblContainerName" ResourceString="general.codename"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CodeName ID="txtContainerName" runat="server" MaxLength="200" />
                    <cms:CMSRequiredFieldValidator ID="rfvCodeName" runat="server" ControlToValidate="txtContainerName" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblContainerText" ResourceString="Container_Edit.ContainerText"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:ExtendedTextArea ID="txtContainerText" runat="server" EnableViewState="false"
                        EditorMode="Advanced" Language="HTMLMixed" MarkErrors="false" Width="500px" Height="200px" />
                </div>
            </div>            
            <asp:PlaceHolder runat="server" ID="plcCssLink">
                <div id="cssLink" class="form-group">
                    <div class="editing-form-value-cell editing-form-value-cell-offset">
                        <cms:LocalizedButton runat="server" ID="btnStyles" EnableViewState="false" ResourceString="general.addcss" ButtonStyle="Default"
                            OnClientClick="document.getElementById('editCss').style.display = 'block'; document.getElementById('cssLink').style.display = 'none'; return false;" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="plcCss">
                <div class="form-group" id="editCss" style="<%=(plcCssLink.Visible ? "display: none": "")%>">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel runat="server" CssClass="control-label" ID="lblContainerCSS" ResourceString="Container_Edit.ContainerCSS"
                            DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:ExtendedTextArea ID="txtContainerCSS" runat="server" EnableViewState="false"
                            EditorMode="Advanced" Language="CSS" Width="500px" Height="200px" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcAssign" runat="server" Visible="false">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblAssign" ResourceString="general.assignwithwebsite" AssociatedControlID="chkAssign"  runat="server" DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox runat="server" ID="chkAssign" Checked="true" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
        <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" ResourceString="general.ok"
                        EnableViewState="false" />
    </asp:Panel>
</asp:Content>