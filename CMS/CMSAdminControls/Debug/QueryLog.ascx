<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_QueryLog"
     Codebehind="QueryLog.ascx.cs" %>
<cms:UIGridView runat="server" ID="gridQueries" ShowFooter="true" AutoGenerateColumns="false" CssClass="wrap-normal">
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <strong>
                    <%#GetIndex(Eval("IsInformation"), Eval("QueryResultsSize"), Eval("QueryParametersSize"), Eval("QueryName"), Eval("QueryText"))%>
                    <%#GetDuplicity(Eval("Duplicit"), Eval("QueryText"))%>
                    <%#GetConnectionString(Eval("ConnectionString"))%>
                </strong>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#GetInformation(Eval("IsInformation"), Eval("ConnectionString"), Eval("ConnectionOp"), Eval("QueryName"), Eval("QueryText"), Eval("QueryParameters"), Eval("QueryParametersSize"), Eval("QueryResults"), Eval("QueryResultsSize"), MaxSize)%>
            </ItemTemplate>
            <FooterTemplate>
                <strong>
                    <%#String.Format(ResHelper.GetAPIString("QueryLog.Total", null, "QueryLog.Total {0} / {1} / {2}"), index, DataHelper.GetSizeString(TotalParamSize), DataHelper.GetSizeString(TotalSize))%>
                </strong>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#GetContext(Eval("Context"))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%#GetDuration(Eval("QueryDuration"))%><br />
                <%#GetDurationChart(Eval("QueryDuration"), 0.005, 0, 0)%>
            </ItemTemplate>
            <FooterTemplate>
                <strong>
                    <%#TotalDuration.ToString("F3")%>
                </strong>
            </FooterTemplate>
        </asp:TemplateField>
    </Columns>
</cms:UIGridView>