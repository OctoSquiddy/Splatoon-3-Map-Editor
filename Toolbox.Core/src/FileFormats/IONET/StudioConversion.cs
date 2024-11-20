using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core.Animations;
using Toolbox.Core;
using IONET.Core.Model;
using IONET.Core.Skeleton;
using IONET.Core.Animation;
using IONET.Core;
using IONET;

namespace IONET.Helpers
{
    public class StudioConversion
    {
        public static IOAnimation FromSTAnimatiom(STAnimation anim)
        {
            IOAnimation animation = new IOAnimation();
            animation.Name = anim.Name;

            //Setup each group
            foreach (STBoneAnimGroup group in anim.AnimGroups)
                animation.Groups.Add(ConvertSTGroup(group));
            return animation;
        }

        public static STAnimation ToStudioAnimatiom(IOAnimation anim)
        {
            STAnimation animation = new STAnimation();
            animation.Name = anim.Name;
            animation.FrameCount = anim.GetFrameCount();
            //Setup each group
            foreach (IOAnimation group in anim.Groups)
                animation.AnimGroups.Add(ConvertIOGroup(group));
            return animation;
        }

        static STAnimGroup ConvertIOGroup(IOAnimation animation)
        {
            STBoneAnimGroup group = new STBoneAnimGroup();
            if (!animation.Name.Contains("Root"))
            group.UseSegmentScaleCompensate = true;
            group.Name = animation.Name;
            foreach (var track in animation.Tracks)
            {
                switch (track.ChannelType)
                {
                    case IOAnimationTrackType.PositionX: ConvertIOTrack(group.TranslateX, track); break;
                    case IOAnimationTrackType.PositionY: ConvertIOTrack(group.TranslateY, track); break;
                    case IOAnimationTrackType.PositionZ: ConvertIOTrack(group.TranslateZ, track); break;
                    case IOAnimationTrackType.RotationEulerX: ConvertIOTrack(group.RotateX, track); break;
                    case IOAnimationTrackType.RotationEulerY: ConvertIOTrack(group.RotateY, track); break;
                    case IOAnimationTrackType.RotationEulerZ: ConvertIOTrack(group.RotateZ, track); break;
                    case IOAnimationTrackType.ScaleX: ConvertIOTrack(group.ScaleX, track); break;
                    case IOAnimationTrackType.ScaleY: ConvertIOTrack(group.ScaleY, track); break;
                    case IOAnimationTrackType.ScaleZ: ConvertIOTrack(group.ScaleZ, track); break;
                }
            }

            return group;
        }

        static IOAnimation ConvertSTGroup(STBoneAnimGroup group)
        {
            IOAnimation animation = new IOAnimation();
            animation.Name = group.Name;

            List<IOAnimationTrack> tracks = new List<IOAnimationTrack>();
            tracks.Add(ConvertTrack(group.TranslateX, IOAnimationTrackType.PositionX));
            tracks.Add(ConvertTrack(group.TranslateY, IOAnimationTrackType.PositionY));
            tracks.Add(ConvertTrack(group.TranslateZ, IOAnimationTrackType.PositionZ));

            if (group.UseQuaternion)
            {
                tracks.Add(ConvertTrack(group.RotateX, IOAnimationTrackType.QuatX));
                tracks.Add(ConvertTrack(group.RotateY, IOAnimationTrackType.QuatY));
                tracks.Add(ConvertTrack(group.RotateZ, IOAnimationTrackType.QuatZ));
                tracks.Add(ConvertTrack(group.RotateW, IOAnimationTrackType.QuatW));
            }
            else
            {
                tracks.Add(ConvertTrack(group.RotateX, IOAnimationTrackType.RotationEulerX));
                tracks.Add(ConvertTrack(group.RotateY, IOAnimationTrackType.RotationEulerY));
                tracks.Add(ConvertTrack(group.RotateZ, IOAnimationTrackType.RotationEulerZ));
            }

            tracks.Add(ConvertTrack(group.ScaleX, IOAnimationTrackType.ScaleX));
            tracks.Add(ConvertTrack(group.ScaleY, IOAnimationTrackType.ScaleY));
            tracks.Add(ConvertTrack(group.ScaleZ, IOAnimationTrackType.ScaleZ));

            foreach (var track in tracks) {
                if (track.KeyFrames.Count > 0)
                    animation.Tracks.Add(track);
            }
            return animation;
        }

