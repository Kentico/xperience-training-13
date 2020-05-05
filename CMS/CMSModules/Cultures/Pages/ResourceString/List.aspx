<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Cultures_Pages_ResourceString_List" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="Default"  Codebehind="List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>

<asp:Content ID="cultureContent" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblCultureSelector" runat="server" ResourceString="culture.general" CssClass="control-label" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:UniSelector ID="cultureSelector" ObjectType="cms.culture" SelectionMode="SingleDropDownList"
                    DisplayNameFormat="{%CultureName%}" runat="server" ReturnColumnName="CultureCode"
                    AllowAll="false" AllowEmpty="false" OrderBy="CultureName ASC"
                    PriorityCSSClass="HighPriority" PriorityWhereCondition="CultureIsUICulture = 1" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:UniGrid ID="gridStrings" runat="server" OrderBy="StringKey" IsLiveSite="false" ObjectType="cms.resourcestringlist"
        Columns="StringKey, DefaultText, CultureText, StringIsCustom">
        <GridActions Parameters="StringKey">
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="deleteall" Caption="$culture.deleteresourcestring$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$culture.deleteresourcestringconfirm$" />
            <ug:Action Name="delete" Caption="$culture.deletetranslation$" FontIconClass="icon-broom" Confirmation="$culture.deletetranslationconfirm$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="StringKey" Caption="$culture.key$" Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="CultureText" ExternalSourceName="culturetext" Caption="$culture.translated$" CssClass="main-column-50" Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="DefaultText" ExternalSourceName="defaulttext" Localize="true" IsText="true" CssClass="main-column-50" Caption="general.default" Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="StringIsCustom" ExternalSourceName="#yesno" Caption="$culture.resourcestringiscustom$" Wrap="false">
                <Filter Type="bool" />
            </ug:Column>
        </GridColumns>
        <GridOptions DisplayFilter="true" FilterLimit="25" />
    </cms:UniGrid>
</asp:Content>
