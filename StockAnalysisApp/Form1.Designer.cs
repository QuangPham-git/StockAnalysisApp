namespace StockAnalysisApp
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.LineAnnotation lineAnnotation1 = new System.Windows.Forms.DataVisualization.Charting.LineAnnotation();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.datePicker_start = new System.Windows.Forms.DateTimePicker();
            this.button_loadData = new System.Windows.Forms.Button();
            this.chart_stockData = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.datePicker_end = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox_leeway = new System.Windows.Forms.TextBox();
            this.label_leeway = new System.Windows.Forms.Label();
            this.button_computeFib = new System.Windows.Forms.Button();
            this.button_displayBeauty = new System.Windows.Forms.Button();
            this.button_applyLeeway = new System.Windows.Forms.Button();
            this.button_update = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart_stockData)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // datePicker_start
            // 
            this.datePicker_start.Location = new System.Drawing.Point(201, 42);
            this.datePicker_start.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.datePicker_start.Name = "datePicker_start";
            this.datePicker_start.Size = new System.Drawing.Size(243, 22);
            this.datePicker_start.TabIndex = 1;
            this.datePicker_start.ValueChanged += new System.EventHandler(this.datePicker_start_ValueChanged);
            // 
            // button_loadData
            // 
            this.button_loadData.Location = new System.Drawing.Point(15, 42);
            this.button_loadData.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_loadData.Name = "button_loadData";
            this.button_loadData.Size = new System.Drawing.Size(128, 23);
            this.button_loadData.TabIndex = 3;
            this.button_loadData.Text = "Load Data";
            this.button_loadData.UseVisualStyleBackColor = true;
            this.button_loadData.Click += new System.EventHandler(this.button_loadData_Click);
            // 
            // chart_stockData
            // 
            lineAnnotation1.AxisXName = "ChartArea_OHLC\\rX";
            lineAnnotation1.ClipToChartArea = "ChartArea_OHLC";
            lineAnnotation1.LineColor = System.Drawing.Color.Red;
            lineAnnotation1.LineWidth = 2;
            lineAnnotation1.Name = "HorizontalLineAnnotation ";
            lineAnnotation1.Y = 150D;
            lineAnnotation1.YAxisName = "ChartArea_OHLC\\rY";
            this.chart_stockData.Annotations.Add(lineAnnotation1);
            chartArea1.AlignWithChartArea = "ChartArea_Volume";
            chartArea1.AxisX.LabelStyle.Format = "MM/dd/yyyy";
            chartArea1.Name = "ChartArea_OHLC";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 60F;
            chartArea1.Position.Width = 100F;
            chartArea2.Name = "ChartArea_Volume";
            chartArea2.Position.Auto = false;
            chartArea2.Position.Height = 30F;
            chartArea2.Position.Width = 100F;
            chartArea2.Position.Y = 65F;
            this.chart_stockData.ChartAreas.Add(chartArea1);
            this.chart_stockData.ChartAreas.Add(chartArea2);
            this.chart_stockData.Dock = System.Windows.Forms.DockStyle.Top;
            legend1.Name = "Legend1";
            this.chart_stockData.Legends.Add(legend1);
            this.chart_stockData.Location = new System.Drawing.Point(0, 0);
            this.chart_stockData.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chart_stockData.Name = "chart_stockData";
            series1.ChartArea = "ChartArea_Volume";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Candlestick;
            series1.CustomProperties = "PriceUpColor=Lime, PriceDownColor=Red";
            series1.Legend = "Legend1";
            series1.Name = "ChartArea_OHLC";
            series1.XValueMember = "Date";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series1.YValueMembers = "Low,High,Open,Close";
            series1.YValuesPerPoint = 4;
            series2.ChartArea = "ChartArea_OHLC";
            series2.Legend = "Legend1";
            series2.Name = "ChartArea_Volume";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series2.YValueMembers = "Volume";
            this.chart_stockData.Series.Add(series1);
            this.chart_stockData.Series.Add(series2);
            this.chart_stockData.Size = new System.Drawing.Size(1924, 711);
            this.chart_stockData.TabIndex = 5;
            this.chart_stockData.Text = "chart1";
            // 
            // datePicker_end
            // 
            this.datePicker_end.Location = new System.Drawing.Point(619, 42);
            this.datePicker_end.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.datePicker_end.Name = "datePicker_end";
            this.datePicker_end.Size = new System.Drawing.Size(247, 22);
            this.datePicker_end.TabIndex = 6;
            this.datePicker_end.ValueChanged += new System.EventHandler(this.datePicker_end_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(260, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Start date";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(700, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 16);
            this.label2.TabIndex = 8;
            this.label2.Text = "End date";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox_leeway);
            this.panel1.Controls.Add(this.label_leeway);
            this.panel1.Controls.Add(this.button_computeFib);
            this.panel1.Controls.Add(this.button_displayBeauty);
            this.panel1.Controls.Add(this.button_applyLeeway);
            this.panel1.Controls.Add(this.button_update);
            this.panel1.Controls.Add(this.button_loadData);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.datePicker_start);
            this.panel1.Controls.Add(this.datePicker_end);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 742);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1924, 111);
            this.panel1.TabIndex = 9;
            // 
            // textBox_leeway
            // 
            this.textBox_leeway.Location = new System.Drawing.Point(1136, 48);
            this.textBox_leeway.Name = "textBox_leeway";
            this.textBox_leeway.Size = new System.Drawing.Size(100, 22);
            this.textBox_leeway.TabIndex = 15;
            this.textBox_leeway.Text = "1.0";
            // 
            // label_leeway
            // 
            this.label_leeway.AutoSize = true;
            this.label_leeway.Location = new System.Drawing.Point(1146, 26);
            this.label_leeway.Name = "label_leeway";
            this.label_leeway.Size = new System.Drawing.Size(77, 16);
            this.label_leeway.TabIndex = 14;
            this.label_leeway.Text = "Leeway (%)";
            // 
            // button_computeFib
            // 
            this.button_computeFib.Location = new System.Drawing.Point(1331, 67);
            this.button_computeFib.Name = "button_computeFib";
            this.button_computeFib.Size = new System.Drawing.Size(181, 23);
            this.button_computeFib.TabIndex = 13;
            this.button_computeFib.Text = "Compute Fibonacci Levels";
            this.button_computeFib.UseVisualStyleBackColor = true;
            this.button_computeFib.Click += new System.EventHandler(this.button_computeFib_Click);
            // 
            // button_displayBeauty
            // 
            this.button_displayBeauty.Location = new System.Drawing.Point(1546, 67);
            this.button_displayBeauty.Name = "button_displayBeauty";
            this.button_displayBeauty.Size = new System.Drawing.Size(175, 23);
            this.button_displayBeauty.TabIndex = 12;
            this.button_displayBeauty.Text = "Display Beauty Plot";
            this.button_displayBeauty.UseVisualStyleBackColor = true;
            this.button_displayBeauty.Click += new System.EventHandler(this.button_displayBeauty_Click);
            // 
            // button_applyLeeway
            // 
            this.button_applyLeeway.Location = new System.Drawing.Point(1331, 26);
            this.button_applyLeeway.Name = "button_applyLeeway";
            this.button_applyLeeway.Size = new System.Drawing.Size(181, 23);
            this.button_applyLeeway.TabIndex = 10;
            this.button_applyLeeway.Text = "Apply Leeway";
            this.button_applyLeeway.UseVisualStyleBackColor = true;
            this.button_applyLeeway.Click += new System.EventHandler(this.button_applyLeeway_Click);
            // 
            // button_update
            // 
            this.button_update.Location = new System.Drawing.Point(951, 42);
            this.button_update.Margin = new System.Windows.Forms.Padding(4);
            this.button_update.Name = "button_update";
            this.button_update.Size = new System.Drawing.Size(100, 28);
            this.button_update.TabIndex = 9;
            this.button_update.Text = "Update";
            this.button_update.UseVisualStyleBackColor = true;
            this.button_update.Click += new System.EventHandler(this.button_update_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1924, 853);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chart_stockData);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.chart_stockData)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DateTimePicker datePicker_start;
        private System.Windows.Forms.Button button_loadData;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_stockData;
        private System.Windows.Forms.DateTimePicker datePicker_end;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button_update;
        private System.Windows.Forms.Button button_computeFib;
        private System.Windows.Forms.Button button_displayBeauty;
        private System.Windows.Forms.Button button_applyLeeway;
        private System.Windows.Forms.TextBox textBox_leeway;
        private System.Windows.Forms.Label label_leeway;
    }
}

