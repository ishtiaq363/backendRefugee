using RefugeeSkillsPlatform.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.Interfaces.Services
{
    public interface IUserService
    {
        int RegisterUser(UserRequest request);

        UserProfileResponse? VerifyUser(Credentials request);
        List<UserProfileResponse>? GetUserProfiles(UserProfileRequest userProfileRequest);
        bool ProviderApproval(ApprovalRequest request);
        bool ServiceApproval(ServiceApprovalRequest request);
        List<PaymentResponse>? GetAllPaymentsForProvider(PaymentProviderRequest request);
    }
}
