<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_Security"
    Theme="Default"  Codebehind="Security.aspx.cs" MaintainScrollPositionOnPostback="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Content/Controls/Security.ascx" TagName="Security"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" runat="server" HandleWorkflow="false" IsLiveSite="false" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlLog" Visible="false">
                <cms:AsyncLog ID="ctlAsyncLog" runat="server" ProvideLogContext="true" LogContextNames="Documents" />
            </asp:Panel>
            <asp:Panel ID="pnlPageContent" runat="server">
                <cms:UIPlaceHolder ID="pnlUIPermissionsPart" runat="server" ModuleName="CMS.Content"
                    ElementName="Security.Permissions">
                    <asp:PlaceHolder ID="plcContainer" runat="server">
                        <asp:Panel ID="pnlPermissionsPart" runat="server" CssClass="NodePermissions">
                            <cms:LocalizedHeading runat="server" ID="headPermissionsPart" Level="4" ResourceString="Security.Permissions" EnableViewState="false" />
                            <asp:Panel ID="pnlPermissionsPartBox" runat="server">
                                <asp:Label ID="lblLicenseInfo" runat="server" Visible="False" EnableViewState="false" CssClass="InfoLabel" />
                                <asp:Panel ID="pnlPermissions" runat="server">
                                    <asp:Label ID="lblInheritanceInfo" CssClass="InfoLabel" runat="server" EnableViewState="false" />
                                    <cms:LocalizedLinkButton ID="lnkInheritance" runat="server" OnClick="lnkInheritance_Click"
                                        EnableViewState="false" ResourceString="Security.Inheritance" CssClass="InfoLabel" />
                                    <cms:Security ID="securityElem" runat="server" IsLiveSite="false" />
                                </asp:Panel>
                            </asp:Panel>
                        </asp:Panel>
                    </asp:PlaceHolder>
                </cms:UIPlaceHolder>
                <asp:Panel ID="pnlAccessPart" runat="server" CssClass="NodePermissions">
                    <cms:LocalizedHeading runat="server" ID="headAccess" Level="4" ResourceString="Security.Access" EnableViewState="false" />
                    <asp:Panel ID="pnlAccessBox" CssClass="form-horizontal" runat="server">
                        <cms:UIPlaceHolder ID="pnlAuth" runat="server" ModuleName="CMS.Content" ElementName="Security.Authentication">
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel ID="lblReqAuthent" runat="server" EnableViewState="false" ResourceString="Security.RadioCaption" CssClass="control-label" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <div class="radio-list-vertical">
                                        <cms:CMSRadioButton GroupName="reqAuth" ID="radYes" runat="server" ResourceString="general.yes" />
                                        <cms:CMSRadioButton GroupName="reqAuth" ID="radNo" runat="server" ResourceString="general.no" />
                                        <asp:PlaceHolder ID="plcAuthParent" runat="server">
                                            <cms:CMSRadioButton GroupName="reqAuth" ID="radParent" runat="server" />
                                        </asp:PlaceHolder>

                                    </div>
                                </div>
                            </div>
                        </cms:UIPlaceHolder>
                    </asp:Panel>
                </asp:Panel>
                <asp:Panel ID="pnlInheritance" runat="server" Visible="False">
                    <cms:LocalizedHeading runat="server" ID="headInheritance" Level="4" ResourceString="Security.Permissions" EnableViewState="false" />
                    <asp:Panel ID="pnlCont" runat="server">
                        <asp:PlaceHolder runat="server" ID="plcRestore">
                            <div class="content-block">
                                <cms:LocalizedLinkButton ID="lnkRestoreInheritance" runat="server" OnClick="lnkRestoreInheritance_Click"
                                    EnableViewState="false" ResourceString="Security.RestoreInheritance" /><br />
                            </div>
                            <div class="content-block">
                                <cms:LocalizedLinkButton ID="lnkRestoreInheritanceRecursively" runat="server" OnClick="lnkRestoreInheritanceRecursively_Click"
                                    EnableViewState="false" ResourceString="Security.RestoreInheritanceRecursively" /><br />
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" ID="plcBreakCopy">
                            <div class="content-block">
                                <cms:LocalizedLinkButton ID="lnkBreakWithCopy" runat="server" OnClick="lnkBreakWithCopy_Click"
                                    EnableViewState="false" ResourceString="Security.BreakWithCopy" /><br />
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" ID="plcBreakClear">
                            <div class="content-block">
                                <cms:LocalizedLinkButton ID="lnkBreakWithClear" runat="server" OnClick="lnkBreakWithClear_Click"
                                    EnableViewState="false" ResourceString="Security.BreakWithClear" /><br />
                            </div>
                        </asp:PlaceHolder>
                        <div>
                            <cms:LocalizedButton ID="btnCancelAction" runat="server" ButtonStyle="Primary"
                                OnClick="btnCancelAction_Click" EnableViewState="false" ResourceString="general.cancel" />
                        </div>
                    </asp:Panel>
                </asp:Panel>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="lnkRestoreInheritanceRecursively" />
            <asp:PostBackTrigger ControlID="lnkRestoreInheritance" />
            <asp:PostBackTrigger ControlID="lnkBreakWithCopy" />
            <asp:PostBackTrigger ControlID="lnkBreakWithClear" />
            <asp:PostBackTrigger ControlID="btnCancelAction" />
        </Triggers>
    </cms:CMSUpdatePanel>
</asp:Content>
