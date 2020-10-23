using System;
using System.Collections.Generic;

namespace The_Gamer_Text_Weather_Fighter
{

    class Map
    {
        public int mapX = 30;
        public int mapY = 30;
        public bool isGenerated = false;

        //stores the map as a list of strings
        public List<string> mapList = new List<string>();

        //stores all the possible events
        //static List<char> randTiles = new List<char>() { 'p', 'b' };

        //public Map(List<string> newTiles)
        //{
        //    randTiles = newTiles;

        //}

    }

    class Item
    {
        public string Name { get; set; }
        public int Weight { get; set; }

        public Item(string Name, int Weight)
        {
            this.Weight = Weight;
            this.Name = Name;
        }
    }

    class Weapon : Item
    {
        public int Damage { get; set; }

        //the Goodness of the weapon: 1 is perfect 0 is broken
        public int Quality { get; set; }

        public Weapon(string Name, int Weight, int Damage) : base(Name, Weight)
        {
            this.Damage = Damage;
        }

    }

    class Materials : Item
    {
        public Materials(string Name, int Weight) : base(Name, Weight)
        {

        }
    }

    class Program
    {

        #region INITIALIZE OBJECTS/LISTS

        // Creates a new map
        static Map Town = new Map();

        //Possible Items in the game
        static readonly List<Item> possibleItems = new List<Item>() {
            new Weapon("sword", 10, 10),
            new Weapon("dagger", 3, 5),
            new Weapon("axe", 14, 13),
            new Weapon("spear", 5, 10)
        };

        //stores all the possible events
        static readonly List<char> randTiles = new List<char>() {
            'b', 'b', 'b', 'b', 'b', 'b', 'b', 'b', 'b', 'b', 'b', // 11/23
            'm', // 1/23
            't', 't', 't', 't', 't', 't', // 6/23
            'r', 'r', 'r', 'r',  // 4/23
            'h' // 1/23
        };

        //Random number
        static readonly Random random = new Random();

        #endregion

        /* %%%%%%%%%%%%% PLAYER STATS %%%%%%%%%%%%% */
        #region PLAYER STATS/IMPOTANT VARIBALES
        //Player Things-------------
        static bool playerAlive = true;
        static int playerX = 0;
        static int playerY = 0;
        static string currentPlayerTile = "b";
        static int playerHP = 10;
        static int playerSkill = 0;
        static List<Item> playerInventory = new List<Item>();

        #endregion

        /* %%%%%%%%%%%%% MAIN %%%%%%%%%%%%% */
        #region MAIN
        static void Main(string[] args)
        {

            //Run Game
            while (true)
            {

                //ask to play a new game
                TW("Would you like to start a new game?  yes/no");

                if (GetInput() == "yes")
                {
                    playerAlive = true;
                }

                Town.mapList = MapGen(Town.mapX, Town.mapY, Town.mapList, Town.isGenerated);

                Town.isGenerated = true;

                PrintMap(Town.mapX, Town.mapY, Town.mapList);

                while (playerAlive)
                {

                    PlayerAction(Town.mapList, Town.mapX, Town.mapY);

                    if (playerHP <= 0)
                    {
                        playerAlive = false;
                    }

                }

                Console.Clear();
                TW("-------------Game Over-------------");
            }
        }
        #endregion

        /* %%%%%%%%%%%%% EXTRA FUNCTIONS %%%%%%%%%%%%% */
        #region EXTRA FUNCTIONS

        static void TW(string text)
        {
            Console.WriteLine();
            for (int i = 0; i < text.Length; i++)
            {
                Console.Write(text.Substring(i, 1));
                System.Threading.Thread.Sleep(50);
            }
            System.Threading.Thread.Sleep(800);
            Console.WriteLine();

        }

        static void PlayerGetItem(string name)
        {
            foreach (Item item in possibleItems)
            {
                if (item.Name == name)
                {
                    if (item.GetType() == typeof(Weapon))
                    {

                        Weapon weapon = (Weapon)item;
                        weapon.Quality = random.Next(50, 100) / 100;
                        playerInventory.Add(weapon);

                    }

                    else
                    {
                        playerInventory.Add(item);
                    }

                }
            }

        }

        #endregion

        /* %%%%%%%%%%%%% MAP STUFF %%%%%%%%%%%%% */
        #region MAP STUFF 

        static void PrintMap(int mapX, int mapY, List<string> mapList)
        {


            for (int i = 0; i < (mapX * mapY); i++)
            {

                if (mapList[i / mapX].Substring(i - (i / mapX * mapX), 1) == "p")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(" + ");
                }

                if (mapList[i / mapX].Substring(i - (i / mapX * mapX), 1) == "b")
                {
                    Console.Write("   ");
                }

                if (mapList[i / mapX].Substring(i - (i / mapX * mapX), 1) == "t")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" Y ");
                }

