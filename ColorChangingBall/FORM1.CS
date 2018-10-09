using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Drawing2D;

namespace Game
{
    /// <summary>
    /// 变色球 学习开发
    /// </summary>
    public partial class FrmColorBall : Form
    {
        static Bitmap DeskBmp = null; // 小球绘图区
        Point OldMousePosition = new Point(-1, -1); // 上次的鼠标坐标
        static Random Rnd = new Random();

        const int WIDTH = 500; // 小球绘图区宽度
        const int HEIGHT = 500; // 小球绘图区高度
        const int TITLE = 30; // 标题绘图区高度 
        const int MAXR = 30; // 最大半径
        const double F = 0.001; // 摩擦力

        static List<Ball> Balls = new List<Ball>(); // 小球集合
        static List<LevelInfo> Levels = null; // 关卡集合
        static double Source = 0; // 得分
        static int CurrentLevel = 0; // 当前关卡  
        static bool IsLock = false; // 是否锁定鼠标
        static Point CENTER_POINT = new Point(WIDTH / 2, (HEIGHT - TITLE) / 2); // 绘图区中央点
        static List<DelayText> DelayTexts = new List<DelayText>(); // 滞留文字控制类集合

        Thread MainThread = null; // 绘制小球线程

        public FrmColorBall()
        {
            InitializeComponent();

            this.ClientSize = new Size(WIDTH, HEIGHT + TITLE);
        }

        /// <summary>
        /// 关卡信息
        /// </summary>
        class LevelInfo
        {
            public int MinR = 0; // 最小允许半径
            public int Number = 0; // 小球数量
            public int TurnColor1Time = 0; // 小球变1色秒数
            public int TurnColor2Time = 0; // 小球变2色秒数
            public int TurnColor3Time = 0; // 小球变3色秒数
            public int GameOverTime = 0; // 游戏结束秒数
            public int StartTickCount = 0; // 关卡开始时间戳
            bool _IsStart = false; // 是否已开始
            int TotalElapseTime = 0;  // 关卡时间

            int _ElapseTime = 0; // 剩余时间
            public int ElapseTime
            {
                get
                {
                    return _ElapseTime;
                }
            }

            /// <summary>
            /// 计算剩余时间
            /// </summary>
            public void CalElapseTime()
            {
                _ElapseTime = TotalElapseTime - (Environment.TickCount - StartTickCount) / 1000;
            }

            public bool IsStart
            {
                get
                {
                    return _IsStart;
                }
            }

            public LevelInfo(int ElapseTime, int MinR, int Number, int TurnColor1Time,
               int TurnColor2Time, int TurnColor3Time, int GameOverTime)
            {
                this.MinR = MinR;
                this.Number = Number;
                this.TurnColor1Time = TurnColor1Time;
                this.TurnColor2Time = TurnColor2Time;
                this.TurnColor3Time = TurnColor3Time;
                this.GameOverTime = GameOverTime;
                this._ElapseTime = ElapseTime;
                this.TotalElapseTime = ElapseTime;
            }

            public void Start()
            {
                this.StartTickCount = Environment.TickCount;
                this._IsStart = true;
            }

            public void Stop()
            {
                this._IsStart = false;
            }
        }

        /// <summary>
        /// 滞留文字控制类
        /// </summary>
        class DelayText
        {
            public String Text = String.Empty; // 要显示的文字
            public double EndTickCount = 0;  // 结束时间戳，-1代表永远显示
            public AfterDelayTextEvent AfterEvent = null; // 结束事件
            public bool EventIsDone = false; // 是否已完成结束事件

            public DelayText(String Text, double EndTickCount, AfterDelayTextEvent AfterEvent)
            {
                this.Text = Text;
                this.EndTickCount = EndTickCount;
                this.AfterEvent = AfterEvent;
            }
        }

