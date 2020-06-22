<%@ Control Language="C#" AutoEventWireup="false" Codebehind="SelectScore.ascx.cs" Inherits="CMSModules_Scoring_FormControls_SelectScore" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>

<cms:UniSelector ID="uniSelector" runat="server" SelectionMode="SingleDropDownList"
    AllowAll="true" AllowEmpty="false" ResourcePrefix="scoreselect" ObjectType="om.score" OrderBy="ScoreName" />