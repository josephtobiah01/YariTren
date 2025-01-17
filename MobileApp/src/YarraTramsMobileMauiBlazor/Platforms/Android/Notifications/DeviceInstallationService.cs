﻿namespace YarraTramsMobileMauiBlazor;

using System.Diagnostics;
using Android.Gms.Common;
using Android.Gms.Extensions;
using Android.Provider;
using Firebase.Messaging;
using Microsoft.Azure.NotificationHubs;


public partial class DeviceInstallationService
{
    private static bool NotificationsSupported
        => GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(Platform.AppContext) == ConnectionResult.Success;

    public static string? GetDeviceId()
        => Settings.Secure.GetString(Platform.AppContext.ContentResolver, Settings.Secure.AndroidId);

    //private static async Task<Installation?> GetInstallation()
    //{
    //    if (!NotificationsSupported)
    //    {
    //        return null;
    //    }

    //    try
    //    {
    //        var firebaseToken = await FirebaseMessaging.Instance.GetToken();
    //        return new Installation
    //        {
    //            InstallationId = GetDeviceId(),
    //            Platform = NotificationPlatform.FcmV1,
    //            PushChannel = firebaseToken.ToString()
    //        };
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine(ex.Message);
    //        return null;
    //    }
    //}


    public static async Task RegisterDevice(string notificationHub, string connectionString)
    {
        try
        {
            if (!NotificationsSupported)
            {
                return;
            }

            var firebaseToken = await FirebaseMessaging.Instance.GetToken();
            var installation = new Installation
            {
                InstallationId = GetDeviceId(),
                Platform = NotificationPlatform.FcmV1,
                PushChannel = firebaseToken.ToString()
            };
            var client = NotificationHubClient.CreateClientFromConnectionString(connectionString, notificationHub, true);
            await client.CreateOrUpdateInstallationAsync(installation);
        }
        catch (Exception ex) 
        {
            Console.WriteLine(ex);
        }
        
    }
}
