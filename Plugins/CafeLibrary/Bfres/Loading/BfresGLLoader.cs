﻿using System;
using System.Collections.Generic;
using System.Linq;
using GLFrameworkEngine;

using Syroot.NintenTools.NSW.Bfres;
using Syroot.NintenTools.NSW.Bfres.Helpers;

//using BfresLibrary;
//using BfresLibrary.Helpers;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using Toolbox.Core.IO;

namespace CafeLibrary.Rendering
{
    public class BfresGLLoader
    {
        public static List<VaoAttribute> LoadAttributes(Model mdl, Shape shape, Material material)
        {
            var vertexBuffer = mdl.VertexBuffers[shape.VertexBufferIndex];

            List<VaoAttribute> attributes = new List<VaoAttribute>();

            int offset = 0;
            foreach (VertexAttrib att in vertexBuffer.Attributes)
            {
                if (!ElementCountLookup.ContainsKey(att.Name.Remove(2)))
                    continue;

                bool assigned = false;
                int stride = 0;

                var assign = material.ShaderAssign;
                if (assign.AttribAssigns.Count == 0)
                {
                    VaoAttribute vaoAtt = new VaoAttribute();
                    vaoAtt.vertexAttributeName = att.Name;
                    vaoAtt.name = att.Name;
                    vaoAtt.ElementCount = ElementCountLookup[att.Name.Remove(2)];
                    vaoAtt.Assigned = assigned;
                    vaoAtt.Offset = offset;

                    if (att.Name.Contains("_i"))
                        vaoAtt.Type = VertexAttribPointerType.Int;
                    else
                        vaoAtt.Type = VertexAttribPointerType.Float;

                    attributes.Add(vaoAtt);

                    if (!assigned)
                    {
                        stride = vaoAtt.Stride;
                        assigned = true;
                    }
                }
                else
                {
                    foreach (var matAttribute in assign.AttribAssigns)
                    {
                        if (matAttribute == att.Name)
                        {
                            //Get the translated attribute that is passed to the fragment shader
                            //Models can assign the same attribute to multiple uniforms (ie u0 to u1, u2)
                            string translated = matAttribute;

                            VaoAttribute vaoAtt = new VaoAttribute();
                            vaoAtt.vertexAttributeName = att.Name;
                            vaoAtt.name = translated;
                            vaoAtt.ElementCount = ElementCountLookup[att.Name.Remove(2)];
                            vaoAtt.Assigned = assigned;
                            vaoAtt.Offset = offset;

                            if (att.Name.Contains("_i"))
                                vaoAtt.Type = VertexAttribPointerType.Int;
                            else
                                vaoAtt.Type = VertexAttribPointerType.Float;

                            attributes.Add(vaoAtt);

                            if (!assigned)
                            {
                                stride = vaoAtt.Stride;
                                assigned = true;
                            }
                        }
                    }
                }

                offset += stride;
            }

            return attributes;
        }

        public static byte[] LoadIndexBufferData(Shape shape)
        {
            var mem = new System.IO.MemoryStream();
            using (var writer = new Toolbox.Core.IO.FileWriter(mem))
            {
                foreach (var mesh in shape.Meshes) {
                    var lodFaces = mesh.GetIndices().ToArray();
                    for (int i = 0; i < lodFaces.Length; i++)
                    {
                        switch (mesh.IndexFormat)
                        {
                            case Syroot.NintenTools.NSW.Bfres.GFX.IndexFormat.UInt16:
                                writer.Write((ushort)(lodFaces[i] + mesh.FirstVertex));
                                break;
                            default:
                                writer.Write((uint)(lodFaces[i] + mesh.FirstVertex));
                                break;
                        }
                    }
                }

            }
            return mem.ToArray();
        }

        public static byte[] LoadBufferData(ResFile resFile, Model mdl, Shape shape, List<VaoAttribute> attributes)
        {
            //Create a buffer instance which stores all the buffer data
            VertexBufferHelper helper = new VertexBufferHelper(
                mdl.VertexBuffers[shape.VertexBufferIndex], resFile.ByteOrder);

            //Fill a byte array of data
            int vertexCount = helper.Attributes.FirstOrDefault().Data.Length;

            var mem = new System.IO.MemoryStream();
            using (var writer = new Toolbox.Core.IO.FileWriter(mem))
            {
                var strideTotal = attributes.Sum(x => x.Stride);
                for (int i = 0; i < vertexCount; i++)
                {
                    foreach (var attribute in attributes)
                    {
                        var att = helper.Attributes.FirstOrDefault(x => x.Name == attribute.vertexAttributeName);
                        if (att == null)
                            continue;

                        writer.SeekBegin(attribute.Offset + (i * strideTotal));
                        for (int j = 0; j < attribute.ElementCount; j++)
                        {
                            if (attribute.vertexAttributeName == "_i0" && mdl.Skeleton.MatrixToBoneList?.Count > (int)att.Data[i][j])
                                writer.Write((int)mdl.Skeleton.MatrixToBoneList[(int)att.Data[i][j]]);
                            else if (attribute.vertexAttributeName == "_p0")
                                writer.Write(att.Data[i][j] * GLContext.PreviewScale);
                            else
                                writer.Write(att.Data[i][j]);
                        }
                    }
                }
            }
            return mem.ToArray();
        }

        static Dictionary<string, int> ElementCountLookup = new Dictionary<string, int>()
        {
            { "_u", 2 },
            { "_p", 3 },
            { "_n", 3 },
            { "_t", 4 },
            { "_b", 4 },
            { "_c", 4 },
            { "_w", 4 },
            { "_i", 4 },
        };

        public class VaoAttribute
        {
            public string name;
            public string vertexAttributeName;
            public VertexAttribPointerType Type;
            public int ElementCount;

            public int Offset;

            public bool Assigned;

            public int Stride
            {
                get { return Assigned ? 0 : ElementCount * FormatSize(); }
            }

            public string UniformName
            {
                get
                {
                    switch (name)
                    {
                        case "_p0": return GLConstants.VPosition;
                        case "_n0": return GLConstants.VNormal;
                        case "_w0": return GLConstants.VBoneWeight;
                        case "_i0": return GLConstants.VBoneIndex;
                        case "_u0": return GLConstants.VTexCoord0;
                        case "_u1": return GLConstants.VTexCoord1;
                        case "_u2": return GLConstants.VTexCoord2;
                        case "_c0": return GLConstants.VColor;
                        case "_t0": return GLConstants.VTangent;
                        case "_b0": return GLConstants.VBitangent;
                        default:
                            return name;
                    }
                }
            }

            private int FormatSize()
            {
                switch (Type)
                {
                    case VertexAttribPointerType.Float: return sizeof(float);
                    case VertexAttribPointerType.Byte: return sizeof(byte);
                    case VertexAttribPointerType.Double: return sizeof(double);
                    case VertexAttribPointerType.Int: return sizeof(int);
                    case VertexAttribPointerType.Short: return sizeof(short);
                    case VertexAttribPointerType.UnsignedShort: return sizeof(ushort);
                    default: return 0;
                }
            }
        }
    }
}
