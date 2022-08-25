using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStoichiometry
{
    class Program
    {
        //static List<Ingredient> Made = new List<Ingredient>();
        //static List<Ingredient> Needed = new List<Ingredient>();
        static void Main(string[] args)
        {
            var fileInput = File.ReadAllLines("input.txt");
            var formulas = ParseFormulas(fileInput);

            //foreach (var f in formulas)
            //{
            //    Console.WriteLine(f.ToString());
            //}
            var leftovers = new List<Ingredient>();
            var oreToMkae = FindOreToMake(new Ingredient() { Name = "FUEL", Quantity = 1 }, formulas, leftovers);
            Console.WriteLine("1 FUEL = " + oreToMkae + " ORE");

            var fuelToBeMade = 1000000000000 / oreToMkae;
            var totalOre = 1000000000000;
            long fuelMade = 0;
            leftovers = new List<Ingredient>();
            while (totalOre > oreToMkae)
            {
                totalOre -= FindOreToMake(new Ingredient() { Name = "FUEL", Quantity = fuelToBeMade }, formulas, leftovers);
                fuelMade += fuelToBeMade;
                fuelToBeMade = totalOre / oreToMkae;
            }

            Console.WriteLine(fuelMade + " FUEL made with 1000000000000 ORE");
            Console.ReadLine();
        }

        static Formula[] ParseFormulas(string[] stringFormulas)
        {
            Formula[] formulas = new Formula[stringFormulas.Length];
            for (int n = 0; n < formulas.Length; n++)
            //foreach (string stringFormula in stringFormulas)
            {
                var stringFormula = stringFormulas[n];
                var formula = new Formula();
                var parts = stringFormula.Split("=>");
                formula.Result = ParseIngredient(parts[1]);
                var ingredientsList = parts[0].Split(",");
                var ingredients = new Ingredient[ingredientsList.Length];
                for (int i = 0; i < ingredientsList.Length; i++)
                {
                    ingredients[i] = ParseIngredient(ingredientsList[i]);
                }
                formula.Ingredients = ingredients;
                formulas[n] = formula;
            }
            return formulas;

        }

        static Ingredient ParseIngredient(string ingredientString)
        {
            var parts = ingredientString.Trim().Split(" ");
            var ingredient = new Ingredient();
            ingredient.Name = parts[1];
            ingredient.Quantity = long.Parse(parts[0]);
            return ingredient;
        }

        static long FindOreToMake(Ingredient toMake, Formula[] formulas, List<Ingredient> leftovers)
        {
            //var needed = Needed.FirstOrDefault(m => m.Name == toMake.Name);
            //if (needed == default(Ingredient)) Needed.Add(new Ingredient() { Name = toMake.Name, Quantity = toMake.Quantity });
            //else needed.Quantity += toMake.Quantity;

            CheckForLeftovers(toMake, leftovers);
            
            if (toMake.Quantity <= 0) return 0;

            var formula = formulas.First(f => f.Result.Name == toMake.Name);
            var amount = (long)Math.Ceiling(toMake.Quantity / (decimal)formula.Result.Quantity);

            CalculateLeftovers(toMake, formula, leftovers, amount);

            if (formula.Ingredients[0].Name == "ORE")
            {
                //var made = Made.FirstOrDefault(m => m.Name == formula.Ingredients[0].Name);
                //if (made == default(Ingredient)) Made.Add(new Ingredient() { Name = formula.Ingredients[0].Name, Quantity = amount * formula.Ingredients[0].Quantity });
                //else made.Quantity += amount * formula.Ingredients[0].Quantity;
                return amount * formula.Ingredients[0].Quantity;
            }


            long oreCount = 0;
            foreach (var ing in formula.Ingredients)
            {
                long newQty = ing.Quantity * amount;

                //var made = Made.FirstOrDefault(m => m.Name == ing.Name);
                //if (made == default(Ingredient)) Made.Add(new Ingredient() { Name = ing.Name, Quantity = newQty });
                //else made.Quantity += newQty;

                oreCount += (FindOreToMake(new Ingredient() { Name = ing.Name, Quantity = newQty}, formulas, leftovers));
            }
            return oreCount;
        }

        static void CheckForLeftovers(Ingredient toMake, List<Ingredient> leftovers)
        {
            var existingLeftovers = leftovers.FirstOrDefault(l => l.Name == toMake.Name);
            if (existingLeftovers != default(Ingredient))
            {
                var orig = toMake.Quantity;
                toMake.Quantity = toMake.Quantity - existingLeftovers.Quantity;

                if (toMake.Quantity <= 0)
                {
                    
                    existingLeftovers.Quantity = Math.Abs(toMake.Quantity);
                    toMake.Quantity = 0;
                }
                else
                {
                    existingLeftovers.Quantity = 0;
                }

            }
        }

        static void CalculateLeftovers(Ingredient toMake, Formula formula, List<Ingredient> leftovers, long amount)
        {
            var amountLeftover = (formula.Result.Quantity * amount) - toMake.Quantity;

            var leftover = leftovers.FirstOrDefault(l => l.Name == toMake.Name);
            if (leftover == default(Ingredient))
            {
               leftover = new Ingredient()
               { Name = toMake.Name, Quantity = amountLeftover };
                leftovers.Add(leftover);
            }
            else leftover.Quantity += amountLeftover;
            
        }

        static long gcf(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        static long lcm(long a, long b)
        {
            return (a / gcf(a, b)) * b;
        }

        static long lcm(long[] numbers)
        {
            var num = numbers[0];
            for(int i = 1; i < numbers.Length; i++)
            {
                num = lcm(num, numbers[i]);
            }
            return num;
        }
    }
}