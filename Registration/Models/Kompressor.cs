using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Registration.Models
{
    public class Kompressor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please, fill in this area")]
        public string PressIn { get; set; }
        [Required(ErrorMessage = "Please, fill in this area")]
        public string PressOut { get; set; }
        [Required(ErrorMessage = "Please, fill in this area")]
        public string Performance { get; set; }
        [Required(ErrorMessage = "Please, fill in this area")]
        public string Rodo { get; set; }


    }
}