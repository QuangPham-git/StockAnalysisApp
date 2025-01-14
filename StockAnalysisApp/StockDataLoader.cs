using System;
using System.Collections.Generic;
using System.IO;

namespace StockAnalysisApp
{
    /// <summary>
    /// Provides functionality to load stock data from a file and convert it into a list of <see cref="SmartCandlestick"/> objects.
    /// </summary>
    internal class StockDataLoader
    {
        /// <summary>
        /// Detects the delimiter used in the file based on the header row.
        /// </summary>
        /// <param name="header">The header line of the file, which contains column names.</param>
        /// <returns>
        /// A string representing the detected delimiter: tab (`\t`), comma (`,`), or space (` `).
        /// Returns <c>null</c> if no valid delimiter is found.
        /// </returns>
        private string DetectDelimiter(string header)
        {
            if (header.Contains("\t")) return "\t"; // Tab-delimited file
            if (header.Contains(",")) return ",";   // Comma-delimited file
            if (header.Contains(" ")) return " ";   // Space-delimited file
            return null;                            // No valid delimiter found
        }

        /// <summary>
        /// Loads stock data from the specified file path and converts it into a list of <see cref="SmartCandlestick"/> objects.
        /// </summary>
        /// <param name="filePath">The path of the file to load.</param>
        /// <returns>A list of <see cref="SmartCandlestick"/> objects containing the stock data.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="InvalidDataException">Thrown when the file format is invalid or no valid data is found.</exception>
        public List<SmartCandlestick> LoadStockDataFromPath(string filePath)
        {
            // Initialize a list to store the loaded stock data
            List<SmartCandlestick> stockDataList = new List<SmartCandlestick>();

            // Check if the file exists before attempting to read
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found.", filePath);

            // Open the file for reading using a StreamReader
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Read the header line to detect the delimiter
                string header = reader.ReadLine();
                string delimiter = DetectDelimiter(header);

                // Validate the delimiter and ensure the header has at least 6 columns
                if (delimiter == null || header.Split(delimiter.ToCharArray()).Length < 6)
                {
                    throw new InvalidDataException("The file format is invalid or missing required columns.");
                }

                // Read each line of the file until the end
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();              // Read the current line
                    string[] values = line.Split(delimiter.ToCharArray()); // Split the line into values using the detected delimiter

                    // Validate the number of columns in the line
                    if (values.Length < 6)
                    {
                        Console.WriteLine($"Skipping invalid line: {line}");
                        continue; // Skip lines with insufficient data
                    }

                    try
                    {
                        // Create a new SmartCandlestick object and populate its properties
                        SmartCandlestick data = new SmartCandlestick
                        {
                            Date = DateTime.Parse(values[0]),    // Parse the date
                            Open = decimal.Parse(values[1]),     // Parse the opening price
                            High = decimal.Parse(values[2]),     // Parse the highest price
                            Low = decimal.Parse(values[3]),      // Parse the lowest price
                            Close = decimal.Parse(values[4]),    // Parse the closing price
                            Volume = long.Parse(values[5])       // Parse the trading volume
                        };

                        // Add the parsed data to the list
                        stockDataList.Add(data);
                    }
                    catch (Exception ex)
                    {
                        // Log parsing errors and continue with the next line
                        Console.WriteLine($"Error parsing line: {line}\n{ex.Message}");
                        continue;
                    }
                }
            }

            // Check if the list contains any valid data
            if (stockDataList.Count == 0)
            {
                throw new InvalidDataException("No valid data could be loaded from the file.");
            }

            // Return the loaded stock data
            return stockDataList;
        }
    }
}
