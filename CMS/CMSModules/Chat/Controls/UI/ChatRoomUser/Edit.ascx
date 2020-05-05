<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Edit.ascx.cs" Inherits="CMSModules_Chat_Controls_UI_ChatRoomUser_Edit" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Users/selectuser.ascx" TagName="UserSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/DropDownListControl.ascx" TagName="DropDownList" TagPrefix="cms" %>

<cms:UIForm runat="server" ID="EditForm" ObjectType="chat.roomuser" DefaultFieldLayout="TwoColumns">
    <LayoutTemplate>
        <cms:FormField runat="server" ID="fUserID" Field="ChatRoomUserChatUserID" ResourceString="general.user" DisplayColon="true" UseFFI="false">
            <cms:UserSelector runat="server" ID="fUserSelector" IsLiveSite="false" AllowAll="false" AllowDefault="false" AllowEmpty="false" />
            <div class="explanation-text"><asp:Literal runat="server" ID="litChatUserLink"></asp:Literal></div>
        </cms:FormField>
        <cms:FormField runat="server" ID="fAdminLevel" Field="ChatRoomUserAdminLevel" ResourceString="chat.adminlevel" DisplayColon="true" UseFFI="false">
            <cms:DropDownList ID="fdrpAdminLevel" runat="server" CssClass="DropDownField" />
        </cms:FormField>
        <cms:FormSubmit runat="server" ID="fSubmit" />
    </LayoutTemplate>
</cms:UIForm>