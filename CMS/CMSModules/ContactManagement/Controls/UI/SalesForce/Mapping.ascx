<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Mapping.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_SalesForce_Mapping" %>
<%@ Import Namespace="CMS.SalesForce" %>
<%@ Register TagPrefix="cms" TagName="SalesForceError" Src="~/CMSModules/ContactManagement/Controls/UI/SalesForce/Error.ascx" %>
<cms:SalesForceError ID="SalesForceError" runat="server" EnableViewState="false" />
<div id="ContainerControl" runat="server">
    <table cellspacing="0" cellpadding="0" style="margin-bottom:1em">
        <tbody>
            <tr>
                <td style="font-weight:bold;color:inherit"><%= HTMLHelper.HTMLEncode(GetString("sf.mapping.attributeheader"))%></td>
                <td style="font-weight:bold;padding-left:2em;color:inherit"><%= HTMLHelper.HTMLEncode(GetString("sf.mapping.sourceheader"))%></td>
            </tr>
            <asp:Repeater ID="ItemRepeater" runat="server">
                <ItemTemplate>
                    <tr>
                        <td style="color:inherit"><%# HTMLHelper.HTMLEncode(((MappingItem)Container.DataItem).AttributeLabel) %></td>
                        <td style="padding-left:2em;color:inherit">
                            <%# HTMLHelper.HTMLEncode(FormatSourceLabel((MappingItem)Container.DataItem)) %>
                            <span style="color:Gray;font-size:smaller">(<%# HTMLHelper.HTMLEncode(FormatSourceType((MappingItem)Container.DataItem))%>)</span>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
</div>
<p id="MessageControl" runat="server" enableviewstate="false" visible="false"></p>
