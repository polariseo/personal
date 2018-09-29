using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Helpers;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.UserSkins;
using System.Runtime.InteropServices;
using System.Collections;
using GxIAPINET;
using GxIAPINET.Sample.Common;
using System.IO.Ports;
using System.Drawing.Imaging;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraSpreadsheet.Model;

namespace DXApplication7
{
    //主窗委托子窗
    public delegate void HistEventHandler(Bitmap bmpToForm2, int PictureNum);

    public partial class 实验软件 : RibbonForm
    {
        public IGXStream m_objIGXStream = null;
        public IGXDevice m_objIGXDevice = null;
        public IGXFactory m_objIGXFactory = null;
        public IGXFeatureControl m_objIGXFeatureControl = null;
        public GxBitmap m_objGxBitmap = null;
        public string m_strFilePath = "";
        public string m_strBalanceWhiteAutoValue = "Off";
        public SerialPort Com;
        public int NumPZTstep = 0;
        public double LengthPZTstep = 0.0;
        public bool PZTOROpen = false;
        public Bitmap CCDImg;
        public bool CCDTake = false;
        public bool CCDStop = false;
        public bool unwrap13D = false;
        public bool unwrap3DFinish = false;
        public bool unwrap23D = false;
        public static int zeridx;
        public bool orfit = false;
        public bool Firdown = false;

        //委托事件
        public event HistEventHandler HistEvent;

        public static Bitmap fourstep1, fourstep2, fourstep3, fourstep4, fourstep5;

        public 实验软件()
        {
            m_strFilePath = Directory.GetCurrentDirectory().ToString();
            m_strFilePath = m_strFilePath + "\\GxSingleCamImages";
            if (!Directory.Exists(m_strFilePath))
            {
                Directory.CreateDirectory(m_strFilePath);
            }

            InitializeComponent();

            //picturebox句柄
            picShow[0] = FirPic;
            picShow[1] = SecPic;
            //InitSkinGallery();
            ZernikeInterfere.Visible = false;
            barEditItem3.EditValue = true;
            CCDShow.BringToFront();
            GainSet.EditValue = 1;
            ExposureTime.EditValue = 40;
            Draw3D.Visible = false;
            CreateCurve.Visible = false;
            SaveData.Visible = false;
            SourcePicL.Visible = false;
            ML.Visible = false;
            Unwrap1L.Visible = false;
            Unwrap2L.Visible = false;
            CoreEdit.EditValue = "3";
            //
            CCDCatchPicture.Enabled = false;
            CCDGETPICTURE.Enabled = false;
            CCDSave.Enabled = false;
            GainSet.Enabled = false;
            ExposureTime.Enabled = false;
            length.Enabled = false;
            NumSteps.Enabled = false;
            CCDContinuous.Enabled = false;
            Comfirm.Enabled = false;
            PZTSet0.Enabled = false;
            Firunwrap.Enabled = false;
            Secunwrap.Enabled = false;
            Button11.Enabled = false;
            GETREAL.Enabled = false;
            SETSTD.Enabled = false;
            Button10.Enabled = false;
            PZTSet0.Enabled = false;
            PZTConfirm.Enabled = false;
            button7.Enabled = false;
            EdgeDetect.Enabled = false;
            button9.Enabled = false;
            barButtonItem13.Enabled = false;
            CheckGray.Enabled = false;
            comboBox3.Enabled = false;
            ERODE.Enabled = false;
            dilate.Enabled = false;
            barEditItem3.Enabled = false;
            PV.Enabled = false;
            RMS.Enabled = false;
        }

        //void InitSkinGallery()
        //{
        //SkinHelper.InitSkinGallery(rgbiSkins, true);
        //}

        //**// ZYH Add
        //picturebox句柄
        public static PictureBox[] picShow = new PictureBox[2];
        public static int PictureNum; //图片显示位置

        public static Bitmap bmp; //图像传递BitMap格式
        public static Bitmap bmpI;//输入图像的原图
        public static Bitmap bmpM;//输入图像的调制度图
        public static IntPtr Mask; //孔径掩膜板
        public static bool MaskFlag = false; //掩膜板Flag

        public static Bitmap bmpFilt;    //滤波后的Bmp
        public static Bitmap bmpErode;   //腐蚀后的bmp
        public static Bitmap bmpDilate;  //膨胀后的bmp
        public static Bitmap bmpCanny;   //边缘提取后的bmp
        public static Bitmap bmpCircle;  //孔径确定后的bmp
        public static Bitmap bmpToCanny; //

        public static bool dilateFlag = false;
        public static bool erodeFlag = false;

        //委托函数：对比图调整
        public void ShowContrast(Bitmap bmpContrast)
        {
            picShow[PictureNum].Image = bmpContrast;
        }

        //**// ZYH Add

        public static BitmapInfo GetImagePixel(Bitmap Source)
        {
            byte[] result; int step;
            int iWidth = Source.Width; int iHeight = Source.Height;
            Rectangle rect = new Rectangle(0, 0, iWidth, iHeight);
            System.Drawing.Imaging.BitmapData bmpData = Source.LockBits(rect,
                System.Drawing.Imaging.ImageLockMode.ReadWrite, Source.PixelFormat);
            IntPtr iPtr = bmpData.Scan0;
            int iBytes = iWidth * iHeight * 1;  //根据通道数进行设置 
            byte[] PixelValues = new byte[iBytes];
            System.Runtime.InteropServices.Marshal.Copy(iPtr, PixelValues, 0, iBytes);
            Source.UnlockBits(bmpData);
            step = bmpData.Stride;
            result = PixelValues;
            BitmapInfo bi = new BitmapInfo { Result = result, Step = step };
            return bi;
        }





        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        bool ifopenCCD = false;
        public int slinex, sliney, olinex, oliney;
        public Graphics g;
        public System.Drawing.Drawing2D.GraphicsState state;
        private void OpenCCD_Click(object sender, ItemClickEventArgs e)
        {
            m_objIGXFactory = IGXFactory.GetInstance();
            if (null != m_objIGXFactory)
            {
                m_objIGXFactory.Uninit();
            }
           
         
            m_objIGXFactory.Init();


            if (null != m_objIGXFeatureControl)
            {
                m_objIGXFeatureControl.GetCommandFeature("AcquisitionStop").Execute();
            }
            if (null != m_objIGXStream)
            {
                m_objIGXStream.StopGrab();
                m_objIGXStream.UnregisterCaptureCallback();
                m_objIGXStream.Close();
                m_objIGXStream = null;
            }
            if (null != m_objIGXDevice)
            {
                m_objIGXDevice.Close();
                m_objIGXDevice = null;
            }
  


            CCDShow.BringToFront();
            CCDStop = false;
            List<IGXDeviceInfo> listGXDeviceInfo = new List<IGXDeviceInfo>();

           
            m_objIGXFactory.UpdateDeviceList(200, listGXDeviceInfo);
            if (listGXDeviceInfo.Count <= 0)
            {
                XtraMessageBox.Show("未发现设备!");
                return;
            }
            //打开列表第一个设备

            if(listGXDeviceInfo[0].GetAccessStatus()== GX_ACCESS_STATUS.GX_ACCESS_STATUS_NOACCESS)
            {
                MessageBox.Show("连接错误，请重新拔插CCD电源");
                return;
            }
            m_objIGXDevice = m_objIGXFactory.OpenDeviceBySN(listGXDeviceInfo[0].GetSN(), GX_ACCESS_MODE.GX_ACCESS_EXCLUSIVE);
            m_objIGXFeatureControl = m_objIGXDevice.GetRemoteFeatureControl();
          



            if (null != m_objIGXDevice)
            {
                m_objIGXStream = m_objIGXDevice.OpenStream(0);
            }
            if (null != m_objIGXFeatureControl)
            {
                //设置采集模式连续采集
                m_objIGXFeatureControl.GetEnumFeature("AcquisitionMode").SetValue("Continuous");

            }
            XtraMessageBox.Show("CCD已打开");
            CCDCatchPicture.Enabled = true;
            CCDGETPICTURE.Enabled = true;
            CCDSave.Enabled = true;
            GainSet.Enabled = true;
            ExposureTime.Enabled = true;


            //粘贴实时
            CCDShow.Show();
            CCDTake = false;
            __SetEnumValue("TriggerMode", "Off", m_objIGXFeatureControl);

            //开启采集流通道
            if (null != m_objIGXStream)
            {
                m_objIGXStream.RegisterCaptureCallback(this, __CaptureCallbackPro);
                m_objIGXStream.StartGrab();
            }

            //发送开采命令
            if (null != m_objIGXFeatureControl)
            {
                m_objIGXFeatureControl.GetCommandFeature("AcquisitionStart").Execute();
            }
            m_objIGXFeatureControl.GetFloatFeature("ExposureTime").SetValue(40);
            m_objIGXFeatureControl.GetFloatFeature("Gain").SetValue(1);
            ifopenCCD = true;

        }

        private void CCDGETPICTURE_Click(object sender, ItemClickEventArgs e)
        {
            
            CCDShow.Show();
            CCDTake = false;
            __SetEnumValue("TriggerMode", "Off", m_objIGXFeatureControl);
            //开启采集流通道
            if (null != m_objIGXStream)
            {
                m_objIGXStream.RegisterCaptureCallback(this, __CaptureCallbackPro);
                m_objIGXStream.StartGrab();
            }

            //发送开采命令
            if (null != m_objIGXFeatureControl)
            {
                m_objIGXFeatureControl.GetCommandFeature("AcquisitionStart").Execute();
            }
            m_objIGXFeatureControl.GetFloatFeature("ExposureTime").SetValue(dShutterValue);
            m_objIGXFeatureControl.GetFloatFeature("Gain").SetValue(dGain);
            
        }

        
            private void __CaptureCallbackPro(object objUserParam, IFrameData objIFrameData)
            {
                try
                {
                    实验软件 objGxSingleCam = objUserParam as 实验软件;
                    objGxSingleCam.ImageShowAndSave(objIFrameData);
                }
                catch (Exception)
                {
                }
            }


            void ImageShowAndSave(IFrameData objIFrameData)
            {
                try
                {

                    IntPtr temppicture = objIFrameData.GetBuffer();
                    int twidth = (int)objIFrameData.GetWidth();
                    int theight = (int)objIFrameData.GetHeight();
                    int tsteps = twidth;

                    if (twidth % 4 == 0)
                    {
                        tsteps = twidth;
                    }
                    if (twidth % 4 == 1)
                    {
                        tsteps = twidth + 3;
                    }
                    if (twidth % 4 == 2)
                    {
                        tsteps = twidth + 2;
                    }
                    if (twidth % 4 == 3)
                    {
                        tsteps = twidth + 1;
                    }


                    CCDImg = new Bitmap(twidth, theight, tsteps,
                      System.Drawing.Imaging.PixelFormat.Format8bppIndexed, temppicture);
                    CCDImg.Palette = CvToolbox.GrayscalePalette;

                    //pictureEdit2.Image = CCDImg;
                    //pictureEdit2.Invalidate();
                    //pictureEdit2.Refresh();
                    CCDShow.Image = CCDImg;
                    CCDShow.Invalidate();
                    CCDShow.Refresh();

                }
                catch (Exception)
                {
                }
            }

            
        private void CCDCatchPicture_Click(object sender, ItemClickEventArgs e)
        {
            CCDTake = true;
            __SetEnumValue("TriggerMode", "On", m_objIGXFeatureControl);
        }
        
        private void __SetEnumValue(string strFeatureName, string strValue,
              IGXFeatureControl objIGXFeatureControl)
        {

            if (null != objIGXFeatureControl)
            {
                //设置当前功能值
                objIGXFeatureControl.GetEnumFeature(strFeatureName).SetValue(strValue);
            }

        }
        