        static IOAnimationTrack ConvertTrack(STAnimationTrack track, IOAnimationTrackType type)
        {
            var iotrack = new IOAnimationTrack()
            {
                ChannelType = type,
            };

            foreach (var keyFrame in track.KeyFrames)
            {
                IOKeyFrame kf = new IOKeyFrame();
                float multiplier = 1.0f;
                if (type == IOAnimationTrackType.RotationEulerX ||
                    type == IOAnimationTrackType.RotationEulerY || 
                    type == IOAnimationTrackType.RotationEulerZ)
                {
                    multiplier = STMath.Rad2Deg;
                }

                kf.Frame = keyFrame.Frame;
                kf.Value = keyFrame.Value * multiplier;
                if (keyFrame is STHermiteKeyFrame)
                {
                    kf = new IOKeyFrameHermite()
                    {
                        Frame = keyFrame.Frame,
                        Value = keyFrame.Value * multiplier,
                        TangentSlopeInput = ((STHermiteKeyFrame)keyFrame).TangentIn * multiplier,
                        TangentSlopeOutput = ((STHermiteKeyFrame)keyFrame).TangentOut * multiplier,
                    };
                }
                else if (keyFrame is STBezierKeyFrame)
                {
                    kf = new IOKeyFrameHermite()
                    {
                        Frame = keyFrame.Frame,
                        Value = keyFrame.Value * multiplier,
                    };
                }
                iotrack.KeyFrames.Add(kf);
            }
            return iotrack;
        }

        static void ConvertIOTrack(STAnimationTrack sttrack, IOAnimationTrack track)
        {
            if (track.KeyFrames.Any(x => x is IOKeyFrameHermite))
                sttrack.InterpolationType = STInterpoaltionType.Hermite;

            foreach (var keyFrame in track.KeyFrames)
            {
                STKeyFrame kf = new STKeyFrame();
                kf.Frame = keyFrame.Frame;
                kf.Value = (float)keyFrame.Value;

                if (sttrack.InterpolationType == STInterpoaltionType.Hermite)
                {
                    kf = new STHermiteKeyFrame()
                    {
                        Frame = keyFrame.Frame,
                        Value = (float)keyFrame.Value,
                    };
                    if (keyFrame is IOKeyFrameHermite)
                    {
                        ((STHermiteKeyFrame)kf).TangentIn = ((IOKeyFrameHermite)keyFrame).TangentSlopeInput;
                        ((STHermiteKeyFrame)kf).TangentOut = ((IOKeyFrameHermite)keyFrame).TangentSlopeOutput;
                    }
                }
                sttrack.KeyFrames.Add(kf);
            }
        }

