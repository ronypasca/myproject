using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using com.SML.BIGTRONS.Hubs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace com.SML.BIGTRONS.Hubs
{
    public class NegotiationHub : Hub
    {
        private NegotiationTimer negoTimer;
        public void BrodcastActivatedRound(string FPTID, string RoundID, double RoundTime, decimal TopValue, decimal BottomValue)
        {
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NegotiationHub>();

            Task.Run(async delegate
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                hubContext.Clients.All.updateGrid(FPTID, RoundID, TopValue,BottomValue);
                negoTimer = new NegotiationTimer();
                negoTimer.TimeSpan = TimeSpan.FromSeconds(RoundTime);
                negoTimer.FPTID = FPTID;
                negoTimer.RoundID = RoundID;
                negoTimer.Start();

                if (!Global.TaskTimer.ContainsKey(FPTID))
                    Global.TaskTimer.Add(FPTID, negoTimer);
                else
                    Global.TaskTimer[FPTID] = negoTimer;

            });


        }

        public void BrodcastDeactivatedRound(string FPTID, string RoundID, double RoundTime)
        {
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NegotiationHub>();


            if (Global.TaskTimer.ContainsKey(FPTID))
            {
                negoTimer = (NegotiationTimer)Global.TaskTimer[FPTID];
                negoTimer.TimeSpan = TimeSpan.FromSeconds(RoundTime);

                Global.TaskTimer[FPTID] = null;
            }

        }

        public void BroadcastBidMonitoring(string BudgetPlanID, string RoundID, string VendorID)
        {
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NegotiationHub>();
            hubContext.Clients.All.updateBidMonitoring(BudgetPlanID, RoundID, VendorID);
        }

    }
}