        public int CCDPICidx = 0;
        private void CCDSave_Click(object sender, ItemClickEventArgs e)
        {
            
            if (CCDTake)
            {
 
                string localFilePath = String.Empty;
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "图片文件(*.BMP)|*.BMP";
                saveDialog.RestoreDirectory = true;
                if(saveDialog.ShowDialog()==DialogResult.OK)
                {
                    localFilePath = saveDialog.FileName.ToString();
                    CCDImg.Save(localFilePath);
                }

            }
            
        }
        
        public bool orPZT = false;

        private void OpenPZT_Click(object sender, ItemClickEventArgs e)
        {
            length.Enabled = true;
            NumSteps.Enabled = true;
            CCDContinuous.Enabled = true;
            Comfirm.Enabled = true;
            PZTSet0.Enabled = true;
            Com = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            try
            {
                Com.Open();
            }
            catch
            {
                XtraMessageBox.Show("串口打开失败");
                return;
            } 
            byte[] data = new byte[] { 0xAA, 0x00,
                0x82, 0x03, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x2F };
            Com.Write(data, 0, data.Length);
            PZTOROpen = true;
            Com.Close();
            Thread.Sleep(500);
            Com.Open();
            byte[] data0 = PZTU2Data(0.0);
            Com.Write(data0, 0, data0.Length);
            Thread.Sleep(500);
            Com.Close();
            orPZT = true;
            PZTSet0.Enabled = true;
            PZTConfirm.Enabled = true;
        }
        
        private void STOPCCD_Click(object sender, ItemClickEventArgs e)
        {
            
            CCDStop = true;
            if (null != m_objIGXFeatureControl)
            {
                m_objIGXFeatureControl.GetCommandFeature("AcquisitionStop").Execute();
            }


            //关闭采集流通道

            if (null != m_objIGXStream)
            {
                m_objIGXStream.StopGrab();
                //注销采集回调函数
                m_objIGXStream.UnregisterCaptureCallback();
                m_objIGXStream.Close();
                m_objIGXStream = null;
            }
            if (null != m_objIGXDevice)
            {
                m_objIGXDevice.Close();
                m_objIGXDevice = null;
            }
        
        }
     
        private void PZTConfirm_Click(object sender, ItemClickEventArgs e)
        {

        }

        private void PZTSet0_Click(object sender, ItemClickEventArgs e)
        {
            Com.Open();
            byte[] data0 = PZTU2Data(0.0);
            Com.Write(data0, 0, data0.Length);
            Thread.Sleep(1000);
            Com.Close();
        }
        
        private void IImageData2bitmap(IImageData objImageData)
        {

            //
            //
            IntPtr temppicture = objImageData.GetBuffer();
            int twidth = (int)objImageData.GetWidth();
            int theight = (int)objImageData.GetHeight();
            int tsteps = twidth;

            if (twidth % 4 == 0)
            {
                tsteps = twidth;
            }
            if (twidth % 4 == 1)
            {
                tsteps = twidth + 3;
            }
            if (twidth % 4 == 2)
            {
                tsteps = twidth + 2;
            }
            if (twidth % 4 == 3)
            {
                tsteps = twidth + 1;
            };

            CCDImg = new Bitmap(twidth, theight, tsteps,
              System.Drawing.Imaging.PixelFormat.Format8bppIndexed, temppicture);
            CCDImg.Palette = CvToolbox.GrayscalePalette;

        }
        
        [DllImport("CDLLrgb2gray.dll")]
        public static extern IntPtr rgbTogray(IntPtr image, int width, int height, int size, out int step, int channels);
        public int c = 0;//yueyue
        private void fourstep_Click(object sender, ItemClickEventArgs e)
        {
            /*        
            int numSteps = 10;
            if (!orPZT)
            {
                XtraMessageBox.Show("请打开PZT");
                return;
            }
            if (SelectExp.DataIndex == -1)
            {
                XtraMessageBox.Show("请选择实验");              
                return;
            }
            if(!ifopenCCD)
            {
                XtraMessageBox.Show("请打开CCD");
                return;
            }
            else
            {
                int ExpNo = SelectExp.DataIndex + 1;
                string picpath = "F:\\" + "Exp" + ExpNo.ToString() + "Pictures" ;
                if(!Directory.Exists(picpath))
                {
                    Directory.CreateDirectory(picpath);
                }

                if (!Fourstepscatter.Checked)
                {
                    int idxExp1 = SelectExp.DataIndex + 1;
                    string picname = "F:\\" + "Exp" + idxExp1.ToString() + "Pictures" + "\\no1"
                        + ".BMP";

                    Com.Open();
                    byte[] U0 = PZTU2Data(0);

                    Com.Write(U0, 0, U0.Length);
                    Thread.Sleep(2000);
                    CCDImg.Save(picname);
                    Thread.Sleep(500);
                    //     fourstep1 = CCDImg.Clone(new Rectangle(0, 0, CCDImg.Width, CCDImg.Height), CCDImg.PixelFormat);
                    if (null != m_objIGXStream)
                    {
                        m_objIGXStream.FlushQueue();
                    }
                  
                  
                    CCDShow.Invalidate();
                    CCDShow.Refresh();


                    string picname2 = "F:\\" + "Exp" + idxExp1.ToString() + "Pictures" + "\\no2"
             + ".BMP";
                    double l8u = PZTX2U(0.6328 / 8 );
                    byte[] U1 = PZTU2Data(10);

                    Com.Write(U1, 0, U1.Length);
                    Thread.Sleep(2000);
                    CCDImg.Save(picname2);
             //       fourstep2 = CCDImg.Clone(new Rectangle(0, 0, CCDImg.Width, CCDImg.Height), CCDImg.PixelFormat);

                    Thread.Sleep(500);

                    if (null != m_objIGXStream)
                    {
                        m_objIGXStream.FlushQueue();
                    }

                    CCDShow.Invalidate();
                    CCDShow.Refresh();

             

                    string picname3 = "F:\\" + "Exp" + idxExp1.ToString() + "Pictures" + "\\no3"
             + ".BMP";
                    double l4u = PZTX2U(0.6328 / 4);
                    byte[] U2 = PZTU2Data(20);
                    Com.Write(U2, 0, U2.Length);
                    Thread.Sleep(2000);
                    CCDImg.Save(picname3);
              //      fourstep3 = CCDImg.Clone(new Rectangle(0, 0, CCDImg.Width, CCDImg.Height), CCDImg.PixelFormat);

                    Thread.Sleep(500);
                    if (null != m_objIGXStream)
                    {
                        m_objIGXStream.FlushQueue();
                    }

                    CCDShow.Invalidate();
                    CCDShow.Refresh();



                    string picname4 = "F:\\" + "Exp" + idxExp1.ToString() + "Pictures" + "\\no4"
            + ".BMP";
                    //double l38u = PZTX2U(0.6328 * 3 / 8 + x0);
                    byte[] U3 = PZTU2Data(30);
                    Com.Write(U3, 0, U3.Length);
                    Thread.Sleep(2000);
                    CCDImg.Save(picname4);
               //     fourstep4 = CCDImg.Clone(new Rectangle(0, 0, CCDImg.Width, CCDImg.Height), CCDImg.PixelFormat);

                    Thread.Sleep(500);
                    if (null != m_objIGXStream)
                    {
                        m_objIGXStream.FlushQueue();
                    }

                    CCDShow.Invalidate();
                    CCDShow.Refresh();





                    string picname5 = "F:\\" + "Exp" + idxExp1.ToString() + "Pictures" + "\\no5"
              + ".BMP";
             //       double l2u = PZTX2U(0.6328 / 2 + x0);
                    byte[] U4 = PZTU2Data(40);
                    Com.Write(U4, 0, U4.Length);
                    Thread.Sleep(2000);
                    CCDImg.Save(picname5);
              //      fourstep5 = CCDImg.Clone(new Rectangle(0, 0, CCDImg.Width, CCDImg.Height), CCDImg.PixelFormat);

                    Thread.Sleep(500);
                    Com.Close();
                    CCDShow.Hide();                             
                                       

                }


                // 分步移动
                else
                {
                    int idxExp1 = SelectExp.DataIndex + 1;
                    string picname = "F:\\" + "Exp" + idxExp1.ToString() + "Pictures" + "\\no1"
                        + ".BMP";
                    CCDImg.Save(picname);
               

                    Thread.Sleep(2000);
                    if (null != m_objIGXStream)
                    {
                        m_objIGXStream.FlushQueue();
                    }
                    Com.Open();

                    CCDShow.Invalidate();
                    CCDShow.Refresh();

                    for (int i = 1; i < numSteps; i++)
                    {
                      
                        byte[] tempdata = PZTU2Data(i);
                        Com.Write(tempdata, 0, tempdata.Length);
                        Thread.Sleep(1000);
                        CCDShow.Invalidate();
                        CCDShow.Refresh();
                        if (null != m_objIGXStream)
                        {
                            m_objIGXStream.FlushQueue();
                        }
                    
                    }

                    string picname2 = "F:\\" + "Exp" + idxExp1.ToString() + "Pictures" + "\\no2"
             + ".BMP";
                   
                    byte[] U1 = PZTU2Data(10);
                    Com.Write(U1, 0, U1.Length);
                    Thread.Sleep(2000);
                    CCDImg.Save(picname2);
            //        fourstep2 = CCDImg.Clone(new Rectangle(0, 0, CCDImg.Width, CCDImg.Height), CCDImg.PixelFormat);
                    Thread.Sleep(500);

                    if (null != m_objIGXStream)
                    {
                        m_objIGXStream.FlushQueue();
                    }

                    CCDShow.Invalidate();
                    CCDShow.Refresh();



                    for (int i = 1; i < numSteps; i++)
                    {
                       
                        byte[] tempdata = PZTU2Data(10+i);
                        Com.Write(tempdata, 0, tempdata.Length);
                        Thread.Sleep(1000);
                        CCDShow.Invalidate();
                        CCDShow.Refresh();
                        if (null != m_objIGXStream)
                        {
                            m_objIGXStream.FlushQueue();
                        }
                 
                    }


                    string picname3 = "F:\\" + "Exp" + 1.ToString() + "Pictures" + "\\no3"
             + ".BMP";
                    double l4u = PZTX2U(0.6328 / 4);
                    byte[] U2 = PZTU2Data(20);
                    Com.Write(U2, 0, U2.Length);
                    Thread.Sleep(2000);
                    CCDImg.Save(picname3);
           //         fourstep3 = CCDImg.Clone(new Rectangle(0, 0, CCDImg.Width, CCDImg.Height), CCDImg.PixelFormat);

                    Thread.Sleep(500);
                    if (null != m_objIGXStream)
                    {
                        m_objIGXStream.FlushQueue();
                    }

                    CCDShow.Invalidate();
                    CCDShow.Refresh();


                    for (int i = 1; i < numSteps; i++)
                    {
                      
                        byte[] tempdata = PZTU2Data(20+i);
                        Com.Write(tempdata, 0, tempdata.Length);
                        Thread.Sleep(1000);
                        CCDShow.Invalidate();
                        CCDShow.Refresh();
                        if (null != m_objIGXStream)
                        {
                            m_objIGXStream.FlushQueue();
                        }
                      
                    }


                    string picname4 = "F:\\" + "Exp" + idxExp1.ToString() + "Pictures" + "\\no4"
            + ".BMP";
                   
                    byte[] U3 = PZTU2Data(30);
                    Com.Write(U3, 0, U3.Length);
                    Thread.Sleep(2000);
                    CCDImg.Save(picname4);
          //          fourstep4 = CCDImg.Clone(new Rectangle(0, 0, CCDImg.Width, CCDImg.Height), CCDImg.PixelFormat);

                    Thread.Sleep(500);
                    if (null != m_objIGXStream)
                    {
                        m_objIGXStream.FlushQueue();
                    }

                    CCDShow.Invalidate();
                    CCDShow.Refresh();


                    for (int i = 1; i < numSteps; i++)
                    {
                        
                        byte[] tempdata = PZTU2Data(30+i);
                        Com.Write(tempdata, 0, tempdata.Length);
                        Thread.Sleep(1000);
                        CCDShow.Invalidate();
                        CCDShow.Refresh();
                        if (null != m_objIGXStream)
                        {
                            m_objIGXStream.FlushQueue();
                        }
                     
                    }


                    string picname5 = "F:\\" + "Exp" + idxExp1.ToString() + "Pictures" + "\\no5"
              + ".BMP";
                
                    byte[] U4 = PZTU2Data(40);
                    Com.Write(U4, 0, U4.Length);
                    Thread.Sleep(2000);
                    CCDImg.Save(picname5);
         //           fourstep5 = CCDImg.Clone(new Rectangle(0, 0, CCDImg.Width, CCDImg.Height), CCDImg.PixelFormat);
                    Thread.Sleep(500);

                    CCDShow.Invalidate();
                    CCDShow.Refresh();


                   
                    CCDShow.Hide();
                  
                    Com.Close();
                }
                
               // Thread.Sleep(5000);
                
            }*/
            //ZYH ADD
         
            panel1.Visible = true;
            panel2.Visible = true;
            panel1.Enabled = true;
            panel2.Enabled = true;
            panelFlag = false;
            //zjp
            CCDShow.Hide();
            images = new List<Bitmap>();
            ArrayList list1 = new ArrayList();
            //
            int idxExp;
            if (SelectExp.DataIndex == -1)
            {
                idxExp = 1;
            }
            else
            {
                idxExp = SelectExp.DataIndex + 1;
            }

            //
            string unwrapPicPath = "F:\\" + "Exp" + idxExp.ToString() + "Pictures";
            DirectoryInfo direc = new DirectoryInfo(unwrapPicPath);
            int num = 0;
            foreach (FileInfo files in direc.GetFiles("*.BMP"))
            {
                {
                    Bitmap image = new Bitmap(files.FullName);
                    images.Add(image);
                    num++;
                }
            }
            /******************************************/
            width = images[1].Width;
            height = images[1].Height;
            FirPic.Image = images[1];
            bmpI = images[1];
            List<BitmapInfo> Images = new List<BitmapInfo>();
            for (int i = 0; i < 5; i++)
            {
                BitmapInfo bi = GetImagePixel(images[i]);
                Images.Add(bi);
            }
            IntPtr M;
            M = unwrap1M(Images[1].Result, Images[2].Result, Images[3].Result, Images[4].Result, width, height, Images[0].Step, 1, 0.1F);
            Bitmap Mpic = new Bitmap(width, height, Images[0].Step, System.Drawing.Imaging.PixelFormat.Format8bppIndexed, M);
            Mpic.Palette = CvToolbox.GrayscalePalette;
            SecPic.Image = Mpic;
            bmpM = Mpic;
            FourStepsPic panel = new FourStepsPic();
            panel.StartPosition = FormStartPosition.Manual;
            panel.Show();
            SourcePicL.Visible = true;
            ML.Visible = true;
            Unwrap2L.Visible = true;
            Unwrap1L.Visible = true;


            Draw3D.Visible = true;
            CreateCurve.Visible = true;
            SaveData.Visible = true;
            button7.Enabled = true;
            EdgeDetect.Enabled = true;
            button9.Enabled = true;
            barButtonItem13.Enabled = true;
            CheckGray.Enabled = true;
            comboBox3.Enabled = true;
            ERODE.Enabled = true;
            dilate.Enabled = true;
            barEditItem3.Enabled = true;
        }
        public static List<Bitmap> images;

