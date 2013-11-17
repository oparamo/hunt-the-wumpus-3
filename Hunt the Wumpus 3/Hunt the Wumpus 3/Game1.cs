using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Hunt_the_Wumpus_3 {
    public class Game1 : Microsoft.Xna.Framework.Game {
        //global variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameObject ground;
        Camera gameCamera;
        PlayerView view;
        MapRep[] reps;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            ground = new GameObject();
            gameCamera = new Camera();

            //initialize map representations
            reps = new MapRep[2];

            //initializes the room and line representations
            reps[0] = new MapRep();
            reps[0].LoadContent(Content, "sphere1uR");
            reps[0].Position = new Vector3(0, 0, 30);
            reps[1] = new MapRep();
            reps[1].LoadContent(Content, "cylinder10uR");
            reps[1].Position = new Vector3(10, 0, 30);

            //initialize the player's view
            view = new PlayerView();

            base.Initialize();
        }

        protected override void LoadContent() {
            //create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ground.Model = Content.Load<Model>("ground");
        }

        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime) {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            float rotation = 0.0f;
            Vector3 position = Vector3.Zero;
            gameCamera.Update(rotation, position,
                GraphicsDevice.Viewport.AspectRatio);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            graphics.GraphicsDevice.Clear(Color.Black);

            DrawTerrain(ground.Model);

            foreach (MapRep rep in reps)
                rep.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);

            base.Draw(gameTime);
        }

        private void DrawTerrain(Model model) {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = Matrix.Identity;

                    // Use the matrices provided by the game camera
                    effect.View = gameCamera.ViewMatrix;
                    effect.Projection = gameCamera.ProjectionMatrix;
                }
                mesh.Draw();
            }
        }
    }
}
