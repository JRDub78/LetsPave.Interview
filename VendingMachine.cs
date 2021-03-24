using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LetsPave.Vending
{
    public class VendingItem
    {
        public string Selection { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

    }
    public class VendingMachine
    {
        const decimal QUARTER = 0.25M;
        const decimal DIME = 0.10M;
        const decimal NICKEL = 0.05M;
        const decimal PENNY = 0.01M;


        Dictionary<string,VendingItem> currentStock;

        private decimal funds;

        public decimal Funds
        {
            get { return funds; }
        }


        public VendingMachine()
      {
            currentStock = new Dictionary<string, VendingItem>();
      }
      public void Reset()
      {
            currentStock = new Dictionary<string, VendingItem>();
            funds = 0;
            Stock();
      }           

      public void Stock()
      {
            string inventory = ConfigurationManager.AppSettings.Get("Inventory");

            foreach (string item in inventory.Split(";"))
            {
                VendingItem vi = new VendingItem();
                string[] itemArrayData = item.Split(",");

                vi.Selection = itemArrayData[0];
                vi.Name = itemArrayData[1];
                vi.Quantity = int.Parse(itemArrayData[2]);
                vi.Price = decimal.Parse(itemArrayData[3]);

                if (currentStock.Keys.Contains(vi.Selection))
                {
                    currentStock[vi.Selection].Quantity = currentStock[vi.Selection].Quantity + vi.Quantity;
                }
                else
                {
                    currentStock.Add(vi.Selection, vi);
                }
            }
      }

      public void AddFunds(decimal money)
        {
            funds = funds + money;
        }

      public string CustomerChoices()
      {
            StringBuilder choices = new StringBuilder();

            choices.AppendLine("-----------------------------------------------------------");

            foreach (var item in currentStock)
            {
                string choiceDisplayFormat = "{0} - {1}\t${2}\t{3} Remaining";

                choices.AppendLine(string.Format(choiceDisplayFormat, item.Value.Selection, item.Value.Name, item.Value.Price, item.Value.Quantity));
            }

            choices.AppendLine("-----------------------------------------------------------");

            return choices.ToString();
      }

      public string DisplayInstructions()
      {
            string instructions = ConfigurationManager.AppSettings.Get("VendingMachineInstructions");

            StringBuilder availableSelections = new StringBuilder();

            foreach (var item in currentStock)
            {
                if (item.Value.Quantity > 0)
                {
                    availableSelections.Append(item.Key + " ");
                }
            }
                        
            return string.Format(instructions, availableSelections.ToString());
        }

      public string AvailableFunds()
      {
         return string.Format("Available Funds: ${0}", funds);
      }
      public string DispenseChange()
      {
            string changeDisplayFormat = "Change\r\n{0} Quarters\r\n{1} Dimes\r\n{2} Nickels\r\n{3} Pennies\r\n";

            int NumberOfQuarters = 0;
            int NumberOfDimes = 0;
            int NumberOfNickels = 0;
            int NumberOfPennies = 0;

            NumberOfQuarters = getNumberOfCoins(funds, QUARTER);
            NumberOfDimes = getNumberOfCoins(funds, DIME);
            NumberOfNickels = getNumberOfCoins(funds, NICKEL);
            NumberOfPennies = getNumberOfCoins(funds, PENNY);

            return string.Format(changeDisplayFormat, NumberOfQuarters, NumberOfDimes, NumberOfNickels, NumberOfPennies);
        }

        public string PromptUser()
        {
            string userPrompt = "Make a selection or type in a vending machine feature:";

            if (funds == 0)
            {
                userPrompt = "Enter money or type in a vending machine feature:";
            }

            return userPrompt;
        }

      public bool isValidSelection(string userInput)
      {
            bool result = false;
            Regex rg = new Regex(@"^[a-zA-Z0-9\s,]*$");

            //Valid Alpha Numeric
            if (rg.IsMatch(userInput) == true )
            {
                foreach(string key in currentStock.Keys)
                {
                    if (key.ToLower() == userInput.ToLower())
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

      public bool Vend (string userInput, out string vendingMessage)
      {
            bool success = false;
            vendingMessage = "";
            VendingItem vi = new VendingItem();

            vi = currentStock[userInput.ToUpper()];

            if (vi.Quantity == 0)
            {
                vendingMessage = string.Format("There are no more {0} left.\r\n", vi.Name);
            }
            else
            {
                if (funds >= vi.Price && vi.Quantity > 0)
                {
                    vendingMessage = string.Format("Successfully Vending {0}\r\n", vi.Name);
                    funds = funds - vi.Price;
                    decrementInventory(userInput.ToUpper());
                    success = true;
                }
                else
                {
                    vendingMessage = string.Format("Insufficent funds. Either add more money or select another item.\r\n");
                }
            }

            return success;
        }

        private int getNumberOfCoins(decimal money, decimal coinValue)
        {
            int numberOfCoins = 0;

            numberOfCoins = (int)(money / coinValue);
            funds = funds - numberOfCoins * coinValue;

            return numberOfCoins;
        }
        private void decrementInventory(string selection)
        {
            currentStock[selection].Quantity = currentStock[selection].Quantity - 1;
        }

    }
}
