<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="RecycleBinFilter.ascx.cs"
    Inherits="CMSModules_Content_Controls_Filters_RecycleBinFilter" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/SelectUser.ascx" TagName="SelectUser"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter"
    TagPrefix="cms" %>
<asp:Panel ID="pnlContent" runat="server" DefaultButton="btnShow" CssClass="form-horizontal">
    <div class="form-horizontal form-filter">
        <asp:PlaceHolder ID="plcUsers" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblUsers" runat="server" DisplayColon="true" ResourceString="general.user" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:SelectUser ID="userSelector" runat="server" IsLiveSite="false" SelectionMode="SingleDropDownList"
                        AllowAll="true" AllowEmpty="false" ShowSiteFilter="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcNameFilter" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblName" runat="server" DisplayColon="true" ResourceString="general.documentname" />
                </div>
                <cms:TextSimpleFilter ID="nameFilter" runat="server" Column="VersionDocumentName" />
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcPathFilter" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblPath" runat="server" DisplayColon="true" ResourceString="general.path" />
                </div>
                <cms:TextSimpleFilter ID="pathFilter" runat="server" Column="[CMS_VersionHistory].[VersionNodeAliasPath]" />
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcClass" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblClass" runat="server" DisplayColon="true" ResourceString="general.documenttype" />
                </div>
                <cms:TextSimpleFilter ID="classFilter" runat="server" Column="ClassDisplayName" />
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcDateTime" runat="server" Visible="False">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblFilter" AssociatedControlID="txtFilter" runat="server"
                        EnableViewState="false" ResourceString="recyclebin.DateTimeFilter" DisplayColon="True" />
                </div>
                <div class="filter-form-condition-cell">
                    <cms:CMSTextBox ID="txtFilter" runat="server" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSDropDownList ID="drpFilter" runat="server" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell-wide">
                <cms:LocalizedButton ID="btnReset" runat="server" ButtonStyle="Default" EnableViewState="false" />
                <cms:LocalizedButton ID="btnShow" runat="server" ResourceString="general.filter" ButtonStyle="Primary"
                    OnClick="btnShow_Click" />
            </div>
        </div>
    </div>
</asp:Panel>
