using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using GLFrameworkEngine;

namespace MapStudio.UI
{
    public class UVMap 
    {
        #region Draw UVs

        static RenderMesh<Vector2> MeshDrawer;

        private List<Vector2> Points = new List<Vector2>();

        public static void Init()
        {
            if (MeshDrawer == null)
                MeshDrawer = new RenderMesh<Vector2>(new Vector2[0], PrimitiveType.LineLoop);
        }

        public void Reset() {
            Points.Clear();
        }

        public void Draw(UVViewport.Camera2D camera, Vector2 scale)
        {
            GL.Disable(EnableCap.CullFace);

            var shader = GlobalShaders.GetShader("UV_WINDOW");
            shader.Enable();

            var cameraMtx = camera.ViewMatrix * camera.ProjectionMatrix;

            shader.SetMatrix4x4("mtxCam", ref cameraMtx);

            shader.SetFloat("brightness", 1.0f);
            shader.SetInt("hasTexture", 0);
            shader.SetVector2("scale", scale);
            shader.SetVector4("uColor", ColorUtility.ToVector4(Runtime.UVEditor.UVColor));

            MeshDrawer.Draw(shader);

            GL.Enable(EnableCap.CullFace);
        }

        public void UpdateVertexBuffer(int PolygonGroupIndex, int UvChannelIndex, List<STGenericMesh> genericObjects, STGenericTextureMap textureMap)
        {
            Init();

            Points.Clear();

            if (genericObjects.Count == 0) return;

            foreach (var genericObject in genericObjects)
            {
                List<uint> f = new List<uint>();
                int displayFaceSize = 0;
                if (genericObject.PolygonGroups.Count > 0)
                {
                    if (PolygonGroupIndex == -1)
                    {
                        foreach (var group in genericObject.PolygonGroups)
                        {
                            f.AddRange(group.Faces);
                            displayFaceSize += group.Faces.Count;
                        }
                    }
                    else
                    {
                        if (genericObject.PolygonGroups.Count > PolygonGroupIndex)
                        {
                            f = genericObject.PolygonGroups[PolygonGroupIndex].Faces;
                            displayFaceSize = genericObject.PolygonGroups[PolygonGroupIndex].Faces.Count;
                        }
                    }
                }

                if (genericObject.Vertices.Count == 0 ||
                    genericObject.Vertices[0].TexCoords.Length == 0)
                    return;

                for (int v = 0; v < displayFaceSize; v += 3)
                {
                    if (displayFaceSize < 3 || genericObject.Vertices.Count < 3)
                        return;

                    if (f.Count <= v + 2)
                        continue;

                    if (genericObject.Vertices.Count > f[v + 2])
                    {
                        if (genericObject.Vertices[(int)f[v]].TexCoords.Length <= UvChannelIndex)
                            continue;

                        Vector2 v1 = genericObject.Vertices[(int)f[v]].TexCoords[UvChannelIndex];
                        Vector2 v2 = genericObject.Vertices[(int)f[v + 1]].TexCoords[UvChannelIndex];
                        Vector2 v3 = genericObject.Vertices[(int)f[v + 2]].TexCoords[UvChannelIndex];

                        v1 = new Vector2(v1.X, v1.Y);
                        v2 = new Vector2(v2.X, v2.Y);
                        v3 = new Vector2(v3.X, v3.Y);

                        AddUVTriangle(v1, v2, v3, textureMap);
                    }
                }
            }
            MeshDrawer.UpdateVertexData(Points.ToArray());
        }

        private void AddUVTriangle(Vector2 v1, Vector2 v2, Vector2 v3, STGenericTextureMap textureMap)
        {
            Vector2 scaleUv = new Vector2(2);
            Vector2 transUv = new Vector2(-1f);

            if (textureMap != null && textureMap.Transform != null)
            {
                scaleUv *= textureMap.Transform.Scale;
                transUv += textureMap.Transform.Translate;
            }
            Points.AddRange(TransformUVTriangle(v1, v2, v3, scaleUv, transUv));
        }

        private static List<Vector2> TransformUVTriangle(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 scaleUv, Vector2 transUv)
        {
            List<Vector2> points = new List<Vector2>();
            points.Add(v1 * scaleUv + transUv);
            points.Add(v2 * scaleUv + transUv);

            points.Add(v2 * scaleUv + transUv);
            points.Add(v3 * scaleUv + transUv);

            points.Add(v3 * scaleUv + transUv);
            points.Add(v1 * scaleUv + transUv);
            return points;
        }

        #endregion

    }
}
