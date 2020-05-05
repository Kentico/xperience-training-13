<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Versions.ascx.cs" Inherits="CMSModules_Content_Controls_Versions" %>
<%@ Register Src="~/CMSModules/Content/Controls/VersionList.ascx" TagName="VersionList"
    TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlVersions">
    <asp:PlaceHolder ID="plcForm" runat="server">
        <cms:LocalizedHeading runat="server" ID="headCheckOut" ResourceString="properties.scopenotset" Level="4" EnableViewState="false" />
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblVersion" runat="server" ResourceString="VersionsProperties.Version" AssociatedControlID="txtVersion" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtVersion" runat="server" MaxLength="50" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblComment" runat="server" ResourceString="VersionsProperties.Comment"
                        EnableViewState="false" AssociatedControlID="txtComment" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextArea ID="txtComment" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-value-cell editing-form-value-cell-offset">
                    <cms:LocalizedButton ID="btnCheckout" runat="server" ButtonStyle="Primary" Visible="false"
                        OnClick="btnCheckout_Click" ResourceString="VersionsProperties.btnCheckout" EnableViewState="false" />

                    <cms:LocalizedButton ID="btnUndoCheckout" runat="server" Visible="false" OnClick="btnUndoCheckout_Click"
                        ResourceString="VersionsProperties.btnUndoCheckout" ButtonStyle="Default"
                        EnableViewState="false" />
                    <cms:LocalizedButton ID="btnCheckin" runat="server" ButtonStyle="Primary" Visible="false"
                        OnClick="btnCheckin_Click" ResourceString="VersionsProperties.btnCheckin" EnableViewState="false" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
    <cms:VersionList ID="versionsElem" runat="server" IsLiveSite="false" />
</asp:Panel>
