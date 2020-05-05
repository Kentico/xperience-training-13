<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="CMSModules_Content_CMSDesk_MVC_Edit" Theme="Default" %>

<%@ Register Src="~/CMSModules/Content/Controls/EditMenu.ascx" TagName="editmenu" TagPrefix="cms" %>

<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <div class="preview-edit-panel">
        <cms:editmenu runat="server" ID="editMenu" ShortID="m" />
    </div>
    <div class="page-builder">
        <div class="messages-wrapper">
            <cms:MessagesPlaceHolder runat="server" ID="plcMess" OffsetX="16" OffsetY="16" UseRelativePlaceHolder="False">
                <div>
                    <cms:AlertLabel runat="server" ID="alSuccess" AlertType="Confirmation" Text="&nbsp;" CssClass="hidden" EnableViewState="False" />
                </div>
                <div>
                    <cms:AlertLabel runat="server" ID="alWarning" AlertType="Warning" Text="&nbsp;" CssClass="alert-dismissable hidden" EnableViewState="False" />
                </div>
                <div>
                    <cms:AlertLabel runat="server" ID="alError" AlertType="Error" Text="&nbsp;" CssClass="alert-dismissable hidden" EnableViewState="False" />
                </div>
            </cms:MessagesPlaceHolder>
        </div>
        <iframe width="100%" height="100%" id="pageview" name="pageview" scrolling="auto" frameborder="0" runat="server" class="ContentFrame scroll-area"></iframe>
    </div>
</asp:Content>
