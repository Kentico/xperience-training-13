<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="LargeTextArea.ascx.cs"
    Inherits="CMSFormControls_Inputs_LargeTextArea" %>

<cms:ActionContainer runat="server" ID="cntElem">
    <InputTemplate>
        <cms:ExtendedTextArea ID="txtArea" runat="server" />
    </InputTemplate>
    <ActionsTemplate>
        <cms:CMSAccessibleButton ID="btnMore" runat="server" EnableViewState="false" IconCssClass="icon-arrow-right-top-square" />
    </ActionsTemplate>
</cms:ActionContainer>
