using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.DTOs
{
    public class ZoomSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
    }

    public class ZoomUserInfo
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string AccountId { get; set; }
    }

    public class ZoomMeetingRequest
    {
        public string Topic { get; set; } = "New Meeting";
        public DateTime StartTime { get; set; } = DateTime.UtcNow.AddMinutes(10);
        public int Duration { get; set; } = 30; // in minutes
        public bool ZoomPassword { get; set; } = false;
        public string? Password { get; set; }
    }

public class ZoomMeetingResponse
    {
        [JsonPropertyName("uuid")]
        public string? Uuid { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("host_id")]
        public string? HostId { get; set; }

        [JsonPropertyName("host_email")]
        public string? HostEmail { get; set; }

        [JsonPropertyName("topic")]
        public string? Topic { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("start_time")]
        public DateTime? StartTime { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("start_url")]
        public string? StartUrl { get; set; }

        [JsonPropertyName("join_url")]
        public string? JoinUrl { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }

        [JsonPropertyName("h323_password")]
        public string? H323Password { get; set; }

        [JsonPropertyName("pstn_password")]
        public string? PstnPassword { get; set; }

        [JsonPropertyName("encrypted_password")]
        public string? EncryptedPassword { get; set; }
    }


}
