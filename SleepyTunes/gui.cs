using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Timers;
using CoreAudioApi;
using System.Diagnostics;
using SleepyTunes.Properties;
using ZedGraph;

namespace SleepyTunes
{
    public partial class Gui : Form
    {

        private readonly MMDevice _audDev;
        private readonly System.Timers.Timer _clock;
        private DateTime _tStart, _tEnd;
        private double _endVol;
        private bool _isRunning;

        public Gui()
        {
            MMDeviceEnumerator devEnum;
            try
            {
                devEnum = new MMDeviceEnumerator();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.gui_Error);
                Application.Exit();
                return;
            }

            _audDev = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            if (_audDev == null)
            {
                MessageBox.Show(Resources.gui_No_default_audio_device_found, Resources.gui_Error);
                Application.Exit();
                return;
            }
            //tbStartVol.Value = (int)(audDev.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            //chkStartMute.Checked = audDev.AudioEndpointVolume.Mute;
            _audDev.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
            InitializeComponent();
            double currentVol = (_audDev.AudioEndpointVolume.MasterVolumeLevelScalar * 100);

            CreateGraph(zg1, currentVol > 0 ? currentVol : 50, 0);
            SetSize(zg1);

            txtRestoreVol.Text = "" + (int)currentVol;

            _clock = new System.Timers.Timer();
            _clock.Elapsed += ClockElapsed;
            _clock.Interval = 500; // ms

        }

        #region ZedGraph

        private void CreateGraph(ZedGraphControl zgc, double startVol, double endVol)
        {
            GraphPane myPane = zgc.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "";
            myPane.XAxis.Title.Text = "Elapsed Time (h:m:s)";
            myPane.YAxis.Title.Text = "Volume (%)";

            myPane.XAxis.ScaleFormatEvent += XAxisScaleFormatEvent;

            // Add a progress symbol
            var listProgress = new PointPairList {{0, -1}};
            var progress = myPane.AddCurve("", listProgress, Color.Orange, SymbolType.Star);
            progress.Symbol.Fill = new Fill(Color.Orange);

            // Create the initial graph
            var list = new PointPairList
                           {
                               {0, startVol},
                               {15*60, (startVol - ((startVol - endVol)*0.08))},
                               {30*60, (startVol - ((startVol - endVol)*0.25))},
                               {60*60, endVol}
                           };

            // Generate a blue curve with circle symbols
            LineItem myCurve = myPane.AddCurve("", list, Color.Blue, SymbolType.Circle);
            // Fill the area under the curve with a white-red gradient at 45 degrees
            myCurve.Line.Fill = new Fill(Color.White, Color.FromArgb(40, Color.Red), 45F);
            // Make the symbols opaque by filling them with white
            myCurve.Symbol.Fill = new Fill(Color.White);

            // Add the current volume as a bar
            var listVolBar = new PointPairList {{0, (_audDev.AudioEndpointVolume.MasterVolumeLevelScalar*100)}};
            myPane.AddBar("Current Volume", listVolBar, (_audDev.AudioEndpointVolume.Mute) ? Color.Red : Color.Green);


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
            var formRect = ClientRectangle;
            formRect.Inflate(-10, -85);
            formRect.Offset(0, -75);

            if (zgc.Size != formRect.Size)
            {
                zgc.Location = formRect.Location;
                zgc.Size = formRect.Size;
            }
        }

        static string XAxisScaleFormatEvent(GraphPane pane, Axis axis, double val, int index)
        {
            var h = (int)val / 60 / 60;
            var m = (int)val / 60 - (h * 60);
            var s = val - (m * 60) - (h * 60 * 60);
            return "" + ((h > 0) ? (h.ToString("0") + ":") : "") + m.ToString("00") + ":" + s.ToString("00.##");
        }

        private bool Zg1DoubleClick(ZedGraphControl control, MouseEventArgs e)
        {
            double curX, curY;
            CurveItem nCurve;
            int i;
            var myPane = control.GraphPane;
            var mousePt = new PointF(e.X, e.Y);
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
                var newP = new PointPair(curX, curY);
                enforceBounds(newP);
                myPane.CurveList[1].AddPoint(newP);
            }
            enforceFunction(myPane.CurveList[1]);
            control.Refresh();

