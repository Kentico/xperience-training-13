<%@ Page Language="C#" Inherits="CMSAdminControls_Calendar_Calendar"
    Theme="Default"  Codebehind="Calendar.aspx.cs" %>

<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>
        <%=pagetitle%>
    </title>
    <style type="text/css">
        body {
            margin: 0px;
            padding: 0px;
            height: 100%;
        }
    </style>
</head>
<body class="<%=mBodyClass%>">
    <form id="Form1" method="post" runat="server">

        <script type="text/javascript">
            //<![CDATA[
            function CloseWindow(destinationid, value) {
                var elem = wopener.document.getElementById(destinationid);
                elem.value = value;
                if (typeof elem.onchange == "function") { elem.onchange(); }
                CloseDialog();
            }
            //]]>
        </script>

        <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
        <asp:Panel ID="pnlBody" runat="server">
            <asp:Calendar ID="calDate" runat="server" OnSelectionChanged="calDate_SelectionChanged"
                Width="100%" CssClass="CalendarTable" TitleStyle-BackColor="#B5C3D6" BorderColor="#B5C3D6"
                SelectedDayStyle-CssClass="CalendarDaySelected" SelectedDayStyle-ForeColor="Black"
                NextPrevStyle-CssClass="CalendarNextPrev" DayStyle-CssClass="CalendarDay" CellPadding="1">
                <TitleStyle CssClass="UniGridHead" BorderColor="#B5C3D6" Font-Bold="true" />
            </asp:Calendar>
            <div class="CalendarBottom">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="editing-form-value-cell editing-form-value-cell-offset">
                            <cms:CMSDropDownList ID="drpMonth" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpMonth_SelectedIndexChanged" CssClass="form-control input-width-40" />
                            <asp:Label CssClass="form-control-text" Text="/" runat="server" />
                            <cms:CMSDropDownList ID="drpYear" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpYear_SelectedIndexChanged" CssClass="form-control input-width-40" />
                            <cms:CMSButton ID="btnNow" runat="server" ButtonStyle="Default" OnClick="btnNow_Click" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-value-cell editing-form-value-cell-offset">
                            <asp:Panel runat="server" ID="pnlTime">
                                <cms:CMSDropDownList ID="drpHours" runat="server" CssClass="form-control input-width-20" />
                                <asp:Label ID="Label1" CssClass="form-control-text" Text=":" runat="server" />
                                <cms:CMSDropDownList ID="drpMinutes" runat="server" CssClass="form-control input-width-20" />
                                <asp:Label ID="Label2" CssClass="form-control-text" Text=":" runat="server" />
                                <cms:CMSDropDownList ID="drpSeconds" runat="server" CssClass="form-control input-width-20" />
                            </asp:Panel>
                            <asp:Label ID="lblGMTShift" runat="server" EnableViewState="false" Visible="false" CssClass="form-control-text" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="PageFooterLine">
                <div class="FloatRight">
                    <cms:CMSButton ID="btnCancel" runat="server" ButtonStyle="Default" />
                    <cms:CMSButton ID="btnNA" runat="server" ButtonStyle="Primary" />
                    <cms:CMSButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOK_Click" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
