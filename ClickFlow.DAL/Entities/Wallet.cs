﻿namespace ClickFlow.DAL.Entities
{
    public class Wallet
    {
        public int Id { get; set; }
        public int Balance { get; set; }
        public User? User { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
