<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Customers_Customer_Edit_Address_Edit" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" Title="Address properties"  Codebehind="Customer_Edit_Address_Edit.aspx.cs" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:UIForm runat="server" ID="EditForm" ObjectType="ecommerce.address"
         RedirectUrlAfterCreate="Customer_Edit_Address_Edit.aspx?customerId={%EditedObjectParent.ID%}&addressId={%EditedObject.ID%}&saved=1" />
</asp:Content>
