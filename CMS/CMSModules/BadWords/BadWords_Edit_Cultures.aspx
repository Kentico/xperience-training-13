<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_BadWords_BadWords_Edit_Cultures" Theme="Default"  Codebehind="BadWords_Edit_Cultures.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="radio-list-vertical">
                        <cms:CMSRadioButton ID="radAll" runat="server" ResourceString="badwords.culturesall"
                            GroupName="grpCultures" AutoPostBack="true" />
                        <cms:CMSRadioButton ID="radSelected" runat="server" ResourceString="badwords.culturesselected"
                            GroupName="grpCultures" AutoPostBack="true" />
                        <div class="selector-subitem">
                            <cms:UniSelector ID="usWordCultures" runat="server" IsLiveSite="false" ObjectType="cms.culture"
                                SelectionMode="Multiple" OrderBy="CultureName" DialogWindowHeight="650" />
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
