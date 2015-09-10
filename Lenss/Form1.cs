using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExifLib;
using System.IO;

namespace Lenss
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class MyGraphNum
        {
            public int One { get; set; }
            public int Two { get; set; }
            public int Three { get; set; }
            public int Four { get; set; }
            public int Five { get; set; }
            public int Six { get; set; }

            public MyGraphNum()
            { One = 0; Two = 0; Three = 0; Four = 0; Five = 0; Six = 0; }
        }

        public struct Exif
        {
            public double Aperture;
            public double Focal;
            public string Lens;
            public string Camera;
        }

        public class Camera
        {
            public string CameraID { get; set; }
            public int CamCount { get; set; }
            public Camera() { }

        }

        public class Lens
        {
            public string LensID { get; set; }
            public int LensCount { get; set; }
            public Lens(){}
        }

        public void CamereLensId(List<Camera> L1,List<Lens>L2,Exif check)
        {
            
            int index = L1.FindIndex(item => item.CameraID == check.Camera);
            if (index == 0)
            {
                // element not exists, do what you need
                L1.Add(new Camera { CameraID = check.Camera, CamCount = 1 });
            }
            else
            {
                foreach (var cc in L1.Where(x => x.CameraID == check.Camera))
                    cc.CamCount++;
            }

            int index2 = L2.FindIndex(item => item.LensID == check.Lens);
            if (index2 == 0)
            {
                // element not exists, do what you need
                L2.Add(new Lens { LensID = check.Lens, LensCount = 1 });
            }
            else
            {
                foreach (var lc in L2.Where(x => x.LensID == check.Lens))
                    lc.LensCount++;
            }
        }


        public Exif RFocus(string name)
        {
            Exif exif;
            try
            {
                using (ExifReader reader = new ExifReader(name))
                {

                    reader.GetTagValue<double>(ExifTags.FocalLength, out exif.Focal); if (exif.Focal > 250) { exif.Focal = 250; }
                    reader.GetTagValue<double>(ExifTags.ApertureValue, out exif.Aperture); if (exif.Aperture > 32) { exif.Aperture = 32; }
                    reader.GetTagValue<string>(ExifTags.LensModel, out exif.Lens);
                    reader.GetTagValue<string>(ExifTags.Model, out exif.Camera);

                    return exif;
                }
            }
            catch (Exception ex)
            {
                exif.Aperture = 0; exif.Camera = ""; exif.Focal = 0; exif.Lens = ""; return exif;
            }
        }


        public void DoWork()
        {
            var  ListCam = new List<Camera>();
            var ListLens = new List<Lens>() ;

            //L1.Add(new Camera { CameraID = check.Camera, CamCount = 1 });

            progressBar1.Minimum = 0;

            this.chart1.Series["Focal"].Points.Clear();
            this.chart2.Series["Aperture"].Points.Clear();
            chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.LineWidth = 0;
            chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineWidth = 0;

            chart2.ChartAreas["ChartArea1"].AxisX.MajorGrid.LineWidth = 0;
            chart2.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineWidth = 0;
            MyGraphNum init = new MyGraphNum();
            MyGraphNum init2 = new MyGraphNum();
            textBox4.Clear();

            folderBrowserDialog1.ShowDialog();
            var files = Directory.EnumerateFiles(folderBrowserDialog1.SelectedPath, "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".jpg"));

            progressBar1.Maximum = files.Count();
            double sum = 0;
            double sumA = 0;
            double temp2=0;
            Exif temp;
            int temp1 = 0;
            int truecountF = 0;
            int truecountA = 0;
            try
            {
                foreach (var item in files)
                {
                    progressBar1.Value++;
                    temp = RFocus(item);

                    ListCam.Add(new Camera { CameraID = temp.Camera, CamCount = 1 });
                    ListLens.Add(new Lens { LensID = temp.Lens, LensCount = 1 });
                    //CamereLensId(ListCam, ListLens, temp);
                    if (!String.IsNullOrEmpty(Convert.ToString(temp.Focal)))
                    {
                        temp1 = Convert.ToInt32(Math.Round(temp.Focal, 0));
                        if (temp1 < 20) { init.One++; }
                        else if (temp1 < 31) { init.Two++; }
                        else if (temp1 < 51) { init.Three++; }
                        else if (temp1 < 86) { init.Four++; }
                        else if (temp1 < 136) { init.Five++; }
                        else if (temp1 > 135) { init.Six++; }

                       sum += Convert.ToDouble(temp.Focal);
                       truecountF++;

                        if (!String.IsNullOrEmpty((temp.Aperture.ToString())))
                        {
                            temp2 = Math.Round(temp.Aperture, 2);
                            if (temp2 < 2.0) { init2.One++; }
                            else if (temp2 < 4.0) { init2.Two++; }
                            else if (temp2 < 5.7) { init2.Three++; }
                            else if (temp2 < 8.1) { init2.Four++; }
                            else if (temp2 < 10.1) { init2.Five++; }
                            else if (temp2 > 10) { init2.Six++; }
                            sumA += Convert.ToDouble(temp.Aperture);
                            truecountA++;
                        }

                    }

                }
                sum = Math.Round(sum / truecountF, 0);
                sumA = Math.Round(sumA / truecountA, 1);
                textBox1.Text = Convert.ToString(files.Count());
                textBox2.Text = sum.ToString();
                textBox3.Text = sumA.ToString();

            }
            catch (Exception ex)
            {
                // ex.ToString()
                //textBox1.AppendText("Error!\n");
            }


            progressBar1.Value = 0;
            this.chart1.Series["Focal"].Points.AddXY("0-20", init.One);
            this.chart1.Series["Focal"].Points.AddXY("20-30", init.Two);
            this.chart1.Series["Focal"].Points.AddXY("30-50", init.Three);
            this.chart1.Series["Focal"].Points.AddXY("50-85", init.Four);
            this.chart1.Series["Focal"].Points.AddXY("85-130", init.Five);
            this.chart1.Series["Focal"].Points.AddXY("130-x", init.Six);

            this.chart2.Series["Aperture"].Points.AddXY("<2.0", init2.One);
            this.chart2.Series["Aperture"].Points.AddXY("<4", init2.Two);
            this.chart2.Series["Aperture"].Points.AddXY("<5.6", init2.Three);
            this.chart2.Series["Aperture"].Points.AddXY("<8", init2.Four);
            this.chart2.Series["Aperture"].Points.AddXY("<10", init2.Five);
            this.chart2.Series["Aperture"].Points.AddXY("10<x", init2.Six);

            textBox4.AppendText("Cameras:\n");
            var Listofcameramodels = ListCam.Select(x => x.CameraID).Distinct();
            foreach (var item in Listofcameramodels)
            {
                if (!String.IsNullOrWhiteSpace(item))
                {
                    textBox4.AppendText(item + "\n");
                }
                
            }

            var Listoflensmodels = ListLens.Select(x => x.LensID).Distinct();
            textBox4.AppendText("Lenses:\n");
            foreach (var item in Listoflensmodels)
            {
                if (!String.IsNullOrWhiteSpace(item))
                {
                    textBox4.AppendText(item + "\n");
                }
                
            }         
                
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DoWork();
        }
    }
}
