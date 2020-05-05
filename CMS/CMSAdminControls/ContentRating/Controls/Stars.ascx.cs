using System;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Helpers;

public partial class CMSAdminControls_ContentRating_Controls_Stars : AbstractRatingControl, IPostBackEventHandler
{
    private string starCssClass = "rating-star cms-icon-80";
    private string emptyStarCssClass  = "icon-star-empty";
    private string filledStarCssClass  = "icon-star-full";
    private string waitingStarCssClass = "saved-rating-star";
    private int ratingValue;

    private string RatingClienId => $"{ClientID}_A";

    private bool RtlDirection { get; set; }

    /// <summary>
    /// Enables/disables rating scale.
    /// </summary>
    public override bool Enabled { get; set; }
    

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
            CurrentRating = (double)ratingValue / MaxRating;
        }
        return CurrentRating;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        RtlDirection = CultureHelper.IsPreferredCultureRTL();
        ReloadData();
    }


    public override void ReloadData()
    {
        if (MaxRating <= 0)
        {
            Visible = false;
            return;
        }

        if (Enabled && !ExternalManagement)
        {
            ratingValue = 0;
        }
        else
        {
            ratingValue = Convert.ToInt32(Math.Round(CurrentRating * MaxRating, MidpointRounding.AwayFromZero));
        }
    }
       

    protected override void OnPreRender(EventArgs e)
    {
        if (Enabled && Visible)
        {
            if (ControlsHelper.IsInUpdatePanel(this))
            {
                ScriptManager.GetCurrent(Page)?.RegisterAsyncPostBackControl(this);
            }

            ScriptHelper.RegisterModule(this, "AdminControls/StarsRating", new
            {
                Id = RatingClienId,
                StarCssClass = starCssClass,
                EmptyStarCssClass = emptyStarCssClass,
                FilledStarCssClass = filledStarCssClass,
                WaitingStarCssClass = waitingStarCssClass,
                UniqueID = UniqueID,
                RatingValue = ratingValue,
                RtlDirection = RtlDirection
            });
        }

        base.OnPreRender(e);
    }


    protected override void Render(HtmlTextWriter writer)
    {
        if (!Enabled)
        {
            writer.AddAttribute("class", "disabled");
        }
        
        writer.RenderBeginTag(HtmlTextWriterTag.Div);

        writer.AddAttribute("id", RatingClienId);
        writer.AddAttribute("style", "text-decoration:none");
        writer.AddAttribute("href", "javascript:void(0)");
        writer.AddAttribute("title", ratingValue.ToString());

        writer.RenderBeginTag(HtmlTextWriterTag.A);

        for (var i = 1; i < MaxRating + 1; i++)
        {
            writer.AddAttribute("id", $"{ClientID}_Star_{i.ToString()}");
            writer.AddStyleAttribute("float", "left");

            var cssClass = ((i <= ratingValue) && !RtlDirection) || ((i > MaxRating - ratingValue) && RtlDirection) ? 
                $"{starCssClass} {filledStarCssClass}" : $"{starCssClass} {emptyStarCssClass}";

            writer.AddAttribute("class", cssClass);

            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write("&nbsp;");

            // SPAN
            writer.RenderEndTag();
        }

        // A
        writer.RenderEndTag();
        // Div
        writer.RenderEndTag();

        base.Render(writer);
    }


    public void RaisePostBackEvent(string eventArgument)
    {
        if (!Enabled && !Visible)
        {
            return;
        }

        int argumentValue;
        if (Int32.TryParse(eventArgument, out argumentValue) && (argumentValue > 0) &&(argumentValue <= MaxRating))
        {
            ControlsHelper.UpdateCurrentPanel(this);

            ratingValue = argumentValue;
            GetCurrentRating();
            
            OnRating();
        }
    }
}
