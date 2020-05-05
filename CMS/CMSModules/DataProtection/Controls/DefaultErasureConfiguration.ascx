<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultErasureConfiguration.ascx.cs" Inherits="CMSModules_DataProtection_Controls_DefaultErasureConfiguration" %>

<div class="cms-bootstrap">
    <div class="form-horizontal">
        <cms:LocalizedHeading runat="server" Level="4" ResourceString="dataprotection.app.deletedata.contactdata" />
        <cms:CMSCheckBox ID="chbDeleteContacts" runat="server" ResourceString="dataprotection.app.deletedata.contacts" />
        <cms:CMSCheckBox ID="chbDeleteContactFromAccounts" runat="server" ResourceString="dataprotection.app.deletedata.contactfromaccounts"/>
        <cms:CMSCheckBox ID="chbDeleteSubscriptionFromNewsletters" runat="server" ResourceString="dataprotection.app.deletedata.subscriptionfromnwsletters" />
        <cms:CMSCheckBox ID="chbDeleteActivities" runat="server" ResourceString="dataprotection.app.deletedata.activities" />
        <cms:CMSCheckBox ID="chbDeleteSubmittedFormsActivities" runat="server" ResourceString="dataprotection.app.deletedata.submittedformsactivities" />
        <cms:CMSCheckBox ID="chbDeleteSubmittedFormsData" runat="server" ResourceString="dataprotection.app.deletedata.submittedformsdata" />
    </div>
    
    <div class="form-horizontal">
        <cms:LocalizedHeading runat="server" Level="4" ResourceString="dataprotection.app.deletedata.cutomerdata" />
        <cms:CMSCheckBox ID="chbDeleteCustomers" runat="server" ResourceString="dataprotection.app.deletedata.customers" />
        <div class="control-group-inline">    
            <cms:CMSCheckBox ID="chbDeleteOrdersOlderThanYears" runat="server" ResourceString="dataprotection.app.deletedata.ordersolderthan" />
            <cms:CMSTextBox ID="txtNumberOfYears" runat="server" CssClass="input-width-15 input-sm input-number" MaxLength="2" />
            <cms:LocalizedLabel ID="ltlYears" runat="server" ResourceString="dataprotection.app.deletedata.years" CssClass="checkbox" />
        </div>
        <cms:CMSCheckBox ID="chbDeleteShoppingCarts" runat="server" ResourceString="dataprotection.app.deletedata.shoppingcarts" />
        <cms:CMSCheckBox ID="chbDeleteWishlists" runat="server" ResourceString="dataprotection.app.deletedata.wishlists" />
    </div>
    
    <script type="text/javascript">
        $cmsj(document).ready(function () {
            var textbox = $cmsj('#<%= txtNumberOfYears.ClientID %>');
            var checkbox = $cmsj('#<%= chbDeleteOrdersOlderThanYears.ClientID %>');
            
            textbox.keypress(function () {
                checkbox.prop('checked', true);
            });

            textbox.change(function () {
                if (textbox.val()) {
                    checkbox.prop('checked', true);
                } else {
                    checkbox.prop('checked', false);
                }
            });
        });
    </script>
</div>