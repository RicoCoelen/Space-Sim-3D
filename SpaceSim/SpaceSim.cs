#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace SpaceSim
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SpaceSim : Game
    {
        GraphicsDeviceManager graphDev;
        Color background = new Color(2, 0, 6);
        public static SpaceSim World;
        Vector3 cameraPosition = new Vector3(0f, 30f, 80f);
        Vector3 cameraLookAt = new Vector3(0f, 0f, 0f);
        Matrix cameraOrientationMatrix = Matrix.Identity;
        public Matrix View;
        public Matrix Projection;
        public static GraphicsDevice Graphics;

        List<Sphere> spheres;

        // planets
        Sphere sun;
        Sphere earth;
        Sphere mars;
        Sphere jupiter;
        Sphere saturn;
        Sphere uranus;

        // moons
        Sphere moon;
        double moonRotation = 0;

        Spaceship spaceship;
        Vector3 spaceshipPosition = new Vector3(0f, 28f, 77f);
        Matrix spaceshipOrientationMatrix = Matrix.CreateFromYawPitchRoll(0f, -0.17f, 0f);
        Vector3 spaceshipFollowPoint = new Vector3(0f, 0.09f, 0.2f);
        Vector3 spaceshipLookAtPoint = new Vector3(0f, 0.05f, 0f);
        Vector3 bulletSpawnPosition = new Vector3(0f, 0f, -0.1f);

        Skybox skybox;

        SpriteBatch spriteBatch;
        Texture2D reticle, controls;
        Point mousePosition;
        bool wKeyDown, aKeyDown, sKeyDown, dKeyDown;
        bool mouseButton, mouseDown, lastMouseButton;
        float reticleHalfWidth, reticleHalfHeight;

        Vector2 screenCenter;

        public SpaceSim()
            : base()
        {
            Content.RootDirectory = "Content";

            World = this;
            graphDev = new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            Graphics = GraphicsDevice;

#if DEBUG
            graphDev.PreferredBackBufferWidth = 1600;
            graphDev.PreferredBackBufferHeight = 900;
            graphDev.IsFullScreen = false;
#else
            graphDev.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            graphDev.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            graphDev.IsFullScreen = true;
#endif
            graphDev.ApplyChanges();

            SetupCamera(true);
            Window.Title = "HvA - Simulation & Physics - Opdracht 6 - SpaceSim";
            spriteBatch = new SpriteBatch(Graphics);

            spheres = new List<Sphere>();

            // change sun to yellow color
            spheres.Add(sun = new Sphere(Matrix.Identity, Color.Yellow, 30));
            // added the rest of the planets
            spheres.Add(earth = new Sphere(Matrix.Identity, Color.DeepSkyBlue, 16));
            spheres.Add(mars = new Sphere(Matrix.Identity, Color.Red, 21));
            spheres.Add(jupiter = new Sphere(Matrix.Identity, Color.Orange, 27));
            spheres.Add(saturn = new Sphere(Matrix.Identity, Color.Khaki, 36));
            spheres.Add(uranus = new Sphere(Matrix.Identity, Color.Cyan, 43));

            // scale the sun 2x
            sun.Transform = Matrix.CreateScale(2f);
            // scale the rest
            earth.Transform = Matrix.CreateScale(1f);
            mars.Transform = Matrix.CreateScale(0.6f);
            jupiter.Transform = Matrix.CreateScale(1.7f);
            saturn.Transform = Matrix.CreateScale(1.6f);
            uranus.Transform = Matrix.CreateScale(1.5f);

            // translate for position
            earth.Transform *= Matrix.CreateTranslation(16f, 0.0f, 0.0f);
            mars.Transform *= Matrix.CreateTranslation(21f, 0.0f, 0.0f);
            jupiter.Transform *= Matrix.CreateTranslation(27f, 0.0f, 0.0f);
            saturn.Transform *= Matrix.CreateTranslation(36f, 0.0f, 0.0f);
            uranus.Transform *= Matrix.CreateTranslation(43f, 0.0f, 0.0f);

            // used random class
            Random rand = new Random();

            // create rotation
            earth.Transform *= Matrix.CreateRotationY((float)(rand.NextDouble() * 2.0 * Math.PI));
            mars.Transform *= Matrix.CreateRotationY((float)(rand.NextDouble() * 2.0 * Math.PI));
            jupiter.Transform *= Matrix.CreateRotationY((float)(rand.NextDouble() * 2.0 * Math.PI));
            saturn.Transform *= Matrix.CreateRotationY((float)(rand.NextDouble() * 2.0 * Math.PI));
            uranus.Transform *= Matrix.CreateRotationY((float)(rand.NextDouble() * 2.0 * Math.PI));

            // added the moon
            spheres.Add(moon = new Sphere(Matrix.Identity, Color.LightGray, 30));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            spaceship = new Spaceship(spaceshipOrientationMatrix * Matrix.CreateTranslation(spaceshipPosition), Content);
            skybox = new Skybox(Matrix.CreateScale(1000f) * Matrix.CreateTranslation(cameraPosition), Content);
            reticle = Content.Load<Texture2D>("Reticle");
            reticleHalfWidth = reticle.Width / 2f;
            reticleHalfHeight = reticle.Height / 2f;
            controls = Content.Load<Texture2D>("Controls");

            IsMouseVisible = false;
        }

        private void SetupCamera(bool initialize = false)
        {
            View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, cameraOrientationMatrix.Up);
            if (initialize) Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, SpaceSim.World.GraphicsDevice.Viewport.AspectRatio, 0.1f, 2000.0f);
        }

        int i = 0;
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(background);

            SetupCamera();

            skybox.Draw();

            foreach (Sphere sphere in spheres)
            {
                sphere.Draw();
            }



            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.Draw(reticle, new Vector2(mousePosition.X - reticleHalfWidth, mousePosition.Y - reticleHalfHeight), Color.White);
            spriteBatch.Draw(controls, new Vector2(10f, 10f), Color.White);
            spriteBatch.End();
        }

        protected override void Update(GameTime gameTime)
        {
            cameraPosition = Vector3.Transform(spaceshipFollowPoint, spaceship.Transform);
            cameraLookAt = Vector3.Transform(spaceshipLookAtPoint, spaceship.Transform);
            cameraOrientationMatrix = spaceshipOrientationMatrix;

            // Helpers for input
            KeyboardState keyboard = Keyboard.GetState();
            wKeyDown = keyboard.IsKeyDown(Keys.W);
            aKeyDown = keyboard.IsKeyDown(Keys.A);
            sKeyDown = keyboard.IsKeyDown(Keys.S);
            dKeyDown = keyboard.IsKeyDown(Keys.D);
            if (keyboard.IsKeyDown(Keys.Escape)) Exit();
            MouseState mouse = Mouse.GetState();
            mousePosition = mouse.Position;
            mouseButton = mouse.LeftButton == ButtonState.Pressed;
            mouseDown = mouseButton && !lastMouseButton;
            lastMouseButton = mouseButton;

            skybox.Transform = Matrix.CreateScale(1000f) * Matrix.CreateTranslation(cameraPosition);
            // add earth rotation update
            Matrix earthRotation = Matrix.CreateRotationY((float) gameTime.ElapsedGameTime.TotalSeconds * 0.50f);
            earth.Transform = earth.Transform * earthRotation;
            // add mars rotation update
            Matrix marsRotation = Matrix.CreateRotationY((float)gameTime.ElapsedGameTime.TotalSeconds * 0.30f);
            mars.Transform = mars.Transform * marsRotation;
            // add jupiter rotation update    
            Matrix jupiterRotation = Matrix.CreateRotationY((float)gameTime.ElapsedGameTime.TotalSeconds * 0.20f);
            jupiter.Transform = jupiter.Transform * jupiterRotation;
            // add saturn rotation update
            Matrix saturnRotation = Matrix.CreateRotationY((float)gameTime.ElapsedGameTime.TotalSeconds * 0.14f);
            saturn.Transform = saturn.Transform * saturnRotation;
            // add uranus rotation update
            Matrix uranusRotation = Matrix.CreateRotationY((float)gameTime.ElapsedGameTime.TotalSeconds * 0.07f);
            uranus.Transform = uranus.Transform * uranusRotation;

            // moon rotation update
            moonRotation = moonRotation + gameTime.ElapsedGameTime.TotalSeconds * 1.5;
            // scale and position of moon
            moon.Transform = Matrix.CreateScale(0.5f);
            moon.Transform *= Matrix.CreateTranslation(2f, 0.0f, 0.0f);
            // create rotation on x and y relative to earth
            moon.Transform *= Matrix.CreateRotationY((float) moonRotation);
            moon.Transform *= Matrix.CreateRotationX((float) Math.PI / 4);
            moon.Transform *= Matrix.CreateTranslation(Vector3.Transform(Vector3.Zero, earth.Transform));

            base.Update(gameTime);
        }

        static void RotateOrientationMatrixByYawPitchRoll(ref Matrix matrix, float yawChange, float pitchChange, float rollChange)
        {
            if (rollChange != 0f || yawChange != 0f || pitchChange != 0f)
            {
                Vector3 pitch = matrix.Right * pitchChange;
                Vector3 yaw = matrix.Up * yawChange;
                Vector3 roll = matrix.Forward * rollChange;

                Vector3 overallOrientationChange = pitch + yaw + roll;
                float overallAngularChange = overallOrientationChange.Length();
                Vector3 overallRotationAxis = Vector3.Normalize(overallOrientationChange);
                Matrix orientationChange = Matrix.CreateFromAxisAngle(overallRotationAxis, overallAngularChange);
                matrix *= orientationChange;
            }
        }
    }
}
