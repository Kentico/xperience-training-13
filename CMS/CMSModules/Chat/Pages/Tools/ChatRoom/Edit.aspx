<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Chat room properties – General"
    Inherits="CMSModules_Chat_Pages_Tools_ChatRoom_Edit" Theme="Default"  Codebehind="Edit.aspx.cs" %>            
<%@ Register Src="~/CMSModules/Chat/Controls/UI/ChatRoom/Edit.ascx"
    TagName="ChatRoomEdit" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ChatRoomEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>