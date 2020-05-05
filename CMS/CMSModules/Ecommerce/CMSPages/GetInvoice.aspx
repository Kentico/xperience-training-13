<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_CMSPages_GetInvoice"  Codebehind="GetInvoice.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head runat="server" enableviewstate="false">
        <title>Invoice</title>
	    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
        <style type="text/css">
            body
            {
                margin: 0;
                padding: 0;
            }
        </style>
    </head>
    <body <%= QueryHelper.GetBoolean("print", false) ? "onload='window.print();'" : "" %>>
        <asp:Literal runat="server" ID="ltlInvoice" EnableViewState="false" />
    </body>
</html>
