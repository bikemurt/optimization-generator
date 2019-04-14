using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace NodeOptimization
{
    public partial class Form1 : Form
    {
        // parameters: xr = x rate, o = offset, r = radius, fpx = first position x, fpy = first position y
        int xr = 80;
        int yr = 80;
        int xo = 100;
        int yo = 100;
        int r = 4;

        int fpx = 0;
        int fpy = 0;
        int lpx = 0;
        int lpy = 0;

        // these must be even
        int xc = 20;
        int yc = 10;

        // limit on distance random number
        int dlim = 10;

        public Form1()
        {
            InitializeComponent();
            fpx = -1 * xr + xo;
            fpy = (yc / 2) * yr + yo;
            lpx = xc * xr + xo;
            lpy = (yc / 2) * yr + yo;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // how to prevent the paint handler from clearing my graphics?
        }

        public Dictionary<string, int> prevYS = new Dictionary<string, int>();
        public void LoadNetwork(bool yRand = true)
        {
            this.Invoke(new Action(() =>
            {
                optLabel.Visible = true;
            }));
            using (Graphics e = this.CreateGraphics())
            {
                e.Clear(Control.DefaultBackColor);
                Pen p = new Pen(Color.FromArgb(0, 0, 0));
                SolidBrush b = new SolidBrush(Color.White);
                //e.Graphics.DrawLine(p, new Point(20, 20), new Point(100, 100));
                //e.Graphics.DrawEllipse(p, 50, 50, 100, 100);

                Node[,] nodes = new Node[xc, yc];
                int cXPos = 0;
                int cYPos = 0;
                int cXPosPrev = 0;
                int cYPosPrev = 0;


                Random rand = new Random();
                Font f = new Font(FontFamily.GenericSansSerif, 10.0f);
                SolidBrush fb = new SolidBrush(Color.Black);
                Node A = new Node("A", Opt, Error);
                Node Z = new Node("Z", Opt, Error);
                for (int x = 0; x < xc; x++)
                {
                    cXPos = (x * xr) + xo;
                    for (int y = 0; y < yc; y++)
                    {
                        string nName = x.ToString() + "_" + y.ToString();
                        nodes[x, y] = new Node(nName, Opt, Error);
                        cYPos = (y * yr) + yo;
                        if (x == 0)
                        {
                            e.DrawLine(p, new Point(fpx + r, fpy + r), new Point(cXPos + r, cYPos + r));
                            int d1 = rand.Next(1, dlim);
                            int hx = (int)Math.Round((fpx + cXPos) * 1.0 / 2, 0);
                            int hy = (int)Math.Round((fpy + cYPos) * 1.0 / 2, 0);
                            e.DrawString(d1.ToString(), f, fb, new Point(hx, hy));
                            if (yRand)
                            {
                                Faults.States.Add("A" + nName, 1);
                                Faults.Distances.Add("A" + nName, d1);
                            }
                            A.Fire += nodes[x, y].ReceiveInput;
                        }
                        if (x > 0)
                        {
                            int pyr = 0;
                            if (yRand)
                            {
                                pyr = rand.Next(0, yc - 1);
                                prevYS.Add(x + "," + y, pyr);
                            }
                            else
                            {
                                if (prevYS.ContainsKey(x + "," + y))
                                {
                                    pyr = prevYS[x + "," + y];
                                }
                            }

                            int prevYRand = (pyr * yr) + yo;
                            e.DrawLine(p, new Point(cXPosPrev + r, prevYRand + r), new Point(cXPos + r, cYPos + r));
                            int d2 = rand.Next(1, dlim);
                            int hx = (int)Math.Round((cXPosPrev + cXPos) * 1.0 / 2, 0);
                            int hy = (int)Math.Round((prevYRand + cYPos) * 1.0 / 2, 0);
                            e.DrawString(d2.ToString(), f, fb, new Point(hx, hy));
                            string pName = (x - 1).ToString() + "_" + pyr.ToString();
                            if (yRand)
                            {
                                Faults.States.Add(pName + nName, 1);
                                Faults.Distances.Add(pName + nName, d2);
                            }
                            nodes[x - 1, pyr].Fire += nodes[x, y].ReceiveInput;
                        }
                        if (x == xc - 1)
                        {
                            e.DrawLine(p, new Point(cXPos + r, cYPos + r), new Point(lpx + r, lpy + r));
                            int d3 = rand.Next(1, dlim);
                            int hx = (int)Math.Round((lpx + cXPos) * 1.0 / 2, 0);
                            int hy = (int)Math.Round((lpy + cYPos) * 1.0 / 2, 0);
                            e.DrawString(d3.ToString(), f, fb, new Point(hx, hy));
                            if (yRand)
                            {
                                Faults.States.Add(nName + "Z", 1);
                                Faults.Distances.Add(nName + "Z", d3);
                            }
                            nodes[x, y].Fire += Z.ReceiveInput;
                        }
                        // probably won't use
                        cYPosPrev = cYPos;
                    }
                    cXPosPrev = cXPos;
                }
                for (int x = 0; x < xc; x++)
                {
                    cXPos = (x * xr) + xo;
                    for (int y = 0; y < yc; y++)
                    {
                        cYPos = (y * yr) + yo;
                        e.FillEllipse(b, cXPos, cYPos, 2 * r, 2 * r);
                        e.DrawEllipse(p, cXPos, cYPos, 2 * r, 2 * r);
                    }
                }
                A.PathReady += A_PathReady;
                Z.Fire += ToDest;

                e.FillEllipse(b, fpx, fpy, 2 * r, 2 * r);
                e.DrawEllipse(p, fpx, fpy, 2 * r, 2 * r);
                e.FillEllipse(b, lpx, lpy, 2 * r, 2 * r);
                e.DrawEllipse(p, lpx, lpy, 2 * r, 2 * r);

                if (yRand)
                {
                    Faults.States.Add("0A", 1);
                    Faults.Distances.Add("0A", 1);
                }
                A.ReceiveInput(new Node("0"));
            }
        }

        private void A_PathReady(Node n)
        {
            n.FireDownstream("Z");
        }

        private void ToDest(Node upstream)
        {
            //Console.WriteLine(upstream.Name + " to dest");
            upstream.LoopBack(upstream);
        }

        double pCnt = 0;
        double pSum = 0;
        public void Error(double p)
        {
            pSum += p;
            pCnt++;
        }
        
        public void Opt(string data)
        {
            this.Invoke(new Action(() =>
            {
                errorTextBox.Text = (pSum / pCnt).ToString();
                optLabel.Visible = false;
            }));
            
            using (Graphics g = this.CreateGraphics())
            {
                try
                {
                    int x1 = 0;
                    int y1 = 0;
                    int x2 = 0;
                    int y2 = 0;
                    int x1pos = 0;
                    int y1pos = 0;
                    int x2pos = 0;
                    int y2pos = 0;
                    if (data.Contains("A"))
                    {
                        x2 = Convert.ToInt32(data.Split('-')[1].Split('_')[0]);
                        y2 = Convert.ToInt32(data.Split('-')[1].Split('_')[1]);

                        x1pos = fpx;
                        y1pos = fpy;
                        x2pos = (x2 * xr) + xo;
                        y2pos = (y2 * yr) + yo;
                    }
                    else if (data.Contains("Z"))
                    {
                        x1 = Convert.ToInt32(data.Split('-')[0].Split('_')[0]);
                        y1 = Convert.ToInt32(data.Split('-')[0].Split('_')[1]);

                        x1pos = (x1 * xr) + xo;
                        y1pos = (y1 * yr) + yo;
                        x2pos = lpx;
                        y2pos = lpy;
                    }
                    else
                    {
                        x1 = Convert.ToInt32(data.Split('-')[0].Split('_')[0]);
                        y1 = Convert.ToInt32(data.Split('-')[0].Split('_')[1]);
                        x2 = Convert.ToInt32(data.Split('-')[1].Split('_')[0]);
                        y2 = Convert.ToInt32(data.Split('-')[1].Split('_')[1]);
                        x1pos = (x1 * xr) + xo;
                        y1pos = (y1 * yr) + yo;
                        x2pos = (x2 * xr) + xo;
                        y2pos = (y2 * yr) + yo;
                    }
                    g.DrawLine(new Pen(Color.FromArgb(255, 0, 0), 3.0f), new Point(x1pos+r, y1pos+r), new Point(x2pos+r, y2pos+r));
                }
                catch { }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            prevYS.Clear();
            Faults.States.Clear();
            Faults.Distances.Clear();

            LoadNetwork();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string data = "";
            foreach (var item in prevYS)
            {
                data += item.Key + "=" + item.Value.ToString() + ";";
            }
            data += "&";
            foreach (var item in Faults.States)
            {
                data += item.Key + "=" + item.Value.ToString() + ";";
            }
            data += "&";
            foreach (var item in Faults.Distances)
            {
                data += item.Key + "=" + item.Value.ToString() + ";";
            }
            SaveFileDialog fd = new SaveFileDialog();
            fd.InitialDirectory = @"C:\Users\mburt\Desktop";
            fd.Filter = "Opt Files (*.opt)|*.opt";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(fd.FileName, data);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.InitialDirectory = @"C:\Users\mburt\Desktop";
            fd.Filter = "Opt Files (*.opt)|*.opt";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                prevYS.Clear();
                Faults.States.Clear();
                Faults.Distances.Clear();

                string data = System.IO.File.ReadAllText(fd.FileName);
                string[] segs = data.Split('&');
                string[] vars1 = segs[0].Split(';');
                for (int i = 0; i < vars1.Length; i++)
                {
                    string line = vars1[i];
                    if (line == "") continue;
                    string key = line.Split('=')[0];
                    string val = line.Split('=')[1];
                    try
                    {
                        prevYS.Add(key, Convert.ToInt32(val));
                    }
                    catch { }
                }
                string[] vars2 = segs[1].Split(';');
                for (int i = 0; i < vars2.Length; i++)
                {
                    string line = vars2[i];
                    if (line == "") continue;
                    string key = line.Split('=')[0];
                    string val = line.Split('=')[1];
                    try
                    {
                        Faults.States.Add(key, Convert.ToInt32(val));
                    }
                    catch { }
                }
                string[] vars3 = segs[2].Split(';');
                for (int i = 0; i < vars3.Length; i++)
                {
                    string line = vars3[i];
                    if (line == "") continue;
                    string key = line.Split('=')[0];
                    string val = line.Split('=')[1];
                    try
                    {
                        Faults.Distances.Add(key, Convert.ToInt32(val));
                    }
                    catch { }
                }
                LoadNetwork(false);
            }
        }
    }
}
