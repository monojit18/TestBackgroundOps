using System;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.Views;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Locations;
using Android.Runtime;
using TestBackgroundOps;

namespace TestBackgroundOps.Droid
{

    [Service(Name = "com.monojit.development.TestForegroundService")]
    public class TestForegroundService : Service, ILocationListener
    {

        private LocationManager _locationManager;
        private Activity _hostActivity;

        // public event EventHandler<LocationChangedEventArgs>

        public TestForegroundService()
        {




        }

        public override StartCommandResult OnStartCommand(Intent intent,
                                                          StartCommandFlags flags,
                                                          int startId)
        {
            var appContext = this.ApplicationContext;
            _locationManager = appContext.GetSystemService(LocationService)
                                                 as LocationManager;

            _locationManager.RequestLocationUpdates(LocationManager.GpsProvider,
                                                    2000, 1, this);

            var text = Resources.GetString(Resource.String.app_name);
            var notification = new Notification(Android.Resource.Drawable
                                                .IcNotificationOverlay,
                                                text);

            StartForeground(1, notification);
            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {

            return null;

        }


        public void OnLocationChanged(Location location)
        {

            var intent = new Intent("com.monojit.development.TestBackgroundOps.LocationReceiver");
            intent.PutExtra("lat", location.Latitude.ToString());
            intent.PutExtra("lng", location.Longitude.ToString());
            SendBroadcast(intent);

        }


        public void OnProviderDisabled(string provider)
        {


        }


        public void OnProviderEnabled(string provider)
        {


        }

        public void OnStatusChanged(string provider, Availability status,
                                    Bundle extras)
        {



        }
    }
}
