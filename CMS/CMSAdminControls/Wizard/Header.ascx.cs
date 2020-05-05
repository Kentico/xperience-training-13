using System;

using CMS.UIControls;


public partial class CMSAdminControls_Wizard_Header : CMSUserControl
{
    /// <summary>
    /// Header title text.
    /// </summary>
    public string Title
    {
        get
        {
            return lblTitle.Text;
        }
        set
        {
            lblTitle.Text = value;
        }
    }


    /// <summary>
    /// Header description text.
    /// </summary>
    public string Description
    {
        get
        {
            return lblDescription.Text;
        }
        set
        {
            lblDescription.Text = value;
        }
    }


    /// <summary>
    /// Header header text.
    /// </summary>
    public string Header
    {
        get
        {
            return headHeader.Text;
        }
        set
        {
            headHeader.Text = value;
        }
    }


    /// <summary>
    /// Header description visible.
    /// </summary>
    public bool DescriptionVisible
    {
        get
        {
            return lblDescription.Visible;
        }
        set
        {
            lblDescription.Visible = value;
        }
    }


    /// <summary>
    /// Header title visible.
    /// </summary>
    public bool TitleVisible
    {
        get
        {
            return lblTitle.Visible;
        }
        set
        {
            if (!value)
            {
                divDescription.Attributes.Remove("class");
            }
            lblTitle.Visible = value;
        }
    }


    /// <summary>
    /// Header header visible.
    /// </summary>
    public bool HeaderVisible
    {
        get
        {
            return headHeader.Visible;
        }
        set
        {
            headHeader.Visible = value;
        }
    }
}