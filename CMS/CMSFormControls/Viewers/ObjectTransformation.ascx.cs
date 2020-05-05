using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_Viewers_ObjectTransformation : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets object identifier
    /// </summary>
    public override object Value
    {
        get
        {
            return objTrans.ObjectID == 0 ? null : (object)objTrans.ObjectID;
        }
        set
        {
            objTrans.ObjectID = ValidationHelper.GetInteger(value, 0);
        }
    }


    /// <summary>
    /// Gets or sets transformation - Column name (e.g. "ResourceName"), Internal UniGrid transformation (e.g. "#yesno") or a macro (e.g. "{% FirstName %} {% LastName %}")
    /// </summary>
    public string Transformation
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Transformation"), String.Empty);
        }
        set
        {
            SetValue("Transformation", value);
            objTrans.Transformation = value;
        }
    }


    /// <summary>
    /// Gets or sets object type for given object identifier
    /// </summary>
    public string ObjectType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ObjectType"), String.Empty);

        }
        set
        {
            SetValue("ObjectType", value);
            objTrans.ObjectType = value;
        }
    }


    /// <summary>
    /// Indicates if output should be encoded.
    /// </summary>
    public bool EncodeOutput
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EncodeOutput"), true);

        }
        set
        {
            SetValue("EncodeOutput", value);
            objTrans.EncodeOutput = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// PageLoad event handler
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControl();
        }
    }


    /// <summary>
    /// Setups controls.
    /// </summary>
    private void SetupControl()
    {
        objTrans.ContextResolver = ContextResolver;
        objTrans.Transformation = Transformation;
        objTrans.ObjectType = ObjectType;
        objTrans.EncodeOutput = EncodeOutput;
    }

    #endregion
}