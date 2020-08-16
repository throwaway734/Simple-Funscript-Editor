/* My philosophy on this sort of thing is "Why can't we all just get along." That said, this program uses FFmpeg,
 * so now I have to make sure everything is LGPL-compliant. I'm pretty sure it's right. Please let me know if not. 
 * 
 * Preferably without involving a lawyer.
 * 
 * If it's not right I'll probably just take this down, because I'd rather delete everything than spend another minute
 * reading legalese.
 */


using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace ScriptEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		private bool mediaPlayerIsPlaying = false;
		private bool userIsDraggingSlider = false;
		public Dictionary<double, int> keyframesDict = new Dictionary<double, int>();
		string videoPath = "";
		double CurrFrame
        {
			get => Math.Round(mePlayer.FramePosition.TotalMilliseconds);
		}

		public MainWindow()
		{
			Unosquare.FFME.Library.FFmpegDirectory = @"/ffmpeg/bin";
			InitializeComponent();


			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(0.05);
			timer.Tick += timer_Tick;
			timer.Start();
			mePlayer.Volume = 0;
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			if ((mePlayer.Source != null) && (mePlayer.NaturalDuration.HasValue) && (!userIsDraggingSlider))
			{
				sliProgress.Minimum = 0;
				sliProgress.Maximum = mePlayer.NaturalDuration.Value.TotalSeconds;
				sliProgress.Value = mePlayer.Position.TotalSeconds;
			}
			//Draw current haptic position
			double currPos = 0f;
			var prevHaptic = keyframesDict.Where(a => a.Key < CurrFrame).OrderByDescending(a => a.Key).FirstOrDefault();
			var nextHaptic = keyframesDict.Where(a => a.Key >= CurrFrame).OrderBy(a => a.Key).FirstOrDefault();
			double t = ((double)CurrFrame - (double)prevHaptic.Key) / ((double)nextHaptic.Key - (double)prevHaptic.Key);
			currPos = (double)prevHaptic.Value + ((nextHaptic.Value - prevHaptic.Value) * t);
			if (double.IsNaN(currPos) || currPos < 0)
				currPos = 0d;
			currPosition.Value = currPos;
			if (keyframesDict.ContainsKey(CurrFrame))
			{
				keyframeLabel.Content = "Keyframe";
			}
			else
			{
				keyframeLabel.Content = "";
			}
		}

		private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			keyframesDict.Clear();
			UpdateKeyframeDisplay();
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if(!String.IsNullOrEmpty(videoPath))
				openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(videoPath);
			openFileDialog.Filter = "Video Files|*.webm;*.mkv;*.flv;*.264;*.264;*.flv;*.264;*.vob;*.262;*.vob;*.ogv;*.ogg;*.drc;*.gif;*.gifv;*.g.;*.webm;*.gifv;*.mng;*.avi;*.MTS;*.M2TS;*.TS;*.264;*.mov;*.qt;*.wmv;*.net;*.yuv;*.rm;*.rmvb;*.viv;*.263;*.723;*.asf;*.amv;*.mp4;*.m4p;*.m4v;*.264;*.mpg;*.mp2;*.mpeg;*.mpe;*.mpv;*.mpg;*.mpeg;*.m2v;*.262;*.m4v;*.264;*.svi;*.3gp;*.263;*.264;*.3g2;*.263;*.264;*.mxf;*.roq;*.nsv;*.flv;*.f4v;*.f4p;*.f4a;*.f4b";
			if (openFileDialog.ShowDialog() == true)
			{
				mePlayer.Open(new Uri(openFileDialog.FileName));
				videoPath = openFileDialog.FileName;
				var fsPath = videoPath.Substring(0, videoPath.Length - Path.GetExtension(videoPath).Length) + ".funscript";
				if (File.Exists(fsPath))
                {
					LoadFS(fsPath);
                }
			}
		}

		private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (mePlayer != null) && (mePlayer.Source != null);
		}

		private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			mePlayer.Play();
			mediaPlayerIsPlaying = true;
		}
		public void NextFrame_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }
		private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = mediaPlayerIsPlaying;
		}

		private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			mePlayer.Pause();
		}

		private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = mediaPlayerIsPlaying;
		}

		private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			mePlayer.Stop();
			mediaPlayerIsPlaying = false;
		}

		private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
		{
			userIsDraggingSlider = true;
		}

		private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			userIsDraggingSlider = false;
			mePlayer.Position = TimeSpan.FromSeconds(sliProgress.Value);
		}

		private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			lblProgressStatus.Text = TimeSpan.FromSeconds(Math.Round(sliProgress.Value)).ToString();
		}

		private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			mePlayer.SpeedRatio += (e.Delta > 0) ? 0.1 : -0.1;
		}

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
			{
				case Key.S:
					mePlayer.SpeedRatio -= 0.1f;
					break;
				case Key.W:
					mePlayer.SpeedRatio += 0.1f;
					break;
				case Key.Space:
					if (!mePlayer.IsOpen)
						break;
					if (mePlayer.IsPlaying)
						mePlayer.Pause();
					else
						mePlayer.Play();
					break;
				case Key.A:
					mePlayer.StepBackward();
					if (mePlayer.IsPlaying)
						mePlayer.Pause();
					break;
				case Key.D:
					mePlayer.StepForward();
					if (mePlayer.IsPlaying)
						mePlayer.Pause();
					break;
				case Key.Q:
				case Key.Delete:
					if (keyframesDict.ContainsKey(CurrFrame))
					{
						keyframesDict.Remove(CurrFrame);
						UpdateKeyframeDisplay();
					}
					break;
				case Key.NumPad1:
				case Key.D1:
					AddKeyframe(11);
					break;
				case Key.D2:
				case Key.NumPad2:
					AddKeyframe(22);
					break;
				case Key.D3:
				case Key.NumPad3:
					AddKeyframe(33);
					break;
				case Key.D4:
				case Key.NumPad4:
					AddKeyframe(44);
					break;
				case Key.D5:
				case Key.NumPad5:
					AddKeyframe(55);
					break;
				case Key.D6:
				case Key.NumPad6:
					AddKeyframe(66);
					break;
				case Key.D7:
				case Key.NumPad7:
					AddKeyframe(77);
					break;
				case Key.D8:
				case Key.NumPad8:
					AddKeyframe(88);
					break;
				case Key.D9:
				case Key.NumPad9:
					AddKeyframe(99);
					break;
				case Key.D0:
				case Key.NumPad0:
					AddKeyframe(0);
					break;
			}
        }
		private void AddKeyframe(int num)
        {
			if (keyframesDict.ContainsKey(CurrFrame))
				keyframesDict[CurrFrame] = num;
			else
				keyframesDict.Add(CurrFrame, num);
			UpdateKeyframeDisplay();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if(!String.IsNullOrEmpty(videoPath))
				openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(videoPath);
			openFileDialog.Filter = "Funscript Files|*.funscript";
			if (openFileDialog.ShowDialog() == true)
            {
				LoadFS(openFileDialog.FileName);
			}
		}
		private void LoadFS(string path)
        {

			var inFile = File.ReadAllText(path);
			var keys = Regex.Matches(inFile, ".pos.:([0-9]+),.?.at.: *([0-9]+)");
			foreach (Match a in keys)
				keyframesDict.Add(Convert.ToInt32(a.Groups[2].Value), Convert.ToInt32(a.Groups[1].Value));
			UpdateKeyframeDisplay();
		}
		private void UpdateKeyframeDisplay()
		{
			//Obsolete
		}
		private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
			e.CanExecute = keyframesDict.Count > 0;
        }
		private void Save_Executed(object sender, RoutedEventArgs e)
        {
			SaveFileDialog sd = new SaveFileDialog();
			if (!String.IsNullOrEmpty(videoPath))
				sd.InitialDirectory = System.IO.Path.GetDirectoryName(videoPath);
			if (!String.IsNullOrEmpty(videoPath))
				sd.FileName = System.IO.Path.GetFileNameWithoutExtension(videoPath) + ".funscript";
			sd.Filter = "Funscript Files|*.funscript";
			sd.AddExtension = true;
			sd.DefaultExt = "funscript";
            if (sd.ShowDialog() == true)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("{\"version\": \"1.0\", \"actions\": [{");
				sb.Append(String.Join("},{", keyframesDict.Keys.OrderBy(a => a).Select(a => $"\"pos\":{keyframesDict[a]}, \"at\":{a}")));
				sb.Append("}], \"inverted\": false, \"range\": 100}");
				File.WriteAllText(sd.FileName, sb.ToString());
			}
        }

	}
}
