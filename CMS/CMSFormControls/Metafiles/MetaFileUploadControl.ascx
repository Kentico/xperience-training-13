<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="MetaFileUploadControl.ascx.cs"
    Inherits="CMSFormControls_Metafiles_MetaFileUploadControl" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/File.ascx" TagName="MetafileUploader"
    TagPrefix="cms" %>
<cms:MetafileUploader ID="fileUploader" runat="server" />
