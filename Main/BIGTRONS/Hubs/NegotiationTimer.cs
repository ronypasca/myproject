using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Timers;
using System.Web.Hosting;
using System.Threading.Tasks;

namespace com.SML.BIGTRONS.Hubs
{
    public sealed class NegotiationTimer : IRegisteredObject
    {

        private Timer _taskTimer;
        private TimeSpan _taskTimeSpan;
        private IHubContext _hub;
        private string _FPTID;

        #region Property

        public string FPTID { get { return _FPTID; } set { _FPTID = value; } }
        public string RoundID { get; set; }
        public TimeSpan TimeSpan { get { return _taskTimeSpan; } set { _taskTimeSpan = value; } }
        public Timer Timer { get { return _taskTimer; } set { _taskTimer = value; } }
        #endregion

        public NegotiationTimer()
        {
            HostingEnvironment.RegisterObject(this);

            _hub = GlobalHost.ConnectionManager.GetHubContext<NegotiationHub>();

            
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _taskTimeSpan = _taskTimeSpan.Subtract(TimeSpan.FromSeconds(1));

            if (_taskTimeSpan.TotalSeconds == 0)
            {
                _taskTimer.Enabled = false;
                _taskTimer.Stop();
                this.Stop(true);

                Task.Run(async delegate
                {
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    _hub.Clients.All.updateEndRound(FPTID, RoundID);
                });
            }
            _hub.Clients.All.countDown(FPTID, RoundID, string.Format(@"{0:hh\:mm\:ss} ", _taskTimeSpan));

            
            System.Diagnostics.Debug.WriteLine($"HUBTIMER FPTT: {FPTID} Seconds : " + _taskTimeSpan.Seconds);
            System.Diagnostics.Debug.WriteLine($"HUBTIMER FPTT: {FPTID}: hh:mm:ss : " + _taskTimeSpan.ToString("c"));
            
        }

        public void Start() {
            _taskTimer = new Timer();
            _taskTimer.Interval = 1000;
            _taskTimer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);
            _taskTimer.Enabled = true;
            _taskTimer.Start();
            _taskTimer.AutoReset = true;
        }

        public void Stop(bool imediate)
        {
            _taskTimer.Stop();
            _taskTimer.Dispose();

            HostingEnvironment.UnregisterObject(this);
        }
    }

}