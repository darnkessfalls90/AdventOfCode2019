using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStoichiometry
{
    public class Formula
    {
        public Ingredient[] Ingredients { get; set; }
        public Ingredient Result { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach(var ing in Ingredients)
            {
                builder.Append(ing.Quantity + " " + ing.Name + ", ");
            }

            builder.Remove(builder.Length - 2, 2);

            builder.Append(" => " + Result.Quantity + " " + Result.Name);

            return builder.ToString();
        }
    }

    public class Ingredient
    {
        public string Name { get; set; }
        public long Quantity { get; set; }
    }
}