        /// <summary>
        /// 绘制滞留文字
        /// </summary>
        void DrawDelayText()
        {
            IsLock = false;
            for (int i = 0; i < DelayTexts.Count; i++)
                if (DelayTexts[i].EndTickCount == -1 || Environment.TickCount < DelayTexts[i].EndTickCount)
                {
                    IsLock = true; // 将鼠标锁定
                    DrawText(DelayTexts[i].Text, 30, Brushes.White);
                    if (i > 0 && DelayTexts[i - 1].AfterEvent != null
                        && !DelayTexts[i - 1].EventIsDone)
                    {
                        DelayTexts[i - 1].AfterEvent(); // 如果有，则执行结束事件
                        DelayTexts[i - 1].EventIsDone = true;
                    }
                    return;
                }

            if (DelayTexts.Count > 0 && DelayTexts[DelayTexts.Count - 1].AfterEvent != null
                && !DelayTexts[DelayTexts.Count - 1].EventIsDone) // 执行最后一个成员的结束事件
            {
                DelayTexts[DelayTexts.Count - 1].AfterEvent();
                DelayTexts[DelayTexts.Count - 1].EventIsDone = true;
            }
        }

        delegate void AfterDelayTextEvent();

        /// <summary>
        /// 绘制小球线程
        /// </summary>
        void DoDrawDesk()
        {
            int StartTickCount = Environment.TickCount;
            double FrameCount = 0; // 总帧数

            while (true)
                try
                {
                    FrameCount++;
                    DeskBmp = new Bitmap(PBDesk.ClientRectangle.Width, PBDesk.ClientRectangle.Height);
                    using (Graphics G = Graphics.FromImage(DeskBmp))
                        G.Clear(Color.Black);

                    foreach (Ball B in Balls)
                    {
                        B.Move();
                        B.Draw();
                    }

                    DrawDelayText();
                    DrawTitle();
                    DrawFPSText(FrameCount, (Environment.TickCount - StartTickCount) / 1000);

                    if (!IsLock)
                        if (Levels[CurrentLevel - 1].IsStart)
                            Levels[CurrentLevel - 1].CalElapseTime();
                        else
                            Levels[CurrentLevel - 1].Start();
                    if (Levels[CurrentLevel - 1].ElapseTime == 0) // 关卡时间到，进入下一关
                        if (CurrentLevel + 1 <= Levels.Count)
                            NewLevel();
                        else
                        {
                            Levels[CurrentLevel - 1].Stop(); // 已经通关
                            DelayTexts.Add(new DelayText("YOU ARE WINNER!", -1, null));
                        }
                    this.Invoke(new SetImage(DoSetImage), new Object[] { DeskBmp });

                    if (!IsLock)
                    {
                        Source += 100 + CurrentLevel * 50;
                        CatchMouse();
                    }
                }
                catch
                {
                    // 忽略程序退出和重新开局的异常
                }
        }

        /// <summary>
        /// 绘制每秒帧数
        /// </summary>
        /// <param name="FrameCount"></param>
        /// <param name="Time"></param>
        void DrawFPSText(double FrameCount, double Time)
        {
            DrawText(String.Format("FPS {0:0.0}", FrameCount / Time), 10, Brushes.White, new PointF(0, TITLE));
        }

        /// <summary>
        /// 绘制文字
        /// </summary>
        /// <param name="S"></param>
        /// <param name="FontSize"></param>
        /// <param name="B"></param>
        /// <param name="Location"></param>
        void DrawText(String S, int FontSize, Brush B, PointF Location)
        {
            using (Graphics G = Graphics.FromImage(DeskBmp))
            {
                SizeF SF = G.MeasureString(S, new Font("宋体", FontSize));

                G.DrawString(S, new Font("宋体", FontSize), B, Location);
            }
        }

