<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSPages_logon" Theme="Default"  Codebehind="logon.aspx.cs" %>

<%@ Register Src="../CMSAdminControls/UI/System/RequireScript.ascx" TagName="RequireScript"
    TagPrefix="cms" %>

<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title><%=GetString("LogonForm.Admin")%></title>
</head>
<body class="<%=mBodyClass%> LogonPageBody">
    <form id="form1" class="logon-page" runat="server">
        <cms:RequireScript ID="reqScript" runat="server" />
        <div class="logo">&nbsp;</div>
        <asp:Login ID="Login1" runat="server" DestinationPageUrl="~/Default.aspx" RenderOuterTable="False">
            <LayoutTemplate>
                <div class="center-box">
                    <div class="logon-box">
                        <asp:Panel ID="pnlContainer" runat="server" DefaultButton="LoginButton" CssClass="logon-inputs">
                            
                            <div id="JavaScript-Errors"></div>

                            <asp:PlaceHolder runat="server" ID="plcInfo" Visible="False">
                                <div class="alert alert-info">
                                    <span class="alert-icon"><i class="icon-i-circle"></i></span>
                                    <div class="alert-label">
                                        <cms:LocalizedLabel ID="txtInfo" runat="server" EnableViewState="False" />
                                    </div>
                                </div>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder runat="server" ID="plcError" Visible="False">
                                <div class="alert alert-error">
                                    <span class="alert-icon"><i class="icon-times-circle"></i></span>
                                    <div class="alert-label">
                                        <cms:LocalizedLabel ID="FailureText" runat="server" EnableViewState="False" />
                                    </div>
                                </div>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder runat="server" ID="plcWarning" Visible="False">
                                <div class="alert alert-warning">
                                    <span class="alert-icon"><i class="icon-exclamation-triangle"></i></span>
                                    <div class="alert-label">
                                        <cms:LocalizedLabel ID="txtWarning" runat="server" EnableViewState="False" />
                                    </div>
                                </div>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder runat="server" ID="plcTokenInfo" Visible="False" >
                                <div class="row">
                                    <div class="logon-label">
                                        <cms:LocalizedLabel ID="lblTokenIDlabel" runat="server" AssociatedControlClientID="lblTokenID" ResourceString="mfauthentication.label.token" CssClass="control-label" />
                                    </div>
                                    <div class="logon-text token-label">
                                        <cms:LocalizedLabel ID="lblTokenID" runat="server" />
                                    </div>
                                </div>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder runat="server" ID="plcPasscodeBox" Visible="False">
                                <div class="row">
                                    <div class="logon-label">
                                        <cms:LocalizedLabel ID="lblPasscode" runat="server" AssociatedControlID="txtPasscode" ResourceString="mfauthentication.label.passcode" CssClass="control-label" />
                                    </div>
                                    <div class="logon-text">
                                        <cms:CMSTextBox ID="txtPasscode" runat="server" MaxLength="100" />
                                    </div>
                                </div>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder runat="server" ID="plcLoginInputs">
                                <div class="row">
                                    <div class="logon-label">
                                        <cms:LocalizedLabel ID="lblUserName" runat="server" AssociatedControlID="UserName" CssClass="control-label" />
                                    </div>
                                    <div class="logon-text">
                                        <cms:CMSTextBox ID="UserName" runat="server" MaxLength="100" />
                                    </div>
                                </div>

                                <div class="row" runat="server" ID="divPassword">
                                    <div class="logon-label">
                                        <cms:LocalizedLabel ID="lblPassword" runat="server" AssociatedControlID="Password" CssClass="control-label" />
                                    </div>
                                    <div class="logon-text">
                                        <cms:CMSTextBox ID="Password" runat="server" TextMode="Password" MaxLength="100" />
                                    </div>
                                </div>

                                <asp:PlaceHolder runat="server" ID="plcRememberMe">
                                    <div class="row">
                                        <div class="logon-text logon-text-offset">
                                            <cms:LocalizedCheckBox ID="chkRememberMe" ResourceString="LogonForm.RememberMe" runat="server" />
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                            </asp:PlaceHolder>

                            <div class="row">
                                <div class="logon-text logon-text-offset">
                                    <cms:LocalizedButton ID="LoginButton" runat="server" CommandName="Login" ValidationGroup="Login1"
                                        ButtonStyle="Primary" CssClass="login-btn" />
                                </div>
                            </div>
                        </asp:Panel>
                    </div>

                    <div class="logon-help-links">
                        <cms:LocalizedHyperlink ID="lnkHelp" NavigateUrl="~/CMSPages/GetDocLink.ashx?link=logon_troubleshooting" Target="_blank" runat="server" ResourceString="LogonForm.TroubleShooting" />
                        | <cms:LocalizedLinkButton ID="lnkPassword" ResourceString="LogonForm.ForgottenPassword" runat="server" Visible="false" />
                        | <cms:LocalizedLinkButton ID="lnkLanguage" ResourceString="LogonForm.SelectLanguage" runat="server" Visible="false" />
                    </div>
                    <div id="pnlLanguage" runat="server" class="language-selector" style="display: none;">
                        <cms:LocalizedLabel ID="lblCulture" EnableViewState="false" runat="server" />
                        <cms:CMSDropDownList ID="drpCulture" runat="server" CssClass="LogonDropDownList" />
                    </div>
                </div>
            </LayoutTemplate>
        </asp:Login>

        <asp:Literal ID="ltlScript" EnableViewState="false" runat="server" />
    </form>
</body>
</html>
