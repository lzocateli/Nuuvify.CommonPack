using System;
using System.Reflection;
using Microsoft.OpenApi.Models;

namespace Nuuvify.CommonPack.OpenApi
{
    internal class SwaggerInfoModel
    {


        public string AppName { get; private set; }
        public string AppVersion { get; private set; }
        public string BuildVersion { get; private set; }
        public string VersionName { get; set; }
        public string DeveloperName { get; private set; }
        public string DeveloperEmail { get; private set; }
        public string LicenseType { get; private set; }
        public Uri UrlLicenseValid { get; private set; }
        public Uri UrlTermsValid { get; private set; }
        public Uri UrlAppValid { get; private set; }



        public SwaggerInfoModel(string developerName, string developerEmail, string licenseType)
        {
            DeveloperName=developerName;
            DeveloperEmail = developerEmail;
            LicenseType = licenseType;


            AppName = Assembly.GetEntryAssembly().GetName().Name?
                                .Replace(".WebApi", "");

            BuildVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            AppVersion = $"{Assembly.GetEntryAssembly().GetName().Version.Major}." +
                            $"{Assembly.GetEntryAssembly().GetName().Version.Minor}." +
                            $"{Assembly.GetEntryAssembly().GetName().Version.Build}";
            
        }



        public void DefineUrls(string urlApp, string urlRepository, string urlTermService, string urlLicense)
        {

            if (!Uri.TryCreate(urlApp, UriKind.RelativeOrAbsolute, out Uri UrlAppValid))
            {
                Uri.TryCreate(urlRepository, UriKind.RelativeOrAbsolute, out UrlAppValid);
            }
            Uri.TryCreate(urlTermService, UriKind.RelativeOrAbsolute, out Uri UrlTermsValid);
            Uri.TryCreate(urlLicense, UriKind.RelativeOrAbsolute, out Uri UrlLicenseValid);


        }

        public OpenApiInfo CreateInfoForApiVersion()
        {


            var info = new OpenApiInfo
            {
                Title = AppName,
                Version = VersionName,
                Description = $@"Api documentation

# Application version #
{AppVersion}

# Build number #
{BuildVersion}",

                Contact = new OpenApiContact
                {
                    Name = DeveloperName,
                    Email = DeveloperEmail,
                    Url = UrlAppValid,
                },
                TermsOfService = UrlTermsValid,
                License = new OpenApiLicense
                {
                    Name = LicenseType,
                    Url = UrlLicenseValid
                },
            };

            return info;
        } 
    }
}