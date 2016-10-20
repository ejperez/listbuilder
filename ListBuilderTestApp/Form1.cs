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
        private Dictionary<string, IEnumerable<Animal>> dictionary;
        private Dictionary<string, IEnumerable<Animal>> dictionarySelected;

        public Form1()
        {
            InitializeComponent();

            dictionary = new Dictionary<string, IEnumerable<Animal>>();
            dictionarySelected = new Dictionary<string, IEnumerable<Animal>>();

            dictionary.Add("1", new Animal[] { new Animal() { Id = 1, Name = "Dog", Type = "Mammal" }, new Animal() { Id = 2, Name = "Cat", Type = "Mammal" } });
            dictionary.Add("2", new Animal[] { new Animal() { Id = 1, Name = "Dog", Type = "Mammal" }, new Animal() { Id = 2, Name = "Cat", Type = "Mammal" }, new Animal() { Id = 3, Name = "Bat", Type = "Mammal" } });

            listBuilder1.DictionaryItems = dictionary["1"].ToDictionary(a => a.Id.ToString(), a => (object)a);
            listBuilder1.SelectionChanged += listBuilder1_SelectionChanged;
            listBuilder1.FilterBy = "Name";
            listBuilder1.KeyProperty = "Id";
            listBuilder1.SearchText = "Search by name";
            listBuilder1.PropertiesToHide = new string[] { "Id", "DiscoveredDate" };
            listBuilder1.SelectedKeys = new string[] { "1", "3" };
        }

        void listBuilder1_SelectionChanged(object sender, EventArgs e)
        {
            var result = listBuilder1.SelectedItems;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var result = listBuilder1.SelectedItems;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            dictionarySelected["2"] = listBuilder1.SelectedItems.Select(a => (Animal)a).ToList();

            listBuilder1.DictionaryItems = dictionary["1"].ToDictionary(a => a.Name, a => (object)a);
        }

        private void button2_Click(object sender, EventArgs e)
        {            
            dictionarySelected["1"] = listBuilder1.SelectedItems.Select(a => (Animal)a).ToList();

            listBuilder1.DictionaryItems = dictionary["2"].ToDictionary(a => a.Name, a => (object)a);
        }

        private void button3_Click(object sender, EventArgs e)
        {

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
