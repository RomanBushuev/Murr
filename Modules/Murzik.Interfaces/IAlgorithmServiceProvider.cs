using System.Threading.Tasks;

namespace Murzik.Interfaces
{
    public interface IAlgorithmServiceProvider
    {
        Task CheckJob(string serviceName);
    }
}
