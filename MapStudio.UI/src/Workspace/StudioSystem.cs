using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLFrameworkEngine;

namespace MapStudio.UI
{
    public class StudioSystem
    {
        /// <summary>
        /// The active instance of the studio system.
        /// </summary>
        public static StudioSystem Instance;

        /// <summary>
        /// A list of actors in the scene.
        /// </summary>
        public List<ActorBase> Actors = new List<ActorBase>();

        /// <summary>
        /// Determines if the game timer is playing or not.
        /// </summary>
        public bool IsPlaying => Timer.IsPlaying;

        private GameTimer Timer = new GameTimer();

        public StudioSystem() {
            Timer.OnFrameUpdate += OnFrameUpdate;
        }

        /// <summary>
        /// Runs the current scene during the calculation loop.
        /// </summary>
        public void Run() { Timer.Play(); }

        /// <summary>
        /// Pauses all current scene calculations.
        /// </summary>
        public void Pause() { Timer.Pause(); }

        public void AddActor(ActorBase actor)
        {
            if (Actors.Contains(actor)) //Actor already added
                return;

            actor.CreateIdx = Actors.Count;
            actor.Age = 0;
            Actors.Add(actor);
        }

        public void RemoveActor(ActorBase actor)
        {
            if (!Actors.Contains(actor)) //Actor already removed
                return;

            Actors.Remove(actor);
            actor.CreateIdx = -1;
        }

        private bool updating = false;

        private void OnFrameUpdate(object sender, EventArgs e)
        {
            if (updating) return;

            updating = true;

            //Make sure the list is set as another list to prevent conflicts if the actor list is updated
            var actors = Actors.ToList();

            Begin(actors);
            CalculateScene(actors);

            updating = false;
        }

        private void Begin(List<ActorBase> actors)
        {
            //Reset the default settings
            for (int i = 0; i < actors.Count; i++)
                actors[i].BeginFrame();
        }

        /// <summary>
        /// Calculates the current scene and is calculated each frame.
        /// </summary>
        public void CalculateScene(List<ActorBase> actors)
        {
            //Force update the viewport to update the render cache
            GLContext.ActiveContext.UpdateViewport = true;
            AnimationStats.Reset();

            foreach (var actor in actors)
                actor.Calc();

            //Make sure to update the window directly during scene calculation.
            //This is due to these being calculated on another thread so it is better to store them in the window.
            MapStudio.UI.StatisticsWindow.SkeletalAnims = AnimationStats.SkeletalAnims;
            MapStudio.UI.StatisticsWindow.MaterialAnims = AnimationStats.MaterialAnims;
        }

        public void Dispose()
        {
            this.Timer?.Dispose();
            foreach (var actor in Actors)
                actor.Dispose();
        }
    }
}
