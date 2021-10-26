using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using FallingBlocks.Engine.Core.Audio;
using FallingBlocks.Engine.Core.Core;
using FallingBlocks.Engine.Core.Core.Events;
using FallingBlocks.Engine.Core.Core.Primitive;
using FallingBlocks.Engine.Core.Core.Resource;
using FallingBlocks.Engine.Core.Util;
using FallingBlocks.Game.Objects;

namespace FallingBlocks.Game.Scene
{
    class FallingBlocksGameScene : ASceneGraph
    {
        /*****  Game Parameter START *****/

        internal const int ImageSize = 32;                          // Size of one cube, squared
        private const int CubeCountHorizontal = 12;                 // Size of the game field, horizontal, in cube counts
        private const int CubeCountVertical = 22;                   // Size of the game field, vertical, in cube counts
        private const int CubeCountSpaceToSideBoxes = 3;            // Size of the space between game field and side boxes, in cube counts
        private const int CubeCountNextCubePreviewTop = 10;         // Position of the next cube preview, from top, in cube counts
        private const int CubeCountSideBoxesWidth = 6;              // Width of the side boxes, in cube counts

        private const int EventResetTime = 150;                     // Reset time for key events, after this amount of milliseconds a new input is handled.
        private const int StartInterval = 1000;                     // Interval for moving down the moving shape. In milliseconds between two moves.
        private const int MinInterval = 100;                        // Minimum interval for the moving shape.
        private const int DropShapeInterval = 1;                    // Interval for the moving shape while dropping (down event).
        private const int MoveWhenDownInterval = 300;               // Interval after movements when the shape is down, to allow further movements before fix. Must be greater than the EventResetTime!
        private const int LinesNeededForLevelup = 5;                // After that much lines the player gets a level-up.
        private const int MaxSpeedupLevels = 10;                    // Maximum count of levels, where a speedup happens.
        private const int StartLevel = 1;                           // Start value for the level counter.
        private const int PointsForLines1 = 40;                     // Points for clearing 1 row.
        private const int PointsForLines2 = 100;                    // Points for clearing 2 rows.
        private const int PointsForLines3 = 300;                    // Points for clearing 3 rows.
        private const int PointsForLines4 = 1200;                   // Points for clearing 4 rows.
        private const int LineDestroyBlinkCount = 2;                // Blink count of a cleared line. 1 blink = set opacity to LineDestroyBlinkOpacity, wait, set the opacity back to LineDestroyFullOpacity.
        private const int LineDestroyBlinkInterval = 100;           // Interval of the blinking line. WholeBlinkDuration = LineDestroyBlinkInterval * LineDestroyBlinkCount * 2;
        private const float LineDestroyBlinkOpacity = 0.5f;         // Opacity while blinking. from 0.0f to 1.0f.
        private const float FullOpacity = 1.0f;                     // Default opacity of the shapes. Should be 1.0f.
        private const float GhostShapeOpacity = 0.2f;               // Opacity of the ghost shapes after the blink. Should be barely visible.
        private const float MediaPlayerVolumeStep = 0.1f;           // Step to increase/decrease volume
        private const float MediaPlayerVolumeInitial = 0.5f;        // Initial volume level

        private const bool DefaultSoundEnabled = true;
        private const bool DefaultGhostEnabled = true;

        private const Text2D.TextFont TextFont = Text2D.TextFont.Helvetica_18;
        private const Text2D.TextFont ControlsTextFont = Text2D.TextFont.Helvetica_12;

        private static readonly Color WindowBackgroundColor = Color.LightSteelBlue;
        private static readonly Color GameFieldBackgroundColor = Color.Black.AdjustAlpha(0.9f);
        private static readonly Color TextColor = Color.DeepSkyBlue;

        /*****  Game Parameter END  *****/

        /*****  Calculated Values START *****/

        private const int MaxCubeIndexHorizontal = CubeCountHorizontal - 1;
        private const int MaxCubeIndexVertical = CubeCountVertical - 1;

        private const int SpeedupPerLevelup = (StartInterval - MinInterval) / MaxSpeedupLevels; // Each level the game (currentInterval) gets faster by this value.

        private readonly PointF shapesStartPoint = new PointF((int)(MaxCubeIndexHorizontal / 2), 0);
        private readonly PointF nextShapePoint = new PointF(MaxCubeIndexHorizontal + CubeCountSpaceToSideBoxes + CubeCountSideBoxesWidth / 2, CubeCountNextCubePreviewTop);

        /*****  Calculated Glol Values END *****/

        internal static ImageResource CubeLime;
        internal static ImageResource CubeRed;
        internal static ImageResource CubeYellow;
        internal static ImageResource CubeOrange;
        internal static ImageResource CubeBlue;
        internal static ImageResource CubeMagenta;
        internal static ImageResource CubeCyan;
        internal static ImageResource BackgroundResource;

