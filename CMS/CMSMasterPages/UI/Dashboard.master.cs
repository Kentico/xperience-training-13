using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.UIControls;

public partial class CMSMasterPages_UI_Dashboard : CMSMasterPage
{
    /// <summary>
    /// Head elements
    /// </summary>
    public override Literal HeadElements
    {
        get
        {
            return ltlTags;
        }
        set
        {
            ltlTags = value;
        }
    }


    /// <summary>
    /// Page title control
    /// </summary>
    public override PageTitle Title
    {
        get
        {
            return titleElem;
        }
    }


    /// <summary>
    /// Body panel.
    /// </summary>
    public override Panel PanelBody
    {
        get
        {
            return pnlBody;
        }
    }


    /// <summary>
    /// Ensures that for dashboard pages there is not global messages placeholder
    /// </summary>
    protected override MessagesPlaceHolder EnsureMessagesPlaceHolder()
    {
        return null;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        BodyClass += " DashboardMode ContentBody";
    }
}
