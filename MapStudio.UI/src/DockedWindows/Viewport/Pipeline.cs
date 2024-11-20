using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using GLFrameworkEngine;

namespace MapStudio.UI
{
    public class ViewportRenderer
    {
        DrawableInfiniteFloor _floor;
        ObjectLinkDrawer _objectLinkDrawer;
        OrientationGizmo _orientationGizmo;
        DrawableBackground _background;

        public List<IRenderableFile> Files = new List<IRenderableFile>();
        public List<CameraAnimation> CameraAnimations = new List<CameraAnimation>();

        private bool isView2D;

        /// <summary>
        /// Determines to use a 2D or 3D camera view.
        /// </summary>
        public bool IsViewport2D
        {
            get { return isView2D; }
            set
            {
                isView2D = value;

                //Set the camera instance
                if (value)
                    _context.Camera = _camera2D;
                else
                    _context.Camera = _camera3D;
                //Update with changes
                _context.UpdateViewport = true;
                //Update camera context with window info
               // OnResize();
            }
        }

        public int Width { get; set; }
        public int Height { get; set; }

        public GLContext _context;

        public Camera _camera3D;
        public Camera _camera2D;

        private OpenTK.Vector2 _previousPosition = OpenTK.Vector2.Zero;

        private DepthTexture DepthTexture;

        private Framebuffer PostEffects;
        private Framebuffer BloomEffects;
        private Framebuffer ScreenBuffer;
        private Framebuffer GBuffer;

        static bool USE_GBUFFER => ShadowMainRenderer.Display;

        public void InitScene()
        {
            if (_background != null)
                return;

            _background = new DrawableBackground();
            _floor = new DrawableInfiniteFloor();
            _objectLinkDrawer = new ObjectLinkDrawer();
            _orientationGizmo = new OrientationGizmo();

            _context = new GLContext();
            _context.SetActive();
            _context.ScreenBuffer = ScreenBuffer;

            //For 2D controls/displaying

            //Top down, locked rotation, ortho projection
            _camera2D = new Camera();
            _camera2D.IsOrthographic = true;
            _camera2D.Mode = Camera.CameraMode.Inspect;
            _camera2D.ResetViewportTransform();
            _camera2D.LockRotation = true;
            _camera2D.Direction = Camera.FaceDirection.Top;
            _camera2D.UpdateMatrices();

            //3D camera
            _camera3D = new Camera();
            _context.Camera = _camera3D;
            _context.Camera.ResetViewportTransform();
            if (_context.Camera.Mode == Camera.CameraMode.Inspect)
            {
                _context.Camera.TargetPosition = new OpenTK.Vector3(0, 0, 0);
                _context.Camera.Distance = 50;
            }
            else
                _context.Camera.TargetPosition = new OpenTK.Vector3(0, 0, 50);

            _context.Scene.Init();
        }

        public void InitBuffers()
        {
            InitScene();

            ScreenBuffer = new Framebuffer(FramebufferTarget.Framebuffer,
                this.Width, this.Height, 16, PixelInternalFormat.Rgba16f, 2); //2 color attachments
            ScreenBuffer.Resize(Width, Height);
            //color pass + selection highlight pass
            ScreenBuffer.SetDrawBuffers(
             DrawBuffersEnum.ColorAttachment0, 
             DrawBuffersEnum.ColorAttachment0); //We will use masking to determine when to show/hide selection alpha mask of color 0

            PostEffects = new Framebuffer(FramebufferTarget.Framebuffer,
                 Width, Height, PixelInternalFormat.Rgba16f, 2);
            PostEffects.Resize(Width, Height);

            BloomEffects = new Framebuffer(FramebufferTarget.Framebuffer,
                 Width, Height, PixelInternalFormat.Rgba16f, 1);
            BloomEffects.Resize(Width, Height);

            DepthTexture = new DepthTexture(Width, Height, PixelInternalFormat.DepthComponent24);

            //Set the GBuffer (Depth, Normals and another output)
            GBuffer = new Framebuffer(FramebufferTarget.Framebuffer);
            GBuffer.AddAttachment(FramebufferAttachment.ColorAttachment0,
                GLTexture2D.CreateUncompressedTexture(Width, Height, PixelInternalFormat.R11fG11fB10f, PixelFormat.Rgba, PixelType.Float));
            GBuffer.AddAttachment(FramebufferAttachment.ColorAttachment3,
                GLTexture2D.CreateUncompressedTexture(Width, Height, PixelInternalFormat.Rgb10A2, PixelFormat.Rgba, PixelType.Float));
            GBuffer.AddAttachment(FramebufferAttachment.ColorAttachment4,
                GLTexture2D.CreateUncompressedTexture(Width, Height, PixelInternalFormat.Rgb10A2, PixelFormat.Rgba, PixelType.Float));
            GBuffer.AddAttachment(FramebufferAttachment.DepthAttachment, DepthTexture);

            GBuffer.SetReadBuffer(ReadBufferMode.None);
            GBuffer.SetDrawBuffers(
                DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.None, DrawBuffersEnum.None,
                DrawBuffersEnum.ColorAttachment3, DrawBuffersEnum.ColorAttachment4);
            GBuffer.Unbind();
        }

