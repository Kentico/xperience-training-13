using System;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;

public partial class CMSMasterPages_LiveSite_EmptyPage : AbstractMasterPage
{
    #region "Properties"

    /// <summary>
    /// Prepared for specifying the additional HEAD elements.
    /// </summary>
    public override Literal HeadElements
    {
        get
        {
            return ltlHeadElements;
        }
        set
        {
            ltlHeadElements = value;
        }
    }


    /// <summary>
    /// Body element control
    /// </summary>
    public override HtmlGenericControl Body
    {
        get
        {
            return bodyElem;
        }
    }


    /// <summary>
    /// Gets the labels container.
    /// </summary>
    public override PlaceHolder PlaceholderLabels
    {
        get
        {
            return plcLabels;
        }
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        PageStatusContainer = plcStatus;
        bodyElem.Attributes["class"] = mBodyClass;
    }
}