using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend
{
    public class MenuItem
    {
        string category;
        string pic;
        string name;
        string allergen1;
        string allergen2;
        string allergen3;
        string price;
        string desc;

        // Constructor with default values for allergens
        public MenuItem(string category, string pic, string name, string price, string desc, string allergen1 = null, string allergen2 = null, string allergen3 = null)
        {
            this.category = category;
            this.pic = pic;
            this.name = name;
            this.price = price;
            this.allergen1 = allergen1;
            this.allergen2 = allergen2;
            this.allergen3 = allergen3;
            this.desc = desc;
        }

        public string Category { get => category; set => category = value; }
        public string Pic { get => pic; set => pic = value; }
        public string Name { get => name; set => name = value; }
        public string Allergen1 { get => allergen1; set => allergen1 = value; }
        public string Allergen2 { get => allergen2; set => allergen2 = value; }
        public string Allergen3 { get => allergen3; set => allergen3 = value; }
        public string Price { get => price; set => price = value; }
        public string Desc { get => desc; set => desc = value; }
    }
}
