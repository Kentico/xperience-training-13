<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_OnlineMarketing_Controls_UI_ABVariant_List"
     Codebehind="List.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<script type="text/javascript">
    //<![CDATA[
    function EditVariant(id) {
        var red = 'Edit.aspx?abTestID=<%=ABTestID%>&variantId=' + id;
        var node = '<%=nodeID%>';
        if (node != '0') {
            red += '&nodeid=' + node;
        }
        window.location.replace(red);
    }
    //]]>
</script>

<cms:UniGrid ID="gridElem" runat="server" OrderBy="ABVariantDisplayName" ObjectType="om.abvariant"
    OnOnExternalDataBound="gridElem_OnExternalDataBound" ShowObjectMenu="false" OnOnAction="gridElem_OnAction">
    <GridActions>
        <ug:Action Name="edit" Caption="$general.edit$" OnClick="if (window.EditVariant) {window.EditVariant({0}); return false;}" FontIconClass="icon-edit" FontIconStyle="Allow" />
        <ug:Action Name="delete" Caption="$general.delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$general.confirmdelete$" ModuleName="CMS.ABTest" Permissions="manage" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="ABVariantDisplayName" Caption="$abtesting.variantname$" Wrap="false">
            <Filter Type="text" Size="200" />
        </ug:Column>
        <ug:Column Source="ABVariantPath" Caption="$general.path$" Wrap="false" ExternalSourceName="path" />
        <ug:Column CssClass="filling-column" />
    </GridColumns>
    <GridOptions DisplayFilter="true" />
</cms:UniGrid>