            return true;
        }

        private string Zg1PointEditEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            enforceBounds(curve[iPt]);
            enforceFunction(curve);

            if (curve == pane.CurveList[2])
            {
                // Set the system volume
                _audDev.AudioEndpointVolume.MasterVolumeLevelScalar = ((float)curve[iPt].Y / 100.0f);
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
            var ppl = (PointPairList)c.Points;
            ppl.Sort();
            if (ppl.Count > 0)
            {
                ppl[0].X = 0;
                for (var i = (ppl.Count - 1); i > 0; i--)
                {
                    if (ppl[i].X == ppl[i - 1].X)
                    {
                        ppl.RemoveAt(i);
                    }
                }
            }
        }

        private void Zg1ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            sender.GraphPane.XAxis.Scale.Min = 0;
        }

        private string Zg1PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            var val = curve[iPt].X;
            var h = (int)val / 60 / 60;
            var m = (int)val / 60 - (h * 60);
            var s = val - (m * 60) - (h * 60 * 60);
            return "(" + ((h > 0) ? (h.ToString("0") + ":") : "") + m.ToString("00") + ":" + s.ToString("00.##") + ", " + curve[iPt].Y.ToString("0.##") + "%)";
        }

        #endregion


        PointPair PrevPoint(double t)
        {
            var ppl = (PointPairList)zg1.GraphPane.CurveList[1].Points;

            var prev = ppl[0];


            foreach (var p in ppl)
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

        PointPair NextPoint(double t)
        {
            var ppl = (PointPairList)zg1.GraphPane.CurveList[1].Points;

            foreach (var p in ppl.Where(p => t < p.X))
            {
                return p;
            }

            return ppl[ppl.Count - 1];
        }

        //This function runs once every second
        void ClockElapsed(object sender, ElapsedEventArgs e)
        {
            // Figure out elapsted time, t
            var t = DateTime.Now.Subtract(_tStart).TotalSeconds;
            var tRem = _tEnd.Subtract(DateTime.Now).TotalSeconds;

            if (tRem > 0)
            {
                PointPair prev = PrevPoint(t);
                PointPair next = NextPoint(t);

                if (prev.X == next.X)
                {
                    return;
                }

                var slope = (next.Y - prev.Y) / (next.X - prev.X);

                var vol = slope * (t - prev.X) + prev.Y;

                _audDev.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(vol / 100.0);

                // Show progress indicator at current point
                var theShit = new MethodInvoker(() =>
                                                    {
                                                        zg1.GraphPane.CurveList[0].Points[0].X = t;
                                                        zg1.GraphPane.CurveList[0].Points[0].Y = vol;
                                                        zg1.Refresh();
                                                    });
                if (InvokeRequired)
                {
                    BeginInvoke(theShit);
                }
                else
                {
                    theShit.Invoke();
                }
            }
            else
            {
                _clock.Stop();
                _audDev.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(_endVol / 100.0);

                var theShit = new MethodInvoker(() =>
                                                    {
                                                        //Set Audio to Mute
                                                        if (chkMuteWhenDone.Checked)
                                                        {
                                                            _audDev.AudioEndpointVolume.Mute = true;
                                                        }

                                                        zg1.GraphPane.CurveList[0].Points[0].X = t;
                                                        zg1.GraphPane.CurveList[0].Points[0].Y = _endVol;
                                                        zg1.Refresh();

                                                        //Set Volume Level
                                                        if (chkRestoreVol.Checked)
                                                        {
                                                            try
                                                            {
                                                                _audDev.AudioEndpointVolume.MasterVolumeLevelScalar =
                                                                    Single.Parse(txtRestoreVol.Text)/100.0f;
                                                            }
                                                            catch {}
                                                        }

                                                        //Standby
                                                        switch (comboAction.SelectedIndex)
                                                        {
                                                            case 1:
                                                                Application.SetSuspendState(PowerState.Suspend, true, true);
                                                                break;
                                                            case 2:
                                                                Application.SetSuspendState(PowerState.Hibernate, true, true);
                                                                break;
                                                            case 3:
                                                                CmdExec("shutdown /s /t 0");
                                                                break;
                                                            case 4:
                                                                CmdExec("shutdown /r /t 0");
                                                                break;
                                                            case 5:
                                                                Program.LockWorkStation();
                                                                break;
                                                            case 6:
                                                                Program.ExitWindowsEx((uint) Program.ExitVals.LogOff, 0);
                                                                break;
                                                            case 7:
                                                                CmdExec(txtRunCmd.Text);
                                                                break;
                                                        }

                                                        //Close App, this has to be last
                                                        if (chkExit.Checked)
                                                        {
                                                            Application.Exit();
                                                        }
                                                        else
                                                        {
                                                            _isRunning = false;
                                                            btnGo.Image = Resources.startButton;
                                                            zg1.Enabled = true;
                                                        }
                                                    });
                if (InvokeRequired)
                {
                    BeginInvoke(theShit);
                }
                else
                {
                    theShit.Invoke();
                }
            }
        }

        static void CmdExec(String cmd)
        {
            var p = new Process
                        {
                            StartInfo =
                                {
                                    FileName = "cmd",
                                    Arguments = "/c " + cmd,
                                    CreateNoWindow = true,
                                    WindowStyle = ProcessWindowStyle.Hidden
                                }
                        };
            p.Start();
        }

        void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            var theShit = new MethodInvoker(() =>
                                                {
                                                    var c = zg1.GraphPane.CurveList[2];
                                                    var ppl = (PointPairList) c.Points;

                                                    ppl[0].Y = (_audDev.AudioEndpointVolume.MasterVolumeLevelScalar*100);
                                                    c.Color = (_audDev.AudioEndpointVolume.Mute)
                                                                  ? Color.Red
                                                                  : Color.Green;

                                                    zg1.Refresh();
                                                });
            if (InvokeRequired)
            {
                BeginInvoke(theShit);
            }
            else
            {
                theShit.Invoke();
            }
        }

        private void BtnGoClick(object sender, EventArgs e)
        {
            if (!_isRunning)
            {
                // Start running
                var ppl = (PointPairList)zg1.GraphPane.CurveList[1].Points;
                var endPt = ppl[ppl.Count - 1];

                _tStart = DateTime.Now;
                _tEnd = _tStart.AddSeconds(endPt.X);

                _endVol = endPt.Y;

                if (_tEnd.Subtract(_tStart).TotalSeconds > 0)
                {
                    btnGo.Image = Resources.stopButton;
                    zg1.Enabled = false;
                    zg1.AxisChange();
                    _isRunning = true;
                    _clock.Start();
                }
                else
                {
                    MessageBox.Show(Resources.Gui_BtnGoClick_The_duration_must_be_greater_than_0_seconds_);
                }
            }
            else
            {
                // Abort current run!
                _clock.Stop();
                _isRunning = false;
                btnGo.Image = Resources.startButton;
                zg1.Enabled = true;
            }
            
        }


        private void ChkRestoreVolCheckedStateChanged(object sender, EventArgs e)
        {
            txtRestoreVol.Enabled = (chkRestoreVol.CheckState == CheckState.Checked);
        }

        private void TxtRestoreVolLeave(object sender, EventArgs e)
        {
            try
            {
                if (Single.Parse(txtRestoreVol.Text) > 100)
                {
                    txtRestoreVol.Text = @"100";
                }
                else if (Single.Parse(txtRestoreVol.Text) < 0)
                {
                    txtRestoreVol.Text = @"0";
                }
            }
            catch (Exception)
            {
                txtRestoreVol.Text = @"0";
            }
        }

        private void ComboActionSelectedIndexChanged(object sender, EventArgs e)
        {
            txtRunCmd.Visible = (comboAction.SelectedIndex == 7);
        }

        private void GuiResize(object sender, EventArgs e)
        {
            SetSize(zg1);
        }
    }
}
