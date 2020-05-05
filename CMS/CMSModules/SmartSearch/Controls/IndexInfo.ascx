<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IndexInfo.ascx.cs" Inherits="CMSModules_SmartSearch_Controls_IndexInfo" %>

<asp:Panel ID="pnlInfo" runat="server" CssClass="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel runat="server" EnableViewState="false" ResourceString="srch.general.numberofitems" DisplayColon="true" AssociatedControlID="lblItemCount" CssClass="control-label editing-form-label" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizedLabel runat="server" ID="lblItemCount" EnableViewState="false" CssClass="form-control-text" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel runat="server" EnableViewState="false" ResourceString="srch.general.filesize" DisplayColon="true" AssociatedControlID="lblFileSize" CssClass="control-label editing-form-label" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizedLabel runat="server" ID="lblFileSize" EnableViewState="false" CssClass="form-control-text" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel runat="server" EnableViewState="false" ResourceString="srch.index.indexstatus" DisplayColon="true" AssociatedControlID="lblStatus" CssClass="control-label editing-form-label" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizedLabel runat="server" ID="lblStatus" EnableViewState="false" CssClass="form-control-text" />
            <asp:Literal runat="server" ID="ltlStatus" EnableViewState="False" Visible="False" />
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="plcOptimized">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel runat="server" EnableViewState="false" ResourceString="srch.index.isoptimized" DisplayColon="true" AssociatedControlID="lblIsOptimized" CssClass="control-label editing-form-label" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizedLabel runat="server" ID="lblIsOptimized" EnableViewState="false" CssClass="form-control-text" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel runat="server" EnableViewState="false" ResourceString="srch.index.lastupdate" DisplayColon="true" AssociatedControlID="lblLastUpdate" CssClass="control-label editing-form-label" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizedLabel runat="server" ID="lblLastUpdate" EnableViewState="false" CssClass="form-control-text" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel runat="server" EnableViewState="false" ResourceString="srch.index.lastrebuild" DisplayColon="true" AssociatedControlID="lblLastRebuild" CssClass="control-label editing-form-label" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizedLabel runat="server" ID="lblLastRebuild" EnableViewState="false" CssClass="form-control-text" />
        </div>
    </div>
</asp:Panel>
