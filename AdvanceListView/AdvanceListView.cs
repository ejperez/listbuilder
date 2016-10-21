using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdvanceListView
{
    public partial class AdvanceListView : UserControl
    {
        private DataTable dataTable;

        public AdvanceListView()
        {
            InitializeComponent();

            // Default key is Id
            keyProperty = "Id";
        }

        private string keyProperty;

        public string KeyProperty
        {
            set { keyProperty = value; }
        }

        public ContextMenuStrip ListViewContextMenuStrip
        {
            set
            {
                dgvItems.ContextMenuStrip = value;
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

        public object SelectedItem
        {
            get
            {
                if (dgvItems.Rows.Count == 0)
                    return null;

                return dictionaryItems[((string)dgvItems.CurrentRow.Cells[keyProperty].Value)];
            }
        }

        private Dictionary<string, object> dictionaryItems;

        public Dictionary<string, object> DictionaryItems
        {
            get
            {
                return dictionaryItems;
            }
            set
            {
                if (value == null || value.Count() == 0)
                {
                    if (dataTable != null)
                        dataTable.Clear();

                    dgvItems.DataSource = null;

                    lblCount.Text = "No rows";

                    return;
                }

                dictionaryItems = value;

                // Create new
                dataTable = new DataTable();

                // Add columns
                List<string> columns = new List<string>();
                foreach (var item in dictionaryItems.Values.First().GetType().GetProperties())
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
                    dataTable.Rows.Add(list.ToArray());
                }

                dgvItems.DataSource = dataTable;

                // Make the first column editable, make the rest read only
                dgvItems.Columns[0].ReadOnly = false;
                for (int i = 1; i < dgvItems.Columns.Count; i++)
                {
                    dgvItems.Columns[i].ReadOnly = true;
                }

                lblCount.Text = string.Format("{0} rows(s)", dictionaryItems.Count);
            }
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            if (dataTable.Rows.Count > 0 && propertiesForFiltering.Count() > 0)
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

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Clear filter
            dataTable.DefaultView.RowFilter = string.Empty;
            txtFilter.Clear();
        }

        private void dgvItems_SelectionChanged(object sender, EventArgs e)
        {
            if (ListViewSelectionChanged != null)
                ListViewSelectionChanged(null, null);
        }

        public event EventHandler ListViewSelectionChanged;
    }
}
