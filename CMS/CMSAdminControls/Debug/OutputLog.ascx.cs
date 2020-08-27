using System;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_Debug_OutputLog : OutputLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        if (Log != null)
        {
            // Get the output
            string output = ValidationHelper.GetString(Log.Value, null);
            if (!String.IsNullOrEmpty(output))
            {
                Visible = true;
                pnlHeading.Visible = DisplayHeader;

                int size = output.Length;               

                lblSize.Text = DataHelper.GetSizeString(size);
                lblSizeCaption.Text = GetSizeChart(size, 0, 0, 0);
                txtOutput.Text = output.Trim();

                // Size chart
                MaxSize = 102400;
                if (size > MaxSize)
                {
                    MaxSize = size;
                }

                TotalSize = size;
            }
        }
    }
}