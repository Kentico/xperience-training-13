using System;

using CMS.Synchronization;
using CMS.UIControls;

// Edited object
[EditedObject(IntegrationConnectorInfo.OBJECT_TYPE, "connectorId")]
// Breadcrumbs
[Breadcrumbs()]
[Breadcrumb(0, "integration.connector.list", "~/CMSModules/Integration/Pages/Administration/Connectors/List.aspx", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "integration.connector.new", NewObject = true)]
public partial class CMSModules_Integration_Pages_Administration_Connectors_Edit : CMSIntegrationPage
{

}
