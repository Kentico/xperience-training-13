<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/Dashboard/documents.ascx.cs"
    Inherits="CMSWebParts_DashBoard_Documents" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Documents/Documents.ascx" TagName="Documents"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/RecycleBin/Controls/RecycleBin.ascx" TagName="RecycleBin"
    TagPrefix="cms" %>
<cms:Documents runat="server" ID="ucDocuments" Visible="false" StopProcessing="true" />
<cms:RecycleBin runat="server" ID="ucRecycle" Visible="false" StopProcessing="true" />
