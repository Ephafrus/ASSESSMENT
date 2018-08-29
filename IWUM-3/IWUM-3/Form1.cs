using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace IWUM_3
{
    public partial class Form1 : Form
    {
        public string path = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Load_file_Click(object sender, EventArgs e)
        {
            

            stepps.Value = 25;
            string filename = Get_File();

            if(File.Exists(@filename) == true)
            {
                Display.Text = "File Selected......\r\n";

                int[] Department = Get_Department(filename);
                Display.Text += "Department ID Loaded......\r\n";
                stepps.Value = 35;


                string[] Date = Get_Dates(filename);
                Display.Text += "Dates Loaded......\r\n";
                stepps.Value = 45;



                float[] Volume = Get_Volume(filename);
                Display.Text += "Volumes Loaded......\r\n";
                stepps.Value = 55;

                string[] Headers = Get_Headers(filename);
                Display.Text += "Headers Loaded......\r\n";
                stepps.Value = 75;

                Generate_report_one(Headers, Department, Date, Volume, Department.GetUpperBound(0));
                stepps.Value = 100;


            }
            else
            {
              MessageBox.Show("Please Make Sure you Select a Valid Csv File", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }

        private void Generate_report_one(string[] headers, int[] department, string[] date, float[] volume, int num_rows)
        {
            string[] Headers = headers;

            int match = 0;
            int index = 0;
            var List_dept = new List<int>();
            var List_Date = new List<string>();
            var List_Volume = new List <float>();

            int[] Depts = new int[num_rows];
            string[] Dates = new string [num_rows];
            float[] Volumes =new float[num_rows];

            for (int i = 0; i < num_rows; i++)
            {
                for(int j = 0; j<num_rows; j++)
                {
                    if(department[i] == Depts[j] && date[i] == Dates[j])
                    {
                        for(int k =0; k< Depts.GetUpperBound(0);k++)
                        {
                            if(department[i]==Depts[k] & Dates[k] ==date[i])
                            {
                                Volumes[k] += Volumes[i];
                                match = 1;
                            }
                        }
                    }
                }

                if(match == 0)
                {
                    Depts[index] = department[i];
                    Dates[index]=  date[i];
                    Volumes[index] = volume[i];

                    index += 1;
                    match = 0;
                }
                else if(match == 1)
                {
                    match = 0;
                }
            }

            Display.Text += "\r\nReport Once Computed.Now Writing....";
            Generate_Report_1(Depts, Dates, Volumes, Headers, index);

            stepps.Value = 85;
            Generate_Report_2(Headers, Depts, Dates, Volumes, index);

            MessageBox.Show(Dates[66]);
            
        }

        private void Generate_Report_1(int[] depts, string[] dates, float[] volumes,string[] heads, int index)
        {
            if (!File.Exists("report1_" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".csv"))
            {
                
                StreamWriter sw = new StreamWriter(new FileStream("report1_" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".csv", FileMode.Create, FileAccess.Write));
                sw.WriteLine(heads[0] + "," +heads[1]+ "," + heads[2]);

                for (int item = 1; item < index+1; item++)
                {
                    
                    sw.WriteLine(depts[item].ToString() + "," + dates[item] + "," + volumes[item].ToString());

                }
                Display.Text += "\r\nNew Report FIle Created......";
            }
            else
            {
                File.Delete("report1_" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".csv");
                StreamWriter sw = new StreamWriter(new FileStream("report1_" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".csv", FileMode.Create, FileAccess.Write));
                sw.WriteLine(heads[0] + "," + heads[1] + "," + heads[2]);

                for (int item = 1; item < index+1; item++)
                {
                    
                    sw.WriteLine(depts[item].ToString() + "," + dates[item] + "," + volumes[item].ToString());

                }
                Display.Text += "\r\nReport FIle Updated......";
            }

           


        }

        private void Generate_Report_2(string[] heads, int[] depts, string[] dates, float[] volumes, int index)
        {
            DateTime Date = new DateTime();
            string s=""; 
            string[] New_ID = new string[index + 1];
            for (int i = 1; i< index; i ++)
            {
               // MessageBox.Show(dates[i]);

                Date = DateTime.Parse(dates[i]);
                New_ID[i] = depts[i] + "-" + Date.Month.ToString() + "-" + Date.Year.ToString();
                
            }

            Display.Text = s;
            //Use Lambda Calculus to Calculate Daily AVG per Month as Per Given Data Available
            var result = New_ID.Zip(volumes, (d, v) => new { Keys = d, Volumes = v })
                                       .ToLookup(x => x.Keys, x => x.Volumes);

            string[] D_Key = result.Select(dv => dv.Key).ToArray();
            float[] Davg = result.Select(dv => dv.Average()).ToArray();

            int al = D_Key.GetUpperBound(0) + 1;

            //Write Csv File
            s = "ORIGINAL     SPLIT";
            if (!File.Exists("report 2_" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".csv"))
            {

                StreamWriter sw = new StreamWriter(new FileStream("report1 2_" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".csv", FileMode.Create, FileAccess.Write));
                sw.WriteLine(heads[0] + ", Month-Year," + heads[2]);

                for (int item = 1; item< al ; item++)
                {
                    s += D_Key[item] +" ** ";
                    var chr = D_Key[item].Split('-');

                    sw.WriteLine(chr[0].ToString() + "," + chr[1]+"-"+chr[2] + "," + Davg[item].ToString());
                    s += chr[0] + "\r\n";
                }
                Display.Text += "\r\nNew Report 2 FIle Created......";
                Display.Text = s;
            }
            else
            {
                File.Delete("report 2_" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".csv");
                StreamWriter sw = new StreamWriter(new FileStream("report2_" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".csv", FileMode.Create, FileAccess.Write));
                sw.WriteLine(heads[0] + ", Month-Year," + heads[2]);


                for (int item = 1; item < al  ; item++)
                {

                    
                    var chr = D_Key[item].Split('-');

                    sw.WriteLine(chr[0].ToString() + "," + chr[1] + "-" + chr[2] + "," + Davg[item].ToString());
                }
                Display.Text += "\r\nReport 2 FIle Updated......";
            }
        
            stepps.Value = 95;
        }

        private string[] Get_Headers(string filename)
        {
            var reader = new StreamReader(File.OpenRead(@filename));
            var result = new List<string>();
           
                var row = reader.ReadLine();
                var rowvalue = row.Split(',');

                result.Add(rowvalue[0]);
                result.Add(rowvalue[1]);
                result.Add(rowvalue[2]);


           string[] values = result.ToArray();

            return values;
        }

        private int[] Get_Department(string filename)
        {
            var reader = new StreamReader(File.OpenRead(@filename)); //Open File 
            int c = 0;
            var result = new List<string>();  //Create a list of Entire Row <String>
            while (!reader.EndOfStream)
            {
                
                var row = reader.ReadLine();
                //Read From File
                if (row.Contains(",")) {
                    var rowvalue = row.Split(',');
                    result.Add(rowvalue[0]);

                    c = c + 1;
                }
                   //Split Row to columns 

                 //Get Column 1 Items 

            }
            Total_Rows.Text = "Total Rows  :"+c.ToString();
            string[] holder = result.ToArray();  //Add Vallues to Temp Array Holder 

            int[] values = new int[holder.GetUpperBound(0)+1]; //Get Full Leghth of Array Items 

            for (int i = 1; i < holder.GetUpperBound(0) + 1; i++)   //Convert To Id as Int Based On Give Assessment Input 
            {

                values[i] = int.Parse(holder[i]);   //Passing Values to Int 


            }

            return values;    //Returning array of Dept ID's
        }

        private string[] Get_Dates(string filename)
        {
            var reader = new StreamReader(File.OpenRead(@filename));
            var result = new List<string>();
            while (!reader.EndOfStream)
            {
                var row = reader.ReadLine();
                if (row.Contains(","))
                {
                    var rowvalue = row.Split(',');
                    result.Add(rowvalue[1]); //Date is on Index 1
                }  
            }

            string[] holder = result.ToArray();  //Add Vallues to Temp Array Holder 

            string[] values = new string[holder.GetUpperBound(0) + 1]; //Get Full Leghth of Array Items 
            for (int i = 1; i < holder.GetUpperBound(0) + 1; i++)   //Convert To Id as Int Based On Give Assessment Input 
            {
                
                values[i] = holder[i];   //Passing Values to Int 

            }
            

            return values;
        }

        private float[] Get_Volume( string filename)
        {
            //Read File From System 

            
            var reader = new StreamReader(File.OpenRead(@filename));
            var result = new List<string>();
            while (!reader.EndOfStream)
            {
                var row = reader.ReadLine();
                if (row.Contains(","))
                {
                    var rowvalue = row.Split(',');

                    result.Add(rowvalue[2]); //Volumes Are at Index 3
                }
               

            }


            string[] holder = result.ToArray();  //Add Vallues to Temp Array Holder 

            float[] values = new float[holder.GetUpperBound(0) + 1]; //Get Full Leghth of Array Items 
            for (int i = 1; i < holder.GetUpperBound(0) + 1; i++)   //Convert To Id as Int Based On Give Assessment Input 
            {
                
                values[i] = float.Parse(holder[i]);   //Passing Values to Int 

            }

           
            return values;
        }

        private string Get_File()
        {
            string fname = "";
            OpenFileDialog Browse = new OpenFileDialog();
           
            if(Browse.ShowDialog() == DialogResult.OK)
            {
                File_Path.Text = Browse.FileName;
                fname = Browse.FileName;
            }
            return fname;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Display.BackColor = Color.GhostWhite;
        }
    }
}
