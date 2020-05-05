<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CopyMoveLinkProperties.ascx.cs"
    Inherits="CMSModules_Content_Controls_Dialogs_Properties_CopyMoveLinkProperties" %>

<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog" TagPrefix="cms" %>

<asp:Panel runat="server" ID="pnlLog" Visible="false">
    <cms:AsyncLog ID="ctlAsyncLog" runat="server" ProvideLogContext="true" LogContextNames="Documents" />
</asp:Panel>
<div class="DialogInfoArea" id="ContentDiv">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <asp:Panel runat="server" ID="pnlEmpty" Visible="true" EnableViewState="false">
        <asp:Label runat="server" ID="lblEmpty" EnableViewState="false" />
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlGeneralTab" Visible="false">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCopyMoveInfo" ResourceString="dialogs.copymove.target"
                        EnableViewState="false" DisplayColon="true" AssociatedControlID="lblAliasPath" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label runat="server" CssClass="form-control-text" ID="lblAliasPath" EnableViewState="false" />
                </div>
            </div>
            <asp:PlaceHolder ID="plcUnderlying" runat="server">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblUnderlying" runat="server" DisplayColon="True"
                            ResourceString="contentrequest.copyunderlying" AssociatedControlID="chkUnderlying" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkUnderlying" runat="server" AutoPostBack="true" OnCheckedChanged="chkUnderlying_OnCheckedChanged"
                            Checked="true" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcCopyPermissions" runat="server">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="contentrequest.copypermissions"
                            DisplayColon="True" AssociatedControlID="chkCopyPermissions" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkCopyPermissions" runat="server" AutoPostBack="true"
                            OnCheckedChanged="chkCopyPermissions_OnCheckedChanged" Checked="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcPreservePermissions" runat="server">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" DisplayColon="True"
                            ResourceString="contentrequest.preservepermissions" AssociatedControlID="chkPreservePermissions" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkPreservePermissions" runat="server" AutoPostBack="true"
                            OnCheckedChanged="chkPreservePermissions_OnCheckedChanged" Checked="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblDocToCopy" runat="server" DisplayColon="true"
                        EnableViewState="false" AssociatedControlID="lblDocToCopyList" />
                </div>
                <div class="editing-form-value-cell">
                    <div class="form-control vertical-scrollable-list">
                        <asp:Label ID="lblDocToCopyList" runat="server" EnableViewState="false" />
                    </div>
                </div>
            </div>
        </div>
        <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" EnableViewState="false" />
        <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" />
    </asp:Panel>
</div>
