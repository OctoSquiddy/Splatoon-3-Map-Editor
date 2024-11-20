using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImGuiNET;

namespace MapStudio.UI
{
    public class DialogHandler
    {
        static string Name = "";

        static Action DialogRender = null;

        static bool open = false;
        static bool isPopupOpen = true;
        static Action<bool> Result;

        static float Width;
        static float Height;

        static ImGuiWindowFlags Flags = ImGuiWindowFlags.None;

        public static void RenderActiveWindows()
        {
            if (DialogRender == null)
                return;

            if (open)
            {
                ImGui.OpenPopup(Name);
                isPopupOpen = true;
                open = false;
            }

            if (!isPopupOpen) ClosePopup(false);

            // Always center this window when appearing
            var center = ImGui.GetMainViewport().GetCenter();
            ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new System.Numerics.Vector2(0.5f, 0.5f));
            if (Width != 0 && Height != 0)
                ImGui.SetNextWindowSize(new System.Numerics.Vector2(Width, Height), ImGuiCond.Appearing);

            if (ImGui.BeginPopupModal(Name, ref isPopupOpen, Flags))
            {
                DialogRender?.Invoke();
                ImGui.EndPopup();
            }
        }

        public static void ShowException(Exception ex)
        {
            string message = ex.Message.Replace("'", "");

            Clipboard.SetText($"{ex.Message} \n{ex.StackTrace}");
            TinyFileDialog.MessageBoxErrorOk($"{message} Details copied to clipboard!");
        }
        public static void Show(string name, float width, float height, Action dialogRender, Action<bool> dialogResult)
        {
            Width = width;
            Height = height;
            Name = name;
            DialogRender = dialogRender;
            open = true;
            Result = dialogResult;
            Flags = ImGuiWindowFlags.None;
        }

        public static void Show(string name, Action dialogRender, Action<bool> dialogResult)
        {
            //unconfigured. Defaults to ini settings
            Width = 0;
            Height = 0;
            Name = name;
            DialogRender = dialogRender;
            open = true;
            Result = dialogResult;
            Flags = ImGuiWindowFlags.None;
        }

        //Todo this freezes
        /*  public static bool Show(string name, Action dialogRender) {
              return Task.Run(() => ShowAsync(name, dialogRender)).Result;
          }

          public static async Task<bool> ShowAsync(string name, Action dialogRender)
          {
              Name = name;
              DialogRender = dialogRender;
              open = true;

              var tcs = new TaskCompletionSource<bool>();
              Result = (e) => { tcs.TrySetResult(e); };

              return await tcs.Task.ConfigureAwait(false);
          }*/

        public static void ClosePopup(bool isOk)
        {
            DialogRender = null;
            ImGui.CloseCurrentPopup();

            Result?.Invoke(isOk);
            Result = null;
        }

        public static void ClosePopup()
        {
            DialogRender = null;
            ImGui.CloseCurrentPopup();
        }

        public static void FinishTask(bool isOk)
        {
            Result?.Invoke(isOk);
            Result = null;
        }
    }
}
