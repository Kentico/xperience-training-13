<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_PageElements_AnchorDropup"  Codebehind="AnchorDropup.ascx.cs"%>

<asp:Panel ID="pnlWrapper" runat="server">        
    <div class="btn-group context-menu dropup anchor-dropup pull-right <%= IsOpened ? "open" : "" %><%= CssClass %>">        
        <button type="button" class="btn btn-default dropdown-toggle icon-only" data-toggle="dropdown">
            <i aria-hidden="true" class="icon-compass"></i><span class="sr-only"><%= GetString("anchordropup.buttontooltip")  %></span>
        </button>        
        <asp:Repeater ID="repNavigationItems" runat="server">
            <HeaderTemplate>
                <ul class="dropdown-menu pull-up pull-left sharp-corner">
            </HeaderTemplate>     
            <ItemTemplate> 
                <li><a href="#<%# Eval("Value") %>">
                    <%# HTMLHelper.HtmlToPlainText(Eval("Key").ToString()) %>
                </a></li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>                
    </div>   
</asp:Panel>