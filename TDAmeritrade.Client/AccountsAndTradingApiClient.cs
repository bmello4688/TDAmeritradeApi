using RestSharp;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using TDAmeritradeApi.Client.Models;
using TDAmeritradeApi.Client.Models.AccountsAndTrading;

namespace TDAmeritradeApi.Client
{
    public class AccountsAndTradingApiClient
    {
        private BaseApiClient baseApiClient;

        public AccountsAndTradingApiClient(BaseApiClient baseApiClient)
        {
            this.baseApiClient = baseApiClient;
        }

        public async Task<Account> GetAccountAsync(string accountID)
        {
            var parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("fields", "positions,orders"),
            };

            var partiallyLoadedAccount = await baseApiClient.SendRequest<Account>($"accounts/{accountID}", Method.GET, parameters);

            Account account = FullyLoadAccountInfo(partiallyLoadedAccount);

            return account;
        }

        public async Task<List<Account>> GetAllAccountsAsync()
        {
            var parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("fields", "positions,orders"),
            };

            var partiallyLoadedAccountList = await baseApiClient.SendRequest<List<Account>>($"accounts", Method.GET, parameters);

            List<Account> accounts = new List<Account>();
            foreach (var partiallyLoadedAccount in partiallyLoadedAccountList)
            {
                Account account = FullyLoadAccountInfo(partiallyLoadedAccount);

                accounts.Add(account);
            }

            return accounts;
        }

        private static Account FullyLoadAccountInfo(Account partiallyLoadedAccount)
        {
            JsonElement accountJson = partiallyLoadedAccount.securitiesAccount;

            string json = accountJson.GetRawText();

            var options = BaseApiClient.GetJsonSerializerOptions();

            Account account;
            if (json.Contains(AccountType.CASH.ToString()))
                account = JsonSerializer.Deserialize<CashAccount>(json, options);
            else
                account = JsonSerializer.Deserialize<MarginAccount>(json, options);

            return account;
        }

        public async Task<OrderStrategy> GetSavedOrderAsync(string accountID, string savedOrderID)
        {
            var savedOrder = await baseApiClient.SendRequest<OrderStrategy>($"accounts/{accountID}/savedorders/{savedOrderID}", Method.GET, null);

            return savedOrder;
        }

        public async Task<List<OrderStrategy>> GetAllSavedOrderAsync(string accountID)
        {
            var savedOrder = await baseApiClient.SendRequest<List<OrderStrategy>>($"accounts/{accountID}/savedorders", Method.GET, null);

            return savedOrder;
        }

        public async Task<OrderStrategy> GetOrderAsync(string accountID, string orderID)
        {
            var order = await baseApiClient.SendRequest<OrderStrategy>($"accounts/{accountID}/orders/{orderID}", Method.GET, null);

            return order;
        }

        public async Task<List<OrderStrategy>> GetAllOrdersAsync(string accountID = null, OrderStrategyStatusType? statusType = null, int maxResults = -1, DateTime? fromEnteredDate = null, DateTime? toEnteredDate = null)
        {
            var parameters = new List<KeyValuePair<string, string>>();

            if (string.IsNullOrWhiteSpace(accountID))
                parameters.Add(new KeyValuePair<string, string>("accountId", accountID));
            if (maxResults > 0)
                parameters.Add(new KeyValuePair<string, string>("maxResults", maxResults.ToString()));
            if (fromEnteredDate.HasValue)
                parameters.Add(new KeyValuePair<string, string>("fromEnteredTime", fromEnteredDate.Value.ToString("yyyy-MM-dd")));
            if (toEnteredDate.HasValue)
                parameters.Add(new KeyValuePair<string, string>("toEnteredTime", toEnteredDate.Value.ToString("yyyy-MM-dd")));
            if (statusType.HasValue)
                parameters.Add(new KeyValuePair<string, string>("status", statusType.Value.ToString()));

            List<OrderStrategy> orders;
            if (string.IsNullOrWhiteSpace(accountID))
                orders = await baseApiClient.SendRequest<List<OrderStrategy>>($"orders", Method.GET, parameters);
            else
                orders = await baseApiClient.SendRequest<List<OrderStrategy>>($"accounts/{accountID}/orders", Method.GET, parameters);

            return orders;
        }

        public void EnsureValidOrderStrategy(OrderStrategy orderStrategy)
        {
            //https://developer.tdameritrade.com/content/place-order-samples

            bool isOrderLegValid = true;
            foreach (var orderLeg in orderStrategy.orderLegCollection)
            {
                if (orderLeg.orderLegType == InstrumentAssetType.EQUITY)
                    isOrderLegValid = orderLeg.instruction == OrderInstructionType.BUY ||
                                        orderLeg.instruction == OrderInstructionType.SELL ||
                                        orderLeg.instruction == OrderInstructionType.BUY_TO_COVER ||
                                        orderLeg.instruction == OrderInstructionType.SELL_SHORT;
                else if (orderLeg.orderLegType == InstrumentAssetType.OPTION)
                    isOrderLegValid = orderLeg.instruction == OrderInstructionType.BUY_TO_OPEN ||
                                        orderLeg.instruction == OrderInstructionType.BUY_TO_CLOSE ||
                                        orderLeg.instruction == OrderInstructionType.SELL_TO_OPEN ||
                                        orderLeg.instruction == OrderInstructionType.SELL_TO_CLOSE;

                if (!isOrderLegValid)
                    throw new Exception($"Invalid order leg instruction: {orderLeg.instruction.ToString()} for type {Enum.GetName(orderLeg.orderLegType)} for symbol: {orderLeg.instrument.symbol}");
            }
        }

        public async Task PlaceOrderAsync(string accountID, OrderStrategy orderStrategy)
        {
            EnsureValidOrderStrategy(orderStrategy);

            await baseApiClient.SendRequest($"accounts/{accountID}/orders", Method.POST, null, orderStrategy);
        }

        public async Task ReplaceOrderAsync(string accountID, long orderId, OrderStrategy newOrderStrategy)
        {
            await baseApiClient.SendRequest($"accounts/{accountID}/orders/{orderId}", Method.PUT, null, newOrderStrategy);
        }

        public async Task CancelOrderAsync(string accountID, long orderId)
        {
            await baseApiClient.SendRequest($"accounts/{accountID}/orders/{orderId}", Method.DELETE, null);
        }

        public async Task<List<Transaction>> GetTransactionsAsync(string accountID, TransactionFilterType transactionType = TransactionFilterType.ALL, string symbol=null, DateTime? startDate=null, DateTime? endDate=null)
        {
            var parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("type", transactionType.ToString()),
            };

            if (string.IsNullOrWhiteSpace(symbol))
                parameters.Add(new KeyValuePair<string, string>("symbol", symbol));
            if (startDate.HasValue)
                parameters.Add(new KeyValuePair<string, string>("startDate", startDate.Value.ToString("yyyy-MM-dd")));
            if (endDate.HasValue)
                parameters.Add(new KeyValuePair<string, string>("endDate", endDate.Value.ToString("yyyy-MM-dd")));


            var transactions = await baseApiClient.SendRequest<List<Transaction>>($"accounts/{accountID}/transactions", Method.GET, parameters);

            return transactions;
        }
    }
}
