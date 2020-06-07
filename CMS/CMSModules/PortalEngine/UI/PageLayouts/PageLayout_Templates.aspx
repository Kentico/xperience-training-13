<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="PageLayout_Templates.aspx.cs"
    Inherits="CMSModules_PortalEngine_UI_PageLayouts_PageLayout_Templates" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Page Layouts - Templates" %>

<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <script type="text/javascript" language="javascript">
    //<![CDATA[
        function RefreshWOpener(w) {
            window.location = window.location.href;
        }
    //]]>
    </script>
    <cms:CMSPanel ID="pnlContentWrap" runat="server" ShortID="p">
        <cms:UniGrid ID="gridTemplates" runat="server" OrderBy="PageTemplateDisplayName" Columns="PageTemplateID, PageTemplateDisplayName"
            IsLiveSite="false" ObjectType="cms.pagetemplate" ShowObjectMenu="false">
            <GridActions Parameters="PageTemplateID">
                <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" OnClick="EditPageTemplate({0}); return false;" />
            </GridActions>
            <GridColumns>
                <ug:Column Source="PageTemplateDisplayName" Caption="$general.displayname$" Wrap="false" Localize="true">
                    <Filter Type="text" />
                </ug:Column>
                <ug:Column CssClass="filling-column" />
            </GridColumns>
            <GridOptions DisplayFilter="true" />
        </cms:UniGrid>
    </cms:CMSPanel>
</asp:Content>