        public static STGenericScene ToGeneric(IOScene scene)
        {
            var model = new STGenericModel();
            model.Skeleton = new STSkeleton();

            STGenericScene genericScene = new STGenericScene();
            genericScene.Models.Add(model);

            foreach (var anim in scene.Animations)
            {
                var genericAnim = ToStudioAnimatiom(anim);
                genericScene.Animations.Add(genericAnim);
            }
            Dictionary<string, string> texturePaths = new Dictionary<string, string>();
            foreach (var daeModel in scene.Models)
            {
                model.Name = daeModel.Name;

                var bones = daeModel.Skeleton.BreathFirstOrder();
                foreach (var bone in daeModel.Skeleton.BreathFirstOrder())
                {
                    STBone bn = new STBone(model.Skeleton);
                    bn.Name = bone.Name;
                    bn.ParentIndex = bones.IndexOf(bone.Parent);
                    bn.Position = new OpenTK.Vector3(
                        bone.Translation.X, bone.Translation.Y, bone.Translation.Z);
                    bn.Rotation = new OpenTK.Quaternion(
                        bone.Rotation.X, bone.Rotation.Y, bone.Rotation.Z, bone.Rotation.W);
                    bn.Scale = new OpenTK.Vector3(
                        bone.Scale.X, bone.Scale.Y, bone.Scale.Z);
                    model.Skeleton.Bones.Add(bn);
                }
                model.Skeleton.Reset();

                foreach (var daeMat in scene.Materials)
                {
                    STGenericMaterial mat = new STGenericMaterial();
                    mat.Name = daeMat.Name;
                    model.Materials.Add(mat);

                    if (daeMat.DiffuseMap != null)
                    {
                        string name = daeMat.DiffuseMap.Name;
                        if (name == null)
                            name = System.IO.Path.GetFileNameWithoutExtension(daeMat.DiffuseMap.FilePath);

                        mat.TextureMaps.Add(new STGenericTextureMap()
                        {
                            Name = name,
                            Type = STTextureType.Diffuse,
                        });
                        if (!texturePaths.ContainsKey(daeMat.DiffuseMap.FilePath))
                            texturePaths.Add(daeMat.DiffuseMap.FilePath, name);
                    }
                }

                foreach (var daeMesh in daeModel.Meshes)
                {
                    STGenericMesh msh = new STGenericMesh();
                    msh.Name = daeMesh.Name;
                    model.Meshes.Add(msh);
                    foreach (var daePoly in daeMesh.Polygons)
                    {
                        var poly = new STPolygonGroup();
                        poly.MaterialIndex = model.Materials.FindIndex(x => x.Name == daePoly.MaterialName);
                        if (poly.MaterialIndex != -1)
                            poly.Material = model.Materials[poly.MaterialIndex];
                        poly.Faces = daePoly.Indicies.Select(x => (uint)x).ToList();
                        msh.PolygonGroups.Add(poly);
                    }
                    foreach (var daeVertex in daeMesh.Vertices)
                    {
                        var vert = new STVertex()
                        {
                            Position = ToVec3(daeVertex.Position),
                            Normal = ToVec3(daeVertex.Normal),
                            Tangent = ToVec4(new System.Numerics.Vector4(daeVertex.Tangent, 1)),
                            Bitangent = ToVec4(new System.Numerics.Vector4(daeVertex.Binormal, 1)),
                        };
                        msh.Vertices.Add(vert);
                        vert.TexCoords = new OpenTK.Vector2[daeVertex.UVs.Count];
                        vert.Colors = new OpenTK.Vector4[daeVertex.Colors.Count];
                        for (int i = 0; i < vert.TexCoords.Length; i++)
                            vert.TexCoords[i] = new OpenTK.Vector2(daeVertex.UVs[i].X, 1 - daeVertex.UVs[i].Y);
                        for (int i = 0; i < vert.Colors.Length; i++)
                            vert.Colors[i] = ToVec4(daeVertex.Colors[i]);
                        foreach (var boneWeight in daeVertex.Envelope.Weights)
                        {
                            var index = model.Skeleton.Bones.FindIndex(x => x.Name == boneWeight.BoneName);
                            vert.BoneIndices.Add(index);
                            vert.BoneWeights.Add(boneWeight.Weight);
                        }
                    }
                }
            }

            foreach (var tex in texturePaths)
                if (System.IO.File.Exists(tex.Key))
                    model.Textures.Add(new GenericBitmapTexture(tex.Key) { Name = tex.Value });

            return genericScene;
        }

