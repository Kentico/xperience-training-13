using System;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_DocumentTypes_Pages_Development_HierarchicalTransformations_General : CMSDesignPage
{
    #region "Variables"

    private TransformationInfo mTransInfo;

    #endregion


    #region "Properties"

    /// <summary>
    /// Transformation info object
    /// </summary>
    public TransformationInfo TransInfo
    {
        get
        {
            if (mTransInfo == null)
            {
                mTransInfo = TransformationInfoProvider.GetTransformation(QueryHelper.GetInteger("objectid", 0));
                if (mTransInfo == null)
                {
                    mTransInfo = TransformationInfoProvider.GetTransformation(QueryHelper.GetString("name", ""));
                }
            }
            return mTransInfo;
        }
        set
        {
            mTransInfo = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        ucTransf.TransInfo = TransInfo;
    }
}