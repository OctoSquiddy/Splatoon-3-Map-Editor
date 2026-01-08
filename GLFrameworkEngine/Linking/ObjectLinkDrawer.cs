using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Toolbox.Core;

namespace GLFrameworkEngine
{
    public class ObjectLinkDrawer
    {
        RenderMesh<LinkVertex> LineRender;
        StandardMaterial LineMaterial = new StandardMaterial();

        //View colors (default - blue to yellow gradient)
        Vector4 SrcColor_View = new Vector4(0, 0, 1, 1);
        Vector4 DestColor_View = new Vector4(1, 1, 0, 1);
        //Editing colors
        Vector4 SrcColor_Edit = new Vector4(0, 1, 0, 1);
        Vector4 DestColor_Edit = new Vector4(1, 0, 0, 1);

        const float LINE_WIDTH = 5;

        /// <summary>
        /// Callback to get custom colors for links based on source object and destination index.
        /// Parameters: (sourceObject, destObject, destIndex) -> (srcColor, destColor)
        /// If null or returns null, default colors are used.
        /// </summary>
        public static Func<IObjectLink, ITransformableObject, int, (Vector4 srcColor, Vector4 destColor)?> GetLinkColorCallback { get; set; }

        /// <summary>
        /// When true, all links are always drawn regardless of selection state.
        /// This is separate from context.LinkingTools.DisplayAllLinks.
        /// </summary>
        public static bool AlwaysShowAllLinks { get; set; } = false;

        public void DrawPicking(GLContext context)
        {

        }

        public void Draw(GLContext context)
        {
            if (LineRender == null)
                LineRender = new RenderMesh<LinkVertex>(new LinkVertex[0], PrimitiveType.LineLoop);

            LineMaterial.hasVertexColors = true;
            LineMaterial.Render(context);

            var objects = context.Scene.GetSelectableObjects();
            foreach (var obj in objects) {
                //Connect references from IObjectLink types
                if (obj is IObjectLink)
                    DrawLinks(context, (IObjectLink)obj);
            }
        }

        private void DrawLinks(GLContext context, IObjectLink obj)
        {
            if (obj is IDrawable && !((IDrawable)obj).IsVisible)
                return;

            //Connect from the current source of the object.
            var sourcePos = ((ITransformableObject)obj).Transform.Position;
            List<LinkVertex> points = new List<LinkVertex>();

            //Connect to each link reference
            int destIndex = 0;
            foreach (ITransformableObject linkedObj in obj.DestObjectLinks) {
                if (((ITransformableObject)obj).IsSelected || linkedObj.IsSelected || context.LinkingTools.DisplayAllLinks || AlwaysShowAllLinks)
                {
                    var destPos = linkedObj.Transform.Position;

                    // Try to get custom colors from callback
                    Vector4 srcColor = SrcColor_View;
                    Vector4 destColor = DestColor_View;

                    if (GetLinkColorCallback != null)
                    {
                        var customColors = GetLinkColorCallback(obj, linkedObj, destIndex);
                        if (customColors.HasValue)
                        {
                            srcColor = customColors.Value.srcColor;
                            destColor = customColors.Value.destColor;
                        }
                    }

                    //2 types of colors, edit mode and link view
                    bool editMode = false;
                    if (editMode) {
                        points.Add(new LinkVertex(sourcePos, SrcColor_Edit));
                        points.Add(new LinkVertex(destPos, DestColor_Edit));
                    }
                    else {
                        // Draw line with embedded arrow markers
                        AddLineWithArrows(points, sourcePos, destPos, srcColor, destColor);
                    }
                }
                destIndex++;
            }

            if (points.Count > 0) {
                GL.LineWidth(LINE_WIDTH);
                LineRender.UpdateVertexData(points.ToArray(), BufferUsageHint.DynamicDraw);
                LineRender.Draw(context);
                GL.LineWidth(1);
            }
        }

        /// <summary>
        /// Draws the main line with arrow markers embedded in it
        /// The line goes: source -> wing1 -> tip -> wing2 -> ... -> dest
        /// </summary>
        private void AddLineWithArrows(List<LinkVertex> points, Vector3 sourcePos, Vector3 destPos, Vector4 srcColor, Vector4 destColor)
        {
            Vector3 direction = destPos - sourcePos;
            float length = direction.Length;
            if (length < 0.001f)
            {
                points.Add(new LinkVertex(sourcePos, srcColor));
                points.Add(new LinkVertex(destPos, destColor));
                return;
            }

            direction.Normalize();

            // Arrow marker settings
            float arrowSpacing = 100f;
            float arrowSize = 12f;

            // Calculate perpendicular vector for arrow wings
            Vector3 up = Math.Abs(direction.Y) < 0.9f ? Vector3.UnitY : Vector3.UnitX;
            Vector3 right = Vector3.Cross(direction, up);
            right.Normalize();

            // Calculate number of arrows
            int numArrows = Math.Max(1, (int)(length / arrowSpacing));

            // Build list of arrow t-positions
            List<float> arrowTs = new List<float>();
            for (int i = 1; i <= numArrows; i++)
            {
                arrowTs.Add((float)i / (numArrows + 1));
            }

            // Current position along the line
            Vector3 currentPos = sourcePos;
            float currentT = 0f;

            foreach (float arrowT in arrowTs)
            {
                // Point just before the arrow
                Vector3 preArrow = sourcePos + (destPos - sourcePos) * arrowT - direction * arrowSize;
                Vector4 preColor = Vector4.Lerp(srcColor, destColor, arrowT - arrowSize / length);

                // Line segment to pre-arrow point
                points.Add(new LinkVertex(currentPos, Vector4.Lerp(srcColor, destColor, currentT)));
                points.Add(new LinkVertex(preArrow, preColor));

                // Arrow tip position
                Vector3 arrowTip = sourcePos + (destPos - sourcePos) * arrowT;
                Vector4 arrowColor = Vector4.Lerp(srcColor, destColor, arrowT);

                // Wing positions
                Vector3 wing1 = preArrow + right * arrowSize * 0.5f;
                Vector3 wing2 = preArrow - right * arrowSize * 0.5f;

                // Draw the arrow shape: wing1 -> tip -> wing2 (simple V shape)

                // Wing1 to tip
                points.Add(new LinkVertex(wing1, arrowColor));
                points.Add(new LinkVertex(arrowTip, arrowColor));

                // Tip to wing2
                points.Add(new LinkVertex(arrowTip, arrowColor));
                points.Add(new LinkVertex(wing2, arrowColor));

                currentPos = arrowTip;
                currentT = arrowT;
            }

            // Final segment to destination
            points.Add(new LinkVertex(currentPos, Vector4.Lerp(srcColor, destColor, currentT)));
            points.Add(new LinkVertex(destPos, destColor));
        }

        class LinkDrawer
        {
            public Vector3 SourcePos;
            public Vector3 DestPos;

            public void Draw()
            {

            }
        }

        struct LinkVertex
        {
            [RenderAttribute("vPosition", VertexAttribPointerType.Float, 0)]
            public Vector3 Position;

            [RenderAttribute("vColor", VertexAttribPointerType.Float, 12)]
            public Vector4 vColor;

            public LinkVertex(Vector3 position, Vector4 color)
            {
                Position = position;
                vColor = color;
            }
        }
    }
}
