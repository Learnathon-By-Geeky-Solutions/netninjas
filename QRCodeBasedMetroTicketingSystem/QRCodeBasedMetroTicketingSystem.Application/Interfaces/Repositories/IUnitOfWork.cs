namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IStationRepository StationRepository { get; }
        IStationDistanceRepository StationDistanceRepository { get; }
        ISettingsRepository SettingsRepository { get; }
        IUserRepository UserRepository { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
