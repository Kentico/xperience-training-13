using System;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;

public partial class CMSFormControls_Basic_TextBoxControl : TextBoxControl, IControlWithSentimentAnalysisComponent
{

    /// <summary>
    /// Indicates whether the sentiment analysis component should be rendered.
    /// </summary>
    public bool RenderSentimentAnalysisComponent { get; set; } = false;


    /// <summary>
    /// Textbox control
    /// </summary>
    protected override CMSTextBox TextBox
    {
        get
        {
            return txtText;
        }
    }


    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        if (IsTextArea && RenderSentimentAnalysisComponent)
        {
            var initFunction = @"
var element =  document.getElementById('" + TextBox.ClientID + @"');
sentimentAnalysisComponent.relatedField = element;
sentimentAnalysisComponent.getText = function() { return element.value };
sentimentAnalysisComponent.dataset.sentimentAnalysisForSelector = '#" + TextBox.ClientID + @"';
element.oninput = function() { sentimentAnalysisComponent.onTextChanged() };";

            var button = new SentimentAnalysisButton(initFunction);
            Controls.Add(button);
        }
    }
}
