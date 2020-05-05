using System;
using System.Collections.Generic;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Protection;
using CMS.SiteProvider;
using CMS.UIControls;


[EditedObject(BadWordInfo.OBJECT_TYPE, "badwordid")]
[UIElement(ModuleName.BADWORDS, "Administration.BadWords")]
public partial class CMSModules_BadWords_BadWords_Edit_General : GlobalAdminPage
{
    #region "Protected variables"

    protected int badWordId = 0;
    protected BadWordInfo badWordObj = null;

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        // Get badWord id from querystring		
        badWordId = QueryHelper.GetInteger("objectid", 0);

        // Must be called after the master page file is set
        base.OnPreInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Initialize resource strings
        rqfWordExpression.ErrorMessage = GetString("general.requiresvalue");

        if (badWordId > 0)
        {
            badWordObj = BadWordInfoProvider.GetBadWordInfo(badWordId);

            // Set edited object
            EditedObject = badWordObj;

            if (badWordObj != null)
            {
                // Fill editing form
                if (!RequestHelper.IsPostBack())
                {
                    LoadData(badWordObj);

                    // Show that the badWord was created or updated successfully
                    if (QueryHelper.GetString("saved", string.Empty) == "1")
                    {
                        ShowChangesSaved();
                    }
                }
                else
                {
                    // Set selected action
                    SetSelectedAction(badWordObj);
                }
            }
        }
        else
        {
            PageTitle.TitleText = GetString("BadWords_Edit.NewItemCaption");
            // Initialize breadcrumbs
            List<BreadcrumbItem> breadCrumbItems = new List<BreadcrumbItem>();
            breadCrumbItems.Add(new BreadcrumbItem
            {
                Text = GetString("badwords_edit.itemlistlink"),
                RedirectUrl = "~/CMSModules/BadWords/BadWords_List.aspx",
                Target = "_self",
            });
            breadCrumbItems.Add(new BreadcrumbItem
            {
                Text = GetString("badwords_list.newitemcaption"),
            });
            PageBreadcrumbs.Items = breadCrumbItems;
            SetSelectedAction(null);
        }

        // Enable / disable actions (depending on inheritance)
        SelectBadWordActionControl.Enabled = !chkInheritAction.Checked;
        txtWordReplacement.Enabled = !chkInheritReplacement.Checked;

        // Show / hide replacement textbox depending on action
        plcReplacement.Visible = ((BadWordActionEnum)Enum.Parse(typeof(BadWordActionEnum), SelectBadWordActionControl.Value.ToString())) == BadWordActionEnum.Replace;
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        string errorMessage = new Validator().NotEmpty(txtWordExpression.Text, GetString("general.requiresvalue")).Result;

        if (errorMessage == string.Empty)
        {
            if (badWordObj == null)
            {
                badWordObj = new BadWordInfo();
            }

            // Set edited object
            EditedObject = badWordObj;

            // If bad word doesn't already exist, create new one
            if (!(((badWordId <= 0) || WordExpressionHasChanged()) && BadWordInfoProvider.BadWordExists(txtWordExpression.Text.Trim())))
            {
                badWordObj.WordExpression = txtWordExpression.Text.Trim();
                BadWordActionEnum action =
                    (BadWordActionEnum)Convert.ToInt32(SelectBadWordActionControl.Value.ToString().Trim());
                badWordObj.WordAction = !chkInheritAction.Checked ? action : 0;
                badWordObj.WordReplacement = (!chkInheritReplacement.Checked && (action == BadWordActionEnum.Replace)) ? txtWordReplacement.Text : null;
                badWordObj.WordLastModified = DateTime.Now;
                badWordObj.WordIsRegularExpression = chkIsRegular.Checked;
                badWordObj.WordMatchWholeWord = chkMatchWholeWord.Checked;

                if (badWordId <= 0)
                {
                    badWordObj.WordIsGlobal = true;
                }

                BadWordInfoProvider.SetBadWordInfo(badWordObj);

                if (badWordId <= 0)
                {
                    string redirectTo = badWordId <= 0 ? UIContextHelper.GetElementUrl("CMS.Badwords", "Administration.BadWords.Edit") : UIContextHelper.GetElementUrl("CMS.Badwords", "Administration.BadWords.Edit.General");
                    redirectTo = URLHelper.AddParameterToUrl(redirectTo, "objectid", badWordObj.WordID.ToString());
                    redirectTo = URLHelper.AddParameterToUrl(redirectTo, "saved", "1");
                    URLHelper.Redirect(redirectTo);
                }
                else
                {
                    ScriptHelper.RefreshTabHeader(Page, txtWordExpression.Text.Trim());
                    ShowChangesSaved();
                }
            }
            else
            {
                ShowError(GetString("badwords_edit.badwordexists"));
            }
        }
        else
        {
            ShowError(errorMessage);
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Determines whedther wordExpression has changed
    /// </summary>
    private bool WordExpressionHasChanged()
    {
        if (badWordObj != null)
        {
            return badWordObj.WordExpression != txtWordExpression.Text.Trim();
        }
        return !String.IsNullOrEmpty(txtWordExpression.Text.Trim());
    }

    /// <summary>
    /// Load data of editing badWord.
    /// </summary>
    /// <param name="badWordObj">BadWord object</param>
    protected void LoadData(BadWordInfo badWordObj)
    {
        // Check inheritance
        chkInheritAction.Checked = (badWordObj.WordAction == 0);
        chkInheritReplacement.Checked = (badWordObj.WordReplacement == null);

        // Set selected action
        SetSelectedAction(badWordObj);

        // Load rest of values
        txtWordExpression.Text = badWordObj.WordExpression;
        chkIsRegular.Checked = badWordObj.WordIsRegularExpression;
        chkMatchWholeWord.Checked = badWordObj.WordMatchWholeWord;
    }


    /// <summary>
    /// Sets selected action.
    /// </summary>
    protected void SetSelectedAction(BadWordInfo badWordObj)
    {
        // Find postback invoker
        string invokerName = Page.Request.Params.Get(postEventSourceID);
        Control invokeControl = !string.IsNullOrEmpty(invokerName) ? Page.FindControl(invokerName) : null;

        // Ensure right postback actions
        if ((invokeControl == chkInheritAction) || !RequestHelper.IsPostBack())
        {
            // Deselect all items
            SelectBadWordActionControl.ReloadData();

            // Check inheritance of settings
            if (chkInheritAction != null)
            {
                // Get action
                if ((!chkInheritAction.Checked) && (badWordObj != null))
                {
                    BadWordActionEnum action = badWordObj.WordAction;
                    SelectBadWordActionControl.Value = ((int)action).ToString();
                }
                else
                {
                    SelectBadWordActionControl.Value = ((int)BadWordsHelper.BadWordsAction(SiteContext.CurrentSiteName)).ToString();
                }
            }
        }

        // Get replacement
        if ((invokeControl == chkInheritReplacement) || !RequestHelper.IsPostBack())
        {
            if (chkInheritReplacement != null)
            {
                if (!chkInheritReplacement.Checked && (badWordObj != null))
                {
                    txtWordReplacement.Text = badWordObj.WordReplacement;
                }
                else
                {
                    txtWordReplacement.Text = BadWordsHelper.BadWordsReplacement(SiteContext.CurrentSiteName);
                }
            }
        }
    }

    #endregion
}