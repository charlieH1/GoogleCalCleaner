using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Calendar.v3.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Google_Calendar_Cleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please Provide calendar Id:");
            string calendarId = Console.ReadLine();
            Console.WriteLine("Please provide bearer Token");
            string bearerToken = Console.ReadLine();
            WebRequest request = WebRequest.Create("https://www.googleapis.com/calendar/v3/calendars/" + calendarId + "/events");
            request.Headers.Add("Authorization", "Bearer " + bearerToken);
            var response = request.GetResponse();
            var rawJson = new StreamReader(response.GetResponseStream()).ReadToEnd();
            response.Close();
            var converted = JsonConvert.DeserializeObject<Events>(rawJson);
            int succesfulDeletes = 0;
            while (converted.Items.Count > 0)
            {
                
                foreach (var item in converted.Items)
                {
                    
                    try
                    {
                        HttpWebRequest deleteRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/calendar/v3/calendars/" + calendarId + "/events/" + item.Id);
                        deleteRequest.Method = "DELETE";
                        deleteRequest.Headers.Add("Authorization", "Bearer " + bearerToken);
                        HttpWebResponse deleteResponse = (HttpWebResponse)deleteRequest.GetResponse();
                        if (deleteResponse.StatusCode == HttpStatusCode.NoContent)
                        {
                            succesfulDeletes++;
                        }
                        deleteResponse.Close();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Eror occured: " + ex.Message + " " + ex.InnerException + " Succesfull Deletes: " + succesfulDeletes);
                        Console.WriteLine("Sleeping for 5 minutes");
                        Thread.Sleep(5 * 60 * 1000);
                        Console.WriteLine("Awake again");

                    }

                }
                
                Console.WriteLine("Sleeping for 5 minutes after " + succesfulDeletes + " Succesful Deletes" );
                Thread.Sleep(5 * 60 * 1000);
                Console.WriteLine("Awake again");
                request = WebRequest.Create("https://www.googleapis.com/calendar/v3/calendars/" + calendarId + "/events");
                request.Headers.Add("Authorization", "Bearer " + bearerToken);
                response = request.GetResponse();
                rawJson = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                converted = JsonConvert.DeserializeObject<Events>(rawJson);

            }
            Console.WriteLine("Successfully Deleted " + succesfulDeletes + " Events");
            Console.ReadLine();
        }
    }
}
