using System;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.Net;
using System.IO;
using BankingTool;
using System.Diagnostics;

namespace ServerTool
{
    public partial class Reg : Form
    {
        bool english = false;
        public static bool nowEnglish;

        public Reg()
        {
            InitializeComponent();
            iRegistry(0, "servertool", false.ToString());
            CenterToScreen();
          //  if()
        }
        void englishing()
        {
            label1.Text = "License not found.";
            button1.Text = "Author";
            button2.Text = "Apply";
            nsButton1.Text = "Copy";
            nowEnglish = true;
        }
        #region Registry - Работа с реестром
        //При viI=0 - установка и восстановление параметров
        //При viI=1 - сохранение параметров
        private int iRegistry(int viI, string sS, string sS1)
        {
            //Формируем имя нашего подраздела (здесь HKEY_CURRENT_USER\Software\Wladm\hello)
            sS = @"Software\" + sS + @"\" + sS1;
            Microsoft.Win32.RegistryKey clRegistryKey;
            if (viI == 0)
            {
                //Проверяем есть ли наш подраздел
                clRegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(sS);
                if (clRegistryKey == null)
                {
                    //Раздела нет - создаем и открываем для чтения и добавления
                    clRegistryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(sS);
                }
            }
            else
            {
                //Открываем на запись в существующий
                clRegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(sS, true);
            }
            //К этому времени раздел должен быть
            if (clRegistryKey != null)
            {
                if (viI == 0)
                {
                    //Есть ли параметры в разделе
                    string[] sKeyNames = clRegistryKey.GetValueNames();
                    if (sKeyNames.Length != 0)
                    {
                        //Параметры есть
                        //Получаем хранимые значения параметров
                        foreach (string s in sKeyNames)
                        {
                            switch (s)
                            {
                                case "language":
                                   // Width = (int)clRegistryKey.GetValue("language");
                                   if(Convert.ToBoolean(clRegistryKey.GetValue("language"))==false)
                                    {
                                        
                                    }
                                    else { englishing(); }
                                    break;
                            }
                        }
                        clRegistryKey.Close();
                        return 0;
                    }//if(sKeyNames.Length != 0)
                    else
                    {
                        //Параметров нет
                        // Записываем инициализирующие значения
                        clRegistryKey.SetValue("language", english.ToString());
                        return 0;
                    }
                }//if(viI == 0)
                else
                {
                    try
                    {
                        //Сохраняем текущие значения
                        clRegistryKey.SetValue("language", english.ToString());
                        clRegistryKey.Close();
                    }
                    catch (Exception)
                    {
                        //Ошибка работы с реестром
                        return 1;
                    }//try catch
                    return 0;
                }//if(viI == 0) else
            }

            return 1;
        }//private int iRegistry() 
        #endregion
        private static string GetHWID()
        {
            var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
            ManagementObjectCollection mbsList = mbs.Get();
            string id = "";
            foreach (ManagementObject mo in mbsList)
            {
                id = mo["ProcessorId"].ToString();
                break;
            }
            return id;
        }
        public string getDays(TimeSpan time)
        {
            if (nowEnglish == true) { return ((time < TimeSpan.Zero) ? String.Format("License expired {0} days {1} hours {2} minuts before", Math.Abs(time.Days).ToString(), Math.Abs(time.Hours).ToString(), Math.Abs(time.Minutes).ToString()) : String.Format("License active. Left {0} days {1} hours {2} minutes", time.Days, time.Hours, time.Minutes)); }
            else return ((time < TimeSpan.Zero) ? String.Format("Лицензия истекла {0} дней {1} часов {2} минут назад", Math.Abs(time.Days).ToString(), Math.Abs(time.Hours).ToString(), Math.Abs(time.Minutes).ToString()) : String.Format("Лицензия активна. Осталось {0} дней {1} часов {2} минут", time.Days, time.Hours, time.Minutes));
        }

