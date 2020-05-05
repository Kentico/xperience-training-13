<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="List.ascx.cs" Inherits="CMSModules_Scoring_Controls_UI_Score_List" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<script type="text/javascript">
    function RefreshPage() {
        __doPostBack('', '');
    }
</script>
<cms:UniGrid runat="server" ID="gridElem" ObjectType="om.score" OrderBy="ScoreDisplayName" Columns="*" IsLiveSite="false">
    <GridActions>
        <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
        <ug:Action Name="#delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$"
            ModuleName="CMS.Scoring" Permissions="Modify" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="ScoreDisplayName" Localize="true" Caption="$general.displayname$"
            Wrap="false">
            <Filter Type="text" Size="200" />
        </ug:Column>
        <ug:Column Source="##ALL##" Caption="$general.status$" ExternalSourceName="scorestatus" Wrap="false" />
        <ug:Column CssClass="filling-column" />
    </GridColumns>
    <GridOptions DisplayFilter="true" />
</cms:UniGrid>