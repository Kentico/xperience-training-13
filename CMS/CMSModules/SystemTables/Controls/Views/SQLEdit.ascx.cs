using System;
using System.Data;

using CMS.Helpers;

using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.IO;
using CMS.UIControls;


public partial class CMSModules_SystemTables_Controls_Views_SQLEdit : SqlEditControl
{
    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Set max length of object name (according to development mode)
            if (SystemContext.DevelopmentMode)
            {
                txtObjName.MaxLength = 128;
            }
            else
            {
                if (IsView == true)
                {
                    txtObjName.MaxLength = 128 - VIEW_CUSTOM_PREFIX.Length;
                }
                else
                {
                    txtObjName.MaxLength = 128 - PROCEDURE_CUSTOM_PREFIX.Length;
                }
            }


            btnOk.Visible = !HideSaveButton;

            ShowWarning();
        }
    }


    #region "Event handlers"

    /// <summary>
    /// Generate default query.
    /// </summary>
    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        switch (IsView)
        {
            case true:
                txtObjName.Text = (SystemContext.DevelopmentMode ? VIEW_CUSTOM_PREFIX : String.Empty) + "MyView";
                txtSQLText.Text = "SELECT * FROM CMS_Document";
                break;

            case false:
                txtObjName.Text = (SystemContext.DevelopmentMode ? PROCEDURE_CUSTOM_PREFIX : String.Empty) + "MyProcedure";
                txtParams.Text = " @MyIntegerVar int," + Environment.NewLine +
                                 " @MyStringVar nvarchar(50)";
                txtSQLText.Text = "SELECT 1";
                break;
        }
    }


    /// <summary>
    /// Saves data of edited or new query into DB.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        SaveObject();
    }


    /// <summary>
    /// Initializes the controls. Returns false if parsing code of existing view/procedure failed.
    /// </summary>
    public bool SetupControl()
    {
        return SetupControl(null, null);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the controls. Returns false if parsing code of existing view/procedure failed.
    /// </summary>
    private bool SetupControl(string objectCreateCode, string indexesCreateCode)
    {
        bool result = true;

        if (!String.IsNullOrEmpty(ObjectName) && String.IsNullOrEmpty(objectCreateCode))
        {
            if (IsView != null)
            {
                var tm = new TableManager(null);

                objectCreateCode = tm.GetCode(ObjectName);

                DataSet indexesDs = tm.GetIndexes(ObjectName);
                if (!DataHelper.DataSourceIsEmpty(indexesDs))
                {
                    indexesCreateCode = 
                        indexesDs.Tables[0].Rows
                            .Cast<DataRow>()
                            .Select(dr => ValidationHelper.GetString(dr["CreateScript"], "").Trim())
                            .ToArray().Join(Environment.NewLine + Environment.NewLine);
                }
            }
        }

        if (IsView == true)
        {
            chkWithBinding.Visible = true;
            plcGenerate.Visible = true;

            if (objectCreateCode == null)
            {
                lblCreateLbl.Text = "CREATE VIEW " + (!SystemContext.DevelopmentMode ? VIEW_CUSTOM_PREFIX : String.Empty);
                plcGenerate.Visible = true;
                
                State = SqlEditModeEnum.CreateView;
            }
            else
            {
                lblCreateLbl.Text = "ALTER VIEW";
                plcGenerate.Visible = false;
                
                string name, body;
                bool withbinding;
                result = ParseView(objectCreateCode, out name, out body, out withbinding);
                
                txtObjName.Enabled = false;
                txtObjName.ReadOnly = true;
                if (result)
                {
                    txtObjName.Text = name.Trim();
                    txtSQLText.Text = body.Trim();
                }
                if (!string.IsNullOrEmpty(indexesCreateCode))
                {
                    txtIndexes.Text = indexesCreateCode.Trim();
                }

                chkWithBinding.Checked = withbinding;
                
                State = SqlEditModeEnum.AlterView;
            }

            plcParams.Visible = false;
            chkWithBinding_CheckedChanged(null, null);

            lblBegin.Text = "AS";
        }
        else
        {
            if (objectCreateCode == null)
            {
                lblCreateLbl.Text = "CREATE PROCEDURE " + (!SystemContext.DevelopmentMode ? PROCEDURE_CUSTOM_PREFIX : String.Empty);
                plcGenerate.Visible = true;
                State = SqlEditModeEnum.CreateProcedure;
            }
            else
            {
                plcGenerate.Visible = false;
                lblCreateLbl.Text = "ALTER PROCEDURE";
                string name, param, body;
                result = ParseProcedure(objectCreateCode, out name, out param, out body);
                txtObjName.Enabled = false;
                txtObjName.ReadOnly = true;
                if (result)
                {
                    txtObjName.Text = name.Trim();
                    txtParams.Text = param.Trim();
                    txtSQLText.Text = body.Trim();
                }
                State = SqlEditModeEnum.AlterProcedure;
            }
            plcParams.Visible = true;
            lblBegin.Text = "AS<br/>BEGIN";
            lblEnd.Text = "END";
        }

        if (!result)
        {
            // Parsing code failed => disable all controls
            DisableControl(txtObjName);
            DisableControl(txtParams);
            txtSQLText.EditorMode = EditorModeEnum.Basic;
            DisableControl(txtSQLText);
            btnGenerate.Enabled = false;

            ShowWarning(GetString((IsView == true) ? "systbl.view.parsingfailed" : "systbl.proc.parsingfailed"));
        }

        FailedToLoad = !result;
        return result;
    }


    private void DisableControl(TextBox txt)
    {
        txt.ReadOnly = true;
        txt.Enabled = false;
    }


    /// <summary>
    /// Runs edited view or stored procedure.
    /// </summary>
    public void SaveObject()
    {
        string objName = txtObjName.Text.Trim();
        string body = txtSQLText.Text.Trim();
        string indexes = txtIndexes.Text.Trim();

        // Prepare parameters for stored procedure
        string param = txtParams.Text.Trim();

        bool binding = chkWithBinding.Checked;

        var result = SaveObject(objName, param, body, binding, indexes);

        // Show error message if any
        if (!String.IsNullOrEmpty(result))
        {
            ShowError(result);
        }
    }


    /// <summary>
    /// Loads code of original view/stored procedure from SQL script.
    /// </summary>
    public void Rollback()
    {
        var state = State;

        if ((state == SqlEditModeEnum.AlterView) || (state == SqlEditModeEnum.AlterProcedure))
        {
            string filePath;

            if (SQLScriptExists(ObjectName, out filePath))
            {
                if (!String.IsNullOrEmpty(filePath))
                {
                    string code = File.ReadAllText(filePath);

                    // $ doesn't match carriage return, so \r?$ is used as a workaround: http://msdn.microsoft.com/en-us/library/yd1hzczs.aspx#Multiline
                    string[] statements = RegexHelper.GetRegex("^GO\r?$", RegexOptions.Multiline).Split(code);

                    var createObjectStatement = FindCreateObjectStatement(statements);

                    if (createObjectStatement != null)
                    {
                        var createIndexesStatements = IsView == true ? FindCreateIndexesStatements(statements) : null;
                        SetupControl(createObjectStatement, createIndexesStatements);
                        txtObjName.Text = ObjectName;
                    }
                }
                else
                {
                    // Rollback object from system
                    string indexes;
                    string query = SqlGenerator.GetSystemViewSqlQuery(ObjectName, out indexes);

                    SetupControl(query, indexes);
                }

                // Update current object
                SaveObject();
                SetupControl();
            }
            else
            {
                ShowError(GetString("systbl.unabletorollback"));
            }
        }
    }


    private string FindCreateIndexesStatements(string[] statements)
    {
        var createIndexRegex = RegexHelper.GetRegex("CREATE.*INDEX", true);
        return string.Join(Environment.NewLine, statements.Where(s => createIndexRegex.IsMatch(s)));
    }


    private string FindCreateObjectStatement(string[] statements)
    {
        var createObjectRegex = RegexHelper.GetRegex((IsView == true) ? "\\s*CREATE\\s+VIEW\\s+" : "\\s*CREATE\\s+PROCEDURE\\s+", true);

        string createObjectQuery = statements.FirstOrDefault(createObjectRegex.IsMatch);
        return createObjectQuery;
    }


    protected void chkWithBinding_CheckedChanged(object sender, EventArgs e)
    {
        plcIndexes.Visible = chkWithBinding.Checked;
    }

    #endregion
}