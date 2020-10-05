<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_Tags"
    Theme="Default" CodeBehind="Tags.aspx.cs" MaintainScrollPositionOnPostback="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Content/Controls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" runat="server" HandleWorkflow="false" IsLiveSite="false" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlContent" runat="server">
        <cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
            <ContentTemplate>
                <cms:CMSForm runat="server" ID="editForm" AlternativeFormFullName="cms.document.tags" DefaultFieldLayout="Inline" UseColonBehindLabel="False" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
    <div class="Clear">
    </div>
</asp:Content>
