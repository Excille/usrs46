using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
namespace JeuDeCombat
{
    struct RatioGetter
    {
        public float percentAi1;
        public float percentAi2;
        public float percentDraw;
        public void SetValue(float _percentAi1, float _percentAi2, float _percentDraw)
        {
            percentAi1 = _percentAi1;
            percentAi2 = _percentAi2;
            percentDraw = _percentDraw;
        }
    }

    class Program
    {
        static List<string> availableClass = new List<string> { "D", "H", "T","V" };
        static bool difficulty = false;
        static void Main(string[] args)
        {
            //string fileName = @"C:\Users\b\Desktop\TheoAscii.txt";
            //string text = File.ReadAllText(fileName);
            //Console.WriteLine(text);

            Init();

        }

        static void Init()
        {
            int choice = -1;
            PrintWelcome();
            do
                PrintChooseGameMode();
            while (!int.TryParse(Console.ReadLine(), out choice) || choice <= 0 || choice > 3);

            switch (choice)
            {
                case 1:
                    //Lauch game
                    Console.Clear();
                    SetUpPlayerVsAi(availableClass);
                    break;
                case 2:
                    Console.Clear();
                    Simulation(availableClass);
                    //Trigger Simulation
                    break;
                case 3:
                    //quit
                    break;

            }
        }


        #region Player vs AI

        static void SetUpPlayerVsAi(List<string> availableClass)
        {
            PrintWelcome();

            var choice = -1;
            Random rdm = new Random();


            var dif = 0;
            
            do
                ChooseDifficulty();
            while (!int.TryParse(Console.ReadLine(), out dif) || dif < 1 || dif > 2);

            difficulty = dif == 2 ? true : false;
            Console.Clear();
            PrintWelcome();
            do
                PrintChooseCharacter();
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > availableClass.Count +1);

            if (choice == availableClass.Count + 1)
            {
                Console.Clear();
                Main(null);
                return;
            }

            var player = availableClass[choice-1];

            Console.Clear();

            PrintWelcome();

