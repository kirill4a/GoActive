using System.Text;

namespace GoActive.Tests.Common;

public static class StreamExtensions
{
    /// <summary>
    /// Creates a MemoryStream from a string.
    /// The string is converted to bytes using the specified encoding (default is UTF8).
    /// </summary>
    /// <param name="content">The string to be converted to stream.</param>
    /// <param name="encoding">Character encoding (default is UTF-8).</param>
    public static MemoryStream AsMemoryStream(this string content, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var bytes = encoding.GetBytes(content);
        return new MemoryStream(bytes);
    }
}
