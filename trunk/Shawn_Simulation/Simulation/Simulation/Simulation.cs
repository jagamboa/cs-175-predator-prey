using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Simulation
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Simulation : Microsoft.Xna.Framework.Game
    {
        Random rand = new Random();

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;

        Texture2D boid_texture;

        PreyFlock flock;
        PredatorPack preds;

        public Simulation()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Boids Simulator";
            flock = new PreyFlock();
            preds = new PredatorPack();

            for (int i = 0; i < 50; i++)
            {
                flock.addPrey(new Prey(new Vector2(500+rand.Next(-50,50), 500+rand.Next(-50,50)), 5f));
            }

            for (int i = 0; i < 5; i++)
            {
                preds.addPred(new Predator(new Vector2(500+rand.Next(-400,400), 500+rand.Next(-400,400)), 4.5f));
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            device = graphics.GraphicsDevice;
            boid_texture = Content.Load<Texture2D>("boid");
            // TODO: use this.Content to load your game content here
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            flock.update_all(ref preds);
            preds.update_all(ref flock);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateBlue);
            // TODO: Add your drawing code here

            spriteBatch.Begin();
            flock.draw_all(spriteBatch, boid_texture);
            preds.draw_all(spriteBatch, boid_texture);
            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
