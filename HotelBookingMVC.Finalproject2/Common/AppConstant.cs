namespace HotelBookingMVC.Finalproject2.Common
{
    public static class AppConstant
    {
        public static readonly HashSet<string> VALID_FILE_EXTENSIONS = new HashSet<string>
    {
        "jpg", "jpeg", "png", "gif", "mp4", "avi" // Add your valid extensions here
    };
        public static string CART_SESSION = "ShoppingCart";
        public static string IP_SERVER = "192.168.10.18";
    }
}
