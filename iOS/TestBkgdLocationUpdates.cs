using System;
using UIKit;
using Foundation;
using CoreLocation;

namespace TestBackgroundOps.iOS
{
    public class TestBkgdLocationUpdates
    {

        private CLLocationManager _locationManager;

        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated;

        public TestBkgdLocationUpdates()
        {

            _locationManager = new CLLocationManager();
            _locationManager.PausesLocationUpdatesAutomatically = false;
            _locationManager.RequestAlwaysAuthorization();
            _locationManager.AllowsBackgroundLocationUpdates = true;

            _locationManager.LocationsUpdated += (object sender,
                                                  CLLocationsUpdatedEventArgs e) => 
            {

                var locationsArray = e.Locations;
                if ((locationsArray != null) && (locationsArray.Length > 0))
                {

                    var lastLocation = locationsArray[locationsArray.Length - 1];
                    LocationUpdated.Invoke(sender,
                                           new LocationUpdatedEventArgs(lastLocation));

                }
                    
            };

        }

        public void StartLocationTracking()
        {

            _locationManager.StartUpdatingLocation();

        }

    }

    public class LocationUpdatedEventArgs : EventArgs
    {

        private readonly CLLocation _location;

        public LocationUpdatedEventArgs(CLLocation location)
        {

            _location = location;

        }

        public CLLocation Location
        {

            get
            {

                return _location;

            }

        }

    }
}
