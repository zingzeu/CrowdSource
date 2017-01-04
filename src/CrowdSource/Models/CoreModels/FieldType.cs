﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CrowdSource.Models.CoreModels
{
    public class FieldType
    {
        [Key]
        public int FieldTypeId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public FieldDataType DataType { get; set; }
    }

    public enum FieldDataType
    {
        StringType,
        BooleanType
    }
}
