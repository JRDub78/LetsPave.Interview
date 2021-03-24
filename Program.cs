using System;
using System.Text.RegularExpressions;

namespace LetsPave.Vending
{
    class Program
    {
        static void Main(string[] args)
        {
            string userInput;
            string vendingMessage= "";
            decimal moneyInput;

            VendingMachine LunchRoomVM = new VendingMachine();                        
            
            //Initialize and Stock the Vending Machine
            //Data found in the App.config file
            LunchRoomVM.Stock();

            //Display Initial Instructions
            Console.WriteLine(LunchRoomVM.DisplayInstructions());

            do
            {
                Console.WriteLine(LunchRoomVM.CustomerChoices());
                Console.WriteLine(LunchRoomVM.AvailableFunds());
                Console.Write(LunchRoomVM.PromptUser());
               
                userInput = Console.ReadLine();
                Console.Clear();

                //-------------------------------------------------------------------------
                //Handle Input from the Customer
                //-------------------------------------------------------------------------

                //If the input is money
                if (decimal.TryParse(userInput, out moneyInput))
                {
                    LunchRoomVM.AddFunds(moneyInput);
                }
                else
                {
                    //Determine if the input are features of the vending machine or selection
                    switch (userInput.ToLower())
                    {
                        case "reset":
                            LunchRoomVM.Reset();
                            break;
                        case "help":
                            Console.WriteLine(LunchRoomVM.DisplayInstructions());
                            break;
                        case "refund":
                            Console.WriteLine(LunchRoomVM.DispenseChange());
                            break;
                        case "restock":
                            LunchRoomVM.Stock();
                            break;
                        default:
                            {
                                if (LunchRoomVM.isValidSelection(userInput))
                                {
                                    if (LunchRoomVM.Vend(userInput, out vendingMessage))
                                    {
                                        Console.WriteLine(vendingMessage);
                                        Console.WriteLine(LunchRoomVM.DispenseChange());
                                    }
                                    else
                                    {
                                        Console.WriteLine(vendingMessage);
                                    }
                                }
                                else
                                {
                                    if (userInput.ToLower() != "quit")
                                    {
                                        Console.WriteLine("Invalid Selection\r\n");
                                    }
                                }
                            }
                            break;
                    }
                }
                
            }
            while (userInput.ToLower() != "quit" );
        }
    }
}
