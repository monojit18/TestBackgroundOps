using System;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using TestBackgroundOps;

namespace TestBackgroundOps.Droid
{

    [Service(Name = "com.monojit.development.TestBinderServie")]
    public class TestBinderService : Service
    {
    
        private Binder _binder;

        public override void OnCreate()
        {
            base.OnCreate();
            _binder = new TestBinder(this);
        }

        public override IBinder OnBind(Intent intent)
        {


            return _binder;

        }

        public override bool OnUnbind(Intent intent)
        {
            return base.OnUnbind(intent);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public async Task UploadPostsAsync(TestBkgdUploadTask testBkgdUploadTask)
        {

            await testBkgdUploadTask.UploadPostsAsync();

        }

    }

    public class TestBinder : Binder
    {


        public TestBinderService TestBinderService { get; private set; }

        public TestBinder(TestBinderService testBinderService)
        {

            TestBinderService = testBinderService;

        }

    }

    public class TestConnection : Java.Lang.Object, IServiceConnection
    {


        private Activity _activity;
        TestBkgdUploadTask _testBkgdUploadTask;

        public TestBinder TestBinder { get; private set; }

        public TestConnection(Context context, TestBkgdUploadTask testBkgdUploadTask)
        {

            _activity = context as Activity;
            _testBkgdUploadTask = testBkgdUploadTask;

        }

        public async void OnServiceConnected(ComponentName name, IBinder service)
        {

            TestBinder = service as TestBinder;
            var testService = TestBinder.TestBinderService;
            await testService.UploadPostsAsync(_testBkgdUploadTask);


        }

        public void OnServiceDisconnected(ComponentName name)
        {

            TestBinder = null;

        }

    }
}
