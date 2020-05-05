<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ObjectsRecycleBinFilter.ascx.cs"
    Inherits="CMSModules_Content_Controls_Filters_ObjectsRecycleBinFilter" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/SelectUser.ascx" TagName="SelectUser"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Objects/FormControls/BinObjectTypeSelector.ascx" TagName="ObjectTypeSelector"
    TagPrefix="cms" %>
<asp:Panel ID="pnlContent" runat="server" DefaultButton="btnShow" CssClass="form-horizontal">
    <div class="form-horizontal form-filter">
        <asp:PlaceHolder ID="plcUsers" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblUsers" runat="server" DisplayColon="true" ResourceString="general.user" EnableViewState="false" />
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
                    <cms:LocalizedLabel CssClass="control-label" ID="lblName" runat="server" DisplayColon="true" ResourceString="general.objectname" EnableViewState="false" />
                </div>
                <cms:TextSimpleFilter ID="nameFilter" runat="server" Column="VersionObjectDisplayName" />
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcObjectTypeFilter" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblObjectType" runat="server" DisplayColon="true" ResourceString="general.objecttype" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:CMSUpdatePanel ID="pnlObjType" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cms:ObjectTypeSelector ID="objTypeSelector" runat="server" />
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </div>
            </div>
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
        </asp:PlaceHolder>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell-wide">
                <cms:LocalizedButton ID="btnReset" runat="server" ButtonStyle="Default" EnableViewState="false" />
                <cms:LocalizedButton ID="btnShow" runat="server" ResourceString="general.filter" ButtonStyle="Primary"
                    OnClick="btnShow_Click" EnableViewState="false" />
            </div>
        </div>
    </div>
</asp:Panel>