        private static readonly ShapeStyle[] AllStyleValues = Enum.GetValues(typeof(ShapeStyle)).Cast<ShapeStyle>().ToArray();

        public readonly int GameFieldLeft;
        public readonly int GameFieldTop;
        public readonly int GameFieldHeight;
        public readonly int GameFieldWidth;

        private readonly Rectangle2D gameFieldRectangle;
        private readonly Rectangle2D statisticsRectangle;
        private readonly Rectangle2D nextShapeRectangle;
        private readonly Rectangle2D controlsRectangle;
        private readonly Text2D currentLevelLabelText;
        private readonly Text2D currentLineCountLabelText;
        private readonly Text2D currentPointsLabelText;
        private readonly Text2D nextElementLabelText;
        private readonly Text2D currentLevelText;
        private readonly Text2D currentLineCountText;
        private readonly Text2D currentPointsText;
        private readonly Text2D controlsHeaderText;
        private readonly Text2D controlsKeysText;
        private readonly Text2D controlsHintsText;
        private readonly Image2D backgroundImage;

        private List<IFallingBlocksObject> allObjects;
        private List<Cube> currentStaticCubes;
        private Cube[,] currentGameField;

        private Shape currentMovingShape;
        private Shape nextMovingShape;
        private Shape ghostShape;
        private List<ShapeStyle> nextShapeStyles;
        private List<ShapeStyle> prevShapeStyles;
        private int currentInterval;
        private long nextMoveTimestamp;

        private long nextAcceptTime_Left;
        private long nextAcceptTime_Right;
        private long nextAcceptTime_Down;
        private long nextAcceptTime_Up;
        private long nextAcceptTime_Space;
        private long nextAcceptTime_S;
        private long nextAcceptTime_G;
        private long nextAcceptTime_Add;
        private long nextAcceptTime_Subtract;

        private int currentLevel;
        private int currentLineCount;
        private int currentPoints;

        private bool isDropping;
        private bool isDown;
        private int currentDropPoints;

        private bool isDestroyMode;
        private List<int> linesToDestroy;
        private int currentBlinkCount;
        private float currentBlinkStatus;

        private bool soundEnabled = FallingBlocksGameScene.DefaultSoundEnabled;
        private bool ghostEnabled = FallingBlocksGameScene.DefaultGhostEnabled;

        private IMediaPlayer mediaPlayerTheme;
        private float mediaPlayerVolume = FallingBlocksGameScene.MediaPlayerVolumeInitial;

