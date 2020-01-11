﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common.Pagination;
using MadPay724.Data.Dtos.Site.Panel.BankCards;
using MadPay724.Data.Dtos.Site.Panel.Users;
using MadPay724.Data.Dtos.Site.Panel.Wallet;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.Accountant
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Accountant")]
    [ApiController]
    public class InventoriesController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<InventoriesController> _logger;



        public InventoriesController(IUnitOfWork<Main_MadPayDbContext> dbContext,
            IMapper mapper,
            ILogger<InventoriesController> logger)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Accountant.GetInventories)]
        public async Task<IActionResult> GetInventories(string id, [FromQuery]PaginationDto paginationDto)
        {

            var usersFromRepo = await _db.UserRepository
                    .GetAllPagedListAsync(
                    paginationDto,
                    paginationDto.Filter.ToUserExpression(true),
                    paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                    "Wallets,Photos");//,BankCards

            Response.AddPagination(usersFromRepo.CurrentPage, usersFromRepo.PageSize,
                usersFromRepo.TotalCount, usersFromRepo.TotalPage);

            var users = new List<UserForAccountantDto>();

            foreach (var item in usersFromRepo)
            {
                users.Add(_mapper.Map<UserForAccountantDto>(item));
            }

            return Ok(users);
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Accountant.GetInventoryWallets)]
        public async Task<IActionResult> GetInventoryWallets(string userId)
        {

            var walletsFromRepo = await _db.WalletRepository
                .GetManyAsync(p => p.UserId == userId, s => s.OrderByDescending(x => x.IsMain).ThenByDescending(x => x.IsSms), "");

            var wallets = _mapper.Map<List<WalletForReturnDto>>(walletsFromRepo);

            return Ok(wallets);
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Accountant.GetInventoryBankCard)]
        public async Task<IActionResult> GetInventoryBankCard(string userId)
        {
            var bankCardsFromRepo = await _db.BankCardRepository
           .GetManyAsync(p => p.UserId == userId, s => s.OrderByDescending(x => x.Approve), "");


            var bankcards = _mapper.Map<List<BankCardForUserDetailedDto>>(bankCardsFromRepo);

            return Ok(bankcards);
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpPatch(ApiV1Routes.Accountant.BlockInventoryWallet)]
        public async Task<IActionResult> BlockInventoryWallet(string walletId, WalletBlockDto walletBlockDto)
        {
            var walletsFromRepo = await _db.WalletRepository.GetByIdAsync(walletId);
            walletsFromRepo.IsBlock = walletBlockDto.Block;
            _db.WalletRepository.Update(walletsFromRepo);

            if (await _db.SaveAsync())
            {
                return NoContent();
            }
            else
            {
                return BadRequest("خطا در تغییر بلاکی بودن کیف پول");
            }
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpPatch(ApiV1Routes.Accountant.ApproveInventoryWallet)]
        public async Task<IActionResult> ApproveInventoryWallet(string bankcardId, BankCardApproveDto bankCardApproveDto)
        {
            var bankcardFromRepo = await _db.BankCardRepository.GetByIdAsync(bankcardId);
            bankcardFromRepo.Approve = bankCardApproveDto.Approve;
            _db.BankCardRepository.Update(bankcardFromRepo);

            if (await _db.SaveAsync())
            {
                return NoContent();
            }
            else
            {
                return BadRequest("خطا در تغییر تاییدی بودن کارت");
            }
        }

    }
}