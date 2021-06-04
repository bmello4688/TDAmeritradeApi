using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TDAmeritradeApi.Client.Tests
{
    [TestClass]
    public class AmeritradeClientTest
    {

        [TestMethod, TestCategory("Integration")]
        public async Task GetWatchlists_Returns_a_List_of_Watchlists()
        {
            var client = new TDAmeritradeClient("EXAMPLE", "uri");
            var watchlist = await client.WatchListsApi.GetWatchListsForAllAccountsAsync();
            Assert.IsNotNull(watchlist);
            Assert.IsTrue(watchlist.Count > 0);
        }
    }
}
