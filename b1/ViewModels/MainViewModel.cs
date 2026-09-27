using System;
using System.Collections.Generic;
using System.Text;
using OpenCvSharp;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;



namespace b1.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        //bien camera
        private VideoCapture camera;
        //bien chua hinh anh
        private BitmapSource cameraImage;
        public BitmapSource CameraImage
        {
            get { return cameraImage; }
            set {
                cameraImage = value;
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs("CameraImage"));
            }
        }
        //timer cap nhat hinh anh
        private DispatcherTimer timer;
        public RelayCommand OpenCamCommand { get; set; }
        public RelayCommand CloseCamCommand { get; set; }
        public MainViewModel()
        {
            OpenCamCommand = new RelayCommand(OpenCamera);
            CloseCamCommand = new RelayCommand(CloseCamera);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(33);
            timer.Tick += UpdateCamera;
        }
        //ham mo cam 
        public void OpenCamera()
        {
            camera = new VideoCapture(0);
            if (camera.IsOpened())
            {
                timer.Start();

            }
            else
            {
                camera.Dispose();
                camera = null;
            }
        }
        //doc hinh anh camera
        private void UpdateCamera(object sender,EventArgs e)
        {
            if(camera == null)
            {
                return;
            }
            Mat frame = new Mat();
            camera.Read(frame);
            if (!frame.Empty())
            {
               // CameraImage = frame.ToBitmapSource();
               // CameraImage.Freeze();
               // tao mat moi de chua anh xam
               Mat gray = new Mat();
                //chuyen anh mau sang anh xam
                Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);
                //chuyen mat cua opencv ==> bitmapsouce cua wpf
                //BitmapSource image = gray.ToBitmapSource();
                //doc hinh anh
              //  image.Freeze();
                //gan anh vao cameraimage
             //   CameraImage = image;
                //giai phong gray
             //   gray.Dispose();
                // b2 : chuyen anh xam ==> den, trang
                Mat binary = new Mat();
                Cv2.Threshold(gray, binary, 120, 255, ThresholdTypes.Binary);
                //hien thi b2
                //    BitmapSource image = binary.ToBitmapSource();
                //    image.Freeze();
                //    CameraImage = image;
                //    gray.Dispose();
                //    binary.Dispose();
                // BUOC 3
                // TIM CONTOUR
                // =========================

                Cv2.FindContours(
                    binary,
                    out Point[][] contours,
                    out HierarchyIndex[] hierarchy,
                    RetrievalModes.External,
                    ContourApproximationModes.ApproxSimple);


                // =========================
                // BUOC 4
                // VE KHUNG VAT THE
                // =========================

                foreach (Point[] contour in contours)
                {
                    Rect rect =
                        Cv2.BoundingRect(contour);


                    Cv2.Rectangle(
                        frame,
                        rect,
                        Scalar.Red,
                        2);
                }


                // =========================
                // BUOC 5
                // HIEN THI
                // =========================

                BitmapSource image =
                    frame.ToBitmapSource();

                image.Freeze();

                CameraImage = image;


                // Giai phong
                gray.Dispose();

                binary.Dispose();

            }
            frame.Dispose();
        }
        //tat camera
        public void CloseCamera()
        {
            timer.Stop();
            if (camera != null)
            {
                camera.Release();
                camera.Dispose();
                camera = null;
            }
            CameraImage = null;


        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
