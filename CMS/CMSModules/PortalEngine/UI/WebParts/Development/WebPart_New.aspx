<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_WebParts_Development_WebPart_New"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Web parts - New"
     Codebehind="WebPart_New.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Dialogs/FileSystemSelector.ascx" TagPrefix="cms"
    TagName="FileSystemSelector" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel ID="PanelUsers" runat="server">
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
            <ContentTemplate>
                <asp:PlaceHolder ID="plcTable" runat="server">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lbWebPartNew" runat="server" EnableViewState="False"
                                    ResourceString="objecttype.cms_webpart" DisplayColon="true" ShowRequiredMark="true" AssociatedControlID="radNewWebPart" />
                            </div>
                            <div class="editing-form-value-cell">
                                <div class="radio-list-vertical">
                                    <cms:CMSRadioButton runat="server" ID="radNewWebPart" ResourceString="developmentwebparteditnewwepart"
                                        GroupName="wpSelect" Checked="true" OnCheckedChanged="radNewWebPart_CheckedChanged"
                                        AutoPostBack="true" />
                                    <cms:CMSRadioButton runat="server" ID="radInherited" GroupName="wpSelect" ResourceString="Development-WebPart_Edit.Inherited"
                                        OnCheckedChanged="radNewWebPart_CheckedChanged" AutoPostBack="true" />
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lbWebPartDisplaytName" runat="server" EnableViewState="False"
                                    ResourceString="general.displayname" DisplayColon="true" ShowRequiredMark="true" />
                            </div>
                            <div class="editing-form-value-cell">
                                <div class="control-group-inline">
                                    <cms:LocalizableTextBox ID="txtWebPartDisplayName" runat="server"
                                        MaxLength="95" />
                                    <cms:CMSRequiredFieldValidator ID="rfvWebPartDisplayName" runat="server" EnableViewState="false"
                                        ControlToValidate="txtWebPartDisplayName:cntrlContainer:textbox" Display="dynamic"></cms:CMSRequiredFieldValidator>
                                </div>
                            </div>
                        </div>

                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lbWebPartName" runat="server" EnableViewState="False" ResourceString="general.codename"
                                    DisplayColon="true" ShowRequiredMark="true" />
                            </div>
                            <div class="editing-form-value-cell">
                                <div class="control-group-inline">
                                    <cms:CodeName ID="txtWebPartName" runat="server" MaxLength="95" />
                                    <cms:CMSRequiredFieldValidator ID="rfvWebPartName" runat="server" EnableViewState="false"
                                        ControlToValidate="txtWebPartName" Display="dynamic"></cms:CMSRequiredFieldValidator>
                                </div>
                            </div>
                        </div>

                        <asp:PlaceHolder ID="plcCodeFiles" runat="server">
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblCodeFiles" runat="server" EnableViewState="False" ResourceString="webpart.codefiles"
                                        DisplayColon="true" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="radio-list-vertical">
                                        <cms:CMSRadioButton ID="radNewFile" runat="server" AutoPostBack="true" Checked="true"
                                            GroupName="codeFile" ResourceString="webpart.generatefiles" OnCheckedChanged="radNewFile_CheckedChanged" />
                                        <cms:CMSRadioButton ID="radExistingFile" runat="server" AutoPostBack="true"
                                            GroupName="codeFile" ResourceString="webpart.useexistingfile" OnCheckedChanged="radNewFile_CheckedChanged" />
                                    </div>
                                </div>
                            </div>
                            
                            <asp:PlaceHolder ID="plcFileName" runat="server">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel CssClass="control-label" ID="lblFileName" runat="server" EnableViewState="False"
                                            ResourceString="webpart.filename" DisplayColon="true" ShowRequiredMark="true" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <div class="control-group-inline">
                                            <cms:CMSTextBox ID="txtCodeFileName" runat="server" MaxLength="95"  />
                                            <cms:CMSRequiredFieldValidator ID="rfvCodeFileName" runat="server" EnableViewState="false"
                                                ControlToValidate="txtCodeFileName" Display="dynamic"></cms:CMSRequiredFieldValidator>
                                            <cms:LocalizedLabel ID="lblFileNameExplanation" runat="server" ResourceString="webpart.filenameexplanation" CssClass="explanation-text" />
                                        </div>
                                    </div>
                                </div>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="plcSelectFile" runat="server" Visible="false">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel CssClass="control-label" ID="lblWebPartFileName" runat="server" ResourceString="general.filepath"
                                            DisplayColon="true" ShowRequiredMark="true" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:FileSystemSelector ID="FileSystemSelector" runat="server" />
                                    </div>
                                </div>
                            </asp:PlaceHolder>

                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="plcWebparts" runat="server" Visible="false">
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <asp:Label CssClass="control-label" ID="lblWebpartList" runat="server" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:SelectWebpart ID="webpartSelector" runat="server" ShowWebparts="true" />
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <div class="form-group">
                            <div class="editing-form-value-cell editing-form-value-cell-offset">
                                <cms:FormSubmitButton ID="btnOk" runat="server" EnableViewState="false"
                                    OnClick="btnOK_Click" />
                            </div>
                        </div>
                    </div>
                </asp:PlaceHolder>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</asp:Content>
