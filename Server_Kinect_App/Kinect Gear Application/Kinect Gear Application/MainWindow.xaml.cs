using GearKinect_Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kinect_Gear_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           
            this.Loaded += OnStartServer;

        }
        private KinectHandler kinectHandler = new KinectHandler();
        private SocketHandler socketHandler = new SocketHandler();

        //init program
        private void OnStartServer(object sender, RoutedEventArgs e)
        {
            Response res = kinectHandler.initKinect();
            isFunctionSuccessful(res);

            kinectHandler.setIrImage(irImage);
            kinectHandler.setHeadTrackingUI(headTrackingCanvas, TrackedBodyTextBlock);

            res = socketHandler.init();
            isFunctionSuccessful(res);
        }

        //check if the function was successful if not then show errror message and then shut down the application
        private void isFunctionSuccessful(Response res)
        {
            if (!res.Status)
            {
                MessageBoxResult result = MessageBox.Show(res.ErrorMessage, "Ok", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }
    
    }
}

