using Murzik.Entities.MurrData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murzik.Interfaces
{
    public interface IReaderMurrProvider
    {
        public Task<IReadOnlyCollection<FinDataSources>> GetFinDataSources();
    }
}
