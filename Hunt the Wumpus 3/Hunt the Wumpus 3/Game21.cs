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

namespace Hunt_the_Wumpus_3 {
    enum GameState {
        Prompt,
        Move,
        Shoot,
        GameOver,
        Replay
    }

    public class Game1:Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        KeyboardState oldState;
        SpriteBatch spriteBatch;
        SpriteFont Font;
        Vector2 DrawPos;
        Texture2D mapPaths;
        Texture2D[] mapRooms = new Texture2D[20];
        Texture2D[] icons = new Texture2D[6];
        Texture2D[] yellowSpaces = new Texture2D[3];
        Texture2D[] pinkSpaces = new Texture2D[5];
        Texture2D blueSpace;
        Vector2 mapPathsPos;
        Vector2[] mapRoomPos = new Vector2[20];
        MouseState mouseStateCurrent;
        Camera gameCamera;

        //string that is outputted
        string output = "";
        //different game elements
        int numRooms = 20;
        Map cave;
        Player player;
        Wumpus wumpus;
        SuperBats bats;
        Pit pitA;
        Pit pitB;
        int lastRoom = 0;
        int arrowSpacesLeft = 0;
        int[] arrowPath = new int[5];
        bool resetClick = false;
        //keep track of the game state
        GameState state = GameState.Prompt;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            // Make mouse visible
            this.IsMouseVisible = true;

            //initialize object locations
            int[] locations = generateLocations();

            mapPathsPos = new Vector2(graphics.GraphicsDevice.Viewport.Width * 0.5f, 30);

            //creates a new map
            //the cave variable will hold the locations of nouns in an array
            //ex:cave.CLoc[0] holds player location
            //0 player, 1 wumpus, 2 bats, 3 pita, 4 pitb
            cave = new Map(numRooms, locations[0], locations[1], locations[2],
                               locations[3], locations[4], mapPathsPos);

            //creates the player
            player = new Player(cave.Oloc[0], ref cave);
            //creates the wumpus
            wumpus = new Wumpus(cave.Oloc[1]);
            //creates a group of super bats
            bats = new SuperBats(cave.Oloc[2]);
            //creates a pit
            pitA = new Pit(cave.Oloc[3]);
            //creates a pit
            pitB = new Pit(cave.Oloc[4]);
            //for keyboard listener
            oldState = Keyboard.GetState();

            for(int i = 0; i < 5; i++) {
                arrowPath[i] = 0;
            }
            lastRoom = 0;

            DrawPos = new Vector2(10, 10);

