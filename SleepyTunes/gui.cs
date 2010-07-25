using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using CoreAudioApi;
using System.Diagnostics;
using ZedGraph;

namespace SleepyTunes
{
    public partial class gui : Form
    {

        private MMDevice audDev;
        private System.Timers.Timer clock;
        private DateTime tStart, tEnd;
        private double endVol;
        private bool isRunning = false;

        public gui()
        {
            
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            if (DevEnum == null)
            {
                MessageBox.Show("Could not enumerate audio devices", "Error");
                Application.Exit();
                return;
            }

            audDev = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            if (audDev == null)
            {
                MessageBox.Show("No default audio device found", "Error");
                Application.Exit();
                return;
            }
            //tbStartVol.Value = (int)(audDev.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            //chkStartMute.Checked = audDev.AudioEndpointVolume.Mute;
            audDev.AudioEndpointVolume.OnVolumeNotification += new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification);
            InitializeComponent();
            double currentVol = (audDev.AudioEndpointVolume.MasterVolumeLevelScalar * 100);

            if (currentVol > 0)
            {
                CreateGraph(zg1, currentVol, 0);
            }
            else
            {
                CreateGraph(zg1, 50, 0);
            }
            SetSize(zg1);

            txtRestoreVol.Text = "" + (int)currentVol;

            clock = new System.Timers.Timer();
            clock.Elapsed += new ElapsedEventHandler(clock_Elapsed);
            clock.Interval = 500; // ms

        }

        #region ZedGraph

        private void CreateGraph(ZedGraphControl zgc, double startVol, double endVol)
        {
            GraphPane myPane = zgc.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "";
            myPane.XAxis.Title.Text = "Elapsed Time (h:m:s)";
            myPane.YAxis.Title.Text = "Volume (%)";

            myPane.XAxis.ScaleFormatEvent += new Axis.ScaleFormatHandler(XAxis_ScaleFormatEvent);

            // Add a progress symbol
            PointPairList listProgress = new PointPairList();
            listProgress.Add(0, -1);
            LineItem progress = myPane.AddCurve("", listProgress, Color.Orange, SymbolType.Star);
            progress.Symbol.Fill = new Fill(Color.Orange);

            // Create the initial graph
            PointPairList list = new PointPairList();
            list.Add(0, startVol);
            list.Add(15 * 60, (startVol - ((startVol - endVol) * 0.08)));
            list.Add(30 * 60, (startVol - ((startVol - endVol) * 0.25)));
            list.Add(60 * 60, endVol);
            
            // Generate a blue curve with circle symbols
            LineItem myCurve = myPane.AddCurve("", list, Color.Blue, SymbolType.Circle);
            // Fill the area under the curve with a white-red gradient at 45 degrees
            myCurve.Line.Fill = new Fill(Color.White, Color.FromArgb(40, Color.Red), 45F);
            // Make the symbols opaque by filling them with white
            myCurve.Symbol.Fill = new Fill(Color.White);

            // Add the current volume as a bar
            PointPairList listVolBar = new PointPairList();
            listVolBar.Add(0, (audDev.AudioEndpointVolume.MasterVolumeLevelScalar * 100));
            BarItem bar = myPane.AddBar("Current Volume", listVolBar, (audDev.AudioEndpointVolume.Mute) ? Color.Red : Color.Green);


            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45F);