        void Z()//yueyue
        {
            while (c == 0)
                Thread.Sleep(100);
            splashScreenManager3.CloseWaitForm();
        }
        private void Firunwrap_Click(object sender, ItemClickEventArgs e)
        {

        }
        
        public int noContinuous = 1;
        private void Comfirm_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (PZTOROpen)
            {
                Com.Open();
                byte[] readdata = new byte[]
                {
                0xAA, 0x00 , 0x81 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00
                , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00
                , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x2B
                };
                Com.Write(readdata, 0, readdata.Length);
                byte[] resRead = new byte[26];
                Thread.Sleep(500);
                Com.Read(resRead, 0, resRead.Length);
                string Vol;
                Thread.Sleep(500);
                Vol = BitConverter.ToString(resRead, 8, 1) + BitConverter.ToString(resRead, 7, 1)
                    + BitConverter.ToString(resRead, 6, 1) + BitConverter.ToString(resRead, 5, 1);
                int startVoltage = int.Parse(Vol, System.Globalization.NumberStyles.AllowHexSpecifier);
          //    int startVoltage = 0;
                //PZTPos.Text = startVoltage.ToString();
              
                List<double> PZTStep = new List<double>();
                int idxpic = 0;
                for (int i = 0; i < NumPZTstep; i++)
                {
                    string path = "F:\\CCD_Continuous\\" + "Times" + noContinuous.ToString()
                       + "no" + idxpic.ToString() + ".BMP";
                    double temppos = startVoltage + (i + 1) * LengthPZTstep;
                   // double tempvol = PZTX2U(temppos);
                    byte[] tempdata = PZTU2Data(temppos);
                    Com.Write(tempdata, 0, tempdata.Length);
                    Thread.Sleep(3000);
                    if (CCDContinuous.Checked)
                    {
                        CCDImg.Save(path);
                    }
                    Thread.Sleep(500);
                    idxpic++;
                   
                }
                noContinuous++;
                Com.Close();
            }
            else
            {
                MessageBox.Show("请打开PZT");
            }
        }

        private double PZTX2U(double x)
        {
            double u = 0.7754 * x * x * x * x * x
            - 9.3674 * x * x * x * x + 42.7487 * x * x * x - 91.9911 * x * x + 121.9546 * x + 1.2276;
            return u;
        }
        private double PZTU2X(double u)
        {
            double x = 0.0002 * u * u + 0.0023 * u + 0.0168;
            return x;
        }
        private byte[] PZTU2Data(double u)
        {

            int U = (int)u;
            U = U * 1000;
            string xU = U.ToString("X8");
            byte[] bxU = new byte[xU.Length / 2];
            for (int i = 0; i < xU.Length; i += 2)
            {
                bxU[i / 2] = Convert.ToByte(xU.Substring(i, 2), 16);
            }
            byte[] data = new byte[26];
            data[0] = 0xAA;
            data[1] = 0x00;
            data[2] = 0x80;
            data[3] = 0x90;
            data[4] = 0x01;
            data[5] = 0x40;
            data[6] = 0x9c;
            data[7] = 0x00;
            data[8] = 0x00;
            data[9] = 0x40;
            data[10] = 0x06;
            data[11] = bxU[3];
            data[12] = bxU[2];
            data[13] = bxU[1];
            data[14] = bxU[0];
            data[15] = 0x00;
            data[16] = 0x00; data[17] = 0x00; data[18] = 0x00; data[19] = 0x00; data[20] = 0x00;
            data[21] = 0x00; data[22] = 0x00; data[23] = 0x00; data[24] = 0x00;
            int tdata26 = 0;
            for (int i = 0; i < 25; i++)
            {
                tdata26 += data[i];
            }
            string sdata26 = tdata26.ToString("X5");
            byte[] xdata26 = new byte[1];
            xdata26[0] = Convert.ToByte(sdata26.Substring(sdata26.Length - 2, 2), 16);
            data[25] = xdata26[0];
            return data;

        }

