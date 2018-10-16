using System;
using System.Threading.Tasks;
using Foundation;
using ObjCRuntime;
using UIKit;
using WindowsAzure.Messaging;

namespace TestBackgroundOps.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations

        private const string KHubConnectionString = "Endpoint=sb://workshopspace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=HJeiJ+jVU6nMJDxq+in5nmQs8M5UDuglHm3w1HXUuA0=";
        private const string KHubnameString = "workshophub";

        private SBNotificationHub _notificationHub;

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method

            UIApplication.SharedApplication
                         .SetMinimumBackgroundFetchInterval(10);

            var settings = UIUserNotificationSettings.GetSettingsForTypes(
                            UIUserNotificationType.None, null);

            application.RegisterUserNotificationSettings(settings);
            application.RegisterForRemoteNotifications();

            return true;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        public override void PerformFetch(UIApplication application,
                                                Action<UIBackgroundFetchResult>
                                                completionHandler)
        {

            var viewController = Window.RootViewController as ViewController;
            viewController.PerformSingleBackgroundTransfer();

            //var count = await viewController.SingleDownloadImageAsync();
            //if (count > 0)
            //    completionHandler(UIBackgroundFetchResult.NewData);
            //else
                //completionHandler(UIBackgroundFetchResult.NoData);

        }

        [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
        public override void RegisteredForRemoteNotifications(UIApplication application,
                                                     NSData deviceToken)
        {
            _notificationHub = new SBNotificationHub(KHubConnectionString,
                                                     KHubnameString);
            _notificationHub.RegisterNativeAsync(deviceToken, null,
                                                 (NSError error) =>
            {



            });
        }

        [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
        public override void DidReceiveRemoteNotification(UIApplication application,
                                                          NSDictionary userInfo,
                                                          Action<UIBackgroundFetchResult>
                                                          completionHandler)
        {

            var info = userInfo["aps"] as NSDictionary;
            var contentAvailable = info["content-available"];
            Console.WriteLine($"{contentAvailable}");
            completionHandler(UIBackgroundFetchResult.NewData);


        }

        

      

    }
}

