using Murzik.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Murzik.Interfaces
{
    public interface IAlgorithm
    {
        Task Run();

        long TaskId { get; set; }

        IReadOnlyCollection<ParamDescriptor> GetParamDescriptors();

        void SetParamDescriptors(ParamDescriptor paramDescriptor);
    }
}
