using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;

namespace NFL_Head_Coach
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    enum GameLogic
    {
        Start = 0,
        Pause = 1,
        Resume = 2,
        End = 3
    }
    enum GameMode
    {
        Mode5v5 = 0,
        Mode7v7 = 1,
        Mode11v11 = 2
    }
    enum ControlStatus
    {
        isMoving = 0,
        isThrowing = 1,
        isReceiving = 2
    }

    public partial class MainWindow : Window
    {
        //   Multi-touch NFL Head Coach 
        //   *                  *
        //   *  A   screen  B   *
        //   *                  *
       

        #region Property
        

        // System  
        GameLogic gameLogic = GameLogic.Start;
        GameMode gameMode = GameMode.Mode5v5;
        
        // Multi Thread
        Thread t;
        Thread tt; // Tatics thread
        int frame = 0;

        // Team
        // Test Data
        Team teamACopy = new Team();
        Team teamBCopy = new Team();

        // Two teams
        Team teamA;
        Team teamB;
        List<Player> allPlayers = new List<Player>();
        
        // Yard
        int yard = 0;
        // Touch Data
        Point touchA1 = new Point(0,0);
        Point touchA2 = new Point(0,0);
        Point touchB1 = new Point(0,0);
        Point touchB2 = new Point(0,0);
        bool isTouch = false;
        bool isInk = false;

        Rect bounds = new Rect(220, 173, 818,414);
        //Define 4 rectangles as global variables here with four variables
        // A1: TeamA Navigation
        // A2: TeamA Behavior
        // B1: TeamB Navigation
        // B2: TeamB Behavior
        Rect rectA1 = new Rect(20,120,160,160);
        Rect rectA2 = new Rect(20,300,160,160);
        Rect rectB1 = new Rect(1100,120,160,160);
        Rect rectB2 = new Rect(1100,300,160,160);
        // Rect
        Rect WR1 = new Rect(34, 513, 64, 40);
        Rect WR2 = new Rect(34, 605, 64, 40);
        Rect QB = new Rect(70, 559, 64, 40);
        Rect TG1 = new Rect(104, 513, 64, 40);
        Rect TG2 = new Rect(104, 605, 64, 40);

        Rect MB1 = new Rect(1116, 513, 64, 40);
        Rect MB2 = new Rect(1116, 605, 64, 40);
        Rect SF = new Rect(1152, 559, 64, 40);
        Rect CB1 = new Rect(1186, 513, 64, 40);
        Rect CB2 = new Rect(1186, 605, 64, 40);


        //F1: TeamA Field
        //F2: TeamB Field
        Rect rectF1 = new Rect(0, 0, 600, 760);
        Rect rectF2 = new Rect(680, 0, 600, 760);

        Player currentPlayerTeamA;
        Player currentPlayerTeamB;
        List<Label> playerALabel = new List<Label>();
        List<Label> playerBLabel = new List<Label>();
        Ellipse sellectedA;
        Ellipse sellectedB;
        Point touchDownPos, originalShapePos;

        //for strokes pathes
        int counter = 0;
        List<Shape> strokesA = new List<Shape>();
        List<Shape> strokesACopy = new List<Shape>();
        List<Shape> strokesB = new List<Shape>();
        List<Shape> strokesBCopy = new List<Shape>();

        // Throwing
        Point StartPoint;
        
        Arrow arrow;
        bool ArrowExist;

       
        ControlStatus useControl = ControlStatus.isMoving;


        // Path
        Path path = new Path();
        bool hideTacticsA = false;
        bool hideTacticsB = false;
        #endregion

        #region Init and Unit

        public MainWindow()
        {
            InitializeComponent();
        }

        // Initialization and Unitialization
        public void Window_Loaded(object sender, EventArgs e)
        {
            // Test
            sellectedA = new Ellipse();
            sellectedA.Width = 38;
            sellectedA.Height = 38;
            sellectedA.Fill = System.Windows.Media.Brushes.Orange;
            canvas.Children.Add(sellectedA);
            Canvas.SetZIndex(sellectedA, 0);

            sellectedB = new Ellipse();
            sellectedB.Width = 38;
            sellectedB.Height = 38;
            sellectedB.Fill = System.Windows.Media.Brushes.Red;
            canvas.Children.Add(sellectedB);
            Canvas.SetZIndex(sellectedB, 0);


            // Initialize the dialog box
            dialog.TextWrapping = TextWrapping.Wrap;
            dialog.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            
            // Set the color of grass
            SolidColorBrush grass_brush = new SolidColorBrush();
            grass_brush.Color = Color.FromArgb(255,43,157,17);
            grass_color.Fill = grass_brush;

            // Full screen mode
            SetFullScreen();

            // When the game start, there is no arrow 
            ArrowExist = false;

            // PlayerImage for team A
            BitmapImage bmp_A = new BitmapImage();
            bmp_A.BeginInit();
            bmp_A.UriSource = new Uri(@"Images\h1.png");
            bmp_A.EndInit();
            Image img_A = new Image();
            img_A.Height = 32;
            img_A.Width = 32;
            img_A.Source = bmp_A;

            // PlayerImage for team B
            BitmapImage bmp_B = new BitmapImage();
            bmp_B.BeginInit();
            bmp_B.UriSource = new Uri(@"Images\h2.png");
            bmp_B.EndInit();
            Image img_B = new Image();
            img_B.Height = 32;
            img_B.Width = 32;
            img_B.Source = bmp_B;

            // Teams
            teamA = new Team();
            teamB = new Team();
            teamA.Init(Team.Behavior.Attack, gameMode, img_A);
            teamB.Init(Team.Behavior.Defense, gameMode, img_B);
            // Display Team Player on the pitch
            for (int i = 0; i < 5; i++)
            {
                canvas.Children.Add(teamA.Member[i].PlayerImage);
                canvas.Children.Add(teamB.Member[i].PlayerImage);
            }
            // Set Position for each team
            InitGameAt(640);
           
            for (int i = 0; i < teamA.Member.Count; i++) {
                allPlayers.Add(teamA.Member[i]);
            }
            for (int i = teamA.Member.Count; i < (teamA.Member.Count + teamB.Member.Count); i++) {
                allPlayers.Add(teamB.Member[i - teamA.Member.Count]);
            }
            // Set the position for the Number for each player in one team
            
            for (int i = 0; i < 5; i++)
            {
                // Team A
                Label num = new Label();
                num.Height = 28;
                num.Width = 36;
                num.FontSize = 12;
                num.FontWeight = FontWeights.Bold;
                num.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                num.Background = System.Windows.Media.Brushes.Transparent;
                num.Foreground = System.Windows.Media.Brushes.Black;
                playerALabel.Add(num);
                // Team B
                Label num2 = new Label();
                num2.Height = 28;
                num2.Width = 36;
                num2.FontSize = 12;
                num2.FontWeight = FontWeights.Bold;
                num2.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                num2.Background = System.Windows.Media.Brushes.Transparent;
                num2.Foreground = System.Windows.Media.Brushes.Black;
                playerBLabel.Add(num2);
            }
            // Team A
            playerALabel[0].Content = "QB";
            playerALabel[1].Content = "WR1";
            playerALabel[2].Content = "WR2";
            playerALabel[3].Content = "TG1";
            playerALabel[4].Content = "TG2";
            // Team B
            playerBLabel[0].Content = "SF";
            playerBLabel[1].Content = "MB1";
            playerBLabel[2].Content = "MB2";
            playerBLabel[3].Content = "CB1";
            playerBLabel[4].Content = "CB2";
            // Add all the labels into the canvas
            for (int i = 0; i < 5; i++)
            {
                canvas.Children.Add(playerALabel[i]);
                canvas.Children.Add(playerBLabel[i]);
            }
            // Selection By default
            currentPlayerTeamA = teamA.Member[0];
            currentPlayerTeamB = teamB.Member[0]; 
            // Create a thread to update all the position data for the game
            t = new Thread(new ThreadStart(ThreadProc));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }
        public void Window_Closed(object sender, EventArgs e)
        {
            t.Abort();
        }

        #endregion

        #region Main Thread and Tactics Thread


        // Main Thread
        delegate void NextPrimeDelegate();
        void ThreadProc()
        {
            while (true)
            {
                frame++;
                canvas.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new NextPrimeDelegate(Update));
                Thread.Sleep(50);
            }
        }
        // Update function is called for each frame
        void Update()
        {
            // Testing
            //dialog.Text += "pos = " + touchA1.X.ToString() + " " + touchA1.Y.ToString() + "\n";
            
            //Team A
            // Move the player according to the path
            for (int i=0; i<5;i++)
            {
                if (teamA.Member[i].Role == Player.PlayerRole.TackleGurad)
                {
                    Player cover = GetCatch(teamA.Member[i].PlayerImage, teamB.Member);
                    Point pos = Get(teamA.Member[i].PlayerImage);
                    if (cover != null)
                    {
                        cover.Path = new Path();
                        teamA.Member[i].Path = new Path();
                    }
                    else
                    {
                        MoveWithPath(teamA.Member[i].Path, teamA.Member[i].PlayerImage);
                        MoveWithPath(teamB.Member[i].Path, teamB.Member[i].PlayerImage);
                    }
                }
                else
                {
                    MoveWithPath(teamA.Member[i].Path, teamA.Member[i].PlayerImage);
                    MoveWithPath(teamB.Member[i].Path, teamB.Member[i].PlayerImage);
                }
            }
            currentPlayerTeamA.Path = new Path();
            currentPlayerTeamB.Path = new Path();

            bool Getblock = false;
            if (GetCatch(FIndTheHolder(teamA).PlayerImage, teamB.Member) != null) {
                Getblock = true;
                dialog.Text += "Successful defense\n";
                ScoreBar.Content = "LOSE : WIN";
            }
            if (FIndTheHolder(teamA) != null) {
                if (Get(FIndTheHolder(teamA).PlayerImage).X > 970) {
                    dialog.Text += "Touch Down\n";
                    ScoreBar.Content = "WIN : LOSE";
                }
            }
          
            // Team B 
            // Defend 
            if (!Getblock) {
                if (touchB1 != new Point(0, 0)) {
                    Point offset = new Point(0, 0);

                    Player ballHolder = FIndTheHolder(teamA);

                    Player catcher = new Player();
                    if (ballHolder != null) {
                        for (int i = 0; i < teamB.Member.Count; i++) {
                            catcher = GetCatch(ballHolder.PlayerImage, teamB.Member);
                        }
                    }

                    Point dir = new Point((touchB1.X - Centeriod(rectB1).X) / 20, (touchB1.Y - Centeriod(rectB1).Y) / 20);
                    // Get the start point of the quarterback on the canvas
                    Point sp = Get(currentPlayerTeamB.PlayerImage);
                    // Set the position after moving a step
                    Point ep = new Point(sp.X + dir.X, sp.Y + dir.Y);

                    if (catcher != null) {
                        Move(currentPlayerTeamB.PlayerImage, ep);
                        Move(ballHolder.PlayerImage, ep);
                        Move(ball, ep);
                    } else {
                        Move(currentPlayerTeamB.PlayerImage, ep);
                    }
                }
            }
           
            // Team A
            // Attack
            if (!Getblock) {
                if (useControl == ControlStatus.isMoving) {
                    if (ArrowExist) {
                        canvas.Children.Remove(arrow.ArrowImage);
                        ArrowExist = false;
                    }

                    // Get the direction for navigation
                    Point dir = new Point(0, 0);
                    if (touchA1 != new Point(0, 0)) {
                        dir = new Point((touchA1.X - Centeriod(rectA1).X) / 20, (touchA1.Y - Centeriod(rectA1).Y) / 20);
                    }

                    // Get the start point of the quarterback on the canvas
                    Point sp = Get(currentPlayerTeamA.PlayerImage);
                    // Set the position after moving a step
                    Point ep = new Point(sp.X + dir.X, sp.Y + dir.Y);
                    Player ballHolder = FIndTheHolder(teamA);
                    Move(currentPlayerTeamA.PlayerImage, ep);
                    if (currentPlayerTeamA == ballHolder) {
                        Move(ball, ep);
                    }
                } else if (useControl == ControlStatus.isThrowing) {
                    if (!ArrowExist) {
                        arrow = new Arrow();
                        canvas.Children.Add(arrow.ArrowImage);
                        ArrowExist = true;
                    }
                    Matrix arrowMatrix = new Matrix();

                    StartPoint = Get(teamA.Member[0].PlayerImage);
                    //StartPoint.X += arrow.ArrowImage.Width / 2;

                    // Scale
                    double Strength = 5;
                    double maxDist = 3;

                    double scale = Dist2(touchA2, Centeriod(rectA2));

                    if (scale / arrow.ArrowImage.Width * 2 < maxDist) {
                        arrowMatrix.ScaleAt(scale / arrow.ArrowImage.Width * 2, 1, 0, 0);
                    } else {
                        scale = arrow.ArrowImage.Width / 2 * maxDist;
                        arrowMatrix.ScaleAt(maxDist, 1, 0, 0);
                    }

                    // Rotation
                    Point V = new Point(Centeriod(rectA2).X - touchA2.X, Centeriod(rectA2).Y - touchA2.Y);
                    double angle = Rad2Deg(IntersectAngle(V, new Point(0, 1))) - 90;

                    arrowMatrix.RotateAt(angle, 0, arrow.ArrowImage.Height / 2);
                    Move(arrow.ArrowImage, new Point(StartPoint.X + arrow.ArrowImage.Width / 2, StartPoint.Y));

                    // Calculate translate, scale and rotate matrix for the ball
                    Matrix ballMatrix = new Matrix();
                    Move(ball, new Point(StartPoint.X, StartPoint.Y));
                    ballMatrix.RotateAt(angle, ball.Width / 2, ball.Height / 2);

                    arrow.ArrowImage.RenderTransform = new MatrixTransform(arrowMatrix);
                    ball.RenderTransform = new MatrixTransform(ballMatrix);

                    StylusPoint x1, x2;
                    x1 = new StylusPoint(StartPoint.X, StartPoint.Y);
                    x2 = new StylusPoint(x1.X + Strength * scale * Math.Cos(Deg2Rad(angle)), x1.Y + Strength * scale * Math.Sin(Deg2Rad(angle)));
                    StylusPointCollection sp = new StylusPointCollection();
                    sp.Add(x1);
                    sp.Add(x2);
                    Stroke s = new Stroke(sp);
                    Preprocessing.Resampling(s, 10);

                    path = new Path(s);
                } else if (useControl == ControlStatus.isReceiving) {
                    if (ArrowExist) {
                        canvas.Children.Remove(arrow.ArrowImage);
                        ArrowExist = false;
                    }
                    Point p_ball = new Point();
                    Point p_wr = new Point();

                    // If the wr receive the ball, then the ball will have the same position as the player
                    Player ballHolder = new Player();
                    ballHolder = GetCatch(ball, teamA.WR);
                    if (ballHolder != null) {
                        // 
                        Point sp = new Point();
                        //Point dir = new Point((TouchPointA.X - 640) / 50, (TouchPointA.Y - 400) / 50);

                        // Get the direction for navigation
                        Point dir = new Point(0, 0);
                        if (touchA1 != new Point(0, 0)) {
                            dir = new Point((touchA1.X - Centeriod(rectA1).X) / 20, (touchA1.Y - Centeriod(rectA1).Y) / 20);
                        }

                        // Get the start point of the quarterback on the canvas
                        sp = Get(ballHolder.PlayerImage);
                        // Set the position after moving a step
                        Point ep = new Point();
                        ep.X = sp.X + dir.X;
                        ep.Y = sp.Y + dir.Y;
                        // Move ball and player
                        currentPlayerTeamA = ballHolder;
                        Move(ballHolder.PlayerImage, ep);
                        Move(ball, ep);
                    } else // not catch the ball, then the ball will follow the path
                {
                        // Player movement
                        Point sp = new Point();
                        //Point dir = new Point((TouchPointA.X - 640) / 50, (TouchPointA.Y - 400) / 50);

                        // Get the direction for navigation
                        Point dir = new Point(0, 0);
                        if (touchA1 != new Point(0, 0)) {
                            dir = new Point((touchA1.X - Centeriod(rectA1).X) / 20, (touchA1.Y - Centeriod(rectA1).Y) / 20);
                        }
                        // Get the start point of the quarterback on the canvas
                        sp = Get(currentPlayerTeamA.PlayerImage);
                        // Set the position after moving a step
                        Point ep = new Point();
                        ep.X = sp.X + dir.X;
                        ep.Y = sp.Y + dir.Y;
                        Move(currentPlayerTeamA.PlayerImage, ep);

                        // ball movement
                        MoveWithPath(path, ball);
                    }
                }
            }
            Canvas.SetLeft(sellectedA, Get(currentPlayerTeamA.PlayerImage).X - sellectedA.Width / 2);
            Canvas.SetTop(sellectedA, Get(currentPlayerTeamA.PlayerImage).Y - sellectedA.Height / 2);
            Canvas.SetLeft(sellectedB, Get(currentPlayerTeamB.PlayerImage).X - sellectedB.Width / 2);
            Canvas.SetTop(sellectedB, Get(currentPlayerTeamB.PlayerImage).Y - sellectedB.Height / 2);
            // Move the label to each player
            for (int i = 0; i < 5; i++)
            {
                Move(playerALabel[i], Get(teamA.Member[i].PlayerImage));
                Move(playerBLabel[i], Get(teamB.Member[i].PlayerImage));
            }
        }
        

        // Tatics Thread
        private void TaticsProc()
        {
            while (true) {
                canvas.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new CanvasDelegate(CanvasUpdate));
                inkcanvas.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new InkCanvasDelegate(InkCanvasUpdate));
                Thread.Sleep(50);
            }
        }
        private delegate void CanvasDelegate();
        private delegate void InkCanvasDelegate();
        // Update the players image in the canvas when the game pause
        private void CanvasUpdate()
        {
            for (int i = 0; i < 5; i++) {
                Move(teamA.Member[i].PlayerImage, teamA.Member[i].Position);
            }
            for (int i = 0; i < 5; i++) {
                Move(teamB.Member[i].PlayerImage, teamB.Member[i].Position);
            } 
             // Move the label to each player
            for (int i = 0; i < 5; i++)
            {
                Move(playerALabel[i], Get(teamA.Member[i].PlayerImage));
                Move(playerBLabel[i], Get(teamB.Member[i].PlayerImage));
            }
        }
        // Update the players image in the inkcanvas when the game pause   
        private void InkCanvasUpdate()
        {   
            //Make sure there at least one path is drawn and the team has at least one memeber
            if (allPlayers.Count > 0 && inkcanvas.Strokes.Count > 0) {
                //Make sure that no more strokes will be drawn (if they equal the number of players)
                if (inkcanvas.Strokes.Count == allPlayers.Count) {
                    //No more strokes should be drawn
                    inkcanvas.EditingMode = InkCanvasEditingMode.None;
                }
                //Save the stroke that is drawn for the team member
                for (int i = 0; i < allPlayers.Count; i++) {
                    //Make sure that only one path will be drawn per player
                    if (allPlayers[i].Path.CurrentStatus == Path.Status.Start) {
                            counter++;
                    } else if (inkcanvas.Strokes.Count > 0) {
                        //Make sure that the distance between the first point in the stroke and the player is less than a 40 threshold
                        double dist = Distance(allPlayers[i].Position, inkcanvas.Strokes[(inkcanvas.Strokes.Count - 1)].StylusPoints[0].ToPoint());
                        //Make sure that the path that is drawn is more than 70
                        double pathLength = PathLength(inkcanvas.Strokes[(inkcanvas.Strokes.Count - 1)].StylusPoints);
                        //If the above conditions passed then create the path
                        if (pathLength > 69 && allPlayers[i].Path.CurrentStatus == Path.Status.Null && dist < 41) {
                            allPlayers[i].Path = new Path(inkcanvas.Strokes[(inkcanvas.Strokes.Count - 1)]);
                        } else {
                            counter++;
                        }
                    }
                }
                //Delete the stroke if it is failed the above conditions
                if (counter == allPlayers.Count) {
                    inkcanvas.Strokes.RemoveAt(inkcanvas.Strokes.Count - 1);
                    counter = 0;
                } else {
                    counter = 0;
                    for (int i = 0; i < allPlayers.Count; i++) {
                        //Make sure that only one path will be drawn per player
                        if (allPlayers[i].Path.CurrentStatus == Path.Status.Start) {
                            if (i>=0 && i<teamA.Member.Count)
                            {
                                teamA.Member.RemoveAt(i);
                                teamA.Member.Insert(i,allPlayers[i]);
                                //Convert it to shape
                                if (!hideTacticsA) {
                                    Shape shapeToAdd = convertStrokToShape(teamA.Member[i].Path.Stroke, 1);
                                    strokesA.Add(shapeToAdd);
                                } 
                                else 
                                {
                                    if (strokesA.Count != 0) 
                                    {
                                        foreach (Shape shape in strokesA) 
                                        {
                                            canvas.Children.Remove(shape);
                                        }
                                        strokesA.Clear();
                                    }
                                }
                            } else {
                                teamB.Member.RemoveAt(i - teamA.Member.Count);
                                teamB.Member.Insert(i - teamA.Member.Count, allPlayers[i]);
                                //Convert it to shape
                                if (!hideTacticsB) 
                                {
                                    Shape shapeToAdd = convertStrokToShape(teamB.Member[i - teamA.Member.Count].Path.Stroke, 2);
                                    strokesB.Add(shapeToAdd);
                                } 
                                else 
                                {
                                    if (strokesB.Count != 0)
                                    {
                                        foreach (Shape shape in strokesB)
                                        {
                                            canvas.Children.Remove(shape);
                                        }
                                        strokesB.Clear();
                                    }
                                }
                            } 
                        }
                    }
                }
            }
            // Ball
            Move(ball, Get(teamA.Member[0].PlayerImage));
        }

        #endregion

        #region Touch

        //List to to save the current time in seconds and it is used to know the difference in sec between one finger consecutive touches
        private List<int> currentTimeSec = new List<int>();
        //List to to save the current time in minutes
        private List<int> currentTimeMin = new List<int>();

        // Touch Data
        protected override void OnTouchDown(TouchEventArgs e)
        {
            base.OnTouchDown(e);
            Console.WriteLine(e.Device.Target.GetType().Name);

            inkcanvas.EditingMode = InkCanvasEditingMode.None;

            //Capture this touch device
            this.CaptureTouch(e.TouchDevice);
            TouchPoint touchPoint = e.GetTouchPoint(this);
            Point point = touchPoint.Position;
            Point p = new Point(point.X, point.Y);
            if (QB.Contains(p)) { currentPlayerTeamA = teamA.Member[0]; }
            if (WR1.Contains(p)) { currentPlayerTeamA = teamA.Member[1]; }
            if (WR2.Contains(p)) { currentPlayerTeamA = teamA.Member[2]; }
            if (TG1.Contains(p)) { currentPlayerTeamA = teamA.Member[3]; }
            if (TG2.Contains(p)) { currentPlayerTeamA = teamA.Member[4]; }
            if (SF.Contains(p)) { currentPlayerTeamB = teamB.Member[0]; }
            if (MB1.Contains(p)) { currentPlayerTeamB = teamB.Member[1]; }
            if (MB2.Contains(p)) { currentPlayerTeamB = teamB.Member[2]; }
            if (CB1.Contains(p)) { currentPlayerTeamB = teamB.Member[3]; }
            if (CB2.Contains(p)) { currentPlayerTeamB = teamB.Member[4]; }

            //do what ever want to do here
            if (rectA1.Contains(p))
            {
                // Player A navigation
                touchA1 = point;
                if ( useControl != ControlStatus.isReceiving)
                {
                    useControl = ControlStatus.isMoving;
                }
            }
            else if (rectA2.Contains(p))
            {
                //Player A behavior
                touchA2 = point;
                if (CheckDoubleClick()) {
                    useControl = ControlStatus.isReceiving;
                } 
                else if (useControl != ControlStatus.isReceiving)
                {
                    useControl = ControlStatus.isThrowing;
                }
              
            }
            else if (rectB1.Contains(p))
            {
                //third rectangle
                touchB1 = point;
            }
            else if (rectB2.Contains(p))
            {
                //Forth rectangle
                touchB2 = point;
            } else if (isTouch && rectF1.Contains(p))
            {
                for (int i = 0; i < teamA.Member.Count; i++)
                {
                    double distance = Distance(teamA.Member[i].Position, point);
                    if (distance < 25)
                    {
                        //disaple the ink
                        inkcanvas.EditingMode = InkCanvasEditingMode.None;
                        double originalX = teamA.Member[i].Position.X;
                        double originalY = teamA.Member[i].Position.Y;
                        originalShapePos = new Point(originalX, originalY);
                        touchDownPos = p;
                    }
                }
            } else if (isTouch && rectF2.Contains(p)) {
                for (int i = 0; i < teamB.Member.Count; i++) {
                    double distance = Distance(teamB.Member[i].Position, point);
                    if (distance < 25) {
                        //disaple the ink
                        inkcanvas.EditingMode = InkCanvasEditingMode.None;
                        double originalX = teamB.Member[i].Position.X;
                        double originalY = teamB.Member[i].Position.Y;
                        originalShapePos = new Point(originalX, originalY);
                        touchDownPos = p;
                    }
                }
            }
        }
        protected override void OnTouchMove(TouchEventArgs e)
        {
            base.OnTouchMove(e);
            TouchPoint touchPoint = e.GetTouchPoint(this);
            Point point = touchPoint.Position;
            Point p = new Point(point.X, point.Y);

            //e.TouchDevice.Id == 8
            if (QB.Contains(p)) { currentPlayerTeamA = teamA.Member[0]; }
            if (WR1.Contains(p)) { currentPlayerTeamA = teamA.Member[1]; }
            if (WR2.Contains(p)) { currentPlayerTeamA = teamA.Member[2]; }
            if (TG1.Contains(p)) { currentPlayerTeamA = teamA.Member[3]; }
            if (TG2.Contains(p)) { currentPlayerTeamA = teamA.Member[4]; }
            if (SF.Contains(p)) { currentPlayerTeamB = teamB.Member[0]; }
            if (MB1.Contains(p)) { currentPlayerTeamB = teamB.Member[1]; }
            if (MB2.Contains(p)) { currentPlayerTeamB = teamB.Member[2]; }
            if (CB1.Contains(p)) { currentPlayerTeamB = teamB.Member[3]; }
            if (CB2.Contains(p)) { currentPlayerTeamB = teamB.Member[4]; }

            //do what ever want to do here
            if (rectA1.Contains(p))
            {
                //First rectangle
                touchA1 = point;
            }
            else if (rectA2.Contains(p))
            {
                //Second rectangle
                touchA2 = point;
            }
            else if (rectB1.Contains(p))
            {
                //Third rectangle
                touchB1 = point;
            }
            else if (rectB2.Contains(p))
            {
                //Forth rectangle
                touchB2 = point;
            } else if (isTouch && rectF1.Contains(p))
            {
                for (int i = 0; i < teamA.Member.Count; i++)
                {
                    double distance = Distance(teamA.Member[i].Position, point);
                    if (distance < 25 && teamA.Member[i].Path.CurrentStatus == Path.Status.Null)
                    {
                        //Move the player
                        Point currentPos = p;
                        double offsetX = currentPos.X - touchDownPos.X;
                        double offsetY = currentPos.Y - touchDownPos.Y;
                        //Canvas.SetLeft(teamA.Member[i].PlayerImage, originalShapePos.X + offsetX);
                        teamA.Member[i].Position.X = originalShapePos.X + offsetX;
                        //Canvas.SetTop(teamA.Member[i].PlayerImage, originalShapePos.Y + offsetY);
                        teamA.Member[i].Position.Y = originalShapePos.Y + offsetY;
                        Move(teamA.Member[i].PlayerImage, teamA.Member[i].Position);
                    }
                }
            } else if (isTouch && rectF2.Contains(p)) {
                for (int i = 0; i < teamB.Member.Count; i++) {
                    double distance = Distance(teamB.Member[i].Position, point);
                    if (distance < 25 && teamB.Member[i].Path.CurrentStatus == Path.Status.Null) {
                        //Move the player
                        Point currentPos = p;
                        double offsetX = currentPos.X - touchDownPos.X;
                        double offsetY = currentPos.Y - touchDownPos.Y;
                        //Canvas.SetLeft(teamB.Member[i].PlayerImage, originalShapePos.X + offsetX);
                        teamB.Member[i].Position.X = originalShapePos.X + offsetX;
                        //Canvas.SetTop(teamB.Member[i].PlayerImage, originalShapePos.Y + offsetY);
                        teamB.Member[i].Position.Y = originalShapePos.Y + offsetY;
                        Move(teamB.Member[i].PlayerImage, teamB.Member[i].Position);
                    }
                }
            }
        }
        protected override void OnTouchUp(TouchEventArgs e)
        {
            base.OnTouchUp(e);
            this.CaptureTouch(e.TouchDevice);
            TouchPoint touchPoint = e.GetTouchPoint(this);
            Point point = touchPoint.Position;
            Point p = new Point(point.X, point.Y);

            // Release capture
            if (rectA1.Contains(p))
            {
                //First rectangle
                touchA1 = new Point(0, 0);
            }
            else if (rectA2.Contains(p))
            {
                //Second rectangle
                touchA2 = new Point(0, 0);
                useControl = ControlStatus.isReceiving;
            }
            else if (rectB1.Contains(p))
            {
                //Third rectangle
                touchB1 = new Point(0, 0);
            }
            else if (rectB2.Contains(p))
            {
                //Forth rectangle
                touchB2 = new Point(0, 0);
            }
            canvas.ReleaseTouchCapture(e.TouchDevice);
            //Make it Ink mode again
            if (isInk)
            {
                inkcanvas.EditingMode = InkCanvasEditingMode.Ink;
            }
        }
        private void myMouseMove(object sender,MouseEventArgs e)
        {
            Point pos = e.GetPosition(this);
            dialog.Text += pos.X.ToString() + " " + pos.Y.ToString()+"\n";
        }

        #endregion

        #region Button
        
        
        //Select a player (Team A)
        private void player1TeamA_Click(object sender, RoutedEventArgs e)
        {
            currentPlayerTeamA = teamA.Member[1];
        }
        private void player2TeamA_Click(object sender, RoutedEventArgs e)
        {
            currentPlayerTeamA = teamA.Member[3];
        }
        private void player3TeamA_Click(object sender, RoutedEventArgs e)
        {
            currentPlayerTeamA = teamA.Member[2];
        }
        private void player4TeamA_Click(object sender, RoutedEventArgs e)
        {
            currentPlayerTeamA = teamA.Member[4];
        }
        private void player5TeamA_Click(object sender, RoutedEventArgs e)
        {
            currentPlayerTeamA = teamA.Member[0];
        }
        //Select a player (Team B)
        private void player1TeamB_Click(object sender, RoutedEventArgs e)
        {
            currentPlayerTeamB = teamB.Member[1];
        }
        private void player2TeamB_Click(object sender, RoutedEventArgs e)
        {
            currentPlayerTeamB = teamB.Member[3];
        }
        private void player3TeamB_Click(object sender, RoutedEventArgs e)
        {
            currentPlayerTeamB = teamB.Member[2];
        }
        private void player4TeamB_Click(object sender, RoutedEventArgs e)
        {
            currentPlayerTeamB = teamB.Member[4];
        }
        private void player5TeamB_Click(object sender, RoutedEventArgs e)
        {
            currentPlayerTeamB = teamB.Member[0];
        }


        // Load,Save,Hide and Delete Tactics Team 1
        private void delete1_Click(object sender, RoutedEventArgs e)
        {
            if (gameLogic == GameLogic.Pause) {
                if (teamA.Member.Count > 0) {
                    for (int i = 0; i < teamA.Member.Count; i++) {
                        teamA.Member[i].Path = new Path();
                    }
                    teamACopy = new Team();
                    //Clear the canvas from strokes that are drawn
                    inkcanvas.Strokes.Clear();
                    if (strokesA.Count != 0) {
                        foreach (Shape shape in strokesA) {
                            canvas.Children.Remove(shape);
                        }
                        strokesA.Clear();
                    }
                    if (strokesACopy.Count != 0) {
                        foreach (Shape shape in strokesACopy) {
                            canvas.Children.Remove(shape);
                        }
                        strokesACopy.Clear();
                    }
                }
                dialog.Text = "Deleted";
            }
        }
        private void save1_Click(object sender, RoutedEventArgs e)
        {
            if (gameLogic == GameLogic.Pause) {
                bool isSaved = false;
                for (int i = 0; i < teamA.Member.Count; i++) {
                    if (teamA.Member[i].Path.CurrentStatus == Path.Status.Start) {
                        isSaved = true;
                    }
                }
                if (isSaved) {
                    teamACopy = teamA;
                    dialog.Text = "Saved";
                } else {
                    dialog.Text = "Draw Tactics First";
                }
            }
        }
        private void load1_Click(object sender, RoutedEventArgs e)
        {
            if (gameLogic == GameLogic.Pause) {
                bool isLoaded = false;
                if (teamACopy.Member.Count > 0) {
                    for (int i = 0; i < teamACopy.Member.Count; i++) {
                        if (teamACopy.Member[i].Path.CurrentStatus == Path.Status.Start) {
                            isLoaded = true;
                            Shape shapeToAdd = convertStrokToShape(teamACopy.Member[i].Path.Stroke,1);
                            strokesACopy.Add(shapeToAdd);
                        }
                    }
                }
                if (isLoaded) {
                    teamA = teamACopy;
                    dialog.Text = "Loaded";
                } else {
                    dialog.Text = "No Tactics Are Saved";
                }
            }
        }
        private void hide1_Click(object sender, RoutedEventArgs e)
        {
            hideTacticsA = !hideTacticsA;
            //Clear the strokes but keep the path saved
            
            if (strokesA.Count != 0)
            {
                foreach (Shape shape in strokesA)
                {
                    canvas.Children.Remove(shape);
                }
                strokesA.Clear();
            }
        }
        // Load,Save,Hide and Delete Tactics Team 2
        private void delete2_Click(object sender, RoutedEventArgs e)
        {
            if (gameLogic == GameLogic.Pause) {
                if (teamB.Member.Count > 0) {
                    for (int i = 0; i < teamB.Member.Count; i++) {
                        teamB.Member[i].Path = new Path();
                    }
                    teamBCopy = new Team();
                    //Clear the canvas from strokes that are drawn
                    inkcanvas.Strokes.Clear();
                    if (strokesB.Count != 0) {
                        foreach (Shape shape in strokesB) {
                            canvas.Children.Remove(shape);
                        }
                        strokesB.Clear();
                    }
                    if (strokesBCopy.Count != 0) {
                        foreach (Shape shape in strokesBCopy) {
                            canvas.Children.Remove(shape);
                        }
                        strokesBCopy.Clear();
                    }
                }
                dialog.Text = "Deleted";
            }
        }
        private void save2_Click(object sender, RoutedEventArgs e)
        {
            if (gameLogic == GameLogic.Pause) {
                bool isSaved = false;
                for (int i = 0; i < teamB.Member.Count; i++) {
                    if (teamB.Member[i].Path.CurrentStatus == Path.Status.Start) {
                        isSaved = true;
                    }
                }
                if (isSaved) {
                    teamBCopy = teamB;
                    dialog.Text = "Saved";
                } else {
                    dialog.Text = "Draw Tactics First";
                }
            }
        }
        private void load2_Click(object sender, RoutedEventArgs e)
        {
            if (gameLogic == GameLogic.Pause) {
                bool isLoaded = false;
                if (teamBCopy.Member.Count > 0) {
                    for (int i = 0; i < teamBCopy.Member.Count; i++) {
                        if (teamBCopy.Member[i].Path.CurrentStatus == Path.Status.Start) {
                            isLoaded = true;
                            Shape shapeToAdd = convertStrokToShape(teamBCopy.Member[i].Path.Stroke,2);
                            strokesBCopy.Add(shapeToAdd);
                        }
                    }
                }
                if (isLoaded) {
                    teamB = teamBCopy;
                    dialog.Text = "Loaded";
                } else {
                    dialog.Text = "No Tactics Are Saved";
                }
            }
        }
        private void hide2_Click(object sender, RoutedEventArgs e)
        {
            hideTacticsB = !hideTacticsB;
            //Clear the strokes but keep the path saved
            
            if (strokesB.Count != 0)
            {
                foreach (Shape shape in strokesB)
                {
                    canvas.Children.Remove(shape);
                }
                strokesB.Clear();
            }
        }

        // Pause and Resume
        private void pause_Click(object sender, RoutedEventArgs e)
        {
            // Resume the game
            if (gameLogic == GameLogic.Pause)
            {
                gameLogic = GameLogic.Resume;
                t.Resume();
                tt.Abort();
                if (!canvas.Children.Contains(sellectedA))
                {
                    canvas.Children.Add(sellectedA);
                    Canvas.SetZIndex(sellectedA, -1);
                }
                if (!canvas.Children.Contains(sellectedB))
                {
                    canvas.Children.Add(sellectedB);
                    Canvas.SetZIndex(sellectedB, -1);
                }
                isTouch = false;
                isInk = false;
                inkcanvas.EditingMode = InkCanvasEditingMode.None;
                //Clear the canvas from strokes that are drawn
                inkcanvas.Strokes.Clear();
                //Team A
                if (strokesA.Count != 0)
                {
                    foreach (Shape shape in strokesA)
                    {
                        canvas.Children.Remove(shape);
                    }
                    strokesA.Clear();
                }
                if (strokesACopy.Count != 0)
                {
                    foreach (Shape shape in strokesACopy)
                    {
                        canvas.Children.Remove(shape);
                    }
                    strokesACopy.Clear();
                }
                //Team B
                if (strokesB.Count != 0) {
                    foreach (Shape shape in strokesB) {
                        canvas.Children.Remove(shape);
                    }
                    strokesB.Clear();
                }
                if (strokesBCopy.Count != 0) {
                    foreach (Shape shape in strokesBCopy) {
                        canvas.Children.Remove(shape);
                    }
                    strokesBCopy.Clear();
                }
                currentPlayerTeamA = teamA.Member[0];
                currentPlayerTeamB = teamB.Member[0];
                useControl = ControlStatus.isMoving;
                ScoreBar.Content = "0 : 0";
                //InitGameAt(640);
            }
            // Pause the game
            else
            {
                gameLogic = GameLogic.Pause;
                t.Suspend();
                if (canvas.Children.Contains(sellectedA))
                {
                    canvas.Children.Remove(sellectedA);
                }
                if (canvas.Children.Contains(sellectedB))
                {
                    canvas.Children.Remove(sellectedB);
                }
                isTouch = true;
                tt = new Thread(new ThreadStart(TaticsProc));
                isInk = true;
                //Team A
                if (teamACopy.Member.Count > 0) {
                    teamA = teamACopy;
                } else {
                    //Reset the players to the origion positions
                    InitGameAt(640);
                    for (int i = 0; i < teamA.Member.Count; i++) {
                        teamA.Member[i].Path = new Path();
                    }
                }
                //Team B
                if (teamBCopy.Member.Count > 0) {
                    teamB = teamBCopy;
                } else {
                    //Reset the players to the origion positions
                    InitGameAt(640);
                    for (int i = 0; i < teamB.Member.Count; i++) {
                        teamB.Member[i].Path = new Path();
                    }
                }
                inkcanvas.EditingMode = InkCanvasEditingMode.Ink;
                tt.Start();
                hideTacticsA = false;
                hideTacticsB = false;
            }
        }


        #endregion

        #region Tools

        // Moving object
        private void MoveWithPath(Path p, Image img)
        {
            if (p.CurrentStatus == Path.Status.Start)
            {
                Point pos = p.GetPoint();
                Move(img, pos);
            }
        }
        private void Move(Image img, Point pos)
        {
            if (bounds.Contains(new Point(pos.X -img.Width/2,pos.Y - img.Height/2))) 
            {
                Canvas.SetLeft(img, pos.X - img.Width / 2);
                Canvas.SetTop(img, pos.Y - img.Height / 2);
            }
        }
        private void Move(Label lbl, Point pos)
        {
            Canvas.SetLeft(lbl, pos.X);
            Canvas.SetTop(lbl, pos.Y + lbl.Height/4);
        }
        private Point Get(Image img)
        {
            Point p = new Point();
            p.X = Canvas.GetLeft(img) + img.Width / 2;
            p.Y = Canvas.GetTop(img) + img.Height / 2;
            return p;
        }
        private void InitGameAt(int yard)
        {
            useControl = ControlStatus.isMoving;
            // Set Position for each team
            // Team A
            Move(ball, new Point(yard - 150, 400));
            Move(teamA.Member[0].PlayerImage, new Point(yard - 150, 400));
            Move(teamA.Member[1].PlayerImage, new Point(yard - 100, 300));
            Move(teamA.Member[2].PlayerImage, new Point(yard - 100, 500));
            Move(teamA.Member[3].PlayerImage, new Point(yard - 50, 370));
            Move(teamA.Member[4].PlayerImage, new Point(yard - 50, 430));
            teamA.Member[0].Position = new Point(yard - 150, 400);
            teamA.Member[1].Position = new Point(yard - 100, 300);
            teamA.Member[2].Position = new Point(yard - 100, 500);
            teamA.Member[3].Position = new Point(yard - 50, 370);
            teamA.Member[4].Position = new Point(yard - 50, 430);

            // Team B
            Move(teamB.Member[0].PlayerImage, new Point(yard + 150, 400));
            Move(teamB.Member[1].PlayerImage, new Point(yard + 100, 300));
            Move(teamB.Member[2].PlayerImage, new Point(yard + 100, 500));
            Move(teamB.Member[3].PlayerImage, new Point(yard + 50, 370));
            Move(teamB.Member[4].PlayerImage, new Point(yard + 50, 430));
            teamB.Member[0].Position = new Point(yard + 150, 400);
            teamB.Member[1].Position = new Point(yard + 100, 300);
            teamB.Member[2].Position = new Point(yard + 100, 500);
            teamB.Member[3].Position = new Point(yard + 50, 370);
            teamB.Member[4].Position = new Point(yard + 50, 430);

        }
        private Player FIndTheHolder(Team team)
        {
            Player ballHolder = new Player();
            // Find the ball holder for the teamA
            for (int i = 0; i < team.Member.Count; i++)
            {
                double distLimit = 10;
                if (Dist2(Get(ball), Get(team.Member[i].PlayerImage)) < distLimit)
                {
                    if (team.Member[i].Role != Player.PlayerRole.TackleGurad)
                    {
                        ballHolder = team.Member[i];
                    }
                }
            }
            return ballHolder;
        }

        // Appendix
        private void SetFullScreen()
        {
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.WindowState = System.Windows.WindowState.Maximized;
        }
        private double IntersectAngle(Point v1, Point v2)
        {
            return Math.Acos((v1.X * v2.X + v1.Y * v2.Y) / (Math.Sqrt(v1.X * v1.X + v1.Y * v1.Y) * Math.Sqrt(v2.X * v2.X + v2.Y * v2.Y)));
        }
        private double Dist2(Point p1, Point p2)
        {
            return (Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)));
        }
        private double Rad2Deg(double r)
        {
            return (r * 180.0 / Math.PI);
        }
        private double Deg2Rad(double d)
        {
            return (d * Math.PI / 180.0);
        }
        private void myTextChange(object sender, EventArgs e)
        {
            dialog.ScrollToEnd();
        }
        private void ShowPoint(Point p)
        {
            Ellipse P = new Ellipse();
            P.Fill = System.Windows.Media.Brushes.Red; ;
            P.Width = 10;
            P.Height = 10;
            canvas.Children.Add(P);
            Canvas.SetLeft(P, p.X - P.Width / 2);
            Canvas.SetTop(P, p.Y - P.Height / 2);
        }
        private void ShowPoint(StylusPoint p)
        {
            Ellipse P = new Ellipse();
            P.Fill = System.Windows.Media.Brushes.Red; ;
            P.Width = 10;
            P.Height = 10;
            canvas.Children.Add(P);
            Canvas.SetLeft(P, p.X - P.Width / 2);
            Canvas.SetTop(P, p.Y - P.Height / 2);
        }
        private void ShowStroke(Stroke s)
        {
            foreach (StylusPoint p in s.StylusPoints)
            {
                ShowPoint(p);
            }
        }
        private Player GetCatch(Image obj, List<Player> players)
        {
            double distLimit = 20;

            for (int i = 0; i < players.Count; i++)
            {
                Point objPos = Get(obj);
                Point playerPos = Get(players[i].PlayerImage);

                if (Dist2(objPos, playerPos) < distLimit)
                {
                    return players[i];
                }
            }
            return null;
        }
        private Point Normalize(Point v)
        {
            Point p = new Point();
            p.X = v.X / Math.Sqrt(v.X * v.X + v.Y * v.Y);
            p.Y = v.Y / Math.Sqrt(v.X * v.X + v.Y * v.Y);
            return p;
        }
        //
        private Point Centeriod(Rect r)
        {
            return new Point(r.X + r.Width / 2, r.Y + r.Height / 2);
        }
        private Point GetDirection(Point p, Rect r)
        {
            return new Point(p.X - Centeriod(r).X, p.Y - Centeriod(r).Y);
        }
        //Convert the saved stroke to a shape
        private Shape convertStrokToShape(Stroke s, int teamNumber)
        {
            //Create a shape for each stroke that passed
            Polyline shape = new Polyline();
            //Get the points from the stroke
            PointCollection points = new PointCollection((Point[])s.StylusPoints);
            //use the stroke points as Polyline points
            shape.Points = points;
            //Convert it to shape
            Shape shapeToAdd = shape;
            if (teamNumber == 1 ) 
            {
                shapeToAdd.Stroke = Brushes.Gold;
            } else
            {
                shapeToAdd.Stroke = Brushes.Silver;
            }
            shapeToAdd.StrokeThickness = 4;
            canvas.Children.Add(shapeToAdd);
            return shapeToAdd;
        }
        // 
        private bool CheckDoubleClick()
        {
            DateTime dt = DateTime.Now;
            int minute = dt.Minute;
            int seconds = dt.Second;
            //Add it to the list
            currentTimeSec.Add(seconds);
            currentTimeMin.Add(minute);
            //Check the last two timings and make it by default 10 (any number bigger than 1 sec)
            int diffTimeInSec = 10;
            if (currentTimeSec.Count > 1 && currentTimeMin.Count > 1) {
                if (currentTimeMin[currentTimeMin.Count - 1] - currentTimeMin[currentTimeMin.Count - 2] == 0) {
                    diffTimeInSec = currentTimeSec[currentTimeSec.Count - 1] - currentTimeSec[currentTimeSec.Count - 2];
                }
            }
            //If it is less than 1 sec between 1 finger touches
            if (diffTimeInSec < 1) {
                //throw here for two consective touches
                //dialog.Text += "Two touches";
                return true;
            }
            return false;
        }
        //Calculate the distance between any two points
        private double Distance(Point p1, Point p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        //Caluclate the path length for a stroke
        private double PathLength(StylusPointCollection points)
        {

            double length = 0;
            for (int i = 1; i < points.Count; i++)
            {
                length += Distance(points[i - 1].ToPoint(), points[i].ToPoint());
            }
            return length;
        }

        #endregion
   }


    public static class ControlExtension
    {
        public static void Test(this Control control)
        { 
            
        }
    }
}
