using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Threading.Tasks;

namespace Print_Check
{
    public partial class checkPrint : Form
    {
        PrintPreviewDialog pd = new PrintPreviewDialog();
        PrintDocument printDoc = new PrintDocument();

        public static Task timer = Task.Run(async delegate
        {
            await Task.Delay(5000);
        });

        public checkPrint()
        {
            InitializeComponent();
            load();
        }

        void load()
        {
            Welcome welcome = new Welcome();
            welcome.Show();
            timer.Wait();

            if (Database.dt != null)
            {
                welcome.Hide();
            }
        }

        void searchData(string value = null)
        {
            string number;
            if (string.IsNullOrEmpty(value))
            {
                lblName.Text = "******";
                lblDate.Text = "******";
                lblAmount.Text = "******";
                lblAmountText.Text = "******";
            }

            else
            {
                foreach (DataRow row in Database.dt.Select($"RefNumber like '%{value}%'"))
                {
                    lblName.Text = "***" + row[1].ToString() + "***";
                    lblDate.Text = Convert.ToDateTime(row[2].ToString()).ToString("MMMM dd, yyyy");
                    lblAmount.Text = "**" + (Convert.ToDouble(row[3])).ToString("N") + "**";
                    number = (Convert.ToDouble(row[3])).ToString("N");
                    number = ConvertToWords(Convert.ToDouble(number).ToString());
                    lblAmountText.Text = "**" + number + "**";
                }
            }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            searchData(tbRefNum.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UseWaitCursor = true;
            TopMost = true;
            load load = new load();
            load.Show();
            string query = "SELECT RefNumber, PayeeEntityRefFullName, TxnDate, Amount FROM Check";
            OdbcConnection con = new OdbcConnection("Dsn=QuickBooks Data;server=QODBC;");
            con.Open();
            OdbcCommand cmd = new OdbcCommand(query, con);

            List<Data> data1 = new List<Data>();
            XmlSerializer serializer = new XmlSerializer(typeof(List<Data>));
            
            using (cmd)
            {
                using (OdbcDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data1.Add(new Data() { RefNumber = (reader.GetValue(0) != DBNull.Value) ? reader.GetString(0) : "NULL", PayeeEntityRefFullName = (reader.GetValue(1) != DBNull.Value) ? reader.GetString(1) : "NULL", TxnDate = reader.GetDate(2).ToString(), Amount = reader.GetDecimal(3).ToString() }); ;
                    }
                }
            }

            using (FileStream fs = new FileStream(Environment.CurrentDirectory + "\\FetchedData.xml", FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(fs, data1);
                load.Hide();
                TopMost = false;
                UseWaitCursor= false;
            }
        }

        //Convert Amount To Text

        private static String ones(String Number)
        {
            int _Number = Convert.ToInt32(Number);
            String name = "";
            switch (_Number)
            {

                case 1:
                    name = "One";
                    break;
                case 2:
                    name = "Two";
                    break;
                case 3:
                    name = "Three";
                    break;
                case 4:
                    name = "Four";
                    break;
                case 5:
                    name = "Five";
                    break;
                case 6:
                    name = "Six";
                    break;
                case 7:
                    name = "Seven";
                    break;
                case 8:
                    name = "Eight";
                    break;
                case 9:
                    name = "Nine";
                    break;
            }
            return name;
        }

        private static String tens(String Number)
        {
            int _Number = Convert.ToInt32(Number);
            String name = null;
            switch (_Number)
            {
                case 10:
                    name = "Ten";
                    break;
                case 11:
                    name = "Eleven";
                    break;
                case 12:
                    name = "Twelve";
                    break;
                case 13:
                    name = "Thirteen";
                    break;
                case 14:
                    name = "Fourteen";
                    break;
                case 15:
                    name = "Fifteen";
                    break;
                case 16:
                    name = "Sixteen";
                    break;
                case 17:
                    name = "Seventeen";
                    break;
                case 18:
                    name = "Eighteen";
                    break;
                case 19:
                    name = "Nineteen";
                    break;
                case 20:
                    name = "Twenty";
                    break;
                case 30:
                    name = "Thirty";
                    break;
                case 40:
                    name = "Fourty";
                    break;
                case 50:
                    name = "Fifty";
                    break;
                case 60:
                    name = "Sixty";
                    break;
                case 70:
                    name = "Seventy";
                    break;
                case 80:
                    name = "Eighty";
                    break;
                case 90:
                    name = "Ninety";
                    break;
                default:
                    if (_Number > 0)
                    {
                        name = tens(Number.Substring(0, 1) + "0") + " " + ones(Number.Substring(1));
                    }
                    break;
            }
            return name;
        }

        private static String ConvertWholeNumber(String Number)
        {
            string word = "";
            try
            {
                bool beginsZero = false;//tests for 0XX    
                bool isDone = false;//test if already translated    
                double dblAmt = (Convert.ToDouble(Number));
                //if ((dblAmt > 0) && number.StartsWith("0"))    
                if (dblAmt > 0)
                {//test for zero or digit zero in a nuemric    
                    beginsZero = Number.StartsWith("0");

                    int numDigits = Number.Length;
                    int pos = 0;//store digit grouping    
                    String place = "";//digit grouping name:hundres,thousand,etc...    
                    switch (numDigits)
                    {
                        case 1://ones' range    

                            word = ones(Number);
                            isDone = true;
                            break;
                        case 2://tens' range    
                            word = tens(Number);
                            isDone = true;
                            break;
                        case 3://hundreds' range    
                            pos = (numDigits % 3) + 1;
                            place = " Hundred ";
                            break;
                        case 4://thousands' range    
                        case 5:
                        case 6:
                            pos = (numDigits % 4) + 1;
                            place = " Thousand ";
                            break;
                        case 7://millions' range    
                        case 8:
                        case 9:
                            pos = (numDigits % 7) + 1;
                            place = " Million ";
                            break;
                        case 10://Billions's range    
                        case 11:
                        case 12:

                            pos = (numDigits % 10) + 1;
                            place = " Billion ";
                            break;
                        //add extra case options for anything above Billion...    
                        default:
                            isDone = true;
                            break;
                    }
                    if (!isDone)
                    {//if transalation is not done, continue...(Recursion comes in now!!)    
                        if (Number.Substring(0, pos) != "0" && Number.Substring(pos) != "0")
                        {
                            try
                            {
                                word = ConvertWholeNumber(Number.Substring(0, pos)) + place + ConvertWholeNumber(Number.Substring(pos));
                            }
                            catch { }
                        }
                        else
                        {
                            word = ConvertWholeNumber(Number.Substring(0, pos)) + ConvertWholeNumber(Number.Substring(pos));
                        }

                        //check for trailing zeros    
                        //if (beginsZero) word = " and " + word.Trim();    
                    }
                    //ignore digit grouping names    
                    if (word.Trim().Equals(place.Trim())) word = "";
                }
            }
            catch { }
            return word.Trim();
        }

        private static String ConvertToWords(String numb)
        {
            String val = "", wholeNo = numb, points = "", andStr = "", pointStr = "";
            String endStr = "Pesos Only";
            try
            {
                int decimalPlace = numb.IndexOf(".");
                if (decimalPlace > 0)
                {
                    wholeNo = numb.Substring(0, decimalPlace);
                    points = numb.Substring(decimalPlace + 1);
                    if (Convert.ToInt32(points) > 0)
                    {
                        andStr = "Pesos and";// just to separate whole numbers from points/cents    
                        endStr = "Centavos Only";//Cents    
                        pointStr = ConvertDecimals(points);
                    }
                }
                val = String.Format("{0} {1}{2} {3}", ConvertWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);
            }
            catch { }
            return val;
        }

        private static String ConvertDecimals(String number)
        {
            String cd = "", digit = "", engOne = "";
            for (int i = 0; i < number.Length; i++)
            {
                digit = number[i].ToString();
                if (digit.Equals("0"))
                {
                    engOne = "Zero";
                }
                else
                {
                    engOne = ones(digit);
                }
                cd += " " + engOne;
            }
            return cd;
        }

        //Printing

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Print(panel1);
        }

        public void Print(Panel pnl)
        {
            PrinterSettings ps = new PrinterSettings();
            panel1 = pnl;
            getprintarea(pnl);
            pd.Document = printDoc;
            printDoc.PrintPage += new PrintPageEventHandler(printDoc_printpage);
            pd.ShowDialog();
        }

        public void printDoc_printpage(object sender, PrintPageEventArgs e)
        {
            Rectangle pagearea = e.PageBounds;
            e.Graphics.DrawImage(memoryImage, new Point(20, 30));
        }

        Bitmap memoryImage;

        public void getprintarea(Panel pnl)
        {
            memoryImage = new Bitmap(pnl.Width, pnl.Height);
            pnl.DrawToBitmap(memoryImage, new Rectangle(0, 0, pnl.Width, pnl.Height));
        }

    }
}
