using ObjCRuntime;
using UIKit;

namespace QrSortable;

public class Program
{
	// This is the main entry point of the application.
	static void Main(string[] args)
	{
        // if you want to use a different Application Delegate class from "AppDelegate"
        // you can specify it here.

        Console.WriteLine("QrSortable: PROGRAM MAIN START");

        UIApplication.Main(args, null, typeof(AppDelegate));
	}
}