        public static IOScene FromGeneric(STGenericScene genericScene)
        {
            var scene = new IOScene();
            var daeModel = new IOModel();
            scene.Models.Add(daeModel);

            daeModel.Skeleton = new IOSkeleton();
            foreach (var model in genericScene.Models)
            {
                foreach (var bone in model.Skeleton.Bones)
                {
                    if (bone.ParentIndex != -1)
                        continue;

                    daeModel.Skeleton.RootBones.Add(ConvertBones(bone));
                }
                foreach (var mat in model.Materials)
                {
                    IOMaterial daeMat = new IOMaterial();
                    daeMat.Name = mat.Name;
                    foreach (var tex in mat.TextureMaps)
                    {
                        if (tex.Type == STTextureType.Diffuse)
                        {
                            daeMat.DiffuseMap = new IOTexture();
                            daeMat.DiffuseMap.Name = tex.Name;
                            daeMat.DiffuseMap.FilePath = $"{tex.Name}.png";
                            daeMat.DiffuseMap.UVChannel = 0;
                            daeMat.DiffuseMap.WrapS = WrapMode.REPEAT;
                            daeMat.DiffuseMap.WrapT = WrapMode.REPEAT;
                        }
                    }
                    scene.Materials.Add(daeMat);
                }
                foreach (var mesh in model.Meshes)
                {
                    IOMesh daeMesh = new IOMesh();
                    daeMesh.Name = mesh.Name;
                    daeModel.Meshes.Add(daeMesh);
                    foreach (var poly in mesh.PolygonGroups)
                    {
                        var daePoly = new IOPolygon();
                        daePoly.Indicies = poly.Faces.Select(x => (int)x).ToList();
                        if (poly.MaterialIndex != -1 && scene.Materials.Count > poly.MaterialIndex)
                            daePoly.MaterialName = scene.Materials[poly.MaterialIndex].Name;
                        daePoly.PrimitiveType = IOPrimitive.TRIANGLE;
                        daeMesh.Polygons.Add(daePoly);
                    }
                    foreach (var vertex in mesh.Vertices)
                    {
                        var daeVertex = new IOVertex();
                        daeVertex.Position = ToVec3(vertex.Position);
                        daeVertex.Normal = ToVec3(vertex.Normal);
                        daeVertex.Tangent = ToVec3(vertex.Tangent.Xyz);
                        daeVertex.Binormal = ToVec3(vertex.Bitangent.Xyz);
                        foreach (var color in vertex.Colors)
                            daeVertex.Colors.Add(ToVec4(color));
                        foreach (var texCoord in vertex.TexCoords)
                            daeVertex.UVs.Add(ToVec2(texCoord));
                        for (int i = 0; i < vertex.BoneIndices.Count; i++)
                        {
                            if (vertex.BoneWeights[i] == 0)
                                continue;

                            var index = vertex.BoneIndices[i];
                            if (model.Skeleton.RemapTable.Count > index)
                                index = model.Skeleton.RemapTable[index];

                            var bone = model.Skeleton.Bones[index];

                            var daeWeight = new IOBoneWeight();
                            daeWeight.BoneName = bone.Name;
                            daeWeight.Weight = vertex.BoneWeights.Count > i ? vertex.BoneWeights[i] : 1.0f;
                            daeVertex.Envelope.Weights.Add(daeWeight);
                        }
                        daeMesh.Vertices.Add(daeVertex);
                    }
                }
            }
            return scene;
        }

        static IOBone ConvertBones(STBone bone, IOBone parent = null)
        {
            var daeBone = new IOBone();
            daeBone.Name = bone.Name;
            daeBone.Parent = parent;
            daeBone.Translation = ToVec3(bone.Position);
            daeBone.RotationEuler = ToVec3(bone.EulerRotation);
            daeBone.Scale = ToVec3(bone.Scale);

            foreach (var child in bone.Children)
                daeBone.AddChild(ConvertBones(child, daeBone));

            return daeBone;
        }

        static System.Numerics.Matrix4x4 ToMat4(OpenTK.Matrix4 mat)
        {
            return new System.Numerics.Matrix4x4(
                mat.M11, mat.M21, mat.M31, mat.M41,
                mat.M12, mat.M22, mat.M32, mat.M42,
                mat.M13, mat.M23, mat.M33, mat.M43,
                mat.M14, mat.M24, mat.M34, mat.M44);
        }

        static OpenTK.Vector2 ToVec2(System.Numerics.Vector2 vec)
        {
            return new OpenTK.Vector2(vec.X, vec.Y);
        }

        static System.Numerics.Vector2 ToVec2(OpenTK.Vector2 vec)
        {
            return new System.Numerics.Vector2(vec.X, vec.Y);
        }

        static OpenTK.Vector3 ToVec3(System.Numerics.Vector3 vec)
        {
            return new OpenTK.Vector3(vec.X, vec.Y, vec.Z);
        }

        static System.Numerics.Vector3 ToVec3(OpenTK.Vector3 vec)
        {
            return new System.Numerics.Vector3(vec.X, vec.Y, vec.Z);
        }

        static OpenTK.Vector4 ToVec4(System.Numerics.Vector4 vec)
        {
            return new OpenTK.Vector4(vec.X, vec.Y, vec.Z, vec.W);
        }

        static System.Numerics.Vector4 ToVec4(OpenTK.Vector4 vec)
        {
            return new System.Numerics.Vector4(vec.X, vec.Y, vec.Z, vec.W);
        }
    }
}
