<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_Debug_System_DebugThreads"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="System - SQL"
     Codebehind="System_DebugThreads.aspx.cs" %>

<asp:Content ContentPlaceHolderID="plcActions" runat="server">
    <div class="header-actions-container">
        <cms:CMSButton runat="server" ID="btnRunDummy" OnClick="btnRunDummy_Click" ButtonStyle="Default" />
    </div>
</asp:Content>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:HiddenField runat="server" ID="hdnGuid" EnableViewState="false" />
    <asp:Button runat="server" ID="btnCancel" CssClass="HiddenButton" OnClick="btnCancel_Click" />
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
        <ContentTemplate>
            <cms:LocalizedHeading runat="server" ID="headThreadsRunning" Level="4" CssClass="listing-title" EnableViewState="False" ResourceString="Debug.RunningThreads" DisplayColon="True" />
            <cms:UIGridView runat="server" ID="gridThreads" ShowFooter="true" AutoGenerateColumns="false">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <%#GetIndex()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <%#GetCancelAction(Eval("ThreadGUID"), Eval("Status"))%>
                            <%#GetLogAction(Eval("HasLog"), Eval("ThreadGUID"))%>
                            <%#GetDebugAction(Eval("RequestGUID"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <strong><%#Eval("MethodClassName")%></strong>.<%#Eval("MethodName")%><br />
                            <%#Eval("RequestUrl")%>
                        </ItemTemplate>
                        <ItemStyle CssClass="filling-column"></ItemStyle>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <%#Eval("ThreadID")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <%#Eval("Status")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <%#Eval("ThreadStarted")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <%#GetDuration(Eval("ThreadStarted"), null)%>
                        </ItemTemplate>
                        <FooterTemplate>
                            <strong><%#GetDurationString(totalDuration)%></strong>
                        </FooterTemplate>
                    </asp:TemplateField>
                </Columns>
            </cms:UIGridView>
            <cms:LocalizedHeading runat="server" ID="headThreadsFinished" Level="4" CssClass="listing-title" EnableViewState="False" ResourceString="Debug.FinishedThreads" DisplayColon="true" />
            <cms:UIGridView runat="server" ID="gridFinished" ShowFooter="true" AutoGenerateColumns="false">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <%#GetIndex()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <%#GetDebugAction(Eval("RequestGUID"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <strong><%#Eval("MethodClassName")%></strong>.<%#Eval("MethodName")%><br />
                            <%#Eval("RequestUrl")%>
                        </ItemTemplate>
                        <ItemStyle CssClass="filling-column"></ItemStyle>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <%#Eval("ThreadID")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <%#Eval("Status")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <%#Eval("ThreadStarted")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <%#Eval("ThreadFinished")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <%#GetDuration(Eval("ThreadStarted"), Eval("ThreadFinished"))%>
                        </ItemTemplate>
                        <FooterTemplate>
                            <strong><%#GetDurationString(totalDuration)%></strong>
                        </FooterTemplate>
                    </asp:TemplateField>
                </Columns>
            </cms:UIGridView>
            <asp:Timer runat="server" ID="timRefresh" Enabled="true" Interval="1000" OnTick="timRefresh_Tick" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
