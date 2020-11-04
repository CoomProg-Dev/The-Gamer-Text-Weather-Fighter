using System;
using System.Collections.Generic;
using System.Dynamic;

namespace The_Gamer_Text_Weather_Fighter
{

    class Map
    {
        public int mapX = 30;
        public int mapY = 30;
        public bool isGenerated = false;

        //stores the map as a list of strings
        public List<string> mapList = new List<string>();

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

    class Monster
    {
        public int Health { get; set; }
        public string Name { get; set; }
        public int Damage { get; set; }
        public int XP { get; set; }

        public Monster(string name, int health, int damage, int xp)
        {
            Health = health;
            Name = name;
            Damage = damage;
            XP = xp;
        }
    }

    class Program
    {

        #region INITIALIZE OBJECTS/LISTS

        // Possible Maps

        static Map Town = new Map();

        //Possible Monsters that the player can fight
        static readonly List<Monster> possibleMonsters = new List<Monster>()
        {
            new Monster("goblin", 10, 5, 10),
            new Monster("orc", 30, 6, 30),
            new Monster("ghost", 15, 10, 20),
            new Monster("zombie", 20, 7, 15)
        };

        //Possible Items in the game
        static readonly List<Item> possibleItems = new List<Item>() {
            new Weapon("sword", 10, 10),
            new Weapon("dagger", 3, 5),
            new Weapon("axe", 14, 13),
            new Weapon("spear", 5, 10),
            new Weapon("pickaxe", 10, 7),
            new Item("wood", 1),
            new Item("stone", 2),
            new Item("food", 1)
        };

        //Possible items that could be found in a chest
        static readonly List<Item> possibleChestItems = new List<Item>()
        {
            new Weapon("sword", 10, 10),
            new Weapon("dagger", 3, 5),
            new Weapon("axe", 14, 13),
            new Weapon("spear", 5, 10),
            new Weapon("pickaxe", 10, 7),
            new Item("food", 1)
        };

        //stores all the possible tiles used to generate maps
        static readonly List<char> randTiles = new List<char>() {
            'b', 'b', 'b', 'b', 'b', 'b', 'b', 'b', 'b', 'b', 'b', 'b', 'b', 'b', 'b', 'b', // 11/23
            't', 't', 't', 't', 't', 't', // 6/23
            'r', 'r', 'r', 'r',  // 4/23
            'c' // 1/23
        };

        //Random number
        static readonly Random random = new Random();

        //Affirmative answer list
        static readonly List<string> yesAnswers = new List<string>()
        {
            "yes",
            "y",
            "affirmative",
        };

        // no answer list
        static readonly List<string> noAnswers = new List<string>()
        {
            "no",
            "n"
        };

        //A varible to see if the palyer wants to incratct with things
        static bool eventsEnabled = true;
        #endregion

        /* %%%%%%%%%%%%% PLAYER STATS %%%%%%%%%%%%% */
        #region PLAYER STATS/IMPOTANT VARIBALES
        //Player Things-------------
        static bool playerAlive = false;
        static int playerX = 0;
        static int playerY = 0;
        static string currentPlayerTile = "b";
        static int playerHP = 100;
        static List<Item> playerInventory = new List<Item>();
        static Weapon playerCurWeapon = null;

        #endregion

        /* %%%%%%%%%%%%% MAIN %%%%%%%%%%%%% */
        #region MAIN
        static void Main(string[] args)
        {

            //Run Game
            while (true)
            {

                GameReset();

                //ask to play a new game
                TW("Would you like to start a new game?  yes/no", 0);

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

                    Events();

                    if (playerHP <= 0)
                    {
                        playerAlive = false;
                    }

                }

                Console.Clear();
                TW("-------------Game Over-------------", 0);
            }
        }
        #endregion

        /* %%%%%%%%%%%%% EXTRA FUNCTIONS %%%%%%%%%%%%% */
        #region EXTRA FUNCTIONS

        static void TW(string text, int waitTime)
        {

            Console.WriteLine();
            for (int i = 0; i < text.Length; i++)
            {
                Console.Write(text.Substring(i, 1));
                System.Threading.Thread.Sleep(50);

                if (Console.KeyAvailable)
                {
                    Console.Write(text.Substring(i + 1));
                    Console.ReadKey(true);
                    break;
                }
            }
            System.Threading.Thread.Sleep(waitTime);
            Console.WriteLine(); 

        }

        static void PlayerGetItem(string name)
        {
            foreach (Item item in possibleItems)
            {
                if (item.Name == name)
                {
                    Console.WriteLine("You picked up this item: " + item.Name);

                    if (item.GetType() == typeof(Weapon))
                    {

                        Weapon weapon = (Weapon)item;

                        weapon.Quality = random.Next(50, 100) / 100;

                        playerInventory.Add(weapon);
                        
                        Console.WriteLine("The quality of this item is: " + weapon.Quality);

                        if (playerCurWeapon == null)
                        {
                            playerCurWeapon = weapon;
                        }
                    }

                    else
                    {

                        playerInventory.Add(item);
                    }
                }
            }
        }

        static void GameReset()
        {
            playerAlive = false;
            playerX = 0;
            playerY = 0;
            currentPlayerTile = "b";
            playerHP = 100;
            playerInventory = new List<Item>();

            Town.isGenerated = false;
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

                if (mapList[i / mapX].Substring(i - (i / mapX * mapX), 1) == "c")
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("[#]");
                }

                if (mapList[i / mapX].Substring(i - (i / mapX * mapX), 1) == "C")
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("[ ]");
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
            ValidateMove(thisInput, ConsoleKey.W, playerY, 0);
            PlayerMoveUp(map, thisInput);

