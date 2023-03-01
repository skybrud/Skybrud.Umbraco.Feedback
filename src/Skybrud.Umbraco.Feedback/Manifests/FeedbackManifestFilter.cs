using System.Collections.Generic;
using System.Reflection;
using Umbraco.Cms.Core.Manifest;

namespace Skybrud.Umbraco.Feedback.Manifests {

    /// <inheritdoc />
    public class FeedbackManifestFilter : IManifestFilter {

        /// <inheritdoc />
        public void Filter(List<PackageManifest> manifests) {

            // Initialize a new manifest filter for this package
            PackageManifest manifest = new() {
                PackageName = FeedbackPackage.Name,
                BundleOptions = BundleOptions.Independent,
                Scripts = new[] {
                    $"/App_Plugins/{FeedbackPackage.Alias}/Scripts/Controllers/ContentApp.js",
                    $"/App_Plugins/{FeedbackPackage.Alias}/Scripts/Controllers/ContentAppPage.js",
                    $"/App_Plugins/{FeedbackPackage.Alias}/Scripts/Controllers/SelectStatus.js",
                    $"/App_Plugins/{FeedbackPackage.Alias}/Scripts/Controllers/SelectResponsible.js"
                },
                Stylesheets = new[] {
                    $"/App_Plugins/{FeedbackPackage.Alias}/Styles/Default.css"
                }
            };

            // The "Version" property isn't available in 9.0.0, but it may be in newer releases, so
            // we need to use reflection for setting it's value. Ideally this shouldn't fail, but
            // we might as least add a try/catch to be sure
            try {
                PropertyInfo property = manifest.GetType().GetProperty("Version");
                property?.SetValue(manifest, FeedbackPackage.InformationalVersion);
            } catch {
                // We don't really care about the exception
            }

            manifests.Add(manifest);

        }

    }

}