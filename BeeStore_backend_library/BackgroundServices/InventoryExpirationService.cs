﻿using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Repository.Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using BeeStore_Repository.Utils;
using BeeStore_Repository.Data;

namespace BeeStore_Repository.BackgroundServices
{
    public class InventoryExpirationService : BaseBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;
        public InventoryExpirationService(
            IServiceProvider serviceProvider,
            ILoggerManager logger,
            IUnitOfWork unitOfWork)
            : base(serviceProvider, logger)
        {
            SetInterval(TimeSpan.FromHours(12));
            _unitOfWork = unitOfWork;
        }

        protected override async Task PerformPeriodicTaskAsync(CancellationToken stoppingToken)
        {

            var inventory = await _unitOfWork.RoomRepo.GetQueryable(x => x.Include(o => o.Lots).Include(o => o.OcopPartner)
                                                                                .Where(u => u.ExpirationDate.HasValue
                                                                                    && u.OcopPartnerId.HasValue));

            inventory = inventory.ToList();
            // Update expired items
            foreach (var item in inventory)
            {
                var timeLeft = (item.ExpirationDate.Value.Date - DateTime.Now.Date).TotalDays;
                if (timeLeft <= 3 && timeLeft > 0)
                {
                    _logger.LogDebug($"Send an email here: {item.Id}");
                    EmailSender(item.OcopPartner.Email, null, item.Id);
                }
                if(timeLeft <= 0)
                {
                    var lotName = new List<string>();
                    foreach(var lot in item.Lots)
                    {
                        //add the lotname to a list so you can send an email later.
                        lotName.Add($"Lot Number:{lot.LotNumber}, Lot Name:{lot.Name}");
                        lot.IsDeleted = true;
                    }

                    //send an email then null the inventory id
                    EmailSender(item.OcopPartner.Email, lotName, item.Id);
                    item.OcopPartnerId = null;
                }
            }
            await _unitOfWork.SaveAsync();

        }

        private async void EmailSender(string targetMail, List<string>? LotName, int inventoryId)
        {
            try
            {
                var target = new MailAddress(targetMail);
                var inventory = await _unitOfWork.RoomRepo.SingleOrDefaultAsync(u => u.Id == inventoryId, query => query.Include(o => o.Store));
                
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile(Constants.DefaultString.systemJsonFile).Build();

                var mailConfig = config.GetSection("Mail").Get<AppConfiguration>();

                var keyVault = config.GetSection("KeyVault").Get<AppConfiguration>();

                var _client = new SecretClient(new Uri(keyVault.KeyVaultURL), new EnvironmentCredential());

                string smtpPassword = _client.GetSecret("BeeStore-Smtp-Password").Value.Value;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(mailConfig.sourceMail);
                mailMessage.Subject = Constants.Smtp.roomExpireMailSubject;
                mailMessage.To.Add(target);
                // Ignore this abomination below
                if(LotName != null)
                {
                mailMessage.Body = $@"
                                    <html>
                                      <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
                                        <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f9f9f9; border-radius: 10px;'>
                                          <h2 style='color: #4CAF50;'>Welcome to BeeShelf!</h2>
                                          <p>Dear User,</p>
                                          <p>Your Room has expired: Room {inventory.RoomCode}</p>
                                          <p style='font-weight: bold; font-size: 12px; color: #333;'>Lots inside room: </p>
                                            <span> {FormatLotsForEmail(LotName)} </span>
                                          
                                          <p>Please retrieve these as soon as possible at Store: {inventory.Store.Name}.</p>
                                          <p>You have three business day to retrieve it, if you don't we will have the authority to dismantle the products.</p>
                                          <p>Thank you for using our service!</p>
                                          <p style='margin-top: 30px; font-size: 12px; color: #888;'>This is an automated email, please do not reply.</p>
                                        </div>
                                      </body>
                                    </html>";

                }
                else
                {
                    mailMessage.Body = $@"
                                    <html>
                                      <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
                                        <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f9f9f9; border-radius: 10px;'>
                                          <h2 style='color: #4CAF50;'>Welcome to BeeShelf!</h2>
                                          <p>Dear User,</p>
                                          <p>Your Room is about to be expired in {inventory.ExpirationDate.Value.Day} days: Room {inventory.RoomCode}</p>
    
                                          <p>Please extend your Room if you wish to use it for longer.</p>
                                          <p>You have three business day to retrieve it, if you don't we will have the authority to dismantle the products.</p>
                                          <p>Thank you for using our service!</p>
                                          <p style='margin-top: 30px; font-size: 12px; color: #888;'>This is an automated email, please do not reply.</p>
                                        </div>
                                      </body>
                                    </html>";
                }
                mailMessage.IsBodyHtml = true;

                var smtpClient = new SmtpClient(Constants.Smtp.smtp)
                {
                    Port = 587,
                    Credentials = new NetworkCredential(mailConfig.sourceMail, smtpPassword),
                    EnableSsl = true
                };

                smtpClient.Send(mailMessage);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string FormatLotsForEmail(List<string> lotNames)
        {
            if (lotNames.Count == 0)
            {
                return "";
            }
            var formattedLots = lotNames.Select(lotName =>
                $"{lotName}").ToList();

            return string.Join("<br>", formattedLots);
        }
    }
}
