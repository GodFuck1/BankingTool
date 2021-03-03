using Newtonsoft.Json;
using ServerTool;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BankingTool
{
    public partial class Form1 : Form
    { 
        string path;
        int n = 0;
        public Form1()
        {
            InitializeComponent();
            //Reg.nowEnglish = true;
           if(Reg.nowEnglish==true)
            {
                button1.Text = "Open banking folder";
                button2.Text = "Parse database";
                button3.Text = "Confirm";
                button4.Text = "Save all changes";
                button5.Text = "Search by nickname";
                button6.Text = "Search by SteamID";
                label1.Text = "Number of accounts";
                label2.Text = "Transfer limit";
                label4.Text = "Amount of money";
                //label4.Text = "";
                //label5.Text = "";
                label6.Text = "Changed:";
                //label7.Text = "";
                label10.Text = "Nickname/SteamID";
                groupBox1.Text = "Reducing the amount of money above the limit";
                groupBox2.Text = "Search player";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            int mmoney=0;
            string temp = "/changed";
            string[] files = Directory.GetFiles(path, "*.json");
            if (!Directory.Exists(path+temp))
                Directory.CreateDirectory(temp);
            for (int i = 0, a = files.Count(); i < a; i++)
            {
                try
                {
                    string json = File.ReadAllText(files[i]);
                    Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(json);
                    int money = myDeserializedClass.m_OwnedCurrency;
                    int maxmoney = myDeserializedClass.m_MaxOwnedCurrencyBonus;
                    string namee = myDeserializedClass.m_Username;
                    n += 1;
                    mmoney = mmoney + money;
                    string currid = myDeserializedClass.m_PlainID;
                    dataGridView1.Rows.Add(currid, namee, money, maxmoney);
                }
                catch (Exception)
                {
                }
                label3.Text = n.ToString();
                label5.Text = mmoney.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            using (FolderBrowserDialog dialog = folderBrowserDialog)
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    path = dialog.SelectedPath;
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var changed = 0;
            var money = int.Parse(textBox1.Text);
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[2].Value != null)
                {
                    int onhand;
                    onhand = int.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString());
                    if (money <= onhand)
                    {
                        dataGridView1.Rows[i].Cells[2].Value = money;
                        changed = changed + 1;
                    }
                }
                else
                {

                }
            }
            label7.Text = changed.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                    dataGridView1.Rows[i].Selected = false;
                    if (dataGridView1.Rows[i].Cells[1].Value != null)
                        if (dataGridView1.Rows[i].Cells[1].Value.ToString().Contains(textBox2.Text))
                        {
                            dataGridView1.Rows[i].Selected = true;
                        dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[1];
                        break;
                        }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Rows[i].Selected = false;
                if (dataGridView1.Rows[i].Cells[0].Value != null)
                    if (dataGridView1.Rows[i].Cells[0].Value.ToString().Contains(textBox2.Text))
                    {
                        dataGridView1.Rows[i].Selected = true;
                        dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[0];
                        break;
                    }
            }
        }
        static async void Serialize(string id,string username,int owned,int maxowned)
        {
            using (FileStream fs = new FileStream("BankingProfiles/" + id + ".json", FileMode.Create))
            {
                Root player = new Root() { m_PlainID = id, m_Username = username, m_OwnedCurrency = owned, m_MaxOwnedCurrencyBonus = maxowned };
                await System.Text.Json.JsonSerializer.SerializeAsync(fs, player);
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            progressBar1.Maximum=dataGridView1.RowCount;
            for (int i = 0; i < dataGridView1.RowCount-1; i++)
            {
                string id = dataGridView1.Rows[i].Cells[0].Value.ToString();
                string username= dataGridView1.Rows[i].Cells[1].Value.ToString();
                int owned= Convert.ToInt32(dataGridView1.Rows[i].Cells[2].Value);
                int maxowned= Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value);
                if (!Directory.Exists("BankingProfiles"))
                {
                   Directory.CreateDirectory("BankingProfiles");
                }
                Serialize(id, username, owned, maxowned);
                progressBar1.Value = i;
            }
            System.Diagnostics.Process.Start("explorer", "BankingProfiles");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
    public class Root
    {
        public string m_PlainID { get; set; }
        public string m_Username { get; set; }
        public int m_OwnedCurrency { get; set; }
        public int m_MaxOwnedCurrencyBonus { get; set; }
    }
}
