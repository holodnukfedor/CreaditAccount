using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CreditAccountBLL;
using CurrencyCodesResolver;

namespace CreditAccount.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountService _accountService;
        private ICurrencyCodesResolver _currencyCodesResolver;

        private ActionResult ProcessResult(Result<Dictionary<int, decimal>> result)
        {
            if (result.IsSuccess)
                return Ok(result.Data.Select(x => new BalanceVM(x, _currencyCodesResolver)));

            return BadRequest(result.ErrorMessage);
        }

        private ActionResult<Result<T>> ProcessResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
                return Ok(result.Data);

            return BadRequest(result.ErrorMessage);
        }

        public AccountController(IAccountService accountService, ICurrencyCodesResolver currencyCodesResolver)
        {
            _accountService = accountService;
            _currencyCodesResolver = currencyCodesResolver;
        }

        [HttpGet]
        public async Task<ActionResult<Result<Dictionary<int, decimal>>>> GetBalance(long userId)
        {
            Result<Dictionary<int, decimal>> result = await _accountService.GetBalance(userId);
            return ProcessResult(result);
        }

        [HttpPut]
        public async Task<ActionResult<Result<decimal>>> PutMoney(ChangeBalanceVM changeBalance)
        {
            int currencyCodeInt = _currencyCodesResolver.Resolve(changeBalance.CurrencyCode);
            Result<decimal> result = await _accountService.PutMoney(changeBalance.UserId, changeBalance.Amount, currencyCodeInt);
            return ProcessResult(result);
        }

        [HttpPut]
        public async Task<ActionResult<Result<decimal>>> WithDrawMoney(ChangeBalanceVM changeBalance)
        {
            int currencyCodeInt = _currencyCodesResolver.Resolve(changeBalance.CurrencyCode);
            Result<decimal> result = await _accountService.WithdrawMoney(changeBalance.UserId, changeBalance.Amount, currencyCodeInt);
            return ProcessResult(result);
        }

        [HttpPut]
        public async Task<ActionResult<Result<Dictionary<int, decimal>>>> ChangeCurrency(ChangeCurrencyVM changeCurrency)
        {
            int fromCurrencyCodeInt = _currencyCodesResolver.Resolve(changeCurrency.FromCurrencyCode);
            int toCurrencyCodeInt = _currencyCodesResolver.Resolve(changeCurrency.ToCurrencyCode);
            Result<Dictionary<int, decimal>> result = await _accountService.ChangeCurrency(changeCurrency.UserId, changeCurrency.Amount, fromCurrencyCodeInt, toCurrencyCodeInt);
            return ProcessResult(result);
        }
    }
}