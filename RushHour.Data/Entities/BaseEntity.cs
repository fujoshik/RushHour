﻿using System.ComponentModel.DataAnnotations;

namespace RushHour.Data.Entities
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
