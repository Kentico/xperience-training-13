<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/EventManager/Controls/EventAttendees.ascx.cs"
    Inherits="CMSModules_EventManager_Controls_EventAttendees" %>
<%@ Register Src="~/CMSModules/EventManager/Controls/EventAttendees_Edit.ascx" TagName="AttendeesEdit"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/EventManager/Controls/EventAttendees_List.ascx" TagName="AttendeesList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/BreadCrumbs.ascx" TagName="Breadcrumbs" TagPrefix="cms" %>

<asp:Literal ID="ltlScript" runat="server" />
<asp:Panel runat="server" ID="pnlList">
    <br/>
    <cms:HeaderActions ID="actionsElem" runat="server" />
    <div class="ClearBoth"></div>
    <br />
    <cms:AttendeesList ID="attendeesList" runat="server" />
</asp:Panel>
<asp:Panel ID="pnlEdit" runat="server">
    <div>
        <asp:Panel ID="pnlHeader" runat="server">
            <cms:Breadcrumbs ID="ucBreadcrumbs" runat="server" HideBreadcrumbs="false" EnableViewState="false" PropagateToMainNavigation="false" />
        </asp:Panel>
        <asp:HiddenField ID="hdnState" runat="server" />
        <cms:AttendeesEdit runat="server" ID="attendeeEdit" RedirectAfterInsert="false" />
        <asp:LinkButton ID="lnkBackHidden" runat="server" CausesValidation="false" EnableViewState="false" />
    </div>
</asp:Panel>
