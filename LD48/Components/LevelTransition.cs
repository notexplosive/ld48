using LD48.Data;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD48.Components
{
    public class LevelTransition : BaseComponent
    {
        public Input input;
        private readonly TweenChain levelTransitionTween;
        private bool readyToStartLevel;
        public bool IsInLevelTransition => !this.levelTransitionTween.IsFinished || Asleep;
        private int levelIndex = 0;
        public Level CurrentLevel => this.levelIndex > 0 ? Level.All[this.levelIndex - 1] : Level.All[this.levelIndex];
        public TextCrawl CurrentTextCrawl
        {
            get; private set;
        }

        public bool Asleep
        {
            get; set;
        } = true;
        public Vector2 Velocity
        {
            get; private set;
        }

        public Action onWakeUp;
        public Action onFallAsleep;
        private SoundEffectInstance ambientSound;

        public LevelTransition(Actor actor, int levelIndex) : base(actor)
        {
            Velocity = Vector2.Zero;
            this.levelTransitionTween = new TweenChain();

            this.actor.scene.StartCoroutine(IntroCinematic(LevelDialogue.IntroSequence));

            this.ambientSound = MachinaGame.Assets.GetSoundEffectInstance("underwater-ambience");
            this.ambientSound.IsLooped = true;
            this.ambientSound.Play();
            this.ambientSound.Volume = 0.5f;

            this.levelIndex = levelIndex;
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (key == Keys.F4 && modifiers.None && state == ButtonState.Released)
            {
                MachinaGame.Fullscreen = !MachinaGame.Fullscreen;
            }
        }

        private IEnumerator<ICoroutineAction> IntroCinematic(string[] strings)
        {
            yield return new WaitSeconds(1);
            yield return new WaitUntil(UntilTextCrawlIsFinished(strings[0]));
            yield return new WaitUntil(UntilTextCrawlIsClosed());

            this.levelTransitionTween.AppendCallback(() => { this.input.downward = true; });
            this.levelTransitionTween.AppendWaitTween(2);
            this.levelTransitionTween.AppendCallback(() => { this.input.downward = false; });

            yield return new WaitUntil(() => Velocity.Y == 0);
            this.levelTransitionTween.Clear();

            yield return new WaitSeconds(0.25f);

            for (int i = 1; i < strings.Length; i++)
            {
                yield return new WaitUntil(UntilTextCrawlIsFinished(strings[i]));
                yield return new WaitUntil(UntilTextCrawlIsClosed());
            }

            this.readyToStartLevel = true;
        }

        public float CurrentDepth => (float) this.levelIndex / Level.All.Length * 10034f + 1000f;

        private IEnumerator<ICoroutineAction> BetweenLevelDialogue(string text)
        {
            yield return new WaitUntil(() => Velocity.Y == 0);
            yield return new WaitSeconds(0.25f);
            yield return new WaitUntil(UntilTextCrawlIsFinished(string.Format("Depth: {0} meters.\n", CurrentDepth.ToString("n2")) + text));
            yield return new WaitUntil(UntilTextCrawlIsClosed());
            this.readyToStartLevel = true;
        }

        public IEnumerator<ICoroutineAction> EndingSequence()
        {
            foreach (var text in LevelDialogue.Ending)
            {
                yield return new WaitUntil(UntilTextCrawlIsFinished(text));
                yield return new WaitUntil(UntilTextCrawlIsClosed());

                this.input.upward = true;
            }

            MachinaGame.Quit();
        }

        private Func<bool> UntilTextCrawlIsClosed()
        {
            return () => CurrentTextCrawl == null;
        }

        private Func<bool> UntilTextCrawlIsFinished(string text)
        {
            CurrentTextCrawl = new TextCrawl(text);
            return () => CurrentTextCrawl.IsFinished;
        }

        public override void Update(float dt)
        {
            this.ambientSound.Pitch = this.actor.scene.TimeScale - 1;

            this.levelTransitionTween.Update(dt);
            ProcessInput(dt);
            transform.Position += Velocity * dt * 60;

            if (this.readyToStartLevel && Velocity.Y == 0)
            {
                this.readyToStartLevel = false;
                StartNextLevel();
            }

            CurrentTextCrawl?.Update(dt);
        }

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState state)
        {
            if (state == ButtonState.Pressed)
            {
                if (CurrentTextCrawl != null)
                {
                    if (!CurrentTextCrawl.IsFinished)
                    {
                        CurrentTextCrawl.SkipToEnd();
                    }
                    else
                    {
                        CurrentTextCrawl = null;
                    }
                }
            }
        }

        private void ProcessInput(float dt)
        {
            var y = Velocity.Y;
            if (this.input.upward)
            {
                y -= dt * 5;
            }

            if (this.input.downward)
            {
                y += dt * 5;
            }

            if (this.input.None)
            {
                if (y > 0)
                {
                    y -= dt * 5;
                }

                if (y < 0)
                {
                    y += dt * 5;
                }

                if (Math.Abs(y) < dt * 5)
                {
                    y = 0;
                }
            }

            Velocity = new Vector2(0, y);
        }

        public void FinishLevel(int duration)
        {
            this.levelTransitionTween.Clear();
            this.levelTransitionTween.AppendCallback(() => { FallAsleep(); });
            this.levelTransitionTween.AppendWaitTween(3);
            this.levelTransitionTween.AppendCallback(() => { this.input.downward = true; });
            this.levelTransitionTween.AppendWaitTween(duration);
            this.levelTransitionTween.AppendCallback(() => { this.input.downward = false; });
            this.levelTransitionTween.AppendCallback(() =>
            {
                var dialogueIndex = this.levelIndex - 1;
                if (dialogueIndex < LevelDialogue.ForLevels.Length)
                {
                    this.actor.scene.StartCoroutine(BetweenLevelDialogue(LevelDialogue.ForLevels[dialogueIndex]));
                }
                else
                {
                    this.readyToStartLevel = true;
                }
            });
        }

        public void StartNextLevel()
        {
            var levels = Level.All;
            MachinaGame.Print("Starting level", this.levelIndex, levels.Length);

            if (levels.Length > this.levelIndex)
            {
                var currentLevel = levels[this.levelIndex];

                for (int i = 0; i < currentLevel.FishCount; i++)
                {
                    var camWidth = this.actor.scene.camera.ViewportWidth;
                    float randomX = MachinaGame.Random.CleanRandom.Next(camWidth / 2, camWidth) * ((MachinaGame.Random.CleanRandom.NextDouble() < 0.5) ? -1f : 1f);
                    var fishPos = new Vector2(randomX, 0);

                    Game1.SpawnNewFish(
                        this.actor.scene, transform.Position + fishPos, RequireComponent<Player>(), currentLevel.FishStats);
                }

                foreach (var seaweedInfo in currentLevel.Seaweed)
                {
                    Game1.SpawnSeaweed(this.actor.scene, transform.Position, seaweedInfo, this);
                }

                for (int i = 0; i < currentLevel.JellyfishCount; i++)
                {
                    Game1.SpawnJellyfish(this.actor.scene, this);
                }
                this.levelIndex++;
            }
            else
            {
                MachinaGame.Print("Finished!");
            }

            WakeUp();
        }

        private void WakeUp()
        {
            //Asleep = false;
            this.onWakeUp?.Invoke();
        }

        private void FallAsleep()
        {
            //Asleep = true;
            this.onFallAsleep?.Invoke();
        }

        public struct Input
        {
            public bool upward;
            public bool downward;

            public bool None => !upward && !downward;
        }
    }
}
