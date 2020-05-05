<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Controls_NewSiteType"  Codebehind="NewSiteType.ascx.cs" %>

<div class="form-horizontal">
    <div class="radio-list-vertical">
        <cms:CMSRadioButton runat="server" ID="radBlank" Checked="true" GroupName="Type" />
        <cms:CMSRadioButton runat="server" ID="radTemplate" GroupName="Type" />
    </div>
</div>