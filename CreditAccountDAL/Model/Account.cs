using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using CurrencyCodesResolver;

namespace CreditAccountDAL
{
    public class Account
    {
        [Column(TypeName = "bigint")]
        public long Id { get; set; }

        [Column(TypeName = "bigint")]
        public long UserId { get; set; }

        public int CurrencyCode { get; set; }

        [Column(TypeName ="money")]
        public decimal Money { get; set; }

        public virtual User User { get; set; }
    }
}
