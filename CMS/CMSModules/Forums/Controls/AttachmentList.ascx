<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Forums_Controls_AttachmentList"  Codebehind="AttachmentList.ascx.cs" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<table class="AttachmentTable" cellpadding="0" cellspacing="0">
    <asp:PlaceHolder runat="server" ID="plcListHeader">
        <tr class="AttachmentTableHeader">
            <th class="AttachmentFileName">
                <span>
                    <%=ResHelper.GetString("forums.attachment.filename")%>
                </span>
            </th>
            <th  class="AttachmentFileSize">
                <span>
                    <%=ResHelper.GetString("forums.attachment.filesize")%>
                </span>
            </th>
            <th>
            </th>
        </tr>
    </asp:PlaceHolder>
    <cms:UniView runat="server" ID="listAttachment">
        <%-- Item template--%>
        <ItemTemplate>
            <tr class="AttachmentTableRow">
                <td class="AttachmentFileName">
                    <a title="Attachment" target="_blank" href="<%#GetAttachmentUrl(Eval("AttachmentGuid"))%>">
                        <%#HTMLHelper.HTMLEncode(Convert.ToString(Eval("AttachmentFileName")))%>
                    </a>
                </td>
                <td class="AttachmentFileSize">
                    <%#Eval("AttachmentFileSize")%> B
                </td>
                <td class="AttachmentAction">
                    <asp:LinkButton ID="btnDelete" runat="server" OnCommand="btnDelete_OnCommand" CommandName="delete"
                        CommandArgument='<%#Eval("AttachmentID")%>' Text='<%#ResHelper.GetString("general.Delete")%>'
                        OnClientClick="return DeleteConfirm();" />
                </td>
            </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <tr class="AttachmentTableAlternateRow">
                <td class="AttachmentFileName">
                     <a title="Attachment" target="_blank" href="<%#GetAttachmentUrl(Eval("AttachmentGuid"))%>">
                        <%#HTMLHelper.HTMLEncode(Convert.ToString(Eval("AttachmentFileName")))%>
                    </a>
                </td>
                <td class="AttachmentFileSize">
                    <%#Eval("AttachmentFileSize")%> B
                </td>
                <td class="AttachmentAction">
                    <asp:LinkButton ID="btnDelete" runat="server" OnCommand="btnDelete_OnCommand" CommandName="delete"
                        CommandArgument='<%#Eval("AttachmentID")%>' Text='<%#ResHelper.GetString("general.Delete")%>'
                        OnClientClick="return DeleteConfirm();" />
                </td>
            </tr>
        </AlternatingItemTemplate>
    </cms:UniView>
    <tr>
        <td colspan="3" class="AttachmentTableUpload">
            <cms:CMSFileUpload ID="fileUpload" runat="server" />
            <cms:CMSButton ID="btnUpload" runat="server" ButtonStyle="Default" OnClick="btnUpload_OnClick" />
            <cms:CMSButton ID="btnBack" runat="server" ButtonStyle="Default" OnClick="btnBack_OnClick" />
        </td>
    </tr>
    <tr>
        <td colspan="3" class="AttachmentTableFooter">
        </td>
    </tr>
</table>
