using System;

namespace MedioClinicCustomizations.Cookies
{
    public class CookieLevel : IEquatable<CookieLevel>
    {
        public static bool operator ==(CookieLevel a, CookieLevel b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }

            return a.Equals(b);
        }

        public static bool operator !=(CookieLevel a, CookieLevel b)
        {
            if (ReferenceEquals(a, null))
            {
                return !ReferenceEquals(b, null);
            }

            return !a.Equals(b);
        }

        public override bool Equals(object obj) =>
            obj is CookieLevel cookieLevel && Equals(cookieLevel);

        public int Level { get; set; }

        public string Name { get; set; }

        public bool Equals(CookieLevel other) =>
            other != null && other.Level == Level;

        public override int GetHashCode() => Level.GetHashCode();
    }
}
