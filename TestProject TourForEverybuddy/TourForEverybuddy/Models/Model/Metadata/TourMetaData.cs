﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TourForEverybuddy.Models
{
    [MetadataType(typeof(TourMetaDate))]
    public partial class Tour
    {
    }
    public class TourMetaDate
    {
        [Required]
        public string title { get; set; }
        [Required]
        public string description { get; set; }
        public int userID { get; set; }
        public Nullable<bool> disable { get; set; }
        [Required]
        [DisplayName("City")]
        public int cityID { get; set; }
        public string fullDescription { get; set; }
        [DisplayName("Maximum travelers ")]
        public Nullable<byte> MaximumTravelers { get; set; }
        [DisplayName("Price")]
        public Nullable<int> price { get; set; }
        [DisplayName("Duration")]
        public Nullable<byte> durationID { get; set; }
        [DisplayName("Start at")]
        public string startsAt { get; set; }
        
    }

}