            Console.WriteLine();
            Console.Write("Vous avez choisi d'incarner un ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(GetFullName(player).ToUpper()+'\n');
            Console.ResetColor();

            choice = rdm.Next(0, availableClass.Count);
            var ai = availableClass[choice];
            Console.Write("Quand à votre adversaire, il choisit d'incarner un ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(GetFullName(ai).ToUpper()+'\n');
            Console.ResetColor();

            var sleepTime = 5000;
            Console.WriteLine("Début du combat dans "+sleepTime /1000+ " seconde(s)!");
            Thread.Sleep(sleepTime);
            Battle(player, ai);
            
        }

        static void Battle( string playerRole, string aiRole )
        {
            Console.Clear();
            PrintWelcome();
            Console.WriteLine("\nQue le combat commence !");

            var playerPv = GetHpByClass(playerRole);
            var aiPv = GetHpByClass(aiRole);
            //PrintPlayerStatus(playerRole, aiRole, playerPv, aiPv);
            int round = 0;
            while(playerPv > 0 && aiPv > 0)
            {
                round++;

                PrintRountStat(round);

                PrintPlayerStatus(playerRole, aiRole, playerPv, aiPv);

                //Boucle
                var playerChoice = 0;
                var aiChoice = 0;
                Random rdm = new Random();
                do
                    PrintChooseAction();
                while (!int.TryParse(Console.ReadLine(), out playerChoice) || playerChoice < 1 || playerChoice > 4);

                if(playerChoice == 4)
                {
                    Console.Clear();
                    Main(null);
                    return;
                    //Abandonner
                    //return;
                }


                if(!difficulty)
                    aiChoice = rdm.Next(1, 4);
                else
                {
                    switch(playerChoice)
                    {
                        case 1:
                            aiChoice = 2;
                            break;
                        case 2:
                            aiChoice = 3;
                            break;
                        case 3:
                            aiChoice = 1;
                            break;
                    }

                    if (aiChoice - DommageParRole(playerRole) <= 0)
                    {
                        aiChoice = 2;
                    }
                    if (playerPv <= DommageParRole(aiRole))
                    {
                        aiChoice = 1;
                    }

                }

                var trade = ResolutionAction(playerChoice, aiChoice, playerRole, aiRole, true);

                if (aiPv + trade.Item2 > GetHpByClass(aiRole))
                    aiPv = GetHpByClass(aiRole);
                else
                    aiPv += trade.Item2;

                if (playerPv + trade.Item1 > GetHpByClass(playerRole))
                    playerPv = GetHpByClass(playerRole);
                else
                    playerPv += trade.Item1;

                Console.WriteLine();

                PrintRoundResults(trade, playerRole, aiRole);

                Thread.Sleep(2000);

                Console.WriteLine();

                
            }

            if(playerPv <= 0 && aiPv <= 0)
            {
                var choice1 = -1;
                Console.WriteLine("Égalité ! Les deux joueurs sont morts !");
                do
                    PrintTryAgain();
                while (!int.TryParse(Console.ReadLine(), out choice1) || choice1 <= 0 || choice1 > 2);

                switch (choice1)
                {
                    case 1:
                        Console.Clear();
                        SetUpPlayerVsAi(availableClass);
                        break;
                    case 2:
                        Console.Clear();
                        Main(null);
                        break;
                }
                return;
            }

            if(playerPv <= 0)
                Console.WriteLine("Perdu ! L'IA vous a vaicu !");

            if (aiPv <= 0)
                Console.WriteLine("Félicitation ! Vous avez vaincu !");

            int choice = -1;

            do
                PrintTryAgain();
            while (!int.TryParse(Console.ReadLine(), out choice) || choice <= 0 || choice > 2);

            switch(choice)
            {
                case 1:
                    Console.Clear();
                    SetUpPlayerVsAi(availableClass);
                    break;
                case 2:
                    Console.Clear();
                    Main(null);
                    break;
            }
        }

        #endregion

        #region SIMULATION
        static void Simulation(List<string> availableClass)
        {
            int choice = -1;

            do
                PrintChooseAiForTest();
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 8);

            if(choice == 8)
            {
                Console.Clear();
                Main(null);
                return;
            }

            int nbSimulation = -1;

            do
                PrintChooseNbOfSimulation();
            while (!int.TryParse(Console.ReadLine(), out nbSimulation) || nbSimulation < 0);

            int choice2 = -1;

            bool showDebug = false;

            if(choice < 7)
            {
                do
                    PrintChooseDebug();
                while (!int.TryParse(Console.ReadLine(), out choice2) || choice2 < 1 || choice2 > 2);

                showDebug = choice2 == 1 ? true : false;

            }

            switch(choice)
            {
                case 1:
                    //Damager vs Healer
                    AiVsAi(availableClass[0], availableClass[1], nbSimulation, showDebug);
                    break;
                case 2:
                    //Damager vs Tank
                    AiVsAi(availableClass[0], availableClass[2], nbSimulation, showDebug);
                    break;
                case 3:
                    //Damager vs Vampire
                    AiVsAi(availableClass[0], availableClass[3], nbSimulation, showDebug);
                    break;
                case 4:
                    //Tank vs Healer
                    AiVsAi(availableClass[1], availableClass[2], nbSimulation, showDebug);
                    break;
                case 5:
                    //Vampire vs Healer
                    AiVsAi(availableClass[3], availableClass[1], nbSimulation, showDebug);
                    break;
                case 6:
                    //Tank vs Vampire
                    AiVsAi(availableClass[2], availableClass[3], nbSimulation, showDebug);
                    break;
                case 7:
                    //Test everyone
                    TestAllAi(availableClass, nbSimulation);
                    break;
               
            }

            do
            {
                PrintTryMoreTest();
            } while (!int.TryParse(Console.ReadLine(), out choice) || choice <= 0 || choice > 2);

                Console.Clear();
            if (choice == 1)
            {
                Simulation(availableClass);
            }
            else
                Main(null);
        }

        static void AiVsAi(string ai1, string ai2, int nbSimulation, bool showDebug)
        {
            var ratio = BattleAi(ai1, ai2, nbSimulation, showDebug);
            Console.WriteLine("Pourcentage d'égalité : " + ratio.percentDraw + "%");
            Console.WriteLine("Pourcentage de victoire du "+ GetFullName(ai1)+ " : "  + ratio.percentAi1 + "%");
            Console.WriteLine("Pourcentage de victoire du "+ GetFullName(ai2)+ " : "  + ratio.percentAi2 + "%");
        }

