using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.Entities
{
    [Table("UserZoomAccounts")]
    public class UserZoomAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ZoomAccountId { get; set; }

        [Required]
        [ForeignKey("User")]
        public long UserId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string AccessToken { get; set; }

        [Required]
        [MaxLength(2000)]
        public string RefreshToken { get; set; }

        [Required]
        public DateTime TokenExpiresAt { get; set; }

        [MaxLength(200)]
        public string? ZoomUserId { get; set; }

        [MaxLength(300)]
        public string? ZoomEmail { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedOn { get; set; }

        // Navigation property
        public virtual Users? User { get; set; }
    }
}
