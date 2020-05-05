using System;

using CMS;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


[assembly: RegisterCustomClass("TransformationTabsExtender", typeof(TransformationTabsExtender))]

/// <summary>
/// Extender for SKU tab of product document detail
/// </summary>
public class TransformationTabsExtender : UITabsExtender
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
            return mTransInfo ?? (mTransInfo = GetTransformation());
        }
        set
        {
            mTransInfo = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Gets the transformation object for current context
    /// </summary>
    private TransformationInfo GetTransformation()
    {
        var ti = TransformationInfoProvider.GetTransformation(QueryHelper.GetInteger("objectid", 0));
        if (ti == null)
        {
            // Load transformation by name
            ti = TransformationInfoProvider.GetTransformation(QueryHelper.GetString("objectid", ""));

            // Setup context properly
            var ctx = Control.UIContext;

            ctx.EditedObject = ti;

            if (ti != null)
            {
                ctx.ObjectID = ti.TransformationID;
            }
        }
            
        return ti;
    }


    public override void OnInit()
    {
        base.OnInit();

        // Check for selector ID
        string selector = QueryHelper.GetControlClientId("selectorid", String.Empty);

        if (!string.IsNullOrEmpty(selector))
        {
            ScriptHelper.RegisterWOpenerScript(Control.Page);

            // Add selector refresh
            string script = $@"
if (wopener) {{ 
    wopener.US_SelectNewValue_{selector}('{TransInfo.TransformationFullName}'); 
}}
";

            ScriptHelper.RegisterStartupScript(Control.Page, GetType(), "UpdateSelector", script, true);
        }
    }


    /// <summary>
    /// Initialization of tabs.
    /// </summary>
    public override void OnInitTabs()
    {
        Control.OnTabCreated += OnTabCreated;
    }


    protected void OnTabCreated(object sender, TabCreatedEventArgs e)
    {
        if (e.Tab == null)
        {
            return;
        }

        var tab = e.Tab;
        var element = e.UIElement;

        var ti = TransInfo;
        var hier = (ti != null) && ti.TransformationIsHierarchical;

        switch (element.ElementName.ToLowerInvariant())
        {
            case "transformation.transformations":
                if (!hier)
                {
                    e.Tab = null;
                }
                break;

            case "transformation.general":
                if (hier)
                {
                    // Hide general tab when only code editing enabled
                    var editOnlyCode = QueryHelper.GetBoolean("editonlycode", false);
                    if (editOnlyCode)
                    {
                        e.Tab = null;
                        return;
                    }

                    // Get original query string
                    var query = URLHelper.GetQuery(tab.RedirectUrl);
                    var url = URLHelper.AppendQuery("~/CMSModules/DocumentTypes/Pages/Development/HierarchicalTransformations_General.aspx", query);

                    url = Control.HandleTabQueryString(url, element);
                    
                    tab.RedirectUrl = UrlResolver.ResolveUrl(url);
                }
                break;

            case "transformation.theme":
                if (hier)
                {
                    e.Tab = null;
                }
                break;
        }
    }

    #endregion
}
