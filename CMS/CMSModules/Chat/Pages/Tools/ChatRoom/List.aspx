<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Chat room list"
    Inherits="CMSModules_Chat_Pages_Tools_ChatRoom_List" Theme="Default"  Codebehind="List.aspx.cs" %>

<%@ Register Src="~/CMSModules/Chat/Controls/UI/ChatRoom/List.ascx" TagName="ChatRoomList" TagPrefix="cms" %>

<asp:Content ID="cntSiteSelector" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel runat="server" ID="lblSite" EnableViewState="false" DisplayColon="true"
                    ResourceString="General.Site" CssClass="control-label" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:SiteOrGlobalSelector ID="siteOrGlobalSelector" ShortID="c" runat="server" PostbackOnDropDownChange="True" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ChatRoomList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
