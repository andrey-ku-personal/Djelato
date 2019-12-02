namespace Djelato.Common.Shared
{
    public static class RegexExpressions
    {
        public const string NameRgx = @"^[a-zA-Z0-9_ -]+$";
        public const string EmailRgx = @"^(([^<>()\[\]\\.,;:\s@]+(\.[^<>()\[\]\\.,;:\s@]+)*)|(.+))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";
        public const string PasswordRgx = @"^(?=.*[A-Z])(?=.*\d)(.{8,100})$";
        public const string KeyRgx = @"^[0-9]{6}$";
        public const string PhoneRgx = @"[0-9]+";
    }
}
