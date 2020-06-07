using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.HtmlControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.MacroEngine.Internal;
using CMS.Synchronization;
using CMS.UIControls;


public partial class CMSModules_System_Macros_System_MacroReport : GlobalAdminPage, IUniPageable
{
    /// <summary>
    /// Structure holding macro expression for the list
    /// </summary>
    private class MacroExpr
    {
        public MacroIdentityOption SignedBy;
        public string Expression;
        public bool SignatureValid;
        public bool Error;
        public string ObjectType;
        public int ObjectID;
        public string Field;
        public string RuleText;
        public string ErrorMessage;
        public IEnumerable<MethodNotFoundResult> MembersIssues = Enumerable.Empty<MethodNotFoundResult>();
    }


    private int mTotalItems;


    /// <summary>
    /// True, if the data is currently displayed
    /// </summary>
    private bool DisplayData
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DataDisplayed"], false);
        }

        set
        {
            ViewState["DataDisplayed"] = value;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        pagerItems.PagedControl = this;
        pagerItems.ShowDirectPageControl = false;

        up.ProgressText = GetString("Macros.Collecting");
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        StartSearch();
    }


    protected void btnView_Click(object sender, EventArgs e)
    {
        pagerItems.UniPager.CurrentPage = 1;

        DisplayData = true;
    }


    private void StartSearch()
    {
        DisplayData = false;

        lblInfo.ResourceString = "macroreport.loading";
        lblInfo.Visible = true;

        plcItems.Visible = false;

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "Postback_" + btnView.ClientID, ControlsHelper.GetPostBackEventReference(btnView), true);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsPostBack())
        {
            StartSearch();
        }

        if (DisplayData)
        {
            plcItems.Visible = true;
            ReloadData();
        }
    }


    private void ReloadData()
    {
        // Prepare the indexes for paging
        int pageSize = pagerItems.CurrentPageSize;

        int startIndex = (pagerItems.CurrentPage - 1) * pageSize;
        int endIndex = startIndex + pageSize;

        bool anyRows = false;

        // Process the macros
        try
        {
            int maxRecords = pageSize == -1 ? -1 : (pageSize * (pagerItems.CurrentPage + 11));
            var macros = GetMacros(startIndex, endIndex, maxRecords, out mTotalItems)
                .ToList();

            anyRows = macros.Any();
            macros.ForEach(RenderItem);
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);

            Service.Resolve<IEventLogService>().LogException("Macro report", "GETMACROS", ex);
        }

        plcItems.Visible = anyRows;
        lblInfo.Visible = !anyRows;

        // Call page binding event
        OnPageBinding?.Invoke(this, null);
    }


    /// <summary>
    /// Gets the macros from the system
    /// </summary>
    /// <param name="startIndex">Start index</param>
    /// <param name="endIndex">End index</param>
    /// <param name="maxTotalRecords">Maximum number of total records to process</param>
    /// <param name="totalRecords">Returns the total number of records found</param>
    private IEnumerable<MacroExpr> GetMacros(int startIndex, int endIndex, int maxTotalRecords, out int totalRecords)
    {
        var index = 0;

        var textToSearch = txtTextToSearch.Text;
        var searchByText = !String.IsNullOrEmpty(textToSearch);
        var reportProblems = chkReportProblems.Checked;
        var type = drpType.Text;

        var result = new List<MacroExpr>();

        foreach (var objectType in GetObjectTypes())
        {
            // Skip certain object types
            switch (objectType)
            {
                case ObjectVersionHistoryInfo.OBJECT_TYPE:
                case VersionHistoryInfo.OBJECT_TYPE:
                case StagingTaskInfo.OBJECT_TYPE:
                case IntegrationTaskInfo.OBJECT_TYPE:
                    continue;
            }

            // Process all objects of the given type
            var infos = new ObjectQuery(objectType)
                .TopN(maxTotalRecords)
                .BinaryData(false);

            var typeInfo = infos.TypeInfo;

            // Search particular expression or search macros of specific type
            infos.WhereAnyColumnContains(searchByText ? textToSearch : "{" + type);

            Action<DataRow> collectMacros = dr =>
            {
                // Process all expressions
                MacroProcessor.ProcessMacros(new DataRowContainer(dr), typeInfo, (context, colName) =>
                {
                    // Get original macro text with hash
                    var expression = context.GetOriginalExpression();
                    var isLocalizationMacro = MacroProcessor.IsLocalizationMacro(expression);

                    string originalExpression = MacroProcessor.RemoveMacroBrackets(expression, out _);
                    string processedExpression = context.Expression;

                    var columnAdapter = context.GetMacroProcessingColumnAdapter();
                    if (columnAdapter != null)
                    {
                        originalExpression = columnAdapter.Preprocess(originalExpression);
                        processedExpression = columnAdapter.Preprocess(processedExpression);
                    }

                    MacroExpr e = null;
                    bool add = false;

                    if (!searchByText || (originalExpression.IndexOf(textToSearch, StringComparison.InvariantCultureIgnoreCase) >= 0))
                    {
                        // If not tracking errors, count immediately
                        if (!reportProblems)
                        {
                            // Apply paging. (endIndex is -1 when paging is off)
                            if ((endIndex < 0) || ((index >= startIndex) && (index < endIndex)))
                            {
                                e = GetMacroExpr(originalExpression, processedExpression, isLocalizationMacro);
                                add = true;
                            }

                            index++;
                        }
                        else
                        {
                            e = GetMacroExpr(originalExpression, processedExpression, isLocalizationMacro);

                            // Filter invalid signature / syntax
                            if (!e.SignatureValid || e.Error || e.MembersIssues.Any())
                            {
                                // Apply paging. (endIndex is -1 when paging is off)
                                if ((endIndex < 0) || ((index >= startIndex) && (index < endIndex)))
                                {
                                    add = true;
                                }

                                index++;
                            }
                        }
                    }

                    if (add)
                    {
                        // Fill in the object information
                        e.ObjectType = objectType;
                        e.ObjectID = (typeInfo.IDColumn == ObjectTypeInfo.COLUMN_NAME_UNKNOWN) ? 0 : ValidationHelper.GetInteger(dr[typeInfo.IDColumn], 0);
                        e.Field = colName;

                        result.Add(e);
                    }

                    return context.Expression;
                }, new List<string> { type });

                if ((maxTotalRecords != -1) && (index >= maxTotalRecords))
                {
                    // Enough data - cancel enumeration
                    throw new ActionCancelledException();
                }
            };

            using (var scope = new CMSConnectionScope())
            {
                scope.CommandTimeout = ConnectionHelper.LongRunningCommandTimeout;

                infos.ForEachRow(collectMacros);
            }

            if (((maxTotalRecords != -1) && (index >= maxTotalRecords)) || !CMSHttpContext.Current.Response.IsClientConnected)
            {
                break;
            }
        }

        totalRecords = index;
        return result;
    }


    private IEnumerable<string> GetObjectTypes()
    {
        // Get object types to search
        var selectedType = ValidationHelper.GetString(selObjectType.Value, "");
        if (!String.IsNullOrEmpty(selectedType) && (ObjectTypeManager.GetTypeInfo(selectedType) != null))
        {
            return new List<string> { selectedType };
        }

        return ObjectTypeManager.ObjectTypesWithMacros;
    }


    private static MacroExpr GetMacroExpr(string originalExpression, string processedExpression, bool isLocalizationMacro)
    {
        var macroExpr = new MacroExpr();

        try
        {
            // Handle security
            macroExpr.Expression = MacroSecurityProcessor.RemoveMacroSecurityParams(originalExpression, out macroExpr.SignedBy);
            macroExpr.SignatureValid = MacroSecurityProcessor.CheckMacroIntegrity(originalExpression, macroExpr.SignedBy);

            // Parse rule text
            macroExpr.RuleText = MacroRuleTree.GetRuleText(macroExpr.Expression, true, true);
            macroExpr.Expression = MacroRuleTree.GetRuleCondition(macroExpr.Expression, true);

            // Macro expression does not support anonymous signature, remove the flag
            if (processedExpression.EndsWith("@", StringComparison.Ordinal))
            {
                processedExpression = processedExpression.Substring(0, processedExpression.Length - 1);
            }

            // Check syntax
            var expr = MacroExpression.ParseExpression(processedExpression, isLocalizationMacro);

            macroExpr.MembersIssues = MacroMethodValidator.Validate(expr);
        }
        catch (Exception ex)
        {
            macroExpr.Error = true;
            macroExpr.ErrorMessage = ex.Message + "\r\n\r\n" + ex.StackTrace;
        }
        return macroExpr;
    }


        private void RenderItem(MacroExpr expression)
    {
        var row = new HtmlTableRow();

        foreach (var cell in GetRowCells(expression))
        {
            row.Controls.Add(cell);
        }

        plcRows.Controls.Add(row);
    }


    private IEnumerable<HtmlTableCell> GetRowCells(MacroExpr expression)
    {
        // Expression
        string exprTag = expression.RuleText ?? HTMLHelper.HTMLEncodeLineBreaks(TextHelper.LimitLength(expression.Expression, 500));

        yield return CreateTableCellWithClass("wrap-normal", $"<span class=\"MacroExpression\" title=\"{HTMLHelper.HTMLEncode(expression.Expression)}\">{exprTag}</span>");

        // Syntax valid
        var errorText = UniGridFunctions.ColoredSpanYesNo(!expression.Error);
        if (!String.IsNullOrEmpty(expression.ErrorMessage))
        {
            errorText = $"<span title=\"{HTMLHelper.HTMLEncode(expression.ErrorMessage)}\">{errorText}</span>";
        }

        yield return CreateTableCellWithClass("text-center", errorText);

        // Signature valid
        yield return new HtmlTableCell { InnerHtml = $"<div class=\"text-center\">{UniGridFunctions.ColoredSpanYesNo(expression.SignatureValid)}</div><div class=\"text-center\">{expression.SignedBy.ToString()}</div>" };

        var anyMembersIssue = expression.MembersIssues.Any();
        var membersMessage = $"<div class=\"text-center\">{UniGridFunctions.ColoredSpanYesNo(!anyMembersIssue)}</div>";
        if (anyMembersIssue)
        {
            membersMessage = $"{membersMessage}{String.Join("<br />", expression.MembersIssues.Select(methodIssue => $"<div class=\"text-center\">{FormatMethodIssue(methodIssue)}</div>"))}";
        }

        // Members valid
        yield return new HtmlTableCell { InnerHtml = membersMessage };

        yield return new HtmlTableCell
        {
            Controls =
            {
                new ObjectTransformation(expression.ObjectType, expression.ObjectID)
                {
                    UseEmptyInfoForObjectLimitedByLicense = true,
                    Transformation = "{% Object.GetFullObjectName(true, true) %}"
                }
            }
        };

        // Column
        yield return new HtmlTableCell { InnerText = expression.Field };
    }

    private string FormatMethodIssue(MethodNotFoundResult result)
    {
        if (result.IsObsolete)
        {
            return HTMLHelper.HTMLEncode(String.Format(GetString("macros.methodobsolete"), result.MethodName, result.ObsoleteMessage));
        }

        if (result.ParametersNotMatch)
        {
            return HTMLHelper.HTMLEncode(String.Format(GetString("macros.methodwithparametersnotfound"), result.MethodName));
        }

        return HTMLHelper.HTMLEncode(String.Format(GetString("macros.methodnotfound"), result.MethodName));
    }


    private HtmlTableCell CreateTableCellWithClass(string cssClass, string innerHtml)
    {
        var cell = new HtmlTableCell
        {
            InnerHtml = innerHtml
        };
        cell.Attributes["class"] = cssClass;

        return cell;
    }


    #region "IUniPageable Members"

    /// <summary>
    /// Pager data item object.
    /// </summary>
    public object PagerDataItem
    {
        get
        {
            return null;
        }
        set
        {
        }
    }


    /// <summary>
    /// Pager control.
    /// </summary>
    public UniPager UniPagerControl
    {
        get;
        set;
    }


    /// <summary>
    /// Occurs when the control bind data.
    /// </summary>
    public event EventHandler<EventArgs> OnPageBinding;


    /// <summary>
    /// Occurs when the pager change the page and current mode is postback => reload data
    /// </summary>
    public event EventHandler<EventArgs> OnPageChanged;


    /// <summary>
    /// Evokes control databind.
    /// </summary>
    public void ReBind()
    {
        OnPageChanged?.Invoke(this, null);
    }


    /// <summary>
    /// Gets or sets the number of result. Enables proceed "fake" datasets, where number 
    /// of results in the dataset is not correspondent to the real number of results
    /// This property must be equal -1 if should be disabled
    /// </summary>
    public int PagerForceNumberOfResults
    {
        get
        {
            return mTotalItems;
        }
        set
        {
        }
    }

    #endregion
}
