using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdvanceListViewTestApp
{
    public partial class Form1 : Form
    {
        IEnumerable<Animal> animals;

        public Form1()
        {
            InitializeComponent();

            animals = new Animal[] {
                new Animal() { Id = 1, Name = "Dog", Type = "Mammal" },
                new Animal() { Id = 2, Name = "Cat", Type = "Mammal" },
                new Animal() { Id = 3, Name = "Bird", Type = "Bird" },
                new Animal() { Id = 4, Name = "Lizard", Type = "Reptile" },
                new Animal() { Id = 5, Name = "Gold Fish", Type = "Fish" }
            };

            advanceListView1.DictionaryItems = animals.ToDictionary(a => a.Id.ToString(), a => (object)a);
            //advanceListView1.PropertiesForFiltering = new string[] { "Name", "Type", "DiscoveredDate" };
            advanceListView1.PropertiesToHide = new string[] { "Id" };
            advanceListView1.KeyProperty = "Id";
            advanceListView1.ContextMenuStrip = contextMenuStrip1;
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

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(((Animal)advanceListView1.SelectedItem).Name);
        }

        private void wizardPage1_Commit(object sender, AeroWizard.WizardPageConfirmEventArgs e)
        {
            var list = advanceListView1.DictionaryItems.Values.Select(a => a).ToList();

            foreach (Animal item in list)
            {
                MessageBox.Show(item.Name);
            }
        }
    }
}