        public static string getSignedText(string[] response)
        {
            string returned = String.Empty;
            for (int i = 0; i < response.Length - 1; i++)
            {
                returned += response.GetValue(i) + "\r\n";
            }
            return returned;
        }
        public static string info(Random rn, Int32 token1, string preKey)
        {
            string tokenString = String.Format("token={0}&hwid={1}", DigitalSign.EncryptString(token1.ToString(), preKey), GetHWID());
            return tokenString;
        }
        public static string GetRequest(string url, string post)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            byte[] buffer = Encoding.UTF8.GetBytes(post);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = buffer.Length;
            request.Method = "POST";
            Stream newStream = request.GetRequestStream();
            newStream.Write(buffer, 0, post.Length);
            newStream.Close();
            try
            { 
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader strReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1251));
                string WorkingPage = strReader.ReadToEnd();
                response.Close();
                return WorkingPage;
            }
            catch (Exception e) { MessageBox.Show(e.Message);  return e.Message; }
           
        }


        public static string randomStringWithNumbers(int maxlength, Random rn)
        {
            StringBuilder sb = new StringBuilder();
            char[] allowedChars = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (int i = 0; i < maxlength; i++)
            {
                int n = rn.Next(0, allowedChars.Length);
                if (char.IsLetter(allowedChars[n]))
                {
                    if (rn.Next(0, 2) == 0)
                    {
                        sb.Append(allowedChars[n].ToString().ToUpper());
                    }
                    else
                    {
                        sb.Append(allowedChars[n]);
                    }
                }
                else
                {
                    sb.Append(allowedChars[n]);
                }
            }
            return sb.ToString();
        }
        private void Reg_Load(object sender, EventArgs e)
        {
            textBox1.Text = GetHWID();
            string pubKeyNotXORed = "<RSAKeyValue><Modulus>qhA6WG54Aosn4RFNoAt1F/BX3lB1lXbNb3Pv7w30zzAYvHDLKD/+W/woggq6Y5bunz4GpsNl3z5NqQWsWTDkmpHwSTxMhabn/16G4TBB/RimVKJIn7fb7j3v3lYD5UtOGbJ5U/BXMpVPTIWjs73Zz9KNLmkGS1UQXEgpN4H4rQDOnPAqebuQZkTLKfIUwJgewcTpzip6OhInuWdobnV7oi71W/5SjgyRokgDy98ci/M6Jp41OOp7esVzMM3OU1JRGQmeRjFY26Op58vA5BeWy9zex8O7EOxOZegNRc1g5UlYx8bEShYs4WmAKBtoZW5mTDuXExpzDSSqCg8vb4UEdQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            Random curRandom = new Random();
            string preKey = randomStringWithNumbers(curRandom.Next(15, 21), curRandom);
            int XORkey = curRandom.Next(1, int.MaxValue);
            string urlToScript = DigitalSign.XOR("https://connect.dayz-tool.online/projects/rsa_generator/base.php", XORkey);
            string pubKey = DigitalSign.XOR(pubKeyNotXORed, XORkey);

            int token = curRandom.Next(1000000, int.MaxValue);
            string infoXORed = DigitalSign.XOR(info(curRandom, token, preKey), XORkey);
            string responseXORed = DigitalSign.XOR(GetRequest(DigitalSign.XOR(urlToScript, XORkey), DigitalSign.XOR(infoXORed, XORkey)), XORkey);
            string[] responseSplitted = DigitalSign.XOR(responseXORed, XORkey).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                if (responseSplitted[0].Split('=')[1] == "1")
                {

                    if (int.Parse(DigitalSign.DecryptString(responseSplitted[4], preKey)) == token)
                    {
                        if (GetHWID() == Encoding.UTF8.GetString(Convert.FromBase64String(responseSplitted[1].Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries)[0])))
                        {
                            if (DigitalSign.CompareRSAMethod(getSignedText(responseSplitted), responseSplitted[responseSplitted.Length - 1].Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries)[0], DigitalSign.XOR(pubKey, XORkey)))
                            {

                                DateTime CurrentTime = DateTime.Parse(responseSplitted[2].Split('=')[1]);
                                DateTime EndTime = DateTime.Parse(responseSplitted[3].Split('=')[1]);
                                TimeSpan ActivatedTime = EndTime.Subtract(CurrentTime);
                                if (ActivatedTime < TimeSpan.Zero)
                                {
                                    MessageBox.Show(getDays(ActivatedTime));
                                    label1.Text = getDays(ActivatedTime);
                                   
                                }
                                else
                                {
                                    nsButton2.Enabled = true;
                                   // MessageBox.Show(getDays(ActivatedTime));
                                    label1.Text = getDays(ActivatedTime);
                                   
                                }

                            }
                        }
                    }
                }
                else
                {
                    if(nowEnglish==true) MessageBox.Show("License not found!");
                    else MessageBox.Show("Лицензии не обнаружено!");
                }
            }
            catch (Exception es)
            {
                if (nowEnglish == true) MessageBox.Show(es.Message);
                else MessageBox.Show(es.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/6wUyPQNeYz");
        }

        private void button2_Click(object sender, EventArgs e)
        {
                iRegistry(1, "servertool", false.ToString());
            Application.Restart();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            english = true;
            button2.Text = "Confirm";
            button2.Refresh();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            english=false;
            button2.Text = "Подтвердить";
            button2.Refresh();
        }

        private void nsRadioButton2_CheckedChanged(object sender)
        {
            english = true;
            button2.Text = "Confirm";
            button2.Refresh();
        }

        private void nsRadioButton1_CheckedChanged(object sender)
        {
            english = false;
            button2.Text = "Подтвердить";
            button2.Refresh();
        }

        private void nsButton1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
        }

        private void nsTheme1_Click(object sender, EventArgs e)
        {
            string publicKey = String.Empty;
            string privateKey = String.Empty;
            DigitalSign.AssignNewKey(ref privateKey, ref publicKey);
            textBox2.Text = privateKey;
            textBox3.Text = publicKey;
            label3.Text = "publ";
        }

        private void nsButton2_Click(object sender, EventArgs e)
        {
            Hide();
            Visible = false;
            Opacity = 0;
            Form1 tool = new Form1();
            tool.Show();
        }
    }
}
