<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/Chat/ChatRooms.ascx.cs" Inherits="CMSWebParts_Chat_ChatRooms" %>

<asp:Panel runat="server" ID="pnlChatRooms" CssClass="ChatWebpartContainer ChatWebpartContainerRooms">

<%-- Panel with the list of rooms --%>
<div class="ChatWebpartFilterPaging">
    <asp:Panel ID="pnlChatRoomsFiltering" runat="server" Visible="false">
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <cms:CMSTextBox ID="txtChatRoomsFilter" runat="server" EnableViewState="false" />
                </td>
                <td>
                    <cms:LocalizedButton ID="btnChatRoomsFilter" runat="server" ResourceString="chat.rooms.filterbtn" ButtonStyle="Default" />
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlChatRoomsInfo" runat="server"></asp:Panel>
    </asp:Panel>
    <asp:Panel ID="pnlChatRoomsPaging" runat="server"></asp:Panel>
</div>
<asp:Panel ID="pnlChatRoomsList" runat="server"></asp:Panel>

<%-- Panel for creating new rooms --%>
<asp:Panel runat="server" ID="pnlChatRoomsCreateRoom">
    <cms:LocalizedButton runat="server" ID="btnChatRoomsCreateRoom" ButtonStyle="Primary" CssClass="ChatSubmitButton" style="display: none;" EnableViewState="false" ResourceString="chat.rooms.createroom" />
</asp:Panel>

<%-- Prompt for entering password --%>
<asp:Panel runat="server" ID="pnlChatRoomsPrompt" CssClass="ChatDialogBody ChatPassDialogBody">
        <div class="ChatDialogHeader">
            <table>
                <tr>
                    <td><asp:Image ID="imgChatRoomsPrompt" runat="server" /></td>
                    <td><cms:LocalizedLabel runat="server" ID="lblChatRoomsPromptHeader" SetWindowTitle="false" ResourceString="chat.password" /></td>
                </tr>
            </table>      
        </div>
        <div class="ChatDialogContent ChatPassDialogContent">
            <cms:LocalizedLabel runat="server" ID="lblChatRoomsPromptEnterPassword" ResourceString="chat.rooms.enterpassword" /><br />
            <cms:CMSTextBox runat="server" ID="txtChatRoomsPromptInput" TextMode="Password" MaxLength="100" AutoPostBack="false" /><br />
            <asp:Panel runat="server" ID="pnlChatRoomsPromptPasswordError" CssClass="ChatError"></asp:Panel>
        </div>
        <div class="ChatDialogFooter">
            <div class="ChatDialogFooterButtons">
                <cms:LocalizedButton runat="server" ID="btnChatRoomsPromptSubmit" UseSubmitBehavior="true"
                    ButtonStyle="Primary" ResourceString="chat.ok" />
                <cms:LocalizedButton runat="server" ID="btnChatRoomsPromptClose" UseSubmitBehavior="true"
                    ButtonStyle="Primary" ResourceString="general.close" />
            </div>
        </div>
</asp:Panel>

