<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_WebFarmLog"
     Codebehind="WebFarmLog.ascx.cs" %>
<cms:UIGridView runat="server" ID="gridQueries" ShowFooter="true" AutoGenerateColumns="false">
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <strong><%#GetIndex()%></strong>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <strong><%#Eval("TaskType")%></strong>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <strong><%#HTMLHelper.HTMLEncode(ValidationHelper.GetString(Eval("Target"), ""))%></strong>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <strong><%#GetData(Eval("TextData"), Eval("BinaryData"))%></strong>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#GetContext(Eval("Context"))%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</cms:UIGridView>
