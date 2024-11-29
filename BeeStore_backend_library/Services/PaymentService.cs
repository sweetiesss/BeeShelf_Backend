﻿using AutoMapper;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.PaymentDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace BeeStore_Repository.Services
{


    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly SecretClient _client;
        private readonly string _keyVaultURL;
        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _keyVaultURL = configuration["KeyVault:KeyVaultURL"] ?? throw new ArgumentNullException("Key Vault URL configuration values are missing.");
            _client = new SecretClient(new Uri(_keyVaultURL), new EnvironmentCredential());
        }

        public async Task<string> ConfirmPayment(ConfirmPaymentDTO request)
        {
            var transaction = await _unitOfWork.TransactionRepo.SingleOrDefaultAsync(u => u.Code.Equals(request.OrderCode));
            if (transaction == null)
            {
                throw new KeyNotFoundException(ResponseMessage.TransactionNotFound);
            }
            if(transaction.Status != Constants.PaymentStatus.Pending)
            {
                throw new ApplicationException(ResponseMessage.TransactionAlreadyProcessed);
            }
            if (request.Status == Constants.PaymentStatus.Paid)
            {
                var wallet = await _unitOfWork.WalletRepo.SingleOrDefaultAsync(u => u.OcopPartnerId.Equals(transaction.OcopPartnerId));
                wallet.TotalAmount += transaction.Amount;
            }
            transaction.Status = request.Status;
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;

        }



        public async Task<PaymentResponseDTO> CreateQrCode(CoinPackValue options, string custom_amount, PaymentRequestDTO request)
        {

            var config = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile(Constants.DefaultString.systemJsonFile).Build();

            var payOs = config.GetSection("PayOs").Get<AppConfiguration>();

            //var payOS = _configuration.GetSection("PayOS");

            var clientId = _client.GetSecret("BeeStore-Payment-ClientId").Value.Value;
            var apiKey = _client.GetSecret("BeeStore-Payment-ApiKey").Value.Value;
            var checksum = _client.GetSecret("BeeStore-Payment-CheckSumKey").Value.Value;

            Random randomOrderCode = new Random();
            int _orderCode = randomOrderCode.Next(1, 10000);

            var transaction = await _unitOfWork.TransactionRepo.AnyAsync(u => u.Code.Equals(_orderCode));
            while (transaction != false)
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
                   name = $"{price} Coin Pack",
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

            var createPaymentLinkRes = client.PostAsync(payOs.PaymentRequest, requestContent).Result;

            if (createPaymentLinkRes.IsSuccessStatusCode)
            {
                var responseContent = createPaymentLinkRes.Content.ReadAsStringAsync().Result;
                var responseData = JsonConvert.DeserializeObject<PaymentResponseDTO>(responseContent);

                string jsonResult = JsonConvert.SerializeObject(responseData);

                if (responseData != null && responseData.Code == "00")
                {
                    var newTransa = new Transaction
                    {
                        Code = _orderCode.ToString(),
                        Amount = price,
                        Description = request.Description,
                        OcopPartnerId = user.Id,
                        CreateDate = DateTime.Now,
                        Status = Constants.PaymentStatus.Pending,
                        CancellationReason = null,
                        IsDeleted = false
                    };
                    await _unitOfWork.TransactionRepo.AddAsync(newTransa);
                    await _unitOfWork.SaveAsync();
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

        public async Task<List<PaymentListDTO>> GetPaymentList()
        {
            var list = await _unitOfWork.PaymentRepo.GetQueryable();
            var a = list.ToList();
            var result = _mapper.Map<List<PaymentListDTO>>(a);
            return result;
        }

        public async Task<string> CreateMoneyTransfer(int staffId, int paymentId)
        {
            var payment = await _unitOfWork.PaymentRepo.SingleOrDefaultAsync(u => u.Id.Equals(paymentId));
            if (payment == null)
            {
                throw new KeyNotFoundException(ResponseMessage.PaymentNotFound);
            }
            if(payment.MoneyTransfers.Any(u => u.PaymentId.Equals(payment.Id))){
                throw new ApplicationException(ResponseMessage.PaymentAlreadyMade);
            }
            var employee = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Id.Equals(staffId), 
                                                                               query => query.Include(o => o.Role));
            if (employee == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            if (employee.Role.RoleName != Constants.RoleName.Staff)
            {
                throw new ApplicationException(ResponseMessage.UserRoleNotStaffError);
            }

            payment.IsTransferred = 1;
            var moneyTransfer = new MoneyTransfer{
                Amount = payment.TotalAmount,
                CreateDate = DateTime.Now,
                OcopPartnerId = payment.OcopPartnerId,
                TransferBy = staffId
            };
            await _unitOfWork.MoneyTransferRepo.AddAsync(moneyTransfer);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }
    }
}
