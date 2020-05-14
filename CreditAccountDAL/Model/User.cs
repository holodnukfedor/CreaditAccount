using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CreditAccountDAL
{
    public class User
    {
        [Column(TypeName = "bigint")]
        public long Id { get; set; }

        [Required]
        [Column(TypeName = "nchar(50)")]
        public string Name { get; set; }

        public virtual List<Account> Accounts { get; set; }
    }
}
