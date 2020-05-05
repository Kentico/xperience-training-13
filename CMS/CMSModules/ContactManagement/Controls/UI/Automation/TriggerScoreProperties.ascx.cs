using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSModules_ContactManagement_Controls_UI_Automation_TriggerScoreProperties : FormEngineUserControl
{
    #region "Variables"

    private ObjectParameters mParameters = new ObjectParameters();

    #endregion


    #region "Properties"

    /// <summary>
    /// Parameters of score trigger in XML.
    /// </summary>
    public override object Value
    {
        get
        {
            mParameters["ScoreValue"] = txtScore.Value;
            return mParameters;
        }
        set
        {
            if (value != null)
            {
                mParameters = value as ObjectParameters;
                txtScore.Value = mParameters["ScoreValue"];
            }
        }
    }


    /// <summary>
    /// Whether the parameters are set.
    /// </summary>
    public override bool HasValue
    {
        get
        {
            string value = txtScore.Value.ToString();
            bool isValidLongNumber = (value.Length > 1) && !value.Substring(1).Contains("-");
            bool isValidSingleDigit = (value.Length == 1) && value != "-";
            return !string.IsNullOrEmpty(value) && (isValidLongNumber || isValidSingleDigit);
        }
    }

    #endregion
}