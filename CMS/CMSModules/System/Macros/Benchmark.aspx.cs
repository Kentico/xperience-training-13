using System;
using System.Linq;

using CMS.Base.Internal;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.UIControls;


public partial class CMSModules_System_Macros_Benchmark : CMSDebugPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        editorElem.Editor.Language = LanguageEnum.HTMLMixed;
    }


    protected void btnRun_Click(object sender, EventArgs e)
    {
        // Get the number of iterations
        int iterations = ValidationHelper.GetInteger(txtIterations.Text, 0);
        if (iterations <= 0)
        {
            ShowError(GetString("macros.benchmark.invaliditerations"));
            return;
        }

        // Prepare the benchmark
        var benchmark = new Benchmark<string>(Execute)
        {
            SetupFunc = Setup,
            Iterations = iterations,
            TearDownFunc = TearDown
        };

        // Run the benchmark
        benchmark.Run();

        // Append expression to the results
        var text = editorElem.Text;
        var results = benchmark.GetReport();

        results += String.Format(
@"
Evaluated text: 
---------------
{0}
", 
            text
        );

        // Update UI
        editorElem.Text = text;
        txtOutput.Text = benchmark.Result;
        txtResults.Text = results;
    }
    

    /// <summary>
    /// Sets up the benchmark
    /// </summary>
    /// <param name="benchmark">Benchmark</param>
    private void Setup(Benchmark<string> benchmark)
    {
        // No action by default
    }


    /// <summary>
    /// Executes the benchmark run
    /// </summary>
    /// <param name="benchmark">Benchmark</param>
    private string Execute(Benchmark<string> benchmark)
    {
        return MacroResolver.Resolve(editorElem.Text);
    }


    /// <summary>
    /// Cleans up after the benchmark
    /// </summary>
    /// <param name="benchmark">Benchmark</param>
    private void TearDown(Benchmark<string> benchmark)
    {
        // No action by default
    }
}
