using LD48.Components;
using LD48.Data;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace LD48
{
    public class Game1 : MachinaGame
    {
        public Game1(string[] args) : base("LD48", args, new Point(1600, 900), new Point(1600, 900), ResizeBehavior.MaintainDesiredResolution)
        {

        }

        protected override void OnGameLoad()
        {
            var bgScene = SceneLayers.AddNewScene();
            var gameScene = SceneLayers.AddNewScene();
            var uiScene = SceneLayers.AddNewScene();
            // SceneLayers.BackgroundColor = Color.Black;


            var bgActor = bgScene.AddActor("Background");
            new BackgroundRenderer(bgActor, gameScene.camera);



            var harness = gameScene.AddActor("Harness", new Vector2(gameScene.camera.ViewportCenter.X, -256));
            var levelTransition = new LevelTransition(harness);
            var player = new Player(harness);
            new BubbleSpawner(harness, new MinMax<int>(3, 7));
            new HarnessRenderer(harness);

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
