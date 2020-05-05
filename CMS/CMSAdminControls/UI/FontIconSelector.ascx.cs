using System;
using System.Collections.Generic;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.UIControls;

/// <summary>
/// Form control providing functionality for defining an image by a meta file or a font icon.
/// </summary>
public partial class CMSAdminControls_UI_FontIconSelector : FormEngineUserControl
{
    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return hdnIconClass.Value;
        }
        set
        {
            hdnIconClass.Value = ValidationHelper.GetString(value, null);
        }
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        InitializeFontIconSelector();
    }


    private void InitializeFontIconSelector()
    {
        txtIconClass.ToolTip = GetString("fontIconCss.tooltip");

        rptFontIcons.DataSource = GetIconClassList();
        rptFontIcons.DataBind();

        ScriptHelper.RegisterJQueryUI(Page, false);
        ScriptHelper.RegisterModule(Page, "CMS/FontIconSelector", new
        {
            selectorId = pnlIconSelector.ClientID,
            hiddenIconClassId = hdnIconClass.ClientID
        });
    }


    private IEnumerable<string> GetIconClassList()
    {
        string[] iconsClassFilePaths = new string[] {
            "~/App_Themes/Default/Custom/icon-css-classes.txt",
            "~/App_Themes/Default/Fonts/icon-css-classes.txt"
        };

        var iconCssClasses = new List<string>();

        foreach (string filePath in iconsClassFilePaths)
        {
            string absoluteFilePath = Server.MapPath(filePath);

            if (File.Exists(absoluteFilePath))
            {
                using (StreamReader stream = StreamReader.New(absoluteFilePath))
                {
                    string iconCssClass;

                    while ((iconCssClass = stream.ReadLine()) != null)
                    {
                        if (!String.IsNullOrWhiteSpace(iconCssClass))
                        {
                            iconCssClasses.Add(iconCssClass);
                        }
                    }
                }
            }
        }

        return iconCssClasses;
    }
}