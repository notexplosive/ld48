using LD48.Components;
using LD48.Data;
using Machina.Components;
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
            SceneLayers.BackgroundColor = Color.Black;

            var bgActor = bgScene.AddActor("Background");
            new BackgroundRenderer(bgActor, gameScene.camera);

            var eye = gameScene.AddActor("Eye", new Vector2(gameScene.camera.ViewportCenter.X, -256));
            var player = new Player(eye);
            var eyeRenderer = new EyeRenderer(eye);

            var targetReticalActor = gameScene.AddActor("Redical");
            targetReticalActor.transform.Depth -= 20;
            new TargetRedical(targetReticalActor, player);


            var jellyFishActor = gameScene.AddActor("Jellyfish", new Vector2(200, 200));
            new BubbleSpawner(jellyFishActor, new Machina.Data.MinMax<int>(5, 7));
            new Fish(jellyFishActor, player.transform, FishStats.jellyfish);
            new Jellyfish(jellyFishActor, player);
            new JellyfishRenderer(jellyFishActor);


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
            new BubbleSpawner(fishActor, new Machina.Data.MinMax<int>(5, 10));
            new TimeAccumulator(fishActor);
            new Fish(fishActor, player.transform, stats);
            new Eatable(fishActor);
            new FishRenderer(fishActor);
            new PlayerTarget(fishActor, player);
        }

        public static void SpawnSeaweed(Scene gameScene, Vector2 position, SeaweedInfo seaweedInfo)
        {
            var yOffset = gameScene.camera.ViewportHeight / 2;
            var x = gameScene.camera.ViewportWidth * seaweedInfo.XPercent;
            MachinaGame.Print(seaweedInfo.XPercent, x);

            var seaweedActor = gameScene.AddActor("Seaweed", new Vector2(x, position.Y + yOffset + seaweedInfo.Length * 2), -MathF.PI / 2 + seaweedInfo.Angle);
            new Seaweed(seaweedActor, seaweedInfo.Length, seaweedInfo.NodeCount, 5);
            new SeaweedRenderer(seaweedActor);
            var tween = new TweenChainComponent(seaweedActor);

            tween.AddLocalMoveTween(new Vector2(seaweedActor.transform.Position.X, position.Y + yOffset), 1, EaseFuncs.QuadraticEaseOut);
        }
    }
}
