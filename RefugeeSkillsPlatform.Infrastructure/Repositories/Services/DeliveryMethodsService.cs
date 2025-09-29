using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Entities;
using RefugeeSkillsPlatform.Core.Interfaces;
using RefugeeSkillsPlatform.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Infrastructure.Repositories.Services
{
    public class DeliveryMethodsService : IDeliveryMethodsService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeliveryMethodsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<DeliveryMethodsResponse> GetDeliveryMethodsResponseFor()
        {
                var deliveryMethods = _unitOfWork.GetRepository<DeliveryMethods>().GetAll();
                if (deliveryMethods == null  || !deliveryMethods.Any())
                {
                    return new List<DeliveryMethodsResponse>();                   
                }
                var deliveryMethosList = deliveryMethods.Select(x => new DeliveryMethodsResponse()
                {
                    DeliveryMethodId = x.DeliveryMethodId,
                    MethodName = x.MethodName,
                    Description = x.Description
                }).ToList();

                return deliveryMethosList;            
        }       
    }
}
