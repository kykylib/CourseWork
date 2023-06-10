using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public partial class MainForm : Form
    {
        private Timer timer;
        private const int MinuteHandSize = 10;
        private const int HourHandSize = 20;
        private DateTime alarmTime;
        private bool isAlarmSet;
        private int stopwatchMilliseconds;
        private bool isStopwatchRunning;

        private readonly PointF[] MinuteHandPoints =
        {
            new PointF(0, -MinuteHandSize),
            new PointF(-4, 0),
            new PointF(0, 4),
            new PointF(4, 0)
        };

        private readonly PointF[] HourHandPoints =
        {
            new PointF(0, -HourHandSize),
            new PointF(-4, 0),
            new PointF(0, 4),
            new PointF(4, 0)
        };

        private Label timeLabel;
        private Button alarmButton;
        private DateTimePicker alarmTimePicker;
        private Panel clockPanel;
        private Button startStopwatchButton;
        private Button resetStopwatchButton;
        private Label stopwatchLabel;

        public MainForm()
        {
            InitializeComponent();

            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            stopwatchMilliseconds = 0;
            isStopwatchRunning = false;
        }

        private void InitializeComponent()
        {
            this.timeLabel = new Label();
            this.alarmButton = new Button();
            this.alarmTimePicker = new DateTimePicker();
            this.clockPanel = new Panel();
            this.startStopwatchButton = new Button();
            this.resetStopwatchButton = new Button();
            this.stopwatchLabel = new Label();
            this.SuspendLayout();
            
            this.timeLabel.AutoSize = true;
            this.timeLabel.Font = new Font("Arial", 16F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.timeLabel.ForeColor = Color.Black;
            this.timeLabel.Location = new Point(20, 20);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new Size(0, 26);
            
            this.alarmButton.Location = new Point(20, 60);
            this.alarmButton.Name = "alarmButton";
            this.alarmButton.Size = new Size(100, 30);
            this.alarmButton.Text = "Set Alarm";
            this.alarmButton.Click += new EventHandler(alarmButton_Click);
            
            this.alarmTimePicker.Format = DateTimePickerFormat.Custom;
            this.alarmTimePicker.CustomFormat = "HH:mm";
            this.alarmTimePicker.ShowUpDown = true;
            this.alarmTimePicker.Location = new Point(130, 60);
            this.alarmTimePicker.Name = "alarmTimePicker";
            this.alarmTimePicker.Size = new Size(70, 20);
            
            this.clockPanel.BackColor = Color.Black;
            this.clockPanel.Location = new Point(20, 120);
            this.clockPanel.Name = "clockPanel";
            this.clockPanel.Size = new Size(180, 180);
            this.clockPanel.Paint += new PaintEventHandler(clockPanel_Paint);
            
            this.startStopwatchButton.Location = new Point(20, 320);
            this.startStopwatchButton.Name = "startStopwatchButton";
            this.startStopwatchButton.Size = new Size(80, 30);
            this.startStopwatchButton.Text = "Start";
            this.startStopwatchButton.Click += new EventHandler(startStopwatchButton_Click);
            
            this.resetStopwatchButton.Location = new Point(110, 320);
            this.resetStopwatchButton.Name = "resetStopwatchButton";
            this.resetStopwatchButton.Size = new Size(80, 30);
            this.resetStopwatchButton.Text = "Reset";
            this.resetStopwatchButton.Click += new EventHandler(resetStopwatchButton_Click);
             
            this.stopwatchLabel.AutoSize = true;
            this.stopwatchLabel.Font = new Font("Arial", 14F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.stopwatchLabel.Location = new Point(20, 370);
            this.stopwatchLabel.Name = "stopwatchLabel";
            this.stopwatchLabel.Size = new Size(0, 23);
            
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(220, 420);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.alarmButton);
            this.Controls.Add(this.alarmTimePicker);
            this.Controls.Add(this.clockPanel);
            this.Controls.Add(this.startStopwatchButton);
            this.Controls.Add(this.resetStopwatchButton);
            this.Controls.Add(this.stopwatchLabel);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Clock";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;

            int hours = currentTime.Hour % 12;
            int minutes = currentTime.Minute;
            int seconds = currentTime.Second;

            clockPanel.Invalidate();

            UpdateTimeLabel(currentTime);

            if (isAlarmSet && currentTime.Hour == alarmTime.Hour && currentTime.Minute == alarmTime.Minute)
            {
                MessageBox.Show("Alarm!");
                isAlarmSet = false;
            }

            if (isStopwatchRunning)
            {
                stopwatchMilliseconds += 100;
                UpdateStopwatchLabel();
            }
        }

        private void clockPanel_Paint(object sender, PaintEventArgs e)
        {
            int cx = clockPanel.Width / 2;
            int cy = clockPanel.Height / 2;
            int r = Math.Min(cx, cy) - 10;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.Black);
            Pen pen = new Pen(Color.White, 2);

            DrawClockFace(g, cx, cy, r, pen);

            DateTime currentTime = DateTime.Now;
            int hours = currentTime.Hour % 12;
            int minutes = currentTime.Minute;
            int seconds = currentTime.Second;

            DrawHands(g, cx, cy, r, hours, minutes, seconds, pen);
        }

        private void DrawClockFace(Graphics g, int cx, int cy, int r, Pen pen)
        {
            g.DrawEllipse(pen, cx - r, cy - r, r * 2, r * 2);

            for (int i = 1; i <= 12; i++)
            {
                double angle = i * Math.PI / 6;
                int x = (int)(cx + (r - 20) * Math.Sin(angle));
                int y = (int)(cy - (r - 20) * Math.Cos(angle));
                g.DrawString(i.ToString(), Font, Brushes.White, x, y);
            }
        }

        private void DrawHands(Graphics g, int cx, int cy, int r, int hours, int minutes, int seconds, Pen pen)
        {
            double hourAngle = (hours + minutes / 60.0) * Math.PI / 6;
            double minuteAngle = minutes * Math.PI / 30;
            double secondAngle = seconds * Math.PI / 30;

            DrawHand(g, cx, cy, r - 50, hourAngle, pen);
            DrawHand(g, cx, cy, r - 20, minuteAngle, pen);
            DrawHand(g, cx, cy, r - 10, secondAngle, pen);
        }

        private void DrawHand(Graphics g, int cx, int cy, int r, double angle, Pen pen)
        {
            int x1 = cx;
            int y1 = cy;
            int x2 = (int)(cx + r * Math.Sin(angle));
            int y2 = (int)(cy - r * Math.Cos(angle));
            g.DrawLine(pen, x1, y1, x2, y2);
            int size = 10;
            double arrowAngle = Math.PI / 6;
            int x3 = (int)(x2 - size * Math.Sin(angle - arrowAngle));
            int y3 = (int)(y2 + size * Math.Cos(angle - arrowAngle));
            int x4 = (int)(x2 - size * Math.Sin(angle + arrowAngle));
            int y4 = (int)(y2 + size * Math.Cos(angle + arrowAngle));
            g.DrawLine(pen, x2, y2, x3, y3);
            g.DrawLine(pen, x2, y2, x4, y4);
        }

        private void UpdateTimeLabel(DateTime time)
        {
            timeLabel.Text = time.ToString("HH:mm:ss tt");
        }

        private void alarmButton_Click(object sender, EventArgs e)
        {
            alarmTime = alarmTimePicker.Value;
            isAlarmSet = true;
            MessageBox.Show("Alarm set for " + alarmTime.ToString("HH:mm:ss tt"));
        }

        private void startStopwatchButton_Click(object sender, EventArgs e)
        {
            isStopwatchRunning = !isStopwatchRunning;
            startStopwatchButton.Text = isStopwatchRunning ? "Stop" : "Start";
        }

        private void resetStopwatchButton_Click(object sender, EventArgs e)
        {
            stopwatchMilliseconds = 0;
            UpdateStopwatchLabel();
        }

        private void UpdateStopwatchLabel()
        {
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(stopwatchMilliseconds);
            stopwatchLabel.Text = timeSpan.ToString(@"hh\:mm\:ss");
        }
    }
}