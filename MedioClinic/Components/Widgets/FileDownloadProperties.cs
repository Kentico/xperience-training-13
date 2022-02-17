using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedioClinic.Components.Widgets
{
    public class FileDownloadProperties : IWidgetProperties
    {
        [EditingComponent(MediaFilesSelector.IDENTIFIER, Label = "{$MedioClinic.InlineEditor.FileDownloadSelector.DownloadedFile$}", Order = 1)]
        [EditingComponentProperty(nameof(MediaFilesSelectorProperties.MaxFilesLimit), 1)]
        [Required]
        public IEnumerable<MediaFilesSelectorItem> DownloadedFile { get; set; } = new List<MediaFilesSelectorItem>();

        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "{$MedioClinic.InlineEditor.FileDownloadSelector.LinkTextResourceKey$}", Order = 2)]
        [Required]
        public string? LinkTextResourceKey { get; set; }

        /// <remarks>This property shall be accompanied by another configuration property that specifies roles entitled for download.</remarks>
        [EditingComponent(CheckBoxComponent.IDENTIFIER, Label = "{$MedioClinic.InlineEditor.FileDownloadSelector.SecuredDownload$}", Order = 3)]
        public bool SecuredDownload { get; set; }
    }
}