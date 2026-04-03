using FluentValidation;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartnerTransactionAPI.Models;
using PartnerTransactionAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace PartnerTransactionAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class TransactionController : ControllerBase
    {
        private static readonly ILog _log =
            LogManager.GetLogger(typeof(TransactionController));

        private readonly SignatureService _signatureService;
        private readonly DiscountService  _discountService;
        private readonly IValidator<TransactionRequest> _validator;
        private readonly List<PartnerConfig> _partners;

        public TransactionController(SignatureService signatureService,
            DiscountService discountService,
            IValidator<TransactionRequest> validator,
            IConfiguration configuration)
        {
            _signatureService = signatureService;
            _discountService  = discountService;
            _validator        = validator;
            _partners         = configuration.GetSection("Partners").Get<List<PartnerConfig>>() ?? new List<PartnerConfig>();
        }

        [HttpPost("submittrxmessage")]
        public IActionResult SubmitTransaction([FromBody] TransactionRequest request)
        {
            // Log incoming request (password masked) 

            _log.Info($"REQUEST | {JsonSerializer.Serialize(new {
                request.PartnerKey,
                request.PartnerRefNo,
                PartnerPassword = "*****",
                request.TotalAmount,
                request.Timestamp
            })}");

            // Helper: log failure and return
            IActionResult Fail(string msg)
            {
                _log.Warn($"VALIDATION FAILED | {msg}");
                return Ok(TransactionResponse.Failure(msg));
            }

            // FluentValidation — field-level rules
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.First().ErrorMessage;
                return Fail(firstError);
            }

            // Timestamp expiry (+-5 minutes)
            DateTime.TryParse(request.Timestamp, null,
                System.Globalization.DateTimeStyles.RoundtripKind,
                out DateTime parsedTimestamp);

            if (Math.Abs((DateTime.UtcNow - parsedTimestamp).TotalMinutes) > 5)
                return Fail("Expired.");

            // Partner authentication
            var partner = _partners.FirstOrDefault(p => p.PartnerKey == request.PartnerKey);

            if (partner == null) return Fail("Access Denied!");

            // Password verification
            string decodedPassword;
            try
            {
                decodedPassword = Encoding.UTF8.GetString(
                    Convert.FromBase64String(request.PartnerPassword!));
            }
            catch
            {
                return Fail("Access Denied!");
            }

            if (decodedPassword != partner.Password)
                return Fail("Access Denied!");

            // Signature validation
            var sigValid = _signatureService.ValidateSignature(
                request.PartnerKey!, request.PartnerRefNo!,
                request.PartnerPassword!, request.TotalAmount!.Value,
                request.Timestamp!, request.Sig!);

            if (!sigValid) return Fail("Access Denied!");

            // Items total amount cross-check 
            if (request.Items != null && request.Items.Any())
            {
                var calculatedTotal = request.Items
                    .Sum(i => (long)i.Qty!.Value * i.UnitPrice!.Value);

                if (calculatedTotal != request.TotalAmount.Value)
                    return Fail("Invalid Total Amount.");
            }

            // Calculate discount and return success 
            var (discount, finalAmount) =
                _discountService.Calculate(request.TotalAmount.Value);

            var response = TransactionResponse.Success(
                request.TotalAmount.Value, discount, finalAmount);

            _log.Info($"RESPONSE | {JsonSerializer.Serialize(response)}");

            return Ok(response);
        }

    }
}