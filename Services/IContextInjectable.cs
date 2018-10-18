using System;
using System.Collections.Generic;
using System.Text;

namespace Nerdable.DbHelper.Services
{
    public interface IContextInjectable<TDbContext>
    {
        TDbContext _dbContext { get; set; }

        void SetContext(TDbContext context);
    }
}