        //Adds a camera to the scene for path viewing
        public void AddCameraAnimation(CameraAnimation animation)
        {
            CameraAnimations.Clear();
            CameraAnimations.Add(animation);
        }

        public void AddFile(IRenderableFile renderFile) {
            Files.Add(renderFile);
            _context.Scene.AddRenderObject(renderFile.Renderer);
        }

        public void AddFile(IDrawable renderFile) {
            _context.Scene.AddRenderObject(renderFile);
        }

        public Bitmap SaveAsScreenshot(Framebuffer outputBuffer, int width, int height, bool useAlpha = false)
        {
            _context.UpdateViewport = true;

            //Resize the current viewport
            Width = width;
            Height = height;
            this.OnResize(outputBuffer);

            RenderScene(new RenderFrameArgs()
            {
                DisplayAlpha = useAlpha,
                DisplayBackground = !useAlpha,
                DisplayOrientationGizmo = false,
                DisplayGizmo = false,
                DisplayCursor3D = false,
                DisplayFloor = false,
            }, outputBuffer);

            _context.UpdateViewport = true;

            return outputBuffer.ReadImagePixels(useAlpha);
        }

        private OpenTK.Matrix4 viewProjection;

        public void RenderScene(Framebuffer outputBuffer) {
            _context.Camera.UpdateMatrices();

            //Here we want the scene to only re draw when necessary for performance improvements
            if (viewProjection == _context.Camera.ViewProjectionMatrix && !_context.UpdateViewport)
                return;

            viewProjection = new OpenTK.Matrix4(
                _context.Camera.ViewProjectionMatrix.Row0,
                _context.Camera.ViewProjectionMatrix.Row1,
                _context.Camera.ViewProjectionMatrix.Row2,
                _context.Camera.ViewProjectionMatrix.Row3);

            _context.UpdateViewport = false;

            //Scene is drawn with frame arguments.
            //This is to customize what can be drawn during a single frame.
            //Backgrounds, alpha, and other data can be toggled for render purposes.
            RenderScene(_context.FrameArgs, outputBuffer);
        }

        public void RenderScene(RenderFrameArgs frameArgs, Framebuffer outputBuffer)
        {
            _context.Width = this.Width;
            _context.Height = this.Height;

            GL.Enable(EnableCap.DepthTest);

            var dir = _context.Scene.LightDirection;

            if (ShadowMainRenderer.Display)
                _context.Scene.ShadowRenderer.Render(_context, new OpenTK.Vector3(dir.X, dir.Y, dir.Z));

            ResourceTracker.ResetStats();

            DrawModels();

            //Transfer the screen buffer to the post effects buffer (screen buffer is multi sampled)
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, ScreenBuffer.ID);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, PostEffects.ID);

            int x = _context.ViewportX;
            int y = _context.ViewportY;
            int w = _context.ViewportWidth;
            int h = _context.ViewportHeight;

            GL.BlitFramebuffer(x, y, w, h, x, y, w, h, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);

            outputBuffer.Bind();
            SetViewport();