        public FallingBlocksGameScene()
        {
            CubeLime = this.AddResource(new ImageResource(Resources.cube_lime_32));
            CubeRed = this.AddResource(new ImageResource(Resources.cube_red_32));
            CubeYellow = this.AddResource(new ImageResource(Resources.cube_yellow_32));
            CubeOrange = this.AddResource(new ImageResource(Resources.cube_orange_32));
            CubeBlue = this.AddResource(new ImageResource(Resources.cube_blue_32));
            CubeMagenta = this.AddResource(new ImageResource(Resources.cube_magenta_32));
            CubeCyan = this.AddResource(new ImageResource(Resources.cube_cyan_32));
            BackgroundResource = this.AddResource(new ImageResource(Resources.Background));

            this.GameFieldWidth = MaxCubeIndexHorizontal * ImageSize;
            this.GameFieldHeight = MaxCubeIndexVertical * ImageSize;
            this.GameFieldLeft = -GameFieldWidth / 2;
            this.GameFieldTop = -GameFieldHeight / 2 + ImageSize / 2;

            var sideBoxesSpaceWidth = ImageSize * CubeCountSpaceToSideBoxes;
            var sideBoxesWidth = ImageSize * CubeCountSideBoxesWidth;

            var leftBoxesLeft = this.GameFieldLeft - sideBoxesSpaceWidth - sideBoxesWidth;
            var rightBoxesLeft = this.GameFieldLeft + GameFieldWidth + sideBoxesSpaceWidth;

            this.backgroundImage = this.AddObject(new Image2D(BackgroundResource));

            this.gameFieldRectangle = this.AddObject(new Rectangle2D());
            this.gameFieldRectangle.Position = new PointF(this.GameFieldLeft - ImageSize / 2, this.GameFieldTop - ImageSize / 2);
            this.gameFieldRectangle.Size = new SizeF(this.GameFieldWidth + ImageSize, this.GameFieldHeight + ImageSize);
            this.gameFieldRectangle.Color = GameFieldBackgroundColor;

            this.statisticsRectangle = this.AddObject(new Rectangle2D());
            this.statisticsRectangle.Position = new PointF(rightBoxesLeft, this.GameFieldTop - ImageSize / 2);
            this.statisticsRectangle.Size = new SizeF(sideBoxesWidth, ImageSize * 4);
            this.statisticsRectangle.Color = GameFieldBackgroundColor;

            this.nextShapeRectangle = this.AddObject(new Rectangle2D());
            this.nextShapeRectangle.Position = new PointF(rightBoxesLeft, this.GameFieldTop + ImageSize * (this.nextShapePoint.Y - 2.5f));
            this.nextShapeRectangle.Size = new SizeF(sideBoxesWidth, ImageSize * 6);
            this.nextShapeRectangle.Color = GameFieldBackgroundColor;

            this.nextElementLabelText = this.AddObject(new Text2D());
            this.nextElementLabelText.Position = new PointF(rightBoxesLeft + 20, this.GameFieldTop + ImageSize * 8 + 10);
            this.nextElementLabelText.Font = TextFont;
            this.nextElementLabelText.Text = "Next:";
            this.nextElementLabelText.Color = TextColor;

            this.currentLevelLabelText = this.AddObject(new Text2D());
            this.currentLevelLabelText.Position = new PointF(rightBoxesLeft + 20, this.GameFieldTop + 20);
            this.currentLevelLabelText.Font = TextFont;
            this.currentLevelLabelText.Text = "Level:";
            this.currentLevelLabelText.Color = TextColor;

            this.currentLineCountLabelText = this.AddObject(new Text2D());
            this.currentLineCountLabelText.Position = new PointF(rightBoxesLeft + 20, this.GameFieldTop + ImageSize + 20);
            this.currentLineCountLabelText.Font = TextFont;
            this.currentLineCountLabelText.Text = "Lines:";
            this.currentLineCountLabelText.Color = TextColor;

            this.currentPointsLabelText = this.AddObject(new Text2D());
            this.currentPointsLabelText.Position = new PointF(rightBoxesLeft + 20, this.GameFieldTop + ImageSize * 2 + 20);
            this.currentPointsLabelText.Font = TextFont;
            this.currentPointsLabelText.Text = "Points:";
            this.currentPointsLabelText.Color = TextColor;

            this.currentLevelText = this.AddObject(new Text2D());
            this.currentLevelText.Position = this.currentLevelLabelText.Position.Add(70, 0);
            this.currentLevelText.Font = TextFont;
            this.currentLevelText.Color = TextColor;

            this.currentLineCountText = this.AddObject(new Text2D());
            this.currentLineCountText.Position = this.currentLineCountLabelText.Position.Add(70, 0);
            this.currentLineCountText.Font = TextFont;
            this.currentLineCountText.Color = TextColor;

            this.currentPointsText = this.AddObject(new Text2D());
            this.currentPointsText.Position = this.currentPointsLabelText.Position.Add(70, 0);
            this.currentPointsText.Font = TextFont;
            this.currentPointsText.Color = TextColor;

            this.controlsRectangle = this.AddObject(new Rectangle2D());
            this.controlsRectangle.Position = new PointF(leftBoxesLeft, this.GameFieldTop - ImageSize / 2);
            this.controlsRectangle.Size = new SizeF(sideBoxesWidth, ImageSize * 8);
            this.controlsRectangle.Color = GameFieldBackgroundColor;

            this.controlsHeaderText = this.AddObject(new Text2D());
            this.controlsHeaderText.Position = new PointF(leftBoxesLeft + 20, this.controlsRectangle.Position.Y + 30);
            this.controlsHeaderText.Font = TextFont;
            this.controlsHeaderText.Color = TextColor;
            this.controlsHeaderText.Text = "Controls:";

            this.controlsKeysText = this.AddObject(new Text2D());
            this.controlsKeysText.Position = new PointF(leftBoxesLeft + 20, this.controlsRectangle.Position.Y + 45);
            this.controlsKeysText.Font = ControlsTextFont;
            this.controlsKeysText.Color = TextColor;
            this.controlsKeysText.Text =
                Environment.NewLine + " < >" +
                Environment.NewLine + "  v" +
                Environment.NewLine + "  ^" +
                Environment.NewLine + "SPACE" +
                Environment.NewLine +
                Environment.NewLine + "  G" +
                Environment.NewLine +
                Environment.NewLine + "  S" +
                Environment.NewLine + "  +" +
                Environment.NewLine + "  -" +
                Environment.NewLine +
                Environment.NewLine + " ESC" +
                "";

            this.controlsHintsText = this.AddObject(new Text2D());
            this.controlsHintsText.Position = new PointF(this.controlsRectangle.Position.X + 80, this.controlsRectangle.Position.Y + 45);
            this.controlsHintsText.Font = ControlsTextFont;
            this.controlsHintsText.Color = TextColor;
            this.controlsHintsText.Text =
                Environment.NewLine + "move left/right" +
                Environment.NewLine + "fast down" +
                Environment.NewLine + "rotate" +
                Environment.NewLine + "rotate" +
                Environment.NewLine +
                Environment.NewLine + "toggle ghost" +
                Environment.NewLine +
                Environment.NewLine + "toggle sound" +
                Environment.NewLine + "volume up" +
                Environment.NewLine + "volume down" +
                Environment.NewLine + "" +
                Environment.NewLine + "pause" +
                Environment.NewLine + "";

            this.Background = WindowBackgroundColor;

            this.nextShapeStyles = new List<ShapeStyle>();
            this.prevShapeStyles = new List<ShapeStyle>();
            this.allObjects = new List<IFallingBlocksObject>();
            this.currentStaticCubes = new List<Cube>();
            this.currentGameField = new Cube[MaxCubeIndexHorizontal + 1, MaxCubeIndexVertical + 1];
        }