        /// <summary>
        /// 绘制文字
        /// </summary>
        /// <param name="S"></param>
        /// <param name="FontSize"></param>
        /// <param name="B"></param>
        void DrawText(String S, int FontSize, Brush B)
        {
            SizeF SF = TextRenderer.MeasureText(S, new Font("宋体", FontSize));

            DrawText(S, FontSize, B, PointF.Add(CENTER_POINT, new SizeF(-SF.Width / 2, -SF.Height / 2)));
        }

        /// <summary>
        /// 绘制标题绘图区
        /// </summary>
        void DrawTitle()
        {
            DrawText(String.Format("得分：{0:000000000000} 时间：{1:000} 关卡：{2:00}/{3:00}",
                 new Object[] { Source, Levels[CurrentLevel - 1].ElapseTime, CurrentLevel, Levels.Count }), 18,
                 Brushes.White, new PointF(5, 5));
        }

        /// <summary>
        /// 捕捉鼠标拨球操作
        /// </summary>
        void CatchMouse()
        {
            if (OldMousePosition.X == -1)
                OldMousePosition = (Point)this.Invoke(new GetPoint(DoGetPoint),
                    new Object[] { Control.MousePosition });
            else
            {
                Point CurrentMousePosition = (Point)this.Invoke(new GetPoint(DoGetPoint),
                    new Object[] { Control.MousePosition });

                foreach (Ball B in Balls)
                {
                    if (B.Bounds.Contains(CurrentMousePosition))
                    {
                        int VX = CurrentMousePosition.X - OldMousePosition.X;
                        int VY = CurrentMousePosition.Y - OldMousePosition.Y;

                        B.VX = VX == 0 ? 0 : VX / Math.Abs(VX) * 2;
                        B.VY = VY == 0 ? 0 : VY / Math.Abs(VY) * 2;
                        B.E += 100000;
                        B.Start(); // 小球重新开始计时

                        Source += 1000 + CurrentLevel * 500;
                    }
                }
                OldMousePosition = CurrentMousePosition;
            }
        }

        delegate void SetImage(Bitmap DeskBmp);
        delegate Point GetPoint(Point P);

        /// <summary>
        /// 显示绘图结果
        /// </summary>
        /// <param name="DeskBmp"></param>
        void DoSetImage(Bitmap DeskBmp)
        {
            PBDesk.Image = DeskBmp;
        }

        /// <summary>
        /// 鼠标屏幕坐标换算成绘图坐标
        /// </summary>
        /// <param name="P"></param>
        /// <returns></returns>
        Point DoGetPoint(Point P)
        {
            return PBDesk.PointToClient(P);
        }

        /// <summary>
        /// 小球类
        /// </summary>
        class Ball
        {
            float X = 0; // 横坐标
            float Y = 0; // 纵坐标
            float R; // 半径
            public double VX = 0; // 横向速度分量
            public double VY = 0; // 纵向速度分量
            public double E = 0; // 动能
            public int StartTickCount = 0; // 开始计时时间戳
            public int TurnColor1TickCount = 0; // 小球变1色时间戳
            public int TurnColor2TickCount = 0; // 小球变2色时间戳
            public int TurnColor3TickCount = 0; // 小球变3色时间戳
            public int GameOverTickCount = 0; // 游戏结束时间戳
            bool IsStart = false;

            public Ball(Point Location, int R)
            {
                this.X = Location.X;
                this.Y = Location.Y;
                this.R = R;
            }

            public void Start()
            {
                this.StartTickCount = Environment.TickCount;

                LevelInfo LI = Levels[CurrentLevel - 1];

                // 将关卡相关信息转化成小球相关信息
                this.TurnColor1TickCount = StartTickCount + LI.TurnColor1Time * 1000;
                this.TurnColor2TickCount = TurnColor1TickCount + LI.TurnColor2Time * 1000;
                this.TurnColor3TickCount = TurnColor2TickCount + LI.TurnColor3Time * 1000;
                this.GameOverTickCount = TurnColor3TickCount + LI.GameOverTime * 1000;

                this.IsStart = true;
            }

