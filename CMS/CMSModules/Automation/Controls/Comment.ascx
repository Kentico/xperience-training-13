<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Comment.ascx.cs" Inherits="CMSModules_Automation_Controls_Comment" %>
<cms:CMSPanel ID="pnlContainer" ShortID="pC" runat="server">
    <div class="form-horizontal">
        <asp:PlaceHolder ID="plcSteps" runat="server" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSteps" runat="server" ResourceString="doc.stepslist" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSDropDownList ID="drpSteps" runat="server" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblComment" runat="server" ResourceString="worfklowproperties.comment" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextArea ID="txtComment" CssClass="form-control" runat="server" Rows="10" />
            </div>
        </div>
    </div>
</cms:CMSPanel>