<%-- Prompt for creating new chat room --%>
<asp:Panel runat="server" ID="pnlChatRoomsCreatePrompt" CssClass="ChatDialogBody ChatDialogBodyCreateRoom">
    <div class="ChatDialogHeader">
        <table>
            <tr>
                <td><asp:Image ID="imgChatRoomsCreatePrompt" runat="server" /></td>
                <td><cms:LocalizedLabel runat="server" ID="lblChatRoomsCreateHeader" SetWindowTitle="false" ResourceString="chat.rooms.createroom" /></td>
            </tr>
        </table>      
    </div>
    
    <div class="ChatDialogContent ChatDialogContentCreateRoom">
        <asp:Panel runat="server" ID="pnlChatRoomsCreateError" CssClass="ChatDialogContentError">
            <table>
                <tr>
                    <td><asp:Image ID="imgChatRoomsCreateError" runat="server" /></td>
                    <td><cms:LocalizedLabel ID="lblChatRoomsCreateError" runat="server" ResourceString="chat.passwordsdonotmatch" CssClass="ChatError" /></td>
                </tr>
            </table> 
        </asp:Panel>
        <table>
            <tr>
                <td><cms:LocalizedLabel runat="server" ID="lblChatRoomsCreateName" EnableViewState="false" ResourceString="chat.roomname" DisplayColon="true" /></td>
                <td><cms:CMSTextBox runat="server" ID="txtChatRoomsCreateName" MaxLength="100" onkeydown = "return (event.keyCode!=13);" /></td>
            </tr>
            <tr>
                <td><cms:LocalizedLabel runat="server" ID="lblChatRoomsCreateDescription" EnableViewState="false" ResourceString="general.description" DisplayColon="true" /></td>
                <td><cms:CMSTextArea runat="server" ID="txtChatRoomsCreateDescription" Columns="20" Rows="5" CssClass="input-width-125" /></td>
            </tr>
            <tr>
                <td><cms:LocalizedLabel runat="server" ID="lblChatRoomsCreateAllowAnonym" EnableViewState="false" ResourceString="chat.allowanonym" DisplayColon="true" /></td>
                <td><cms:CMSCheckBox runat="server" ID="chkChatRoomsCreateAllowAnonym" EnableViewState="false" /></td>
            </tr>
            <tr>
                <td><cms:LocalizedLabel runat="server" ID="lblChatRoomsCreateIsPrivate" EnableViewState="false" ResourceString="chat.private" DisplayColon="true" /></td>
                <td><cms:CMSCheckBox runat="server" ID="chkChatRoomsCreateIsPrivate" EnableViewState="false"/></td>
            </tr>
            <tr>
                <td><cms:LocalizedLabel runat="server" ID="lblChatRoomsCreatePassword" EnableViewState="false" ResourceString="chat.password" DisplayColon="true" /></td>
                <td><cms:CMSTextBox runat="server" ID="txtChatRoomsCreatePassword" TextMode="Password" MaxLength="100" onkeydown="return (event.keyCode!=13);" /></td>
            </tr>
            <tr>
                <td><cms:LocalizedLabel runat="server" ID="lblChatRoomsCreatePasswordConfirm" EnableViewState="false" ResourceString="chat.passwordconfirm" DisplayColon="true" /></td>
                <td><cms:CMSTextBox runat="server" ID="txtChatRoomsCreatePasswordConfirm" TextMode="Password" MaxLength="100" onkeydown="return (event.keyCode!=13);" /></td>
            </tr>
            
        </table>
        <asp:Panel runat="server" ID="pnlChatRoomsCreateErrorConfirm" CssClass="ChatError"></asp:Panel>
    </div>
      
    <div class="ChatDialogFooter">
        <div class="ChatDialogFooterButtons">
            <cms:LocalizedButton runat="server" ID="btnChatRoomsCreatePromptSubmit" UseSubmitBehavior="true"
                ButtonStyle="Primary" ResourceString="chat.ok" />
            <cms:LocalizedButton runat="server" ID="btnChatRoomsCreatePromptClose" UseSubmitBehavior="true"
                ButtonStyle="Primary" ResourceString="general.close" />
        </div>
    </div>
</asp:Panel>

<%-- Prompt for confirming deletion --%>
<asp:Panel runat="server" ID="pnlChatRoomsDeletePrompt" CssClass="ChatDialogBody ChatDeleteDialogBody">
    <div class="ChatDialogHeader">
        <table>
            <tr>
                <td><asp:Image ID="imgChatRoomsDeletePrompt" runat="server" /></td>
                <td><cms:LocalizedLabel runat="server" ID="lblChatRoomsDeleteHeader" SetWindowTitle="false" ResourceString="chat.disableroom" /></td>
            </tr>
        </table>      
    </div>
    <div class="ChatDialogContent ChatDeleteDialogContent">
        <cms:LocalizedLabel runat="server" ID="lblChatRoomsDeletePromptConfirm" ResourceString="chat.confirmdelete" /><br />
    </div>
    <div class="ChatDialogFooter">
        <div class="ChatDialogFooterButtons">
            <cms:LocalizedButton runat="server" ID="btnChatRoomsDeletePromptSubmit" UseSubmitBehavior="true"
                ButtonStyle="Primary" ResourceString="chat.ok" />
            <cms:LocalizedButton runat="server" ID="btnChatRoomsDeletePromptClose" UseSubmitBehavior="true"
                ButtonStyle="Primary" ResourceString="general.close" />
        </div>
    </div>
</asp:Panel>

<%-- Prompt for confirming abandon room--%>
<asp:Panel runat="server" ID="pnlChatRoomsAbandonPrompt" CssClass="ChatDialogBody ChatAbandonDialogBody">
    <div class="ChatDialogHeader">
        <table>
            <tr>
                <td><asp:Image ID="imgChatRoomsAbandonPrompt" runat="server" /></td>
                <td><cms:LocalizedLabel runat="server" ID="lblChatRoomsAbandonHeader" SetWindowTitle="false" ResourceString="chat.abandonroom" /></td>
            </tr>
        </table>      
    </div>
    <div class="ChatDialogContent ChatAbandonDialogContent">
        <cms:LocalizedLabel runat="server" ID="lblChatRoomsAbandonPromptConfirm" ResourceString="chat.confirmabandonroom" /><br />
    </div>
    <div class="ChatDialogFooter">
        <div class="ChatDialogFooterButtons">
            <cms:LocalizedButton runat="server" ID="btnChatRoomsAbandonPromptSubmit" UseSubmitBehavior="true"
                ButtonStyle="Primary" ResourceString="chat.ok" />
            <cms:LocalizedButton runat="server" ID="btnChatRoomsAbandonPromptClose" UseSubmitBehavior="true"
                ButtonStyle="Primary" ResourceString="general.close" />
        </div>
    </div>