        [DllImport("WaveFilter.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WaveFilter(byte[] image, int width, int height, int step, int method, int coreValue, out int gstep);

        private void button7_ItemClick(object sender, ItemClickEventArgs e)
        {
            int CoreValue = Convert.ToInt32(CoreEdit.EditValue);
            this.trackbar1.EditValue = 0;
            this.trackbar2.EditValue = 0;
            if (bmp == null)
            {
                XtraMessageBox.Show("请选择需处理的图像", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            BitmapInfo bmpt = GetImagePixel(bmp);
            int stride;
            int gstep;
            stride = bmpt.Step;
            int method;
            String Value = comboBox3.DataIndex.ToString();
            if (Value == "0")
            {
                method = 0;
            }
            else if (Value == "1")
            {
                method = 1;
            }
            else
            {
                method = 2;
            }
            if (Value == "-1")
            {
                MessageBox.Show("请选择滤波方式");
                return;
            }
            if((method==1||method==2)&&(CoreValue%2==0))
            {
                MessageBox.Show("中值滤波和高斯滤波核尺寸只能为奇数");
                return;
            }
            IntPtr OutputImage = WaveFilter(bmpt.Result, width, height, stride, method, CoreValue, out gstep);
            
            bmpFilt = new Bitmap(bmp.Width, bmp.Height, gstep,
                 System.Drawing.Imaging.PixelFormat.Format8bppIndexed, OutputImage);
            bmpFilt.Palette = CvToolbox.GrayscalePalette;    
            picShow[PictureNum].Image = bmpFilt;
        }

        //**// ZYH Add
        //eorde
        [DllImport("ErodeProc.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr erodeProc(byte[] image, int width, int height, int step, int ksize, out int gstep);

        private void trackBar1_ItemClick(object sender, ItemClickEventArgs e)
        {
            //判断是否选择了图片
           
        }

        //**// ZYH Add
        //dilate
        [DllImport("DilateProc.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr dilateProc(byte[] image, int width, int height, int step, int ksize, out int gstep);

        private void trackBar2_ItemClick(object sender, ItemClickEventArgs e)
        {
            //判断是否选择了图片
        
        }

        [DllImport("CannyDetect.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr cannyDetect(byte[] image, int width, int height, int step, int thresh, out int gstep);

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            //判断是否选择了图片
            if (bmp == null)
            {
                XtraMessageBox.Show("请选择需处理的图像", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //  this.trackBar3.Value = 0;
                return;
            }

          // this.trackBar3.Maximum = 10;
         //   this.trackBar3.Minimum = 0;
            int thresh = 5 ;

            Bitmap temp_bmp;

            if (dilateFlag || erodeFlag)
            {
                temp_bmp = bmpToCanny;
            }
            else if (bmpFilt == null)
            {
                temp_bmp = bmp;
            }
            else
            {
                temp_bmp = bmpFilt;
            }

            BitmapInfo bmpt = GetImagePixel(temp_bmp);

            int step = bmpt.Step;
            int gstep;
            if (thresh > 0)
            {
                IntPtr OutputImage = cannyDetect(bmpt.Result, width, height, step, thresh, out gstep);
                bmpCanny = new Bitmap(bmp.Width, bmp.Height, gstep,
                     System.Drawing.Imaging.PixelFormat.Format8bppIndexed, OutputImage);
                bmpCanny.Palette = CvToolbox.GrayscalePalette;
                //pictureBox1.Image = bmpCanny;
            }
            else
            {
                bmpCanny = new Bitmap(temp_bmp);
                //pictureBox1.Image = bmpCanny;
            }
            picShow[PictureNum].Image = bmpCanny;
        }

        [DllImport("CircleFit.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr circleFit(byte[] image, int width, int height, int step,
            out int gstep, out IntPtr Mask, out double CenterFit_x, out double CenterFit_y, out double RadiusFit);

        [DllImport("MaskProc_new.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr maskProc_new(byte[] image, int width, int height, int step,
            IntPtr Mask, out int gstep);

        //拖动鼠标画圆事件
        private bool CatchFlag = false;            //鼠标点击事件的flag
        private bool CatchStart = false;           //画图事件的flag
  
        private Point CenterPoint = Point.Empty;   //中心坐标
        private Point EndPoint = Point.Empty;      //结束坐标
        //private Bitmap originBmp;
        private Graphics BmpG; //画图
        private double CenterFit_x, CenterFit_y, RadiusFit; //用于解调的孔径区域参数

        public static int wwidth, wheight, wsteps;
        public static Point[] points = new Point[2];
        public int curvedowntimes = 0;

        private void FirPic_DoubleClick(object sender, EventArgs e)
        {
            if (orfit && or3d)
            {
                FirPic.BringToFront();
                FirPic.Refresh();

                DrawMat(this.FirPic.Handle, 0, 0, rUnwrap, wwidth, wheight, wsteps);
                unwrap3DFinish = true;
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (panelFlag)
            {
                return;
            }
            panel2.BackColor = SystemColors.GradientInactiveCaption;
            panel1.BackColor = SystemColors.Highlight;
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            panel2.BorderStyle = System.Windows.Forms.BorderStyle.None;

            PictureNum = 0;
            bmp = bmpI;

            //参数初始化
            erodeFlag = false;
            dilateFlag = false;
            bmpFilt = null;
            bmpCanny = null;
            //bmpCircle = null;
            bmpToCanny = null;

            //待添加 HistFlag
            if (HistFlag)
            {
                if (bmpEqualizedIn1 != null) //按下均衡化按钮
                {
                    bmpToForm2 = bmpEqualizedIn1;
                    CheckGray.Checked = true;
                    //button1.Caption = "恢复原灰度";
                    ButtonFlag = true;
                }
                else //未按
                {
                    bmpToForm2 = bmpI;
                    //button1.Caption = "灰度均衡化";
                    CheckGray.Checked = false;
                    ButtonFlag = false;
                }

                HistEvent(bmpToForm2, PictureNum);

            }
        }

        public int startfirx, startfiry;
        private void FirPic_MouseDown(object sender, MouseEventArgs e)
        {
            if (orfit && or3d)
            {
                Firdown = true;
                startfirx = e.X;
                startfiry = e.Y;
            }
        }

        public int shifty = 0, shiftx = 0;
        private void FirPic_MouseMove(object sender, MouseEventArgs e)
        {
            if (Firdown && orfit && or3d)
            {

                int firlengthx = e.X - startfirx;
                int firlengthy = e.Y - startfiry;
                DrawMat(this.FirPic.Handle, shiftx - firlengthx, shifty + firlengthy, rUnwrap, wwidth, wheight, wsteps);

            }
        }


        private void FirPic_MouseUp(object sender, MouseEventArgs e)
        {
            if (orfit && or3d)
            {
                Firdown = false;
                int firlengthx = e.X - startfirx;
                int firlengthy = e.Y - startfiry;
                shiftx -= firlengthx;
                shifty += firlengthy;
            }
        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {

        }

        private void SecPic_Click(object sender, EventArgs e)
        {

        }

        private void SecPic_DoubleClick(object sender, EventArgs e)
        {
            if (orfit && or3d)
            {
                DrawMat(this.SecPic.Handle, 0, 0, WoForShow, wwidth, wheight, wsteps);
                unwrap3DFinish = true;
            }
        }

        private void Form2_MouseClick(object sender, MouseEventArgs e)
        {
            if (panelFlag)
            {
                return;
            }
            panel1.BackColor = SystemColors.GradientInactiveCaption;
            panel2.BackColor = SystemColors.Highlight;
            panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.None;

            PictureNum = 1;
            bmp = bmpM;

            //参数初始化
            erodeFlag = false;
            dilateFlag = false;
            bmpFilt = null;
            //bmpCanny = null;
            //bmpCircle = null;
            bmpToCanny = null;

            if (!CatchFlag)
            {
                bmpCanny = null;
            }

            //待添加 CatchFlag HistFlag
            Form2 form2 = new Form2();
            if (form2 == null)
            {
                return;
            }

            if (HistFlag && bmpCircle != null)
            {
                if (bmpEqualizedIn2 != null)
                {
                    bmpToForm2 = bmpEqualizedIn2;
                    //button1.Caption = "恢复原灰度";
                    CheckGray.Checked = true;
                    ButtonFlag = true;
                }
                else
                {
                    bmpToForm2 = bmpCircle;
                    //button1.Caption = "灰度均衡化";
                    CheckGray.Checked = false;
                    ButtonFlag = false;
                }
                HistEvent(bmpToForm2, PictureNum);
            }
            else if (HistFlag && bmpCircle == null)
            {
                MessageBox.Show("请完成干涉图孔径确定", "错误");
                return;
            }
        }

        public bool Secdown = false;
        public int secstartfirx, secstartfiry, secshiftx, secshifty;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (CatchFlag && !orfit)
            {
                CenterPoint.X = e.X;
                CenterPoint.Y = e.Y;
                CatchStart = true;
            }
            if (orfit && or3d)
            {
                Secdown = true;
                secstartfirx = e.X;
                secstartfiry = e.Y;
            }
        }

        private int Radius;
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (CatchStart && !orfit)
            {
                BmpG = SecPic.CreateGraphics();
                Pen p = new Pen(Color.Blue, 2);

                Radius = (int)Math.Sqrt((e.X - CenterPoint.X) * (e.X - CenterPoint.X)
                    + (e.Y - CenterPoint.Y) * (e.Y - CenterPoint.Y));
                Rectangle rect = new Rectangle(CenterPoint.X - Radius, CenterPoint.Y - Radius,
                    2 * Radius, 2 * Radius);

                //rect.Height += 1;
                //rect.Width += 1;
                SecPic.Invalidate();
                SecPic.Update();
                BmpG.DrawEllipse(p, rect);
                p.Dispose();
                BmpG.Dispose();
            }
            if (Secdown && orfit && or3d)
            {

                int seclengthx = e.X - secstartfirx;
                int seclengthy = e.Y - secstartfiry;
                DrawMat(this.SecPic.Handle, secshiftx - seclengthx, secshifty + seclengthy, WoForShow, wwidth, wheight, wsteps);

            }
        }
        double RadiusFit2;
        [DllImport("MaskProc.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr maskProc(byte[] image, int width, int height, int step,
           int centerx, int centery, int radius, int radius2, out IntPtr Mask, out int gstep);
        private int Radius2 = 0;
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (CatchStart && !orfit)
            {
                BmpG = SecPic.CreateGraphics();
                Pen p = new Pen(Color.Red, 2);

                if (SelectExp.ItemIndex != 3)
                {
                    Rectangle rect = new Rectangle(CenterPoint.X - Radius, CenterPoint.Y - Radius,
                        2 * Radius, 2 * Radius);
                    BmpG.DrawEllipse(p, rect);
                    p.Dispose();
                    BmpG.Dispose();
                }
                else
                {
                    Radius2 = (int)(Radius / 0.65);
                    Rectangle rect1 = new Rectangle(CenterPoint.X - Radius, CenterPoint.Y - Radius,
                        2 * Radius, 2 * Radius);
                    Rectangle rect2 = new Rectangle(CenterPoint.X - Radius2, CenterPoint.Y - Radius2,
                        2 * Radius2, 2 * Radius2);
                    BmpG.DrawEllipse(p, rect1);
                    BmpG.DrawEllipse(p, rect2);
                    p.Dispose();
                    BmpG.Dispose();
                }
                //CatchStart = false;
                //CatchFlag = false;

                //显示孔径
                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                DialogResult dr = MessageBox.Show("是否显示已确定的孔径？", "手动孔径确定", messButton);
                if (dr == DialogResult.OK)
                {
                    int centerx = (int)(width * CenterPoint.X / SecPic.Width);
                    int centery = (int)(height * CenterPoint.Y / SecPic.Height);
                    int radius = (int) (width * Radius / SecPic.Width);
                    int radius2 = (int)(width * Radius2 / SecPic.Width);
                    BitmapInfo bmpt = GetImagePixel(bmpI);
                    int step = bmpt.Step;
                    int gstep;
                    IntPtr OutputImage = maskProc(bmpt.Result, width, height, step,
                        centerx, centery, radius, radius2, out Mask, out gstep);
                    bmpCircle = new Bitmap(bmp.Width, bmp.Height, gstep,
                        System.Drawing.Imaging.PixelFormat.Format8bppIndexed, OutputImage);
                    bmpCircle.Palette = CvToolbox.GrayscalePalette;
                    SecPic.Image = bmpCircle;

                    MaskFlag = true;
                    CatchFlag = false;
                    CatchStart = false;

                    //用于位相解调的孔径参数（double型）
                    CenterFit_x = centerx;
                    CenterFit_y = centery;
                    RadiusFit = radius;
                    RadiusFit2 = radius2;
                }
                else
                {
                    SecPic.Image = bmpM;
                    //SecPic.Invalidate();
                    //SecPic.Refresh();
                    CatchStart = false;
                    CatchFlag = false;
                }
            }

            if (orfit && or3d)
            {
                Secdown = false;
                int seclengthx = e.X - secstartfirx;
                int seclengthy = e.Y - secstartfiry;
                secshiftx -= seclengthx;
                secshifty += seclengthy;
            }

            SecPic.Cursor = Cursors.Arrow;
        }

        private void FirPic_Click(object sender, EventArgs e)
        {

        }
    
        private void button9_ItemClick(object sender, ItemClickEventArgs e)
        {
            //pictureBox1.Image = bmp;

            if (PictureNum != 1)
            {
                XtraMessageBox.Show("仅能对调制度图操作", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            
            //处理当前图片进行画圆
            if (!Convert.ToBoolean(barEditItem3.EditValue))
            {
                XtraMessageBox.Show("按住鼠标左键", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                CatchFlag = true;
                //checkItem2.Checke = false;
                SecPic.Cursor = Cursors.Cross;
            }
            else if (Convert.ToBoolean(barEditItem3.EditValue))
            {
                if (bmpCanny == null)
                {
                    XtraMessageBox.Show("请完成边缘提取", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);


                    return;
                }

                CatchFlag = false;
                //checkItem1.Checked = false;
                BitmapInfo bmpt = GetImagePixel(bmpCanny);
                int step = bmpt.Step;
                int gstep;

                //CenterFit_x,CenterFit_y,RadiusFit是函数输出的孔径参数
                IntPtr OutputImage = circleFit(bmpt.Result, width, height, step, out gstep, out Mask, out CenterFit_x, out CenterFit_y, out RadiusFit);
                Bitmap img = new Bitmap(bmp.Width, bmp.Height, gstep,
                     System.Drawing.Imaging.PixelFormat.Format8bppIndexed, OutputImage);
                img.Palette = CvToolbox.GrayscalePalette;
                SecPic.Image = img;

                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                DialogResult dr = MessageBox.Show("是否显示已确定的孔径？", "自动孔径确定", messButton);
                if (dr == DialogResult.OK)
                {
                    BitmapInfo bmpt1 = GetImagePixel(bmpI);
                    IntPtr OutputImage1 = maskProc_new(bmpt1.Result, width, height, step, Mask, out gstep);
                    bmpCircle = new Bitmap(bmp.Width, bmp.Height, gstep,
                        System.Drawing.Imaging.PixelFormat.Format8bppIndexed, OutputImage1);
                    bmpCircle.Palette = CvToolbox.GrayscalePalette;
                    SecPic.Image = bmpCircle;
                    MaskFlag = true;
                }
                else
                {
                    SecPic.Image = bmpCanny;
                }
            }

            Firunwrap.Enabled = true;
            Secunwrap.Enabled = true;

        }
        //解包裹
        public static float xc, yc, R, scalev, unwrap1scalev;
        public static int pichgap, picwgap, picwidth, picheight, unwrap1width, unwrap1height, unwrap1hgap, unwrap1wgap, unwrap1ResStep;
        public Graphics ginimg, unwrap1ginimg;
        public bool unwrap = false;
        public Bitmap Unwrap1ResPic;
        public static Bitmap bmpFit;
        public float Wunwrap_Max;  //相位最大值
        public float Wunwrap_Min;  //相位最小值
        public int a = 0;
        //灰度转伪彩色
        [DllImport("ColorBar.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Colorbar(IntPtr image, int width, int height, int step, out int stepC);
        public Bitmap img;
        void X()//yueyue
        {
            while (a == 0)

                Thread.Sleep(100);
            splashScreenManager4.CloseWaitForm();
        }
        
        private void Secunwrap_Click(object sender, ItemClickEventArgs e)
        {
            FourthPic.Visible = true;
            splashScreenManager4.ShowWaitForm();//yueyue
            Thread t = new Thread(new ThreadStart(X));
            t.Start();
            if (SelectExp.ItemIndex != 3)
            {
                rUnwrap = unwrap2(W1, width, height, out W, out wwidth, out wheight, out wsteps, (float)CenterFit_x, (float)CenterFit_y, (float)RadiusFit, out Wunwrap_Max, out Wunwrap_Min,0);
            }
            else
            {
                rUnwrap = unwrap2(W1, width, height, out W, out wwidth, out wheight, out wsteps, (float)CenterFit_x, (float)CenterFit_y, (float)RadiusFit2, out Wunwrap_Max, out Wunwrap_Min, (float)RadiusFit);
            }
            a = 1;
            //img = new Bitmap(wwidth, wheight, wsteps,
            //   System.Drawing.Imaging.PixelFormat.Format8bppIndexed, rUnwrap);
            //img.Palette = CvToolbox.GrayscalePalette;

            //FourthPic.Image = img;

            //伪彩色图
            int stepc;
            IntPtr bmpColor = Colorbar(rUnwrap, wwidth, wheight, wsteps, out stepc);
            img = new Bitmap(wwidth, wheight, stepc,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb, bmpColor);

            Colormap4.Visible = true;
            Colormap4.Image = Image.FromFile(Application.StartupPath + "\\colorscale_jet2.jpg");
            Fourth_lab1.Visible = true; Fourth_lab1.Text = Math.Round(Wunwrap_Max, 3).ToString();
            Fourth_lab2.Visible = true; Fourth_lab2.Text = Math.Round(Wunwrap_Min, 3).ToString();

            FourthPic.Image = img;


            double k = wwidth / (double)wheight;
            if (FourthPic.Height * k > FourthPic.Width)
            {
                picheight = (int)(FourthPic.Width / k);
                picwidth = FourthPic.Width;
                pichgap = (int)((FourthPic.Height - picheight) / 2.0);
                picwgap = 0;
            }
            else
            {
                picwidth = (int)(FourthPic.Height * k);
                picheight = FourthPic.Height;
                picwgap = (int)((FourthPic.Width - picwidth) / 2.0);
                pichgap = 0;
            }
            scalev = wwidth / (float)picwidth;
            unwrap = true;
            ginimg = FourthPic.CreateGraphics();
            Button10.Enabled = true;
            PV.Enabled = true;
            RMS.Enabled = true;
        }

        private void 实验软件_Load(object sender, EventArgs e)
        {  
            
            
        }

        private void trackBar3_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void Remove_Click(object sender, ItemClickEventArgs e)
        {

        }

        public double newwidth;
        public static IntPtr UnwrapRes1;
        public static Bitmap BytesToBitmap(byte[] Bytes)
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(Bytes);
                return new Bitmap((Image)new Bitmap(stream));
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            finally
            {
                stream.Close();
            }
        }

        private void radioButton1_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void radioButton2_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void barButtonItem13_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (bmp == null)
            {
                XtraMessageBox.Show("请选择需处理的图像", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (PictureNum == 1 && !MaskFlag)
            {
                XtraMessageBox.Show("请完成干涉图孔径提取", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
           
                return;
            }

            if (bmpEqualized != null)
            {
                bmpToForm2 = bmpEqualized;
            }
            else
            {
                bmpToForm2 = bmpI; //干涉图原图
                                   //传递给灰度直方图显示窗体
            }

            //添加委托
            Form2 form2 = new Form2();

            HistEvent += new HistEventHandler(form2.ShowPicture);
            if (this.HistEvent != null)
            {
                this.HistEvent(bmpToForm2, PictureNum);
            }

            form2.SendEvent += new SendEventHandler(ShowContrast);

            Point p = new Point(0, 0);
            p = SecPic.PointToScreen(p);
            Point curvepos = new Point(p.X + form2.Width + 20, p.Y - 5);
            form2.StartPosition = FormStartPosition.Manual;
            form2.Location = curvepos;
            
            form2.TopMost = true;
            form2.Show();
            HistFlag = true;
        }

        [DllImport("HistShow.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr histShow(byte[] image, int width,
                 int height, int step, out int gstep);
        [DllImport("HistShow_Mask", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr histShow_Mask(byte[] image, int width,
            int height, int step, IntPtr Mask, out int gstep);
        [DllImport("ContractProc.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr contractProc(byte[] image, int width, int height, int step, float thresh, out int gstep);
        [DllImport("Equalized.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr equalized(byte[] image, int width, int height, int step, out int gstep);
        //数据传递
        public static Bitmap bmpToForm2;
        public static bool HistFlag = false;
        public static Bitmap bmpEqualized;
        public static Bitmap bmpEqualizedIn1, bmpEqualizedIn2;
        public static bool ButtonFlag = false;

        private void CheckGray_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            if (bmp == null)
            {
                XtraMessageBox.Show("请选择需处理的图像", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          
                return;
            }
            else if (PictureNum == 1 && bmpCircle == null)
            {
                XtraMessageBox.Show("请完成干涉图孔径确定", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
               
                return;
            }

            //Bitmap bmpEqualized;

            if (!CheckGray.Checked) //未按
            {
                //button1.Caption = "恢复原灰度";
                bmpEqualized = null;
                //bmpEqualizedIn1 = null;
                //bmpEqualizedIn2 = null;

                if (PictureNum == 0)
                {
                    bmpToForm2 = bmpI;
                    bmpEqualizedIn1 = null;
                }
                else
                {
                    bmpToForm2 = bmpCircle;
                    bmpEqualizedIn2 = null;
                }

                picShow[PictureNum].Image = bmpToForm2;
                ButtonFlag = false;
            }
            else  //按下了按钮
            {
                //button1.Caption = "灰度均衡化";
                Bitmap temp_bmp;
                if (PictureNum == 0)
                    temp_bmp = bmpI;
                else
                    temp_bmp = bmpCircle;

                BitmapInfo bmpt = GetImagePixel(temp_bmp);

                int step = bmpt.Step;
                int gstep;

                //Show Picture
                IntPtr OutputImage = equalized(bmpt.Result, width, height, step, out gstep);
                bmpEqualized = new Bitmap(bmp.Width, bmp.Height, gstep,
                     System.Drawing.Imaging.PixelFormat.Format8bppIndexed, OutputImage);
                bmpEqualized.Palette = CvToolbox.GrayscalePalette;

                if (PictureNum == 0)
                {
                    bmpEqualizedIn1 = bmpEqualized;
                    bmpToForm2 = bmpEqualizedIn1;
                }
                else
                {
                    bmpEqualizedIn2 = bmpEqualized;
                    bmpToForm2 = bmpEqualizedIn2;
                }

                picShow[PictureNum].Image = bmpEqualized;
                ButtonFlag = true;
            }

            //委托
            if (HistEvent != null)
            {
                HistEvent(bmpToForm2, PictureNum);
            }
        }

        //解调
        [DllImport("FirUnwrapDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr unwrap1(byte[] img1, byte[] img2, byte[] img3, byte[] img4, int width,
            int height, int step, int channels, out IntPtr W1, double xc, double yc, double R, out int nwidth, out int nheight, out int steps,double r);
        [DllImport("FirUnwrapDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr unwrap1M(byte[] img1, byte[] img2, byte[] img3, byte[] img4, int width,
      int height, int step, int channels, float thresh);
        [DllImport("SecUnwrapDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr unwrap2(IntPtr img1, int width, int height,
            out IntPtr W, out int outwidth, out int outheight, out int steps, float xc, float yc, float R, out float Max_Value, out float Min_Value,float rmin);
        public bool panelFlag = false;
        public static int newheight1, newwidth1, steps;
        private void Firunwrap_ItemClick(object sender, ItemClickEventArgs e)
        {
            WhichPic.Visible = false;
            //ZYH Add
            //取消panel颜色变化
            panel1.BackColor = SystemColors.GradientInactiveCaption;
            panel2.BackColor = SystemColors.GradientInactiveCaption;
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            panel2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            panelFlag = true;

            ThirdPic.Visible = true;
            //FourthPic.Visible = true;

            CCDShow.Hide();
            List<Bitmap> images = new List<Bitmap>();
            ArrayList list1 = new ArrayList();
            int idxExp;
            if (SelectExp.DataIndex == -1)
            {
                idxExp = 1;
            }
            else
            {
                idxExp = SelectExp.DataIndex + 1;
            }
            string unwrapPicPath = "F:\\" + "Exp" + idxExp.ToString() + "Pictures";
            DirectoryInfo direc = new DirectoryInfo(unwrapPicPath);
            int num = 0;
            foreach (FileInfo files in direc.GetFiles("*.BMP"))
            {
                {
                    Bitmap image = new Bitmap(files.FullName);
                    images.Add(image);
                    num++;
                }
            }
            width = images[1].Width;
            height = images[1].Height;
            List<BitmapInfo> Images = new List<BitmapInfo>();
            for (int i = 0; i < 5; i++)
            {
                BitmapInfo bi = GetImagePixel(images[i]);
                Images.Add(bi);
            }

            //待修改
            if (SelectExp.ItemIndex != 3)
            {
                UnwrapRes1 = unwrap1(Images[1].Result, Images[2].Result, Images[3].Result, Images[4].Result,
                             width, height, Images[0].Step, 1, out W1, CenterFit_x, CenterFit_y, RadiusFit, out newwidth1, out newheight1, out steps,0);
            }
            else
            {
                UnwrapRes1 = unwrap1(Images[1].Result, Images[2].Result, Images[3].Result, Images[4].Result,
                           width, height, Images[0].Step, 1, out W1, CenterFit_x, CenterFit_y, RadiusFit2, out newwidth1, out newheight1, out steps, RadiusFit);
            }
            unwrap1ResStep = Images[0].Step;

            Unwrap1ResPic = new Bitmap(newwidth1, newheight1, steps,
                 System.Drawing.Imaging.PixelFormat.Format8bppIndexed, UnwrapRes1);
            Unwrap1ResPic.Palette = CvToolbox.GrayscalePalette;

            ThirdPic.Image = Unwrap1ResPic;

            //画线
            double k = newwidth1 / (double)newheight1;
            if (ThirdPic.Height * k > ThirdPic.Width)
            {
                unwrap1height = (int)(ThirdPic.Width / k);
                unwrap1width = ThirdPic.Width;
                unwrap1hgap = (int)((ThirdPic.Height - unwrap1height) / 2.0);
                unwrap1wgap = 0;
            }
            else
            {
                unwrap1width = (int)(ThirdPic.Height * k);
                unwrap1height = ThirdPic.Height;
                unwrap1wgap = (int)((ThirdPic.Width - unwrap1width) / 2.0);
                unwrap1hgap = 0;
            }
            unwrap1scalev = newwidth1 / (float)unwrap1width;
            unwrap = true;
            unwrap1ginimg = ThirdPic.CreateGraphics();
        }

        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }
        public IntPtr W1, W;
        public static IntPtr rUnwrap;
        public static int width, height;
      
        [DllImport("3D_MATDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DrawMat(IntPtr handle, double X, double Y, IntPtr image, int width, int height, int steps);
        [DllImport("3D_MATDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drawcurve(IntPtr handle, IntPtr image, int width, int height, int x1, int y1, int x2, int y2, int steps);
        public bool or3d = false;
        private void Draw3D_Click(object sender, EventArgs e)
        {
            Draw3D.Text = "显示二维";
            //   DrawMat(this.inimg.Handle, 0, 0, rUnwrap, wwidth, wheight, wsteps);
            or3d = true;
            if (orfit)
            {
                shiftx = 0;
                shifty = 0;
                secshiftx = 0;
                secshifty = 0;
                thirdshiftx = 0;
                thirdshifty = 0;
                fourthshiftx = 0;
                fourthshifty = 0;

                if (unwrap3DFinish == true)
                {
                    //FirPic.Refresh();
                    FivePic.Image = img;
                    SixPic.Image = imgColor;
                    ThirdPic.Image = imgResidual;
                    if (imgColor2 != null)
                    {
                        FourthPic.Image = imgColor2;
                    }
                    Draw3D.Text = "生成三维";
                    or3d = false;
                    unwrap3DFinish = false;

                }
            }
            else
            {
                shiftx = 0;
                shifty = 0;
                secshiftx = 0;
                secshifty = 0;
                thirdshiftx = 0;
                thirdshifty = 0;
                fourthshiftx = 0;
                fourthshifty = 0;
                if (unwrap3DFinish == true)
                {
                    ThirdPic.Refresh();
                    ThirdPic.Image = Unwrap1ResPic;
                    FourthPic.Refresh();
                    FourthPic.Image = img;
                    Draw3D.Text = "生成三维";
                    or3d = false;
                    unwrap3DFinish = false;
                    unwrap13D = false;
                    unwrap23D = false;
                }
            }
        }

        public int startx, starty, overx, overy, lengthy, lengthx;
        public bool orcurve = false;

        private void length_EditValueChanged(object sender, EventArgs e)
        {
            LengthPZTstep = Convert.ToDouble(length.EditValue);
        }

        private void NumSteps_EditValueChanged(object sender, EventArgs e)
        {
            NumPZTstep = Convert.ToInt32(NumSteps.EditValue);
        }
        public double dGain = 1;
        private void GainSet_EditValueChanged(object sender, EventArgs e)
        {
           
            double dMin = 0.0;           //最小值
            double dMax = 0.0;           //最大值
            dGain = Convert.ToDouble(GainSet.EditValue);
            //
            if (null != m_objIGXFeatureControl)
            {
                dMin = m_objIGXFeatureControl.GetFloatFeature("Gain").GetMin();
                dMax = m_objIGXFeatureControl.GetFloatFeature("Gain").GetMax();

                //判断输入值是否在增益值的范围内
                //若输入的值大于最大值则将增益值设置成最大值
                if (dGain > dMax)
                {
                    dGain = dMax;
                }

                //若输入的值小于最小值则将增益的值设置成最小值
                if (dGain < dMin)
                {
                    dGain = dMin;
                }

                m_objIGXFeatureControl.GetFloatFeature("Gain").SetValue(dGain);
            }
            
        }
        public double dShutterValue = 40;
        private void ExposureTime_EditValueChanged(object sender, EventArgs e)
        {
            
            double dMin = 0.0;                       //最小值
            double dMax = 0.0;                       //最大值
            dShutterValue = Convert.ToDouble(ExposureTime.EditValue);
            if (null != m_objIGXFeatureControl)
            {

                dMin = m_objIGXFeatureControl.GetFloatFeature("ExposureTime").GetMin();
                dMax = m_objIGXFeatureControl.GetFloatFeature("ExposureTime").GetMax();
                //判断输入值是否在曝光时间的范围内j
                //若大于最大值则将曝光值设为最大值
                if (dShutterValue > dMax)
                {
                    dShutterValue = dMax;
                }
                //若小于最小值将曝光值设为最小值
                if (dShutterValue < dMin)
                {
                    dShutterValue = dMin;
                }
         
                m_objIGXFeatureControl.GetFloatFeature("ExposureTime").SetValue(dShutterValue);
            }
            
        }

        public bool radioButton1 = false, radioButton2 = false;

        private void checkItem2_EditValueChanged(object sender, EventArgs e)
        {
         
            CheckEdit pCheckEdit = sender as CheckEdit;
            if (pCheckEdit.Checked)
            {
                radioButton2 = true;
                radioButton1 = false;
            }
        }

        
        private void trackbar1_EditValueChanged(object sender, EventArgs e)
        {
           
           
            
            
        }

        private void trackbar2_EditValueChanged(object sender, EventArgs e)
        {
           
        }

        private void trackbar1_ItemClick_1(object sender, ItemClickEventArgs e)
        {

        }

        private void SelectExp_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (SelectExp.DataIndex == 0)
            {
                SelectExp.Caption = "平面镜面形检测";
            }
            if (SelectExp.DataIndex == 1)
            {
                SelectExp.Caption = "球面镜面形检测";
            }
            if (SelectExp.DataIndex == 2)
            {
                SelectExp.Caption = "非球面镜面形检测";
            }
            if (SelectExp.DataIndex == 3)
            {
                SelectExp.Caption = "非球面无像差点法检测";
            }
        }

        private void comboBox3_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (comboBox3.DataIndex == 0)
            {
                comboBox3.Caption = "均值滤波";
            }
            if (comboBox3.DataIndex == 1)
            {
                comboBox3.Caption = "中值滤波";
            }
            if (comboBox3.DataIndex == 2)
            {
                comboBox3.Caption = "高斯滤波";
            }
        }

        private void barEditItem3_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            int SelectNum;
            for (int i = 0; i < 37; i++)
            {
                cNum[i] = 0;
            }
            for (int i = 0; i < listView1.CheckedItems.Count; i++)
            {
                if (this.listView1.CheckedItems[i].Checked)
                {
                    SelectNum = this.listView1.CheckedItems[i].Index;
                    cNum[SelectNum] = 1;
                }
            }
        }

        private void barListItem4_ListItemClick(object sender, ListItemClickEventArgs e)
        {
            ZernikeInterfere.Visible = true;
            zeridx = ZernikeToInterfer.DataIndex + 1;
            //
           // ZernikeToInterfer.

            string filepath = "F:\\zernike像差干涉图\\" + 实验软件.zeridx.ToString() + ".jpg";
            Bitmap zerpic = new Bitmap(filepath);
            ZernikeInterfere.Image = zerpic;

            ZernikeToInterfer.Caption = ZernikeToInterfer.Strings[zeridx - 1];

        }

        private void FivePic_DoubleClick(object sender, EventArgs e)
        {
            if (orfit && or3d)
            {
                FivePic.BringToFront();             
                DrawMat(this.FivePic.Handle, 0, 0, rUnwrap, wwidth, wheight, wsteps);
                unwrap3DFinish = true;
            }
        }

        private void SixPic_DoubleClick(object sender, EventArgs e)
        {
            if (orfit && or3d)
            {
                DrawMat(this.SixPic.Handle, 0, 0, WoForShow, wwidth, wheight, wsteps);
                unwrap3DFinish = true;
            }
        }

        private void FivePic_MouseDown(object sender, MouseEventArgs e)
        {
            if (orfit && or3d)
            {
                Firdown = true;
                startfirx = e.X;
                startfiry = e.Y;
            }
        }

        private void FivePic_MouseMove(object sender, MouseEventArgs e)
        {
            if (Firdown && orfit && or3d)
            {

                int firlengthx = e.X - startfirx;
                int firlengthy = e.Y - startfiry;
                DrawMat(this.FivePic.Handle, shiftx - firlengthx, shifty + firlengthy, rUnwrap, wwidth, wheight, wsteps);

            }
        }

        private void FivePic_MouseUp(object sender, MouseEventArgs e)
        {
            if (orfit && or3d)
            {
                Firdown = false;
                int firlengthx = e.X - startfirx;
                int firlengthy = e.Y - startfiry;
                shiftx -= firlengthx;
                shifty += firlengthy;
            }
        }

        private void SixPic_MouseDown(object sender, MouseEventArgs e)
        {
            if (orfit && or3d)
            {
                Secdown = true;
                secstartfirx = e.X;
                secstartfiry = e.Y;
            }
        }

        private void SixPic_MouseMove(object sender, MouseEventArgs e)
        {
            if (Secdown && orfit && or3d)
            {

                int seclengthx = e.X - secstartfirx;
                int seclengthy = e.Y - secstartfiry;
                DrawMat(this.SixPic.Handle, secshiftx - seclengthx, secshifty + seclengthy, WoForShow, wwidth, wheight, wsteps);

            }
        }

        private void SixPic_MouseUp(object sender, MouseEventArgs e)
        {
            if (orfit && or3d)
            {
                Secdown = false;
                int seclengthx = e.X - secstartfirx;
                int seclengthy = e.Y - secstartfiry;
                secshiftx -= seclengthx;
                secshifty += seclengthy;
            }
        }

        private void checkItem1_EditValueChanged(object sender, EventArgs e)
        {
            /*
            CheckEdit pCheckEdit = sender as CheckEdit;
            if(pCheckEdit.Checked)
            {
                radioButton1 = true;
                radioButton2 = false;
            }*/
        }

        private void CreateCurve_Click(object sender, EventArgs e)
        {
            Array.Clear(points, 0, points.Length);
            Array.Clear(up1points, 0, up1points.Length);
            //   CurveBox.Refresh();
            curvedowntimes = 0;
            unwrap1ginimg.DrawImage(Unwrap1ResPic, unwrap1wgap, unwrap1hgap, unwrap1width, unwrap1height);
            ginimg.DrawImage(img, picwgap, pichgap, picwidth, picheight);
            orcurve = true;
            ThirdPic.Cursor = Cursors.Cross;
            FourthPic.Cursor = Cursors.Cross;
        }
        public int PZTUno = 0;
        private void ImposeU_ItemClick(object sender, ItemClickEventArgs e)
        {
            string path = "F:\\Exp2Pictures\\" + "U" + Convert.ToString(UValue.EditValue) + "no" + PZTUno.ToString() + ".BMP";
            Com.Open();
            string sU = Convert.ToString(UValue.EditValue);
            double dU = double.Parse(sU);
            byte[] dataU = PZTU2Data(dU);
            Com.Write(dataU, 0, dataU.Length);
            Thread.Sleep(1000);
          //  CCDImg.Save(path);
            PZTUno++;
            Com.Close();
        }

        private void 实验软件_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            if (!CCDStop)
            {
                if (null != m_objIGXFeatureControl)
                {
                    m_objIGXFeatureControl.GetCommandFeature("AcquisitionStop").Execute();
                }
                if (null != m_objIGXStream)
                {
                    m_objIGXStream.StopGrab();
                    m_objIGXStream.UnregisterCaptureCallback();
                    m_objIGXStream.Close();
                    m_objIGXStream = null;
                }
                if (null != m_objIGXDevice)
                {
                    m_objIGXDevice.Close();
                    m_objIGXDevice = null;
                }
            }
         
            
        }

      

        private void pictureEdit2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureEdit2_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void barStaticItem2_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void WhichPic_SelectedIndexChanged(object sender, EventArgs e)
        {
            int picidx = WhichPic.SelectedIndex;
            FirPic.Image = images[picidx];
            bmpI = images[picidx];
        }
        int ScreenShotidx = 1;
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            Bitmap bit = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(bit);
            g.CopyFromScreen(this.Location, new Point(0, 0), bit.Size);
            bit.Save("F:\\ExpScreenShot\\ScreenShot" + ScreenShotidx.ToString() + ".BMP", System.Drawing.Imaging.ImageFormat.Bmp);
            ScreenShotidx++;
        }

        private void SelectExp_ListItemClick(object sender, ListItemClickEventArgs e)
        {
            if(SelectExp.ItemIndex == 3)
            {
                barEditItem3.Enabled = false;
                barEditItem3.EditValue = false;
                //barEditItem3.Enabled = false;
            }
            else
            {
                barEditItem3.Enabled = true;
                barEditItem3.EditValue = true;
            }
        }
        [DllImport("SaveGetStdDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetStd(IntPtr stdimg, int width, int height, int steps,int channels);
        [DllImport("SaveGetStdDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetStd(IntPtr img, int width, int height, int step, int channels,out int steps);
        private void SETSTD_ItemClick(object sender, ItemClickEventArgs e)
        {
            SetStd(WoFit, wwidth, wheight, Gsteps, 1);
        }

        private void ShowSavePicture_Click(object sender, ItemClickEventArgs e)
        {

        }

        private void GETREAL_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (WoFit != null && File.Exists(@"F:\\tempstd.xml"))
            {
                int realstep;
                IntPtr realsurface = GetStd(WoFit, wwidth, wheight, Gsteps, 1, out realstep);
                int stepc;
                IntPtr bmpColor = Colorbar(realsurface, wwidth, wheight, realstep, out stepc);
                Bitmap realShow = new Bitmap(wwidth, wheight, stepc,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb, bmpColor);
                FourthPic.Image = realShow;
            }
            else if(!File.Exists(@"F:\\tempstd.xml"))
            {
                MessageBox.Show("请先保存系统误差");
            }
            else
            {
                return;
            }
           
        }

    

        private void ReExp_Click(object sender, EventArgs e)
        {
            MessageBoxButtons ifclose = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("是否重新开始实验", "提示", ifclose);
            if (dr == DialogResult.OK)
            {
                System.Diagnostics.Process.Start("G:\\UnwrapFit6.0\\干涉仪软件第四版\\DXApplication7\\bin\\x86\\Debug\\DXApplication7.exe");
                Application.Exit();
            }
        }

        private void ERODE_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (bmp == null)
            {
                XtraMessageBox.Show("请选择需处理的图像", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.trackbar1.EditValue = 0;
                return;

            }

            int ksize = Convert.ToInt32(trackbar1.EditValue) * 2;
            Bitmap temp_bmp;

            if (dilateFlag)
            {
                temp_bmp = bmpDilate;
            }
            else if (bmpFilt != null)
            {
                temp_bmp = bmpFilt;
            }
            else
            {
                temp_bmp = bmp;
            }

            BitmapInfo bmpt = GetImagePixel(temp_bmp);

            int step = bmpt.Step;
            int gstep;
            if (ksize > 0)
            {
                IntPtr OutputIamge = erodeProc(bmpt.Result, width, height, step, ksize, out gstep);
                bmpErode = new Bitmap(bmp.Width, bmp.Height, gstep,
                    System.Drawing.Imaging.PixelFormat.Format8bppIndexed, OutputIamge);
                bmpErode.Palette = CvToolbox.GrayscalePalette;

                erodeFlag = true;

            }
            else
            {
                bmpErode = new Bitmap(temp_bmp);             
                erodeFlag = false;

            }
            picShow[PictureNum].Image = bmpErode;
            bmpToCanny = bmpErode;
 
        }

        private void trackbar1_HiddenEditor(object sender, ItemClickEventArgs e)
        {

        }

        private void dilate_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (bmp == null)
            {
                XtraMessageBox.Show("请选择需处理的图像", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.trackbar2.EditValue = 0;
                return;
            }

            int ksize = 2 * Convert.ToInt32(trackbar2.EditValue); //当前刻度值

            Bitmap temp_bmp; //当前图片

            if (erodeFlag)
            {
                temp_bmp = bmpErode;
            }
            else if (bmpFilt != null)
            {
                temp_bmp = bmpFilt;
            }
            else
            {
                temp_bmp = bmp;
            }

            //当前处理的Bitmap图
            //Bitmap temp_bmp = new Bitmap(bmp);
            BitmapInfo bmpt = GetImagePixel(temp_bmp);
            int step = bmpt.Step;
            int gstep;
            if (ksize > 0)
            {
                IntPtr OutputImage = dilateProc(bmpt.Result, width, height, step, ksize, out gstep);
                bmpDilate = new Bitmap(bmp.Width, bmp.Height, gstep,
                     System.Drawing.Imaging.PixelFormat.Format8bppIndexed, OutputImage);
                bmpDilate.Palette = CvToolbox.GrayscalePalette;
                //pictureBox1.Image = bmpDilate;
                dilateFlag = true;
                //bmpToCanny = bmpDilate;
            }
            else
            {
                bmpDilate = new Bitmap(temp_bmp);
                //pictureBox1.Image = bmpDilate;
                dilateFlag = false;
                //bmpToCanny = bmpDilate;
            }
            picShow[PictureNum].Image = bmpDilate;
            bmpToCanny = bmpDilate;
        }

        //Zernike拟合
        [DllImport("ZernikeFit2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Zernikefit(IntPtr image, int width, int height, out int gstep, out float PV, out float RMS,
            out IntPtr outWo, out IntPtr dstA, out int rhoNum, out IntPtr Zernike, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] float[] A,
            out IntPtr residual, out float PV_residual, out float RMS_residual, out float Wo_max, out float Wo_min, out float Residual_max, out float Residual_min);

        [DllImport("AnnularZernike.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AnnularZernikefit(IntPtr image, int width, int height, out int gstep, out float PV, out float RMS,
            out IntPtr outWo, out IntPtr dstA, out int rhoNum, out IntPtr Zernike, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] float[] A,
            out IntPtr residual, out float PV_residual, out float RMS_residual, out float Wo_max, out float Wo_min, out float Residual_max, out float Residual_min);

        public static IntPtr dstA;        //Zernike多项式对应的37项系数
        public static float[] A = new float[37];       //Zernike系数（数组形式）
        public static IntPtr Zernike;     //Zernike多项式
        public static IntPtr Wo;          //拟合出的波面（32位Float型）
        public static int rhoNum;         //总数据点数
        public static float PVvalue;      //PV
        public static float RMSvalue;     //RMS
        public static float PV_residual;  //residual PV
        public static float RMS_residual; //residual RMS
        public IntPtr WoForShow;
        public IntPtr residual;
        public Bitmap imgColor, imgResidual;
        public int b = 0;//yueyue
        private void Button10_ItemClick(object sender, ItemClickEventArgs e)
        {
            SourcePicL.Visible = false;
            ML.Visible = false;
            Unwrap1L.Visible = false;
            Unwrap2L.Visible = false;

            ZernikeInterfere.Visible = true;
            int gstep;

            float Wo_max, Wo_min, Residual_max, Residual_min;
            splashScreenManager4.ShowWaitForm();//yueyue
            Thread t = new Thread(new ThreadStart(Y));
            t.Start();

            if (SelectExp.ItemIndex != 3)
            {
                WoForShow = Zernikefit(W, wwidth, wheight, out gstep, out PVvalue, out RMSvalue,
                    out Wo, out dstA, out rhoNum, out Zernike, A, out residual, out PV_residual, out RMS_residual,
                    out Wo_max, out Wo_min, out Residual_max, out Residual_min);
            }
            else
            {
                WoForShow = AnnularZernikefit(W, wwidth, wheight, out gstep, out PVvalue, out RMSvalue,
                    out Wo, out dstA, out rhoNum, out Zernike, A, out residual, out PV_residual, out RMS_residual,
                    out Wo_max, out Wo_min, out Residual_max, out Residual_min);
            }
            
            b = 1;
            //Bitmap WoShow= new Bitmap(wwidth, wheight, gstep,
            //         System.Drawing.Imaging.PixelFormat.Format8bppIndexed, WoForShow);
            //WoShow.Palette = CvToolbox.GrayscalePalette;
            //ThirdPic.Image = WoShow;

            //拟合位相图
            int stepc;
            IntPtr bmpColor = Colorbar(WoForShow, wwidth, wheight, wsteps, out stepc);
            imgColor = new Bitmap(wwidth, wheight, stepc,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb, bmpColor);


            //残差图
            IntPtr bmpResidual = Colorbar(residual, wwidth, wheight, wsteps, out stepc);
            imgResidual = new Bitmap(wwidth, wheight, stepc,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb, bmpResidual);


            PV.EditValue = PVvalue.ToString();
            RMS.EditValue = RMSvalue.ToString();

            //PVresidual.Text = PV_residual.ToString();
            //RMSresidual.Text = RMS_residual.ToString();

            //ListView
            listView1.Visible = true;
            ListShow(A);

            FourthPic.Visible = false;
            FivePic.Visible = true;
            SixPic.Visible = true;
            panel1.Visible = false;
            panel2.Visible = false;
            CCDShow.Visible = false;
            Colormap4.Visible = false;
            Fourth_lab1.Visible = false;
            Fourth_lab2.Visible = false;
            //图像重新排列与显示
            Colormap1.Visible = true;
            Colormap2.Visible = true;
            Colormap3.Visible = true;
            Colormap1.Image = Image.FromFile(Application.StartupPath + "\\colorscale_jet2.jpg");
            Colormap2.Image = Image.FromFile(Application.StartupPath + "\\colorscale_jet2.jpg");
            Colormap3.Image = Image.FromFile(Application.StartupPath + "\\colorscale_jet2.jpg");
            

            First_lab1.Visible = true; First_lab1.Text = Math.Round(Wunwrap_Max, 3).ToString();
            First_lab2.Visible = true; First_lab2.Text = Math.Round(Wunwrap_Min, 3).ToString();
            Second_lab1.Visible = true; Second_lab1.Text = Math.Round(Wo_max, 3).ToString();
            Second_lab2.Visible = true; Second_lab2.Text = Math.Round(Wo_min, 3).ToString();
            Third_lab1.Visible = true; Third_lab1.Text = Math.Round(Residual_max, 3).ToString();
            Third_lab2.Visible = true; Third_lab2.Text = Math.Round(Residual_min, 3).ToString();
           

            label2.Visible = true;
            label3.Visible = true;
            label4.Visible = true;

            FivePic.Image = img;
            SixPic.Image = imgColor;
            ThirdPic.Image = imgResidual;
            //FourthPic.Image = null;

            orfit = true;

            Button11.Enabled = true;
        }
        void Y()//yueyue
        {
            while (b == 0)
                Thread.Sleep(100);
            splashScreenManager4.CloseWaitForm();
        }

        //去除对应的Zernike系数
        [DllImport("RemoveZernike.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RemoveZernike(IntPtr dstAo, IntPtr Zernike, out IntPtr WoFit, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] float[] Ar,
            out float PV, out float RMS, int width, int height, out int gstep, int rhoNum, int[] cNum, out float Max_Value, out float Min_Value, int method);
        public static IntPtr WoFit;
        public int[] cNum = new int[37];  //添加修改项
        public IntPtr WoForShow2;
        public Bitmap imgColor2;
        int Gsteps;
      
        private void Button11_Click(object sender, ItemClickEventArgs e)
        {
            int gstep;
            float Max_Value, Min_Value;
            if(SelectExp.ItemIndex != 3)
            {
                WoForShow2 = RemoveZernike(dstA, Zernike, out WoFit, A, out PVvalue, out RMSvalue, wwidth, 
                    wheight, out gstep, rhoNum, cNum, out Max_Value, out Min_Value, 1);
            }
            else
            {
                WoForShow2 = RemoveZernike(dstA, Zernike, out WoFit, A, out PVvalue, out RMSvalue, wwidth,
                    wheight, out gstep, rhoNum, cNum, out Max_Value, out Min_Value, 2);
            }
            
            //Bitmap ImgForShow= new Bitmap(wwidth, wheight, gstep,
            //         System.Drawing.Imaging.PixelFormat.Format8bppIndexed, WoForShow);
            //ImgForShow.Palette = CvToolbox.GrayscalePalette;
            // ThirdPic.Image = ImgForShow;

            int stepc;
            IntPtr bmpColor = Colorbar(WoForShow2, wwidth, wheight, wsteps, out stepc);
            imgColor2 = new Bitmap(wwidth, wheight, stepc,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb, bmpColor);
            Gsteps = gstep;

            FourthPic.Visible = true;
            Colormap4.Visible = true;

            Colormap4.Image = Image.FromFile(Application.StartupPath + "\\colorscale_jet2.jpg");
            Fourth_lab1.Visible = true; Fourth_lab1.Text = Math.Round(Max_Value, 3).ToString();
            Fourth_lab2.Visible = true; Fourth_lab2.Text = Math.Round(Min_Value, 3).ToString();
            label5.Visible = true;

            FourthPic.Image = imgColor2;

            PV.EditValue = PVvalue.ToString();
            RMS.EditValue = RMSvalue.ToString();          
            //表格显示
            ListShow(A);
            GETREAL.Enabled = true;
            SETSTD .Enabled = true;
        }

        private void ListShow(float[] A)
        {
            listView1.Items.Clear();
            int Num = 37;
            double tempValue;
            this.listView1.BeginUpdate();
            for (int i = 0; i < Num; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = "第" + (i + 1) + "项";
                tempValue = Math.Round(A[i], 6);
                lvi.SubItems.Add(tempValue.ToString());
                this.listView1.Items.Add(lvi);
            }
            this.listView1.EndUpdate();
        }


        private void ThirdPic_DoubleClick(object sender, EventArgs e)
        {
            if (or3d == true && !orfit)
            {
                if (UnwrapRes1 != null)
                {
                    ThirdPic.Refresh();
                    //Thread.Sleep(5000);
                    DrawMat(this.ThirdPic.Handle, 0, 0, UnwrapRes1, newwidth1, newheight1, steps);
                    unwrap13D = true;
                    unwrap3DFinish = true;
                }
                else
                {
                    MessageBox.Show("请先进行解调操作");
                }
            }
            if (or3d && orfit)
            {
                ThirdPic.BringToFront();
                ThirdPic.Refresh();
                DrawMat(this.ThirdPic.Handle, 0, 0, residual, wwidth, wheight, wsteps);
                unwrap3DFinish = true;
            }
        }

        bool down = false;
        public static Point[] up1points = new Point[2];
        public bool Thirddown = false;
        public int thirdstartfirx, thirdstartfiry;
        private void ThirdPic_MouseDown(object sender, MouseEventArgs e)
        {
            if (!orcurve && !orfit)
            {
                if (unwrap13D == true)
                {
                    startx = e.X;
                    starty = e.Y;
                    down = true;
                }
            }
            else if (orcurve && !orfit)
            {
                slinex = e.X;
                sliney = e.Y;
                up1points[0] = new Point(slinex, sliney);
                down = true;
            }
            if (orfit && or3d)
            {
                Thirddown = true;
                thirdstartfirx = e.X;
                thirdstartfiry = e.Y;
            }
        }

        public int thirdshiftx, thirdshifty;
        private void ThirdPic_MouseMove(object sender, MouseEventArgs e)
        {
            if (!orcurve && !orfit)
            {
                if (unwrap13D == true)
                {
                    if (down)
                    {
                        overx = e.X;
                        overy = e.Y;
                        lengthx = overx - startx;
                        lengthy = overy - starty;
                        if (or3d)
                        {
                            DrawMat(this.ThirdPic.Handle, shiftx - lengthx, shifty + lengthy, UnwrapRes1, newwidth1, newheight1, steps);
                        }
                    }
                }
            }
            else if (orcurve && !orfit)
            {
                // inimg.Image = img;
                if (down)
                {

                  
                    unwrap1ginimg.DrawImage(Unwrap1ResPic, unwrap1wgap, unwrap1hgap, unwrap1width, unwrap1height);
                    Pen blackpen = new Pen(Color.Red, 2F);
                    //    g.DrawLine(blackpen ,slinex, sliney,overx,overy);
                    unwrap1ginimg.DrawLine(blackpen, slinex, sliney, e.X, e.Y);

                }
            }
            if (Thirddown && orfit && or3d)
            {

                int thirdlengthx = e.X - thirdstartfirx;
                int thirdlengthy = e.Y - thirdstartfiry;
                DrawMat(this.ThirdPic.Handle, thirdshiftx - thirdlengthx, thirdshifty + thirdlengthy, residual, wwidth, wheight, wsteps);

            }
        }

        private void ThirdPic_MouseUp(object sender, MouseEventArgs e)
        {
            if (!orcurve && !orfit)
            {
                if (unwrap13D == true)
                {
                    overx = e.X;
                    overy = e.Y;
                    int lengthx = overx - startx;
                    int lengthy = overy - starty;
                    down = false;
                    shifty += lengthy;
                    shiftx -= lengthx;
                    lengthy = 0;
                }
            }
            else if (orcurve && !orfit)
            {
                down = false;
                Pen blackpen = new Pen(Color.Black, 2F);
                up1points[1] = new Point(e.X, e.Y);
                unwrap1ginimg.DrawLine(blackpen, up1points[0], up1points[1]);
                curveBox2 panel2 = new curveBox2();
                panel2.StartPosition = FormStartPosition.Manual;
                Point p = new Point(0, 0);
                p = CreateCurve.PointToScreen(p);
                Point curve2Pos = new Point(p.X - panel2.Width, p.Y - panel2.Height);
                panel2.Location = curve2Pos;
                panel2.Show();
                panel2.Refresh();
                orcurve = false;

                

            }
            if (orfit && or3d)
            {
                Thirddown = false;
                int thirdlengthx = e.X - thirdstartfirx;
                int thirdlengthy = e.Y - thirdstartfiry;
                thirdshiftx -= thirdlengthx;
                thirdshifty += thirdlengthy;
            }
        }

        private void FourthPic_DoubleClick(object sender, EventArgs e)
        {
            if (or3d == true && !orfit)
            {
                if (rUnwrap != null)
                {
                    DrawMat(this.FourthPic.Handle, 0, 0, rUnwrap, wwidth, wheight, wsteps);
                    unwrap23D = true;
                    unwrap3DFinish = true;
                }
                else
                {
                    MessageBox.Show("请先进行解包裹操作");
                }
            }
            if (orfit && or3d)
            {

                DrawMat(this.FourthPic.Handle, 0, 0, WoForShow2, wwidth, wheight, wsteps);
                unwrap3DFinish = true;
            }
        }

        public bool Fourthdown = false;
        public int fourthstartfirx, fourthstartfiry, fourthshiftx, fourthshifty;
        private void FourthPic_MouseDown(object sender, MouseEventArgs e)
        {
            if (!orcurve && !orfit)
            {
                if (unwrap23D)
                {
                    startx = e.X;
                    starty = e.Y;
                    down = true;
                }
            }
            else if (orcurve && !orfit)
            {

                slinex = e.X;
                sliney = e.Y;
                points[0] = new Point(slinex, sliney);
                down = true;

            }
            if (orfit && or3d)
            {
                Fourthdown = true;
                fourthstartfirx = e.X;
                fourthstartfiry = e.Y;
            }
        }

        private void FourthPic_MouseMove(object sender, MouseEventArgs e)
        {
            if (!orcurve)
            {
                if (unwrap23D)
                {
                    if (down)
                    {
                        overx = e.X;
                        overy = e.Y;
                        lengthx = overx - startx;
                        lengthy = overy - starty;

                        if (or3d)
                        {
                            DrawMat(this.FourthPic.Handle, shiftx - lengthx, shifty + lengthy, rUnwrap, wwidth, wheight, wsteps);
                        }
                    }
                }
            }
            else
            {
                // inimg.Image = img;
                if (down)
                {

                  
                    ginimg.DrawImage(img, picwgap, pichgap, picwidth, picheight);
                    Pen blackpen = new Pen(Color.Red, 2F);
                    ginimg.DrawLine(blackpen, slinex, sliney, e.X, e.Y);


                }
            }
            if (Fourthdown && orfit && or3d)
            {

                int fourthlengthx = e.X - fourthstartfirx;
                int fourthlengthy = e.Y - fourthstartfiry;
                DrawMat(this.FourthPic.Handle, fourthshiftx - fourthlengthx, fourthshifty + fourthlengthy, WoForShow2, wwidth, wheight, wsteps);

            }
        }

        private void FourthPic_MouseUp(object sender, MouseEventArgs e)
        {
            if (!orcurve && !orfit)
            {
                if (unwrap23D)
                {
                    overx = e.X;
                    overy = e.Y;
                    int lengthx = overx - startx;
                    int lengthy = overy - starty;
                    down = false;
                    shifty += lengthy;
                    shiftx -= lengthx;
                    lengthy = 0;
                }
            }
            else if (orcurve && !orfit)
            {
                
                down = false;
                Pen blackpen = new Pen(Color.Red, 2F);
                points[1] = new Point(e.X, e.Y);
                ginimg.DrawLine(blackpen, points[0], points[1]);
                CurvePic pane1 = new CurvePic();

                Point p = new Point(0, 0);
                p = CreateCurve.PointToScreen(p);
                Point curvepos = new Point(p.X, p.Y - pane1.Height);
                pane1.StartPosition = FormStartPosition.Manual;
                pane1.Location = curvepos;
                pane1.Show();
                pane1.Refresh();
                orcurve = false;
            }
            if (orfit && or3d)
            {
                Fourthdown = false;
                int fourthlengthx = e.X - fourthstartfirx;
                int fourthlengthy = e.Y - fourthstartfiry;
                fourthshiftx -= fourthlengthx;
                fourthshifty += fourthlengthy;
            }
        }
    }

    public class BitmapInfo
    {
        public byte[] Result { get; set; }
        public int Step { get; set; }
    }
    public static class CvToolbox
    {

        // #region Color Pallette  
        /// <summary>  
        /// The ColorPalette of Grayscale for Bitmap Format8bppIndexed  
        /// </summary>  
        public static readonly System.Drawing.Imaging.ColorPalette GrayscalePalette = GenerateGrayscalePalette();

        private static System.Drawing.Imaging.ColorPalette GenerateGrayscalePalette()
        {
            using (Bitmap image = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format8bppIndexed))
            {
                System.Drawing.Imaging.ColorPalette palette = image.Palette;
                for (int i = 0; i < 256; i++)
                {
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                }
                return palette;
            }
        }
    }
}