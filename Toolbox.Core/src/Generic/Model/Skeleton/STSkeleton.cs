﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using Toolbox.Core.ModelView;

namespace Toolbox.Core
{
    /// <summary>
    /// Represents a skeleton which stores multiple <see cref="STBone"/>.
    /// This is used for rendering, editing and exporting a skeleton with its bones.
    /// </summary>
    public class STSkeleton
    {
        /// <summary>
        /// A list of bones attatched to the skeleton.
        /// </summary>
        public List<STBone> Bones = new List<STBone>();

        /// <summary>
        /// A table for remapping skinning indices for the skeleton used by a mesh.
        /// </summary>
        public List<int> RemapTable = new List<int>();

        /// <summary>
        /// Determines if the skeleton is currently visible in the scene.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// The scaling of the bones when they are displayed.
        /// </summary>
        public float PreviewScale { get; set; } = 1.0f;

        public Matrix4 RootTransform = Matrix4.Identity;
        public Matrix4 LocalTransform = Matrix4.Identity;

        /// <summary>
        /// Resets the current pose back to the original state.
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < Bones.Count; i++)
            {
                Bones[i].AnimationController.Position = new Vector3(
                    Bones[i].Position.X,
                    Bones[i].Position.Y,
                    Bones[i].Position.Z);
                Bones[i].AnimationController.Rotation = new Quaternion(
                    Bones[i].Rotation.X,
                    Bones[i].Rotation.Y,
                    Bones[i].Rotation.Z,
                    Bones[i].Rotation.W);
                Bones[i].AnimationController.Scale = new Vector3(
                    Bones[i].Scale.X,
                    Bones[i].Scale.Y,
                    Bones[i].Scale.Z);

                foreach (var skeleton in Bones[i].SkeletonAttachments)
                {
                    skeleton.Reset();
                }
            }

            Update();

            for (int i = 0; i < Bones.Count; i++)
            {
                try
                {
                    var local = Bones[i].Transform * RootTransform.Inverted();
                    Bones[i].Inverse = Matrix4.Invert(local);
                }
                catch (InvalidOperationException)
                {
                    Bones[i].Inverse = Matrix4.Zero;
                }
            }
        }

        public bool Updated = false;

        /// <summary>
        /// Updates the current pose.
        /// </summary>
        public void Update()
        {
            Updated = true;
            foreach (STBone Bone in Bones) {
                Bone.Transform = GetWorldMatrix(Bone);
                Bone.TransformUpdated?.Invoke(Bone, EventArgs.Empty);
                foreach (var skeleton in Bone.SkeletonAttachments)
                {
                    skeleton.RootTransform = skeleton.LocalTransform * Bone.Transform;
                    skeleton.Update();
                }
            }
        }

        public ObjectTreeNode[] CreateBoneTree()
        {
            List<ObjectTreeNode> nodes = new List<ObjectTreeNode>();
            foreach (var bone in Bones)
                nodes.Add(new ObjectTreeNode(bone.Name) { Tag = bone, ImageKey = "Bone" });

            List<ObjectTreeNode> roots = new List<ObjectTreeNode>();
            foreach (var bone in Bones)
            {
                int index = Bones.IndexOf(bone);
                if (bone.ParentIndex != -1)
                    nodes[bone.ParentIndex].AddChild(nodes[index]);
                else
                    roots.Add(nodes[index]);
            }

            return roots.ToArray();
        }

        private Matrix4 GetWorldMatrix(STBone bone) {
            var transform =
                Matrix4.CreateScale(bone.AnimationController.Scale) *
                Matrix4.CreateFromQuaternion(bone.AnimationController.Rotation) *
                Matrix4.CreateTranslation(bone.AnimationController.Position);

            if ((bone.UseSegmentScaleCompensate ||
                 bone.AnimationController.UseSegmentScaleCompensate) && bone.Parent != null)
            {
                transform *= Matrix4.CreateScale(
                         1f / ((STBone)bone.Parent).AnimationController.Scale.X,
                         1f / ((STBone)bone.Parent).AnimationController.Scale.Y,
                         1f / ((STBone)bone.Parent).AnimationController.Scale.Z);
            }

            if (bone.ParentIndex != -1 && !bone.AnimationController.WorldTransform)
                return transform * GetWorldMatrix(bone.Parent);
            else
                return transform * RootTransform;
        }

        public void ConvertWorldToLocalSpace()
        {
            foreach (var bone in Bones)
                ConvertWorldToLocalSpace(bone);

            Reset();
        }

        private void ConvertWorldToLocalSpace(STBone bone)
        {
            if (bone.ParentIndex != -1)
            {
                var mat = GetBoneTransform(bone.Parent).Inverted();
                bone.Position = Vector3.TransformPosition(bone.Position, mat);
                bone.Rotation = mat.ExtractRotation() * bone.Rotation;
            }
        }

        /// <summary>
        /// Gets the bone transform in world space by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Matrix4 GetBoneTransform(int index)
        {
            return GetBoneTransform(Bones[index]);
        }

        /// <summary>
        /// Gets the bone transform in world space.
        /// </summary>
        /// <param name="bone"></param>
        /// <returns></returns>
        public Matrix4 GetBoneTransform(STBone bone)
        {
            if (bone == null)
                return Matrix4.Identity;
            if (bone.ParentIndex == -1)
                return bone.GetTransform();
            else
                return bone.GetTransform() * GetBoneTransform(Bones[bone.ParentIndex]);
        }

        /// <summary>
        /// Searches for the bone by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public STBone SearchBone(string name)
        {
            for (int i = 0; i < Bones.Count; i++)
                if (Bones[i].Name == name)
                    return Bones[i];

            return null;
        }
    }
}