            GL.ClearColor(0, 0, 0, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            //Draw post effects onto the final buffer
            DrawPostScreenBuffer(outputBuffer, PostEffects, frameArgs);

            //Finally transfer the screen buffer depth onto the final buffer for non post processed objects
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, ScreenBuffer.ID);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, outputBuffer.ID);

            GL.BlitFramebuffer(x, y, w, h, x, y, w, h, ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);

            SetViewport();

            DrawSceneNoPostEffects();

            _context.CurrentShader = null;

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Disable(EnableCap.Blend);

            if (ShadowMainRenderer.Display) {
                _context.Scene.ShadowRenderer.DrawDebugShadowPrePass(_context);
                _context.Scene.ShadowRenderer.DrawDebugQuad(_context);
            }

            //Background
            if (frameArgs.DisplayBackground)
                _background.Draw(_context, Pass.OPAQUE);

            //Draw ui
            _context.UIDrawer.Render(_context);

            _context.Scene.SpawnMarker?.DrawModel(_context, Pass.OPAQUE);

            if (frameArgs.Display2DSprites)
                DrawSprites();
            if (frameArgs.DisplayFloor)
                _floor.Draw(_context);
            if (frameArgs.DisplayOrientationGizmo)
                _orientationGizmo.Draw(_context);
            if (frameArgs.DisplayCursor3D)
                _context.Scene.Cursor3D.DrawModel(_context, Pass.OPAQUE);
            if (frameArgs.DisplayGizmo && _context.Scene.GetSelected().Count > 0)
                _context.TransformTools.Draw(_context);

            _objectLinkDrawer.Draw(_context);

            _context.SelectionTools.Render(_context,
                _context.CurrentMousePoint.X,
               _context.CurrentMousePoint.Y);

            _context.LinkingTools.Render(_context,
                _context.CurrentMousePoint.X,
               _context.CurrentMousePoint.Y);

            _context.BoxCreationTool.Render(_context);

            GL.Enable(EnableCap.DepthTest);

            foreach (var anim in CameraAnimations)
                anim.DrawPath(_context);


            outputBuffer.Unbind();
        }

        public void OnResize(Framebuffer outputBuffer)
        {
            // Update the opengl viewport
            SetViewport();

            //Resize all the screen buffers
            outputBuffer.Resize(Width, Height);
            ScreenBuffer?.Resize(Width, Height);
            PostEffects?.Resize(Width, Height);
            GBuffer?.Resize(Width, Height);
            BloomEffects?.Resize(Width, Height);

            //Store the screen buffer instance for color buffer effects
            _context.ScreenBuffer = ScreenBuffer;
            _context.Width = this.Width;
            _context.Height = this.Height;
            _context.Camera.Width = this.Width;
            _context.Camera.Height = this.Height;
            _context.Camera.UpdateMatrices();
        }

        public ITransformableObject GetPickedObject(MouseEventInfo e)
        {
            OpenTK.Vector2 position = new OpenTK.Vector2(e.Position.X, _context.Height - e.Position.Y);
            return _context.Scene.FindPickableAtPosition(_context, position);
        }

        private void DrawModels()
        {
            DrawGBuffer();

            SetViewport();
            ScreenBuffer.Bind();

            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            DrawSceneWithPostEffects();

            _context.EnableSelectionMask();

            foreach (var actor in StudioSystem.Instance.Actors)
                actor.Draw(_context);

            _context.ColorPicker.UpdatePickingDepth(_context, new OpenTK.Vector2(
                _context.CurrentMousePoint.X,
               _context.Height - _context.CurrentMousePoint.Y));

            ScreenBuffer.Unbind();
        }

        private void DrawSprites()
        {
            foreach (var file in _context.Scene.Objects)
            {
                if (!file.IsVisible)
                    continue;

                if (file is EditableObject)
                    ((EditableObject)file).DrawSprite(_context);
            }
        }

        private void DrawSceneWithPostEffects()
        {
            foreach (var file in _context.Scene.Objects)
                if (file.IsVisible && file is EditableObject && ((EditableObject)file).UsePostEffects)
                    file.DrawModel(_context, Pass.OPAQUE);

            foreach (var file in _context.Scene.Objects)
                if (file.IsVisible && file is EditableObject && ((EditableObject)file).UsePostEffects)
                    file.DrawModel(_context, Pass.TRANSPARENT);

            ScreenBufferTexture.FilterScreen(_context);

            foreach (var file in _context.Scene.Objects)
                if (file.IsVisible && file is GenericRenderer)
                    ((GenericRenderer)file).DrawColorBufferPass(_context);

            GL.DepthMask(true);
        }

        private void DrawSceneNoPostEffects()
        {
            foreach (var file in _context.Scene.Objects)
            {
                if (!file.IsVisible || file is EditableObject && ((EditableObject)file).UsePostEffects)
                    continue;

                file.DrawModel(_context, Pass.OPAQUE);
            }
            foreach (var file in _context.Scene.Objects)
            {
                if (!file.IsVisible || file is EditableObject && ((EditableObject)file).UsePostEffects)
                    continue;

                file.DrawModel(_context, Pass.TRANSPARENT);
            }
        }

        private bool draw_shadow_prepass = false;
        private void DrawGBuffer()
        {
            if (!USE_GBUFFER)
            {
                if (draw_shadow_prepass)
                {
                    _context.Scene.ShadowRenderer.ResetShadowPrepass();
                    draw_shadow_prepass = false;
                }
                return;
            }

            GBuffer.Bind();
            SetViewport();

            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (var file in _context.Scene.Objects)
            {
                if (file is GenericRenderer)
                    ((GenericRenderer)file).DrawGBuffer(_context);
            }

            GBuffer.Unbind();

            _context.Scene.ShadowRenderer.DrawShadowPrepass(_context,
                 ((GLTexture2D)GBuffer.Attachments[1]),
                  DepthTexture);

           draw_shadow_prepass = true;
        }

        private GLTexture2D bloomPass;

        private void DrawPostScreenBuffer(Framebuffer outputBuffer, Framebuffer screen, RenderFrameArgs frameArgs)
        {
            if (bloomPass == null)
            {
                bloomPass = GLTexture2D.CreateUncompressedTexture(1, 1);
            }

            var colorPass = (GLTexture2D)screen.Attachments[0];
            var highlightPass = (GLTexture2D)ScreenBuffer.Attachments[1];

            if (_context.EnableBloom)
            {
                var brightnessTex = BloomExtractionTexture.FilterScreen(_context, colorPass);
                BloomProcess.Draw(brightnessTex, BloomEffects, _context, Width, Height);
                bloomPass = (GLTexture2D)BloomEffects.Attachments[0];
            }

            outputBuffer.Bind();
            DeferredRenderQuad.Draw(_context, colorPass, highlightPass, bloomPass, frameArgs);
        }

        private void SetViewport()
        {
            _context.SetViewportSize();
        }
    }
}
