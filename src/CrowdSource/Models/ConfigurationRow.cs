using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSource.Models
{
    public class ConfigurationRow
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
