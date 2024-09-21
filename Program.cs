using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace MyApp
{
    internal class Program
    {
        static int[] cards = FillArray(false);
        static int[] player1Cards = FillArray(true);
        static int[] player2Cards = FillArray(true);
        static (int, int) remainingCards = (26, 26);
        static int roundCounter = 1;
        static int player1;
        static int player2;
        static bool player1Won = false;
        static bool player2Won = false;
        static string basePath = AppDomain.CurrentDomain.BaseDirectory;
        static string logPath = basePath + "log.txt";
        static void Main(string[] args)
        {
            bool someoneWon = false;

            if (!File.Exists(logPath)) {
                using (File.CreateText(logPath)) {}

                using (StreamWriter logs = File.AppendText(logPath)) {
                    logs.WriteLine("\nLogs for the War game.                             by Danko     \n");
                    logs.WriteLine("            BATTLE          RESULT       P1 LEFT  P2 LEFT      ");
                    logs.WriteLine("           ________________________________________________     ");
                }
            }

            do {
                Console.WriteLine("\nPlease, type 'Surrender' to end the game, otherwise click 'return' to continue to the next round:");

                if (RequestInput()) {
                    player1Won = true;
                    player2Won = true;
                    break;
                }

                if (!player1Won && !player2Won) {
                    Console.Clear();
                    Console.WriteLine($"\n\n               --- Round {roundCounter} ---\n");

                    player1 = AttemptDraw(player1Cards);
                    player2 = AttemptDraw(player2Cards);

                    Console.WriteLine($"Player 1 got a card valued as {player1 + 2}, while player 2 got {player2 + 2}");
                    
                    CompareCards();
                    roundCounter++;
                }

                someoneWon = DidSomeoneWin();

            } while (!someoneWon);

            if (player1Won && player2Won) {
                Console.WriteLine("The match remains undisputed...");

            } else if (player1Won) {
                Console.WriteLine("Congratulations! Player nr1 has won.");

            } else if (player2Won) {
                Console.WriteLine("Congratulations! Player nr2 has won.");

            }
        }


        static int[] FillArray(bool withoutCards) {
            int assignedValue;

            if (withoutCards) {
                assignedValue = 0;
            } else {
                assignedValue = 4;
            }

            int[] Result = new int [14];

            for (int i = 0; i < 13; i++) {
                Result[i] = (assignedValue);
            }

            return Result;
        }

        static void LogIt(string targetPath, string content) {
            using (StreamWriter logs = File.AppendText(targetPath)) {
                logs.WriteLine(content);
            }
        }

        static bool RequestInput() {

            string? input = Console.ReadLine();

            if (input == "surrender") {
                return true;
            } else {
                return false;
            }
        }

        static int AttemptDraw(int[] playerCards) {
            bool drawFailed = DidSomeoneWin();

            if (!drawFailed) {
                return GetValidRandom(playerCards);
            } else {
                return 0;
            }
        }

        static int GetValidRandom(int[] playerCards) {
            int card;

            do {
                Random getPlayerRandom = new Random();
                card = getPlayerRandom.Next(0, 13);

                if (cards[card] >= 1) {
                    cards[card] --;
                    return card;

                } else if (playerCards[card] >= 1) {
                    playerCards[card] --;
                    return card;
                }
            } while (true);
        }

        static void CompareCards() {
            string content = "";

            if (player1 > player2) {
                Console.WriteLine("Which means that player 1 demolishes player 2");


                player1Cards[player1]++;
                player1Cards[player2]++;
                player2Cards[player1]--;
                player2Cards[player2]--;

                remainingCards.Item1++;
                remainingCards.Item2--;

                content = $"ENTRY {ConformToSize(roundCounter, 4, ' ', false)} |   {ConformToSize(player1, 2, ' ', true)} vs {ConformToSize(player2, 2, ' ', false)}    |  P1 Winner |   {ConformToSize(remainingCards.Item1, 3, ' ', false)}  |   {ConformToSize(remainingCards.Item2, 3, ' ', false)}  |";
                LogIt(logPath,  content);

            } else if (player1 < player2) {
                Console.WriteLine("Which means that player 2 destroys player 1");
                

                player1Cards[player1]--;
                player1Cards[player2]--;
                player2Cards[player1]++;
                player2Cards[player2]++;

                remainingCards.Item1--;
                remainingCards.Item2++;

                content = $"ENTRY {ConformToSize(roundCounter, 4, ' ', false)} |   {ConformToSize(player1, 2, ' ', true)} vs {ConformToSize(player2, 2, ' ', false)}    |  P2 Winner |   {ConformToSize(remainingCards.Item1, 3, ' ', false)}  |   {ConformToSize(remainingCards.Item2, 3, ' ', false)}  |";
                LogIt(logPath,  content);

            } else if (player1 == player2) {
                Console.WriteLine("A draw means that a battle is imminent...\n\n3 additional cards are drawn from each player and are at stake\nThe winner takes everything. Good luck!");
                RequestInput();

                content = $"ENTRY {ConformToSize(roundCounter, 4, ' ', false)} |   {ConformToSize(player1, 2, ' ', true)} vs {ConformToSize(player2, 2, ' ', false)}    |    Draw    |   {ConformToSize(remainingCards.Item1, 3, ' ', false)}  |   {ConformToSize(remainingCards.Item2, 3, ' ', false)}  |";

                byte n0CardsAtStake = 2;
                List<int> stakes = new List<int>();
                HandleDraw(ref stakes, ref n0CardsAtStake);
            }
        }

        static void HandleDraw(ref List<int> stakes, ref byte n0CardsAtStake) {

            string content = "";

            stakes.AddRange(new int[] {player1, player2, AttemptDraw(player1Cards), AttemptDraw(player1Cards), AttemptDraw(player1Cards), AttemptDraw(player2Cards), AttemptDraw(player2Cards), AttemptDraw(player2Cards)});
            n0CardsAtStake += 8;

            player1 = AttemptDraw(player1Cards);
            player2 = AttemptDraw(player2Cards);

            Console.WriteLine($"Player 1 will field {player1 + 2}, while player 2 entrusts {player2 + 2}");

            if (player1 > player2) {
                Console.WriteLine("Which means that player 1 takes it home!");

                content = $"ENTRY {ConformToSize(roundCounter, 4, ' ', false)} |   {ConformToSize(player1, 2, ' ', true)} vs {ConformToSize(player2, 2, ' ', false)}    |  P1 Winner |   {ConformToSize(remainingCards.Item1, 3, ' ', false)}  |   {ConformToSize(remainingCards.Item2, 3, ' ', false)}  |";
                LogIt(logPath,  content);

                remainingCards.Item1 =+ n0CardsAtStake;
                remainingCards.Item2 =- n0CardsAtStake;

                for (int i = 0; i < stakes.Count; i++) {
                    player1Cards[stakes[i]]++;
                }

            } else if (player1 < player2) {
                Console.WriteLine("Which means that player 2 comes out victorious!");

                content = $"ENTRY {ConformToSize(roundCounter, 4, ' ', false)} |   {ConformToSize(player1, 2, ' ', true)} vs {ConformToSize(player2, 2, ' ', false)}    |  P2 Winner |   {ConformToSize(remainingCards.Item1, 3, ' ', false)}  |   {ConformToSize(remainingCards.Item2, 3, ' ', false)}  |";
                LogIt(logPath,  content);

                remainingCards.Item1 =- n0CardsAtStake;
                remainingCards.Item2 =+ n0CardsAtStake;

                for (int i = 0; i < stakes.Count; i++) {
                    player2Cards[stakes[i]]++;
                }
            } else if (player1 == player2) {
                Console.WriteLine("War of atrition it is! Battle again");

                content = $"ENTRY {ConformToSize(roundCounter, 4, ' ', false)} |   {ConformToSize(player1, 2, ' ', true)} vs {ConformToSize(player2, 2, ' ', false)}    |    Draw    |   {ConformToSize(remainingCards.Item1, 3, ' ', false)}  |   {ConformToSize(remainingCards.Item2, 3, ' ', false)}  |";
                LogIt(logPath,  content);

                HandleDraw(ref stakes, ref n0CardsAtStake);
            }
        }

        static bool DidSomeoneWin() {
            bool noCardsInMainDeck = CheckDeck(cards);

            if (remainingCards.Item1 < 0 && noCardsInMainDeck) {
                player2Won = true;
                return true;
            } else if (remainingCards.Item2 < 0 && noCardsInMainDeck) {
                player1Won = true;
                return true;
            }

                return false;
        }

        static bool CheckDeck(int[] deck) {
            byte counter = 0;

            for (int i = 0; i < deck.Length; i++) {
                if (deck[i] <= 0) counter++;
            }

            if (counter >= 13) {
                return true;
            } else {
                return false;
            }
        }

        static string ConformToSize(int intToFormat, int paddingLevel, char filler, bool reversed) {
            string stringToFormat = intToFormat.ToString();

            if(reversed) {
                for (int i = paddingLevel - stringToFormat.Length; i >= 1; i--) {
                    stringToFormat = filler + stringToFormat;
                }

            } else if (!reversed) {
                for (int i = paddingLevel - stringToFormat.Length; i >= 1; i--) {
                    stringToFormat = stringToFormat + filler;
                }
            }
            
            return stringToFormat;
        }
    }
}