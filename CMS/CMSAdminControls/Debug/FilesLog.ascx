<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_FilesLog"
    EnableViewState="false"  Codebehind="FilesLog.ascx.cs" %>
<cms:UIGridView runat="server" ID="gridStates" ShowFooter="true" AutoGenerateColumns="false">
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <strong><%#GetIndex()%><%#GetWarning(Eval("FileNotClosed"), Eval("FileOperation"))%></strong>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#GetFileOperation(Eval("FileOperation"), Eval("FileParameters"))%><br />
                <%#GetSizeAndAccesses(MaxSize, Eval("FileSize"), Eval("FileAccesses"), false)%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#GetPath(Eval("FilePath"))%>
                <%#GetText(Eval("FileText"))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%# GetList(Eval("ProviderName")) %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#GetContext(Eval("Context"))%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</cms:UIGridView>
