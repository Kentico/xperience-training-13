
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.UIControls;


[Help("onlinemarketing_account_new", "helptopic")]
[Breadcrumbs]
[Breadcrumb(0, "om.account.list", "~/CMSModules/ContactManagement/Pages/Tools/Account/List.aspx?{%QueryString|(encode)false%}", null)]
[Breadcrumb(1, "om.account.new")]

[UIElement(ModuleName.CONTACTMANAGEMENT, "Accounts")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_New : CMSContactManagementPage
{
}