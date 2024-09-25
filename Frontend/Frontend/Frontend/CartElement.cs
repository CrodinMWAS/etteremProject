using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend
{
    public class CartElement : INotifyPropertyChanged
    {
        //This watches the variable's value, if changed updates "DOM"
        public event PropertyChangedEventHandler PropertyChanged;

        private int _amount;
        public int Amount
        {
            get { return _amount; }
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    OnPropertyChanged("Amount");
                }
            }
        }

        int id;
        MenuItem item;
        int amount;

        public CartElement(int id, MenuItem item, int amount)
        {
            this.id = id;
            this.item = item;
            Amount = amount;
        }

        public int Id { get => id; set => id = value; }
        public MenuItem Item { get => item; set => item = value; }

        //This watches the variable's value, if changed updates "DOM"
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
