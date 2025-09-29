using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Interfaces.Services;
using RefugeeSkillsPlatform.WebApi.Common;

namespace RefugeeSkillsPlatform.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DeliveryMethodsController : ControllerBase
    {
        private readonly IDeliveryMethodsService _deliveryMethodsSerivc;
        public DeliveryMethodsController(IDeliveryMethodsService deliveryMethodsSerivc)
        {
            _deliveryMethodsSerivc = deliveryMethodsSerivc;
        }
        //api/DeliveryMethods/GetDeliveryMethods
        [HttpGet]
        public IActionResult GetDeliveryMethods()
        {
            var deliveryMethodResponse = _deliveryMethodsSerivc.GetDeliveryMethodsResponseFor();
            if (deliveryMethodResponse == null)
            {
                return Ok(new StandardRequestResponse<List<DeliveryMethodsResponse>>() { Data = null, Success = false, Message = "Interal Server Error", Status =500}
                    );
            }
            return Ok(new StandardRequestResponse<List<DeliveryMethodsResponse>>()
            {
                Data = deliveryMethodResponse,
                Message = "Successfully fetch methods",
                Success = true,
                Status = 200
            });
           
        }

    }
}
