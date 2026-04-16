using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RogerTech.Tool;
using System.IO;

namespace TestComm
{
    public partial class Form1 : Form
    {
        Group releaseCheckSfc;
        Group releaseSfc;
        Server plc;
        Form2 frm2 = new Form2();
        List<Group> groups;

        public Form1()
        {
            InitializeComponent();
            plc = Server.GetInstance();

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Config");
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] fi = di.GetFiles();
            foreach (var item in fi)
            {
                Group gorup = new Group(item.Name, plc);
            }
            foreach (var item in plc.Groups)
            {
                TreeNode node = new TreeNode(item.GroupName);
                node.Tag = item;
                treeView1.Nodes.Add(node);
            }
            treeView1.SelectedNode = treeView1.Nodes[0];
        }
        Group tagGroup;
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(treeView1.SelectedNode != null)
            {
                tagGroup = (Group)treeView1.SelectedNode.Tag;
                p = 0;
                rowIndex = 0;
                UpdataDataGrid();
            }
        }
        private void UpdataDataGrid()
        {
            if (dataGridView1.SelectedRows != null && dataGridView1.SelectedRows.Count > 0)
            {
                rowIndex = dataGridView1.SelectedRows[0].Index;
            }
            if (tagGroup != null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("TagName"));
                dt.Columns.Add(new DataColumn("IP"));
                dt.Columns.Add(new DataColumn("DataType"));
                dt.Columns.Add(new DataColumn("DbNr"));
                dt.Columns.Add(new DataColumn("StartAddr"));
                dt.Columns.Add(new DataColumn("DataLength"));
                dt.Columns.Add(new DataColumn("DataBit"));
                dt.Columns.Add(new DataColumn("Avilid"));
                dt.Columns.Add(new DataColumn("Value"));
                foreach (var item in tagGroup.Tags)
                {
                    dt.Rows.Add(item.TagName, item.Connection.IP, item.DateType, item.Dbnr, item.StartAddress, item.DataLength, item.DataBit, item.Result.Avaliable, item.Result.Value.ToString());

                    //if (item.Result.Value is DateTime)
                    //{
                    //    DateTime dtTemp = (DateTime)item.Result.Value;
                    //    dt.Rows.Add(item.TagName, item.Connection.IP, item.DateType, item.Dbnr, item.StartAddress, item.DataLength, item.DataBit, item.Result.Avaliable, dtTemp.ToString());
                    //}
                    //else
                    //{
                    //    dt.Rows.Add(item.TagName, item.Connection.IP, item.DateType, item.Dbnr, item.StartAddress, item.DataLength, item.DataBit,item.Result.Avaliable,item.Result.Value.ToString());
                    //}
                }
                dataGridView1.DataSource = dt;
                foreach (DataGridViewColumn  item in dataGridView1.Columns)
                {
                    item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    item.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                dataGridView1.Columns["Value"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.AllowUserToResizeRows = false;
                dataGridView1.MultiSelect = false;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.RowHeadersVisible = false;
                dataGridView1.ReadOnly = true;
                dataGridView1.FirstDisplayedScrollingRowIndex = p;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdataDataGrid();

            dataGridView1.Rows[rowIndex].Selected = true;

        }
        int p = 0;
        int rowIndex = 0;
        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            p = e.NewValue;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            
        }
        
        private void writeDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm2 = new Form2();
            frm2.StartPosition = FormStartPosition.CenterScreen;
            frm2.OnDataChange += new Action<string>(WritePlcData);
            frm2.Show();
            //frm2.OnDataChange -= WritePlcData;
        }

        private void WritePlcData(string obj)
        {
            Group group = (Group)treeView1.SelectedNode.Tag;
            if(this.dataGridView1.SelectedRows!=null)
            {
                string name = this.dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                Tag tag = group.GetTag(name);
                tag.WriteValue(obj);
            }
            Console.WriteLine();
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                this.contextMenuStrip1.Show(MousePosition);
            }
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
            Group group = (Group)treeView1.SelectedNode.Tag;
            if (this.dataGridView1.SelectedRows != null)
            {
                string name = this.dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                Tag tag = group.GetTag(name);
                if (tag.Result.Avaliable)
                {
                    if (tag.DateType == DataType.BIT)
                    {
                        if (tag.Result.Value.ToString()=="False")
                        {
                            tag.WriteValue(true);
                        }
                        else
                        {
                            tag.WriteValue(false);
                        }
                        
                    }
                }
               
               
            }
            Console.WriteLine();
        }
    }
}