            //Move Down
            ValidateMove(thisInput, ConsoleKey.S, playerY, mapY - 1);
            PlayerMoveDown(map, mapY, thisInput);

            //Move Left
            ValidateMove(thisInput, ConsoleKey.A, playerX, 0);
            PlayerMoveLeft(map, thisInput);

            //Move Right
            ValidateMove(thisInput, ConsoleKey.D, playerX, mapX - 1);
            PlayerMoveRight(map, mapX, thisInput);

            map[playerY] = map[playerY].Substring(0, playerX) + "p" + map[playerY].Substring(playerX + 1);

            
            //display inventory
            if (thisInput == ConsoleKey.Tab)
            {
                DisplayInventory(ConsoleKey.Tab);
            }

            //quit the game
            if (thisInput == ConsoleKey.Escape)
            {
                Environment.Exit(0);
            }

            //select main weapon
            if (thisInput == ConsoleKey.Q)
            {

            }

            PrintMap(mapX, mapY, map);

            return map;
        }

        private static void PlayerMoveRight(List<string> map, int mapX, ConsoleKey thisInput)
        {
            if (thisInput == ConsoleKey.D && playerX < mapX - 1)
            {
                playerX += 1;
                currentPlayerTile = map[playerY].Substring(playerX, 1);
            }
        }

        private static void PlayerMoveLeft(List<string> map, ConsoleKey thisInput)
        {
            if (thisInput == ConsoleKey.A && playerX > 0)
            {
                playerX -= 1;
                currentPlayerTile = map[playerY].Substring(playerX, 1);
            }
        }

        private static void PlayerMoveDown(List<string> map, int mapY, ConsoleKey thisInput)
        {
            if (thisInput == ConsoleKey.S && playerY < mapY - 1)
            {
                playerY += 1;
                currentPlayerTile = map[playerY].Substring(playerX, 1);
            }
        }

        private static void PlayerMoveUp(List<string> map, ConsoleKey thisInput)
        {
            if (thisInput == ConsoleKey.W && playerY > 0)
            {
                playerY -= 1;
                currentPlayerTile = map[playerY].Substring(playerX, 1);
            }
        }

        private static void DisplayInventory( ConsoleKey thisInput)
        {
            Console.WriteLine("Inventory:");
            int i = 1;
            foreach (Item item in playerInventory)
            {
                
                Console.WriteLine(i.ToString()+ ". " + item.Name);
                Console.WriteLine("");
                i++;
            } 

            while (!Console.KeyAvailable)
            {

            }

            if (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
        }

        private static void ValidateMove(ConsoleKey thisInput, ConsoleKey checkKey, int playerXY, int checkWall)
        {
            if (thisInput == checkKey && playerXY == checkWall)
            {
                Console.WriteLine("You cant move there");
            }
        }

        private static void PlayerTakeDamage(int damage)
        {
            playerHP -= damage;
            Console.WriteLine("You take " + damage + " damage!");
            System.Threading.Thread.Sleep(100);
        }


        private static void SetCurrentWeapon()
        {
            
        }

        static void Events()
        {

            //Fight monsters
            if (random.Next(1,7) == 1)
            {
                Monster monster = possibleMonsters[random.Next(0, possibleMonsters.Count)];
                monster.Damage += random.Next(-3, 4);
                monster.Health += random.Next(-3, 4);
                monster.XP += random.Next(-3, 4);

                TW("You come accross a " + monster.Name, 400);

                while (monster.Health > 0 && playerHP > 0)
                {
                    PlayerTakeDamage(monster.Damage);

                    try
                    {
                        monster.Health -= playerCurWeapon.Damage * playerCurWeapon.Quality;
                    }

                    catch
                    {
                        monster.Health -= random.Next(1,4);
                    }
                }
            }

            //Step on a chest
            if (currentPlayerTile == "c" && eventsEnabled == true)
            {

                string loot = possibleChestItems[random.Next(0, possibleChestItems.Count)].Name;

                TW("You open the chest and find an " + loot + "... Would you like to pick it up?  yes/no", 0);

                if (yesAnswers.Contains(GetInput()))
                {
                    currentPlayerTile = "C";
                    PlayerGetItem(loot);
                }
            }

            //step on a tree
            if (currentPlayerTile == "t")
            {
                if (CheckInvtryName("axe"))
                {
                    TW("You have come across a tree... Would you like to chop it down?  yes/no", 0);

                    if (yesAnswers.Contains(GetInput()))
                    {
                        CurrentTileErase();
                        TW("You chopped down the tree", 400);
                        PlayerGetItem("wood");
                    }

                    else
                    {
                        TW("You did not chop down the tree", 400);
                    }
                }

                else
                {
                    TW("You have come across a tree... you might be able to cut this down some how", 400);
                }
            }

            //step on a rock
            if (currentPlayerTile == "r")
            {
                if (CheckInvtryName("pickaxe"))
                {
                    TW("You have come across a rock... Would you like to break it down?  yes/no", 0);

                    if (yesAnswers.Contains(GetInput()))
                    {
                        CurrentTileErase();
                        TW("You broke down the rock", 400);

                        PlayerGetItem("stone");

                        if (random.Next(1,3) == 1)
                        {
                            PlayerGetItem("iron");
                        }
                    }

                    else
                    {
                        TW("You did not chop down the rock", 400);
                    }
                }

                else
                {
                    TW("You have come across a rock... you might be able to break this down some how", 400);
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
