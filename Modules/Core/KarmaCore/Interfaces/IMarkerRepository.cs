using KarmaCore.Utils;

namespace KarmaCore.Interfaces
{
    /// <summary>
    /// Репозиторий для сохранения данных
    /// </summary>
    public interface IMarkerRepository
    {
        public void Save(Currencies currencies);
    }
}
