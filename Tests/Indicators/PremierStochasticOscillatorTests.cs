using NUnit.Framework;
using QuantConnect.Data.Market;
using QuantConnect.Indicators;

namespace QuantConnect.Tests.Indicators
{
    [TestFixture]
    public class PremierStochasticOscillatorTests : CommonIndicatorTests<IBaseDataBar>
    {
        protected override IndicatorBase<IBaseDataBar> CreateIndicator()
        {
            // Adjust the period as needed for your tests
            return new PremierStochasticOscillator(14);
        }

        protected override string TestFileName => "spy_pso_formatted.csv"; // You need to provide a test file with data
        protected override string TestColumnName => "PSO"; // Adjust if your file has a different column name for PSO values

        // You can add additional tests specific to the PremierStochasticOscillator here
        [Test]
        public void ComparesAgainstExternalData()
        {
            var pso = CreateIndicator();
            
            // Load data from a file or create data manually to test the PSO
            // Example: pso.Update(new TradeBar{...});
            // Assert.AreEqual(expectedValue, pso.Current.Value);
        }

        // More tests can be added to validate specific behaviors of your indicator
    }
}
