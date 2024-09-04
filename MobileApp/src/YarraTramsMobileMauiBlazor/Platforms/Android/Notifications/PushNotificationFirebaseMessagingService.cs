namespace YarraTramsMobileMauiBlazor;

using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using AndroidX.Core.Content;
using Firebase.Messaging;



[Service(Exported = false)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class PushNotificationFirebaseMessagingService : FirebaseMessagingService
{
    int messageId;
    public override void OnMessageReceived(RemoteMessage message)
    {
        base.OnMessageReceived(message);
        MainThread.InvokeOnMainThreadAsync(() =>
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(33)
                && ContextCompat.CheckSelfPermission(this, Manifest.Permission.PostNotifications) != Permission.Granted)
            {
                return;
            }

            if (!OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                return;
            }

            var pushNotification = message.GetNotification();

            var manager = (NotificationManager?)Application.Context.GetSystemService(NotificationService);
            var channel = new NotificationChannel(pushNotification.ChannelId ?? "MauiNotifications", "MauiNotifications", NotificationImportance.Max);
            manager?.CreateNotificationChannel(channel);

            var intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra("NotificationPage", "WebViewPage");
            intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);

            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notification = new Notification.Builder(Application.Context, channel.Id)
                                        .SetContentTitle(pushNotification.Title)
                                        .SetContentText(pushNotification.Body)
                                        .SetSmallIcon(Android.Resource.Mipmap.SymDefAppIcon)
                                        .SetContentIntent(pendingIntent)
                                        .SetAutoCancel(true)
                                        .Build();

            manager?.Notify(messageId++, notification);
        });
    }
}
