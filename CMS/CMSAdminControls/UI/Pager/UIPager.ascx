<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_Pager_UIPager"
     Codebehind="UIPager.ascx.cs" %>

<div class="pagination">
    <cms:UniPager ID="pagerElem" ShortID="p" runat="server">
        <FirstPageTemplate>
            <li class="pagination-list-navigation">
                <a href="<%#EvalHtmlAttribute("FirstURL")%>" title="<%=FirstPageText %>">
                    <i class="icon-chevron-double-left" aria-hidden="true"></i>
                    <span class="sr-only"><%=FirstPageText %></span>
                </a>
            </li>
        </FirstPageTemplate>
        <PreviousPageTemplate>
            <li class="pagination-list-navigation">
                <a href="<%#EvalHtmlAttribute("PreviousURL")%>" title="<%=PreviousPageText %>">
                    <i class="icon-chevron-left" aria-hidden="true"></i>
                    <span class="sr-only"><%=PreviousPageText %></span>
                </a>
            </li>
        </PreviousPageTemplate>
        <PreviousGroupTemplate>
            <li>
                <a href="<%#EvalHtmlAttribute("PreviousGroupURL")%>"><%=PreviousGroupText%></a>
            </li>
        </PreviousGroupTemplate>
        <PageNumbersTemplate>
            <li>
                <a href="<%#EvalHtmlAttribute("PageURL")%>"><%#Eval("Page")%></a>
            </li>
        </PageNumbersTemplate>
        <PageNumbersSeparatorTemplate>
        </PageNumbersSeparatorTemplate>
        <CurrentPageTemplate>
            <li class="active">
                <span><%#Eval("Page")%></span>
            </li>
        </CurrentPageTemplate>
        <NextGroupTemplate>
            <li>
                <a href="<%#EvalHtmlAttribute("NextGroupURL")%>"><%=NextGroupText%></a>
            </li>
        </NextGroupTemplate>
        <NextPageTemplate>
            <li class="pagination-list-navigation">
                <a href="<%#EvalHtmlAttribute("NextURL")%>" title="<%=NextPageText%>">
                    <i class="icon-chevron-right" aria-hidden="true"></i>
                    <span class="sr-only"><%=NextPageText%></span>
                </a>
            </li>
        </NextPageTemplate>
        <LastPageTemplate>
            <li class="pagination-list-navigation">
                <a href="<%#EvalHtmlAttribute("LastURL")%>" title="<%=LastPageText %>">
                    <i class="icon-chevron-double-right" aria-hidden="true"></i>
                    <span class="sr-only"><%=LastPageText %></span>
                </a>
            </li>
        </LastPageTemplate>
        <DirectPageTemplate>
            <div class="pagination-pages">
                <cms:LocalizedLabel ID="lblPage" runat="server" ResourceString="UniGrid.Page" />
                <cms:CMSTextBox ID="txtPage" runat="server" />
                <cms:CMSDropDownList ID="drpPage" runat="server" />
                <span class="pages-max">/ <%#Eval("Pages")%></span>
            </div>
        </DirectPageTemplate>
        <LayoutTemplate>
            <ul class="pagination-list">
                <asp:PlaceHolder runat="server" ID="plcFirstPage"></asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plcPreviousPage"></asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plcPreviousGroup"></asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plcPageNumbers"></asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plcNextGroup"></asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plcNextPage"></asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plcLastPage"></asp:PlaceHolder>
            </ul>
            <asp:PlaceHolder runat="server" ID="plcDirectPage"></asp:PlaceHolder>
        </LayoutTemplate>
    </cms:UniPager>

    <asp:PlaceHolder ID="plcPageSize" runat="server">
        <div class="pagination-items-per-page">
            <cms:LocalizedLabel ID="lblPageSize" runat="server" EnableViewState="false" ResourceString="UniGrid.ItemsPerPage" AssociatedControlID="drpPageSize" />
            <cms:CMSDropDownList ID="drpPageSize" runat="server" AutoPostBack="true" />
        </div>
    </asp:PlaceHolder>
</div>
