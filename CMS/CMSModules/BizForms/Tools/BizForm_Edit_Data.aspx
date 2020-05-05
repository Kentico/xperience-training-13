<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_BizForms_Tools_BizForm_Edit_Data" EnableEventValidation="false"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="BizForm Data" Theme="Default"  Codebehind="BizForm_Edit_Data.aspx.cs" %>

<%@ Register Src="~/CMSModules/BizForms/Controls/BizFormEditData.ascx" TagName="BizFormData" TagPrefix="cms" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="plcContent">
    <cms:BizFormData runat="server" ID="bizFormData" />
</asp:Content>