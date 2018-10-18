using System;
using System.Collections.Generic;
using System.Text;

namespace Nerdable.DbHelper.Models.Response
{
    public enum ReturnCode
    {
        Success = 1,
        PartialSuccess,
        Fail,
        InvalidInput,
        DoesNotExist,
        NoEntitiesMatchQuery,
        MappingFailure,
        DatabaseUpdateFailure,
        DatabaseAddFailure,
        DatabaseRemoveFailure,
        DbSetDoesNotExist
    }
}
