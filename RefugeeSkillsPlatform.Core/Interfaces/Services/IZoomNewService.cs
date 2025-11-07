using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.Interfaces.Services
{
    public interface IZoomNewService
    {
        Task<string> CreateMeetingAsync(string topic, DateTime startTime);
    }
}