        public override void Init(IRenderContext context)
        {
            base.Init(context);

            this.Reset();

            this.InitThemeSound();
            if (this.soundEnabled)
            {
                this.StartThemeSound();
            }
        }

        protected override void UpdateOneFrame(IRenderContext context, long timestampMs, long elapsedMsSinceLastLoop)
        {
            var screen = context.GetScreenBounds();
            var scaleW = screen.Width / this.backgroundImage.Width;
            var scaleH = screen.Height / this.backgroundImage.Height;
            this.backgroundImage.Scale = Math.Max(scaleH, scaleW);

            if (this.nextMoveTimestamp == 0)
            {
                this.nextMoveTimestamp = timestampMs + currentInterval;
            }
            else
            {
                this.ProcessAutomaticMove(timestampMs);
                if (this.IsGameOver)
                {
                    return;
                }

                this.ProcessKeyEvents(timestampMs);
            }

            this.CalcGhostShapePosition();

            // Calculate current level
            var levelups = this.currentLineCount / LinesNeededForLevelup;
            this.currentLevel = StartLevel + levelups;
            this.currentInterval = StartInterval - (SpeedupPerLevelup * levelups);
            this.currentInterval = Math.Max(MinInterval, this.currentInterval);

            // Set info text
            this.currentLevelText.Text = this.currentLevel.ToString();
            this.currentLineCountText.Text = this.currentLineCount.ToString();
            this.currentPointsText.Text = this.currentPoints.ToString();

            // Calculate image position
            this.allObjects.ForEach(x => x.UpdateImagePositions(this));

            base.UpdateOneFrame(context, timestampMs, elapsedMsSinceLastLoop);
        }

        private void Reset()
        {
            this.StopThemeSound();

            this.allObjects.ForEach(x => x.RemoveFromScene(this));

            this.allObjects = new List<IFallingBlocksObject>();
            this.currentStaticCubes = new List<Cube>();
            this.currentGameField = new Cube[MaxCubeIndexHorizontal + 1, MaxCubeIndexVertical + 1];

            this.nextShapeStyles = new List<ShapeStyle>();
            this.prevShapeStyles = new List<ShapeStyle>();
            this.linesToDestroy = new List<int>();

            this.currentInterval = StartInterval;
            this.currentLevel = StartLevel;
            this.currentLineCount = 0;
            this.currentPoints = 0;
            this.isDropping = false;
            this.isDown = false;
            this.nextMoveTimestamp = 0;

            this.currentMovingShape = this.CreateNextShape();
            this.currentMovingShape.ShapePosition = this.shapesStartPoint;

            this.CreateGhostShape();

            this.nextMovingShape = this.CreateNextShape();
        }

        private void InitThemeSound()
        {
            var themeMp3Resource = Resources.ThemeSong;
            if (themeMp3Resource != null)
            {
                this.mediaPlayerTheme = MediaPlayer.Factory?.CreatePlayerLoop(new MemoryStream(themeMp3Resource), this.mediaPlayerVolume, false);
            }
        }

        private void StartThemeSound()
        {
            if (this.mediaPlayerTheme != null)
            {
                this.mediaPlayerTheme.ResetPlayback();
                this.mediaPlayerTheme.Play();
            }
        }

        private void StopThemeSound()
        {
            if (this.mediaPlayerTheme != null)
            {
                this.mediaPlayerTheme.Stop();
                this.mediaPlayerTheme.ResetPlayback();
            }
        }

        private void MediaPlayerVolumeUp()
        {
            this.IncreaseMediaPlayerVolumeInternal(FallingBlocksGameScene.MediaPlayerVolumeStep);
        }

        private void MediaPlayerVolumeDown()
        {
            this.IncreaseMediaPlayerVolumeInternal(-FallingBlocksGameScene.MediaPlayerVolumeStep);
        }

