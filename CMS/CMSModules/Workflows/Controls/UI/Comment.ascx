<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Comment.ascx.cs" Inherits="CMSModules_Workflows_Controls_UI_Comment" %>
<cms:CMSPanel ID="pnlContainer" ShortID="pC" runat="server">
    <div class="form-horizontal">
        <asp:PlaceHolder ID="plcSteps" runat="server" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSteps" runat="server" ResourceString="doc.stepslist" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSDropDownList ID="drpSteps" runat="server" Width="100%" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblComment" runat="server" ResourceString="worfklowproperties.comment" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextArea CssClass="form-control" ID="txtComment" runat="server" Rows="10" />
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hdnArg" runat="server" />
</cms:CMSPanel>