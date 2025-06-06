﻿using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Repositories
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        public TicketRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task CreateTicketAsync(Ticket ticket)
        {
            await _dbSet.AddAsync(ticket);
        }

        public async Task<Ticket?> GetByReferenceAsync(string transactionReference)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.TransactionReference == transactionReference);
        }

        public async Task<Ticket?> GetTicketByIdAsync(int ticketId)
        {
            return await _dbSet
                .AsSplitQuery()
                .Include(t => t.OriginStation)
                .Include(t => t.DestinationStation)
                .FirstOrDefaultAsync(t => t.Id == ticketId);
        }

        public async Task<Ticket?> GetActiveQrTicketByIdAsync(int ticketId)
        {
            return await _dbSet
                .AsSplitQuery()
                .Include(t => t.OriginStation)
                .Include(t => t.DestinationStation)
                .FirstOrDefaultAsync(t => 
                    t.Id == ticketId &&
                    t.Type == TicketType.QRTicket &&
                    (
                        (t.Status == TicketStatus.Active && t.ExpiryTime > DateTime.UtcNow) ||
                        (t.Status == TicketStatus.InUse)
                    ));
        }

        public async Task<IEnumerable<Ticket>> GetQrTicketsByStatusAsync(int userId, TicketStatus status)
        {
            return await _dbSet
                .AsSplitQuery()
                .Include(t => t.OriginStation)
                .Include(t => t.DestinationStation)
                .Where(t => t.UserId == userId && t.Status == status && t.Type == TicketType.QRTicket)
                .ToListAsync();
        }

        public async Task<Ticket?> GetActiveRapidPassTicketByUserIdAsync(int userId)
        {
            return await _dbSet.FirstOrDefaultAsync(t =>
                t.UserId == userId &&
                t.Type == TicketType.RapidPass &&
                (
                    (t.Status == TicketStatus.Active && t.ExpiryTime > DateTime.UtcNow) ||
                    (t.Status == TicketStatus.InUse)
                ));
        }

        public async Task<int> GetActiveAndInUseQrTicketsCountByUserIdAsync(int userId)
        {
            return await _dbSet.CountAsync(t => 
                t.UserId == userId && 
                t.Type == TicketType.QRTicket &&
                (
                    (t.Status == TicketStatus.Active && t.ExpiryTime > DateTime.UtcNow) || 
                    (t.Status == TicketStatus.InUse)
                ));
        }

        public async Task<IDictionary<string, int>> GetTicketTypeDistributionByMonthAsync(int months)
        {
            DateTime startDate = DateTime.UtcNow.AddMonths(-months + 1).Date;

            var counts = await _dbSet
                .Where(t => t.CreatedAt >= startDate && t.Status == TicketStatus.Used)
                .GroupBy(t => t.Type)
                .Select(g => new {
                    Type = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return new Dictionary<string, int>
            {
                { "QRTicket", counts.FirstOrDefault(x => x.Type == TicketType.QRTicket)?.Count ?? 0 },
                { "RapidPass", counts.FirstOrDefault(x => x.Type == TicketType.RapidPass)?.Count ?? 0 }
            };
        }
    }
}
