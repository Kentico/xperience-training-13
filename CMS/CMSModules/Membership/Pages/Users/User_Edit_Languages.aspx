<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Membership_Pages_Users_User_Edit_Languages" ValidateRequest="false"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="User Edit - Languages"  Codebehind="User_Edit_Languages.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>


<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlLanguages" runat="server">
        <div class="form-horizontal">
            <div class="radio-list-vertical">
                <cms:CMSRadioButton ID="radAllLanguages" runat="server" ResourceString="transman.editallcultures"
                    GroupName="grpLanguages" CssClass="" AutoPostBack="true" />
                <cms:CMSRadioButton ID="radSelectedLanguages" runat="server" ResourceString="transman.editselectedcultures"
                    GroupName="grpLanguages" CssClass="" AutoPostBack="true" />
                <div class="selector-subitem">
                    <asp:PlaceHolder ID="plcSite" runat="server">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblSite" runat="server" ResourceString="transman.selectsite"
                                    DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" />
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcCultures" runat="server">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="general.language"
                                    DisplayColon="true" />
                            </div>
                            <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="editing-form-value-cell">
                                        <cms:UniSelector DialogWindowWidth="750" ID="uniSelector" runat="server" IsLiveSite="false" ObjectType="cms.culture"
                                            SelectionMode="Multiple" ResourcePrefix="languageselect" OrderBy="CultureName" />
                                    </div>
                                </ContentTemplate>
                            </cms:CMSUpdatePanel>
                        </div>

                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