</asp:Panel>

<%-- Prompt for editing existing chat room --%>
<asp:Panel runat="server" ID="pnlChatRoomsEditPrompt" CssClass="ChatDialogBody ChatDialogBodyEditRoom">
    <div class="ChatDialogHeader">
        <table>
            <tr>
                <td><asp:Image ID="imgChatRoomsEditPrompt" runat="server" /></td>
                <td><cms:LocalizedLabel runat="server" ID="lblChatRoomsEditHeader" SetWindowTitle="false" ResourceString="chat.chatroom.edit" /></td>
            </tr>
        </table>      
    </div>
    
    <div class="ChatDialogContent ChatDialogContentEditRoom">
        <asp:Panel runat="server" ID="pnlChatRoomsEditError" CssClass="ChatDialogContentError">
            <table>
                <tr>
                    <td><asp:Image ID="imgChatRoomsEditError" runat="server" /></td>
                    <td><cms:LocalizedLabel ID="lblChatRoomsEditError" runat="server" ResourceString="chat.passwordsdonotmatch" CssClass="ChatError" /></td>
                </tr>
            </table> 
        </asp:Panel>
        <table>
            <tr>
                <td><cms:LocalizedLabel runat="server" ID="lblChatRoomsEditName" EnableViewState="false" ResourceString="chat.roomname" DisplayColon="true" /></td>
                <td><cms:CMSTextBox runat="server" ID="txtChatRoomsEditName" MaxLength="100" onkeydown="return (event.keyCode!=13);" /></td>
            </tr>
            <tr>
                <td><cms:LocalizedLabel runat="server" ID="lblChatRoomsEditDescription" EnableViewState="false" ResourceString="general.description" DisplayColon="true" /></td>
                <td><cms:CMSTextArea runat="server" ID="txtChatRoomsEditDescription" Columns="20" Rows="5" /></td>
            </tr>
            <tr>
                <td><cms:LocalizedLabel runat="server" ID="lblChatRoomsEditAllowAnonym" EnableViewState="false" ResourceString="chat.allowanonym" DisplayColon="true" /></td>
                <td><cms:CMSCheckBox runat="server" ID="chkChatRoomsEditAllowAnonym" EnableViewState="false" /></td>
            </tr>
            <tr>
                <td><cms:LocalizedLabel runat="server" ID="lblChatRoomsEditIsPrivate" EnableViewState="false" ResourceString="chat.private" DisplayColon="true" /></td>
                <td><cms:CMSCheckBox runat="server" ID="chkChatRoomsEditIsPrivate" EnableViewState="false"/></td>
            </tr>
            <tr>
                <td><cms:LocalizedLabel runat="server" ID="lblChatRoomsHasPassword" EnableViewState="false" ResourceString="chat.passprotected" DisplayColon="true" /></td>
                <td><cms:CMSCheckBox runat="server" ID="chkChatRoomsHasPassword" EnableViewState="false" /></td>
            </tr>
            <tr>
                <td><cms:LocalizedLabel runat="server" ID="lblChatRoomsEditPasswordNew"  ResourceString="chat.passwordnew" DisplayColon="true" /> </td>
                <td><cms:CMSTextBox runat="server" ID="txtChatRoomsEditPassword" TextMode="Password" MaxLength="100" onkeydown="return (event.keyCode!=13);" /></td>
            </tr>
            <tr>
                <td><cms:LocalizedLabel runat="server" ID="lblChatRoomsEditPasswordConfirm" EnableViewState="false" ResourceString="chat.passwordconfirm" DisplayColon="true" /></td>
                <td><cms:CMSTextBox runat="server" ID="txtChatRoomsEditPasswordConfirm" TextMode="Password" MaxLength="100" onkeydown="return (event.keyCode!=13);" /></td>
            </tr>
            
        </table>
        <asp:Panel runat="server" ID="pnlChatRoomsEditErrorConfirm" CssClass="ChatError"></asp:Panel>
    </div>
      
    <div class="ChatDialogFooter">
        <div class="ChatDialogFooterButtons">
            <cms:LocalizedButton runat="server" ID="btnChatRoomsEditPromptSubmit" UseSubmitBehavior="true"
                ButtonStyle="Primary" ResourceString="chat.ok" />
            <cms:LocalizedButton runat="server" ID="btnChatRoomsEditPromptClose" UseSubmitBehavior="true"
                ButtonStyle="Primary" ResourceString="general.close" />
        </div>
    </div>
</asp:Panel>

</asp:Panel>