﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private IDbContextTransaction? _transaction;
        private bool _disposed = false;

        public IStationRepository StationRepository { get; }
        public IStationDistanceRepository StationDistanceRepository { get; }
        public ISystemSettingsRepository SettingsRepository { get; }
        public IUserRepository UserRepository { get; }
        public IUserTokenRepository UserTokenRepository { get; }
        public IWalletRepository WalletRepository { get; }
        public ITransactionRepository TransactionRepository { get; }

        public UnitOfWork(ApplicationDbContext db,
                          IStationRepository stationRepository,
                          IStationDistanceRepository stationDistanceRepository,
                          ISystemSettingsRepository settingsRepository,
                          IUserRepository userRepository,
                          IUserTokenRepository userTokenRepository,
                          IWalletRepository walletRepository,
                          ITransactionRepository transactionRepository)
        {
            _db = db;
            StationRepository = stationRepository;
            StationDistanceRepository = stationDistanceRepository;
            SettingsRepository = settingsRepository;
            UserRepository = userRepository;
            UserTokenRepository = userTokenRepository;
            WalletRepository = walletRepository;
            TransactionRepository = transactionRepository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _db.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                if(_transaction != null)
                {
                    await _transaction.RollbackAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _transaction?.Dispose();
                _db?.Dispose(); 
            }

            _disposed = true;
        }
    }
}
