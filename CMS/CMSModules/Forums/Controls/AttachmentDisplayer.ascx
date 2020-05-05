<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Forums_Controls_AttachmentDisplayer"   Codebehind="AttachmentDisplayer.ascx.cs" %>

<asp:PlaceHolder ID="plcPostAttachments" runat="server">
    <div class="PostAttachments">
        <div class="PostAttachmentsHeader">
            <asp:Literal ID="ltlHeader" runat="server" />
        </div>
        <div class="PostAttachmentsList">
            <asp:Repeater ID="rptAttachments" runat="server">
                <ItemTemplate>
                    <%#GetAttachmentLink(Container.DataItem)%>
                </ItemTemplate>
                <SeparatorTemplate>
                    <%#mAttachmentSeparator%>
                </SeparatorTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:PlaceHolder>
