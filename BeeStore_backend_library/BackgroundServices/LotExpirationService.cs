using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Repository.Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BeeStore_Repository.Utils;
using BeeStore_Repository.Data;

namespace BeeStore_Repository.BackgroundServices
{
    public class LotExpirationService : BaseBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;
        public LotExpirationService(
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

            var lots = await _unitOfWork.LotRepo.GetQueryable(query => query.Where(x => x.ImportDate.HasValue
                                                                          && x.IsDeleted != true)
                                                                          .Include(o => o.Product)
                                                                          .ThenInclude(o => o.OcopPartner));

            List<string> ExpiredLotNames = new List<string>();
            List<string> AboutToExpiredLots = new List<string>();
            string Email = string.Empty;
            // Update expired items
            foreach (var item in lots)
            {
                var timeLeft = (item.ExpirationDate.Value.Date - DateTime.Now.Date).TotalDays;
                if(item !=  null)
                {
                    Email = item.Product.OcopPartner.Email;
                }

                if (timeLeft <= 5 && timeLeft > 0 || timeLeft <= 0)
                {
                    _logger.LogDebug($"Send an email here: {item.Id}");
                }
                if (timeLeft <= 0)
                {
                    if (item.TotalProductAmount != 0)
                    {
                        ExpiredLotNames.Add(item.Name);
                    }
                    item.IsDeleted = true;
                }
                else
                {
                    AboutToExpiredLots.Add(item.Name);
                }
            }

            //send an email afterward with the list of names
            EmailSender(Email, ExpiredLotNames, AboutToExpiredLots);
            await _unitOfWork.SaveAsync();
        }

        private async void EmailSender(string targetMail, List<string>? LotName, List<string> LotName2)
        {
            try
            {
                var target = new MailAddress(targetMail);

                var config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile(Constants.DefaultString.systemJsonFile).Build();

                var mailConfig = config.GetSection("Mail").Get<AppConfiguration>();

                var keyVault = config.GetSection("KeyVault").Get<AppConfiguration>();

                var _client = new SecretClient(new Uri(keyVault.KeyVaultURL), new EnvironmentCredential());

                string smtpPassword = _client.GetSecret("BeeStore-Smtp-Password").Value.Value;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(mailConfig.sourceMail);
                mailMessage.Subject = Constants.Smtp.lotExpireMailSubject;
                mailMessage.To.Add(target);
                // Ignore this abomination below
                if (LotName != null)
                {
                    mailMessage.Body = $@"
                                    <html>
                                      <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
                                        <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f9f9f9; border-radius: 10px;'>
                                          <h2 style='color: #4CAF50;'>Welcome to BeeShelf!</h2>
                                          <p>Dear User,</p>
                                           <p style='font-weight: bold; font-size: 12px; color: #333;'>About to expired Lots: </p>
                                            <span> {FormatLotsForEmail(LotName2)} </span>
                                          <p style='font-weight: bold; font-size: 12px; color: #333;'>Expired Lots: </p>
                                            <span> {FormatLotsForEmail(LotName)} </span>
                                          <p>Please retrieve these as soon as possible at there respective Store.</p>
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
