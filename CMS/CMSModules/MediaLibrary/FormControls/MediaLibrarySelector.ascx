<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MediaLibrary_FormControls_MediaLibrarySelector"  Codebehind="MediaLibrarySelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" ResourcePrefix="medialibraryselect"
            ObjectType="media.library" OrderBy="LibraryName" AllowEditTextBox="true" SelectionMode="SingleDropDownList" 
            DisplayNameFormat="{%LibraryDisplayName%}" AllRecordValue="" NoneRecordValue="" AllowEmpty="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>