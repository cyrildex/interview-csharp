using UrlShortenerService.Domain.Common;

namespace UrlShortenerService.Domain.Entities;

/// <summary>
/// Url domain entity.
/// </summary>
public class Url : BaseAuditableEntity
{
    #region constructors and destructors

    /// <summary>
    /// Default constructor.
    /// </summary>
    public Url() { }

    #endregion

    #region properties

    /// <summary>
    /// The original url.
    /// </summary>
    public string OriginalUrl { get; set; } = default!;

    /// <summary>
    /// The shortened url.
    /// </summary>
    public string ShortUrl { get; set; } = default!;

    /// <summary>
    /// The short code that is used to redirect to the original url.
    /// </summary>
    public string ShortCode { get; set; } = default!;

    #endregion
}
