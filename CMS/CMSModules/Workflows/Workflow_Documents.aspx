<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Workflows_Workflow_Documents" Title="Workflow - Pages"
    ValidateRequest="false" Theme="Default"  Codebehind="Workflow_Documents.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/Filters/DocumentFilter.ascx" TagName="DocumentFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Documents/Documents.ascx" TagName="Documents"
    TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:AsyncLog ID="ctlAsyncLog" runat="server" ProvideLogContext="true" LogContextNames="Documents" />
    </asp:Panel>
    <asp:Panel ID="pnlContent" runat="server">
        <asp:PlaceHolder ID="plcFilter" runat="server">
            <cms:DocumentFilter ID="filterDocuments" runat="server" LoadSites="true" AllowSiteAutopostback="false" IncludeSiteCondition="True"/>
        </asp:PlaceHolder>
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <cms:Documents ID="docElem" runat="server" ListingType="WorkflowDocuments" IsLiveSite="false" />
                <asp:Panel ID="pnlFooter" runat="server" CssClass="form-horizontal mass-action">
                    <div class="form-group">
                        <div class="mass-action-value-cell">
                            <cms:CMSDropDownList ID="drpWhat" runat="server" />
                            <cms:CMSDropDownList ID="drpAction" runat="server" />
                            <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" ResourceString="general.ok"
                                OnClick="btnOk_OnClick" />
                        </div>
                    </div>
                    <asp:Label ID="lblValidation" runat="server" CssClass="InfoLabel" EnableViewState="false" />
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btnOk" />
            </Triggers>
        </cms:CMSUpdatePanel>
    </asp:Panel>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>