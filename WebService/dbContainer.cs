using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace WebService
{
    public class dbContainer
    {
        static private List<Message> DB;

        static dbContainer() {
            DB = new List<Message>() { };
        }

        public static string SetObject(string json) {
            try
            {
                Message msg = JsonSerializer.Deserialize<Message>(json);
                msg.time = DateTime.Now;

                if (string.IsNullOrEmpty(msg.name)){
                    throw new Exception("Null var exception");
                }
                DB.Add(msg);
                return null;
            }
            catch
            {
                return "invalid json compose";
            }
        }

        public static string GetObjects() {

            if (DB.Count < 1){
                return "nothing data";
            }

            try
            {
                var SortedList = DB.OrderBy(m => m.time.Year)
                                            .ThenBy(m => m.time.Month)
                                            .ThenBy(m => m.time.Day)
                                            .ThenBy(m => m.time.Hour)
                                            .ThenBy(m => m.time.Minute).ToList();

                string currentDT = SortedList.First().time.ToString("yyyy:MM:dd:HH:mm");
                int Value = 0;

                List<Response> resp = new List<Response>() { };

                foreach (var msg in SortedList)
                {
                    if (msg.time.ToString("yyyy:MM:dd:HH:mm") == currentDT)
                    {
                        Value += msg.value;
                    }
                    else
                    {
                        resp.Add(new Response() { time = currentDT, value = Value });
                        currentDT = msg.time.ToString("yyyy:MM:dd:HH:mm");
                        Value = msg.value;
                    }
                }
                resp.Add(new Response() { time = currentDT, value = Value });

                string json = JsonSerializer.Serialize<Response[]>(resp.ToArray());
                return json;
            }
            catch
            {
                return "something wrong";
            }
        }
    }


    public class Message
    {
        public Message() { }
        public string name { get; set; }
        public int value { get; set; }
        public DateTime time { get; set; }
    }

    public class Response
    {
        public Response() { }
        public string time { get; set; }
        public int value { get; set; }
    }
}
