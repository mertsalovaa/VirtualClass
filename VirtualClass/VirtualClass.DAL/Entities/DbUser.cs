using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace VirtualClass.DAL.Entities
{
    public class DbUser : IdentityUser<long>
    {
        [Required]
        [MaxLength(30)]
        public string FullName { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required]
        public string Image { get; set; }

        [Required]
        public string Address { get; set; }
        public virtual ICollection<DbUserRole> UserRoles { get; set; }
    }
}
