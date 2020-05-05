<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Attachments_DirectFileUploader_DirectFileUploader"
     Codebehind="DirectFileUploader.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/MultiFileUploader/MultiFileUploader.ascx"
    TagPrefix="cms" TagName="MultiFileUploader" %>
<div id="containerDiv" runat="server" class="direct-file-uploader">
    <asp:Panel ID="pnlInnerDiv" runat="server" CssClass="InnerDiv" EnableViewState="false">
        <i ID="uploadIcon" class="icon-arrow-up-line cms-icon-80 uploader-icon" Visible="false" aria-hidden="true" runat="server"></i>
        <cms:LocalizedButton ID="btnUpload" ButtonStyle="Default" EnableViewState="false" runat="server"></cms:LocalizedButton>
    </asp:Panel>
    <div class="UploaderDiv" style="position: absolute; top: 0; left: 0px;">
        <cms:MultiFileUploader ID="mfuDirectUploader" runat="server">
            <AlternateContent>
                <iframe id="uploaderFrame" runat="server" frameborder="0" scrolling="no" marginheight="0"
                    marginwidth="0" enableviewstate="false" style="width: 0;
                    height: 0;" />
            </AlternateContent>
        </cms:MultiFileUploader>
    </div>
    <asp:Panel ID="pnlLoading" class="LoadingDiv" runat="server">
        <i ID="imgLoading" class="icon-spinner cms-icon-80 spinning" EnableViewState="false" aria-hidden="true" runat="server"></i><asp:Label ID="lblProgress" runat="server" EnableViewState="false" />
    </asp:Panel>
</div>
