using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RogerTech.Service;
using RogerTech.Tool;

namespace TestComm
{
    public partial class FrmMain : Form
    {
        UcCellCheck cellCheck;
        UcDataCollect dataCollect;
        UcReleaseSfc releaseSfc;
        UcReleaseSfc ucReleaseCheckSfc;
        Server plc = Server.GetInstance();
        public FrmMain()
        {
            InitializeComponent();

            Group group = new Group("", plc);
            string interfaceName = "CellCheck";
            cellCheck = new UcCellCheck(group,"");
            TreeNode node = new TreeNode(group.GroupName);
            node.Tag = cellCheck;
            treeView1.Nodes.Add(node);


        }
    }
}
