<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Newsletter_Send.aspx.cs"
    Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Send" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Tools - Newsletter send" %>

<%@ Register Src="~/CMSModules/Newsletters/Controls/SendIssue.ascx" TagPrefix="cms" TagName="SendIssue" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:SendIssue ID="sendElem" runat="server" ShowScheduler="false" ShowSendDraft="true" ShowSendLater="false" ShortID="s" />
    <table>
        <tr>
            <td>
                <div class="UnderRadioContent">
                    <cms:FormSubmitButton ID="btnSend" runat="server"
                        ResourceString="general.Send" OnClick="btnSend_Click" Enabled="false"
                        EnableViewState="false" />
                </div>
            </td>
        </tr>
    </table>
</asp:Content>