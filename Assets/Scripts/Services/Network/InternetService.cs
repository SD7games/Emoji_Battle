using UnityEngine;

public static class InternetService
{
    public static bool IsOnline
    {
        get
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }

    public static bool IsOnlineMobileOrWifi
    {
        get
        {
            var r = Application.internetReachability;
            return r == NetworkReachability.ReachableViaCarrierDataNetwork ||
                   r == NetworkReachability.ReachableViaLocalAreaNetwork;
        }
    }
}