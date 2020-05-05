using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;

public partial class CMSAdminControls_ContentRating_Controls_DropDown : AbstractRatingControl
{
    /// <summary>
    /// Enables/disables rating scale
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return btnSubmit.Enabled;
        }
        set
        {
            btnSubmit.Enabled = value;
            drpRatings.Enabled = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ReloadData();
    }


    /// <summary>
    /// Returns current rating.
    /// </summary>
    public override double GetCurrentRating()
    {
        if (MaxRating <= 0)
        {
            CurrentRating = 0;
        }
        else
        {
            CurrentRating = (double)ValidationHelper.GetInteger(drpRatings.SelectedValue, 0) / MaxRating;
        }

        return CurrentRating;
    }


    public override void ReloadData()
    {
        drpRatings.Items.Clear();

        // Insert '(none)' when external management is used (the ability to send message without rating)
        if (ExternalManagement)
        {
            drpRatings.Items.Add(new ListItem(ResHelper.GetString("general.selectnone"), "0"));
        }

        int currPos = Convert.ToInt32(Math.Round(CurrentRating * MaxRating, MidpointRounding.AwayFromZero));
        for (int i = 1; i <= MaxRating; i++)
        {
            drpRatings.Items.Add(new ListItem(i.ToString(), i.ToString()));
            if (i == currPos)
            {
                drpRatings.SelectedIndex = drpRatings.Items.Count - 1;
            }
        }

        if (Enabled)
        {
            btnSubmit.Text = ResHelper.GetString("general.ok");
            btnSubmit.Click += new EventHandler(btnSubmit_Click);
        }

        // Hide button when control is disabled or external management is used
        btnSubmit.Visible = Enabled && !ExternalManagement;
    }


    private void btnSubmit_Click(object sender, EventArgs e)
    {
        // Actualize CurrentRating property
        GetCurrentRating();
        // Throw the rating event
        OnRating();
    }
}