﻿namespace Gate
{
    internal static class OwinConstants
    {
        public const string Version = "owin.Version";
        public const string RequestScheme = "owin.RequestScheme";
        public const string RequestMethod = "owin.RequestMethod";
        public const string RequestPathBase = "owin.RequestPathBase";
        public const string RequestPath = "owin.RequestPath";
        public const string RequestQueryString = "owin.RequestQueryString";
        public const string RequestHeaders = "owin.RequestHeaders";
        public const string RequestBody = "owin.RequestBody";

        public const Owin.TempEnum CompletedSynchronously = Owin.TempEnum.CompletedSynchronously; // = true;
        public const Owin.TempEnum CompletingAsynchronously = Owin.TempEnum.CompletingAsynchronously; // = false;
    }
}
