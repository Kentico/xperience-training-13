<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="BizFormEditData.ascx.cs" Inherits="CMSModules_BizForms_Controls_BizFormEditData" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<cms:unigrid runat="server" id="gridData" islivesite="false" >
    <GridActions>
       <ug:action name="edit" externalsourcename="edit" caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
       <ug:action name="delete" caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" confirmation="$general.confirmdelete$"/>
    </GridActions>
    <GridColumns>
    </GridColumns>
    <GridOptions DisplayFilter="true" />
</cms:unigrid>