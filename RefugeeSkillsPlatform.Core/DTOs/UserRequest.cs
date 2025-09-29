using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.DTOs
{
    public enum UserTypes
    {
        Client ,
        Provider,
        Admin 
    }
    public class UserRequest
    {
        [Required]
        [StringLength(30,MinimumLength =3)]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [StringLength(30, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string FirstName { get; set; } = string.Empty;
        [StringLength(30, MinimumLength = 3)]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [StringLength(10, MinimumLength =6)]
        [EnumDataType(typeof(UserTypes))]
        public string? UserType { get; set; } = "Client";
    }

    public class Credentials
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class UserProfileResponse
    {
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ProfileImage { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public long RowNum { get; set; }
        public DateTime CreatedOn { get; set; }
    }


    public class UserProfileRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ApprovalRequest
    {
        public string Email { get; set; }
    }

    public class ServiceApprovalRequest
    {
        public long ServiceId { get; set; }
    }

}
