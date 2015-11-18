using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using System.Threading;

namespace SP500IndexStatistics
{
    class StandardsAndPoorsUtility
    {
        private const int numDataColumns = 7;
        private const int indexOfAdjustedClose = 6;

        public static string[] HistoricalStandardsAndPoorsByDateRange(int beginMonth, int beginDay, int beginYear, int endMonth, int endDay, int endYear)
        {
            string[] dataLines = null;
            // formatted query for barclay stock data that tracks the performance of the S & P 500
            string sp500Query = "http://ichart.finance.yahoo.com/table.csv?s=IVV&" + 
                                string.Format("d={0}&e={1}&f={2}&g=d&a={3}&b={4}&c={5}&ignore.csv", 
                                endMonth, endDay, endYear, beginMonth, beginDay, beginYear);

            // issue the request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sp500Query);
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // initializing as a string
                    var responseValue = string.Empty;
                    // extract the response
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (var reader = new StreamReader(responseStream))
                            {
                                // the block of data is extracted as a continuous string
                                responseValue = reader.ReadToEnd();
                            }
                        }
                    }

                    // splitting the data on the carriage returns
                    dataLines = responseValue.Split('\n');
                }
            }
            catch (Exception){ }          
            return dataLines;
        }

        public static void WriteDataToCSV(string outputFileName, string[] stockData)
        {
            if(stockData == null)
            {
                return;
            }
            int numRows = stockData.Length;
            var csvData = new StringBuilder();
            var headerRow = string.Format("{0},{1}{2}", stockData[0], "Percentage Change", Environment.NewLine);
            csvData.Append(headerRow);
            for (int iRow = 1; iRow < numRows; iRow++ )
            {
                string dataItem = stockData[iRow];                
                // checking that we are not at the tail end of the data set 
                // also, check that we are not trying to parse the header row
                if (iRow < (numRows - 1))
                {
                    string dataItemPrevious = stockData[iRow + 1];
                    if(string.IsNullOrEmpty(dataItemPrevious))
                    {
                        var exceptionCaseDataLine = string.Format("{0}{1}", dataItem, Environment.NewLine);
                        csvData.Append(exceptionCaseDataLine);
                        continue;
                    }

                    // catching index out of range and parsing double exceptions
                    try
                    {
                        string adjustedClose = dataItem.Split(',')[indexOfAdjustedClose];
                        string adjustedClosePrevious = dataItemPrevious.Split(',')[indexOfAdjustedClose];
                        double adjustedCloseNumeric = Double.Parse(adjustedClose);
                        double adjustedClosePreviousNumeric = Double.Parse(adjustedClosePrevious);
                        // calculate the percentage change in the time-series
                        double percentChange = 100.00 * ((adjustedCloseNumeric - adjustedClosePreviousNumeric) / adjustedClosePreviousNumeric);
                        var dataLine = string.Format("{0},{1}{2}", dataItem, percentChange, Environment.NewLine);
                        csvData.Append(dataLine);
                    }
                    catch(Exception){ }
                }
                else
                {
                    var dataLine = string.Format("{0}{1}", dataItem, Environment.NewLine);
                    csvData.Append(dataLine);
                }
            }
            File.WriteAllText(outputFileName, csvData.ToString());
        }


        static void Main(string[] args)
        {
            const int requiredNumOfArgs = 8;            
            string[] commandLineArgs = Environment.GetCommandLineArgs();

            if(commandLineArgs.Length != requiredNumOfArgs)
            {
                Console.WriteLine("Usage: TODO, CLG");
            }

            string outputFilePath = commandLineArgs[1];
            // zero-based start day
            int startDay = int.Parse(commandLineArgs[2]);
            int startMonth = int.Parse(commandLineArgs[3]);
            int startYear = int.Parse(commandLineArgs[4]);
            // zero-based end day
            int endDay = int.Parse(commandLineArgs[5]);
            int endMonth = int.Parse(commandLineArgs[6]);
            int endYear = int.Parse(commandLineArgs[7]);

            string[] historicalStockData = HistoricalStandardsAndPoorsByDateRange(startDay, startMonth, startYear, endDay, endMonth, endYear);
            WriteDataToCSV(outputFilePath, historicalStockData);
        }
    }
}
