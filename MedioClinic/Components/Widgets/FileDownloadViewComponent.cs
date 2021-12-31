using Kentico.PageBuilder.Web.Mvc;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MedioClinic.Components.Widgets
{
    public class FileDownloadViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ComponentViewModel<FileDownloadProperties> componentViewModel)
        {
            var properties = componentViewModel?.Properties;
            var linkTextResourceKey = properties?.LinkTextResourceKey;
            var fileGuid = properties?.DownloadedFile?.FirstOrDefault()?.FileGuid;
            var secured = properties?.SecuredDownload ?? false;
            var model = (linkTextResourceKey, fileGuid, secured);

            return View("~/Components/Widgets/_FileDownload.cshtml", model);
        }
    }
}
