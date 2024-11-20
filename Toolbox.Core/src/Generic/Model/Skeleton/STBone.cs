using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace Toolbox.Core
{
    /// <summary>
    /// Represents a bone storing information to allow rendering, editing, and exporting from a skeleton
    /// </summary>
    public class STBone
    {
        /// <summary>
        /// Gets or sets the name of the bone.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Determines how to scale the bone based on the parent's scale.
        /// Typically used in programs like Maya.
        /// </summary>
        public bool UseSegmentScaleCompensate { get; set; }

        public STSkeleton Skeleton;

        private Matrix4 transform;

        /// <summary>
        /// Gets or sets the transformation of the bone.
        /// Setting this will adjust the 
        /// <see cref="Scale"/>, 
        /// <see cref="Rotation"/>, and 
        /// <see cref="Position"/> properties.
        /// </summary>
        public Matrix4 Transform
        {
            set
            {
                transform = value;
            }
            get
            {
                return transform;
            }
        }

        public Matrix4 Inverse { get; set; }

        /// <summary>
        /// Gets or sets the position of the bone in world space.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the scale of the bone in world space.
        /// </summary>
        public Vector3 Scale { get; set; }

        private Matrix3 RotationMatrix = Matrix3.Identity;

        /// <summary>
        /// Gets or sets the rotation of the bone in world space.
        /// </summary>
        public Quaternion Rotation
        {
            get { return RotationMatrix.ExtractRotation(); }
            set
            {
                RotationMatrix = Matrix3.CreateFromQuaternion(value);
                rotationEuler = RotationMatrix.ExtractEulerAngles();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Rotation"/> using euler method. 
        /// </summary>
        public Vector3 EulerRotation 
        {
            get { return rotationEuler; }
            set
            {
                rotationEuler = value;
                RotationMatrix = Matrix3Extension.FromEulerAngles(value);
            }
        }

        private Vector3 rotationEuler;

        /// <summary>
        /// Gets or sets the <see cref="Rotation"/> using euler method. 
        /// </summary>
        public Vector3 EulerRotationDegrees
        {
            get { return rotationEuler * STMath.Rad2Deg; }
            set {
                rotationEuler = value * STMath.Deg2Rad;
                RotationMatrix = Matrix3Extension.FromEulerAngles(rotationEuler);
            }
        }

        public EventHandler TransformUpdated;

        /// <summary>
        /// An attached skeleton on the current bone.
        /// </summary>
        public List<STSkeleton> SkeletonAttachments = new List<STSkeleton>();

        /// <summary>
        /// Gets or sets the parent bone. Returns null if unused.
        /// </summary>
        public STBone Parent;

        /// <summary>
        /// The list of children this bone is parenting to.
        /// </summary>
        public List<STBone> Children = new List<STBone>();

        /// <summary>
        /// Toggles the visibily of the bone when being rendered.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Gets or sets the parent bone index.
        /// </summary>
        public int ParentIndex
        {
            set
            {
                if (Parent != null) Parent.Children.Remove(this);
                if (value > -1 && value < Skeleton.Bones.Count) {
                    Skeleton.Bones[value].Children.Add(this);
                    Parent = Skeleton.Bones[value];
                }
            }
            get
            {
                if (Parent == null)
                    return -1;

                return Skeleton.Bones.IndexOf(Parent);
            }
        }

        public int Index
        {
            get { return Skeleton.Bones.IndexOf(this); }
        }

        /// <summary>
        /// The animation controller storing transformation data for 
        /// displayed animations.
        /// </summary>
        public STBoneAnimController AnimationController = new STBoneAnimController();

        public STBone(STSkeleton parentSkeleton) {
            Skeleton = parentSkeleton;
            Scale = Vector3.One;
            Position = Vector3.Zero;
            Rotation = Quaternion.Identity;
        }

        public STBone(STSkeleton parentSkeleton, string name) {
            Skeleton = parentSkeleton;
            Name = name;
            Scale = Vector3.One;
            Position = Vector3.Zero;
            Rotation = Quaternion.Identity;
        }

        public void UpdateTransform()
        {
            Skeleton.Reset();
        }

        public void AttachSkeleton(STSkeleton skeleton) {
            if (!SkeletonAttachments.Contains(skeleton))
                SkeletonAttachments.Add(skeleton);
        }

        /// <summary>
        /// Gets the transformation of the bone without it's parent transform applied.
        /// </summary>
        public virtual Matrix4 GetTransform()
        {
            return Matrix4.CreateScale(Scale) *
                   new Matrix4(RotationMatrix) *
                   Matrix4.CreateTranslation(Position);
        }
    }
}
