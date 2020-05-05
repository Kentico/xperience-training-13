<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Customers_Customer_Edit_Credit_Edit"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="Customer_Edit_Credit_Edit.aspx.cs" Theme="Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="server">
    <cms:UIForm runat="server" ID="EditForm" ObjectType="ecommerce.creditevent"
         RedirectUrlAfterCreate="Customer_Edit_Credit_Edit.aspx?eventId={%EditedObject.ID%}&customerId={%EditedObject.ParentID%}&saved=1" />
</asp:Content>