using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Hues_Adventure
{

    class GameObject
    {
        public string Name { get; set; }
        public string Print { get; set; }
        public ConsoleColor Colour { get; set; }
        public float Height { get; set; }
        public string Type { get; set; }
        public VectorTwoInt Position { get; set; }
        

        public GameObject(string name, string print, ConsoleColor colour,float height, string type)
        {
            Name = name;
            Print = print;
            Colour = colour;
            Height = height;
            Type = type;
        }

        public GameObject()
        {

        }
    }
    

    class Tile : List<GameObject>
    {

    }

    class Row : List<Tile>
    {

    }

    class Map : List<Row>
    {
        public VectorTwoInt Dimension = new VectorTwoInt(0,0);
        public bool isGenerated = false;

        public Map(int x, int y)
        {
            Dimension.X = x;
            Dimension.Y = y;
        }

        //stores the map as a list of strings

        //public List<string> mapList = new List<string>();
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

    class Material : Item
    {
        public Material(string Name, int Weight) : base(Name, Weight)
        {

        }
    }

    class Consumable : Item
    {
        public string BuffType { get; set; }
        public int Modifier { get; set; }

        public Consumable(string Name, int Weight, string buffType, int modifier) : base(Name, Weight)
        {
            BuffType = buffType;
            Modifier = modifier;
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

    class VectorTwoInt
    {

        public int X { get; set; }
        public int Y { get; set; }

        public VectorTwoInt(int x,int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is VectorTwoInt @int &&
                   X == @int.X &&
                   Y == @int.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }

    class Program
    {

        // Possible Maps

        static Map TheWild = new Map(30, 30);

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
            new Material("wood", 1),
            new Material("stone", 2),
            new Consumable("healing potion", 2 , "health", 25)
        };

        //Possible items that could be found in a chest
        static readonly List<Item> possibleChestItems = new List<Item>()
        {
            new Weapon("sword", 10, 10),
            new Weapon("dagger", 3, 5),
            new Weapon("axe", 14, 13),
            new Weapon("spear", 5, 10),
            new Weapon("pickaxe", 10, 7),
            new Consumable("healing potion", 2 , "health", 25)
        };

        //Stores all the possible game objects
        static List<GameObject> gameObjects = new List<GameObject>()
        {
            new GameObject("tree", " Y ", ConsoleColor.Green, 4, "feature"),
            new GameObject("boulder", " o ", ConsoleColor.Gray, 4, "feature"),
            new GameObject("chest", "[#]", ConsoleColor.DarkYellow, 4, "feature"),
            new GameObject("open chest", "[ ]", ConsoleColor.DarkYellow, 4, "feature"),
            new GameObject("player", " + ", ConsoleColor.Red, 6, "creature"),
            new GameObject("monster", " ! ", ConsoleColor.DarkMagenta, 5, "creature"),
            new GameObject("item", " # ", ConsoleColor.Blue, 3, "item"),
            new GameObject("blank", "   ", ConsoleColor.Black, 0, "placeholder")
        };

        //stores all the possible tiles and ratios of tiles used to generate maps
        static readonly List<string> randGameObjects = new List<string>()
        {

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

        //Answer List for selection sim
        static readonly List<string> answers = new List<string>()
        {
            "Yes",
            "No"
        };

        //A varible to see if the palyer wants to incratct with things
        static bool eventsEnabled = true;

        //Player Things-------------
        static bool playerAlive = false;
        static VectorTwoInt playerPos = new VectorTwoInt(0,0);
        static List<GameObject> currentPlayerTile = new List<GameObject>();
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

                GameReset(ref TheWild);

                //ask to play a new game

                if (PrintSelectionLoop("Would you like to start a new game?", answers) == 1)
                {
                    playerAlive = true;
                }

                else
                {
                    Environment.Exit(0);
                }

                MapInitialize(ref TheWild);

                PrintMap(TheWild);
                break;
                /*
                while (playerAlive)
                {

                    PlayerAction(ref TheWild);

                    Events(ref TheWild);

                    UpdatePlayerLevel(TheWild);

                    if (playerHP <= 0)
                    {
                        playerAlive = false;
                    }

                }

                Console.Clear();
                TW("-------------Game Over-------------", 0);*/
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
                    if (item.Weight * quantity + currentItemWeight <= maxCarryWeight)
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

                        currentItemWeight += item.Weight * quantity;
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

        static void GameReset(ref Map map)
        {
            playerAlive = false;
            playerPos.X = 0;
            playerPos.Y = 0;
            playerHP = 100;
            maxPlayerHP = 100;
            playerStrength = 1;
            playerInventory.Clear();
            currentItemWeight = 0;
            maxCarryWeight = 30;
            playerCurWeapon = null;

            map.isGenerated = false;
        }

        static void PrintMap(Map map)
        {
            foreach(Row row in map)
            {
                foreach(Tile tile in row)
                {
                    GameObject top = GetGameObject("blank");

                    foreach(GameObject gameObject in tile)
                    {
                        if (gameObject.Height > top.Height)
                        {
                            top = gameObject;
                        }
                    }

                    Console.ForegroundColor = top.Colour;
                    Console.Write(top.Print);
                }

                Console.WriteLine();      
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        static void MapInitialize(ref Map map)
        {
            map.Clear();

            for (int m = 0; m < map.Dimension.Y; m++)
            {
                Row row = new Row();

                for (int c = 0; c < map.Dimension.X; c++)
                {
                    row.Add(new Tile());
                }

                map.Add(row);
            }

            Generate(GetGameObject("tree"), ref map, 5);
            Generate(GetGameObject("boulder"), ref map, 3);
            Generate(GetGameObject("chest"), ref map, 0.5f);


            //spawn the player
            playerPos = new VectorTwoInt(random.Next(map.Dimension.X), random.Next(map.Dimension.Y) );
            

            //mapList[playerY] = mapList[playerY].Substring(0, playerX) + "p" + mapList[playerY].Substring(playerX + 1);
            PlaceGameObject(ref map, GetGameObject("player"),playerPos);
            
        }

        static void Generate(GameObject gameObject, ref Map map, float precentOfTiles)
        {
            for (int i = 0; i < map.Dimension.X * map.Dimension.Y / 100 * precentOfTiles; i++)
            {
                VectorTwoInt randomPos = RandPos(map);

                if (!IsObjectAtPos(gameObject, randomPos, map))
                {
                    PlaceGameObject(ref map, gameObject, randomPos);
                }

                else
                {
                    i--;
                }
            }
        }

        static List<GameObject> ReturnObjsAtPos(VectorTwoInt pos, Map map)
        {

            List<GameObject> list = new List<GameObject>();

            list.AddRange(map[pos.Y][pos.X]);

            return list;
        }

        static bool IsObjectAtPos(GameObject gameObject, VectorTwoInt pos ,Map map)
        {
            if (ReturnObjsAtPos(pos,map).Contains(gameObject))
            {
                return true;
            }

            return false;
        }

        static bool IsObjectAtPos(VectorTwoInt pos, Map map)
        {
            if (ReturnObjsAtPos(pos, map) == null)
            {
                return false;
            }

            return true;
        }

        static List<GameObject> MapObjects (Map map)
        {
            List<GameObject> list = new List<GameObject> ();
            foreach(Row row in map)
            {
                foreach(Tile tile in row)
                {
                    list.AddRange(tile);
                }
            }

            return list;
        }

        static VectorTwoInt RandPos (Map map)
        {
            
            return new VectorTwoInt(random.Next(map.Dimension.X), random.Next(map.Dimension.Y));
        }

        static void PlaceGameObject(ref Map map, GameObject gameObject, VectorTwoInt pos)
        {
            map[pos.Y][pos.X].Add(gameObject);
        }

        static void MapClear(ref Map map)
        {
            foreach (Row c in map)
            {
                foreach (Tile t in c)
                {
                    t.Clear();
                }
            }
        }

        static void GameObjectErase(ref Map map, GameObject gameObject)
        {
            map[playerPos.Y][playerPos.X].Remove(gameObject);
        }

        static void PlayerAction(ref Map map)
        {

            ConsoleKey thisInput = new ConsoleKey();

            while (Console.KeyAvailable == false)
            {

            }

            Console.Clear();

            if (Console.KeyAvailable == true)
            {
                thisInput = Console.ReadKey(true).Key;
                map[playerPos.Y][playerPos.X].Remove(GetGameObject("player"));
            }

            //Move UP
            ValidateMove(thisInput, ConsoleKey.W, playerPos.Y, 0);
            PlayerMoveUp(map, thisInput);

            //Move Down
            ValidateMove(thisInput, ConsoleKey.S, playerPos.Y, map.Dimension.Y - 1);
            PlayerMoveDown(map, thisInput);

            //Move Left
            ValidateMove(thisInput, ConsoleKey.A, playerPos.X, 0);
            PlayerMoveLeft(map, thisInput);

            //Move Right
            ValidateMove(thisInput, ConsoleKey.D, playerPos.X, map.Dimension.X - 1);
            PlayerMoveRight(map, thisInput);

            PlaceGameObject(ref map, GetGameObject("player"), playerPos);

            PrintMap(map);

            //display inventory
            if (thisInput == ConsoleKey.Tab)
            {
                DisplayInventory(map);
                PlayerAction(ref map);
            }

            //quit the game
            if (thisInput == ConsoleKey.Escape)
            {
                Environment.Exit(0);
            }

            //Disable Interactions With Game objects
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

                PlayerAction(ref map);
            }

            if (thisInput == ConsoleKey.E)
            {
                DisplayPlayerStats(map);
                PlayerAction(ref map);
            }
        }

        static void PlayerMoveRight(Map map, ConsoleKey thisInput)
        {
            if (thisInput == ConsoleKey.D && playerPos.X < map.Dimension.X - 1)
            {
                playerPos.X += 1;
            }
        }

        static void PlayerMoveLeft(Map map, ConsoleKey thisInput)
        {
            if (thisInput == ConsoleKey.A && playerPos.X > 0)
            {
                playerPos.X -= 1;

            }
        }

        static void PlayerMoveDown(Map map, ConsoleKey thisInput)
        {
            if (thisInput == ConsoleKey.S && playerPos.Y < map.Dimension.Y - 1)
            {
                playerPos.Y += 1;
            }
        }

        static void PlayerMoveUp(Map map, ConsoleKey thisInput)
        {
            if (thisInput == ConsoleKey.W && playerPos.Y > 0)
            {
                playerPos.Y -= 1;
            }
        }

        static void DisplayInventory(Map map)
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

                if (playerInventory.Count != 0)
                {
                    if (ChooseSelection(ref cursorPos, playerInventory.Count))
                    {
                        DisplayItem(playerInventory[cursorPos - 1], cursorPos - 1);
                        WaitForInput(map);
                        break;
                    }
                }

                else
                {
                    break;
                }
            }
        }

        static void DisplayItem(Item item, int index)
        {
            if (item.GetType() == typeof(Material))
            {
                Material material = (Material)item;
                List<string> options = new List<string>() { "Craft", "Drop", "Exit" };

                int cursorPos = 1;

                while (true)
                {
                    Console.Clear();

                    Console.WriteLine("-----" + material.Name + "-----");
                    Console.WriteLine("Quantity: " + material.Quantity);
                    Console.WriteLine("Total Weight: " + (int)(material.Weight * material.Quantity));

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

                    if (ChooseSelection(ref cursorPos, options.Count))
                    {
                        if (options[cursorPos - 1] == options[0])
                        {
                            if (material.Quantity == 1)
                            {
                                playerInventory.RemoveAt(index);
                            }

                            else
                            {
                                material.Quantity -= 1;
                            }

                            Console.WriteLine("This function doesn't function yet");

                            break;
                        }

                        if (options[cursorPos - 1] == options[1])
                        {
                            if (material.Quantity == 1)
                            {
                                playerInventory.RemoveAt(index);
                            }

                            else
                            {
                                material.Quantity -= 1;
                            }

                            break;
                        }

                        if (options[cursorPos - 1] == options[2])
                        {
                            break;
                        }
                    }
                }
            }

            if (item.GetType() == typeof(Consumable))
            {
                Consumable consumable = (Consumable)item;
                List<string> options = new List<string>() { "Use", "Drop", "Exit" };

                int cursorPos = 1;

                while (true)
                {
                    Console.Clear();

                    Console.WriteLine("-----" + consumable.Name + "-----");
                    Console.WriteLine("Quantity: " + consumable.Quantity);
                    Console.WriteLine("Info: " + consumable.BuffType.Substring(0, 1).ToUpper() + consumable.BuffType.Substring(1) + " +" + consumable.Modifier);
                    Console.WriteLine("Total Weight: " + (int)(consumable.Weight * consumable.Quantity));

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

                    if (ChooseSelection(ref cursorPos, options.Count))
                    {
                        if (options[cursorPos - 1] == options[0])
                        {
                            if (consumable.Quantity == 1)
                            {
                                playerInventory.RemoveAt(index);
                            }

                            else
                            {
                                consumable.Quantity -= 1;
                            }

                            PlayerStatChange(consumable.BuffType, consumable.Modifier);

                            break;
                        }

                        if (options[cursorPos - 1] == options[1])
                        {
                            if (consumable.Quantity == 1)
                            {
                                playerInventory.RemoveAt(index);
                            }

                            else
                            {
                                consumable.Quantity -= 1;
                            }

                            break;
                        }

                        if (options[cursorPos - 1] == options[2])
                        {
                            break;
                        }
                    }
                }
            }

            if (item.GetType() == typeof(Weapon))
            {
                Weapon weapon = (Weapon)item;
                List<string> options = new List<string>() { "Equip", "Drop", "Exit" };

                int cursorPos = 1;

                while (true)
                {
                    Console.Clear();

                    Console.WriteLine("-----" + weapon.Name + "-----");
                    Console.WriteLine("Damage: " + weapon.Damage);
                    Console.WriteLine("Quality: " + weapon.Quality);
                    Console.WriteLine("Total Weight: " + (int)(weapon.Weight * weapon.Quantity));

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

                    if (ChooseSelection(ref cursorPos, options.Count))
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

        static void PlayerStatChange(string stat, int modifier)
        {
            if (stat == "health")
            {
                GainHP(modifier);
            }

            if (stat == "maxHealth")
            {
                maxPlayerHP += modifier;
            }
          
            if (stat == "strength")
            {
                playerStrength += modifier;
            }
        }

        static bool ChooseSelection(ref int source, int max)
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

        static int PrintSelectionLoop(string message, List<string> list, string selStart = ">> ", string selEnd = "", string start = " - ", string end = "")
        {
            int cursorPos = 1;
            bool hasRun = false;

            while (true)
            {
                Console.Clear();

                if (hasRun)
                {
                    Console.WriteLine(message);
                }

                else
                {
                    TW(message, 0);
                    hasRun = true;
                }

                if (PrintSelection(ref cursorPos, list, selStart, selEnd, start, end))
                {
                    Console.Clear();
                    return cursorPos;
                }
            }
        }

        static bool PrintSelection(ref int cursorPos,List<string> list, string selStart = ">> ", string selEnd = "", string start = " - ", string end = "")
        {
            for (int i = 1; i <= list.Count; i++)
            {
                string option = list[i - 1];

                if (i == cursorPos)
                {
                    Console.WriteLine(selStart + option + selEnd);
                }

                else
                {
                    Console.WriteLine(start + option + end);
                }
            }

            if (ChooseSelection(ref cursorPos, list.Count))
            {
                return true;
            }
            return false;
        }

        static void DisplayPlayerStats(Map map)
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

            WaitForInput(map);
        }

        static void ValidateMove(ConsoleKey thisInput, ConsoleKey checkKey, int playerXY, int checkWall)
        {
            if (thisInput == checkKey && playerXY == checkWall)
            {
                Console.WriteLine("You cant move there");
            }
        }

        static void GainHP (int HP)
        {
            playerHP += HP;
            TW("You gained " + HP + " hit points", 300);
        }

        static void PlayerTakeDamage(int damage)
        {
            playerHP -= damage;
            Console.WriteLine("You take " + damage + " damage!");
        }

        static void GainXP(int XP)
        {
            playerXP += XP;
            TW("You gained " + XP + " experience points!", 300);
        }

        static void Events(ref Map map)
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
            if (map[playerPos.Y][playerPos.X].Contains(GetGameObject("chest")) && eventsEnabled == true)
            {

                string loot = possibleChestItems[random.Next(0, possibleChestItems.Count)].Name;

                TW("You open the chest and find an " + loot + "... Would you like to pick it up?  yes/no", 0);

                if (yesAnswers.Contains(GetInput()))
                {
                    if (PlayerGetItem(loot, 1))
                    {
                        map[playerPos.Y][playerPos.X].Remove(GetGameObject("chest"));
                        map[playerPos.Y][playerPos.X].Add(GetGameObject("open chest"));
                    }
                }
            }

            //step on a tree
            if (map[playerPos.Y][playerPos.X].Contains(GetGameObject("tree")) && eventsEnabled == true)
            {
                if (CheckInvtryName("axe"))
                {
                    TW("You have come across a tree... Would you like to chop it down?  yes/no", 0);

                    if (yesAnswers.Contains(GetInput()))
                    {
                        GameObjectErase(ref map, GetGameObject("tree"));
                        TW("You chopped down the tree", 400);
                        PlayerGetItem("wood", 1);
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
            if (map[playerPos.Y][playerPos.X].Contains(GetGameObject("rock")) && eventsEnabled == true)
            {
                if (CheckInvtryName("pickaxe"))
                {
                    TW("You have come across a rock... Would you like to break it down?  yes/no", 0);

                    if (yesAnswers.Contains(GetInput()))
                    {
                        GameObjectErase(ref map, GetGameObject("rock"));
                        TW("You broke down the rock", 400);

                        PlayerGetItem("stone", 1);

                        if (random.Next(1, 3) == 1)
                        {
                            PlayerGetItem("iron", 1);
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

        static void UpdatePlayerLevel(Map map)
        {
            if (playerXP >= 30)
            {
                TW("You leveled up!", 300);
                maxCarryWeight = (int)(maxCarryWeight * 1.2);
                maxPlayerHP = (int)(maxPlayerHP * 1.2);
                playerStrength += 1;
                playerXP -= 30;

                DisplayPlayerStats(map);
            }
        }
      
        static void WaitForInput(Map map)
        {
            Console.WriteLine("\nPress any key to exit");
            while (Console.KeyAvailable == false)
            {

            }

            Console.Clear();
            PrintMap(map);

            if (Console.KeyAvailable == true)
            {
                Console.ReadKey(true);
            }
        }

        static GameObject GetGameObject(string name)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                if (name == gameObject.Name)
                {
                    return gameObject;
                }
            }

            return null;
        }
    }
}

