<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="AddRelatedDocument.ascx.cs"
    Inherits="CMSModules_Content_Controls_Relationships_AddRelatedDocument" %>

<%@ Register Src="~/CMSModules/Content/FormControls/Relationships/selectRelationshipNames.ascx"
    TagName="RelationshipNameSelector" TagPrefix="cms" %>

<asp:Panel runat="server" ID="pnlContainer">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel runat="server" CssClass="control-label" DisplayColon="True"
                    AssociatedControlID="txtLeftNode" ResourceString="Relationship.leftSideDoc" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Label ID="lblLeftNode" runat="server" CssClass="form-control-text" />
                <asp:Panel ID="pnlLeftSelectedNode" runat="server" CssClass="control-group-inline keep-white-space-fixed">
                    <cms:CMSTextBox ID="txtLeftNode" runat="server" />
                    <cms:LocalizedButton ID="btnLeftNode" runat="server" ResourceString="Relationship.SelectDocument"
                        ButtonStyle="Default" />
                </asp:Panel>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel runat="server" CssClass="control-label" DisplayColon="True" ResourceString="Relationship.RelationshipName" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="lblRelName" runat="server" CssClass="form-control-text" />
                        <cms:RelationshipNameSelector ID="relNameSelector" runat="server" ReturnColumnName="RelationshipNameID"
                            AllowedForObjects="false" HideAdHocRelationshipNames="true" />
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel runat="server" CssClass="control-label" DisplayColon="True"
                    AssociatedControlID="txtRightNode" ResourceString="Relationship.rightSideDoc" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Label ID="lblRightNode" runat="server" CssClass="form-control-text" />
                <asp:Panel ID="pnlRightSelectedNode" runat="server" CssClass="control-group-inline keep-white-space-fixed">
                    <cms:CMSTextBox ID="txtRightNode" runat="server" />
                    <cms:LocalizedButton ID="btnRightNode" runat="server" ResourceString="Relationship.SelectDocument"
                        ButtonStyle="Default" />
                </asp:Panel>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:LocalizedButton ID="btnSwitchSides" runat="server" ButtonStyle="Default"
                    OnClientClick="SwitchSides();return false;" ResourceString="Relationship.SwitchSides" />
                <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOk_Click" />
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hdnSelectedNodeId" runat="server" Value="" />
    <asp:HiddenField ID="hdnCurrentOnLeft" runat="server" Value="true" />
</asp:Panel>
