<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_FormControls_GroupPictureEdit"
     Codebehind="GroupPictureEdit.ascx.cs" %>
<%@ Reference Control="~/CMSAdminControls/UI/UserPicture.ascx" %>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
<div class="control-group-inline">
    <div id="<%=divId%>" style="display: none">
    </div>
    <asp:PlaceHolder runat="server" ID="plcImageActions" Visible="false">
        <div id="<%=plcImageActions.ClientID%>">
            <asp:PlaceHolder runat="server" ID="plcGroupPicture"></asp:PlaceHolder>&nbsp;
            <asp:ImageButton runat="server" ID="btnDeleteImage" EnableViewState="false" />
        </div>
    </asp:PlaceHolder>
    <div class="uploader">
        <cms:CMSFileUpload runat="server" CssClass="uploader-input-file" ID="uplFilePicture" />
    </div>
</div>
<div class="control-group-inline">
    <cms:CMSButton runat="server" ID="btnShowGallery" EnableViewState="false" ButtonStyle="Default" />
    <asp:HiddenField runat="Server" ID="hiddenAvatarGuid" />
    <asp:HiddenField runat="Server" ID="hiddenDeleteAvatar" />
</div>
