using System;
using UIKit;

namespace Urho.Samples.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }

        internal static object CreateInstance(Type type, ApplicationOptions options)
        {
            throw new NotImplementedException();
        }
    }
}