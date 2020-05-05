using System;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


/// <summary>
/// This form control is used to edit webpart container code.
/// </summary>
public partial class CMSModules_PortalEngine_Controls_WebContainers_WebpartContainerCode : FormEngineUserControl
{
    private const string WP_CHAR = "□";


    #region "Properties"

    /// <summary>
    /// Gets or sets the container value
    /// </summary>
    public override object Value
    {
        get
        {
            return txtContainerText.Text;
        }
        set
        {

        }
    }


    /// <summary>
    /// Returns ExtendedArea object for code editing.
    /// </summary>
    public ExtendedTextArea Editor
    {
        get
        {
            return txtContainerText;
        }
    }


    /// <summary>
    /// Enables or disables the control
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return !txtContainerText.ReadOnly;
        }
        set
        {
            txtContainerText.ReadOnly = !value;
        }
    }

    #endregion


    #region "Page events

    public override object[,] GetOtherValues()
    {
        string text = txtContainerText.Text;
        string after = "";

        int wpIndex = text.IndexOf(WP_CHAR, StringComparison.Ordinal);
        if (wpIndex >= 0)
        {
            after = text.Substring(wpIndex + 1);
            text = text.Substring(0, wpIndex).Replace(WP_CHAR, "");
        }

        object[,] values = new object[2, 2];
        values[0, 0] = "ContainerTextBefore";
        values[0, 1] = text;
        values[1, 0] = "ContainerTextAfter";
        values[1, 1] = after;
        return values;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            LoadOtherValues();
        }
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        txtContainerText.Text = ValidationHelper.GetString(Form.GetDataValue("ContainerTextBefore"), "") + WP_CHAR + ValidationHelper.GetString(Form.GetDataValue("ContainerTextAfter"), "");
    }

    #endregion
}