            /// <summary>
            /// 小球的区域
            /// </summary>
            public RectangleF Bounds
            {
                get
                {
                    return new RectangleF(X - R, Y - R, R * 2, R * 2);
                }
            }

            /// <summary>
            ///  是否和其它小球碰撞
            /// </summary>
            /// <param name="B"></param>
            /// <param name="XS"></param>
            /// <param name="YS"></param>
            /// <returns></returns>
            bool CheckHint(Ball B, float XS, float YS)
            {
                return Math.Sqrt(XS * XS + YS * YS) <= this.R + B.R; // 用两点间距离公式求出当前距离是否是两球的最短距离
            }

            /// <summary>
            /// 移动小球
            /// </summary>
            public void Move()
            {
                AdjustForHintBoder(); // 是否和墙壁相撞

                // 调整速度分量，作用一个反向摩擦力，并在小于0.1时停止
                if (VX > 0)
                {
                    VX -= F * VX * VX;
                    if (VX < 0.1)
                        VX = 0;
                }
                else if (VX < 0)
                {
                    VX += F * VX * VX;
                    if (VX > -0.1)
                        VX = 0;
                }

                if (VY > 0)
                {
                    VY -= F * VY * VY;
                    if (VY < 0.1)
                        VY = 0;
                }
                else if (VY < 0)
                {
                    VY += F * VY * VY;
                    if (VY > -0.1)
                        VY = 0;
                }

                // 如果动能不足则强行停止
                if (--E < 0)
                {
                    E = 0;
                    VX = 0;
                    VY = 0;
                }

                this.X = (float)(X + VX);
                this.Y = (float)(Y + VY);

                foreach (Ball B in Balls)
                {
                    float XS = this.X - B.X;
                    float YS = this.Y - B.Y;

                    if (B != this && CheckHint(B, XS, YS)) // 和其它小球相撞
                    {
                        double Angle = Math.Atan2(YS, XS); // 两球的夹角
                        double AX = (B.X + Math.Cos(Angle) * (this.R + B.R) - this.X); // 横向加速度
                        double AY = (B.Y + Math.Sin(Angle) * (this.R + B.R) - this.Y); // 纵向加速度

                        //简单模拟，调整速度，直接+/-加速度模拟两球弹性碰撞后弹开
                        B.VX -= AX;
                        B.VY -= AY;
                        this.VX += AX;
                        this.VY += AY;

                        // 限速
                        if (Math.Abs(B.VX) > 4)
                            B.VX = Math.Sign(B.VX) * 4;
                        if (Math.Abs(B.VY) > 4)
                            B.VY = Math.Sign(B.VY) * 4;
                        if (Math.Abs(this.VX) > 4)
                            this.VX = Math.Sign(this.VX) * 4;
                        if (Math.Abs(this.VY) > 4)
                            this.VY = Math.Sign(this.VY) * 4;

                        // 简单模拟，整个系统能量不守恒
                        B.E += this.E / 2; // 目标动能增加
                        this.E -= this.E / 2; // 自己动能减少
                    }
                }
            }

            /// <summary>
            /// 检查小球是否撞到墙
            /// </summary>
            void AdjustForHintBoder()
            {
                if (X + R > WIDTH)
                {
                    VX = -VX;
                    X = WIDTH - R;
                }
                else if (X - R < 0)
                {
                    VX = -VX;
                    X = R;
                }
                if (Y + R > HEIGHT)
                {
                    VY = -VX;
                    Y = HEIGHT - R;
                }
                else if (Y - R < TITLE)
                {
                    VY = -VX;
                    Y = R + TITLE;
                }

            }

