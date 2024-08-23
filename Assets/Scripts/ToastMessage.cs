using UnityEngine;

public class ToastMessage : MonoBehaviour
{
    // Method to show a toast message on an Android device
    public void ShowToast(string message)
    {
#if UNITY_ANDROID
        try
        {
            // Create a new AndroidJavaObject for UnityPlayer
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // Create a new AndroidJavaObject for the Toast class
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");

            // Call the makeText method on the Toast class to create a new Toast object
            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", currentActivity, message, toastClass.GetStatic<int>("LENGTH_SHORT"));
                toastObject.Call("show");
            }));
        }
        catch (System.Exception e)
        {
            Debug.LogError("Toast error: " + e.Message);
        }
#else
        Debug.Log("Toast messages only work on Android devices.");
#endif
    }
}