        private void IncreaseMediaPlayerVolumeInternal(float step)
        {
            this.mediaPlayerVolume = Math.Max(0f, Math.Min(1f, this.mediaPlayerVolume + step));
            if (this.mediaPlayerTheme != null)
            {
                this.mediaPlayerTheme.SetVolume(this.mediaPlayerVolume);
            }
        }

        private void ToggleThemeSound()
        {
            this.soundEnabled ^= true;

            if (this.soundEnabled)
            {
                this.StartThemeSound();
            }
            else
            {
                this.StopThemeSound();
            }
        }

        private void ToggleGhostShape()
        {
            this.ghostEnabled ^= true;

            if (this.ghostEnabled)
            {
                this.CreateGhostShape();
            }
            else
            {
                this.DestroyGhostShape();
            }
        }

        private void ProcessAutomaticMove(long timestampMs)
        {
            // Move down automatically
            if (timestampMs >= this.nextMoveTimestamp)
            {
                if (this.currentMovingShape.ShapePosition.Y <= 0 &&
                    this.currentMovingShape.IsCurrentPositionBlocked(this.currentStaticCubes))
                {
                    // new shape spawned over existing blocks. cannot move. game over.
                    this.GameOver(this.currentLevel, this.currentPoints, new Dictionary<string, int>() {{ "Cleared Lines", this.currentLineCount }});
                }

                if (this.isDestroyMode)
                {
                    if (this.currentBlinkCount >= LineDestroyBlinkCount)
                    {
                        this.DestroyLines();
                        this.ScoreFullLines();
                        this.AddNextShape();
                    }
                    else
                    {
                        this.BlinkLines();
                    }
                }
                else
                {
                    bool couldMove = this.Move(this.currentMovingShape, 0, 1);
                    if (couldMove)
                    {
                        this.isDown = false;
                        if (this.isDropping)
                        {
                            // One point for each row the shape is dropping
                            this.currentDropPoints++;
                        }
                    }
                    else
                    {
                        if (this.isDropping)
                        {
                            // shape was dropped, let the player move one interval
                            this.isDropping = false;
                            this.isDown = true;
                            this.DestroyGhostShape();
                        }
                        else
                        {
                            // shape is down.
                            this.isDropping = false;
                            this.isDown = false;
                            this.DestroyGhostShape();

                            this.currentPoints += this.currentDropPoints;
                            this.currentDropPoints = 0;

                            // split the shape into cubes
                            this.AddStaticCubes(this.currentMovingShape.Split());

                            // Search for full lines, if there are any, isDestroyMode is set to start blinking.
                            this.FindFullLines();

                            if (!this.isDestroyMode)
                            {
                                // no full lines found, add next shape
                                this.AddNextShape();
                            }
                        }
                    }
                }
                this.CalcNextMoveTimestamp(timestampMs);
            }
        }

        private void CalcNextMoveTimestamp(long timestampMs)
        {
            if (this.isDropping)
            {
                this.nextMoveTimestamp = timestampMs + DropShapeInterval;
            }
            else if (this.isDown)
            {
                this.nextMoveTimestamp = timestampMs + MoveWhenDownInterval;
            }
            else if (this.isDestroyMode)
            {
                this.nextMoveTimestamp = timestampMs + LineDestroyBlinkInterval;
            }
            else
            {
                this.nextMoveTimestamp = timestampMs + this.currentInterval;
            }
        }

        private void AddNextShape()
        {
            this.currentMovingShape = this.nextMovingShape;
            this.currentMovingShape.ShapePosition = this.shapesStartPoint;

            this.nextMovingShape = this.CreateNextShape();

            this.CreateGhostShape();
        }