        static void TestAllAi(List<string> availableClass, int nbSimulation)
        {

            Console.WriteLine(nbSimulation + " simulations");

            //Damager VS Healer
            //Console.WriteLine("DAMAGER vs HEALER");
            var ai1 = availableClass[0];
            var ai2 = availableClass[1];
            var ratioDamagerHealer = BattleAi(ai1, ai2, nbSimulation);

            //Damager vs TANK
            //Console.WriteLine("DAMAGER vs TANK");
            ai1 = availableClass[0];
            ai2 = availableClass[2];
            var ratioDamagerTank = BattleAi(ai1, ai2, nbSimulation);

            //Healer vs TANK
            //Console.WriteLine("DAMAGER vs TANK");
            ai1 = availableClass[1];
            ai2 = availableClass[2];
            var ratioHealerTank = BattleAi(ai1, ai2, nbSimulation);

            //Damager vs Vampire
            ai1 = availableClass[0];
            ai2 = availableClass[3];
            var ratioDamagerVampire = BattleAi(ai1, ai2, nbSimulation);

            //Tank vs vampire
            ai1 = availableClass[2];
            ai2 = availableClass[3];
            var ratioTankVampire = BattleAi(ai1, ai2, nbSimulation);

            //Vampire vs Healer
            ai1 = availableClass[3];
            ai2 = availableClass[1];
            var ratioVampireHealer = BattleAi(ai1, ai2, nbSimulation);

            PrintResultsTable(ratioDamagerHealer, ratioDamagerTank, ratioHealerTank, ratioDamagerVampire, ratioTankVampire, ratioVampireHealer);
        }

        static RatioGetter BattleAi(string ai1, string ai2, int nbSimulation, bool debug = false)
        {

            int round = 0;
            float ai1VictoryCount = 0f;
            float ai2VictoryCount = 0f;
            float drawCount = 0f;
            Random rdm = new Random();

            for (int i = 0; i < nbSimulation; i++)
            {
                //ResetPV 
                var ai1Pv = GetHpByClass(ai1);
                var ai2Pv = GetHpByClass(ai2);
                round = 0;

                while (ai1Pv > 0 && ai2Pv > 0)
                {
                    round++;
                    if (debug)
                        Console.WriteLine("Round n°"+round);
                    var ai1Choice = rdm.Next(1, 4);
                    var ai2Choice = rdm.Next(1, 4);

                    if(debug)
                    {
                        Console.WriteLine(ai1 + " choose " + ai1Choice);
                        Console.WriteLine(ai2 + " choose " + ai2Choice);
                    }

                    var tradeResult = ResolutionAction(ai1Choice, ai2Choice, ai1, ai2);

                    //Gestion des pv max
                    if(ai2Pv + tradeResult.Item2 > GetHpByClass(ai2))
                        ai2Pv = GetHpByClass(ai2);
                    else
                        ai2Pv += tradeResult.Item2;

                    if (ai1Pv + tradeResult.Item1 > GetHpByClass(ai1))
                        ai1Pv = GetHpByClass(ai1);
                    else
                        ai1Pv += tradeResult.Item1;


                    if (debug)
                    {
                        Console.WriteLine(ai1Pv + " pv AI1");
                        Console.WriteLine(ai2Pv + " pv AI2");
                        Console.WriteLine("=========================");
                    }
                }


                if (!(ai1Pv <= 0 && ai2Pv <= 0))
                {
                    if (ai1Pv <= 0)
                    {
                        ai2VictoryCount++;
                        if (debug)
                        {
                            Console.WriteLine("\n"+GetFullName(ai2) + " a gagné en "+round +" round(s)");
                            Console.WriteLine("________________________\n");
                        }
                    }

                    if (ai2Pv <= 0)
                    {
                        ai1VictoryCount++;
                        if (debug)
                        {
                            Console.WriteLine("\n"+GetFullName(ai1) + " a gagné en "+round +" round(s)");
                            Console.WriteLine("________________________\n");
                        }
                    }
                }
                else
                {
                    drawCount++;
                    if (debug)
                    {
                        Console.WriteLine(" Égalité en +"+round+" round(s)!");
                        Console.WriteLine("________________________\n");
                    }
                }

            }

            //Console.WriteLine("DRAW : " + drawCount);
            //Console.WriteLine( ai1+ " victory : " + ai1VictoryCount);
            //Console.WriteLine(ai2 + " victory : " + ai2VictoryCount);

            //Console.WriteLine("Draw ratio : " + (drawCount / nbSimulation) * 100 + "%");
            //Console.WriteLine(ai1 + " victory percentage : " + (ai1VictoryCount / nbSimulation) * 100 + "%");
            //Console.WriteLine(ai2 + " victory percentage : " + (ai2VictoryCount / nbSimulation) * 100 + "%");
            //Console.WriteLine("==================================");

            RatioGetter ratioGetter = new RatioGetter();
            ratioGetter.SetValue(ai1VictoryCount / nbSimulation * 100, 
                ai2VictoryCount / nbSimulation * 100,
                drawCount / nbSimulation * 100);
            return ratioGetter;
        }

