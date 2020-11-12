using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Hues_Adventure
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
        public int Quantity { get; set; }
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
        public float Quality { get; set; }

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

        // Possible Maps

        static Map Town = new Map();

        //Possible Monsters that the player can fight
        static List<Monster> possibleMonsters = new List<Monster>()
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
            new Item("healing potion", 2)
        };

        //Possible items that could be found in a chest
        static readonly List<Item> possibleChestItems = new List<Item>()
        {
            new Weapon("sword", 10, 10),
            new Weapon("dagger", 3, 5),
            new Weapon("axe", 14, 13),
            new Weapon("spear", 5, 10),
            new Weapon("pickaxe", 10, 7),
            new Item("healing potion", 2)
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

        //Player Things-------------
        static bool playerAlive = false;
        static int playerX = 0;
        static int playerY = 0;
        static string currentPlayerTile = "b";
        static int maxPlayerHP = 100;
        static int playerHP = 100;
        static int playerXP = 0;
        static int playerStrength = 1;
        static List<Item> playerInventory = new List<Item>();
        static int currentItemWeight = 0;
        static int maxCarryWeight = 30;
        static Weapon playerCurWeapon = null;


        
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
                else
                {
                    Environment.Exit(0);
                }

                Town.mapList = MapGen(Town.mapX, Town.mapY, Town.mapList, Town.isGenerated);

                Town.isGenerated = true;

                PrintMap(Town.mapX, Town.mapY, Town.mapList);

                while (playerAlive)
                {

                    PlayerAction(Town.mapList, Town.mapX, Town.mapY);

                    Events();

                    UpdatePlayerLevel();

                    if (playerHP <= 0)
                    {
                        playerAlive = false;
                    }

                }

                Console.Clear();
                TW("-------------Game Over-------------", 0);
            }
        }

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

        static bool PlayerGetItem(string name, int quantity)
        {
            foreach (Item item in possibleItems)
            {
                if (item.Name == name)
                {
                    if (item.Weight*quantity + currentItemWeight <= maxCarryWeight)
                    {
                        Console.WriteLine("You picked up this item: " + item.Name);

                        if (item.GetType() == typeof(Weapon))
                        {

                            Weapon weapon = (Weapon)item;

                            weapon.Quantity = 1;
                            weapon.Quality = random.Next(50, 100) / 100f;

                            playerInventory.Add(weapon);

                            Console.WriteLine("The quality of this item is: " + weapon.Quality);

                            if (playerCurWeapon == null)
                            {
                                playerCurWeapon = weapon;
                            }
                        }

                        else
                        {
                            bool itemFound = false;
                            int i = 0;

                            foreach (Item item1 in playerInventory)
                            {
                                if (item1.Name == item.Name)
                                {
                                    playerInventory[i].Quantity += quantity;
                                    itemFound = true;
                                }
                                i++;
                            }

                            if (!itemFound)
                            {
                                item.Quantity += quantity;
                                playerInventory.Add(item);
                            }  
                        }

                        currentItemWeight += item.Weight*quantity;
                        return true;
                    }

                    else
                    {
                        TW("You cannot pick up this item... It is too heavy", 300);
                    }
                }
            }
            return false;
        }

        static void GameReset()
        {
            playerAlive = false;
            playerX = 0;
            playerY = 0;
            currentPlayerTile = "b";
            playerHP = 100;
            maxPlayerHP = 100;
            playerStrength = 1;
            playerInventory.Clear();
            currentItemWeight = 0;
            maxCarryWeight = 30;
            playerCurWeapon = null;

            Town.isGenerated = false;

            possibleMonsters = new List<Monster>()
            {
                new Monster("goblin", 10, 5, 10),
                new Monster("orc", 30, 6, 30),
                new Monster("ghost", 15, 10, 20),
                new Monster("zombie", 20, 7, 15)
            };
        }

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
                mapList.Clear();

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

            PrintMap(mapX, mapY, map);

            //display inventory
            if (thisInput == ConsoleKey.Tab)
            {
                DisplayInventory(ConsoleKey.Tab);
                PlayerAction(map,mapX,mapY);
            }

            //quit the game
            if (thisInput == ConsoleKey.Escape)
            {
                Environment.Exit(0);
            }

            //select main weapon
            if (thisInput == ConsoleKey.Q)
            {
                eventsEnabled = !eventsEnabled;
                if (eventsEnabled)
                {
                    TW("Events are enabled", 300);
                }

                if (!eventsEnabled)
                {
                    TW("Events are disabled", 300);
                }

                PlayerAction(map, mapX, mapY);
            }

            if (thisInput == ConsoleKey.E)
            {
                DisplayPlayerStats();
                PlayerAction(map, mapX, mapY);
            }

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

        private static void DisplayInventory(ConsoleKey thisInput)
        {
            int cursorPos = 1;

            while (true)
            {
                Console.Clear();

                Console.WriteLine("Inventory:");
                
                int i = 1;

                foreach (Item item in playerInventory)
                {
                    if (i == cursorPos)
                    {
                        if (item.Quantity > 1)
                        {
                            Console.WriteLine(">> " + item.Name + " " + item.Quantity);
                            Console.WriteLine("");
                        }

                        else
                        {
                            Console.WriteLine(">> " + item.Name);
                            Console.WriteLine("");
                        }
                    }

                    else
                    {
                        if (item.Quantity > 1)
                        {
                            Console.WriteLine(i.ToString() + ". " + item.Name + " " + item.Quantity);
                            Console.WriteLine("");
                        }

                        else
                        {
                            Console.WriteLine(i.ToString() + ". " + item.Name);
                            Console.WriteLine("");
                        }
                    }

                    i++;
                }

                if(playerInventory.Count != 0)
                {
                    if (selectionSim(ref cursorPos, playerInventory.Count))
                    {
                        DisplayItem(playerInventory[cursorPos - 1], cursorPos - 1);
                        WaitForInput();
                        break;
                    }
                }
            }
        }

        private static void DisplayItem(Item item, int index)
        {
            

            if (item.GetType() != typeof(Weapon))
            {
                Console.Clear();

                Console.WriteLine("-----" + item.Name + "-----");
                Console.WriteLine("Quantity: " + item.Quantity);
                Console.WriteLine("Total Weight: " + (int)(item.Weight * item.Quantity));
            }

            if (item.GetType() == typeof(Weapon))
            {
                Weapon weapon = (Weapon)item;
                List<string> options = new List<string>() { "Equip", "Drop", "Exit" };

                int cursorPos = 1;

                while (true)
                {
                    Console.Clear();

                    Console.WriteLine("-----" + item.Name + "-----");
                    Console.WriteLine("Damage: " + weapon.Damage);
                    Console.WriteLine("Quality: " + weapon.Quality);
                    Console.WriteLine("Total Weight: " + (int)(item.Weight * item.Quantity));

                    int i = 1;

                    foreach (String s in options)
                    {
                        if (i == cursorPos)
                        {
                            Console.WriteLine(">> " + options[i - 1]);
                        }

                        else
                        {
                            Console.WriteLine(i + ". " + options[i - 1]);
                        }

                        i++;
                    }

                    if (selectionSim(ref cursorPos, options.Count))
                    {
                        if (options[cursorPos - 1] == options[0])
                        {
                            playerCurWeapon = weapon;

                            break;
                        }

                        if (options[cursorPos - 1] == options[1])
                        {
                            if (playerInventory[index] == playerCurWeapon)
                            {
                                playerCurWeapon = null;
                            }

                            playerInventory.RemoveAt(index);

                            break;
                        }

                        if (options[cursorPos - 1] == options[2])
                        {
                            break;
                        }
                    }
                }
            }
        }

        private static bool selectionSim(ref int source,int max)
        {
            ConsoleKey consoleKey = new ConsoleKey();

            
            while (!Console.KeyAvailable)
            {

            }

            if (Console.KeyAvailable)
            {
                consoleKey = Console.ReadKey(true).Key;
            }

            if (consoleKey == ConsoleKey.W || consoleKey == ConsoleKey.UpArrow)
            {
                if (source == 1)
                {
                    source = max;
                }

                else
                {
                    source -= 1;
                }
            }

            if (consoleKey == ConsoleKey.S || consoleKey == ConsoleKey.DownArrow)
            {
                if (source == max)
                {
                    source = 1;
                }

                else
                {
                    source += 1;
                }
            }

            if (consoleKey == ConsoleKey.Enter)
            {
                return true;
            }

                
            return false;
            
        }

        private static bool prntSelectionSim(ref int cursorPos,List<string> lst,string selStart = ">> ",string selEnd = "",string start = "" ,string end = "")
        {

            for (int i = 1; i < lst.Count; i+= 1 )
            {
                string item = lst[i - 1];
                if (i == cursorPos)
                {
                    
                    Console.WriteLine(selStart + item + selEnd +'\n');
                   
                }

                else
                {
                    Console.WriteLine(start+ item + end +"\n");
                    

                }

                
            }

            if (selectionSim(ref cursorPos, playerInventory.Count))
            {
                return true;
                
            }
            return false;
        }

        private static void DisplayPlayerStats()
        {
            Console.Clear();

            Console.WriteLine("Health: " + playerHP);
            Console.WriteLine("Max Health: " + maxPlayerHP);
            Console.WriteLine("Strength: " + playerStrength);
            Console.WriteLine("Experience Points: " + playerXP);
            Console.WriteLine("Current Item Weight: " + currentItemWeight);
            Console.WriteLine("Maximum Carry Weight: " + maxCarryWeight);
            try
            {
                Console.WriteLine("Current Weapon: " + playerCurWeapon.Name);
            }
            catch { }

            WaitForInput();
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
        }

        private static void GainXP( int XP)
        {
            playerXP += XP;
            TW("You gained " + XP + " experience points!", 300);
        }

        static void Events()
        {

            //Fight monsters
            if (random.Next(1, 9) == 1)
            {
                int randMonster = random.Next(0, possibleMonsters.Count);
                Monster monster = new Monster(possibleMonsters[randMonster].Name, possibleMonsters[randMonster].Health, possibleMonsters[randMonster].Damage, possibleMonsters[randMonster].XP);
                monster.Damage += random.Next(-3, 4);
                monster.Health += random.Next(-3, 4);
                monster.XP += random.Next(-3, 4);

                TW("You come accross a " + monster.Name, 400);

                while (monster.Health > 0 && playerHP > 0)
                {
                    PlayerTakeDamage(monster.Damage);
                    System.Threading.Thread.Sleep(300);

                    try
                    {
                        int playerDamage = (int)(playerCurWeapon.Damage * playerCurWeapon.Quality) + random.Next(1, 4) + playerStrength;
                        monster.Health -= playerDamage;
                        Console.WriteLine("You deal " + playerDamage + " to the " + monster.Name + " with your " + playerCurWeapon.Name);
                    }
                    catch (NullReferenceException)
                    {
                        int playerDamage = random.Next(1, 4) + playerStrength;
                        monster.Health -= playerDamage;
                        Console.WriteLine("You deal " + playerDamage + " damage to the " + monster.Name + " with your fists");
                    }

                    System.Threading.Thread.Sleep(300);
                }

                if (monster.Health <= 0)
                {
                    GainXP(monster.XP);
                }
            }

            //Step on a chest
            if (currentPlayerTile == "c" && eventsEnabled == true)
            {

                string loot = possibleChestItems[random.Next(0, possibleChestItems.Count)].Name;

                TW("You open the chest and find an " + loot + "... Would you like to pick it up?  yes/no", 0);

                if (yesAnswers.Contains(GetInput()))
                {
                    if (PlayerGetItem(loot,1))
                    {
                        currentPlayerTile = "C";
                    }
                }
            }

            //step on a tree
            if (currentPlayerTile == "t" && eventsEnabled == true)
            {
                if (CheckInvtryName("axe"))
                {
                    TW("You have come across a tree... Would you like to chop it down?  yes/no", 0);

                    if (yesAnswers.Contains(GetInput()))
                    {
                        CurrentTileErase();
                        TW("You chopped down the tree", 400);
                        PlayerGetItem("wood",1);
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
            if (currentPlayerTile == "r" && eventsEnabled == true)
            {
                if (CheckInvtryName("pickaxe"))
                {
                    TW("You have come across a rock... Would you like to break it down?  yes/no", 0);

                    if (yesAnswers.Contains(GetInput()))
                    {
                        CurrentTileErase();
                        TW("You broke down the rock", 400);

                        PlayerGetItem("stone",1);

                        if (random.Next(1, 3) == 1)
                        {
                            PlayerGetItem("iron",1);
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

        static void UpdatePlayerLevel()
        {
            if (playerXP >= 30)
            {
                TW("You leveled up!", 300);
                maxCarryWeight = (int)(maxCarryWeight * 1.2);
                maxPlayerHP = (int)(maxPlayerHP * 1.2);
                playerStrength += 1;
                playerXP -= 30;

                DisplayPlayerStats();
            }
        }

        static void WaitForInput()
        {
            Console.WriteLine("\nPress any key to exit");
            while (Console.KeyAvailable == false)
            {

            }

            Console.Clear();
            PrintMap(Town.mapX, Town.mapY, Town.mapList);

            if (Console.KeyAvailable == true)
            {
                Console.ReadKey(true);
            }
        }

    }
}
