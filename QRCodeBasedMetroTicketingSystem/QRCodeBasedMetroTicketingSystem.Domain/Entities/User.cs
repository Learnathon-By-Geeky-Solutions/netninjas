﻿using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        [StringLength(100)]
        public required string FullName { get; set; }

        [StringLength(255)]
        public required string Email { get; set; }

        [StringLength(15)]
        public required string PhoneNumber { get; set; }

        [StringLength(30)]
        public required string NID { get; set; }

        public required string PasswordHash { get; set; }

        public bool IsEmailVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Wallet Wallet { get; set; }
    }
}
