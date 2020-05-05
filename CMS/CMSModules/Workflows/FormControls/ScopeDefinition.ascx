<%@ Control Language="C#"  Codebehind="ScopeDefinition.ascx.cs" Inherits="CMSModules_Workflows_FormControls_ScopeDefinition" %>

<%@ Register Src="~/CMSModules/Content/FormControls/Documents/SelectPath.ascx"
    TagName="SelectPath" TagPrefix="cms" %>
<cms:SelectPath runat="server" ID="pathElem" IsLiveSite="false" SinglePathMode="true" />
<cms:CMSRadioButton ID="rbDocAndChildren" runat="server" GroupName="Cover"
    ResourceString="workflowscope.docandchildren" />
<cms:CMSRadioButton ID="rbDoc" runat="server" GroupName="Cover" ResourceString="workflowscope.doc" />
<cms:CMSRadioButton ID="rbChildren" runat="server" GroupName="Cover" ResourceString="workflowscope.children" />