        #endregion

        #region Print functions
        static void PrintResultsTable(RatioGetter ratioDamagerHealer, RatioGetter ratioDamagerTank, RatioGetter ratioHealerTank
            , RatioGetter ratioDamagerVampire, RatioGetter ratioTankVampire, RatioGetter ratioVampireHealer)
        {
            int allPlace = 16;
            Console.WriteLine("======================================================================================");
            Console.WriteLine("|                |                |                |                |                |");
            Console.WriteLine("|      X         |    DAMAGER     |     HEALER     |     TANK       |    VAMPIRE     |");
            Console.WriteLine("|                |                |                |                |                |");
            Console.WriteLine("======================================================================================");
            Console.WriteLine("|                |                |                |                |                |");
            Console.WriteLine("|   DAMAGER      |       X        |"+ FixValuePrint(ratioDamagerHealer.percentAi1, allPlace) +"|"+FixValuePrint(ratioDamagerTank.percentAi1, allPlace)+"|"+FixValuePrint(ratioDamagerVampire.percentAi1, allPlace)+"|");
            Console.WriteLine("|                |                |                |                |                |");
            Console.WriteLine("======================================================================================");
            Console.WriteLine("|                |                |                |                |                |");
            Console.WriteLine("|   HEALER       |" + FixValuePrint(ratioDamagerHealer.percentAi2, allPlace) + "|       X        |" + FixValuePrint(ratioHealerTank.percentAi1, allPlace) + "|" + FixValuePrint(ratioVampireHealer.percentAi2,allPlace) + "|") ;
            Console.WriteLine("|                |                |                |                |                |");
            Console.WriteLine("======================================================================================");
            Console.WriteLine("|                |                |                |                |                |");
            Console.WriteLine("|    TANK        |"+FixValuePrint(ratioDamagerTank.percentAi2,allPlace)+"|"+FixValuePrint(ratioHealerTank.percentAi2,allPlace)+"|       X        |"+FixValuePrint(ratioTankVampire.percentAi1, allPlace)+"|");
            Console.WriteLine("|                |                |                |                |                |");
            Console.WriteLine("======================================================================================");
            Console.WriteLine("|                |                |                |                |                |");
            Console.WriteLine("|   VAMPIRE      |"+FixValuePrint(ratioDamagerVampire.percentAi2,allPlace)+"|"+FixValuePrint(ratioVampireHealer.percentAi1, allPlace)+"|"+FixValuePrint(ratioTankVampire.percentAi2,allPlace)+"|        X       |");
            Console.WriteLine("|                |                |                |                |                |");
            Console.WriteLine("======================================================================================");



        }