            // Fill the pane background with a color gradient
            myPane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45F);

            // Calculate the Axis Scale Ranges
            zgc.AxisChange();

            myPane.XAxis.Scale.MinAuto = false;
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.MaxAuto = true;

            myPane.YAxis.Scale.Min = 0;
            myPane.YAxis.Scale.MinAuto = false;
            myPane.YAxis.Scale.Max = 100;
            myPane.YAxis.Scale.MaxAuto = false;
        }

        private void SetSize(ZedGraphControl zgc)
        {
            // Control is always 10 pixels inset from the client rectangle of the form
            Rectangle formRect = this.ClientRectangle;
            formRect.Inflate(-10, -85);
            formRect.Offset(0, -75);

            if (zgc.Size != formRect.Size)
            {
                zgc.Location = formRect.Location;
                zgc.Size = formRect.Size;
            }
        }

        string XAxis_ScaleFormatEvent(GraphPane pane, Axis axis, double val, int index)
        {
            int h, m;
            double s;

            h = (int)val / 60 / 60;
            m = (int)val / 60 - (h * 60);
            s = val - (m * 60) - (h * 60 * 60);
            return "" + ((h > 0) ? (h.ToString("0") + ":") : "") + m.ToString("00") + ":" + s.ToString("00.##");
        }

        private bool zg1_DoubleClick(ZedGraphControl control, MouseEventArgs e)
        {
            double curX, curY;
            CurveItem nCurve;
            int i;
            GraphPane myPane = control.GraphPane;
            PointF mousePt = new PointF(e.X, e.Y);
            myPane.ReverseTransform(mousePt, out curX, out curY);
            //MessageBox.Show("(" + e.X + "," + e.Y + ") (" + curX + "," + curY + ")");
            if (myPane.FindNearestPoint(mousePt, out nCurve, out i) && nCurve.Points is PointPairList)
            {
                if (nCurve == myPane.CurveList[1])
                {
                    nCurve.RemovePoint(i);
                }
            }
            else
            {
                PointPair newP = new PointPair(curX, curY);
                enforceBounds(newP);
                myPane.CurveList[1].AddPoint(newP);
            }
            enforceFunction(myPane.CurveList[1]);
            control.Refresh();

            return true;
        }

        private string zg1_PointEditEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            enforceBounds(curve[iPt]);
            enforceFunction(curve);

            if (curve == pane.CurveList[2])
            {
                // Set the system volume
                audDev.AudioEndpointVolume.MasterVolumeLevelScalar = ((float)curve[iPt].Y / 100.0f);
            }

            sender.Refresh();
            return default(string);
        }

        private void enforceBounds(PointPair p)
        {
            if (p.X < 0)
            {
                p.X = 0;
            }
            if (p.Y < 0)
            {
                p.Y = 0;
            }
            if (p.Y > 100)
            {
                p.Y = 100;
            }
        }

        private void enforceFunction(CurveItem c)
        {
            PointPairList ppl = (PointPairList)c.Points;
            ppl.Sort();
            if (ppl.Count > 0)
            {
                ppl[0].X = 0;
                for (int i = (ppl.Count - 1); i > 0; i--)
                {
                    if (ppl[i].X == ppl[i - 1].X)
                    {
                        ppl.RemoveAt(i);
                    }
                }
            }
        }

        private void zg1_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            sender.GraphPane.XAxis.Scale.Min = 0;
        }

        private string zg1_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            int h, m;
            double s, val;

            val = curve[iPt].X;

            h = (int)val / 60 / 60;
            m = (int)val / 60 - (h * 60);
            s = val - (m * 60) - (h * 60 * 60);
            return "(" + ((h > 0) ? (h.ToString("0") + ":") : "") + m.ToString("00") + ":" + s.ToString("00.##") + ", " + curve[iPt].Y.ToString("0.##") + "%)";
        }

        #endregion


        PointPair prevPoint(double t)
        {
            PointPairList ppl = (PointPairList)zg1.GraphPane.CurveList[1].Points;

            PointPair prev = ppl[0];


            foreach (PointPair p in ppl)
            {
                if (t == p.X)
                {
                    return p;
                }
                if (t < p.X)
                {
                    return prev;
                }
                prev = p;
            }

            return ppl[ppl.Count-1];
        }

        PointPair nextPoint(double t)
        {
            PointPairList ppl = (PointPairList)zg1.GraphPane.CurveList[1].Points;

            foreach (PointPair p in ppl)
            {
                if (t < p.X)
                {
                    return p;
                }
            }

            return ppl[ppl.Count - 1];
        }

        //This function runs once every second
        void clock_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Figure out elapsted time, t
            double t = DateTime.Now.Subtract(tStart).TotalSeconds;

            double tRem = tEnd.Subtract(DateTime.Now).TotalSeconds;

           

            if (tRem > 0)
            {
                PointPair prev = prevPoint(t);
                PointPair next = nextPoint(t);

                if (prev.X == next.X)
                {
                    return;
                }

                double slope = (next.Y - prev.Y) / (next.X - prev.X);

                double vol = slope * (t - prev.X) + prev.Y;

                audDev.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(vol / 100.0);

                // Show progress indicator at current point
                MethodInvoker theShit = new MethodInvoker(delegate()
                {
                    zg1.GraphPane.CurveList[0].Points[0].X = t;
                    zg1.GraphPane.CurveList[0].Points[0].Y = vol;
                    zg1.Refresh();
                });
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(theShit);
                }
                else
                {
                    theShit.Invoke();
                }
            }
            else
            {
                clock.Stop();
                audDev.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(endVol / 100.0);

                MethodInvoker theShit = new MethodInvoker(delegate()
                {
                    //Set Audio to Mute
                    if (chkMuteWhenDone.Checked)
                    {
                        audDev.AudioEndpointVolume.Mute = true;
                    }

                    zg1.GraphPane.CurveList[0].Points[0].X = t;
                    zg1.GraphPane.CurveList[0].Points[0].Y = endVol;
                    zg1.Refresh();

                    //Set Volume Level
                    if (chkRestoreVol.Checked)
                    {
                        try
                        {
                            audDev.AudioEndpointVolume.MasterVolumeLevelScalar = Single.Parse(txtRestoreVol.Text) / 100.0f;
                        }
                        catch (Exception ex) { }
                    }

                    //Standby
                    if (comboAction.SelectedIndex == 1)
                    {
                        Application.SetSuspendState(PowerState.Suspend, true, true);
                    }
                    //Hibernate
                    else if (comboAction.SelectedIndex == 2)
                    {
                        Application.SetSuspendState(PowerState.Hibernate, true, true);
                    }
                    //Shut down
                    else if (comboAction.SelectedIndex == 3)
                    {
                        //Program.ExitWindowsEx((uint)Program.ExitVals.PowerOff, 0);
                        CMDExec("shutdown /s /t 0");
                    }
                    //Reboot
                    else if (comboAction.SelectedIndex == 4)
                    {
                        //Program.ExitWindowsEx((uint)Program.ExitVals.Reboot, 0);
                        CMDExec("shutdown /r /t 0");
                    }
                    //Lock Workstation
                    else if (comboAction.SelectedIndex == 5)
                    {
                        Program.LockWorkStation();
                    }
                    //Log Off
                    else if (comboAction.SelectedIndex == 6)
                    {
                        Program.ExitWindowsEx((uint)Program.ExitVals.LogOff, 0);
                    }
                    //Execute Command
                    else if (comboAction.SelectedIndex == 7)
                    {
                        CMDExec(txtRunCmd.Text);
                    }

                    //Close App, this has to be last
                    if (chkExit.Checked)
                    {
                        Application.Exit();
                    }
                    else
                    {
                        isRunning = false;
                        btnGo.Image = SleepyTunes.Properties.Resources.startButton;
                        zg1.Enabled = true;
                    }
                });
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(theShit);
                }
                else
                {
                    theShit.Invoke();
                }
            }
        }

        void CMDExec(String cmd)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd";
            p.StartInfo.Arguments = "/c " + cmd;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
        }

        void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            MethodInvoker theShit = new MethodInvoker(delegate()
            {
                CurveItem c = zg1.GraphPane.CurveList[2];
                PointPairList ppl = (PointPairList)c.Points;

                ppl[0].Y = (audDev.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
                c.Color = (audDev.AudioEndpointVolume.Mute) ? Color.Red : Color.Green;

                zg1.Refresh();
            });
            if (this.InvokeRequired)
            {
                this.BeginInvoke(theShit);
            }
            else
            {
                theShit.Invoke();
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                // Start running
                PointPairList ppl = (PointPairList)zg1.GraphPane.CurveList[1].Points;
                PointPair endPt = ppl[ppl.Count - 1];

                tStart = DateTime.Now;
                tEnd = tStart.AddSeconds(endPt.X);

                endVol = endPt.Y;

                if (tEnd.Subtract(tStart).TotalSeconds > 0)
                {
                    btnGo.Image = SleepyTunes.Properties.Resources.stopButton;
                    zg1.Enabled = false;
                    zg1.AxisChange();
                    isRunning = true;
                    clock.Start();
                }
                else
                {
                    MessageBox.Show("The duration must be greater than 0 seconds!");
                }
            }
            else
            {
                // Abort current run!
                clock.Stop();
                isRunning = false;
                btnGo.Image = SleepyTunes.Properties.Resources.startButton;
                zg1.Enabled = true;
            }
            
        }


        private void chkRestoreVol_CheckedStateChanged(object sender, EventArgs e)
        {
            txtRestoreVol.Enabled = (chkRestoreVol.CheckState == CheckState.Checked);
        }

        private void txtRestoreVol_Leave(object sender, EventArgs e)
        {
            try
            {
                if (Single.Parse(txtRestoreVol.Text) > 100)
                {
                    txtRestoreVol.Text = "100";
                }
                else if (Single.Parse(txtRestoreVol.Text) < 0)
                {
                    txtRestoreVol.Text = "0";
                }
            }
            catch (Exception ex)
            {
                txtRestoreVol.Text = "0";
            }
        }

        private void comboAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtRunCmd.Visible = (comboAction.SelectedIndex == 7);
        }

        private void gui_Resize(object sender, EventArgs e)
        {
            SetSize(zg1);
        }
    }
}
