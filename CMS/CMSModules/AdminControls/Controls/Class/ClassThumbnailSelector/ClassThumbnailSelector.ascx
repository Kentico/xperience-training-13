<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ClassThumbnailSelector.ascx.cs" Inherits="CMSModules_AdminControls_Controls_Class_ClassThumbnailSelector_ClassThumbnailSelector" %>
<%@ Import Namespace="CMS.IO" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniFlatSelector.ascx" TagName="UniFlatSelector"
    TagPrefix="cms" %>

<cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
    <ContentTemplate>
        <%-- Wrap the UniFlatSelector in a panel with ItemSelector class to allow JavaScript to resize it as needed --%>
        <asp:Panel runat="server" ID="pnlSelector" CssClass="ItemSelector">
            <cms:UniFlatSelector ID="flatElem"
                                 QueryName="cms.metafile.selectall"
                                 SelectedColumns="MetaFileGUID, MetaFileTitle, MetaFileDescription, MetaFileName"
                                 OrderBy="MetaFileTitle"
                                 ValueColumn="MetaFileGUID"
                                 NoRecordsMessage="classimageselector.norecords"
                                 RememberSelectedItem="True"
                                 SearchLabelResourceString="classimageselector.searchlabel"
                                 SearchColumn="MetaFileTitle;MetaFileDescription"
                                 ImageMaxSideSize="100"
                                 runat="server">
                <HeaderTemplate>
                    <div class="SelectorFlatItems">
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="SelectorEnvelope" title="<%#HTMLHelper.HTMLEncode(Convert.ToString(Eval("MetaFileDescription")))%>">
                        <div class="SelectorFlatImage">
                            <img alt="<%#HTMLHelper.HTMLEncode(Convert.ToString(Eval("MetaFileTitle")))%>" data-metafile-guid="<%#Eval("MetaFileGUID")%>" data-metafile-extensionless-filename="<%# Path.GetFileNameWithoutExtension(Eval("MetaFileName").ToString()) %>"
                                src="<%#flatElem.GetFlatImageUrl(Eval("MetaFileGUID"))%>" />
                        </div>
                        <span class="SelectorFlatText">
                            <%#HTMLHelper.HTMLEncode(Convert.ToString(Eval("MetaFileTitle")))%>
                        </span>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <div style="clear: both"></div>
                    </div> <%-- Closing <div class="SelectorFlatItems"> from <HeaderTemplate> --%>
                </FooterTemplate>
            </cms:UniFlatSelector>
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>