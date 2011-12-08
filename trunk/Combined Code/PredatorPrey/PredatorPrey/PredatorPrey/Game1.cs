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

        RenderTarget2D render;

        // variables for Vision
        //
        //
        //
        //
        //
        ShapeMatcher sm;

        // variables for Semi Supervised
        //
        //
        //
        //
        //

        // variables for toolkit
        List<Wulffies> predatorList;
        List<Fluffies> preyList;
        int updates = 0;
        int bestPredatorIndex = 0;
        int bestPredatorFitness = 0;
        int bestPreyIndex = 0;
        int bestPreyFitness = Int32.MinValue;

        Texture2D predatorSprite;
        Texture2D preySprite;
        RenderTarget2D target; 
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
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.ApplyChanges();
            IsFixedTimeStep = false;
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

            predatorList = new List<Wulffies>(Parameters.numberOfWolves);
            preyList = new List<Fluffies>(Parameters.numberOfSheep);

            for (int i = 0; i < Parameters.numberOfWolves; i++)
            {
                // random position
                //Vector2 pos = new Vector2(Parameters.random.Next(Parameters.worldWidth),
                //                               Parameters.random.Next(Parameters.worldHeight));
                //Vector2 pos = new Vector2(i*100, i*100);
                Vector2 pos = new Vector2(250 - i * 100, 130 + i * 0);
                predatorList.Add(new Wulffies(pos));
            }

            for (int i = 0; i < Parameters.numberOfSheep; i++)
            {
                // random position
                //Vector2 pos = new Vector2(Parameters.random.Next(Parameters.worldWidth),
                //                                Parameters.random.Next(Parameters.worldHeight));
                Vector2 pos = new Vector2(200, 130);
                preyList.Add(new Fluffies(pos));

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

            render = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);

            // load all sprites and fonts needed
            predatorSprite = Content.Load<Texture2D>("Art/Wolf");
            preySprite = Content.Load<Texture2D>("Art/Sheep");
            font = Content.Load<SpriteFont>("Font");

            //initialize the shapeMatcher with the textures
            List<Texture2D> l = new List<Texture2D>();
            l.Add(predatorSprite);
            l.Add(preySprite);
            sm = new ShapeMatcher(l);
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
                    Color[] visionRect;
                    VisionContainer eyes;
                    AudioContainer temp_ac = new AudioContainer();
                    int rectStartX;
                    int rectStartY;
                    int width;
                    int height;
                    foreach (Creature predator in predatorList)
                    {
                        //the creatures will not be able to see past the edge of the screen
                        //therefore this checks if it's vision intersects with any of the walls
                        width = Parameters.predatorVisionWidth;
                        height = Parameters.predatorVisionHeight;
                        rectStartX = (int)predator.position.X - width/2;
                        rectStartY = (int)predator.position.Y - height / 2;

                        if (predator.position.X + width/2 >= GraphicsDevice.PresentationParameters.BackBufferWidth&& predator.position.Y+height/2 >= GraphicsDevice.PresentationParameters.BackBufferHeight)
                        {
                            width = width / 2 +  GraphicsDevice.PresentationParameters.BackBufferWidth-(int)predator.position.X;
                            height = height / 2 +  GraphicsDevice.PresentationParameters.BackBufferHeight-(int)predator.position.Y;
                        }
                        else if(predator.position.X - width / 2 <= 0 && predator.position.Y + height / 2 >= GraphicsDevice.PresentationParameters.BackBufferHeight)
                        {
                            width = width / 2 + (int)predator.position.X;
                            height = height / 2 +  GraphicsDevice.PresentationParameters.BackBufferHeight - (int)predator.position.Y;
                            rectStartX = 0;
                        }
                        else if (predator.position.X - width / 2 <= 0 && predator.position.Y - height / 2 <= 0)
                        {
                            rectStartX = 0;
                            rectStartY = 0;
                            width = width / 2 + (int)predator.position.X;
                            height = height / 2 + (int)predator.position.Y;
                        }
                        else if (predator.position.X - width / 2 <= 0 && predator.position.Y + height / 2 >= GraphicsDevice.PresentationParameters.BackBufferHeight)
                        {
                            width = width / 2 + GraphicsDevice.PresentationParameters.BackBufferWidth - (int)predator.position.X;
                            height = height / 2 + (int)predator.position.Y;
                            rectStartY = 0;
                        }
                        else if (predator.position.X + width / 2 >= GraphicsDevice.PresentationParameters.BackBufferWidth)
                        {
                            width = width / 2 + GraphicsDevice.PresentationParameters.BackBufferWidth- (int)predator.position.X;
                        }
                        else if (predator.position.Y + height / 2 >= GraphicsDevice.PresentationParameters.BackBufferHeight)
                        {
                            height = height / 2 + GraphicsDevice.PresentationParameters.BackBufferHeight - (int)predator.position.Y;
                        }
                        if (rectStartY <= 0)
                        {
                            height = height / 2 + (int)predator.position.Y;
                            rectStartY = 0;
                        }
                        if (rectStartX <= 0)
                        {
                            width = width / 2 + (int)predator.position.X;
                            rectStartX = 0;
                        }

                        visionRect = new Color[height*width];
                        GraphicsDevice.GetBackBufferData<Color>(new Rectangle(rectStartX, rectStartY, width, height), visionRect, 0, height*width);
                        eyes =sm.findObjects(predator, visionRect, width, height);
                        predator.wrap(eyes, temp_ac);
                        // step1: gather this predator's visual percepts

                        // step2: give the predator it's visual percepts and update's it's state (hunger, position, fitness, etc)

                        // step3: the predator runs it's weight improvement routine (through the nerual net)
                    }
                    foreach (Creature prey in preyList)
                    {
                        width = Parameters.preyVisionWidth;
                        height = Parameters.preyVisionHeight;
                        rectStartX = (int)prey.position.X - width / 2;
                        rectStartY = (int)prey.position.Y - height / 2;
                        if (prey.position.X + width / 2 > GraphicsDevice.PresentationParameters.BackBufferWidth && prey.position.Y + height / 2 > GraphicsDevice.PresentationParameters.BackBufferHeight)
                        {
                            width = width / 2 + GraphicsDevice.PresentationParameters.BackBufferWidth - (int)prey.position.X;
                            height = height / 2 + GraphicsDevice.PresentationParameters.BackBufferHeight - (int)prey.position.Y;
                        }
                        else if (prey.position.X - width / 2 < 0 && prey.position.Y + height / 2 > GraphicsDevice.PresentationParameters.BackBufferHeight)
                        {
                            width = width / 2 + (int)prey.position.X;
                            height = height / 2 + GraphicsDevice.PresentationParameters.BackBufferHeight - (int)prey.position.Y;
                            rectStartX = 0;
                        }
                        else if (prey.position.X - width / 2 < 0 && prey.position.Y - height / 2 < 0)
                        {
                            rectStartX = 0;
                            rectStartY = 0;
                            width = width / 2 + (int)prey.position.X;
                            height = height / 2 + (int)prey.position.Y;
                        }
                        else if (prey.position.X - width / 2 < 0 && prey.position.Y + height / 2 > GraphicsDevice.PresentationParameters.BackBufferHeight)
                        {
                            width = width / 2 + GraphicsDevice.PresentationParameters.BackBufferWidth - (int)prey.position.X;
                            height = height / 2 + (int)prey.position.Y;
                            rectStartY = 0;
                        }
                        else if (prey.position.X + width / 2 > GraphicsDevice.PresentationParameters.BackBufferWidth)
                        {
                            width = width / 2 + GraphicsDevice.PresentationParameters.BackBufferWidth - (int)prey.position.X;
                        }
                        else if (prey.position.Y + height / 2 > GraphicsDevice.PresentationParameters.BackBufferHeight)
                        {
                            height = height / 2 + GraphicsDevice.PresentationParameters.BackBufferHeight - (int)prey.position.Y;
                        }
                        if (rectStartY < 0)
                        {
                            height = height / 2 + (int)prey.position.Y;
                            rectStartY = 0;
                        }
                        if (rectStartX < 0)
                        {
                            width = width / 2 + (int)prey.position.X;
                            rectStartX = 0;
                        }
                        //width = 10;
                        //height = 20;
                        visionRect = new Color[height * width];
                        render.GetData<Color>(0,new Rectangle(rectStartX, rectStartY, width, height), visionRect, 0, height * width);
                        eyes = sm.findObjects(prey, visionRect, width, height);
                        eyes = new VisionContainer();
                        foreach (Creature predator in predatorList)
                        {
                            eyes.add(new ObjectSeen(Classification.Predator, Vector2.Subtract(predator.position, prey.position), Vector2.Normalize(predator.velocity)));
                        }
                        prey.wrap(eyes, temp_ac);
                        if (eyes.size() > 0)
                        {
                            prey.leftSideSpeed = 1;
                        }

                        //for (int i = 0; i < height * width; i++)
                        //{
                        //    if (visionRect[i] != new Color(68, 34, 136))
                        //    {
                        //        Console.WriteLine("We Found Something!");
                        //    }
                        //}
                        // step1: gather this prey's visual percepts

                        // step2: give the prey it's visual percepts and update's it's state (hunger, position, fitness, etc)

                        // step3: the prey runs it's weight improvement routine (through the nerual net)
                    }

                    // check if any predators ate any prey
                    for (int i = 0; i < predatorList.Count; i++)
                    {
                        for (int j = 0; j < preyList.Count; j++)
                        {
                            int positionX = (int)(preyList[j].position.X -predatorList[i].position.X);
                            int positionY = (int)(preyList[j].position.Y -predatorList[i].position.Y);
                            if (Vector2.Distance(predatorList[i].position, preyList[j].position) < Parameters.minDistanceToTouch && positionX*predatorList[i].velocity.X>0 && positionY*predatorList[i].velocity.Y>0)
                            {
                                // step1: kill the sheep
                                preyList[j].die();
                                // step2: change any fitness/eat count values accordingly
                                predatorList[i].eat();
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
            GraphicsDevice.SetRenderTarget(render);
            GraphicsDevice.Clear(new Color(168, 134, 236));

            spriteBatch.Begin();

            // draw the predators
            for (int i = 0; i < predatorList.Count; i++)
            {
                if (i == bestPredatorIndex) // draw the best predator
                {
                    spriteBatch.Draw(predatorSprite, predatorList[i].getPosition(), null, Color.White, (float)predatorList[i].getAngle(), centerPoint, 1, SpriteEffects.None, 0);
                    //spriteBatch.Draw(predatorSprite, predatorList[i].getPosition(), null, Color.CadetBlue, (float)predatorList[i].getAngle(), centerPoint, 1, SpriteEffects.None, 0);
                }
                else // draw the other predators
                {
                    spriteBatch.Draw(predatorSprite, predatorList[i].getPosition(), null, Color.White, (float)predatorList[i].getAngle(), centerPoint, 1, SpriteEffects.None, 0);
                }
            }
            // draw the prey
            for (int i = 0; i < preyList.Count; i++)
            {
                if (i == bestPreyIndex) // draw the best prey
                {
                    spriteBatch.Draw(preySprite, preyList[i].getPosition(), null, Color.White, (float)preyList[i].getAngle(), centerPoint, 1, SpriteEffects.None, 0);
                    Console.WriteLine("I'm a Prey and I'm @ (" + preyList[i].getPosition().X + ", " + preyList[i].getPosition().Y + ")");
                }
                else // draw the other prey
                {
                    spriteBatch.Draw(preySprite, preyList[i].getPosition(), null, Color.White, (float)preyList[i].getAngle(), centerPoint, 1, SpriteEffects.None, 0);
                }
            }

            // render text info to screen

            //spriteBatch.DrawString(font, "Predator", predatorText1, Color.Tomato);
            //spriteBatch.DrawString(font, "Generation: " + predatorGenAlg.generationCount, predatorText2, Color.Tomato);
            //spriteBatch.DrawString(font, "Best Fitness: " + predatorGenAlg.bestFitness, predatorText3, Color.Tomato);
            //spriteBatch.DrawString(font, "Average Fitness: " + predatorGenAlg.getAverageFitness(), predatorText4, Color.Tomato);
            //spriteBatch.DrawString(font, "Sheep", preyText1, Color.DarkOrange);
            //spriteBatch.DrawString(font, "Generation: " + sheepGenAlg.generationCount, sheepText2, Color.DarkOrange);
            //spriteBatch.DrawString(font, "Best Fitness: " + sheepGenAlg.bestFitness, sheepText3, Color.DarkOrange);
            //spriteBatch.DrawString(font, "Average Fitness: " + sheepGenAlg.getAverageFitness(), sheepText4, Color.DarkOrange);

            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin();

            spriteBatch.Draw(render, Vector2.Zero, Color.White);

            spriteBatch.End();
            
            base.Draw(gameTime);

            
        }
    }
}