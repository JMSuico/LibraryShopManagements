using System.Net;
using System.Text;

namespace GlassLibraryManagement.Features.Helpers
{
    public static class QRHelper
    {
        public static string GenerateMainId()
            => $"MAIN-{Guid.NewGuid():N}".ToUpperInvariant();

        public static string GenerateCopyId(string mainId, int copyNumber)
            => $"{mainId}-COPY-{copyNumber:000}";

        public static string GenerateQRCodeId(string copyId)
        {
            var safeCopyId = WebUtility.HtmlEncode(copyId);
            var svg = $"<svg xmlns='http://www.w3.org/2000/svg' width='240' height='240' viewBox='0 0 240 240'><rect width='240' height='240' rx='16' fill='white'/><rect x='20' y='20' width='200' height='200' rx='12' fill='black' opacity='0.08'/><text x='120' y='112' text-anchor='middle' font-family='Arial' font-size='18' fill='black'>Glass Library</text><text x='120' y='142' text-anchor='middle' font-family='Arial' font-size='12' fill='black'>{safeCopyId}</text></svg>";
            return $"data:image/svg+xml;base64,{Convert.ToBase64String(Encoding.UTF8.GetBytes(svg))}";
        }
    }
}