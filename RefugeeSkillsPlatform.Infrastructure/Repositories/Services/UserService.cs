﻿using Azure.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Entities;
using RefugeeSkillsPlatform.Core.Interfaces;
using RefugeeSkillsPlatform.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
 

namespace RefugeeSkillsPlatform.Infrastructure.Repositories.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool ProviderApproval(ApprovalRequest request)
        {
            var user = _unitOfWork.GetRepository<Users>().FirstOrDefult(x => x.Email == request.Email);
            if(user == null)
            {
                return false;
            }
            user.IsApproved = true;
            _unitOfWork.Commit();
            return true;

        }
        public bool ServiceApproval(ServiceApprovalRequest request)
        {
            var service = _unitOfWork.GetRepository<RefugeeSkillsPlatform.Core.Entities.Services>().FirstOrDefult(x => x.ServiceId == request.ServiceId);
            if(service ==null)
            {
                return false;
            }
            service.IsApproved = true;
            _unitOfWork.Commit();
            return true;
        }

        public List<UserProfileResponse>? GetUserProfiles(UserProfileRequest request)
        {
            var pageNumParam = new SqlParameter("@PageNumber", SqlDbType.Int) { Value = request.PageNumber };
            var pageSizeParam = new SqlParameter("@PageSize", SqlDbType.Int) { Value = request.PageSize };
            var response = _unitOfWork.SpListRepository<UserProfileResponse>(
           "sp_GetUserProfiles @PageNumber, @PageSize", pageNumParam, pageSizeParam);
            
            return response.Any() ? response : new List<UserProfileResponse>();
            //var listOfUserProfiles = _unitOfWork.GetRepository<Users>().GetAll().Select(x => new ).ToList();
            //var listOfUserProfiles = (from u in _unitOfWork.GetRepository<Users>().GetAll()
            //                          join r in _unitOfWork.GetRepository<Roles>().GetAll()
            //                          on u.RoleId equals r.RoleId
            //select new UserProfileResponse()
            //{
            //    UserName = u.UserName,
            //    Email = u.Email,
            //    FirstName = u.FirstName,
            //    LastName = u.LastName,
            //    RoleName = r.RoleName
            //}).ToList();
            //return listOfUserProfiles;
        }

        public int RegisterUser(UserRequest request)
        {
            var isUserExist = _unitOfWork.GetRepository<Users>().FirstOrDefult(x => x.UserName == request.UserName || x.Email == request.Email);
            if (isUserExist != null)
            {
                return 0;
            }
            long userTypeId = 0;
            if(request.UserType !=null)
            {
                var userType = _unitOfWork.GetRepository<Roles>().FirstOrDefult(x => x.RoleName == request.UserType.ToString());
                if (userType != null) userTypeId = userType.RoleId;
            }
            

            Users user = new Users() { 
            UserName =request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = request.Password,
            RoleId = userTypeId,
            ProfileImage = ""
            };
            if(request.UserType == UserTypes.Client.ToString())
            {
                user.IsApproved = true;
            }else
            {
                user.IsApproved = false;
            }
            _unitOfWork.GetRepository<Users>().Add(user);
            var save = _unitOfWork.Commit();
            return save;
        }

        public UserProfileResponse? VerifyUser(Credentials request)
        {
            var user = _unitOfWork.GetRepository<Users>()
                .FirstOrDefult(x => x.UserName == request.UserName
                                  && x.PasswordHash == request.Password);

            if (user == null)
                return null;

            // Get the role name by RoleId
            var roleName = _unitOfWork.GetRepository<Roles>()
                .GetAll()
                .Where(r => r.RoleId == user.RoleId)
                .Select(r => r.RoleName)
                .FirstOrDefault();

            return new UserProfileResponse()
            {
                UserId = user.UserId,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleName = roleName
            };
        }


        //public UserProfileResponse? VerifyUser(Credentials request)
        //{
        //    var userProfileResponse = _unitOfWork.GetRepository<Users>().FirstOrDefult(x => x.UserName == request.UserName && x.PasswordHash ==request.Password);
        //    if (userProfileResponse != null)
        //    {
        //        return new UserProfileResponse()
        //        {
        //            Email = userProfileResponse.Email,
        //            UserName = userProfileResponse.UserName,
        //            FirstName = userProfileResponse.FirstName,
        //            LastName = userProfileResponse.LastName,
        //        };
        //    }
        //    else
        //    {
        //        return null;// new UserProfileResponse() { };
        //    }
        //}


    }
}