        static void PrintRoundResults(Tuple<int,int> trade, string playerRole, string aiRole)
        {
            if(trade.Item1 == 0 && trade.Item2 == 0)
            {
                Console.WriteLine("Personne n'a subi de dégât");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;

            if (trade.Item1 < 0)
                Console.WriteLine( "Le "+GetFullName(playerRole) + " a reçu " + Math.Abs(trade.Item1) + " point(s) de dégât");
            
            if (trade.Item1 > 0)
                Console.WriteLine( "Le "+GetFullName(playerRole) + " a regagné " + Math.Abs(trade.Item1) + " point(s) de vie");

            if (trade.Item1 == 0)
                Console.WriteLine( "Le "+GetFullName(playerRole) + " n'a pris aucun dégât");

            Console.ForegroundColor = ConsoleColor.Red;
            //===================

            if (trade.Item2 < 0)
                Console.WriteLine( "Le "+GetFullName(aiRole) + " a reçu " + Math.Abs(trade.Item2) + " point(s) de dégât");

            if (trade.Item2 > 0)
                Console.WriteLine( "Le "+GetFullName(aiRole) + " a regagné " + Math.Abs(trade.Item2) + " point(s) de vie");

            if (trade.Item2 == 0)
                Console.WriteLine("Le "+GetFullName(aiRole) + " n'a pris aucun dégât");

            Console.ResetColor();

        }

        static void PrintPlayerStatus(string playerRole, string aiRole, int playerPv, int aiPv)
        {
            //Console.WriteLine("[" + playerPv + "pvs]" +GetFullName(playerRole) + "     [" + aiPv +"pvs]"+GetFullName(aiRole));
            //Console.WriteLine("[Force : "+DommageParRole(playerRole)+ "]    [Force : "+ DommageParRole(aiRole)+"]");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[" + playerPv + "PV] " + GetFullName(playerRole));
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("     [" + aiPv + "PV] " + GetFullName(aiRole)+'\n');
            Console.ResetColor();

        }

        static void PrintWelcome()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("             ^     \\    /      ^       ");
            Console.WriteLine("            / \\    )\\__/(     / \\       ");
            Console.WriteLine("           /   \\  (_\\  /_)   /   \\      ");
            Console.ResetColor();

            Console.Write("      ____");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("/");
            Console.ResetColor();
            Console.Write("_____");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("\\");
            Console.ResetColor();
            Console.Write("__");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("\\@  @/");
            Console.ResetColor();
            Console.Write("___");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("/");
            Console.ResetColor();
            Console.Write("_____");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("\\");
            Console.ResetColor();

            Console.Write("____\n");
            //Console.WriteLine("      ____/_____\\__\\@  @/___/_____\\____");

            Console.Write("     |             ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("|\\../|");
            Console.ResetColor();
            Console.Write("              |\n");

            //Console.WriteLine("     |             |\\../|              |");

            Console.Write("     |              ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("\\VV/");
            Console.ResetColor();
            Console.Write("               |\n");

            //Console.WriteLine("     |              \\VV/               |");
            Console.WriteLine("     |        BATTLE IN DRACONIA       |");
            Console.WriteLine("     |_________________________________|");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("      |    /\\ /      \\        \\ /\\    | ");
            Console.WriteLine("      |  /   V        ))       V  \\   | ");
            Console.WriteLine("      |/     `       //        '    \\ | ");
            Console.WriteLine("      `              V                '");
            Console.ResetColor();
            Console.WriteLine("  ============================================");

            Console.ResetColor();
        }

        static void PrintTryAgain()
        {
            Console.WriteLine("Voulez vous recommencer ?");
            Console.WriteLine("1 - Oui");
            Console.WriteLine("2 - Non");
        }

        static void PrintChooseCharacter()
        {
            Console.WriteLine("\nVeuillez choisir votre champion !\n");

            #region Damager
            Console.WriteLine("1 - DAMAGER");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("    PV : " + GetHpByClass("D"));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("    Force : " + DommageParRole("D"));
            Console.ResetColor();
            Console.WriteLine("    Action spéciale : Inflige en retour les dégâts qui lui sont infligés durant ce tour.");
            Console.WriteLine("    Les dégâts sont quand même subis.\n");
            #endregion

            #region Healer
            Console.WriteLine("2 - HEALER");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("    PV : " + GetHpByClass("H"));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("    Force : " + DommageParRole("H"));
            Console.ResetColor();
            Console.WriteLine("    Action spéciale : Récupère 2 points de vie.\n");
            #endregion

            #region Tank
            Console.WriteLine("3 - TANK");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("    PV : " + GetHpByClass("T"));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("    Force : " + DommageParRole("T"));
            Console.ResetColor();
            Console.WriteLine("    Action spéciale : Sacrifie un de ses points de vie pour augmenter sa force d’attaque de 1 et ce uniquement durant le tour en cours.\n");
            #endregion

            #region Vampire
            Console.WriteLine("4 - VAMPIRE");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("    PV : " + GetHpByClass("V"));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("    Force : " + DommageParRole("V"));
            Console.ResetColor();
            Console.WriteLine("    Action spéciale : Inflige 1 point de dégât à la cible et a 50 % de chance de regagner un point de vie.\n");


            #endregion

            Console.WriteLine("5 - RETOUR ");
        }

        static void PrintChooseAction()
        {
            Console.WriteLine("\nVeuillez choisir une action !\n");
            Console.WriteLine("1 - Attaquer");
            Console.WriteLine("2 - Défendre");
            Console.WriteLine("3 - Action spéciale");
            Console.WriteLine("4 - Abandonner (abandon immédiat)");
        }

        static void ChooseDifficulty()
        {
            Console.WriteLine("\nVeuillez choisir le cerveau adverse : ");
            Console.WriteLine("1 - Random");
            Console.WriteLine("2 - Fumé");
        }

        static void PrintChooseNbOfSimulation()
        {
            Console.WriteLine("Combien de simulation voulez vous faire ?");
        }

        static void PrintChooseDebug()
        {
            Console.WriteLine("Voulez vous afficher tout les combats ?");
            Console.WriteLine("1 - Oui");
            Console.WriteLine("2 - Non");

        }

        static void PrintChooseGameMode()
        {
            Console.WriteLine("\nBienvenue à Draconia ! Que voulez vous faire ?\n");
            Console.WriteLine("1 - Jouer");
            Console.WriteLine("2 - Tester les IA");
            Console.WriteLine("3 - Quitter ");

        }

        static void PrintChooseAiForTest()
        {
            int nbr = 0;
            Console.WriteLine("Que voulez-vous tester ?");

            for (int i = 0; i < availableClass.Count; i++)
            {
                for (int j = i+1; j < availableClass.Count; j++)
                {
                    nbr++;
                    Console.WriteLine(nbr +" - "+ GetFullName(availableClass[i]) +" VS "+ GetFullName(availableClass[j]));
                }
            }

            Console.WriteLine("7 - Tout tester");
            Console.WriteLine("8 - Retour");
        }

        static void PrintTryMoreTest()
        {
            Console.WriteLine("\nVoulez tester autre chose ?");
            Console.WriteLine("1 - Oui");
            Console.WriteLine("2 - Non");
        }

        static void PrintRountStat(int round)
        {
            Console.WriteLine("\n===== DÉBUT DU ROUND " + round + " =====\n");
        }


        #endregion

        #region Coding Rooms function
        static Tuple<int, int> ResolutionAction(int actionJoueur, int actionAi, string roleJoueur, string roleIA, bool narative = false)
        {
            int playerValueModifier = 0;
            int aiValueModifier = 0;

            bool playerSkillActivated = actionJoueur == 3;
            bool aiSkillActivated = actionAi == 3;

            bool playerDefending = actionJoueur == 2;
            bool aiDefending = actionAi == 2;

            //Tour du joueur

            Console.ForegroundColor = ConsoleColor.Green;

            switch(actionJoueur)
            {
                case 1:
                    if (narative)
                        Console.WriteLine("Le " + GetFullName(roleJoueur) + " attaque!");
                    aiValueModifier -= DommageParRole(roleJoueur);
                    break;
                case 2:
                    if(narative)
                        Console.WriteLine("Le " + GetFullName(roleJoueur) + " se défend!");
                    break;
                case 3:
                    playerSkillActivated = true;
                    switch(roleJoueur)
                    {
                        case "D":
                            if (narative)
                                Console.WriteLine("Le " + GetFullName(roleJoueur)+ " active sa compétence ! Il renverra tous les dégâts qu'il aura subi ce tour.");
                            break;
                        case "H":
                            if (narative)
                                Console.WriteLine("Le " + GetFullName(roleJoueur) + " active sa compétence ! Il se restaure 2 points de vie !");
                            playerValueModifier += 2;
                            break;
                        case "T":
                            if (narative)
                                Console.WriteLine("Le " + GetFullName(roleJoueur) + " active sa compétence ! Il sacrifie un point de vie pour augmenter sa force de 1 et attaque!");
                            playerValueModifier -= 1;
                            aiValueModifier -= DommageParRole(roleJoueur) + 1;
                            break;
                        case "V":
                            if (narative)
                                Console.WriteLine("Le " + GetFullName(roleJoueur) + " active sa compétence ! Il mord le cou de l'adversaire pour tenter de gagner un point de vie!");
                            aiValueModifier -= 1;

                            Random rdm = new Random();
                            var dice = rdm.Next(1, 101);

                            if(dice > 50)
                            {
                                //Rater
                                if (narative)
                                    Console.WriteLine("Le " + GetFullName(roleJoueur) + " n'a pas pu se soigner");
                            }
                            else
                            {
                                //Gagner
                                if(narative)
                                    Console.WriteLine("Le " + GetFullName(roleJoueur) + " a réussi son coup ! Il se soigne 1 point de vie !");
                                playerValueModifier += 1;
                            }
                            break;
                    }
                    break;
            }

            Console.ForegroundColor = ConsoleColor.Red;

            switch(actionAi)
            {
                case 1:
                    if(narative)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Le " + GetFullName(roleIA) + " attaque!");
                        Console.ResetColor();
                    }
                    playerValueModifier -= DommageParRole(roleIA);
                    break;

                case 2:
                    if (narative)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Le " + GetFullName(roleIA) + " se défend!");
                        Console.ResetColor();
                    }
                    break;

                case 3:
                    aiSkillActivated = true;
                    switch (roleIA)
                    {
                        case "D":
                            if (narative)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Le " + GetFullName(roleIA) + " active sa compétence ! Il renverra tous les dégâts qu'il aura subi ce tour");
                                Console.ResetColor();
                            }
                            break;
                        case "H":
                            if (narative)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Le " + GetFullName(roleIA) + " active sa compétence ! Il se restaure 2 points de vie !");
                                Console.ResetColor();
                            }
                            aiValueModifier += 2;
                            break;
                        case "T":
                            if (narative)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Le " + GetFullName(roleIA) + " active sa compétence ! Il sacrifie un point de vie pour augmenter sa force de 1 et attaque!");
                                Console.ResetColor();
                            }
                            aiValueModifier -= 1;
                            playerValueModifier -= DommageParRole(roleIA) + 1;
                            break;
                        case "V":
                            if (narative)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Le " + GetFullName(roleIA) + " active sa compétence ! Il mord le cou de l'adversaire et regagne un point de vie !");
                                Console.ResetColor();
                            }
                            playerValueModifier -= 1;

