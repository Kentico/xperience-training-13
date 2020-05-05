<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_SMTPServers_Pages_Administration_Tab_Sites"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="SMTP Server Edit - Sites"
     Codebehind="Tab_Sites.aspx.cs" %>

<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">    
    <cms:CMSUpdatePanel runat="server" ID="pnlAvailability">
        <ContentTemplate>            
            <cms:CMSRadioButtonList runat="server" ID="rblSites" UseResourceStrings="true" AutoPostBack="true">
                <asp:ListItem Text="smtpserver_edit.sites_global" Value="global" Selected="true" />
                <asp:ListItem Text="smtpserver_edit.sites_persite" Value="site" />
            </cms:CMSRadioButtonList>
            <br />            
            <cms:UniSelector ID="usSites" runat="server" IsLiveSite="false" ObjectType="cms.site"
                SelectionMode="Multiple" ResourcePrefix="sitesselect" />                
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>