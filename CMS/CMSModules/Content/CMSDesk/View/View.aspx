<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="View.aspx.cs" Inherits="CMSModules_Content_CMSDesk_View_View"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Title="View / Validate" %>

<%@ Register Src="~/CMSModules/DeviceProfiles/Controls/DeviceView.ascx" TagName="DeviceView"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/EditMenu.ascx" TagPrefix="cms" TagName="EditMenu" %>


<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <div class="preview-edit-panel">
        <cms:EditMenu runat="server" ID="editMenu" ShortID="m" />
    </div>
    <div class="preview-page">
        <cms:MessagesPlaceHolder runat="server" ID="plcMess" OffsetX="16" OffsetY="16" UseRelativePlaceHolder="true">
            <cms:AlertLabel runat="server" ID="alWarning" AlertType="Warning" Text="&nbsp;" CssClass="alert-dismissable alert-warning-absolute hidden" EnableViewState="False" />
        </cms:MessagesPlaceHolder>
    </div>
    <cms:DeviceView ID="ucView" runat="server" />
</asp:Content>