        private void ProcessKeyEvents(long timestampMs)
        {
            // Move left
            bool leftPressed = this.IsPressed(EventType.Left);
            if (leftPressed)
            {
                if (timestampMs >= nextAcceptTime_Left)
                {
                    nextAcceptTime_Left = timestampMs + EventResetTime;
                    this.Move(this.currentMovingShape, -1, 0);

                    if (this.isDown)
                    {
                        this.CalcNextMoveTimestamp(timestampMs);
                    }
                }
            }
            else
            {
                nextAcceptTime_Left = 0;
            }

            // Move right
            bool rightPressed = this.IsPressed(EventType.Right);
            if (rightPressed)
            {
                if (timestampMs >= nextAcceptTime_Right)
                {
                    nextAcceptTime_Right = timestampMs + EventResetTime;
                    this.Move(this.currentMovingShape, 1, 0);

                    if (this.isDown)
                    {
                        this.CalcNextMoveTimestamp(timestampMs);
                    }
                }
            }
            else
            {
                nextAcceptTime_Right = 0;
            }

            // Move down
            bool downPressed = this.IsPressed(EventType.Down);
            if (downPressed)
            {
                if (timestampMs >= nextAcceptTime_Down)
                {
                    nextAcceptTime_Down = timestampMs + EventResetTime;

                    if (!this.isDropping && !this.isDown)
                    {
                        this.isDropping = true;
                        this.CalcNextMoveTimestamp(timestampMs);
                    }
                }
            }
            else
            {
                nextAcceptTime_Down = 0;
            }

            // Rotate right
            bool upPressed = this.IsPressed(EventType.Up);
            if (upPressed)
            {
                if (timestampMs >= nextAcceptTime_Up)
                {
                    nextAcceptTime_Up = timestampMs + EventResetTime;

                    this.TryRotateRight(this.currentMovingShape);

                    if (this.isDown)
                    {
                        this.CalcNextMoveTimestamp(timestampMs);
                    }
                }
            }
            else
            {
                nextAcceptTime_Up = 0;
            }

            // Rotate left
            bool spacePressed = this.IsPressed(EventType.Fire);
            if (spacePressed)
            {
                if (timestampMs >= nextAcceptTime_Space)
                {
                    nextAcceptTime_Space = timestampMs + EventResetTime;

                    this.TryRotateLeft(this.currentMovingShape);

                    if (this.isDown)
                    {
                        this.CalcNextMoveTimestamp(timestampMs);
                    }
                }
            }
            else
            {
                nextAcceptTime_Space = 0;
            }

            bool sPressed = this.IsPressed(EventType.S);
            if (sPressed)
            {
                if (timestampMs >= nextAcceptTime_S)
                {
                    nextAcceptTime_S = timestampMs + EventResetTime;

                    this.ToggleThemeSound();
                }
            }
            else
            {
                nextAcceptTime_S = 0;
            }

            bool gPressed = this.IsPressed(EventType.G);
            if (gPressed)
            {
                if (timestampMs >= nextAcceptTime_G)
                {
                    nextAcceptTime_G = timestampMs + EventResetTime;

                    this.ToggleGhostShape();
                }
            }
            else
            {
                nextAcceptTime_G = 0;
            }

            bool addPressed = this.IsPressed(EventType.Add);
            if (addPressed)
            {
                if (timestampMs >= nextAcceptTime_Add)
                {
                    nextAcceptTime_Add = timestampMs + EventResetTime;

                    this.MediaPlayerVolumeUp();
                }
            }
            else
            {
                nextAcceptTime_Add = 0;
            }

            bool subtractPressed = this.IsPressed(EventType.Subtract);
            if (subtractPressed)
            {
                if (timestampMs >= nextAcceptTime_Subtract)
                {
                    nextAcceptTime_Subtract = timestampMs + EventResetTime;

                    this.MediaPlayerVolumeDown();
                }
            }
            else
            {
                nextAcceptTime_Subtract = 0;
            }
        }

        private Shape CreateNextShape()
        {
            if (this.nextShapeStyles.Count == 0)
            {
                this.nextShapeStyles = AllStyleValues.ToList();
                this.nextShapeStyles.Shuffle();

                while (this.nextShapeStyles.FirstOrDefault() == this.prevShapeStyles.LastOrDefault())
                {
                    this.nextShapeStyles.Shuffle();
                }

                this.prevShapeStyles = this.nextShapeStyles.ToList();
            }

            ShapeStyle newShapeStyle = this.nextShapeStyles[0];
            this.nextShapeStyles.RemoveAt(0);

            var shape = new Shape(newShapeStyle, FullOpacity);
            this.allObjects.Add(shape);
            shape.AddToScene(this);

            if (shape.ShapeStyle == ShapeStyle.I)
            {
                shape.ShapePosition = this.nextShapePoint.Add(0.5f, 0.5f);
            }
            else if (shape.ShapeStyle == ShapeStyle.O)
            {
                shape.ShapePosition = this.nextShapePoint.Add(-0.5f, 0f);
            }
            else
            {
                shape.ShapePosition = this.nextShapePoint;
            }

            return shape;
        }

        private void DestroyGhostShape()
        {
            if (this.ghostShape != null)
            {
                this.ghostShape.RemoveFromScene(this);
                this.allObjects.Remove(this.ghostShape);
            }
        }

        private void CreateGhostShape()
        {
            if (!this.ghostEnabled) return;

            this.DestroyGhostShape();
            this.ghostShape = new Shape(this.currentMovingShape.ShapeStyle, GhostShapeOpacity);
            this.allObjects.Add(this.ghostShape);
            this.ghostShape.AddToScene(this);
            this.currentMovingShape.GhostShape = this.ghostShape;
        }

