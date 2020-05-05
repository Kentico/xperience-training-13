using System.Web.UI;

using CMS.UIControls;

#pragma warning disable BH3501 // Should inherit from some abstract CMS UI WebPart.
public partial class CMSModules_AdminControls_Controls_UIControls_DialogFooter : CMSUserControl, IDialogFooter
#pragma warning restore BH3501 // Should inherit from some abstract CMS UI WebPart.
{
    /// <summary>
    /// Hides cancel button
    /// </summary>
    public void HideCancelButton()
    {
        btnCancel.Visible = false;
    }


    /// <summary>
    /// Makes sure a cancel button is rendered on the page.
    /// </summary>
    public void ShowCancelButton()
    {
        btnCancel.Visible = true;
    }


    /// <summary>
    /// Adds a control to the footer.
    /// </summary>
    /// <param name="control">Control to be added.</param>
    public void AddControl(Control control)
    {
        plcContent.Append(control);
    }
}
