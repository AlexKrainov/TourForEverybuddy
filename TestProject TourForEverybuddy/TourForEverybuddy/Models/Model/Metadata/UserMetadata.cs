﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TourForEverybuddy.Models
{
    [MetadataType(typeof(UserMetaDate))]
    public partial class User
    {
    }

    public class UserMetaDate
    {
        [Required]
        public string Name { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Password { get; set; }
        public byte Age { get; set; }
        [Required]
        public string Email { get; set; }
        public string Phone { get; set; }

        [DisplayName("Country")]
        public Nullable<int> CountryId { get; set; }
        [DisplayName("Language")]
        public Nullable<int> LanguageId { get; set; }
        [DisplayName("Your role ...")]
        public Nullable<byte> RouleId { get; set; }
        public byte[] Photo { get; set; }

        //[DisplayName("Agreement")]
        //public bool Agreement { get; set; }

        public Nullable<System.DateTime> DateRegister { get; set; }
        public Nullable<System.DateTime> LastAuthorization { get; set; }
    }

}