        private void CalcGhostShapePosition()
        {
            if (!this.ghostEnabled) return;

            this.ghostShape.ShapePosition = new PointF(this.currentMovingShape.ShapePosition.X, 0);
            while (this.Move(this.ghostShape, 0, 1))
            {
                // move down until it is blocked
            }
        }

        private bool Move(Shape shape, float offsetX, float offsetY)
        {
            var currentRect = shape.BoundingRectangle;
            var currentPos = shape.ShapePosition;
            bool canMoveOffset = true;

            if (offsetX < 0)
            {
                if (-offsetX > currentRect.Left) // x-offsetX cannot be <0
                {
                    offsetX = -currentRect.Left;
                    canMoveOffset = false;
                }
            }

            if (offsetX > 0)
            {
                if (offsetX > MaxCubeIndexHorizontal - currentRect.Right)    // x+offsetX cannot be >gameBlocksHorizontal
                {
                    offsetX = MaxCubeIndexHorizontal - currentRect.Right;
                    canMoveOffset = false;
                }
            }

            if (offsetY < 0)
            {
                if (-offsetY > currentRect.Top)    // y-offsetY cannot be <0
                {
                    offsetY = -currentRect.Top;
                    canMoveOffset = false;
                }
            }

            if (offsetY > 0)
            {
                if (offsetY > MaxCubeIndexVertical - currentRect.Bottom) // x+offsetX cannot be >gameBlocksVertical
                {
                    offsetY = MaxCubeIndexVertical - currentRect.Bottom;
                    canMoveOffset = false;
                }
            }

            shape.ShapePosition = new PointF(currentPos.X + offsetX, currentPos.Y + offsetY);
            if (shape.IsCurrentPositionBlocked(this.currentStaticCubes))
            {
                shape.ShapePosition = currentPos;
                canMoveOffset = false;
            }
            return canMoveOffset;
        }

        private void MoveIntoView(Shape shape)
        {
            var rect = shape.BoundingRectangle;
            if (rect.Left < 0)
            {
                shape.ShapePosition = shape.ShapePosition.Add(0 - rect.Left, 0);
            }
            if (rect.Right > MaxCubeIndexHorizontal)
            {
                shape.ShapePosition = shape.ShapePosition.Add(MaxCubeIndexHorizontal - rect.Right, 0);
            }
            if (rect.Top < 0)
            {
                shape.ShapePosition = shape.ShapePosition.Add(0, 0 - rect.Top);
            }
            if (rect.Bottom > MaxCubeIndexVertical)
            {
                shape.ShapePosition = shape.ShapePosition.Add(0, MaxCubeIndexVertical - rect.Bottom);
            }
        }

        private void TryRotateLeft(Shape shape)
        {
            bool rollback = false;
            int rotationCount = 0;
            var oldPosition = shape.ShapePosition;
            shape.RotateLeft();
            if (shape.GhostShape != null) shape.GhostShape.RotateLeft();
            this.MoveIntoView(shape);
            rotationCount++;

            // try up to three rotations. if one of them is allowed, apply. if not, rollback.
            while (shape.IsCurrentPositionBlocked(this.currentStaticCubes))
            {
                if (this.isDown)
                {
                    if (rotationCount >= 3)
                    {
                        rollback = true;
                        break;
                    }

                    shape.RotateLeft();
                    if (shape.GhostShape != null) shape.GhostShape.RotateLeft();
                    this.MoveIntoView(shape);
                    rotationCount++;
                }
                else
                {
                    rollback = true;
                    break;
                }
            }

            if (rollback)
            {
                for (int i = 0; i < rotationCount; i++)
                {
                    shape.RotateRight();
                    if (shape.GhostShape != null) shape.GhostShape.RotateRight();
                }
                shape.ShapePosition = oldPosition;
            }
        }

        private void TryRotateRight(Shape shape)
        {
            bool rollback = false;
            int rotationCount = 0;
            var oldPosition = shape.ShapePosition;
            shape.RotateRight();
            if (shape.GhostShape != null) shape.GhostShape.RotateRight();
            this.MoveIntoView(shape);
            rotationCount++;

            // try up to three rotations. if one of them is allowed, apply. if not, rollback.
            while (shape.IsCurrentPositionBlocked(this.currentStaticCubes))
            {
                if (this.isDown)
                {
                    if (rotationCount >= 3)
                    {
                        rollback = true;
                        break;
                    }

                    shape.RotateRight();
                    if (shape.GhostShape != null) shape.GhostShape.RotateRight();
                    this.MoveIntoView(shape);
                    rotationCount++;
                }
                else
                {
                    rollback = true;
                    break;
                }
            }

            if (rollback)
            {
                for (int i = 0; i < rotationCount; i++)
                {
                    shape.RotateLeft();
                    if (shape.GhostShape != null) shape.GhostShape.RotateLeft();
                }
                shape.ShapePosition = oldPosition;
            }
        }

