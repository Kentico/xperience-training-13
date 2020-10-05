using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.UIControls;

[Security(ModuleName.LOCALIZATION, "LocalizeStrings", "Development.Cultures.Site")]
[UIElement(ModuleName.LOCALIZATION, "Development.Cultures.Site")]
public partial class CMSModules_Cultures_Pages_ResourceString_List : CMSDeskPage
{
    #region "Variables"

    /// <summary>
    /// Holds Culture code parameter from url (e.g. "en-US")
    /// </summary>
    private string mCultureCode;


    /// <summary>
    /// CultureInfo based on Culture code parameter from url
    /// </summary>
    private CultureInfo mCultureInfo;


    /// <summary>
    /// Default UI Culture Info
    /// </summary>
    private CultureInfo mDefaultUICultureInfo = CultureInfo.Provider.Get(CultureHelper.DefaultUICultureCode);

    #endregion


    #region "Properties"

    private bool CurrentCultureIsDefault
    {
        get
        {
            return CMSString.Equals(mCultureInfo.CultureCode, CultureHelper.DefaultUICultureCode, true);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// OnInit
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        RequireSite = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        gridStrings.OnAction += UniGridCultures_OnAction;
        gridStrings.OnExternalDataBound += gridStrings_OnExternalDataBound;

        CurrentMaster.DisplaySiteSelectorPanel = true;

        cultureSelector.OnSelectionChanged += cultureSelector_OnSelectionChanged;
        cultureSelector.DropDownSingleSelect.AutoPostBack = true;
        cultureSelector.OnListItemCreated += cultureSelector_OnListItemCreated;

        mCultureCode = QueryHelper.GetString("culturecode", CultureHelper.DefaultUICultureCode);
        mCultureInfo = CultureInfo.Provider.Get(mCultureCode);

        if (mCultureInfo != null && mDefaultUICultureInfo != null)
        {
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@Culture", mCultureInfo.CultureID);
            parameters.AddId("@DefaultCultureID", mDefaultUICultureInfo.CultureID);
            gridStrings.QueryParameters = parameters;

            string defaultTextCaption = String.Format(GetString("culture.defaultwithparameter"), CultureHelper.DefaultUICultureCode);
            gridStrings.GridColumns.Columns.Find(column => column.Source == "DefaultText").Caption = defaultTextCaption;

            if (CurrentCultureIsDefault)
            {
                // Set default translation column to full width
                gridStrings.GridColumns.Columns[2].Width = "100%";

                // Remove 'CultureText' column if current culture is default
                gridStrings.GridColumns.Columns.RemoveAt(1);
            }
            else
            {
                if (!LocalizationHelper.ResourceFileExistsForCulture(mCultureInfo.CultureCode))
                {
                    string url = "https://devnet.kentico.com/download/localization-packs";
                    string downloadPage = String.Format(@"<a href=""{0}"" target=""_blank"" >{1}</a> ", url, HTMLHelper.HTMLEncode(url));
                    ShowInformation(String.Format(GetString("culture.downloadlocalization"), downloadPage));
                }
            }

            InitializeMasterPage();
        }
        else
        {
            gridStrings.StopProcessing = true;
            ShowError(String.Format(GetString("culture.doesntexist"), HTMLHelper.HTMLEncode(mCultureCode)));
        }
    }


    private void cultureSelector_OnListItemCreated(ListItem item)
    {
        if (CMSString.Equals(item.Value, CultureHelper.DefaultUICultureCode, true))
        {
            item.Text += " (" + GetString("general.default").ToLowerInvariant() + ")";
        }
    }


    private void cultureSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        URLHelper.Redirect(UrlResolver.ResolveUrl("List.aspx?culturecode=" + ValidationHelper.GetString(cultureSelector.Value, CultureHelper.DefaultUICultureCode)));
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        cultureSelector.Value = mCultureCode;
    }


    protected void InitializeMasterPage()
    {
        // Set actions
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("culture.newstring"),
            RedirectUrl = ResolveUrl("Edit.aspx?culturecode=" + mCultureCode)
        });
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void UniGridCultures_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "edit":
                URLHelper.Redirect(UrlResolver.ResolveUrl(String.Format("Edit.aspx?stringkey={0}&culturecode={1}", actionArgument, mCultureInfo.CultureCode)));
                break;

            case "delete":
                ResourceStringInfoProvider.DeleteResourceStringInfo(actionArgument.ToString(), mCultureInfo.CultureCode);
                break;

            case "deleteall":
                ResourceStringInfoProvider.DeleteResourceStringInfo(actionArgument.ToString());
                break;
        }
    }


    protected object gridStrings_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerInvariant())
        {
            case "culturetext":
            case "defaulttext":
                {
                    var text = ValidationHelper.GetString(parameter, String.Empty);

                    if (text.IndexOf('{') >= 0)
                    {
                        text = MacroSecurityProcessor.RemoveSecurityParameters(text, true, null);
                    }

                    return text;
                }
        }

        return parameter;
    }

    #endregion
}