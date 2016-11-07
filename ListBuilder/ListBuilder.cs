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
                if (dgvItems.Columns.Count > 0 && value.Count() > 0)
                {
                    foreach (string property in value)
                    {
                        dgvItems.Columns[property].Visible = false;
                    }
                }
            }
        }

        private IEnumerable<string> propertiesForFiltering;

        public IEnumerable<string> PropertiesForFiltering
        {
            set
            {
                propertiesForFiltering = value;

                if (dataTable != null)
                    dataTable.DefaultView.RowFilter = string.Empty;
            }
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
                if (dictionaryItems == null || dictionaryItems.Count() == 0 || value == null || value.Count() == 0)
                    return;

                foreach (DataRow row in dataTable.Rows)
                {
                    if (value.Contains((string)row[keyProperty]))
                    {
                        row["Selected"] = true;
                    }
                }

                UpdateSelectedCountLabel();
            }
        }

        private Dictionary<string, object> dictionaryItems;

        public Dictionary<string, object> DictionaryItems
        {
            get { return dictionaryItems; }
            set
            {
                button4_Click(null, null);

                if (value == null || value.Count == 0)
                {
                    if (dataTable != null)
                        dataTable.Clear();

                    dgvItems.DataSource = null;
                    UpdateSelectedCountLabel();

                    return;
                }

                dictionaryItems = value;

                // Create new
                dataTable = new DataTable();

                // Add checbox column
                dataTable.Columns.Add("Selected", typeof(bool));

                // Add columns
                List<string> columns = new List<string>();
                foreach (var item in dictionaryItems.First().Value.GetType().GetProperties())
                {
                    dataTable.Columns.Add(item.Name, typeof(string));
                    columns.Add(item.Name);
                }

                // Default properties for filtering are all columns
                PropertiesForFiltering = columns;

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

                UpdateSelectedCountLabel();
            }
        }

        public IEnumerable<object> SelectedItems
        {
            get
            {
                if (dataTable == null)
                    return null;

                var list = new List<object>();
                foreach (DataRow row in dataTable.Rows)
                {
                    if ((bool)row["Selected"])
                    {
                        list.Add(dictionaryItems[(string)row[keyProperty]]);
                    }
                }

                return list;
            }
        }

        public ListBuilder()
        {
            InitializeComponent();
        }

        private void txtItemsFilter_TextChanged(object sender, EventArgs e)
        {
            if (dataTable.Rows.Count > 0 && propertiesForFiltering.Count() > 0 && !string.IsNullOrWhiteSpace(txtFilter.Text))
            {
                var properties = propertiesForFiltering.ToArray();
                string filter = string.Empty;

                for (int i = 0; i < properties.Length; i++)
                {
                    if (i == 0)
                    {
                        filter += string.Format("[{0}] LIKE '%{1}%'", properties[i], txtFilter.Text);
                    }
                    else
                    {
                        filter += string.Format(" OR [{0}] LIKE '%{1}%'", properties[i], txtFilter.Text);
                    }

                }

                dataTable.DefaultView.RowFilter = filter;
            }
        }

        // Show selected
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataTable != null)
                dataTable.DefaultView.RowFilter = string.Format("Selected = True");                
            txtFilter.Clear();
        }

        // Show all
        private void button4_Click(object sender, EventArgs e)
        {
            if (dataTable != null)
                dataTable.DefaultView.RowFilter = string.Empty;
            txtFilter.Clear();
        }

        // Select all visible in list
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataTable != null)
            {
                // Get filtered ids
                List<string> selectedIds = new List<string>();
                var dt = dataTable.DefaultView.ToTable();

                foreach (DataRow row in dt.Rows)
                {
                    selectedIds.Add((string)row[keyProperty]);
                }

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    int keyPropertyIndex = dataTable.Columns[keyProperty].Ordinal;
                    int selectedIndex = dataTable.Columns["Selected"].Ordinal;

                    if (selectedIds.Contains((string)dataTable.Rows[i].ItemArray[keyPropertyIndex]))
                    {
                        dataTable.Rows[i][selectedIndex] = true;
                    }
                }

                dataTable.AcceptChanges();
            }

            UpdateSelectedCountLabel();

            if (SelectionChanged != null)
                SelectionChanged(null, null);
        }

        // Deselect all visible in list
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataTable != null)
            {
                // Get filtered ids
                List<string> selectedIds = new List<string>();
                var dt = dataTable.DefaultView.ToTable();

                foreach (DataRow row in dt.Rows)
                {
                    selectedIds.Add((string)row[keyProperty]);
                }

                // Select filtered ids
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    int keyPropertyIndex = dataTable.Columns[keyProperty].Ordinal;
                    int selectedIndex = dataTable.Columns["Selected"].Ordinal;

                    if (selectedIds.Contains((string)dataTable.Rows[i].ItemArray[keyPropertyIndex]))
                    {
                        dataTable.Rows[i][selectedIndex] = false;
                    }
                }

                dataTable.AcceptChanges();
            }

            UpdateSelectedCountLabel();

            if (SelectionChanged != null)
                SelectionChanged(null, null);
        }

        private void UpdateSelectedCountLabel()
        {
            int count = 0;

            if (dataTable != null)
            {
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

                lblSelectedCount.Text += string.Format(" out of {0}", dataTable.Rows.Count);
            }
        }

        private void dgvItems_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateSelectedCountLabel();
            if (SelectionChanged != null)
                SelectionChanged(null, null);
        }

        public event EventHandler SelectionChanged;
    }
}
