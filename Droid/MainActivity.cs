using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Widget;
using Android.OS;

namespace TestBackgroundOps.Droid
{
    [Activity(Label = "TestBackgroundOps", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        private TestConnection _testConnection;
        private List<Intent> _intentsList;

        private void StartBindService()
        {

            var testBkgdUploadTask = new TestBkgdUploadTask();
            _testConnection = new TestConnection(this, testBkgdUploadTask);
            var testIntent = new Intent(this, typeof(TestBinderService));

            var couldBind = BindService(testIntent, _testConnection,
                                        Bind.AutoCreate);
            Console.WriteLine(couldBind.ToString());

        }

        private void StartIntentService()
        {
            _intentsList = new List<Intent>();

            for (int i = 10, j = 10; i < 500; ++i, ++j)
            {

                var testIntent = new Intent(this, typeof(TestIntentService));
                testIntent.PutExtra("width", i);
                testIntent.PutExtra("height", j);
                _intentsList.Add(testIntent);

                StartService(testIntent);

            }

        }

        private void StartForegroundService()
        {


            if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) ==
               Permission.Denied)
                return;


            var testIntent = new Intent(this, typeof(TestForegroundService));
            StartForegroundService(testIntent);

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);

            button.Click +=  delegate
            {

                // StartBindService()
                // StartIntentService();
                StartForegroundService();

            };
        }
    }
}

