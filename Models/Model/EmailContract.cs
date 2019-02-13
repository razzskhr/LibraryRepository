using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Model
{
    public class EmailContract
    {
        [Required]
        [Display(Name = "From Email Address")]
        public string FromEmailAddress { get; set; }

        public string Alias { get; set; }

        [Required]
        [Display(Name = "To Email Address")]
        public List<string> ToEmailAddress { get; set; }
        [Display(Name = "Cc Email Address")]
        public string CcEmailAddress { get; set; }
        [Display(Name = "Bcc Email Address")]
        public string BccEmailAddress { get; set; }
        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }
        [Required]
        [Display(Name = "Body")]
        public string Body { get; set; }
    }
}
