<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Viewers_Documents_ImageGallery"  Codebehind="~/CMSWebParts/Viewers/Documents/imagegallery.ascx.cs" %>

<table cellspacing="0">
    <asp:PlaceHolder ID="plcButtonsTop" runat="server">
        <asp:PlaceHolder runat="server" ID="plcNavigation" Visible="false">
            <tr class="ImageGalleryPager">
                <td style="text-align: left; width: 20%;">
                    <asp:HyperLink runat="server" ID="lnkPrevious" EnableViewState="false" /></td>
                <td style="text-align: center; width: 60%;">
                    <cms:LocalizedLabel ID="lblImages" AssociatedControlID="drpImages" runat="server" />
                    <cms:CMSDropDownList runat="server" ID="drpImages" CssClass="input-width-20" />
                    &nbsp;<asp:Label runat="server" ID="lblOf" EnableViewState="false" />
                    &nbsp;<strong><asp:Label runat="server" ID="lblTotal" EnableViewState="false" /></strong>&nbsp;
                    <asp:HyperLink runat="server" ID="lnkThumbnails" EnableViewState="false" />
                </td>
                <td style="text-align: right; width: 20%">
                    <asp:HyperLink runat="server" ID="lnkNext" EnableViewState="false" /></td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcPager">
            <tr class="ImageGalleryPager">
                <td colspan="3" style="text-align: center;">
                    <cms:DataPager ID="pagerElem" runat="server" ResultsFormat="" />
                </td>
            </tr>
        </asp:PlaceHolder>
    </asp:PlaceHolder>
    <tr>
        <td colspan="3" style="height: 76px">
            <cms:CMSDataList ID="lstImages" runat="server" EnablePaging="true" />
        </td>
    </tr>
    <asp:PlaceHolder ID="plcButtonsBottom" runat="server" />
</table>
<asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />

<script type="text/javascript">
//<![CDATA[
    function ChangeImage()
    {
        url = drpElem.value;
        document.location.replace(url);
    }
//]]>
</script>
