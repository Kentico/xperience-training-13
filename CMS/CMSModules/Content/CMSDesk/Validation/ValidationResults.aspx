<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="ValidationResults.aspx.cs"
    Inherits="CMSModules_Content_CMSDesk_Validation_ValidationResults" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/Objects/Controls/ViewObjectDataSet.ascx" TagPrefix="cms"
    TagName="ViewDataSet" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:ViewDataSet ID="viewDataSet" ShortID="vds" runat="server" ForceRowDisplayFormat="true"
        EncodeDisplayedData="false" />
</asp:Content>
