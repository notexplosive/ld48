using LD48.Components;
using Machina.Components;
using Machina.Engine;
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

        internal static void SpawnNewFish(Scene gameScene, Vector2 position, Player player, Fish.FishStats stats)
        {
            var fishActor = gameScene.AddActor("Fish", position);
            new BubbleSpawner(fishActor, new Machina.Data.MinMax<int>(5, 10));
            new TimeAccumulator(fishActor);
            var fish = new Fish(fishActor, player.transform, stats);
            new FishRenderer(fishActor);
            new PlayerTarget(fishActor, player);
        }
    }
}
