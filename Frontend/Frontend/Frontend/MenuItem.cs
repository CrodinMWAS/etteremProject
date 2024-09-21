using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend
{
    public class MenuItem
    {
        string pic;
        string name;
        string[] allergens;
        string price;

        public MenuItem(string pic, string name, string[] allergens, string price)
        {
            this.pic = pic;
            this.name = name;
            this.allergens = allergens;
            this.price = price;
        }

        public string Pic { get => pic; set => pic = value; }
        public string Name { get => name; set => name = value; }
        public string[] Allergens { get => allergens; set => allergens = value; }
        public string Price { get => price; set => price = value; }
    }
}
