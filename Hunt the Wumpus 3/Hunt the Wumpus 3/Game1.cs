using System;
using System.Linq;
using System.Collections.Generic;
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
            reps = new MapRep[20];

            //initializes the room and line representations
            reps[0] = new MapRep();
            reps[0].LoadContent(Content, "sphere1uR");
            reps[0].Position = new Vector3(16.2f, 1, 0.9f);
            reps[1] = new MapRep();
            reps[1].LoadContent(Content, "sphere1uR");
            reps[1].Position = new Vector3(32.4f, 1, 12.5f);
            reps[2] = new MapRep();
            reps[2].LoadContent(Content, "sphere1uR");
            reps[2].Position = new Vector3(26.2f, 1, 31.6f);
            reps[3] = new MapRep();
            reps[3].LoadContent(Content, "sphere1uR");
            reps[3].Position = new Vector3(6.2f, 1, 31.6f);
            reps[4] = new MapRep();
            reps[4].LoadContent(Content, "sphere1uR");
            reps[4].Position = new Vector3(0.1f, 1, 12.6f);
            reps[5] = new MapRep();
            reps[5].LoadContent(Content, "sphere1uR");
            reps[5].Position = new Vector3(5.4f, 1, 14.3f);
            reps[6] = new MapRep();
            reps[6].LoadContent(Content, "sphere1uR");
            reps[6].Position = new Vector3(9.8f, 1, 8.8f);
            reps[7] = new MapRep();
            reps[7].LoadContent(Content, "sphere1uR");
            reps[7].Position = new Vector3(16.2f, 1, 6.6f);
            reps[8] = new MapRep();
            reps[8].LoadContent(Content, "sphere1uR");
            reps[8].Position = new Vector3(22.9f, 1, 8.7f);
            reps[9] = new MapRep();
            reps[9].LoadContent(Content, "sphere1uR");
            reps[9].Position = new Vector3(27.2f, 1, 14.2f);
            reps[10] = new MapRep();
            reps[10].LoadContent(Content, "sphere1uR");
            reps[10].Position = new Vector3(27.1f, 1, 21.3f);
            reps[11] = new MapRep();
            reps[11].LoadContent(Content, "sphere1uR");
            reps[11].Position = new Vector3(23.0f, 1, 27.1f);
            reps[12] = new MapRep();
            reps[12].LoadContent(Content, "sphere1uR");
            reps[12].Position = new Vector3(16.2f, 1, 28.9f);
            reps[13] = new MapRep();
            reps[13].LoadContent(Content, "sphere1uR");
            reps[13].Position = new Vector3(9.5f, 1, 27.1f);
            reps[14] = new MapRep();
            reps[14].LoadContent(Content, "sphere1uR");
            reps[14].Position = new Vector3(5.5f, 1, 21.3f);
            reps[15] = new MapRep();
            reps[15].LoadContent(Content, "sphere1uR");
            reps[15].Position = new Vector3(10.8f, 1, 19.6f);
            reps[16] = new MapRep();
            reps[16].LoadContent(Content, "sphere1uR");
            reps[16].Position = new Vector3(13.1f, 1, 13.2f);
            reps[17] = new MapRep();
            reps[17].LoadContent(Content, "sphere1uR");
            reps[17].Position = new Vector3(19.6f, 1, 13.2f);
            reps[18] = new MapRep();
            reps[18].LoadContent(Content, "sphere1uR");
            reps[18].Position = new Vector3(21.6f, 1, 19.6f);
            reps[19] = new MapRep();
            reps[19].LoadContent(Content, "sphere1uR");
            reps[19].Position = new Vector3(16.2f, 1, 23.4f);

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
            gameCamera.Update(rotation, position, GraphicsDevice.Viewport.AspectRatio);

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
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = Matrix.Identity;

                    //use the matrices provided by the game camera
                    effect.View = gameCamera.ViewMatrix;
                    effect.Projection = gameCamera.ProjectionMatrix;
                }
                mesh.Draw();
            }
        }
    }
}
