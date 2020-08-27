<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_ValuesTable"
     Codebehind="ValuesTable.ascx.cs" %>
<div>
    <asp:Literal runat="server" ID="ltlInfo" EnableViewState="false" />
    <cms:UIGridView runat="server" ID="gridValues" AutoGenerateColumns="false">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <strong><%#GetIndex()%></strong>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </cms:UIGridView>
</div>
