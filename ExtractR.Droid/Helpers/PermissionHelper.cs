using Android;

namespace ExtractR.Droid.Helpers
{
    public class PermissionHelper
    {
        public static bool ShouldDelete = true;

        public static string[] PermissionsNeeded = new string[]
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage
        };
    }
}