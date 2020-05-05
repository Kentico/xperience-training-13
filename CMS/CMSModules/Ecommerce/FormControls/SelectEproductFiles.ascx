<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SelectEproductFiles.ascx.cs"
    Inherits="CMSModules_Ecommerce_FormControls_SelectEproductFiles" %>
<%@ Register TagPrefix="cms" TagName="File" Src="~/CMSModules/AdminControls/Controls/MetaFiles/File.ascx" %>
<%@ Register TagPrefix="cms" TagName="FileList" Src="~/CMSModules/AdminControls/Controls/MetaFiles/FileList.ascx" %>
<%-- Messages --%>
<cms:MessagesPlaceHolder ID="plcMessages" runat="server" Visible="false" />
<%-- File uploader for new e-product --%>
<asp:PlaceHolder ID="plcNewProduct" runat="server" Visible="false">
    <cms:File ID="fileElem" runat="server" ShortID="f" Enabled="false" />
</asp:PlaceHolder>
<%-- Files list and uploader for existing e-product --%>
<asp:PlaceHolder ID="plcExistingProduct" runat="server" Visible="false">
    <cms:FileList ID="fileListElem" runat="server" ShortID="fl" UploadLabelVisible="false" AllowEdit="false"
        ItemsPerPage="10" ShowPageSize="false" ShowObjectMenu="false" IsLiveSite="false" />
</asp:PlaceHolder>
