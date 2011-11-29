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

namespace NeuralNetworkToolkit
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class WolvesAndSheep : Microsoft.Xna.Framework.Game
    {
        // XNA variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // variables for toolkit
        List<Creature> wolfList;
        GeneticAlgorithm wolfGenAlg;
        GeneticAlgorithm sheepGenAlg;
        List<Creature> sheepList;
        int ticks = 0;
        int bestWolfIndex = 0;
        int bestWolfFitness = 0;
        int bestSheepIndex = 0;
        int bestSheepFitness = Int32.MinValue;

        Texture2D wolfSprite;
        Texture2D sheepSprite;
        SpriteFont font;
        Vector2 centerPoint = new Vector2(5, 10);
        Vector2 wolfText1 = new Vector2(5, 8);
        Vector2 wolfText2 = new Vector2(5, 24);
        Vector2 wolfText3 = new Vector2(5, 40);
        Vector2 wolfText4 = new Vector2(5, 56);
        Vector2 sheepText1 = new Vector2(290, 8);
        Vector2 sheepText2 = new Vector2(290, 24);
        Vector2 sheepText3 = new Vector2(290, 40);
        Vector2 sheepText4 = new Vector2(290, 56);

        // input
        KeyboardState keyState;
        Boolean spaceDown = false;
        Boolean loop = false;

        public WolvesAndSheep()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 500;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Parameters.random = new Random();
            Parameters.worldWidth = GraphicsDevice.Viewport.Width;
            Parameters.worldHeight = GraphicsDevice.Viewport.Height;

            wolfList = new List<Creature>(Parameters.numberOfWolves);
            sheepList = new List<Creature>(Parameters.numberOfSheep);

            for (int i = 0; i < Parameters.numberOfWolves; i++)
            {
                Vector pos = new Vector(Parameters.random.Next(Parameters.worldWidth),
                                                Parameters.random.Next(Parameters.worldHeight));
                
                wolfList.Add(new Creature(pos));
            }

            for (int i = 0; i < Parameters.numberOfSheep; i++)
            {
                Vector pos = new Vector(Parameters.random.Next(Parameters.worldWidth),
                                                Parameters.random.Next(Parameters.worldHeight));

                sheepList.Add(new Creature(pos));
            }

            wolfGenAlg = new GeneticAlgorithm(wolfList);
            sheepGenAlg = new GeneticAlgorithm(sheepList);

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

            wolfSprite = Content.Load<Texture2D>("Art/Wolf");
            sheepSprite = Content.Load<Texture2D>("Art/Sheep");
            font = Content.Load<SpriteFont>("Font");
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

            keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Space) && !spaceDown)
            {
                spaceDown = true;
                loop = !loop;
                this.IsFixedTimeStep = !loop;
                graphics.SynchronizeWithVerticalRetrace = !loop;
            }
            else if (keyState.IsKeyUp(Keys.Space))
            {
                spaceDown = false;
            }

            do
            {
                if (ticks < Parameters.numberOfTicks)
                {
                    foreach (Creature wolf in wolfList)
                    {
                        wolf.update(sheepList);
                    }
                    foreach (Creature sheep in sheepList)
                    {
                        sheep.update(wolfList);
                    }

                    // check if any wolves ate any sheep
                    for (int i = 0; i < wolfList.Count; i++)
                    {
                        for (int j = 0; j < sheepList.Count; j++)
                        {
                            if (Vector.Distance(wolfList[i].position, sheepList[j].position) < Parameters.minDistanceToTouch)
                            {
                                sheepList[j].reposition();
                                sheepList[j].decrementFitness();
                                wolfList[i].incrementFitness();
                            }
                        }

                        if (wolfList[i].fitness > bestWolfFitness)
                        {
                            bestWolfFitness = wolfList[i].fitness;
                            bestWolfIndex = i;
                        }
                    }

                    // update best sheep
                    for (int i = 0; i < sheepList.Count; i++)
                    {
                        if (sheepList[i].fitness > bestSheepFitness)
                        {
                            bestSheepFitness = sheepList[i].fitness;
                            bestSheepIndex = i;
                        }
                    }

                    ticks++;
                }
                else
                {
                    ticks = 0;
                    bestWolfFitness = 0;
                    bestWolfIndex = 0;

                    wolfGenAlg.nextGeneration(wolfList);
                    sheepGenAlg.nextGeneration(sheepList);

                    foreach (Creature wolf in wolfList)
                    {
                        wolf.reset();
                    }
                    foreach (Creature sheep in sheepList)
                    {
                        sheep.reset();
                    }
                }

            } while (loop && ticks != 0);





            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            for (int i = 0; i < wolfList.Count; i++)
            {
                if (i == bestWolfIndex)
                {
                    spriteBatch.Draw(wolfSprite, wolfList[i].getPosition(), null, Color.Tomato, (float)wolfList[i].getAngle(), centerPoint, 1.5f, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(wolfSprite, wolfList[i].getPosition(), null, Color.Turquoise, (float)wolfList[i].getAngle(), centerPoint, 1, SpriteEffects.None, 0);
                }
            }
            for (int i = 0; i < sheepList.Count; i++)
            {
                if (i == bestSheepIndex)
                {
                    spriteBatch.Draw(sheepSprite, sheepList[i].getPosition(), null, Color.SpringGreen, (float)sheepList[i].getAngle(), centerPoint, 1, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(sheepSprite, sheepList[i].getPosition(), null, Color.White, (float)sheepList[i].getAngle(), centerPoint, 1, SpriteEffects.None, 0);
                }
            }

            spriteBatch.DrawString(font, "Wolf", wolfText1, Color.Tomato);
            spriteBatch.DrawString(font, "Generation: " + wolfGenAlg.generationCount, wolfText2, Color.Tomato);
            spriteBatch.DrawString(font, "Best Fitness: " + wolfGenAlg.bestFitness, wolfText3, Color.Tomato);
            spriteBatch.DrawString(font, "Average Fitness: " + wolfGenAlg.getAverageFitness(), wolfText4, Color.Tomato);
            spriteBatch.DrawString(font, "Sheep", sheepText1, Color.DarkOrange);
            spriteBatch.DrawString(font, "Generation: " + sheepGenAlg.generationCount, sheepText2, Color.DarkOrange);
            spriteBatch.DrawString(font, "Best Fitness: " + sheepGenAlg.bestFitness, sheepText3, Color.DarkOrange);
            spriteBatch.DrawString(font, "Average Fitness: " + sheepGenAlg.getAverageFitness(), sheepText4, Color.DarkOrange);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
