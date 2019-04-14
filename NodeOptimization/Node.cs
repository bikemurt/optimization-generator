using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;

namespace NodeOptimization
{
    public class Node
    {
        public event FireHandler Fire;
        public delegate void FireHandler(Node upstream);
        public event PathReadyHandler PathReady;
        public delegate void PathReadyHandler(Node n);

        public string Name = "";
        
        // for loop back
        public ConcurrentBag<Node> Upstream = new ConcurrentBag<Node>();

        // this node gets set at the decision stage
        public ConcurrentDictionary<Node, Node> Downstream = new ConcurrentDictionary<Node, Node>();
        public bool Dest = false;
        public Action<string> OptCallback = null;
        public Action<double> ErrorCallback = null;
        public Node(string _name = "", Action<string> optCallback = null, Action<double> errorCallback = null)
        {
            Name = _name;
            Dest = false;
            OptCallback = optCallback;
            ErrorCallback = errorCallback;
        }
        public void ReceiveInput(Node upstream)
        {
            /*
            new Thread(() =>
            {
                if (Faults.LinkState(upstream.Name, this.Name))
                {
                    AccSleep(Faults.LinkDistance(upstream.Name, this.Name)*30);
                    Upstream.Add(upstream);
                    //Console.WriteLine(Name + " received input from " + upstream.Name + " and fired to downstream.");
                    Fire(this);
                }
            }).Start();
            */
            ThreadPool.QueueUserWorkItem(callback =>
            {
                if (Faults.LinkState(upstream.Name, this.Name))
                {
                    AccSleep(Faults.LinkDistance(upstream.Name, this.Name)*100);
                    Upstream.Add(upstream);
                    //Console.WriteLine(Name + " received input from " + upstream.Name + " and fired to downstream.");
                    if (Fire != null) Fire(this);
                }
            });
        }

        public void FireDownstream(string dest)
        {
            if (Downstream.Count == 0) return;
            List<Node> dests = new List<Node>(Downstream.Keys);
            for (int i = 0; i < dests.Count; i++)
            {
                if (dests[i].Name == dest)
                {
                    Node d;
                    if (Downstream.TryGetValue(dests[i], out d))
                    {
                        //Console.WriteLine("Dest " + dests[i].Name + " optimal path: " + this.Name + " to " + d.Name);
                        if (OptCallback != null) OptCallback(this.Name + "-" + d.Name);
                        d.FireDownstream(dest);
                    }
                }
            }
        }

        public void ValidPath(Node dest, Node downstream)
        {
            if (!Downstream.ContainsKey(dest))
            {
                Downstream.TryAdd(dest, downstream);
                //Console.WriteLine("For dest " + dest.Name + ", valid path between " + this.Name + " and " + downstream.Name);
            }
        }

        // loop back the information to the originator
        public void LoopBack(Node dest)
        {
            if (Upstream.Count == 0) return;
            for (int i = 0; i < Upstream.Count; i++)
            {
                Node u;
                if (Upstream.TryPeek(out u))
                {
                    if (!u.Downstream.ContainsKey(dest))
                    {
                        u.ValidPath(dest, this);
                        if (PathReady != null) PathReady(this);
                        //Console.WriteLine(this.Name + " relaying back to " + u.Name);
                        u.LoopBack(dest);
                    }
                }
            }
        }

        public void AccSleep(int msec)
        {
            System.Diagnostics.Stopwatch stpw = new System.Diagnostics.Stopwatch();
            stpw.Start();
            while (stpw.ElapsedMilliseconds < msec)
            {
                Thread.Sleep(10);
            }
            int em = (int)stpw.ElapsedMilliseconds;
            double p = Math.Round(100 * (em * 1.0 - msec * 1.0) / msec * 1.0, 2);
            if (ErrorCallback != null) ErrorCallback(p);
            //Console.WriteLine(p + "% error");
        }
    }
}