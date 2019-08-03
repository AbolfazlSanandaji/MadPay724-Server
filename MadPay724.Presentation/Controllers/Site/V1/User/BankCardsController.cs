﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.ErrorAndMessage;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Users;
using MadPay724.Data.Models;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Upload.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.User
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel")]
    [ApiController]
    public class BankCardsController : ControllerBase
    {
        private readonly IUnitOfWork<MadpayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<BankCardsController> _logger;

        public BankCardsController(IUnitOfWork<MadpayDbContext> dbContext, IMapper mapper,
            ILogger<BankCardsController> logger)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpPost(ApiV1Routes.BankCard.AddBankCard)]
        public async Task<IActionResult> AddBankCard(string id, BankCardForUpdateDto bankCardForUpdateDto)
        {
            var bankCardFromRepo = await _db.BankCardRepository.GetAsync(p => p.CardNumber == bankCardForUpdateDto.CardNumber);
            if (bankCardFromRepo == null)
            {
                var cardForCreate = new BankCard()
                {
                    UserId = id
                };
                var bankCard = _mapper.Map(bankCardForUpdateDto, cardForCreate);

              await  _db.BankCardRepository.InsertAsync(bankCard);

                if (await _db.SaveAsync())
                    return CreatedAtRoute("GetBankCard",new { id = bankCard.Id, userId=id }, bankCard);
                else
                    return BadRequest("خطا در ثبت اطلاعات");
            }
            {
                return BadRequest("این کارت قبلا ثبت شده است");
            }
        }

        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.BankCard.GetBankCard, Name = "GetBankCard")]
        public async Task<IActionResult> GetBankCard(string id, string userId)
        {
            var bankCardFromRepo = await _db.BankCardRepository.GetByIdAsync(id);
            if (bankCardFromRepo != null)
            {
                if (bankCardFromRepo.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {
                    var bankCard = _mapper.Map<BankCardForReturnDto>(bankCardFromRepo);

                    return Ok(bankCard);
                }
                else
                {
                    _logger.LogError($"کاربر   {RouteData.Values["userId"]} قصد دسترسی به کارت دیگری را دارد");

                    return BadRequest("شما اجازه دسترسی به کارت کاربر دیگری را ندارید");
                }
            }
            else
            {
                return BadRequest("کارتی وجود ندارد");
            }

        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpPut(ApiV1Routes.BankCard.UpdateBankCard)]
        public async Task<IActionResult> UpdateBankCard(string id, BankCardForUpdateDto bankCardForUpdateDto)
        {
            var bankCardFromRepo = await _db.BankCardRepository.GetByIdAsync(id);
            if (bankCardFromRepo != null)
            {
                if (bankCardFromRepo.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {
                    var bankCard = _mapper.Map(bankCardForUpdateDto, bankCardFromRepo);

                    _db.BankCardRepository.Update(bankCard);

                    if (await _db.SaveAsync())
                        return NoContent();
                    else
                        return BadRequest("خطا در ثبت اطلاعات");
                }
                else
                {
                    _logger.LogError($"کاربر   {RouteData.Values["userId"]} قصد اپدیت به کارت دیگری را دارد");

                    return BadRequest("شما اجازه اپدیت کارت کاربر دیگری را ندارید");
                }
            }
            {
                return BadRequest("کارتی وجود ندارد");
            }
        }
        [Authorize(Policy = "RequireUserRole")]
        [HttpDelete(ApiV1Routes.BankCard.DeleteBankCard)]
        public async Task<IActionResult> DeleteBankCard(string id)
        {
            var bankCardFromRepo = await _db.BankCardRepository.GetByIdAsync(id);
            if (bankCardFromRepo != null)
            {
                if (bankCardFromRepo.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {
                    _db.BankCardRepository.Delete(bankCardFromRepo);

                    if (await _db.SaveAsync())
                        return NoContent();
                    else
                        return BadRequest("خطا در حذف اطلاعات");
                }
                else
                {
                    _logger.LogError($"کاربر   {RouteData.Values["userId"]} قصد حذف به کارت دیگری را دارد");

                    return BadRequest("شما اجازه حذف کارت کاربر دیگری را ندارید");
                }
            }
            else
            {
                return BadRequest("کارتی وجود ندارد");
            }
        }
    }
}