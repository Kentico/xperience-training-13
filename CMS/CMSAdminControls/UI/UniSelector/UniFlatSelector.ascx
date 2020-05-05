<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSAdminControls_UI_UniSelector_UniFlatSelector"  Codebehind="UniFlatSelector.ascx.cs" %> 
<%@ Register Src="~/CMSAdminControls/UI/Pager/UIPager.ascx" TagName="UIPager"
    TagPrefix="cms" %>
<div id="<%=ClientID%>" class="UniFlatSelector">
    <script type="text/javascript" language="javascript">
        //<![CDATA[
        // Initialize variables
        var selectedFlatItem = null;
        var selectedValue = null;
        var selectedItemName = null;
        //]]>
    </script>


    <asp:Panel runat="server" ID="pnlSearch" CssClass="uni-flat-search form-horizontal" DefaultButton="btnSearch">
        <div class="form-horizontal form-filter">
            <div class="form-group">
                <div class="control-group-inline">
                    <cms:LocalizedLabel runat="server" CssClass="sr-only" ResourceString="general.search" AssociatedControlID="txtSearch"></cms:LocalizedLabel>
                    <cms:CMSTextBox runat="server" ID="txtSearch" MaxLength="200" type="search" CssClass="uni-flat-search-input" />
                    <cms:CMSAccessibleButton runat="server" ID="btnSearch" ResourceString="general.search"
                        OnClick="btnSearch_Click" CssClass="uni-flat-search-btn" EnableViewState="false" IconOnly="True" IconCssClass="icon-magnifier uni-flat-search-icon"/>
                    <cms:CMSCheckBox ID="chkSearch" runat="server" Visible="false" CssClass="uni-flat-search-check-box" AutoPostBack="true" />
                </div>
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="plcAdditional" />
    </asp:Panel>
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlFlatArea" CssClass="UniFlatContent">
                <asp:Panel runat="server" ID="pnlRepeater" CssClass="UniFlatContentItems">
                    <asp:Panel runat="server" ID="pnlLabel" CssClass="selector-no-results" Visible="false"
                        EnableViewState="false">
                        <cms:LocalizedLabel runat="server" ID="lblError" CssClass="ErrorLabel" Visible="false" EnableViewState="false" />
                        <asp:Label runat="server" ID="lblNoRecords" Text="uniflatselector.norecords"
                            EnableViewState="false" Visible="false" />
                    </asp:Panel>
                    <cms:QueryRepeater runat="server" ID="repItems" OnItemDataBound="repItems_ItemDataBound" />
                    <asp:HiddenField ID="hdnSelectedItem" runat="server" EnableViewState="false" />
                </asp:Panel>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlPager" CssClass="uniflat-pager">
                <cms:UIPager ID="pgrItems" runat="server" ShowPageSize="false" ShowDirectPageControl="false" GroupSize="10"
                    PagerMode="Postback" />
            </asp:Panel>
            <asp:HiddenField ID="hdnItemsCount" runat="server" />
            <asp:Button runat="server" ID="btnUpdate" CssClass="HiddenButton" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</div>
