<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="General.ascx.cs" Inherits="CMSModules_CssStyleSheets_Controls_General" %>
<%@ Register Src="~/CMSModules/Objects/Controls/Locking/ObjectEditPanel.ascx" TagName="ObjectEditPanel"
    TagPrefix="cms" %>

<cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:ObjectEditPanel runat="server" ID="editMenuElem" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
<asp:Panel ID="pnlContainer" runat="server" CssClass="PreviewBody">
    <div class="PageContent">
        <cms:MessagesPlaceHolder runat="server" ID="pnlMessagePlaceholder" IsLiveSite="false" />
        <cms:UIForm runat="server" ID="EditForm" ObjectType="cms.cssstylesheet" OnOnBeforeDataLoad="EditForm_OnBeforeDataLoad"
            OnOnAfterDataLoad="EditForm_OnAfterDataLoad" OnOnBeforeSave="EditForm_OnBeforeSave" OnOnAfterValidate="EditForm_OnAfterValidate" OnPreRender="EditForm_PreRender" RefreshHeader="True" />
        <asp:HiddenField runat="server" ID="hidCompiledCss" EnableViewState="false" />
    </div>
</asp:Panel>
