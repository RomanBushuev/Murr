using System.Threading.Tasks;

namespace Murzik.Interfaces
{
    public interface IAlgorithm
    {
        Task Run();

        long TaskId { get; set; }
    }
}