            base.Initialize();
        }

        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Font = Content.Load<SpriteFont>("Arial");
            mapPaths = Content.Load<Texture2D>("wumpus_map_paths");
            for(int i = 0; i < 20; i++) {
                mapRooms[i] = Content.Load<Texture2D>("wumpus_map_room");
            }
            icons[0] = Content.Load<Texture2D>("player_icon");
            icons[1] = Content.Load<Texture2D>("wumpus_icon");
            icons[2] = Content.Load<Texture2D>("bat_icon");
            icons[3] = Content.Load<Texture2D>("pit_icon");
            icons[4] = Content.Load<Texture2D>("pit_icon");
            icons[5] = Content.Load<Texture2D>("arrow_icon");
            for(int i = 0; i < 3; i++) {
                yellowSpaces[i] = Content.Load<Texture2D>("wumpus_map_room_yellow");
            }
            for(int i = 0; i < 5; i++) {
                pinkSpaces[i] = Content.Load<Texture2D>("wumpus_map_room_pink");
            }
            blueSpace = Content.Load<Texture2D>("wumpus_map_room_blue");
        }

        protected override void UnloadContent() {

        }

        protected override void Update(GameTime gameTime) {
            //updates on input
            UpdateInput();

            base.Update(gameTime);
        }


        //draw method
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.WhiteSmoke);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            //Vector2 FontOrigin = Font.MeasureString(output) / 2;
            Vector2 FontOrigin = new Vector2(0,0);
            spriteBatch.DrawString(Font, output, DrawPos, Color.Black,
                0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.Draw(mapPaths, mapPathsPos, Color.White);
            drawMap();
            spriteBatch.End();
            base.Draw(gameTime);
        }

        //updates the state
        private void UpdateInput() {
            KeyboardState newState = Keyboard.GetState();

            if(state == GameState.Prompt) {
                if(oldState.IsKeyDown(Keys.M) && !newState.IsKeyDown(Keys.M)) {
                    //prompt
                    output = "Tunnels connect to rooms " + cave.Rooms[cave.Cloc[0] - 1].Adj[0] + ", " + cave.Rooms[cave.Cloc[0] - 1].Adj[1] +
                                          ", and " + cave.Rooms[cave.Cloc[0] - 1].Adj[2] + ".\n";
                    output = output + "Click a highlighted room to move.";

                    //check surrounding rooms
                    warning();

                    state = GameState.Move;
                } else {
                    //print out the current room
                    output = "You are in room " + cave.Cloc[0] + ".\n";
                    output = output + "Tunnels connect to rooms " + cave.Rooms[cave.Cloc[0] - 1].Adj[0] + ", " + cave.Rooms[cave.Cloc[0] - 1].Adj[1] +
                                      ", and " + cave.Rooms[cave.Cloc[0] - 1].Adj[2] + ".\n";
                    output = output + "Please (S)hoot or (M)ove.";
                }
                if(oldState.IsKeyDown(Keys.S) && !newState.IsKeyDown(Keys.S)) {
                    //shoot logic
                    state = GameState.Shoot;
                    output = "You shot your crooked arrow!\n";
                    arrowSpacesLeft = 5;

                    //wakes the wumpus on the first shot
                    if(!player.Shot) {
                        player.Shot = true;
                        output = output + "The wumpus has awaken!\n";
                        wumpus.Awake = true;
                    }

                    //start the arrow at the player's location
                    player.Arrow = cave.Cloc[0];

                    output = output + "Your arrow is in room " + player.Arrow + ".\n";
                    output = output + "Tunnels connect to rooms "
                                      + cave.Rooms[player.Arrow - 1].Adj[0] + ", "
                                      + cave.Rooms[player.Arrow - 1].Adj[1] + ", and "
                                      + cave.Rooms[player.Arrow - 1].Adj[2] + ".\n";
                    output = output + "Click on the room you want your arrow to travel to.\n";
                    output = output + "You have " + arrowSpacesLeft + " arrow moves.\n";
                }
            } else if(state == GameState.Move) {
                //key listeners for the moving state

                // Get current mouseState
                mouseStateCurrent = Mouse.GetState();

                // Left MouseClick
                if(mouseStateCurrent.LeftButton == ButtonState.Pressed) {
                    int clickedRoom = 0;
                    for(int i = 0; i < 3; i++) {
                        if(cave.Rooms[cave.Rooms[cave.Cloc[0] - 1].Adj[i] - 1].Contains(new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y))) {
                            clickedRoom = cave.Rooms[cave.Cloc[0] - 1].Adj[i];
                            break;
                        }
                    }
                    if(clickedRoom != 0) {
                        lastRoom = player.Room;
                        player.move(ref cave, clickedRoom);
                        //some wumpus logic
                        if(wumpus.Awake) {
                            wumpus.move(cave);

                            if(wumpus.Room == player.Room) {
                                player.Alive = false;

                                state = GameState.GameOver;
                                output = "Game Over!\nThe Wumpus ate you!Would you like to play again? (Y)es (N)o?";
                            } else {
                                state = GameState.Prompt;
                                //checks for hazards
                                hazardCheck();
                            }
                        } else {
                            state = GameState.Prompt;
                            //checks for hazards
                            hazardCheck();
                        }
                    }
                }
            } else if(state == GameState.Shoot) {
                //key listeners for the arrow shooting state

                //logic for arrow moving, path updates
                if(arrowSpacesLeft != 0) {

                    // Get current mouseState
                    mouseStateCurrent = Mouse.GetState();

                    // Left MouseClick
                    if(mouseStateCurrent.LeftButton == ButtonState.Pressed && !resetClick) {
                        resetClick = true;
                        int clickedRoom = 0;
                        for(int i = 0; i < 3; i++) {
                            if(cave.Rooms[cave.Rooms[player.Arrow - 1].Adj[i] - 1].Contains(new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y))) {
                                clickedRoom = cave.Rooms[player.Arrow - 1].Adj[i];
                                break;
                            }
                        }
                        if(clickedRoom != 0) {
                            if(arrowSpacesLeft != 5) {
                                arrowPath[5 - arrowSpacesLeft] = player.Arrow;
                            }
                            player.Arrow = clickedRoom;
                            arrowSpacesLeft--;

                            //checks to see if wumpus is hit
                            if(player.Arrow == wumpus.Room) {
                                output = "You've slain the Wumpus!\nHee hee hee - the Wumpus will getcha next time!\nWould you like to play again? (Y)es (N)o?";
                                wumpus.Alive = false;
                                state = GameState.GameOver;
                                arrowSpacesLeft = 0;
                            }
                            //checks to see if the player shot himself
                            if(player.Arrow == player.Room) {
                                output = "Ouch! Arrow got you!";
                                arrowSpacesLeft = 0;
                            }
                        }
                    } else {
                        resetClick = false;
                    }
                } else {
                    for(int i = 0; i < 5; i++) {
                        arrowPath[i] = 0;
                    }
                    //some wumpus logic
                    if(wumpus.Awake) {
                        wumpus.move(cave);

                        if(wumpus.Room == player.Room) {
                            player.Alive = false;

                            state = GameState.GameOver;
                            output = "Game Over!\nThe Wumpus ate you!Would you like to play again? (Y)es (N)o?";
                        } else {
                            state = GameState.Prompt;
                        }
                    } else {
                        state = GameState.Prompt;
                    }
                }

                if(wumpus.Alive && player.Arrow != player.Room && player.Alive) {
                    output = "Your arrow didn't hit anything.";
                }
            } else if(state == GameState.GameOver) { //game over messages, asks to replay
                if(oldState.IsKeyDown(Keys.Y) && !newState.IsKeyDown(Keys.Y)) {
                    state = GameState.Replay;
                    output = "Would you like to play with the same map? (Y)es (N)o?";
                } else if(oldState.IsKeyDown(Keys.N) && !newState.IsKeyDown(Keys.N)) {
                    Exit();
                }
            } else if(state == GameState.Replay) { //replay promps, option to reset map
                if(oldState.IsKeyDown(Keys.Y) && !newState.IsKeyDown(Keys.Y)) { //the player wants the same map
                    state = GameState.Prompt;

                    cave = new Map(numRooms, cave.Oloc[0], cave.Oloc[1], cave.Oloc[2],
                                       cave.Oloc[3], cave.Oloc[4], mapPathsPos);
                    //creates the player
                    player = new Player(cave.Oloc[0], ref cave);
                    //creates the wumpus
                    wumpus = new Wumpus(cave.Oloc[1]);
                    //creates a group of super bats
                    bats = new SuperBats(cave.Oloc[2]);
                    //creates a pit
                    pitA = new Pit(cave.Oloc[3]);
                    //creates a pit
                    pitB = new Pit(cave.Oloc[4]);

                    for(int i = 0; i < 5; i++) {
                        arrowPath[i] = 0;
                    }
                    lastRoom = 0;
                } else if(oldState.IsKeyDown(Keys.N) && !newState.IsKeyDown(Keys.N)) { //new map
                    state = GameState.Prompt;
                    Initialize();
                }
            }

            // Update saved state.
            oldState = newState;
        }

        //draws the map
        private void drawMap() {
            for(int i = 0; i < 20; i++) {
                spriteBatch.Draw(mapRooms[i], cave.Rooms[i].Pos, Color.White);
            }
            if(lastRoom != 0) {
                spriteBatch.Draw(blueSpace, cave.Rooms[lastRoom - 1].Pos, Color.White);
            }
            if(state == GameState.Move) {
                for(int i = 0; i < 3; i++) {
                    spriteBatch.Draw(yellowSpaces[i], cave.Rooms[cave.Rooms[cave.Cloc[0] - 1].Adj[i] - 1].Pos, Color.White);
                }
            } else if(state == GameState.Shoot) {
                for(int i = 0; i < 3; i++) {
                    spriteBatch.Draw(yellowSpaces[i], cave.Rooms[cave.Rooms[player.Arrow - 1].Adj[i] - 1].Pos, Color.White);
                }
                for(int i = 0; i < 4; i++) {
                    if(arrowPath[i] != 0) {
                        spriteBatch.Draw(pinkSpaces[i], cave.Rooms[arrowPath[i] - 1].Pos, Color.White);
                    }
                }
                if(arrowSpacesLeft != 5 && arrowSpacesLeft != 0) {
                    spriteBatch.Draw(pinkSpaces[4], cave.Rooms[player.Arrow - 1].Pos, Color.White);
                    spriteBatch.Draw(icons[5], cave.Rooms[player.Arrow - 1].Pos, Color.White);
                }
            }
            for(int i = 0; i < 5; i++) {
                if(cave.Rooms[cave.Cloc[i] - 1].Visited || !wumpus.Alive) {
                    if(i != 1 || wumpus.Awake || !wumpus.Alive) {
                        spriteBatch.Draw(icons[i], cave.Rooms[cave.Cloc[i] - 1].Pos, Color.White);
                    }
                }
            }
        }
        
        //checks for hazards
        public void hazardCheck() {
            //spike logic
            if(player.Room == pitA.Room || player.Room == pitB.Room) {
                player.Alive = false;

                state = GameState.GameOver;
                output = "YYYIIIIEEEE . . . fell in a pit!\nGame Over!\nWould you like to play again? (Y)es (N)o?";
            }
            //superbat logic
            if(player.Room == bats.Room) {
                output = "Zap--Super Bat snatch! Elsewhereville for you!";

                //moves the player to a random room
                Random random = new Random();
                player.move(ref cave, random.Next(1, 21));
            }
        }

        //update on the character's status
        public void warning() {
            for(int i = 0; i < 3; i++) {
                for(int j = 1; j < 5; j++) {
                    if(cave.Rooms[player.Room - 1].Adj[i] == cave.Cloc[j]) {
                        if(j == 1) {
                            output = output + "\nI smell a Wumpus.";
                        }
                        if(j == 2) {
                            output = output + "\nBats nearby.";
                        }
                        if(j == 3 || j == 4) {
                            output = output + "\nI feel a draft.";
                        }
                    }
                }
            }
        }

        //generates random numbers for the object locations
        //no two objects should be in the same room
        public static int[] generateLocations() {
            //holds the random numbers generated
            int[] numbers = new int[5];
            //new random generator
            Random genRandom = new Random();

            //makes sure there are no duplicate randoms stored
            for(int i = 0; i < numbers.Length; i++) {
                bool found = false;
                int random;

                //the random number can't be 0
                random = genRandom.Next(1, 21);

                //prevents duplicates, if a duplicate is found, the bool is flipped
                for(int j = 0; j < i; j++) {
                    if(random == numbers[j]) {
                        found = true;
                    }
                }

                //if a duplicate was found, restart the loop with a new random
                //otherwise, store the random
                if(found) {
                    i--;
                    continue;
                } else {
                    numbers[i] = random;
                }
            }

            return numbers;
        }
    }
}