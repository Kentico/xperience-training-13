<%@ Page Language="C#" AutoEventWireup="False"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_MyDesk_MyProfile_MyProfile_MyDetails"
    Theme="Default"  Codebehind="MyProfile_MyDetails.aspx.cs" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:DataForm ID="editProfileForm" runat="server" ClassName="cms.user" AlternativeFormFullName="cms.user.EditProfileMyDesk"
        OnOnAfterDataLoad="editProfileForm_OnAfterDataLoad" OnOnBeforeSave="editProfileForm_OnBeforeSave" OnOnAfterSave="editProfileForm_OnAfterSave" />    
</asp:Content>