        private void AddStaticCubes(List<Cube> cubes)
        {
            foreach (var cube in cubes)
            {
                currentGameField[(int)cube.Position.X, (int)cube.Position.Y] = cube;
            }
            this.currentStaticCubes.AddRange(cubes);
        }

        private void FindFullLines()
        {
            var filledLines = new List<int>();

            for (int y = MaxCubeIndexVertical; y >= 0; y--)
            {
                // Check if this line is filled:
                bool lineFilled = true;
                for (int x = MaxCubeIndexHorizontal; x >= 0; x--)
                {
                    if (this.currentGameField[x, y] == null)
                    {
                        lineFilled = false;
                        break;
                    }
                }

                if (lineFilled)
                {
                    filledLines.Add(y);
                }
            }

            if (filledLines.Count > 0)
            {
                this.linesToDestroy = filledLines;
                this.isDestroyMode = true;
                this.currentBlinkCount = 0;
                this.currentBlinkStatus = FullOpacity;
            }
            else
            {
                this.linesToDestroy.Clear();
                this.isDestroyMode = false;
                this.currentBlinkCount = 0;
                this.currentBlinkStatus = FullOpacity;
            }
        }

        private void BlinkLines()
        {
            if (FloatHelper.FloatEquals(this.currentBlinkStatus, FullOpacity))
            {
                this.currentBlinkStatus = LineDestroyBlinkOpacity;
            }
            else
            {
                this.currentBlinkStatus = FullOpacity;
                this.currentBlinkCount++;
            }

            for (int y = MaxCubeIndexVertical; y >= 0; y--)
            {
                if (this.linesToDestroy.Contains(y))
                {
                    // remove the blocks of this line:
                    for (int x = MaxCubeIndexHorizontal; x >= 0; x--)
                    {
                        var cubeToBlink = this.currentGameField[x, y];
                        if (cubeToBlink != null)
                        {
                            cubeToBlink.Opacity = this.currentBlinkStatus;
                        }
                        else
                        {
                            Debugger.Break(); // should not happen
                        }
                    }
                }
            }
        }

        private void DestroyLines()
        {
            var linesLocal = this.linesToDestroy.ToList();
            for (int y = MaxCubeIndexVertical; y >= 0; y--)
            {
                if (linesLocal.Contains(y))
                {
                    // remove the blocks of this line:
                    for (int x = MaxCubeIndexHorizontal; x >= 0; x--)
                    {
                        var removedCube = this.currentGameField[x, y];
                        if (removedCube != null)
                        {
                            this.allObjects.Remove(removedCube);
                            this.currentStaticCubes.Remove(removedCube);
                            removedCube.RemoveFromScene(this);
                        }
                        this.currentGameField[x, y] = null;
                    }

                    // Now move all upper blocks 1 step downwards.
                    for (int yMove = y; yMove > 0; yMove--)
                    {
                        for (int x = MaxCubeIndexHorizontal; x >= 0; x--)
                        {
                            var movedCube = this.currentGameField[x, yMove - 1];
                            if (movedCube != null) movedCube.Position = movedCube.Position.Add(0, 1);
                            this.currentGameField[x, yMove] = movedCube;
                        }
                    }

                    // The first line is null now.
                    for (int x = MaxCubeIndexHorizontal; x >= 0; x--)
                    {
                        this.currentGameField[x, 0] = null;
                    }

                    // remove the destroyed line from the list and check this line index again
                    linesLocal.Remove(y);
                    linesLocal = linesLocal.Select(line => line + 1).ToList();
                    y++;
                }
            }
        }

        private void ScoreFullLines()
        {
            int filledLines = this.linesToDestroy.Count;
            if (filledLines > 0)
            {
                bool allEmpty = true;
                for (int y = MaxCubeIndexVertical; y >= 0 && allEmpty; y--)
                {
                    for (int x = MaxCubeIndexHorizontal; x >= 0 && allEmpty; x--)
                    {
                        if (this.currentGameField[x, y] != null)
                        {
                            allEmpty = false;
                        }
                    }
                }

                int pointsForTheRows = 0;
                switch (filledLines)
                {
                    case 1: pointsForTheRows = PointsForLines1; break;
                    case 2: pointsForTheRows = PointsForLines2; break;
                    case 3: pointsForTheRows = PointsForLines3; break;
                    case 4: pointsForTheRows = PointsForLines4; break;
                }
                int pointsToAdd = pointsForTheRows * this.currentLevel;

                this.currentLineCount += filledLines;
                this.currentPoints += pointsToAdd;
            }

            this.linesToDestroy.Clear();
            this.isDestroyMode = false;
            this.currentBlinkCount = 0;
            this.currentBlinkStatus = FullOpacity;
        }
    }
}
