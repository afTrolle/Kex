using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using Kinect_Gear_Application;

namespace GearKinect_Application
{
    /**
    Handels using the kinect sdk. 
    */
    class KinectHandler
    {

        KinectSensor sensor;

        //infrared reader, gives us infrared camera view.
        InfraredFrameReader irReader;

        //for presenting the ir image
        ushort[] irData;
        byte[] irDataConverted;

        //gives the body trackng of the joints.
        BodyFrameReader bodyReader;
        Body[] bodies;

        WriteableBitmap wbmap;
        Int32Rect wbmapRect;
        int wbmapStride;
        public Response initKinect()
        {
            //gets the kinect sensor can only be one
            sensor = KinectSensor.GetDefault();

            initBodyCamera();

            initIrCamera();

            //start kinect sensor
            sensor.Open();

            if (sensor.IsOpen)
            {
                return new Response(true,null,null);
            }
            else
            {
                return new Response(false, "No Kinect camera found", null);
            }

        }

        private TextBlock trackedBodyTextBlock;
        private Canvas headTrackingCanvas;
        internal void setHeadTrackingUI(Canvas headTrackingCanvas, TextBlock trackedBodyTextBlock)
        {
            this.headTrackingCanvas = headTrackingCanvas;
            this.trackedBodyTextBlock = trackedBodyTextBlock;
        }

        internal void setIrImage(System.Windows.Controls.Image irImage)
        {
            irImage.Source = wbmap;
        }
   
        private void initBodyCamera()
        {
            //setup body reader //give body joints cordinates
            bodyReader = sensor.BodyFrameSource.OpenReader();
            bodies = new Body[6]; //kinect camera can max track 6 people
            bodyReader.FrameArrived += bodyReader_FrameArrived;
        }

        //called when body location has been gathered should be around 30 to 15 fps  depeniding on lightning conditions
        private void bodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs args)
        {
            //TODO implmenent handling of the camera
            using (BodyFrame bodyFrame = args.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    bodyFrame.GetAndRefreshBodyData(bodies);
                    headTrackingCanvas.Children.Clear();
                    trackedBodyTextBlock.Text = "";
                    foreach (Body body in bodies)
                    {
                        if (body.IsTracked)
                        {
                            
                            Joint NeckJoint = body.Joints[JointType.Neck];
                            if (NeckJoint.TrackingState == TrackingState.Tracked)
                            {
                                trackedBodyTextBlock.Text +=  string.Format("person {0} \n x: {1} \n y: {2} \n z: {3}", body.TrackingId, NeckJoint.Position.X,NeckJoint.Position.Y ,NeckJoint.Position.Z);
                                DepthSpacePoint dsp = sensor.CoordinateMapper.MapCameraPointToDepthSpace(NeckJoint.Position);
                                Ellipse headcircle = new Ellipse() { Width = 50, Height = 50, Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0)) };
                                headTrackingCanvas.Children.Add(headcircle);
                                Canvas.SetLeft(headcircle, dsp.X - 25);
                                Canvas.SetTop(headcircle, dsp.Y - 25);

                                //TODO send to connected Devices!
                            }
                        }
                    }
                }
            }
        }
           


        private void initIrCamera()
        {
            //setup ir reader so we get a view of the camera
            irReader = sensor.InfraredFrameSource.OpenReader();
            FrameDescription fd = sensor.InfraredFrameSource.FrameDescription;

            //data holders
            irData = new ushort[fd.LengthInPixels];
            irDataConverted = new byte[fd.LengthInPixels * 4];

            wbmap = new WriteableBitmap(fd.Width, fd.Height, 96, 96, PixelFormats.Gray16, null);
            wbmapRect  = new Int32Rect(0, 0, wbmap.PixelWidth, wbmap.PixelHeight);
            wbmapStride = wbmap.PixelWidth * wbmap.Format.BitsPerPixel / 8;

            irReader.FrameArrived += irReader_FrameArrived;
        }

        private void irReader_FrameArrived(object sender, InfraredFrameArrivedEventArgs args)
        {
            using (InfraredFrame irFrame = args.FrameReference.AcquireFrame())
            {
                if (irFrame != null)
                {
                    irFrame.CopyFrameDataToArray(irData);
                    //write ir data to bitmap
                    wbmap.WritePixels(wbmapRect, irData, wbmapStride, 0);
                }
            }
        }
    }
        

}
