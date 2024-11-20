using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace MapStudio.UI
{
    /// <summary>
    /// Keeps track of the progress of an operation.
    /// </summary>
    public class ProcessLoading
    {
        /// <summary>
        /// The current instance of the progress loader.
        /// </summary>
        public static ProcessLoading Instance = null;

        /// <summary>
        /// Checks if the progress is currently active.
        /// </summary>
        public bool IsLoading;

        /// <summary>
        /// The current amount of progress being set.
        /// </summary>
        public int ProcessAmount;

        /// <summary>
        /// The total amount of progress to target to.
        /// </summary>
        public int ProcessTotal;

        /// <summary>
        /// The process name to display.
        /// </summary>
        public string ProcessName;

        /// <summary>
        /// An event that updates when the progress has been altered.
        /// </summary>
        public EventHandler OnUpdated;

        public string Title { get; set; }

        public ProcessLoading() {
            Instance = this;
        }

        public void UpdateIncrease(int amount, string process)
        {
            ProcessAmount += amount;
            ProcessName = process;
            OnUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void Update(int amount, int total, string process, string title = "Loading")
        {
            Title = title;
            ProcessAmount = amount;
            ProcessTotal = total;
            ProcessName = process;
            OnUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void Draw(int w, int h)
        {
            if (!IsLoading)
                return;

            //Show center view
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(w * 0.5f, h * 0.5f),
                ImGuiCond.Always, new System.Numerics.Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(300, 68));

            var flags = ImGuiWindowFlags.AlwaysAutoResize;

            if (ImGui.Begin(this.Title, ref this.IsLoading, flags))
            {
                float progress = (float)this.ProcessAmount / this.ProcessTotal;
                ImGui.ProgressBar(progress, new System.Numerics.Vector2(300, 20));

                ImGuiHelper.DrawCenteredText($"{this.ProcessName}");
            }
            ImGui.End();
        }
    }
}
