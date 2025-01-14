using System;

namespace StockAnalysisApp
{
    /// <summary>
    /// Represents basic stock data for a single trading day.
    /// </summary>
    internal class StockData
    {
        /// <summary>
        /// Gets or sets the date of the trading day.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the opening price of the stock.
        /// </summary>
        public decimal Open { get; set; }

        /// <summary>
        /// Gets or sets the highest price of the stock during the trading day.
        /// </summary>
        public decimal High { get; set; }

        /// <summary>
        /// Gets or sets the lowest price of the stock during the trading day.
        /// </summary>
        public decimal Low { get; set; }

        /// <summary>
        /// Gets or sets the closing price of the stock.
        /// </summary>
        public decimal Close { get; set; }

        /// <summary>
        /// Gets or sets the trading volume for the day.
        /// </summary>
        public long Volume { get; set; }
    }

    /// <summary>
    /// Represents a candlestick with additional smart calculations for analyzing patterns.
    /// Inherits from the <see cref="StockData"/> class.
    /// </summary>
    internal class SmartCandlestick : StockData
    {
        /// <summary>
        /// Gets the range of the candlestick, calculated as the difference between the high and low prices.
        /// </summary>
        public decimal Range => High - Low;

        /// <summary>
        /// Gets the body range of the candlestick, calculated as the absolute difference between the close and open prices.
        /// </summary>
        public decimal BodyRange => Math.Abs(Close - Open);

        /// <summary>
        /// Gets the top price of the candlestick, which is the maximum of the open and close prices.
        /// </summary>
        public decimal TopPrice => Math.Max(Open, Close);

        /// <summary>
        /// Gets the bottom price of the candlestick, which is the minimum of the open and close prices.
        /// </summary>
        public decimal BottomPrice => Math.Min(Open, Close);

        /// <summary>
        /// Gets the upper tail (wick) length, calculated as the difference between the high and the top price.
        /// </summary>
        public decimal UpperTail => High - TopPrice;

        /// <summary>
        /// Gets the lower tail (wick) length, calculated as the difference between the bottom price and the low.
        /// </summary>
        public decimal LowerTail => BottomPrice - Low;

        /// <summary>
        /// Determines the candlestick pattern based on its price characteristics.
        /// </summary>
        /// <returns>
        /// A string representing the type of candlestick pattern:
        /// - "Doji" if the body range is zero.
        /// - "Marubozu" if there are no upper or lower tails.
        /// - "Hammer" if the lower tail is more than twice the body range.
        /// - "Gravestone Doji" if the upper tail is more than twice the body range.
        /// - "Dragonfly Doji" if both the upper and lower tails are greater than the body range.
        /// - "Bullish" if the close price is higher than the open price.
        /// - "Bearish" if the close price is lower than the open price.
        /// - "Neutral" if none of the conditions match.
        /// </returns>
        public string GetPattern()
        {
            if (BodyRange == 0)
            {
                // If the open and close prices are the same, it’s a Doji pattern.
                return "Doji";
            }
            if (UpperTail == 0 && LowerTail == 0)
            {
                // If there are no upper and lower tails, it’s a Marubozu pattern.
                return "Marubozu";
            }
            if (LowerTail > 2 * BodyRange)
            {
                // If the lower tail is more than twice the body range, it’s a Hammer pattern.
                return "Hammer";
            }
            if (UpperTail > 2 * BodyRange)
            {
                // If the upper tail is more than twice the body range, it’s a Gravestone Doji.
                return "Gravestone Doji";
            }
            if (UpperTail > BodyRange && LowerTail > BodyRange)
            {
                // If both tails are greater than the body range, it’s a Dragonfly Doji.
                return "Dragonfly Doji";
            }
            if (Close > Open)
            {
                // If the close price is higher than the open price, it’s a Bullish candlestick.
                return "Bullish";
            }
            if (Close < Open)
            {
                // If the close price is lower than the open price, it’s a Bearish candlestick.
                return "Bearish";
            }

            // If none of the above conditions are met, it’s a Neutral candlestick.
            return "Neutral";
        }
    }

}
