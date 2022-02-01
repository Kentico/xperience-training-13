<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CookieLevel.aspx.cs" Inherits="CMSApp.CMSModules.MedioClinic.CookieLevel" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSFormControls/MedioClinic/CookieLevelSelector.ascx" TagName="CookieLevelSelector"
    TagPrefix="mc" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent" EnableViewState="false">
    <cms:LocalizedHeading runat="server" ID="headConsentCookieLevel" Level="4" ResourceString="OnlineMarketing.CookieLevel" DisplayColon="true" EnableViewState="false" />
    <p>
        <mc:CookieLevelSelector ID="clsCookieLevel" runat="server" />
    </p>
    <p>
        <cms:LocalizedButton ID="btnSave" ButtonStyle="Default" runat="server"
            OnClick="btnSave_Click" ResourceString="General.Save" EnableViewState="false" />
    </p>
</asp:Content>
