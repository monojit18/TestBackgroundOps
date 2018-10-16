using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using Foundation;
using TestBackgroundOps;

namespace TestBackgroundOps.iOS
{
    public partial class ViewController : UIViewController
    {

        private TestBkgdDownloadTask _testBkgdDownloadTask;
        private TestBkgdLocationUpdates _testBkgdLocation;
        private nint _taskId;
        private List<NSUrlSessionTask> _sessionTasksList;
        private NSUrlSession _backgroundSession;

        private void PerformBackgroundTransfer()
        {

            if (_sessionTasksList == null)
                _sessionTasksList = new List<NSUrlSessionTask>();
            else
                _sessionTasksList.Clear();

            var urlsList = _testBkgdDownloadTask.PrepareURLsList();
            foreach (var urlString in urlsList)
            {

                var url = new NSUrl(urlString);
                var downloadTask = _backgroundSession.CreateDownloadTask(url);
                _sessionTasksList.Add(downloadTask);
                downloadTask.Resume();

            }

        }

        private async Task PerformBackgroundSafeTask()
        {

            _taskId = UIApplication.SharedApplication.BeginBackgroundTask(() =>
            {

                _testBkgdDownloadTask.Cancel();
                UIApplication.SharedApplication.EndBackgroundTask(_taskId);

            });

            await _testBkgdDownloadTask.PoolImagesAsync();

            InvokeOnMainThread(() =>
            {

                var rem = UIApplication.SharedApplication.BackgroundTimeRemaining;
                Console.WriteLine($"time: {rem.ToString()}");

            });

            UIApplication.SharedApplication.EndBackgroundTask(_taskId);

        }

        public ViewController(IntPtr handle) : base(handle)
        {

            _testBkgdDownloadTask = new TestBkgdDownloadTask();
            _testBkgdLocation = new TestBkgdLocationUpdates();
            _testBkgdLocation.LocationUpdated += (object sender,
                                                  LocationUpdatedEventArgs e) => 
            {


                Console.WriteLine(e.Location);
                var latString = e.Location.Coordinate.Latitude.ToString(".00000");
                var lngString = e.Location.Coordinate.Longitude.ToString(".000000");
                LocationLabel.Text = $"{latString} - {lngString}"; 

            };

            var sessionConfiguration = NSUrlSessionConfiguration
                                        .CreateBackgroundSessionConfiguration("com.monojit.development.session.config");
            _backgroundSession = NSUrlSession.FromConfiguration(sessionConfiguration,
                                                                new BackgroundSessionDelegate(),
                                                                new NSOperationQueue());

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            Button.AccessibilityIdentifier = "myButton";
            //Button.TouchUpInside += async delegate
            //{

            //    await PerformBackgroundSafeTask();


            //};

            //Button.TouchUpInside += async delegate
            //{

            //    await Task.Run(() => PerformBackgroundTransfer());

            //};

            Button.TouchUpInside +=  delegate
            {

                _testBkgdLocation.StartLocationTracking();

            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.        
        }

        public async Task<int> SingleDownloadImageAsync()
        {

            int byteCount = await _testBkgdDownloadTask.SingleDownloadImageAsync();
            return byteCount;

        }

        public void PerformSingleBackgroundTransfer()
        {

            if (_sessionTasksList == null)
                _sessionTasksList = new List<NSUrlSessionTask>();

            var urlString = _testBkgdDownloadTask.PrepareCurrentURL();
            var url = new NSUrl(urlString);
            var downloadTask = _backgroundSession.CreateDownloadTask(url);
            _sessionTasksList.Add(downloadTask);
            downloadTask.Resume();

        }
    }

    public class BackgroundSessionDelegate : NSUrlSessionDownloadDelegate
    {


        public override void DidWriteData(NSUrlSession session,
                                          NSUrlSessionDownloadTask downloadTask,
                                          long bytesWritten,
                                          long totalBytesWritten,
                                          long totalBytesExpectedToWrite)
        {

            InvokeOnMainThread(() =>
            {

                Console.WriteLine(bytesWritten.ToString());

            });

        }

        public override void DidFinishDownloading(NSUrlSession session,
                                                  NSUrlSessionDownloadTask
                                                  downloadTask,
                                                  NSUrl location)
        {

        }

        //public void DidFinishDownloading(NSUrlSession session,
        //                                 NSUrlSessionDownloadTask downloadTask,
        //                                 NSUrl location)
        //{





        //}

        //public IntPtr Handle { get; }

        //public void Dispose()
        //{



        //}



    }
}
