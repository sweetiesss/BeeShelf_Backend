﻿namespace BeeStore_Repository.DTO.PaymentDTOs
{
    public class MoneyTransferListDTO
    {
        public int Id { get; set; }

        public int? OcopPartnerId { get; set; }

        public int? TransferBy { get; set; }

        public string? TransferByStaffEmail { get; set; }
        public string? TransferByStaffName { get; set; }
        public ulong? IsTransferred { get; set; }
        public string? PictureLink { get; set; }
        public string? CancellationReason { get; set; }

        public decimal? Amount { get; set; }

        public DateTime? CreateDate { get; set; }
    }
}
