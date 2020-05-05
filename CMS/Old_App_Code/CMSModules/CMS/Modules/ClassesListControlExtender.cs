using System;
using System.Collections.Generic;

using CMS;
using CMS.DataEngine;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;

[assembly: RegisterCustomClass("ClassesListControlExtender", typeof(ClassesListControlExtender))]

/// <summary>
/// Permission list control extender
/// </summary>
public class ClassesListControlExtender : ControlExtender<UniGrid>
{
    /// <summary>
    /// Gets the current resource
    /// </summary>
    public ResourceInfo Resource
    {
        get
        {
            return Control.UIContext.EditedObjectParent as ResourceInfo;
        }
    }


    /// <summary>
    /// OnInit event handler
    /// </summary>
    public override void OnInit()
    {
        Control.OnAction += OnAction;
        Control.OnExternalDataBound += OnExternalDataBound;

        // Show warning when module is not editable
        Control.Load += (sender, args) =>
        {
            if ((Resource != null) && (Resource.ResourceID > 0) && !Resource.IsEditable)
            {
                Control.ShowInformation(Control.GetString("resource.classesinstalledresourcewarning"));
            }
        };
    }


    /// <summary>
    /// OnExternalDataBound event handler
    /// </summary>
    private object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerInvariant())
        {
            // Disable delete and clone options when module is not editable
            case "delete":
                ((CMSGridActionButton)sender).Enabled = (Resource != null) && (Resource.ResourceID > 0) && Resource.IsEditable;
                break;
            case "#objectmenu":
                ((CMSGridActionButton)sender).Visible = (Resource != null) && (Resource.ResourceID > 0) && Resource.IsEditable;
                break;
        }

        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    private void OnAction(string actionName, object actionArgument)
    {
        var classId = ValidationHelper.GetInteger(actionArgument, 0);

        switch (actionName)
        {
            case "delete":
                var databaseDependencies = DataClassInfoProvider.CheckDatabaseDependencies(classId);
                if (databaseDependencies.Count > 0)
                {
                    Control.ShowError(CreateDatabaseDependenciesErrorMessage(databaseDependencies));
                    return;
                }

                try
                {
                    DataClassInfoProvider.DeleteDataClassInfo(classId);
                }
                catch (Exception ex)
                {
                    Control.ShowError(Control.GetString("sysdev.class_list.delete.error"), ex.Message);
                }
                break;
        }
    }


    /// <summary>
    /// Creates error message which contains dependant database object names.
    /// </summary>
    /// <param name="databaseDependencies"></param>
    private string CreateDatabaseDependenciesErrorMessage(List<string> databaseDependencies)
    {
        const string separator = "<br />";
        databaseDependencies = databaseDependencies.ConvertAll(HTMLHelper.HTMLEncode);

        return string.Format("{0}{1}{2}",
            Control.GetString("sysdev.class_list.delete.hasdatabasedependencies"),
            separator,
            TextHelper.Join(separator, databaseDependencies));
    }
}