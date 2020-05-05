<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Licenses_Pages_License_Export_Domains"
Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Licenses - Export domains"  Codebehind="License_Export_Domains.aspx.cs" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <asp:PlaceHolder runat="server" ID="plcTextBox">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ShowRequiredMark="True" AssociatedControlID="txtFileName" CssClass="control-label" ID="lblFileName" runat="server" EnableViewState="False" ResourceString="general.filename" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtFileName" runat="server" MaxLength="200" />
                    <cms:CMSRequiredFieldValidator ID="rfvFileName" runat="server" EnableViewState="false"
                        ControlToValidate="txtFileName" Display="dynamic" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-value-cell editing-form-value-cell-offset">
                    <cms:LocalizedButton ID="btnOk" runat="server" OnClick="btnOk_Click" EnableViewState="false" ResourceString="general.export" ButtonStyle="Primary" />
                </div>
            </div>
        </div>
   </asp:PlaceHolder>
</asp:Content>