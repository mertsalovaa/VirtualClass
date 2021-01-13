using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualClass.Models
{
    public class RegisterUserModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        //[Required(ErrorMessage = "Birthday is required")]
        //[DataType(DataType.Date)]
        //public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Image is required")]
        public string Image { get; set; }
    }
}
