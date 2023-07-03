using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Print_Check
{
    public partial class Welcome : Form
    {
        /*public static Task timer = Task.Run(async delegate
        {
            await Task.Delay(10000);
        });*/

        public Welcome()
        {
            InitializeComponent();
            load();
        }

        public void load()
        {
            Database.ds.ReadXml(Environment.CurrentDirectory + "\\FetchedData.xml");
            Database.dt = Database.ds.Tables["Data"];

            if (Database.ds != null)
            {
                //timer.Wait();
                Hide();
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
