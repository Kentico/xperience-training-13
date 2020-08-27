<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_RequestLog"
     Codebehind="RequestLog.ascx.cs" %>
<%@ Register Src="ValuesTable.ascx" TagName="ValuesTable" TagPrefix="cms" %>

<cms:UIGridView runat="server" ID="gridCache" ShowFooter="true" AutoGenerateColumns="false">
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <strong><%#GetIndex()%></strong>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#GetBeginIndent(Eval("Indent"))%><%#Eval("Method")%><%#GetEndIndent(Eval("Indent"))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#Eval("Parameter")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#GetFromStart(Eval("Time"))%>
                <br />
            </ItemTemplate>
            <FooterTemplate>
                <strong><%#TotalDuration.ToString("F3")%></strong>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#GetDuration(Eval("Duration"))%><br />
                <%#GetDurationChart(Eval("Duration"), 0.005, 0, 0)%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</cms:UIGridView>
<cms:ValuesTable ID="tblReqC" runat="server" EnableViewState="False" />
<cms:ValuesTable ID="tblResC" runat="server" EnableViewState="False" />
<cms:ValuesTable ID="tblVal" runat="server" EnableViewState="False" />
