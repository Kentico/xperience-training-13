<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormerUrlsFilter.ascx.cs" Inherits="CMSModules_Content_Controls_Filters_FormerUrlsFilter" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TimeSimpleFilter.ascx" TagName="TimeSimpleFilter"
    TagPrefix="cms" %>

<asp:Panel ID="pnlFilter" runat="server" DefaultButton="btnSearch">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblFormerUrlPath" runat="server" ResourceString="formerurls.formerurlpath"
                    DisplayColon="true" EnableViewState="false" />
            </div>
            <cms:TextSimpleFilter ID="fltFormerUrlPath" runat="server" Column="PageFormerUrlPathUrlPath"/>
        </div>        
        <asp:PlaceHolder ID="plcAdvancedSearch" runat="server" Visible="false">            
                <div class="form-group">
                    <div class="filter-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblPageName" runat="server" ResourceString="formerurls.pagename" DisplayColon="true"
                            EnableViewState="false" />
                    </div>
                    <cms:TextSimpleFilter ID="fltPageName" runat="server" Column="DocumentName"/>
                </div>
                <div class="form-group">
                    <div class="filter-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblType" runat="server" ResourceString="general.type"
                            DisplayColon="true" EnableViewState="false" />
                    </div>
                    <cms:TextSimpleFilter ID="fltType" runat="server"  Column="ClassDisplayName" />
                </div>
                <div class="form-group">
                    <div class="filter-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblModified" runat="server" ResourceString="formerurls.modified"
                            DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="filter-form-value-cell-wide">
                        <cms:TimeSimpleFilter ID="fltModified" runat="server" Column="PageFormerUrlPathLastModified" />
                    </div>
                </div>
        </asp:PlaceHolder>
        <div class="form-group form-group-buttons">
            <div class="filter-form-label-cell">
                <asp:Panel ID="pnlToggleFilter" runat="server" Visible="true">
                    <asp:LinkButton ID="lnkToggleFilter" runat="server" CssClass="simple-advanced-link" />
                </asp:Panel>
            </div>
            <div class="filter-form-buttons-cell-wide-with-link">
                <cms:LocalizedButton ID="btnReset" runat="server" ButtonStyle="Default" ResourceString="general.reset" OnClick="btnReset_Click" EnableViewState="false" />
                <cms:LocalizedButton ID="btnSearch" runat="server" ButtonStyle="Primary" ResourceString="general.filter" OnClick="btnSearch_Click" EnableViewState="false" />
            </div>
        </div>
    </div>
</asp:Panel>