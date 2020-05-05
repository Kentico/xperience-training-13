<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_FormControls_Avatars_UserPictureEdit"
     Codebehind="UserPictureEdit.ascx.cs" %>

<%@ Reference Control="~/CMSAdminControls/UI/UserPicture.ascx" %>

<cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
        <div class="control-group-inline">
            <asp:Panel runat="server" ID="pnlPreview" Style="display: none">
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlAvatarImage" Visible="false">
                <asp:PlaceHolder runat="server" ID="plcUserPicture"></asp:PlaceHolder>
                &nbsp;<asp:ImageButton runat="server" ID="btnDeleteImage" EnableViewState="false" Visible="false" />
                <cms:CMSIcon ID="imgHelp" runat="server" Visible="false" CssClass="icon-question-circle" data-html="true"/>
                <br />
            </asp:Panel>
            <asp:PlaceHolder runat="server" ID="plcUploader" Visible="false">
                <div class="uploader">
                    <cms:CMSFileUpload runat="server" CssClass="uploader-input-file" ID="uplFilePicture" />
                </div>
            </asp:PlaceHolder>
        </div>
         <asp:HiddenField runat="Server" ID="hiddenAvatarGuid" />
         <asp:HiddenField runat="Server" ID="hiddenDeleteAvatar" />
    </ContentTemplate>
</cms:CMSUpdatePanel>