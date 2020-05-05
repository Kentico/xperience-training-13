<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_EventManager_EventManager"  Codebehind="~/CMSWebParts/EventManager/EventManager.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Inputs/InternationalPhone.ascx" TagPrefix="cms" TagName="PhoneBox" %>
<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput"
    TagPrefix="cms" %>
<asp:Label runat="server" ID="lblInfo" CssClass="EventManagerInfo" Visible="false"
    EnableViewState="false" />
<asp:Panel runat="server" ID="pnlControl">
    <div class="EventManagerRegistration">
        <asp:Label runat="server" ID="lblRegTitle" CssClass="EventManagerRegTitle" EnableViewState="false" Visible="false" />
        <asp:Label runat="server" ID="lblRegInfo" CssClass="EventManagerRegInfo" EnableViewState="false"
            Visible="false" />
        <asp:Label runat="server" ID="lblError" CssClass="EventManagerRegError" EnableViewState="false"
            Visible="false" />
        <asp:Panel runat="server" ID="pnlReg">
            <div class="form-horizontal">
                <asp:PlaceHolder runat="server" ID="plcName">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblFirstName"
                                EnableViewState="false" ResourceString="eventmanager.firstname" AssociatedControlID="txtFirstName" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox runat="server" ID="txtFirstName" EnableViewState="false" MaxLength="100" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblLastName"
                                EnableViewState="false" ResourceString="eventmanager.lastname" AssociatedControlID="txtLastName" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox runat="server" ID="txtLastName" EnableViewState="false" MaxLength="100" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEmail"
                            EnableViewState="false" ResourceString="general.email" AssociatedControlID="txtEmail" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:EmailInput runat="server" ID="txtEmail" EnableViewState="false"/>
                    </div>
                </div>
                <asp:PlaceHolder runat="server" ID="plcPhone">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblPhone"
                                EnableViewState="false" AssociatedControlID="txtPhone" ResourceString="eventmanager.phone" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:PhoneBox runat="server" ID="txtPhone" EnableViewState="false"/>
                        </div>
                    </div>
                </asp:PlaceHolder>
                <div class="form-group form-group-submit">
                    <cms:LocalizedButton runat="server" ID="btnRegister" ButtonStyle="Primary"
                        OnClick="btnRegister_Click" EnableViewState="false" ResourceString="eventmanager.buttonregister" />
                </div>
            </div>
        </asp:Panel>
        <asp:HyperLink runat="server" ID="lnkOutlook" CssClass="EventManagerOutlookLink"
            EnableViewState="false" Visible="false" />
    </div>
</asp:Panel>
