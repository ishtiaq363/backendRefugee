﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefugeeSkillsPlatform.Core.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using System.Security.Principal;

namespace RefugeeSkillsPlatform.Core.DTOs
{
    public  class ServiceRequest
    {
        public long? ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long CategoryId { get; set; }
        public long CreatedByUserId { get; set; }
        public long DeliveryMethodId { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class AllServicesRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        // Add this for filtering by user
        public int? UserId { get; set; }
    }
    public class AllBookingsRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? BookingId { get; set; }
    }
    public class ServiceResponse
    {
        public long ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long CreatedByUserId { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public long DeliveryMethodId { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime? CreatedOn { get; set; }
        public bool? IsApproved { get; set; }
        public long RowNum { get; set; }   // comes from ROW_NUMBER()
        public string? Logo { get; set; }
    }
    
    public class AvailabilitySlotsDTO
    {
        public long AvailabilitySlotId { get; set; }
        public long ServiceId { get; set; }
        public DateTime SlotStartDate { get; set; } = DateTime.Now;
        public DateTime SlotEndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
     }       

}
