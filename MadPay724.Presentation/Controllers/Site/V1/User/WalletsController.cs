﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Wallet;
using MadPay724.Data.Models;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.User
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel")]
    [ApiController]
    [ServiceFilter(typeof(DocumentApproveFilter))]
    public class WalletsController : ControllerBase
    {
        private readonly IUnitOfWork<MadpayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<WalletsController> _logger;

        public WalletsController(IUnitOfWork<MadpayDbContext> dbContext, IMapper mapper,
            ILogger<WalletsController> logger)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
        }


        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Wallet.GetWallets)]
        public async Task<IActionResult> GetWallets(string userId)
        {
            var walletsFromRepo = await _db.WalletRepository
                .GetManyAsync(p => p.UserId == userId, s => s.OrderByDescending(x =>x.IsMain).ThenByDescending(x => x.IsSms), "");

            var bankcards = _mapper.Map<List<WalletForReturnDto>>(walletsFromRepo);

            return Ok(bankcards);
        }

        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Wallet.GetWallet, Name = "GetWallet")]
        public async Task<IActionResult> GetWallet(string id, string userId)
        {
            var walletFromRepo = await _db.WalletRepository.GetByIdAsync(id);
            if (walletFromRepo != null)
            {
                if (walletFromRepo.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {
                    var wallet = _mapper.Map<WalletForReturnDto>(walletFromRepo);

                    return Ok(wallet);
                }
                else
                {
                    _logger.LogError($"کاربر   {RouteData.Values["userId"]} قصد دسترسی به کیف پول دیگری را دارد");

                    return BadRequest("شما اجازه دسترسی به کیف پول کاربر دیگری را ندارید");
                }
            }
            else
            {
                return BadRequest("کیف پولی وجود ندارد");
            }

        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpPost(ApiV1Routes.Wallet.AddWallet)]
        public async Task<IActionResult> AddWallet(string userId, WalletForCreateDto walletForCreateDto)
        {
            var walletFromRepo = await _db.WalletRepository
                .GetAsync(p => p.Name == walletForCreateDto.Name && p.UserId == userId);
            var walletCount = await _db.WalletRepository.WalletCountAsync(userId);

            if (walletFromRepo == null)
            {
                if (walletCount <= 10)
                {
                    //var code = await _db.WalletRepository.GetLastWalletCodeAsync() +1;

                    //while (await _db.WalletRepository.WalletCodeExistAsync(code))
                    //{
                    //    code += 1;
                    //}
                    var cardForCreate = new Wallet()
                    {
                        UserId = userId,
                        IsBlock = false
                        // Code = code
                    };
                    var wallet = _mapper.Map(walletForCreateDto, cardForCreate);

                    await _db.WalletRepository.InsertAsync(wallet);

                    if (await _db.SaveAsync())
                    {
                        var walletForReturn = _mapper.Map<WalletForReturnDto>(wallet);

                        return CreatedAtRoute("GetWallet", new { id = wallet.Id, userId = userId }, walletForReturn);
                    }
                    else
                        return BadRequest("خطا در ثبت اطلاعات");
                }
                {
                    return BadRequest("شما اجازه وارد کردن بیش از 10 کیف پول را ندارید");
                }
            }
            {
                return BadRequest("این کیف پول قبلا ثبت شده است");
            }


        }
    }
}