                            Random rdm = new Random();
                            var dice = rdm.Next(1, 101);

                            if (dice > 50)
                            {
                                //Rater
                                if (narative)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Le " + GetFullName(roleIA) + " n'a pas pu se soigner");
                                    Console.ResetColor();
                                }
                            }
                            else
                            {
                                //Gagner
                                if (narative)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Le " + GetFullName(roleIA) + " a réussi son coup ! Il se soigne 1 point de vie !");
                                    Console.ResetColor();
                                }
                                aiValueModifier += 1;
                            }
                            //playerValueModifier -= 1;
                            break;
                    }
                    break;
            }

            Console.ResetColor();

            if (playerDefending && playerValueModifier + 1 <= 0)
                playerValueModifier++;

            if (aiDefending && aiValueModifier + 1 <= 0)
                aiValueModifier++;

            if(playerSkillActivated && roleJoueur == "D")
            {
                var damageReceive = Math.Abs(playerValueModifier);
                aiValueModifier -= damageReceive;
            }

            if(aiSkillActivated && roleIA == "D")
            {
                var damageReceive = Math.Abs(aiValueModifier);
                playerValueModifier -= damageReceive;
            }
            return new Tuple<int, int>(playerValueModifier, aiValueModifier);
        }
        static int DommageParRole(string role)
        {
            var charactersDM = new Dictionary<string, int>() { { "H", 1 }, { "T", 1 }, { "D", 2 } , {"V", 2 } };
            return charactersDM[role];
        }

        #endregion

        #region Helper function

        static string GetFullName(string role)
        {

            switch(role)
            {
                case "T":
                    return "Tank";
                case "H":
                    return "Healer";
                case "D":
                    return "Damager";
                case "V":
                    return "Vampire";
                
            }
            return string.Empty;
        }

        static int GetHpByClass(string role)
        {
            switch(role)
            {
                case "D":
                    return 3;
                case "H":
                    return 4;
                case "T":
                    return 5;
                case "V":
                    return 3;
            }
            return 0;
        }

        static string FixValuePrint(float value, int fullPlace)
        {
            var nbCharacter = value.ToString().Length + 1;
            var freePlaces = fullPlace - nbCharacter;
            //var placePerSide = freePlaces / 2;

            string constructString = string.Empty;

            for (int i = 0; i <= freePlaces; i++)
            {
                if (freePlaces / 2 == i)
                {
                    constructString += value.ToString() +"%";
                    continue;
                }
                constructString += " ";
            }


            return constructString;
        }

        #endregion


    }

}