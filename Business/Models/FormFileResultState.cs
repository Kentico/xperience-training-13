using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Models
{
    public enum FormFileResultState
    {
        FileOk,
        FileEmpty,
        FileTooBig,
        ForbiddenFileType
    }
}
