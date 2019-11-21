using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Timers;
using System.Threading.Tasks;

namespace com.SML.BIGTRONS.Hubs
{
    public sealed class ConnectionCache
    {

        private static readonly ConnectionCache instance
            = new ConnectionCache();

        private Timer _timer;
        private TimeSpan _timeSpan;


        static ConnectionCache()
        {

        }

        private ConnectionCache()
        {
            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.AutoReset = true;
        }

        public static ConnectionCache Instance
        {
            get
            {
                return instance;
            }
        }

        public TimeSpan TimeSpan { get { return _timeSpan; } set { _timeSpan = value; } }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            _timeSpan = _timeSpan.Subtract(TimeSpan.FromSeconds(1));
            CountDownRoundTime();
        }

        internal void CountDownRoundTime() {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NegotiationHub>();
            
            if (_timer.Enabled == false)
            {
                _timer.Enabled = true;
                _timer.Start();
            }

            if (_timeSpan.TotalSeconds == 0)
            {
                _timer.Enabled = false;
                _timer.Stop();

                NegotiationHub.BroadcastEndRound();
            }
            
            //System.Diagnostics.Debug.WriteLine("HUBTIMER: Seconds : " + TimeSpan.Seconds);
            //System.Diagnostics.Debug.WriteLine("HUBTIMER: hh:mm:ss : " + TimeSpan.ToString("c"));
            NegotiationHub.BroadcastTimeSpan(string.Format(@"{0:hh\:mm\:ss} ", TimeSpan));
            
        }
        
    }

}