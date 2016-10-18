using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ListBuilderTestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var dictionary = new Dictionary<string, object>();
            dictionary.Add("1", new Animal() { Id = 1, Name = "Dog", Type = "Mammal" });
            dictionary.Add("2", new Animal() { Id = 2, Name = "Cat", Type = "Mammal" });
            dictionary.Add("3", new Animal() { Id = 3, Name = "Eagle", Type = "Bird" });

            listBuilder1.DictionaryItems = dictionary;
            listBuilder1.DictionaryItems = null;
            listBuilder1.DictionaryItems = dictionary;
            listBuilder1.FilterBy = "Name";
            listBuilder1.KeyProperty = "Id";
            listBuilder1.SearchText = "Search by name";
            listBuilder1.PropertiesToHide = new string[] { "Id", "DiscoveredDate" };
            listBuilder1.SelectedKeys = new string[] { "1", "3" };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var result = listBuilder1.SelectedItems;
        }
    }

    class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime DiscoveredDate { get; set; }

        public Animal()
        {
            DiscoveredDate = DateTime.Now;
        }
    }
}
