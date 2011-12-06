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

namespace PredatorPrey
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // XNA variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // variables for Vision
        //
        //
        //
        //
        //

        // variables for Semi Supervised
        //
        //
        //
        //
        //

        // variables for toolkit
        List<Predator> predatorList;
        List<Prey> preyList;
        int updates = 0;
        int bestPredatorIndex = 0;
        int bestPredatorFitness = 0;
        int bestPreyIndex = 0;
        int bestPreyFitness = Int32.MinValue;

        Texture2D predatorSprite;
        Texture2D preySprite;
        SpriteFont font;
        Vector2 centerPoint = new Vector2(5, 10);
        Vector2 predatorText1 = new Vector2(5, 8);
        Vector2 predatorText2 = new Vector2(5, 24);
        Vector2 predatorText3 = new Vector2(5, 40);
        Vector2 predatorText4 = new Vector2(5, 56);
        Vector2 preyText1 = new Vector2(290, 8);
        Vector2 preyText2 = new Vector2(290, 24);
        Vector2 preyText3 = new Vector2(290, 40);
        Vector2 preyText4 = new Vector2(290, 56);

        // input
        KeyboardState keyState;
        Boolean spaceDown = false;
        Boolean loop = false;

        public Game1()
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
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.ApplyChanges();
            Parameters.random = new Random();
            Parameters.worldWidth = GraphicsDevice.Viewport.Width;
            Parameters.worldHeight = GraphicsDevice.Viewport.Height;

            predatorList = new List<Predator>(Parameters.numberOfWolves);
            preyList = new List<Prey>(Parameters.numberOfSheep);

            for (int i = 0; i < Parameters.numberOfWolves; i++)
            {
                // random position
                Vector2 pos = new Vector2(Parameters.random.Next(Parameters.worldWidth),
                                                Parameters.random.Next(Parameters.worldHeight));
                
                predatorList.Add(new Predator(pos));
            }

            for (int i = 0; i < Parameters.numberOfSheep; i++)
            {
                // random position
                Vector2 pos = new Vector2(Parameters.random.Next(Parameters.worldWidth),
                                                Parameters.random.Next(Parameters.worldHeight));

                preyList.Add(new Prey(pos));
            }

            // Initialize Vision
            //
            //
            //
            //

            // Initialize Semi Supervised
            //
            //
            //
            //

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

            // load all sprites and fonts needed
            predatorSprite = Content.Load<Texture2D>("Art/Wolf");
            preySprite = Content.Load<Texture2D>("Art/Sheep");
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
                // if the simulation has not timed out
                if (updates < Parameters.numberOfUpdates)
                {
                    foreach (Creature predator in predatorList)
                    {
                        // step1: gather this predator's visual percepts

                        // step2: give the predator it's visual percepts and update's it's state (hunger, position, fitness, etc)

                        // step3: the predator runs it's weight improvement routine (through the nerual net)
                    }
                    foreach (Creature sheep in preyList)
                    {
                        // step1: gather this prey's visual percepts

                        // step2: give the prey it's visual percepts and update's it's state (hunger, position, fitness, etc)

                        // step3: the prey runs it's weight improvement routine (through the nerual net)
                    }

                    // check if any predators ate any prey
                    for (int i = 0; i < predatorList.Count; i++)
                    {
                        for (int j = 0; j < preyList.Count; j++)
                        {
                            if (Vector2.Distance(predatorList[i].position, preyList[j].position) < Parameters.minDistanceToTouch)
                            {
                                // step1: kill the sheep

                                // step2: change any fitness/eat count values accordingly
                            }
                        }

                        // keeps track of the index of the best predator
                        if (predatorList[i].fitness > bestPredatorFitness)
                        {
                            bestPredatorFitness = predatorList[i].fitness;
                            bestPredatorIndex = i;
                        }
                    }

                    // check if any prey has eaten food
                    //
                    //
                    //
                    //
                    //

                    // perform any other enviromental state checks (collision, boundaries, etc)
                    //
                    //
                    //
                    //
                    //
                    //

                    // keeps track of the index of the best prey
                    for (int i = 0; i < preyList.Count; i++)
                    {
                        if (preyList[i].fitness > bestPreyFitness)
                        {
                            bestPreyFitness = preyList[i].fitness;
                            bestPreyIndex = i;
                        }
                    }

                    // increment the tick counter
                    updates++;
                }
                else
                {
                    // CONSOLE OUTPUT
                        // output any statistis that should be outputted after each generation here

                    // UPDATE PREDATORS
                        // step1: gather semi-supervised data

                        // step2: run semi-supervised routine, output new weights

                        // step3: mutate new weights and place them back in predators (Creature.genes)

                    // UPDATE PREY
                        // step1: gather semi-supervised data

                        // step2: run semi-supervised routine, output new weights

                        // step3: mutate new weights and place them back in prey (Creatures.genes)


                    // reset simulation for next generation
                    updates = 0;
                    bestPredatorFitness = 0;
                    bestPredatorIndex = 0;

                    foreach (Creature predator in predatorList)
                    {
                        predator.reset();
                    }
                    foreach (Creature sheep in preyList)
                    {
                        sheep.reset();
                    }
                }

            } while (loop && updates != 0);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin();

            // draw the predators
            for (int i = 0; i < predatorList.Count; i++)
            {
                if (i == bestPredatorIndex) // draw the best predator
                {
                    spriteBatch.Draw(predatorSprite, predatorList[i].getPosition(), null, Color.Tomato, (float)predatorList[i].getAngle(), centerPoint, 1.5f, SpriteEffects.None, 0);
                }
                else // draw the other predators
                {
                    spriteBatch.Draw(predatorSprite, predatorList[i].getPosition(), null, Color.Turquoise, (float)predatorList[i].getAngle(), centerPoint, 1, SpriteEffects.None, 0);
                }
            }
            // draw the prey
            for (int i = 0; i < preyList.Count; i++)
            {
                if (i == bestPreyIndex) // draw the best prey
                {
                    spriteBatch.Draw(preySprite, preyList[i].getPosition(), null, Color.SpringGreen, (float)preyList[i].getAngle(), centerPoint, 1, SpriteEffects.None, 0);
                }
                else // draw the other prey
                {
                    spriteBatch.Draw(preySprite, preyList[i].getPosition(), null, Color.White, (float)preyList[i].getAngle(), centerPoint, 1, SpriteEffects.None, 0);
                }
            }

            // render text info to screen

            spriteBatch.DrawString(font, "Predator", predatorText1, Color.Tomato);
            //spriteBatch.DrawString(font, "Generation: " + predatorGenAlg.generationCount, predatorText2, Color.Tomato);
            //spriteBatch.DrawString(font, "Best Fitness: " + predatorGenAlg.bestFitness, predatorText3, Color.Tomato);
            //spriteBatch.DrawString(font, "Average Fitness: " + predatorGenAlg.getAverageFitness(), predatorText4, Color.Tomato);
            spriteBatch.DrawString(font, "Sheep", preyText1, Color.DarkOrange);
            //spriteBatch.DrawString(font, "Generation: " + sheepGenAlg.generationCount, sheepText2, Color.DarkOrange);
            //spriteBatch.DrawString(font, "Best Fitness: " + sheepGenAlg.bestFitness, sheepText3, Color.DarkOrange);
            //spriteBatch.DrawString(font, "Average Fitness: " + sheepGenAlg.getAverageFitness(), sheepText4, Color.DarkOrange);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
