using EPiServer.Core;

namespace Xunit
{
    public partial class Assert
    {
        public static void Equal(ContentReference expected, ContentReference actual)
        {
            Equal(expected, actual, ContentReferenceComparer.Default);
        }

        public static void Equal(ContentReference expected, ContentReference actual, bool ignoreVersion)
        {
            Equal(expected, actual, ignoreVersion ? ContentReferenceComparer.IgnoreVersion : ContentReferenceComparer.Default);
        }

        public static void NotEqual(ContentReference expected, ContentReference actual)
        {
            NotEqual(expected, actual, ContentReferenceComparer.Default);
        }

        public static void NotEqual(ContentReference expected, ContentReference actual, bool ignoreVersion)
        {
            NotEqual(expected, actual, ignoreVersion ? ContentReferenceComparer.IgnoreVersion : ContentReferenceComparer.Default);
        }

        /// <summary>
        /// Verifies that two <see cref="ContentReference"/> objects are referencing different versions of the same content.
        /// </summary>
        public static void DifferentVersions(ContentReference expected, ContentReference actual)
        {
            Equal(expected, actual, ContentReferenceComparer.IgnoreVersion);
            NotEqual(expected, actual, ContentReferenceComparer.Default);
        }

        public static void NullOrEmpty(ContentReference actual)
        {
            True(ContentReference.IsNullOrEmpty(actual));
        }

    }
}
