<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_AdminControls_Controls_Class_TransformationEdit"
    CodeBehind="TransformationEdit.ascx.cs" %>

<%@ Register Src="~/CMSModules/Objects/Controls/Locking/ObjectEditPanel.ascx" TagName="ObjectEditPanel"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Layouts/TransformationCode.ascx" TagName="TransformationCode"
    TagPrefix="cms" %>

<asp:Panel ID="pnlContent" runat="server">
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
        <ContentTemplate>
            <cms:ObjectEditPanel runat="server" ID="editMenuElem" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <asp:Panel ID="pnlScreen" runat="server" CssClass="PreviewBody">
        <div class="PageContent">
            <cms:MessagesPlaceHolder runat="server" ID="pnlMessagePlaceholder" IsLiveSite="false"/>
            <cms:UIForm FieldGroupHeadingIsAnchor="False" runat="server" ID="editElem" ObjectType="cms.transformation" OnOnAfterDataLoad="editElem_OnAfterDataLoad"
                RedirectUrlAfterCreate="" RefreshHeader="true" DefaultFieldLayout="TwoColumns">
                <LayoutTemplate>
                    <cms:FormField runat="server" ID="fDisplayName" Field="TransformationName" FormControl="TextBoxControl"
                        ResourceString="transformationlist.transformationname" DisplayColon="true" />
                    <cms:FormField runat="server" ID="fCode" Field="TransformationCode" DisplayLabel="false" UseFFI="false" Layout="Inline">
                        <cms:TransformationCode runat="server" ID="ucTransfCode" />
                    </cms:FormField>
                </LayoutTemplate>
            </cms:UIForm>
        </div>
    </asp:Panel>
</asp:Panel>
<asp:HiddenField runat="server" ID="hdnClose" EnableViewState="false" />
