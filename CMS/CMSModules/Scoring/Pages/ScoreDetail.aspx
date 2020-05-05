<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="ScoreDetail.aspx.cs" Inherits="CMSModules_Scoring_Pages_ScoreDetail"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Scoring - Score detail" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagPrefix="cms" TagName="UniGrid" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:UniGrid runat="server" ID="gridElem" ShortID="g" IsLiveSite="false" ObjectType="om.scorecontactrulelist"
        Columns="RuleDisplayName, Value, RuleValue" OrderBy="RuleDisplayName" ShowActionsMenu="true" ShowObjectMenu="false">
        <GridColumns>
            <ug:Column Source="RuleDisplayName" Caption="$om.rule$" Localize="true" Wrap="false" CssClass="main-column-100">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="RuleValue" Caption="$om.score.scorevalue$" Wrap="false" />
            <ug:Column Source="##ALL##" ExternalSourceName="quantity" Caption="$general.quantity$" Wrap="false" />
            <ug:Column Source="Value" Caption="$om.score.totalvalue$" Wrap="false" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
