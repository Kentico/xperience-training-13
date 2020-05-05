<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="ObjectVersions.aspx.cs" Inherits="CMSModules_AdminControls_Pages_ObjectVersions"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Objects/Controls/Versioning/ObjectVersionList.ascx"
    TagPrefix="cms" TagName="VersionList" %>
<asp:Content runat="server" ContentPlaceHolderID="plcContent" ID="cntContent">
    <cms:VersionList ID="versionList" runat="server" />
</asp:Content>
