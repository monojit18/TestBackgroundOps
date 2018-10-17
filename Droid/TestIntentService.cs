using System;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using TestBackgroundOps;

namespace TestBackgroundOps.Droid
{

    [Service(Name = "com.monojit.development.TestIntentServie")]
    public class TestIntentService : IntentService
    {

        private TestBkgdDownloadTask _testBkgdDownloadTask;

        protected override async void OnHandleIntent(Intent intent)
        {

            var width = intent.GetIntExtra("width", 0);
            var height = intent.GetIntExtra("height", 0);

            await _testBkgdDownloadTask.SingleDownloadImageForSizeAsync(width,
                                                                        height);

        }

        public TestIntentService() : base("com.monojit.development.TestIntentServie")
        {

            _testBkgdDownloadTask = new TestBkgdDownloadTask();

        }

    }

}
