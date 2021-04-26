using OculusLeviathan.Components;
using OculusLeviathan.Data;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace OculusLeviathan
{
    public class Game1 : MachinaGame
    {
        public Game1(string[] args) : base("LD48", args, new Point(1920, 1080), new Point(1920 / 2, 1080 / 2), ResizeBehavior.MaintainDesiredResolution)
        {

        }

        protected override void OnGameLoad()
        {
            var bgScene = SceneLayers.AddNewScene();
            var bgScene2 = SceneLayers.AddNewScene();
            var bgScene3 = SceneLayers.AddNewScene();


            var gameScene = SceneLayers.AddNewScene();
            var uiScene = SceneLayers.AddNewScene();
            // SceneLayers.BackgroundColor = Color.Black;

            var bgActor = bgScene.AddActor("Background");
            new BackgroundRenderer(bgActor, gameScene.camera, 0.6f);

            var bgActor2 = bgScene2.AddActor("Background");
            new BackgroundRenderer(bgActor2, gameScene.camera, 1f);

            var world = gameScene.AddActor("World");
            new WorldStuff(world);

            void StartGame()
            {
                var harness = gameScene.AddActor("Harness", new Vector2(gameScene.camera.ViewportCenter.X, -256));

                var levelIndex = 10;

                var levelTransition = new LevelTransition(harness, levelIndex);
                var player = new Player(harness);
                new BubbleSpawner(harness, new MinMax<int>(3, 7));
                new Harness(harness);

                var eye = harness.transform.AddActorAsChild("Eye");
                var eyeRenderer = new EyeRenderer(eye, player, levelTransition);

                var targetReticalActor = gameScene.AddActor("Redical");
                targetReticalActor.transform.Depth -= 20;
                new TargetRedical(targetReticalActor, player);

                {
                    var groupActor = uiScene.AddActor("UI Parent Group");
                    new BoundingRect(groupActor, uiScene.camera.ViewportWidth, uiScene.camera.ViewportHeight);
                    var grp = new LayoutGroup(groupActor, Orientation.Vertical);

                    grp.PixelSpacer(32, (int) (uiScene.camera.ViewportHeight * 9f / 12f));

                    grp.AddBothStretchedElement("TextGroupParent", act =>
                    {
                        var textGroup = new LayoutGroup(act, Orientation.Horizontal);
                        textGroup.HorizontallyStretchedSpacer();
                        textGroup.AddVerticallyStretchedElement("Text", 900, act =>
                        {
                            new BoundedTextRenderer(act, "", MachinaGame.Assets.GetSpriteFont("Roboto"), Color.White, HorizontalAlignment.Left, VerticalAlignment.Top, Overflow.Ignore).EnableDropShadow(Color.Black);
                            new TextCrawlRenderer(act, levelTransition);
                        });
                        textGroup.HorizontallyStretchedSpacer();
                    });

                    grp.PixelSpacer(32, 32);
                }
            }




            var menu = gameScene.AddActor("Main Menu");
            new BoundingRect(menu, gameScene.camera.ViewportWidth, gameScene.camera.ViewportHeight);
            var menuLayout = new LayoutGroup(menu, Orientation.Horizontal);

            menuLayout.PixelSpacer(400);
            menuLayout.AddBothStretchedElement("Menu Inner", innerGroupActor =>
            {
                var innerGroup = new LayoutGroup(innerGroupActor, Orientation.Vertical);

                innerGroup.PixelSpacer(250);
                var titleActGroup = innerGroup.AddHorizontallyStretchedElement("Title", 100, titleAct =>
                {
                    new BoundedTextRenderer(titleAct, "Oculus Leviathan", MachinaGame.Assets.GetSpriteFont("Roboto-Big"));
                });

                innerGroup.AddHorizontallyStretchedElement("Subtitle", 64, subtitleAct =>
                {
                    // This was written hours before the deadline, it's spaghetti

                    new BoundedTextRenderer(subtitleAct, "By NotExplosive\n\nThis game is played entirely with the mouse\nPress F4 to fullscreen\n\nMade for Ludum Dare 48 in 72 hours\nHOLD Left Mouse Button to begin", MachinaGame.Assets.GetSpriteFont("Roboto"));

                    float mouseHeldTimer = 0;
                    var mouseHeld = false;
                    var totalTimer = 0f;
                    var thresholdMet = false;

                    var hoc = new AdHoc(subtitleAct);

                    hoc.onMouseButton += (MouseButton button, Vector2 pos, ButtonState state) =>
                    {
                        mouseHeld = button == MouseButton.Left && state == ButtonState.Pressed;
                    };

                    hoc.onUpdate += (float dt) =>
                    {
                        if (!thresholdMet)
                        {
                            if (mouseHeld)
                            {
                                mouseHeldTimer += dt;
                            }
                            else
                            {
                                if (mouseHeldTimer > 0)
                                {
                                    mouseHeldTimer -= dt;
                                }
                            }


                            gameScene.camera.Position = new Vector2(0, MathF.Sin(totalTimer) * 32);

                            if (mouseHeldTimer > 3f)
                            {
                                subtitleAct.RemoveComponent<BoundedTextRenderer>();
                                titleActGroup.actor.Destroy();
                                thresholdMet = true;
                            }
                        }
                        else
                        {
                            // if thresholdMet:
                            mouseHeldTimer -= dt;

                            if (mouseHeldTimer < 0)
                            {
                                menu.Destroy();
                                StartGame();
                            }
                        }

                        if (mouseHeldTimer > 0)
                        {
                            gameScene.camera.Zoom = 1 + EaseFuncs.CubicEaseIn(mouseHeldTimer) / 3;
                        }

                        totalTimer += dt;
                    };
                });
            });
            menuLayout.PixelSpacer(100);

            CommandLineArgs.RegisterFlagArg("edit", () =>
            {
                var curveBrush = gameScene.AddActor("CurveBrush", gameScene.camera.ViewportCenter);
                new CurveEditor(curveBrush);
            });

            if (DebugLevel >= DebugLevel.Passive)
            {
                var debug = gameScene.AddActor("Debug");
                new PanAndZoomCamera(debug, Keys.LeftControl);
            }
        }

        public static void SpawnNewFish(Scene gameScene, Vector2 position, Player player, FishStats stats)
        {
            var fishActor = gameScene.AddActor("Fish", position);
            new BubbleSpawner(fishActor, new MinMax<int>(5, 10));
            new TimeAccumulator(fishActor);
            new Fish(fishActor, player.transform, stats);
            new ResetTargetOffsetPeriodically(fishActor);
            new SpawnBubblesRandomly(fishActor);
            new Eatable(fishActor);
            new FishRenderer(fishActor);
            new PlayerTarget(fishActor, player);
        }

        public static void SpawnSeaweed(Scene gameScene, Vector2 position, SeaweedInfo seaweedInfo, LevelTransition levelTransition)
        {
            var yOffset = gameScene.camera.ViewportHeight / 2;
            var x = gameScene.camera.ViewportWidth * seaweedInfo.XPercent;

            var seaweedActor = gameScene.AddActor("Seaweed", new Vector2(x, position.Y + yOffset + seaweedInfo.Length * 2), -MathF.PI / 2 + seaweedInfo.Angle);
            new Seaweed(seaweedActor, seaweedInfo.Length, seaweedInfo.NodeCount, 5, levelTransition);
            new SeaweedRenderer(seaweedActor);
            var tween = new TweenChainComponent(seaweedActor);

            tween.AddLocalMoveTween(new Vector2(seaweedActor.transform.Position.X, position.Y + yOffset), 1, EaseFuncs.QuadraticEaseOut);
        }

        public static void SpawnJellyfish(Scene gameScene, LevelTransition levelTransition)
        {
            var sign = MachinaGame.Random.CleanRandom.NextDouble() < 0.5f ? 1 : 1;
            var x = gameScene.camera.ViewportWidth * 1.5f * sign;
            var jellyFishActor = gameScene.AddActor("Jellyfish", new Vector2(x, levelTransition.transform.Position.Y));
            new BubbleSpawner(jellyFishActor, new Machina.Data.MinMax<int>(5, 7));
            new Fish(jellyFishActor, levelTransition.transform, FishStats.jellyfish);
            new Jellyfish(jellyFishActor, levelTransition);
            new JellyfishRenderer(jellyFishActor);
            new NewTargetOffsetWhenReachesCurrent(jellyFishActor);
        }
    }
}
