using LogQuake.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Domain.Interfaces
{
    public interface IKillRepository : IRepositoryBase<Kill>
    {
        List<Kill> GetByIdList(int Id);

        void RemoveAll();
    }
}
