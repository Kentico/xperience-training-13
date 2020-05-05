<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_Widgets_UI_Widget_Edit_Documentation"
    Theme="Default" EnableEventValidation="false"  Codebehind="Widget_Edit_Documentation.aspx.cs" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <table style="width: 100%">
        <tbody>
            <tr>
                <td>
                    <cms:CMSHtmlEditor ID="htmlText" runat="server" Height="500px" Width="100%" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOk_Click" />
                </td>
            </tr>
        </tbody>
    </table>
</asp:Content>
