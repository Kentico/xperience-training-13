<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_MacroLog"
     Codebehind="MacroLog.ascx.cs" %>
<cms:UIGridView runat="server" ID="gridMacros" ShowFooter="true" AutoGenerateColumns="false" CssClass="wrap-normal">
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <strong><%#GetIndex(Eval("Indent"))%></strong>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#GetExpression(Eval("Indent"), Eval("Expression"))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#GetResult(Eval("Result"), Eval("Indent"), Eval("Expression"), Eval("Error"))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#: Eval("Identity").ToString().Length > 0 ? "(identity) " + Eval("Identity") : "" %> <%#: Eval("User").ToString().Length > 0 ? "(user) " + Eval("User") : "" %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#GetContext(Eval("Indent"), Eval("Context"))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#GetDuration(Eval("Duration"))%><br />
                <%#GetDurationChart(Eval("Duration"), 0.005, 0, 0)%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</cms:UIGridView>
