using LD48.Components;
using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

            var eye = gameScene.AddActor("Eye", new Vector2(gameScene.camera.ViewportCenter.X, 256));
            new Player(eye);
            new EyeRenderer(eye);

            CommandLineArgs.RegisterFlagArg("edit", () =>
            {
                var curveBrush = gameScene.AddActor("CurveBrush", gameScene.camera.ViewportCenter);
                new CurveEditor(curveBrush);
            });


            CreateFish(gameScene, new Vector2(300, 300), 5);

            if (DebugLevel >= DebugLevel.Passive)
            {
                var debug = gameScene.AddActor("Debug");
                new PanAndZoomCamera(debug, Keys.LeftControl);
            }
        }

        private void CreateFish(Scene gameScene, Vector2 pos, int size)
        {
            var fishActor = gameScene.AddActor("Fish");
            new TimeAccumulator(fishActor);
            var fish = new Fish(fishActor, size);
            new FishRenderer(fishActor);
            new BubbleSpawner(fishActor, new Machina.Data.MinMax<int>(5, 10));
            fishActor.transform.Position = pos;

            fish.TargetPosition = new Vector2(MachinaGame.Random.CleanRandom.Next(gameScene.camera.ViewportWidth), MachinaGame.Random.CleanRandom.Next(gameScene.camera.ViewportHeight));

        }
    }
}
