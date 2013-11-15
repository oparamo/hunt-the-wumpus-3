using System;
using Microsoft.Xna.Framework;

namespace Hunt_the_Wumpus_3 {
    //a game map, each map created is a unique game
    public class Map {
        //the rooms in the map, stored in an array
        private Room[] rooms;
        public Room[] Rooms {
            get {return rooms;}
            set {rooms = value;}
        }
        //the original locations of objects in the map
        //reused if the player wants to use the same map
        private int[] oLoc = new int[5];
        public int[] Oloc {
            get {return oLoc;}
            set {oLoc = value;}
        }
        //the current locations of objects in the map
        //changes every turn
        private int[] cLoc = new int[5];
        public int[] Cloc {
            get {return cLoc;}
            set {cLoc = value;}
        }
        //constructor
        public Map(int size, int player, int wumpus, 
                   int bats, int pitA, int pitB, Vector2 mapPathsPos) {
            //generates rooms and sets up mapping
            rooms = new Room[size];
            setMap(mapPathsPos);

            //gets the object locations and stores them
            //index is important to remember
            oLoc[0] = player;
            oLoc[1] = wumpus;
            oLoc[2] = bats;
            oLoc[3] = pitA;
            oLoc[4] = pitB;

            //marks the rooms that have objects
            for(int i = 0; i < oLoc.Length; i++) {
                rooms[oLoc[i] - 1].Noun = true;
            }

            //copies to current location array
            Array.Copy(oLoc, cLoc, oLoc.Length);
        }
        //sets up room array and room adjacencies
        private void setMap(Vector2 mapPathsPos) {
            //the room setup, each item in the map array is a room
            rooms[0] = new Room(5, 8, 2, mapPathsPos.X + 162, mapPathsPos.Y + 9);
            rooms[1] = new Room(1, 10, 3, mapPathsPos.X + 324, mapPathsPos.Y + 125);
            rooms[2] = new Room(2, 12, 4, mapPathsPos.X + 262, mapPathsPos.Y + 316);
            rooms[3] = new Room(3, 14, 5, mapPathsPos.X + 62, mapPathsPos.Y + 316);
            rooms[4] = new Room(4, 6, 1, mapPathsPos.X + 1, mapPathsPos.Y + 126);
            rooms[5] = new Room(15, 5, 7, mapPathsPos.X + 54, mapPathsPos.Y + 143);
            rooms[6] = new Room(6, 17, 8, mapPathsPos.X + 98, mapPathsPos.Y + 88);
            rooms[7] = new Room(7, 1, 9, mapPathsPos.X + 162, mapPathsPos.Y + 66);
            rooms[8] = new Room(8, 18, 10, mapPathsPos.X + 229, mapPathsPos.Y + 87);
            rooms[9] = new Room(9, 2, 11, mapPathsPos.X + 272, mapPathsPos.Y + 142);
            rooms[10] = new Room(10, 19, 12, mapPathsPos.X + 271, mapPathsPos.Y + 213);
            rooms[11] = new Room(11, 3, 13, mapPathsPos.X + 230, mapPathsPos.Y + 271);
            rooms[12] = new Room(12, 20, 14, mapPathsPos.X + 162, mapPathsPos.Y + 289);
            rooms[13] = new Room(13, 4, 15, mapPathsPos.X + 95, mapPathsPos.Y + 271);
            rooms[14] = new Room(14, 16, 6, mapPathsPos.X + 55, mapPathsPos.Y + 213);
            rooms[15] = new Room(15, 17, 20, mapPathsPos.X + 108, mapPathsPos.Y + 196);
            rooms[16] = new Room(16, 7, 18, mapPathsPos.X + 131, mapPathsPos.Y + 132);
            rooms[17] = new Room(17, 9, 19, mapPathsPos.X + 196, mapPathsPos.Y + 132);
            rooms[18] = new Room(20, 18, 11, mapPathsPos.X + 216, mapPathsPos.Y + 196);
            rooms[19] = new Room(16, 13, 19, mapPathsPos.X + 162, mapPathsPos.Y + 234);
        }
    }
    
    //an individual room in the map
    public class Room {
        //an array that holds the adjacent rooms
        private int[] adj = new int[3];
        public int[] Adj {
            get {return adj;}
            set {adj = value;}
        }
        //booleans to help check what is in a room
        private bool noun = false;
        public bool Noun {
            get {return noun;}
            set {noun = value;}
        }
        //booleans to help check if the player has visited this room
        private bool visited = false;
        public bool Visited {
            get { return visited; }
            set { visited = value; }
        }
        private Vector2 pos;
        public Vector2 Pos {
            get { return pos; }
            set { pos = value; }
        }
        //the constructor for a room, sets up adjacencies
        public Room(int a, int b, int c, float x, float y) {
            //sets up the adjacent room numbers
            adj[0] = a;
            adj[1] = b;
            adj[2] = c;
            pos = new Vector2(x, y);
        }
        public bool Contains(Vector2 mouse) {
            if (mouse.X < pos.X) return false;
            if (mouse.X > (pos.X + 35)) return false;
            if (mouse.Y < pos.Y) return false;
            if (mouse.Y > (pos.Y + 35)) return false;
            return true;
        }
    }

