using Amazon.Runtime.Internal.Util;
using Amazon.S3.Model;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using MySqlX.XDevAPI;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeeStore_Repository.Logger;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using BeeStore_Repository.Utils;
using BeeStore_Repository.DTO.PaymentDTOs;
using BeeStore_Repository.Data;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Enums.SortBy;

namespace BeeStore_Repository.Services
{


    public class PaymentService : IPaymentService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public PaymentService(UnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaymentResponseDTO> CreateQrCode(CoinPackValue options, string custom_amount, PaymentRequestDTO request)
        {

            var config = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile(Constants.DefaultString.systemJsonFile).Build();

            var payOs = config.GetSection("PayOs").Get<AppConfiguration>();

            //var payOS = _configuration.GetSection("PayOS");

            var clientId = payOs.ClientID;
            var apiKey = payOs.APIKey;
            var checksum = payOs.ChecksumKey;

            Random randomOrderCode = new Random();
            int _orderCode = randomOrderCode.Next(1, 10000);

            var transaction = await _unitOfWork.TransactionRepo.SingleOrDefaultAsync(u => u.Code.Equals(_orderCode));
            while (transaction != null)
            {
                _orderCode = randomOrderCode.Next(1, 10000);
            }

            var user = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Email.Equals(request.BuyerEmail));
            if (user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.PartnerIdNotFound);
            }

            decimal? value = options switch
            {
                CoinPackValue.Ten_Thousand_VND => 10000,
                CoinPackValue.Fifty_Thousand_VND => 50000,
                CoinPackValue.One_Hundred_Thousand_VND => 100000,
                CoinPackValue.Two_Hundred_Thousand_VND => 200000,
                CoinPackValue.Custom_Amount => value = (decimal?)Int32.Parse(custom_amount),
                _ => null
            };


            int price = (int)(value);

            var data = $"amount={price}" +
                       $"&cancelUrl={request.CancelUrl}" +
                       $"&description={request.Description}" +
                       $"&orderCode={_orderCode}" +
                       $"&returnUrl={request.ReturnUrl}";



            var signatures = SHA256.HmacSHA256(data, checksum);



            DateTime expiredAtTime = DateTime.Now.AddMinutes(30);
            long unixTime = new DateTimeOffset(expiredAtTime).ToUnixTimeSeconds();

            var item = new[]
            {
                new
                {
                   name = $"{price} coin pack",
                   quantity = 1,
                   price = price
                }
            };

            var requestData = new
            {
                orderCode = _orderCode,
                amount = price,
                description = request.Description,
                buyerName = user.FirstName + " " + user.LastName,
                buyerEmail = request.BuyerEmail,
                buyerPhone = user.Phone,
                buyerAddress = "",
                items = item,
                cancelUrl = request.CancelUrl,
                returnUrl = request.ReturnUrl,
                expiredAt = unixTime,
                signature = signatures,
            };

            var requestDataJson = JsonConvert.SerializeObject(requestData, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
            });

            var requestContent = new StringContent(requestDataJson, Encoding.UTF8, "application/json");
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-client-id", clientId);
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);

            var createPaymentLinkRes = client.PostAsync(payOs.POSTpaymentRequest, requestContent).Result;

            if (createPaymentLinkRes.IsSuccessStatusCode)
            {
                var responseContent = createPaymentLinkRes.Content.ReadAsStringAsync().Result;
                var responseData = JsonConvert.DeserializeObject<PaymentResponseDTO>(responseContent);

                string jsonResult = JsonConvert.SerializeObject(responseData);

                if (responseData != null && responseData.Code == "00")
                {
                    //return await System.Threading.Tasks.Task.FromResult(jsonResult);
                    return responseData;
                }
                else
                {
                    //return await System.Threading.Tasks.Task.FromResult("false");
                    throw new Exception(responseData.Code);
                }
            }
            else
            {
                //return await System.Threading.Tasks.Task.FromResult(createPaymentLinkRes.StatusCode.ToString());
                throw new Exception(createPaymentLinkRes.StatusCode.ToString());
            }
        }
    }
}
