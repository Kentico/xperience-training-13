using System;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Content_FormControls_Relationships_RelationshipConfiguration : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            radCurrentDoc.Enabled = value;
            radNoRel.Enabled = value;
            radDocWithNodeID.Enabled = value;
            txtNodeID.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (radNoRel.Checked)
            {
                return Guid.Empty;
            }
            if (radCurrentDoc.Checked)
            {
                return new Guid("11111111-1111-1111-1111-111111111111");
            }
            return txtNodeID.Text;
        }
        set
        {
            // No relationship
            if (ValidationHelper.GetGuid(value, Guid.Empty) == Guid.Empty)
            {
                radNoRel.Checked = true;
                radCurrentDoc.Checked = false;
                radDocWithNodeID.Checked = false;
                txtNodeID.Enabled = false;
            }
            else // Relationship with curren document
            {
                if (ValidationHelper.GetGuid(value, Guid.Empty) == new Guid("11111111-1111-1111-1111-111111111111"))
                {
                    radCurrentDoc.Checked = true;
                    radNoRel.Checked = false;
                    radDocWithNodeID.Checked = false;
                    txtNodeID.Enabled = false;
                }
                else // Selected relationship GUID
                {
                    radDocWithNodeID.Checked = true;
                    radNoRel.Checked = false;
                    radCurrentDoc.Checked = false;
                    txtNodeID.Enabled = true;
                    txtNodeID.Text = Convert.ToString(value);
                }
            }
        }
    }


    /// <summary>
    /// Gets ClientID of the textbox with relationship guid.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return txtNodeID.ClientID;
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        if ((!radDocWithNodeID.Checked) || ((radDocWithNodeID.Checked) && (ValidationHelper.IsGuid(txtNodeID.Text))))
        {
            return true;
        }
        else
        {
            ValidationError = GetString("RelationshipConfiguration.ValidationError");
            return false;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            radNoRel.Checked = true;
        }
        // Initialize javascript functionality
        radNoRel.Attributes.Add("OnClick", "RadiobuttonChange()");
        radCurrentDoc.Attributes.Add("OnClick", "RadiobuttonChange()");
        radDocWithNodeID.Attributes.Add("OnClick", "RadiobuttonChange()");
        ltlScript.Text = ScriptHelper.GetScript("var radValueElem = document.getElementById('" + radDocWithNodeID.ClientID + "');" + " var txtValueElem = document.getElementById('" + txtNodeID.ClientID + "');");

        // Enable/diable textbox
        txtNodeID.Enabled = radDocWithNodeID.Checked;
    }

    #endregion
}