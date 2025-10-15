using System;
using System.ServiceProcess;
using System.Timers;
using RestSharp;

namespace SendToRisService
{
    public partial class Service1 : ServiceBase
    {
        private Timer _timer;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _timer = new Timer();
            _timer.Interval = 60000;
            _timer.Elapsed += TimerElapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            Log("Service started.");
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Log($"RIS Sender service started.");

                var client = new RestClient("https://cerebral.ipartner.my/");
                var request = new RestRequest("cmv2-api/api/ris/public/ReSendMissingOrders", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddJsonBody(new { cm = "cm", patientId = "000" });

                var response = client.Execute(request);
                Log($"RIS Request sent. Status: {response.StatusCode}, Count: {response.Content}");
            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Message);
            }
        }

        protected override void OnStop()
        {
            _timer.Stop();
            _timer.Dispose();
            Log("Service stopped.");
        }

        private void Log(string message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\log.txt";
            System.IO.File.AppendAllText(path, DateTime.Now + " - " + message + Environment.NewLine);
        }
    }
}
