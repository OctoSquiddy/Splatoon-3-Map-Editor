using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Core.Animations;
using UIFramework;

namespace MapStudio.UI
{
    public class AnimationProperties : Window
    {
        public override string Name => "Animation Properties";

        public object SelectedObject;

        public EventHandler PropertyChanged;

        public override void Render()
        {
            if (SelectedObject is AnimationTree.TrackNode)
                DrawTrackUI((AnimationTree.TrackNode)SelectedObject);
        }

        private void DrawAnimUI()
        {

        }

        private void DrawGroupUI()
        {

        }

        private void DrawTrackUI(AnimationTree.TrackNode trackNode)
        {
            ImguiBinder.LoadProperties(trackNode.Track, (sender, e) => {

                var handler = (ImguiBinder.PropertyChangedCustomArgs)e;
                //Apply the property
                handler.PropertyInfo.SetValue(handler.Object, sender);

                PropertyChanged?.Invoke(sender, e);
            });

            trackNode.RenderKeyTableUI();
        }
    }
}