                if (mapList[i / mapX].Substring(i - (i / mapX * mapX), 1) == "r")
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(" o ");
                }

                if (mapList[i / mapX].Substring(i - (i / mapX * mapX), 1) == "h")
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("[ ]");
                }

                if (mapList[i / mapX].Substring(i - (i / mapX * mapX), 1) == "H")
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("[");

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("+");

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("]");
                }

                if (mapList[i / mapX].Substring(i - (i / mapX * mapX), 1) == "m")
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write(" ! ");
                }

                if ((i + 1) % mapX == 0)
                {
                    Console.WriteLine();
                }

            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        static List<string> MapGen(int mapX, int mapY, List<string> mapList, bool check)
        {
            //add a check to see if map has already been generated
            if (check == false)
            {

                for (int i = 0; i < mapY; i++)
                {

                    mapList.Add("");

                    for (int n = 0; n < mapX; n++)
                    {

                        int randTile = random.Next(randTiles.Count);

                        char currentTile = randTiles[randTile];

                        mapList[i] = mapList[i] + currentTile.ToString();

                    }
                }

                //spawn the player
                int spawnY = random.Next(mapY);
                int spawnX = random.Next(mapX);
                playerX = spawnX;
                playerY = spawnY;

                //mapList[playerY] = mapList[playerY].Substring(0, playerX) + "p" + mapList[playerY].Substring(playerX + 1);
                EditMapList(mapList, playerX, playerY, "p");
            }

            return mapList;
        }

        static void EditMapList(List<string> mapList, int x, int y, string aString)
        {
            mapList[y] = mapList[y].Substring(0, x) + aString + mapList[y].Substring(x + 1);
        }

        static void CurrentTileErase()
        {
            currentPlayerTile = "b";
        }

        #endregion

        /* %%%%%%%%%%%%% PLAYER DOES STUFF %%%%%%%%%%%%% */
        #region PLAYER DOES STUFF

        static List<string> PlayerAction(List<string> map, int mapX, int mapY)
        {

            ConsoleKey thisInput = new ConsoleKey();

            while (Console.KeyAvailable == false)
            {

            }

            Console.Clear();

            if (Console.KeyAvailable == true)
            {
                thisInput = Console.ReadKey(true).Key;
                map[playerY] = map[playerY].Substring(0, playerX) + currentPlayerTile + map[playerY].Substring(playerX + 1);
            }

            //Move UP
            if (thisInput == ConsoleKey.W && playerY == 0)
            {
                Console.WriteLine("You cant move there");
            }

            if (thisInput == ConsoleKey.W && playerY > 0)
            {
                playerY -= 1;
                currentPlayerTile = map[playerY].Substring(playerX, 1);
            }

            //Move Down
            if (thisInput == ConsoleKey.S && playerY == mapY - 1)
            {
                Console.WriteLine("You cant move there");
            }

            if (thisInput == ConsoleKey.S && playerY < mapY - 1)
            {
                playerY += 1;
                currentPlayerTile = map[playerY].Substring(playerX, 1);
            }

            //Move Left
            if (thisInput == ConsoleKey.A && playerX == 0)
            {
                Console.WriteLine("You cant move there");
            }

            if (thisInput == ConsoleKey.A && playerX > 0)
            {
                playerX -= 1;
                currentPlayerTile = map[playerY].Substring(playerX, 1);
            }

            //Move Right
            if (thisInput == ConsoleKey.D && playerX == mapX - 1)
            {
                Console.WriteLine("You cant move there");
            }

            if (thisInput == ConsoleKey.D && playerX < mapX - 1)
            {
                playerX += 1;
                currentPlayerTile = map[playerY].Substring(playerX, 1);
            }

            map[playerY] = map[playerY].Substring(0, playerX) + "p" + map[playerY].Substring(playerX + 1);

            Events(map);

            PrintMap(mapX, mapY, map);

            return map;
        }

        static void Events(List<string> map)
        {
            if (currentPlayerTile == "h")
            {
                map[playerY] = map[playerY].Substring(0, playerX) + "H" + map[playerY].Substring(playerX + 1);
                TW("You enter the house and find an axe... Would you like to pick it up?  yes/no");

                if (GetInput() == "yes")
                {
                    PlayerGetItem("axe");
                }
            }

            if (currentPlayerTile == "t")
            {
                if (CheckInvtryName("axe"))
                {
                    TW("You have come across a tree... Would you like to chop it down?  yes/no");

                    if (GetInput() == "yes")
                    {
                        CurrentTileErase();
                        TW("you chopped down the tree");
                    }

                    else
                    {
                        TW("You did not chop down the tree");
                    }
                }

                else
                {
                    TW("you have come across a tree... you might be able to cut this down some how");
                }
            }
        }

        #endregion

        /* %%%%%%%%%%%%% CHECKS %%%%%%%%%%%%% */
        #region CHECKS

        static string GetInput()
        {
            string input = Console.ReadLine();
            return input.ToLower();
        }

        static bool CheckInvtryName(string name)
        {
            foreach (Item item in playerInventory)
            {
                if (item.Name == name)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
