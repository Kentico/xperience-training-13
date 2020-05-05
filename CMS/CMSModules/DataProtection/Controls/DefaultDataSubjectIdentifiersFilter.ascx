<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultDataSubjectIdentifiersFilter.ascx.cs" Inherits="CMSModules_DataProtection_Controls_DefaultDataSubjectIdentifiersFilter" %>

<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput" TagPrefix="cms" %>

<div class="form-group">
    <div class="filter-form-label-cell">
        <cms:LocalizedLabel ID="lblEmail" runat="server" ResourceString="dataprotection.app.email" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
    </div>
    <div class="filter-form-value-cell-wide">
        <div class="cms-input-group">
            <cms:EmailInput ID="txtEmail" runat="server" EnableViewState="false" />
        </div>
    </div>
</div>