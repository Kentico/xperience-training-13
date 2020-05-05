<%@ Page Language="C#" Theme="Default" AutoEventWireup="true"
    Inherits="CMSFormControls_LiveSelectors_InsertImageOrMedia_Footer" EnableEventValidation="false"
    MasterPageFile="~/CMSMasterPages/LiveSite/EmptyPage.master"  Codebehind="Footer.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/General/DialogFooter.ascx"
    TagName="Footer" TagPrefix="cms" %>
<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="server">
    <div class="LiveSiteDialog">
        <div class="PageFooterLine">
            <cms:Footer ID="footerElem" runat="server" IsLiveSite="true" />
            <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
        </div>
    </div>
</asp:Content>
