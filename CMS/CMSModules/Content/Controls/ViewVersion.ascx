<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_ViewVersion"
     Codebehind="ViewVersion.ascx.cs" %>

<script type="text/javascript">
    //<![CDATA[
    function TipImage(imageWidth, url, attachmentName) {
        Tip('<div style=\'width:' + imageWidth + 'px;text-align:center;\'><img src=\'' + url + '\' alt=\'' + attachmentName + '\' /></div>');
    }

    function versionExpandWebParts(el) {
        var parent = el.parentElement.parentElement;
        var leftElem = parent.getElementsByClassName('vWpLeft')[0];
        var leftElemLink = parent.getElementsByClassName('vWpLeftLink')[0];
        versionExpandWebPartsInternal(leftElem, leftElemLink);

        var rightElem = parent.getElementsByClassName('vWpRight')[0];
        var rightElemLink = parent.getElementsByClassName('vWpRightLink')[0];
        versionExpandWebPartsInternal(rightElem, rightElemLink);
    }

    function versionExpandWebPartsInternal(elemToShow, elemToHide) {
        if (elemToShow != null) {
            elemToShow.style.display = '';
        }

        if (elemToHide != null) {
            elemToHide.style.display = 'none';
        }
    }

    //]]>
</script>

<asp:Panel ID="pnlControl" runat="server">
    <style type="text/css">
        .VZoneEnvelope {
            padding: 5px;
        }

        .VWebPartEnvelope {
            padding: 5px;
        }

        .UnsortedLeft {
            width: 50%;
            float: left;
        }

        .UnsortedRight {
            width: 50%;
            float: right;
        }

        .PageContent .NotIncluded, .PageContent .HTMLNotIncluded {
            display: inline;
            visibility: hidden;
        }

        td {
            vertical-align: top;
        }

        .json-array {
            padding-top: 5px;
        }

        .json-property {
            padding-left: 20px;
        }

        .vWpLeft > .json-property, .vWpRight > .json-property {
            padding-left: 0px;
        }
    </style>
    <asp:Panel ID="pnlAdditionalControls" runat="server" CssClass="header-panel">
        <div class="form-horizontal form-filter">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel ID="lblCompareTo" CssClass="control-label" runat="server" ResourceString="content.compareto"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSDropDownList ID="drpCompareTo" runat="server" CssClass="DropDownField"
                        AutoPostBack="true" />
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Panel>
<asp:Panel ID="pnlBody" runat="server">
    <cms:MessagesPlaceHolder runat="server" ID="plcMess" />
    <asp:Table ID="tblDocument" runat="server" CellPadding="-1" CellSpacing="-1" CssClass="table wrap-normal table-hover" />
</asp:Panel>