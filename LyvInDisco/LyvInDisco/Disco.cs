using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml.Linq;
using Timer = System.Windows.Forms.Timer;

namespace LyvInDisco
{
    public class Disco
    {
        private Timer disco;
        private Timer test;
        
        private string sURL = "http://192.168.10.135/api/newdeveloper/lights";
        private string groupURL = "http://192.168.10.135/api/newdeveloper/groups/0/action";

        private int i, transition;
        private Random r;

        private Dictionary<int, bool> lights;
        
        private bool strobo;

        private static Queue<XElement> logQueue;
        private static DateTime LastFlushed = DateTime.Now;

        public Disco()
        {
            r = new Random();
            logQueue = new Queue<XElement>();
            i = 1;
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(groupURL);
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.ProtocolVersion = HttpVersion.Version10;
            
            //request.ServicePoint.Expect100Continue = false;
            request.ServicePoint.UseNagleAlgorithm = false;
            //request.Proxy = null;


            string json = "{\"on\":true,\"hue\":15331,\"sat\":121,\"transitiontime\":50,\"bri\":255}";


            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(json);
            }
            
            ThreadPool.QueueUserWorkItem(o => { DoRequest(request); });
        }

        private void DoRequest(HttpWebRequest request)
        {
            Stopwatch stopwatch = new Stopwatch();
            
            stopwatch.Start();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(responseStream))
                        Log(sr.ReadToEnd());
                }
            }
            stopwatch.Stop();

            TimeSpan timeTaken = stopwatch.Elapsed;
            Log(timeTaken.ToString());
        }


        private void TimerDiscoEvent(Object Sender, EventArgs e)
        {
            if (i == 4) i = 1;
            while (!lights[i])
            {
                i++;
                if (i == 4) i = 1;
            }
            
            int hue, sat, bri;
            hue = r.Next(0, 65280);
            sat = r.Next(200, 255);
            bri = r.Next(200, 255);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sURL + "/" + i.ToString() + "/state");
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.ProtocolVersion = HttpVersion.Version10;
            
            //request.ServicePoint.Expect100Continue = false;
            request.ServicePoint.UseNagleAlgorithm = false;
            //request.Proxy = null;
            

            string json = "{\"on\":true,\"hue\":" + hue.ToString() + ",\"sat\":" + sat.ToString() + ",\"transitiontime\":"+transition+",\"bri\":"+bri.ToString()+"}";

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(json);
            }
            
            ThreadPool.QueueUserWorkItem(o => { DoRequest(request); });
            i++;
        }

        private void TimerTestEvent(Object Sender, EventArgs e)
        {
            if ((lights[1]) && (lights[2]) && (lights[3]))
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(groupURL);
                request.Method = "PUT";
                request.ContentType = "application/json";
                request.ProtocolVersion = HttpVersion.Version10;

                //request.ServicePoint.Expect100Continue = false;
                request.ServicePoint.UseNagleAlgorithm = false;
                //request.Proxy = null;
                string json = "";
                if (strobo)
                {
                    json = "{\"on\":true,\"bri\":255,\"ct\":153,\"transitiontime\":" + transition + "}";
                }
                else
                {
                    json = "{\"on\":false,\"transitiontime\":" + transition+"}";
                }

                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(json);
                }

                ThreadPool.QueueUserWorkItem(o => { DoRequest(request); });
            }
            else
            {
                foreach (int l in lights.Keys)
                {
                    if (lights[l])
                    {
                        HttpWebRequest request =
                            (HttpWebRequest)WebRequest.Create(sURL + "/" + l.ToString() + "/state");

                        request.Method = "PUT";
                        request.ContentType = "application/json";
                        request.ProtocolVersion = HttpVersion.Version10;

                        //request.ServicePoint.Expect100Continue = false;
                        request.ServicePoint.UseNagleAlgorithm = false;
                        //request.Proxy = null;
                        
                        string json = "";
                        if (strobo)
                        {
                            json = "{\"on\":true,\"bri\":255,\"ct\":153,\"transitiontime\":" + transition +"}";
                        }
                        else
                        {
                            json = "{\"on\":false,\"transitiontime\":" + transition + "}";
                        }

                        using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                        {
                            writer.Write(json);
                        }
                        DoRequest(request);
                        //ThreadPool.QueueUserWorkItem(o => { DoRequest(request); });
                    }
                }
            }
            strobo = !strobo;
        }

        public void StartDisco(int timer, int transition, Dictionary<int,bool> lights )
        {
            disco = new Timer();
            disco.Interval = timer;
            disco.Tick += new EventHandler(TimerDiscoEvent);
            disco.Enabled = true;
            disco.Start();
            this.transition = transition;
            this.lights = lights;
            i = 1;
        }

        public void StartTest(int timer, int transition, Dictionary<int, bool> lights)
        {
            if ((lights[1]) && (lights[2]) && (lights[3]))
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(groupURL);
                request.Method = "PUT";
                request.ContentType = "application/json";
                request.ProtocolVersion = HttpVersion.Version10;

                //request.ServicePoint.Expect100Continue = false;
                request.ServicePoint.UseNagleAlgorithm = false;
                //request.Proxy = null;


                string json = "{\"on\":false}";

                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(json);
                }

                ThreadPool.QueueUserWorkItem(o => { DoRequest(request); });
            }
            else
            {
                foreach (int l in lights.Keys)
                {
                    if (lights[l] == true)
                    {
                        HttpWebRequest request =
                            (HttpWebRequest)WebRequest.Create(sURL + "/" + l.ToString() + "/state");

                        request.Method = "PUT";
                        request.ContentType = "application/json";
                        request.ProtocolVersion = HttpVersion.Version10;

                        //request.ServicePoint.Expect100Continue = false;
                        request.ServicePoint.UseNagleAlgorithm = false;
                        //request.Proxy = null;


                        string json = "{\"on\":false}";

                        using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                        {
                            writer.Write(json);
                        }

                        ThreadPool.QueueUserWorkItem(o => { DoRequest(request); });
                    }
                }
            }
            test = new Timer();
            test.Interval = timer;
            test.Tick += new EventHandler(TimerTestEvent);
            test.Enabled = true;
            test.Start();
            this.transition = transition;
            this.lights = lights;
            strobo = true;
        }

        public void StopDisco()
        {
            if (disco != null)
            {
                disco.Stop();
                disco.Dispose();
            }

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(groupURL);
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.ProtocolVersion = HttpVersion.Version10;
            
            //request.ServicePoint.Expect100Continue = false;
            request.ServicePoint.UseNagleAlgorithm = false;
            //request.Proxy = null;
            
            string json = "{\"on\":true,\"hue\":15331,\"sat\":121,\"transitiontime\":50,\"bri\":255}";

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(json);
            }
            ThreadPool.QueueUserWorkItem(o => { DoRequest(request); });
        }

        public void StopTest()
        {
            if (test != null)
            {
                test.Stop();
                test.Dispose();
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(groupURL);
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.ProtocolVersion = HttpVersion.Version10;
            
            //request.ServicePoint.Expect100Continue = false;
            request.ServicePoint.UseNagleAlgorithm = false;
            //request.Proxy = null;
            

            string json = "{\"on\":true,\"hue\":15331,\"sat\":121,\"transitiontime\":50,\"bri\":255}";

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(json);
            }
            ThreadPool.QueueUserWorkItem(o => { DoRequest(request); });
        }

        public void Log(string item)
        {
#if DEBUG
    // debugging enabled, Log to Console
                Console.WriteLine(item);
#else
           XElement xmlEntry = new XElement("logEntry",
                                             new XElement("Date", System.DateTime.Now.ToString()),
                                             new XElement("Message", item));
            

            // Lock the queue while writing to prevent contention for the log file
            lock (logQueue)
            {
                logQueue.Enqueue(xmlEntry);

                // If we have reached the Queue Size then flush the Queue
                if (logQueue.Count >= 10 || DoPeriodicFlush())
                {
                    FlushLog();
                }
            }
#endif
        }

        private bool DoPeriodicFlush()
        {
            TimeSpan logAge = DateTime.Now - LastFlushed;
            if (logAge.TotalSeconds >= 10)
            {
                LastFlushed = DateTime.Now;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Flushes the Queue to the physical log file
        /// </summary>
        private void FlushLog()
        {
            while (logQueue.Count > 0)
            {
                XElement entry = logQueue.Dequeue();
                
                using (StreamWriter log = new StreamWriter("log.txt", true))
                {
                    log.WriteLine(entry);
                    log.Close();
                }

            }
        }
    }
}
