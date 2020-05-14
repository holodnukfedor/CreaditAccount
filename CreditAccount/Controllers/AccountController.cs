using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CreditAccountBLL;
using CurrencyCodesResolver;
using CreditAccountDAL;

namespace CreditAccount.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountService _accountService;
        private ICurrencyCodesResolver _currencyCodesResolver;

        private ActionResult ProcessResult(Result<Balance[]> result)
        {
            if (result.IsSuccess)
                return Ok(result.Data.Select(x => new BalanceVM(x, _currencyCodesResolver)));

            return BadRequest(result.ErrorMessage);
        }

        private ActionResult ProcessResult(Result result)
        {
            if (result.IsSuccess)
                return Ok();

            return BadRequest(result.ErrorMessage);
        }

        public AccountController(IAccountService accountService, ICurrencyCodesResolver currencyCodesResolver)
        {
            _accountService = accountService;
            _currencyCodesResolver = currencyCodesResolver;
        }

        [HttpGet]
        public async Task<ActionResult> GetBalance(long userId)
        {
            Result<Balance[]> result = await _accountService.GetBalanceAsync(userId);
            return ProcessResult(result);
        }

        [HttpPut]
        public async Task<ActionResult> PutMoney(ChangeBalanceVM changeBalance)
        {
            int currencyCodeInt = _currencyCodesResolver.Resolve(changeBalance.CurrencyCode);
            Result result = await _accountService.PutMoneyAsync(changeBalance.UserId, changeBalance.Amount, currencyCodeInt);
            return ProcessResult(result);
        }

        [HttpPut]
        public async Task<ActionResult> WithDrawMoney(ChangeBalanceVM changeBalance)
        {
            int currencyCodeInt = _currencyCodesResolver.Resolve(changeBalance.CurrencyCode);
            Result result = await _accountService.WithdrawMoneyAsync(changeBalance.UserId, changeBalance.Amount, currencyCodeInt);
            return ProcessResult(result);
        }

        [HttpPut]
        public async Task<ActionResult> ChangeCurrency(ChangeCurrencyVM changeCurrency)
        {
            int fromCurrencyCodeInt = _currencyCodesResolver.Resolve(changeCurrency.FromCurrencyCode);
            int toCurrencyCodeInt = _currencyCodesResolver.Resolve(changeCurrency.ToCurrencyCode);
            Result result = await _accountService.ChangeCurrencyAsync(changeCurrency.UserId, changeCurrency.Amount, fromCurrencyCodeInt, toCurrencyCodeInt);
            return ProcessResult(result);
        }
    }
}