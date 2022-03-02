using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Linq;

namespace Beautify
{
    public class AppHelper
    {

        public static String GetMonthName(int month)
        {
            string monthName = "";
            switch (month)
            {
                case 1:
                    monthName = "JAN";
                    break;
                case 2:
                    monthName = "FEB";
                    break;
                case 3:
                    monthName = "MAR";
                    break;
                case 4:
                    monthName = "APR";
                    break;
                case 5:
                    monthName = "MAY";
                    break;
                case 6:
                    monthName = "JUN";
                    break;
                case 7:
                    monthName = "JUL";
                    break;
                case 8:
                    monthName = "AUG";
                    break;
                case 9:
                    monthName = "SEP";
                    break;
                case 10:
                    monthName = "OCT";
                    break;
                case 11:
                    monthName = "NOV";
                    break;
                case 12:
                    monthName = "DEC";
                    break;
            }
            return monthName; // Return the month name
        }

        public static String GetCurrencySymbol()
        {
            //Load the default Currency symbol
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString = @"SELECT Symbol FROM Currency";
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            dt = new DataTable();
            da.Fill(dt);
            string currencySymbol = "";
            if (dt.Rows.Count != 0) // Ensure that the data table contains a row
            {
                currencySymbol = dt.Rows[0]["Symbol"].ToString();
            }
            
            da.Dispose();
            dt.Dispose();
            conn.Close();
            // Return the currency symbol
            return currencySymbol;
        }

        /// <summary>
        /// Sends an SMS to a specified phone number using the 'Express Bulk SMS Mobile API'
        /// </summary>
        /// <param name="message">The message to be sent</param>
        /// <param name="recipientPhoneNumber">The recipient's phone number</param>
        public static void SendSMS(string message, string recipientPhoneNumber)
        {
            // Get the date that this SMS is sent. That is today's date
            string day = DateTime.Now.Day.ToString();
            // If the day is not a 2 digit number, add a zero before the day
            if (day.Length != 2)
            {
                day = "0" + day;
            }
            string month = DateTime.Now.Month.ToString();
            // If the month is not a 2 digit number, add a zero before the month
            if (month.Length != 2)
            {
                month = "0" + month;
            }
            // If the hour is not a 2 digit number, add a zero before the hour
            string hour = DateTime.Now.Hour.ToString();
            if (hour.Length != 2)
            {
                hour = "0" + hour;
            }
            // If the minute is not a 2 digit number, add a zero before the minute
            string minute = DateTime.Now.Minute.ToString();
            if (minute.Length != 2)
            {
                minute = "0" + minute;
            }
            string currentDate = day + "/" + AppHelper.GetMonthName(int.Parse(month)) + "/" + DateTime.Now.Year;
            string currentTime = hour + ":" + minute;


            XDocument sendDataXMLConfigDoc = XDocument.Load(System.Web.HttpContext.Current.Server.MapPath("~/Content/SMS_Config/SendData.xml"));

            // Fetch the settings element
            XElement settingsElement = sendDataXMLConfigDoc.Element("senddata").Element("settings");

            // Configure the settings elements
            settingsElement.Element("default_date").Value = currentDate;
            settingsElement.Element("default_time").Value = currentTime;

            // Fetch the entries elements
            XElement entriesElement = sendDataXMLConfigDoc.Element("senddata").Element("entries");

            // Configure the entries elements
            entriesElement.Element("numto").Value = recipientPhoneNumber;
            entriesElement.Element("customerid").Value = Guid.NewGuid().ToString();
            entriesElement.Element("data1").Value = message;

            // Create an API soap client object
            ExpressBulkSMS_MobileAPI.APISoapClient smsClient = new ExpressBulkSMS_MobileAPI.APISoapClient();
            
            string userName = System.Configuration.ConfigurationManager.AppSettings["expressBulkSmsUsername"];
            string password = System.Configuration.ConfigurationManager.AppSettings["expressBulkSmsPassword"];


            // Send the SMS and get the response
            XDocument smsResponse = XDocument.Parse(smsClient.Send_STR_STR(userName, password, sendDataXMLConfigDoc.ToString()));

            // Return the response
            //return smsResponse.ToString();
        }
    }
}