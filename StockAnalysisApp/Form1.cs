using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace StockAnalysisApp
{
    public partial class Form1 : Form
    {
        // Holds the current stock data displayed on the main chart
        private List<SmartCandlestick> currentStockData;

        // Dictionary to store additional stock charts with their associated forms
        private Dictionary<string, Tuple<Form, System.Windows.Forms.DataVisualization.Charting.Chart>> stockCharts;

        // Tracks the starting index of the wave selected by the user
        private int waveStartIndex = -1;

        // Tracks the ending index of the wave selected by the user
        private int waveEndIndex = -1;

        // Provides visual feedback when selecting a wave (rectangle annotation)
        private RectangleAnnotation selectionRectangle = null;

        private decimal tolerancePercentage = 0.01m; // Default leeway (1%)
        
        /// <summary>
        /// Initializes a new instance of the Form1 class.
        /// Sets up event handlers, initializes the chart, and sets default date ranges.
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            // Assign mouse event handlers for the stock chart
            chart_stockData.MouseDown += chart_stockData_MouseDown;
            chart_stockData.MouseMove += chart_stockData_MouseMove;
            chart_stockData.MouseUp += chart_stockData_MouseUp;

            // Initialize the main chart with OHLC and Volume chart areas
            InitializeChart(chart_stockData);

            // Initialize the dictionary to manage multiple stock charts
            stockCharts = new Dictionary<string, Tuple<Form, System.Windows.Forms.DataVisualization.Charting.Chart>>();

            // Set default start and end dates for the date pickers
            datePicker_start.Value = DateTime.Parse("10/01/2024");
            datePicker_end.Value = DateTime.Parse("11/15/2024");
        }

        /// <summary>
        /// Initializes the specified chart with OHLC (candlestick) and Volume chart areas and series.
        /// </summary>
        /// <param name="chart">The chart control to be initialized.</param>
        private void InitializeChart(System.Windows.Forms.DataVisualization.Charting.Chart chart)
        {
            // Clear any existing series and chart areas to avoid duplication
            chart.Series.Clear();
            chart.ChartAreas.Clear();

            // Define the OHLC (candlestick) chart area for price data
            var ohlcArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea("ChartArea_OHLC")
            {
                // Set the position of the OHLC chart area (occupies the top 65% of the chart)
                Position = new System.Windows.Forms.DataVisualization.Charting.ElementPosition(0, 0, 100, 65),

                // Set inner margins to ensure the chart content does not touch the edges
                InnerPlotPosition = new System.Windows.Forms.DataVisualization.Charting.ElementPosition(10, 10, 80, 80)
            };

            // Format the X-axis labels to display dates in "MMM dd" format
            ohlcArea.AxisX.LabelStyle.Format = "MMM dd";

            // Disable major grid lines on the X-axis for cleaner visualization
            ohlcArea.AxisX.MajorGrid.Enabled = false;

            // Allow the Y-axis to dynamically adjust its starting point
            ohlcArea.AxisY.IsStartedFromZero = false;

            // Add the OHLC chart area to the chart
            chart.ChartAreas.Add(ohlcArea);

            // Define the Volume chart area for displaying volume data
            var volumeArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea("ChartArea_Volume")
            {
                // Set the position of the Volume chart area (occupies the bottom 30% of the chart)
                Position = new System.Windows.Forms.DataVisualization.Charting.ElementPosition(0, 65, 100, 30),

                // Set inner margins to ensure the chart content does not touch the edges
                InnerPlotPosition = new System.Windows.Forms.DataVisualization.Charting.ElementPosition(10, 10, 80, 80)
            };

            // Enable X-axis labels for the volume chart area
            volumeArea.AxisX.LabelStyle.Enabled = true;

            // Rotate X-axis labels for better readability
            volumeArea.AxisX.LabelStyle.Angle = -45;

            // Ensure the Y-axis starts from zero for volume data
            volumeArea.AxisY.IsStartedFromZero = true;

            // Add the Volume chart area to the chart
            chart.ChartAreas.Add(volumeArea);

            // Add a candlestick series to the chart for OHLC data
            var candlestickSeries = chart.Series.Add("Candlestick");
            candlestickSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Candlestick;
            candlestickSeries.ChartArea = "ChartArea_OHLC";
            candlestickSeries["PriceUpColor"] = "Green"; // Green color for bullish candlesticks
            candlestickSeries["PriceDownColor"] = "Red"; // Red color for bearish candlesticks

            // Add a column series to the chart for volume data
            var volumeSeries = chart.Series.Add("Volume");
            volumeSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            volumeSeries.ChartArea = "ChartArea_Volume";
            volumeSeries.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;

            // Remove the legend for a cleaner chart display
            chart.Legends.Clear();
        }

        /// <summary>
        /// Sets up the chart for displaying the Beauty plot.
        /// </summary>
        /// <param name="chart">The chart control where the Beauty plot will be displayed.</param>
        private void SetupBeautyChart(System.Windows.Forms.DataVisualization.Charting.Chart chart)
        {
            // Try to find an existing "Beauty" series in the chart
            var beautySeries = chart.Series.FindByName("Beauty");

            // If the "Beauty" series does not exist, create a new one
            if (beautySeries == null)
            {
                // Add a new line series for displaying Beauty values
                beautySeries = chart.Series.Add("Beauty");
                beautySeries.ChartType = SeriesChartType.Line;
                beautySeries.ChartArea = "ChartArea_Volume";
                beautySeries.Color = Color.Blue;  // Set the line color to blue
                beautySeries.BorderWidth = 2;     // Set the line width
                beautySeries.IsVisibleInLegend = false; // Hide the legend entry for this series
            }

            // Customize the Y-axis and X-axis titles for the Volume chart area
            var volumeArea = chart.ChartAreas["ChartArea_Volume"];
            volumeArea.AxisY.Title = "Beauty";      // Set the Y-axis title to "Beauty"
            volumeArea.AxisX.Title = "Price";       // Set the X-axis title to "Price"
            volumeArea.AxisY.IsStartedFromZero = false; // Allow dynamic starting point for the Y-axis
            volumeArea.AxisX.LabelStyle.Format = "C";   // Format X-axis labels as currency
        }

        /// <summary>
        /// Normalizes the Y-axis range for the OHLC chart area based on the stock data.
        /// </summary>
        /// <param name="chart">The chart control to normalize.</param>
        /// <param name="stockData">The list of SmartCandlestick data used to determine the Y-axis range.</param>
        private void NormalizeChart(System.Windows.Forms.DataVisualization.Charting.Chart chart, List<SmartCandlestick> stockData)
        {
            // If the stock data is null or empty, exit the method
            if (stockData == null || stockData.Count == 0) return;

            // Find the maximum and minimum high and low prices in the stock data
            decimal maxY = stockData.Max(data => data.High);
            decimal minY = stockData.Min(data => data.Low);

            // Ensure that maxY and minY are not the same to avoid zero range
            if (maxY == minY)
            {
                maxY += 1; // Increase the maximum value by 1
                minY -= 1; // Decrease the minimum value by 1
            }

            // Add a 5% padding to the range for better visualization
            decimal rangePadding = (maxY - minY) * 0.05m;
            double yMax = (double)(maxY + rangePadding);
            double yMin = (double)(minY - rangePadding);

            // Safety check to ensure yMax is greater than yMin
            if (yMax <= yMin)
            {
                yMax = yMin + 1;
            }

            // Set the Y-axis range for the OHLC chart area
            chart.ChartAreas["ChartArea_OHLC"].AxisY.Maximum = yMax;
            chart.ChartAreas["ChartArea_OHLC"].AxisY.Minimum = yMin;

            // Calculate a reasonable interval for the Y-axis
            double interval = (yMax - yMin) / 10;

            // If the interval is zero or negative, default to 1
            if (interval <= 0)
            {
                interval = 1;
            }

            // Set the Y-axis interval
            chart.ChartAreas["ChartArea_OHLC"].AxisY.Interval = interval;

            // Ensure the X-axis interval is set to 1
            chart.ChartAreas["ChartArea_OHLC"].AxisX.Interval = 1;

            // Debugging message to confirm the new Y-axis range and interval
            MessageBox.Show($"Normalized Chart Y-Axis: Min = {yMin}, Max = {yMax}, Interval = {interval}",
                            "Debug: NormalizeChart", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }









        /// <summary>
        /// Normalizes the Y-axis of the Beauty chart based on the provided beauty values.
        /// This ensures that the chart displays a reasonable range and interval.
        /// </summary>
        /// <param name="chart">The chart control where the Beauty series is plotted.</param>
        /// <param name="beautyValues">A list of double values representing beauty scores to be displayed.</param>
        private void NormalizeBeautyChart(System.Windows.Forms.DataVisualization.Charting.Chart chart, List<double> beautyValues)
        {
            // If there are no beauty values, exit the method
            if (beautyValues == null || beautyValues.Count == 0) return;

            // Calculate the maximum beauty value with a 10% padding
            double maxY = beautyValues.Max() * 1.1;

            // Calculate the minimum beauty value with a 10% reduction, ensuring it is non-negative
            double minY = Math.Max(beautyValues.Min() * 0.9, 0);

            // Ensure the maximum Y value is greater than the minimum Y value
            if (maxY <= minY)
            {
                maxY = minY + 1; // Adjust maxY to be slightly greater than minY
            }

            // Calculate a reasonable interval for the Y-axis (divide the range by 10)
            double interval = (maxY - minY) / 10;

            // Ensure the interval is not too small; set a minimum interval of 0.1
            if (interval <= 0.1)
            {
                interval = 0.1;
            }

            // Set the Y-axis maximum, minimum, and interval values for the Beauty chart area
            var volumeArea = chart.ChartAreas["ChartArea_Volume"];
            volumeArea.AxisY.Maximum = maxY;
            volumeArea.AxisY.Minimum = minY;
            volumeArea.AxisY.Interval = interval;
        }

        /// <summary>
        /// Populates the specified chart with stock data and optionally Beauty values.
        /// Handles filtering data by date and displays OHLC (candlestick) or Volume series accordingly.
        /// </summary>
        /// <param name="chart">The chart control to populate with data.</param>
        /// <param name="stockData">A list of SmartCandlestick objects representing the stock data.</param>
        private void PopulateChart(System.Windows.Forms.DataVisualization.Charting.Chart chart, List<SmartCandlestick> stockData)
        {
            // Get the start and end dates from the date pickers
            DateTime startDate = datePicker_start.Value.Date;
            DateTime endDate = datePicker_end.Value.Date;

            // Filter the stock data within the selected date range and sort it by date
            var filteredData = stockData.Where(data => data.Date >= startDate && data.Date <= endDate).OrderBy(data => data.Date).ToList();

            // If there is no data after filtering, clear the chart and show a warning message
            if (filteredData.Count == 0)
            {
                chart.Series.Clear();         // Clear all series
                chart.Annotations.Clear();    // Clear all annotations
                MessageBox.Show("No data available for the selected date range.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Clear points in the Candlestick series if it exists
            if (!chart.Series.IsUniqueName("Candlestick"))
            {
                chart.Series["Candlestick"].Points.Clear();
            }

            // Find the Candlestick series and populate it with filtered data
            var candlestickSeries = chart.Series.FindByName("Candlestick");
            if (candlestickSeries != null)
            {
                candlestickSeries.Points.Clear();
                for (int i = 0; i < filteredData.Count; i++)
                {
                    // Add OHLC values (Open, High, Low, Close) for each candlestick
                    candlestickSeries.Points.AddXY(i, filteredData[i].Low, filteredData[i].High, filteredData[i].Open, filteredData[i].Close);
                }
            }

            // Handle the Beauty series if it exists
            if (chart.Series.IsUniqueName("Beauty"))
            {
                // Remove the Volume series if it exists when displaying Beauty
                if (!chart.Series.IsUniqueName("Volume"))
                {
                    chart.Series.Remove(chart.Series["Volume"]);
                }

                // Set up the Beauty chart area for plotting Beauty values
                SetupBeautyChart(chart);

                // Find the Beauty series and populate it with computed Beauty values
                var beautySeries = chart.Series["Beauty"];
                beautySeries.Points.Clear();

                // Example calculation for Beauty values (replace with actual logic if needed)
                List<double> beautyValues = filteredData.Select(c => (double)(c.High - c.Low)).ToList();

                for (int i = 0; i < beautyValues.Count; i++)
                {
                    // Add beauty values to the Beauty series
                    beautySeries.Points.AddXY(i, beautyValues[i]);
                }

                // Normalize the Beauty chart for better visualization
                NormalizeBeautyChart(chart, beautyValues);
            }
            else
            {
                // Safely find and populate the Volume series if Beauty is not displayed
                var volumeSeries = chart.Series.FindByName("Volume");
                if (volumeSeries != null)
                {
                    volumeSeries.Points.Clear();
                    for (int i = 0; i < filteredData.Count; i++)
                    {
                        volumeSeries.Points.AddXY(i, filteredData[i].Volume);
                    }
                }
            }

            // Normalize the main chart to adjust the Y-axis based on filtered data
            NormalizeChart(chart, filteredData);

            // Add peak and valley annotations to the chart based on the filtered data
            AddPeakValleyAnnotations(chart, filteredData);

            // Set custom X-axis labels for the candlestick chart area
            var axisX = chart.ChartAreas["ChartArea_OHLC"].AxisX;
            axisX.CustomLabels.Clear();
            axisX.IsReversed = false;
            for (int i = 0; i < filteredData.Count; i++)
            {
                axisX.CustomLabels.Add(i - 0.5, i + 0.5, filteredData[i].Date.ToString("MMM dd"));
            }

            // Set custom X-axis labels for the Beauty chart area
            var volumeAxisX = chart.ChartAreas["ChartArea_Volume"].AxisX;
            volumeAxisX.CustomLabels.Clear();
            for (int i = 0; i < filteredData.Count; i++)
            {
                volumeAxisX.CustomLabels.Add(i - 0.5, i + 0.5, filteredData[i].Date.ToString("MMM dd"));
            }
        }

        /// <summary>
        /// Handles the Load Data button click event to load stock data from selected CSV files.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data for the click event.</param>
        private void button_loadData_Click(object sender, EventArgs e)
        {
            // Configure the OpenFileDialog to select multiple CSV files from the "Stock Data" folder
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Stock Data"),
                Filter = "CSV files (*.csv)|*.csv",
                Multiselect = true
            };

            // Show the dialog and check if the user selected any files
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] selectedFiles = openFileDialog.FileNames;

                // Loop through each selected file
                for (int i = 0; i < selectedFiles.Length; i++)
                {
                    string filePath = selectedFiles[i];
                    string stockSymbol = Path.GetFileNameWithoutExtension(filePath);
                    StockDataLoader loader = new StockDataLoader();
                    List<SmartCandlestick> stockData;

                    try
                    {
                        // Load stock data from the file
                        stockData = loader.LoadStockDataFromPath(filePath);

                        // Check if the data is valid
                        if (stockData == null || stockData.Count == 0)
                        {
                            MessageBox.Show($"No valid data in the file: {stockSymbol}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions during file loading
                        MessageBox.Show($"Error loading file {stockSymbol}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    if (i == 0)
                    {
                        // Load the first file into the main chart
                        currentStockData = stockData;
                        PopulateChart(chart_stockData, stockData);
                    }
                    else
                    {
                        // For subsequent files, open new chart forms to display the data
                        OpenNewChartForm(stockSymbol, stockData);
                    }
                }
            }
        }





        /// <summary>
        /// Opens a new form to display a stock chart for the specified stock symbol and data.
        /// </summary>
        /// <param name="stockSymbol">The symbol representing the stock (e.g., "AAPL").</param>
        /// <param name="stockData">A list of SmartCandlestick objects representing the stock's data.</param>
        private void OpenNewChartForm(string stockSymbol, List<SmartCandlestick> stockData)
        {
            // Create a new form for displaying the chart
            Form newForm = new Form
            {
                Text = $"Stock Data: {stockSymbol}", // Set the form's title to include the stock symbol
                Size = new Size(1000, 600)           // Set the form size to 1000x600 pixels
            };

            // Create a new chart control and dock it to fill the form
            var newChart = new System.Windows.Forms.DataVisualization.Charting.Chart
            {
                Dock = DockStyle.Fill // Make the chart fill the entire form
            };

            // Add the new chart control to the form
            newForm.Controls.Add(newChart);

            // Initialize the chart with the required areas and series
            InitializeChart(newChart);

            // Populate the chart with the provided stock data
            PopulateChart(newChart, stockData);

            // Store the new form and chart in the stockCharts dictionary using the stock symbol as the key
            stockCharts[stockSymbol] = new Tuple<Form, System.Windows.Forms.DataVisualization.Charting.Chart>(newForm, newChart);

            // Display the new form
            newForm.Show();
        }

        /// <summary>
        /// Clears existing annotations and horizontal lines from the specified chart.
        /// This method is used to remove peak and valley markers.
        /// </summary>
        /// <param name="chart">The chart control from which to remove annotations.</param>
        /// <param name="stockData">A list of SmartCandlestick objects representing the stock's data.</param>
        private void AddPeakValleyAnnotations(System.Windows.Forms.DataVisualization.Charting.Chart chart, List<SmartCandlestick> stockData)
        {
            // Clear all existing annotations on the chart
            chart.Annotations.Clear();

            // Find and remove any existing horizontal line series (those with names starting with "HorizontalLine_")
            var horizontalLineSeries = chart.Series.Where(s => s.Name.StartsWith("HorizontalLine_")).ToList();
            foreach (var series in horizontalLineSeries)
            {
                chart.Series.Remove(series);
            }

            // If you want to completely remove the functionality, 
            // you can delete or comment out the loop below.
            /*
            for (int i = 1; i < stockData.Count - 1; i++)
            {
                // Detect peaks (this part is now commented out)
                if (stockData[i].High > stockData[i - 1].High && stockData[i].High > stockData[i + 1].High)
                {
                    AddAnnotation(chart, i, (double)stockData[i].High, Color.Green, "Peak");
                    AddHorizontalLine(chart, i, (double)stockData[i].High, Color.Green, stockData.Count);
                }

                // Detect valleys (this part is now commented out)
                if (stockData[i].Low < stockData[i - 1].Low && stockData[i].Low < stockData[i + 1].Low)
                {
                    AddAnnotation(chart, i, (double)stockData[i].Low, Color.Red, "Valley");
                    AddHorizontalLine(chart, i, (double)stockData[i].Low, Color.Red, stockData.Count);
                }
            }
            */
        }

        /*
        /// <summary>
        /// Adds a text annotation to the chart to label a peak or valley.
        /// </summary>
        /// <param name="chart">The chart control where the annotation will be added.</param>
        /// <param name="index">The index of the candlestick to annotate.</param>
        /// <param name="price">The price value where the annotation will be placed.</param>
        /// <param name="color">The color of the annotation text.</param>
        /// <param name="label">The text label for the annotation (e.g., "Peak" or "Valley").</param>
        private void AddAnnotation(System.Windows.Forms.DataVisualization.Charting.Chart chart, int index, double price, Color color, string label)
        {
            var textAnnotation = new System.Windows.Forms.DataVisualization.Charting.TextAnnotation
            {
                AxisX = chart.ChartAreas["ChartArea_OHLC"].AxisX,
                AxisY = chart.ChartAreas["ChartArea_OHLC"].AxisY,
                Text = label,
                ForeColor = color,
                X = index,
                Y = price,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Alignment = ContentAlignment.TopCenter
            };
            chart.Annotations.Add(textAnnotation);
        }
        */

        /*
        /// <summary>
        /// Adds a horizontal line to the chart to mark the price level of a peak or valley.
        /// </summary>
        /// <param name="chart">The chart control where the horizontal line will be added.</param>
        /// <param name="index">The index of the candlestick where the line starts.</param>
        /// <param name="price">The price value at which the line will be drawn.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="dataCount">The total number of data points in the chart.</param>
        private void AddHorizontalLine(System.Windows.Forms.DataVisualization.Charting.Chart chart, int index, double price, Color color, int dataCount)
        {
            var lineSeries = new System.Windows.Forms.DataVisualization.Charting.Series($"HorizontalLine_{price}")
            {
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
                Color = color,
                BorderWidth = 2,
                IsVisibleInLegend = false,
                ChartArea = "ChartArea_OHLC"
            };

            // Remove the series if it already exists
            if (!chart.Series.IsUniqueName(lineSeries.Name))
            {
                chart.Series.Remove(chart.Series[lineSeries.Name]);
            }

            // Add the series to the chart
            chart.Series.Add(lineSeries);

            // Draw a line spanning the entire X-axis range of the chart
            lineSeries.Points.AddXY(0, price);             // Start point at index 0
            lineSeries.Points.AddXY(dataCount - 1, price); // End point at the last index
        }
        */

        /// <summary>
        /// Handles the MouseDown event on the stock chart to begin selecting a wave.
        /// Creates a rectangle annotation to visually indicate the selection.
        /// </summary>
        /// <param name="sender">The chart control where the event occurred.</param>
        /// <param name="e">Mouse event data containing the mouse position.</param>
        private void chart_stockData_MouseDown(object sender, MouseEventArgs e)
        {
            var chart = (System.Windows.Forms.DataVisualization.Charting.Chart)sender;
            var chartArea = chart.ChartAreas["ChartArea_OHLC"];

            // Translate the mouse's pixel position to the corresponding X-axis value
            double xAxisValue = chartArea.AxisX.PixelPositionToValue(e.X);

            // Clear any existing rectangle annotations to avoid duplicates
            var existingAnnotations = chart.Annotations.OfType<RectangleAnnotation>().ToList();
            foreach (var annotation in existingAnnotations)
            {
                chart.Annotations.Remove(annotation);
            }

            // Store the starting index for the wave based on the X-axis value
            waveStartIndex = (int)Math.Round(xAxisValue);

            // Get the current Y-axis range for the candlestick chart
            double yMin = chartArea.AxisY.Minimum; // Bottom of the chart area
            double yMax = chartArea.AxisY.Maximum; // Top of the chart area

            // Calculate padding to ensure the rectangle reaches the X-axis
            double padding = Math.Max((yMax - yMin) * 0.05, (chartArea.AxisY.PixelPositionToValue(chartArea.Position.Bottom) - yMin));
            double extendedYMin = yMin - padding;

            // Create a new rectangle annotation to indicate the selected wave
            selectionRectangle = new RectangleAnnotation
            {
                AxisX = chartArea.AxisX,
                AxisY = chartArea.AxisY,
                LineColor = Color.Blue,
                LineWidth = 2,
                BackColor = Color.FromArgb(50, Color.Blue),
                ClipToChartArea = "ChartArea_OHLC",
                X = waveStartIndex,
                Y = yMax,
                Width = 0,                   // Initial width is zero until the mouse is dragged
                Height = yMax - extendedYMin // Full height extending below the X-axis
            };

            // Add the rectangle annotation to the chart
            chart.Annotations.Add(selectionRectangle);
        }





        /// <summary>
        /// Handles the MouseMove event to update the selection rectangle's width while the mouse is being dragged.
        /// </summary>
        /// <param name="sender">The chart control where the event occurred.</param>
        /// <param name="e">Mouse event data containing the current mouse position.</param>
        private void chart_stockData_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectionRectangle != null)
            {
                var chart = (System.Windows.Forms.DataVisualization.Charting.Chart)sender;
                var chartArea = chart.ChartAreas["ChartArea_OHLC"];

                // Translate pixel coordinates to axis coordinates for the candlestick chart area
                double xAxisValue = chartArea.AxisX.PixelPositionToValue(e.X);

                // Update the rectangle's width based on the drag direction
                int currentIndex = (int)Math.Round(xAxisValue);
                if (currentIndex >= waveStartIndex)
                {
                    // If dragging to the right, increase the width
                    selectionRectangle.Width = currentIndex - waveStartIndex;
                }
                else
                {
                    // If dragging to the left, adjust the starting position and width
                    selectionRectangle.X = currentIndex;
                    selectionRectangle.Width = waveStartIndex - currentIndex;
                }
            }
        }

        /// <summary>
        /// Handles the MouseUp event to finalize the wave selection when the mouse button is released.
        /// </summary>
        /// <param name="sender">The chart control where the event occurred.</param>
        /// <param name="e">Mouse event data containing the mouse position.</param>
        private void chart_stockData_MouseUp(object sender, MouseEventArgs e)
        {
            if (selectionRectangle != null)
            {
                var chart = (System.Windows.Forms.DataVisualization.Charting.Chart)sender;
                var chartArea = chart.ChartAreas["ChartArea_OHLC"];

                // Translate pixel coordinates to axis coordinates for the candlestick chart area
                double xAxisValue = chartArea.AxisX.PixelPositionToValue(e.X);
                waveEndIndex = (int)Math.Round(xAxisValue);

                // Debugging message to display the selected wave's start and end indices
                MessageBox.Show($"Wave selected: Start = {waveStartIndex}, End = {waveEndIndex}", "Debug Info");

                // Validate the wave selection to ensure it is valid
                if (waveStartIndex >= 0 && waveEndIndex >= 0 && waveStartIndex != waveEndIndex)
                {
                    ValidateWaveSelection(waveStartIndex, waveEndIndex);
                }

                // Reset the selection rectangle
                selectionRectangle = null;
            }
        }

        /// <summary>
        /// Validates the wave selection to ensure the starting candlestick is a peak or a valley.
        /// </summary>
        /// <param name="startIndex">The starting index of the wave.</param>
        /// <param name="endIndex">The ending index of the wave.</param>
        private void ValidateWaveSelection(int startIndex, int endIndex)
        {
            // Extract the wave data based on the selected indices
            var wave = currentStockData.Skip(startIndex).Take(endIndex - startIndex + 1).ToList();

            // Check if the starting candlestick is a peak or a valley
            bool isValidStart = IsPeakOrValley(currentStockData[startIndex]);
            if (!isValidStart)
            {
                // If not valid, display a warning message and reset the indices
                MessageBox.Show("The starting candlestick must be a Peak or Valley.", "Invalid Wave", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                waveStartIndex = -1;
                waveEndIndex = -1; // Clear invalid indices
                return;
            }

            // Retain validated indices
            waveStartIndex = startIndex;
            waveEndIndex = endIndex;

            // Debugging message to confirm wave validation
            MessageBox.Show($"Wave validation passed. Start = {waveStartIndex}, End = {waveEndIndex}", "Debug Info");
        }

        /// <summary>
        /// Determines whether the specified candlestick is a peak or a valley.
        /// </summary>
        /// <param name="candlestick">The candlestick to check.</param>
        /// <returns>True if the candlestick is a peak or a valley; otherwise, false.</returns>
        private bool IsPeakOrValley(SmartCandlestick candlestick)
        {
            // Get the index of the candlestick in the current stock data
            int index = currentStockData.IndexOf(candlestick);

            // Check if the candlestick is not the first or last in the list
            if (index > 0 && index < currentStockData.Count - 1)
            {
                // Check if the candlestick is a peak or a valley
                return (candlestick.High > currentStockData[index - 1].High && candlestick.High > currentStockData[index + 1].High) ||
                       (candlestick.Low < currentStockData[index - 1].Low && candlestick.Low < currentStockData[index + 1].Low);
            }
            return false;
        }

        /// <summary>
        /// Handles the ValueChanged event for the start date picker to ensure a valid date range.
        /// </summary>
        /// <param name="sender">The start date picker control.</param>
        /// <param name="e">Event arguments.</param>
        private void datePicker_start_ValueChanged(object sender, EventArgs e)
        {
            ValidateDateRange();
        }

        /// <summary>
        /// Handles the ValueChanged event for the end date picker to ensure a valid date range.
        /// </summary>
        /// <param name="sender">The end date picker control.</param>
        /// <param name="e">Event arguments.</param>
        private void datePicker_end_ValueChanged(object sender, EventArgs e)
        {
            ValidateDateRange();
        }

        /// <summary>
        /// Ensures the selected start date is not later than the end date.
        /// If the start date is invalid, it adjusts it to one day before the end date.
        /// </summary>
        private void ValidateDateRange()
        {
            if (datePicker_start.Value.Date > datePicker_end.Value.Date)
            {
                // Show a warning message if the start date is later than the end date
                MessageBox.Show("Start date cannot be later than the end date.", "Invalid Date Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Adjust the start date to be one day before the end date
                datePicker_start.Value = datePicker_end.Value.Date.AddDays(-1);
            }
        }

        /// <summary>
        /// Updates the charts based on the selected date range.
        /// </summary>
        /// <param name="sender">The update button control.</param>
        /// <param name="e">Event arguments.</param>
        private void button_update_Click_1(object sender, EventArgs e)
        {
            // Check if there is data to update
            if (currentStockData == null || currentStockData.Count == 0)
            {
                MessageBox.Show("No data to update. Load a stock file first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the selected date range
            DateTime startDate = datePicker_start.Value.Date;
            DateTime endDate = datePicker_end.Value.Date;

            // Filter the main stock data based on the date range
            var filteredMainData = currentStockData.Where(data => data.Date >= startDate && data.Date <= endDate).ToList();

            // Check if there is any data available for the selected date range
            if (filteredMainData.Count == 0)
            {
                MessageBox.Show("No data available for the selected date range.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Populate the main chart with the filtered data
            PopulateChart(chart_stockData, filteredMainData);

            // Update all additional stock charts in the dictionary
            foreach (var entry in stockCharts)
            {
                var chart = entry.Value.Item2;
                var filteredData = currentStockData.Where(data => data.Date >= startDate && data.Date <= endDate).ToList();
                PopulateChart(chart, filteredData);
            }
        }

        /// <summary>
        /// Handles the click event for the Apply Leeway button to apply the specified leeway percentage.
        /// </summary>
        /// <param name="sender">The Apply Leeway button control.</param>
        /// <param name="e">Event arguments.</param>
        private void button_applyLeeway_Click(object sender, EventArgs e)
        {
            // Read the leeway value from the textbox
            string leewayText = textBox_leeway.Text;

            // Validate and apply the leeway
            ApplyLeeway(leewayText);
        }




        /// <summary>
        /// Validates and applies the specified leeway percentage to the beauty calculation.
        /// </summary>
        /// <param name="leewayText">The leeway percentage as a string input.</param>
        private void ApplyLeeway(string leewayText)
        {
            // Validate that the input string can be parsed as a positive decimal value
            if (decimal.TryParse(leewayText, out decimal leewayPercentage) && leewayPercentage > 0)
            {
                // Convert leeway percentage (e.g., "5.0") to a decimal (e.g., 0.05)
                tolerancePercentage = leewayPercentage / 100m;

                // Notify the user that the leeway percentage has been updated
                MessageBox.Show($"Leeway updated to {leewayPercentage}%", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // If a valid wave has been selected, recompute and display the beauty plot
                if (waveStartIndex >= 0 && waveEndIndex >= 0)
                {
                    ComputeAndPlotBeauty(waveStartIndex, waveEndIndex);
                }
                else
                {
                    // If no wave is selected, prompt the user to select a wave first
                    MessageBox.Show("Please select a valid wave to apply the new leeway.", "No Wave Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                // If input is invalid, notify the user with an error message
                MessageBox.Show("Invalid leeway percentage. Please enter a positive number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }




        /// <summary>
        /// Handles the click event to compute and display Fibonacci levels for the selected wave.
        /// </summary>
        /// <param name="sender">The button that triggered the event.</param>
        /// <param name="e">Event arguments.</param>
        private void button_computeFib_Click(object sender, EventArgs e)
        {
            // Validate that a wave has been selected before computing Fibonacci levels
            if (waveStartIndex == -1 || waveEndIndex == -1)
            {
                MessageBox.Show("Please select a valid wave first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Compute and display Fibonacci levels for the selected wave range
            ComputeAndDisplayFibonacciLevels(waveStartIndex, waveEndIndex);
        }


        /// <summary>
        /// Computes and displays Fibonacci levels for the selected wave on the chart.
        /// </summary>
        /// <param name="startIndex">The starting index of the wave.</param>
        /// <param name="endIndex">The ending index of the wave.</param>
        private void ComputeAndDisplayFibonacciLevels(int startIndex, int endIndex)
        {
            // Validate the indices to ensure they are within the bounds of the stock data
            if (currentStockData == null || startIndex < 0 || endIndex < 0 || startIndex >= currentStockData.Count || endIndex >= currentStockData.Count)
            {
                MessageBox.Show("Invalid start or end index for Fibonacci calculation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Determine if the wave is upward or downward
            decimal startPrice = currentStockData[startIndex].Close;
            decimal endPrice = currentStockData[endIndex].Close;
            decimal waveHeight = endPrice - startPrice;
            bool isUpwardWave = waveHeight > 0; // Check if wave is upward

            // Define Fibonacci percentages to use for the calculations
            decimal[] fibonacciPercentages = { 0m, 0.236m, 0.382m, 0.5m, 0.618m, 0.764m, 1m };

            // Clear any existing Fibonacci lines and annotations on the chart
            var existingFibSeries = chart_stockData.Series.Where(s => s.Name.StartsWith("FibLine_")).ToList();
            foreach (var series in existingFibSeries)
            {
                chart_stockData.Series.Remove(series);
            }

            var existingFibAnnotations = chart_stockData.Annotations.OfType<TextAnnotation>().Where(a => a.Text.Contains("%")).ToList();
            foreach (var annotation in existingFibAnnotations)
            {
                chart_stockData.Annotations.Remove(annotation);
            }

            // Calculate the pixel positions of the rectangle's bounds for the wave selection
            var chartArea = chart_stockData.ChartAreas["ChartArea_OHLC"];
            double startXPixel = chartArea.AxisX.ValueToPixelPosition(startIndex); // Pixel position of start index
            double endXPixel = chartArea.AxisX.ValueToPixelPosition(endIndex);     // Pixel position of end index

            // Convert pixel positions to logical axis values
            double startX = chartArea.AxisX.PixelPositionToValue(startXPixel);
            double endX = chartArea.AxisX.PixelPositionToValue(endXPixel);

            // Iterate through the Fibonacci percentages and draw horizontal lines
            foreach (decimal percentage in fibonacciPercentages)
            {
                decimal levelPrice = isUpwardWave
                    ? startPrice + (percentage * waveHeight)  // Calculate upward Fibonacci level
                    : startPrice - (percentage * Math.Abs(waveHeight)); // Calculate downward Fibonacci level

                // Create a new series for the Fibonacci line
                var fibSeries = new System.Windows.Forms.DataVisualization.Charting.Series($"FibLine_{percentage * 100:0.0}")
                {
                    ChartType = SeriesChartType.Line,
                    Color = Color.Purple,
                    BorderWidth = 1,
                    IsVisibleInLegend = false,
                    ChartArea = "ChartArea_OHLC"
                };

                // Add points to draw the line spanning the selected wave range
                fibSeries.Points.AddXY(startX, (double)levelPrice); // Starting point
                fibSeries.Points.AddXY(endX, (double)levelPrice);   // Ending point

                chart_stockData.Series.Add(fibSeries);

                // Add a text annotation to label the Fibonacci level percentage
                var fibLabel = new TextAnnotation
                {
                    AxisX = chartArea.AxisX,
                    AxisY = chartArea.AxisY,
                    ClipToChartArea = "ChartArea_OHLC",
                    Text = $"{percentage * 100:0.0}%",
                    ForeColor = Color.Black,
                    Font = new Font("Arial", 8, FontStyle.Bold),
                    X = (startX + endX) / 2.0, // Position the label in the middle of the wave
                    Y = (double)levelPrice
                };
                chart_stockData.Annotations.Add(fibLabel);
            }

            // Notify the user that Fibonacci levels have been added
            MessageBox.Show("Fibonacci levels added within the selected rectangle!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
















        /// <summary>
        /// Handles the click event for the "Display Beauty Plot" button.
        /// Validates the wave selection and calls the method to compute and plot the beauty values.
        /// </summary>
        /// <param name="sender">The button that triggered the event.</param>
        /// <param name="e">Event arguments.</param>
        private void button_displayBeauty_Click(object sender, EventArgs e)
        {
            // Check if a valid wave has been selected
            if (waveStartIndex == -1 || waveEndIndex == -1)
            {
                // Show a warning if no valid wave is selected
                MessageBox.Show("Please select a valid wave first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // If the wave selection is valid, compute and display the beauty plot
            ComputeAndPlotBeauty(waveStartIndex, waveEndIndex);
        }


        /// <summary>
        /// Computes and plots the beauty values based on the selected wave and Fibonacci levels.
        /// </summary>
        /// <param name="startIndex">The starting index of the selected wave.</param>
        /// <param name="endIndex">The ending index of the selected wave.</param>
        private void ComputeAndPlotBeauty(int startIndex, int endIndex)
        {
            // Validate that the stock data is available and the indices are within bounds
            if (currentStockData == null || startIndex < 0 || endIndex < 0 || startIndex >= currentStockData.Count || endIndex >= currentStockData.Count)
            {
                MessageBox.Show("Invalid wave indices. Please select a valid range.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Ensure the end index is greater than or equal to the start index
            if (endIndex < startIndex)
            {
                MessageBox.Show("End index must be greater than or equal to the start index.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Extract the candlestick data for the selected range
            var selectedData = currentStockData.Skip(startIndex).Take(endIndex - startIndex + 1).ToList();

            // Check if the selected data contains any points
            if (selectedData == null || selectedData.Count == 0)
            {
                MessageBox.Show("No data points found in the selected range.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Calculate the starting and ending prices of the wave
            decimal startPrice = currentStockData[startIndex].Close;
            decimal endPrice = currentStockData[endIndex].Close;
            decimal waveHeight = endPrice - startPrice; // Calculate the wave height (price difference)

            // Ensure the wave height is not zero to avoid invalid calculations
            if (waveHeight == 0)
            {
                MessageBox.Show("Wave height is zero. Select a wave with a significant price difference.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Define Fibonacci levels based on the wave height
            decimal[] fibonacciLevels = new decimal[] { 0m, 0.236m, 0.382m, 0.5m, 0.618m, 0.764m, 1m }
                .Select(p => startPrice + (p * waveHeight)) // Calculate each Fibonacci level
                .ToArray();

            // Debugging: Display the calculated Fibonacci levels in the console
            Console.WriteLine("Fibonacci Levels:");
            foreach (var level in fibonacciLevels)
            {
                Console.WriteLine(level);
            }

            // Find or create the "Beauty" series on the chart
            var beautySeries = chart_stockData.Series.FindByName("Beauty");
            if (beautySeries == null)
            {
                // Create the "Beauty" series if it does not exist
                beautySeries = new System.Windows.Forms.DataVisualization.Charting.Series("Beauty")
                {
                    ChartType = SeriesChartType.Line,      // Line chart for beauty values
                    ChartArea = "ChartArea_Volume",        // Plot in the volume chart area
                    Color = Color.Blue,                    // Set the color to blue
                    BorderWidth = 2                        // Set the line width to 2
                };
                chart_stockData.Series.Add(beautySeries);
            }

            // Clear any existing points in the beauty series
            beautySeries.Points.Clear();

            // List to store beauty values for normalization
            List<double> beautyValues = new List<double>();

            // Iterate through each candlestick in the selected range
            foreach (var candlestick in selectedData)
            {
                int beauty = 0; // Initialize beauty count for the current candlestick

                // Compare candlestick's open, high, low, and close prices with Fibonacci levels
                foreach (decimal level in fibonacciLevels)
                {
                    // Increment beauty count if any price is within the tolerance percentage of the Fibonacci level
                    if (Math.Abs(candlestick.Open - level) <= waveHeight * tolerancePercentage) beauty++;
                    if (Math.Abs(candlestick.High - level) <= waveHeight * tolerancePercentage) beauty++;
                    if (Math.Abs(candlestick.Low - level) <= waveHeight * tolerancePercentage) beauty++;
                    if (Math.Abs(candlestick.Close - level) <= waveHeight * tolerancePercentage) beauty++;
                }

                // Debugging: Output the beauty value for each candlestick in the console
                Console.WriteLine($"Date: {candlestick.Date}, Beauty: {beauty}");

                // Add the beauty value to the series for plotting
                beautySeries.Points.AddXY(candlestick.Date, beauty);
                beautyValues.Add(beauty); // Store the beauty value for normalization
            }

            // Normalize the beauty chart to set appropriate Y-axis limits
            NormalizeBeautyChart(chart_stockData, beautyValues);

            // Notify the user if no beauty points were calculated
            if (beautySeries.Points.Count == 0)
            {
                MessageBox.Show("No Beauty points were calculated. Check your data and tolerance settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }





    }
}
