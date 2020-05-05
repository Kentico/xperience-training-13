<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecipientContactGroups.ascx.cs" Inherits="CMSModules_Newsletters_Controls_RecipientContactGroups" %>

<% if (Groups.Count > MAX_DISPLAYED) { %>
   <div class="contact-groups" onclick="$cmsj(this).find('.other-contact-groups').toggle(); $cmsj(this).find('.contact-groups-more').toggle();" title="<%= String.Join(", ", Groups.ToArray()) %>">
       <span><% for (int i = 0; i < MAX_DISPLAYED; i++) { %><%= (i == 0 ? "" : ", ") + Groups[i] %><% } %></span><span class="contact-groups-more"> <a>(+<%= Groups.Count - MAX_DISPLAYED %> more)</a></span>
       <div class="other-contact-groups" style="display: none">
           <% for (int i = MAX_DISPLAYED; i < Groups.Count; i++) { %>
           <div><%= Groups[i] %></div>
           <% } %>
       </div>
   </div>
<% } else { %>
    <%= String.Join(", ", Groups.ToArray()) %>
<% } %>