    //every other object/noun in the game exists in a room
    public class Noun {
        //the current room an object is in
        private int room;
        public int Room {
            get {return room;}
            set {room = value;}
        }
        //constructor
        public Noun(int room) {
            this.room = room;
        }
    }

    public class Live:Noun {
        //the status of a live object
        //for the player and wumpus
        private bool alive;
        public bool Alive {
            get {return alive;}
            set {alive = value;}
        }
        //constructor
        public Live(int room)
            : base(room) {
            alive = true;
        }
    }

    //the player object
    public class Player:Live {
        //the room the arrow is in
        private int arrow = 0;
        public int Arrow {
            get {return arrow;}
            set {arrow = value;}
        }
        //once an arrow is shot this is set to true
        private bool shot = false;
        public bool Shot {
            get {return shot;}
            set {shot = value;}
        }
        //constructor
        public Player(int room, ref Map cave)
            : base(room) {
                cave.Rooms[room - 1].Visited = true;
        }
        //move logic
        public void move(ref Map cave, int newRoom) {
            cave.Rooms[Room - 1].Noun = false;
            Room = newRoom;
            cave.Cloc[0] = Room;
            cave.Rooms[Room - 1].Noun = true;
            cave.Rooms[Room - 1].Visited = true;
        }
        //shoot
        public void shoot(ref Map cave, ref Wumpus wumpus) {
            //arrow prompt
            Console.WriteLine("You shot your crooked arrow!");
            
            //wakes the wumpus on the first shot
            if(!Shot) {
                Shot = true;
                Console.WriteLine("The wumpus has awaken!");
                wumpus.Awake = true;
            }
            
            //start the arrow at the player's location
            Arrow = cave.Cloc[0];

            //logic for arrow moving, path updates
            for(int i = 5; i > 0; i--) {
                //console I/O
                Console.WriteLine("Your arrow is in room " + Arrow);
                Console.WriteLine("Tunnels connect to " 
                                    + cave.Rooms[Arrow].Adj[0] + " " 
                                    + cave.Rooms[Arrow].Adj[1] + " " 
                                    + cave.Rooms[Arrow].Adj[2]);
                Console.WriteLine("What room do you want your arrow to travel to?");
                String move = Console.ReadLine();

                //parse input
                int newRoom;
                int.TryParse(move, out newRoom);
                bool valid = true;

                //checks input against the adjacent rooms
                for(int j = 0; j < 3; j++) {
                    if(newRoom == cave.Rooms[cave.Cloc[0]].Adj[j]) {
                        //updates the arrow's location
                        Arrow = newRoom;
                        break;
                    } else if(i == 2) {
                        Console.WriteLine("Not possible");
                        Console.WriteLine("Please choose a correct path.");
                        valid = false;
                    }
                }
                if(!valid) {
                    continue;
                }

                //checks to see if wumpus is hit
                if(Arrow == wumpus.Room) {
                    Console.WriteLine("You've slain the Wumpus!");
                    wumpus.Alive = false;
                    break;
                }
                    
                //tells player how many moves the arrow has left
                Console.WriteLine("You arrow can travel " + i + " more rooms.");
            }

            Console.WriteLine("Your arrow didn't hit anything.");
        }
    }
    
    //the wumpus object
    public class Wumpus:Live {
        //the wumpus' sleep status
        private bool awake = false;
        public bool Awake {
            get {return awake;}
            set {awake = value;}
        }

        public Wumpus(int room)
            : base(room) {
        }

        public void move(Map cave) {
            //generates a random number
            Random genRandom = new Random();
            int random = genRandom.Next(1, 101);

            //75 percent chance that the wumpus moves
            if(random > 25) {
                //the wumpus moves
                //randomly move to next room
                random = genRandom.Next(1, 4);

                cave.Rooms[Room - 1].Noun = false;
                Room = random;
                cave.Cloc[1] = Room;
                cave.Rooms[Room - 1].Noun = true;
            }
        }
    }
    
    //superbat groups
    public class SuperBats:Live {
        //constructor
        public SuperBats(int room)
            : base(room) {
        }
    }

    //pit objects
    public class Pit:Noun {
        public Pit(int room)
            : base(room) {
        }
    }
}