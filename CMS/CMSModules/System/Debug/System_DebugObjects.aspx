<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_System_Debug_System_DebugObjects" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="System - SQL"  Codebehind="System_DebugObjects.aspx.cs" %>

<asp:Content ContentPlaceHolderID="plcActions" runat="server">
    <div class="header-actions-container">
        <cms:CMSButton runat="server" ID="btnClear" OnClick="btnClear_Click" ButtonStyle="Default" EnableViewState="false" />
    </div>
</asp:Content>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UIGridView runat="server" ID="gridHashtables" ShowFooter="true" AutoGenerateColumns="false">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("TableName")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetTableCount(Eval("ObjectCount"))%>
                </ItemTemplate>
                <FooterTemplate>
                    <strong>
                        <%#totalTableObjects%>
                    </strong>
                </FooterTemplate>
            </asp:TemplateField>
        </Columns>
    </cms:UIGridView>
    <cms:UIGridView runat="server" ID="gridObjects" ShowFooter="true" AutoGenerateColumns="false">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("ObjectType")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetCount(Eval("ObjectCount"))%>
                </ItemTemplate>
                <FooterTemplate>
                    <strong>
                        <%#totalObjects%>
                    </strong>
                </FooterTemplate>
            </asp:TemplateField>
        </Columns>
    </cms:UIGridView>
</asp:Content>

