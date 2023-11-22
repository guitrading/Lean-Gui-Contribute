/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using System.Security.Cryptography;
using QuantConnect.Data;
using QuantConnect.Data.Market;

namespace QuantConnect.Indicators;

/// https://www.investopedia.com/articles/trading/10/premier_stochastic_oscillator_explained.asp
/// https://www.tradingview.com/script/xewuyTA1-Indicator-Premier-Stochastic-Oscillator/

/// <summary>
/// This indicator normalizes the standard stochastic oscillator by applying a double exponential
/// smoothing average of the %K value, resulting in a value that ranges from 0 to 100.
/// </summary>
public class PremierStochasticOscillator : BarIndicator, IIndicatorWarmUpPeriodProvider
{
    // private readonly IndicatorBase<IndicatorDataPoint> _maximum;
    private readonly ExponentialMovingAverage _ema;
    private readonly Stochastic _stoch;

    /// <summary>
    /// Gets the value of the Premier Stochastic Oscillator given Period.
    /// </summary>
    public IndicatorBase<IBaseDataBar> PSO { get; }

    /// <summary>
    /// Create a new Premier Stochastic Oscillator with the specified periods.
    /// </summary>
    /// <param name="name">The name of this indicator.</param>
    /// <param name="period">The period given to calculate the Premier Stochastic Oscillator</param>
    public PremierStochasticOscillator(string name, int period)
        : base(name)
    {
        _ema = new ExponentialMovingAverage(5);
        _stoch = new Stochastic(period, period, period);
        PSO = new FunctionalIndicator<IBaseDataBar>(name + "_PSO",
            input => ComputePSO(period, input),
            isReady => _ema.IsReady && _stoch.IsReady, 
            () =>
            {
            }
        );

        WarmUpPeriod = period;
    }

    /// <summary>
    /// Creates a new <see cref="Premier Stochastic Oscillator"/> indicator from the specified inputs
    /// </summary>
    /// <param name="period"></param>The period given to calculate the Premier Stochastic Oscillator
    public PremierStochasticOscillator(int period)
        : this($"PSO({period})", period)
    {
    }
    
    /// <summary>
    /// Gets a flag indicating when this indicator is ready and fully initialized
    /// </summary>
    public override bool IsReady => PSO.IsReady;
    
    /// <summary>
    /// Required period, in data points, for the indicator to be ready and fully initialized.
    /// </summary>
    public int WarmUpPeriod { get; }
    
    /// <summary>
    /// Computes the next value of this indicator from the given state
    /// </summary>
    /// <param name="input">The input given to the indicator</param>
    protected override decimal ComputeNextValue(IBaseDataBar input)
    {
        PSO.Update(input);
        return PSO.Current.Value;
    }
    
    /// <summary>
    /// Computes the Premier Stochastic Oscillator.
    /// </summary>
    /// <param name="period">The period.</param>
    /// <param name="input">The input.</param>
    /// <returns>The Premier Stochastic Oscillator value.</returns>
    private decimal ComputePSO(int period, IBaseDataBar input)
    {
        var k = _stoch.StochK.Current.Value;
        var nsk = 0.1m * (k - 50);

        _ema.Update(new IndicatorDataPoint(DateTime.UtcNow, nsk));
        var ss = _ema.Current.Value;
        var expss = Math.Exp((double)ss);
        var pso = ((expss - 1) / (expss + 1));
        
        return (decimal)pso;
    }

}
