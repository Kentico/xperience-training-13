<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_BadWords_BadWords_List"
         Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Bad words - List"  Codebehind="BadWords_List.aspx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" tagname="UniGrid" tagprefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="UniGrid" 
                 IsLiveSite="false" 
                 HideFilterButton="true" 
                 ObjectType="badwords.word" 
                 Columns="WordID,WordExpression,WordReplacement,WordAction,WordIsGlobal"
                 OrderBy="WordExpression ASC"
        >
        <GridActions Parameters="WordID">
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridOptions DisplayFilter="true" FilterPath="~/CMSModules/BadWords/Controls/BadWordsFilter.ascx" />
        <GridColumns>
            <ug:Column source="WordExpression" caption="$Unigrid.BadWords.Columns.WordExpression$" wrap="false" CssClass="main-column-100" maxlength="100" >
                <tooltip source="WordExpression" width="0" />
            </ug:Column>
            <ug:Column source="WordAction" externalsourcename="WordAction" caption="$Unigrid.BadWords.Columns.WordAction$" wrap="false" />
            <ug:Column source="##ALL##" externalsourcename="WordReplacement" caption="$Unigrid.BadWords.Columns.WordReplacement$" sort="WordReplacement" wrap="false" />
            <ug:Column source="WordIsGlobal" externalsourcename="Global" caption="$Unigrid.BadWords.Columns.IsGlobal$" wrap="false" />
            <ug:Column source="WordID" visible="false" />
        </GridColumns>
    </cms:UniGrid>
</asp:Content>