            /// <summary>
            /// 绘制小球
            /// </summary>
            public void Draw()
            {
                using (Graphics G = Graphics.FromImage(DeskBmp))
                {
                    G.SmoothingMode = SmoothingMode.HighQuality;

                    Color C = Color.Green;

                    if (Levels[CurrentLevel - 1].IsStart && this.IsStart) // 如果关卡已经开始而且小球也已经开始计时
                    {
                        if (Environment.TickCount > GameOverTickCount) // 是否超过游戏结束时间戳
                        {
                            C = Color.FromArgb(Rnd.Next(255), Rnd.Next(255), Rnd.Next(255));
                            if (!IsLock) // 只需要加一次
                                DelayTexts.Add(new DelayText("GAME OVER!", -1, null)); // 游戏结束
                        }

                        if (!IsLock) // 正常游戏中
                            if (Environment.TickCount > TurnColor3TickCount) // 是否到了该变3色的时间戳
                                C = Color.Red;
                            else if (Environment.TickCount > TurnColor2TickCount) // 是否到了该变2色的时间戳
                                C = Color.Orange;
                            else if (Environment.TickCount > TurnColor1TickCount) // 是否到了该变1色的时间戳
                                C = Color.Yellow;
                    }

                    G.FillEllipse(new SolidBrush(C), Bounds); // 绘制球体

                    Pen P = new Pen(Brushes.White, R / 6);
                    P.StartCap = LineCap.Round;
                    P.EndCap = LineCap.Round;

                    G.DrawArc(P, X - R + 4, Y - R + 4, R * 2 - 8, R * 2 - 8, -100, -70); // 绘制高光
                }
            }
        }

        /// <summary>
        /// 开始游戏菜单项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TmiStart_Click(object sender, EventArgs e)
        {
            Levels = new List<LevelInfo>(new LevelInfo[] // 定义关卡
            {
                new LevelInfo(20,30,10,4,4,4,4),
                new LevelInfo(21,30,12,4,3,3,3),
                new LevelInfo(22,30,13,4,3,3,3),
                new LevelInfo(23,28,13,3,3,3,3),
                new LevelInfo(25,27,13,3,3,3,3),
                new LevelInfo(30,26,14,3,3,2,2),
                new LevelInfo(35,25,14,3,3,2,2),
                new LevelInfo(40,24,14,3,2,2,2),
                new LevelInfo(45,23,15,2,2,2,2),
                new LevelInfo(50,22,15,2,2,2,2),
                new LevelInfo(55,20,15,2,2,2,2),
                new LevelInfo(60,18,16,2,2,2,1),
            });

            Source = 0;
            CurrentLevel = 0;
            NewLevel();

            if (MainThread == null)
            {
                MainThread = new Thread(new ThreadStart(DoDrawDesk)); // 开始游戏
                MainThread.IsBackground = true;
                MainThread.Start();
            }
        }

        /// <summary>
        /// 开始新关
        /// </summary>
        void NewLevel()
        {
            IsLock = false;
            CurrentLevel++;

            // 生成小球集合
            Balls.Clear();
            for (int i = 0; i < Levels[CurrentLevel - 1].Number; i++)
            {
                int BallR = Rnd.Next(Levels[CurrentLevel - 1].MinR, MAXR);

                Balls.Add(new Ball(new Point(Rnd.Next(WIDTH), Rnd.Next(HEIGHT) + TITLE), BallR));
            }

            // 定义滞留文字操作类集合
            DelayTexts.Clear();
            DelayTexts.Add(new DelayText("READY", Environment.TickCount + 2000, null)); // 先显示2秒READY
            DelayTexts.Add(new DelayText("GO", Environment.TickCount + 4000, new AfterDelayTextEvent(StartAllBalls))); // 再显示2秒GO
            // 最后游戏正式开始
        }

        /// <summary>
        /// 所有小球开始计时
        /// </summary>
        void StartAllBalls()
        {
            foreach (Ball B in Balls)
                B.Start();
        }

        void TmiQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        void TmiAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("欢迎使用变色球游戏！");
        }
    }
}