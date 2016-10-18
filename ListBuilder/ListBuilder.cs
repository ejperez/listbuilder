using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ListBuilder
{
    public partial class ListBuilder : UserControl
    {
        private DataTable dataTable;

        public IEnumerable<string> PropertiesToHide
        {
            set
            {
                // Hide columns
                if (value.Count() > 0)
                {
                    foreach (string property in value)
                    {
                        dgvItems.Columns[property].Visible = false;
                    }
                }
            }
        }

        public string SearchText
        {
            set
            {
                lblSearch.Text = value;
            }
        }

        private string filterBy;

        public string FilterBy
        {
            set { filterBy = value; }
        }

        private string keyProperty;

        public string KeyProperty
        {
            set { keyProperty = value; }
        }

        public IEnumerable<string> SelectedKeys
        {
            set
            {
                if (dictionaryItems.Count() == 0 || value == null || value.Count() == 0)
                    return;

                foreach (DataRow row in dataTable.Rows)
                {
                    if (value.Contains((string)row[keyProperty]))
                    {
                        row["Selected"] = true;
                    }
                }
            }
        }

        private Dictionary<string, object> dictionaryItems;

        public Dictionary<string, object> DictionaryItems
        {
            get { return dictionaryItems; }
            set
            {
                if (value == null || value.Count == 0)
                    dgvItems.Rows.Clear();

                dictionaryItems = value;

                // Create new
                dataTable = new DataTable();

                // Add checbox column
                dataTable.Columns.Add("Selected", typeof(bool));

                // Add columns
                foreach (var item in dictionaryItems.First().Value.GetType().GetProperties())
                {
                    dataTable.Columns.Add(item.Name, typeof(string));
                }

                // Add rows
                foreach (var item in dictionaryItems)
                {
                    var list = item.Value.GetType().GetProperties().Select(p => p.GetValue(item.Value)).ToList();
                    list.Insert(0, false);
                    dataTable.Rows.Add(list.ToArray());
                }

                dgvItems.DataSource = dataTable;

                // Make the first column editable, make the rest read only
                dgvItems.Columns[0].ReadOnly = false;
                for (int i = 1; i < dgvItems.Columns.Count; i++)
                {
                    dgvItems.Columns[i].ReadOnly = true;
                }
            }
        }

        public IEnumerable<object> SelectedItems
        {
            get
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    if ((bool)row["Selected"])
                    {
                        yield return dictionaryItems[(string)row[keyProperty]];
                    }
                }
            }
        }

        public ListBuilder()
        {
            InitializeComponent();     
        }

        private void txtItemsFilter_TextChanged(object sender, EventArgs e)
        {
            if (filterBy != null)
            {
                dataTable.DefaultView.RowFilter = string.Format("[{0}] LIKE '%{1}%'", filterBy, txtItemsFilter.Text);
            }
        }

        // Show selected
        private void button3_Click(object sender, EventArgs e)
        {
            dataTable.DefaultView.RowFilter = string.Format("Selected = True");
        }

        // Show all
        private void button4_Click(object sender, EventArgs e)
        {
            dataTable.DefaultView.RowFilter = string.Empty;
            txtItemsFilter.Clear();
        }

        // Select all visible in list
        private void button1_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                if (((string)row[filterBy]).ToUpper().Contains(txtItemsFilter.Text.ToUpper()))
                    row["Selected"] = true;
            }

            UpdateSelectedCountLabel();
        }

        // Deselect all visible in list
        private void button2_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                if (((string)row[filterBy]).ToUpper().Contains(txtItemsFilter.Text.ToUpper()))
                    row["Selected"] = false;
            }

            UpdateSelectedCountLabel();
        }

        private void UpdateSelectedCountLabel()
        {
            int count = 0;

            foreach (DataRow row in dataTable.Rows)
            {
                if ((bool)row["Selected"])
                    count++;
            }

            if (count == 0)
            {
                lblSelectedCount.Text = "Nothing selected";
            }
            else
            {
                lblSelectedCount.Text = string.Format("{0} item(s) selected", count);
            }
        }

        private void dgvItems_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateSelectedCountLabel();
        }
    }
}
