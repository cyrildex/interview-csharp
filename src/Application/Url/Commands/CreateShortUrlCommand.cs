using System.Text;
using FluentValidation;
using HashidsNet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using UrlShortenerService.Application.Common.Interfaces;

namespace UrlShortenerService.Application.Url.Commands;

public record CreateShortUrlCommand : IRequest<string>
{
    public string Url { get; init; } = default!;
    public string HostUrl { get; init; } = default!;
}

public class CreateShortUrlCommandValidator : AbstractValidator<CreateShortUrlCommand>
{
    public CreateShortUrlCommandValidator()
    {
        _ = RuleFor(v => v.Url)
          .NotEmpty()
          .WithMessage("Url is required.");

        _ = RuleFor(v => v.Url)
            .Custom((url, context) =>
            {
                if (!Uri.TryCreate(url, UriKind.Absolute, out _))
                {
                    context.AddFailure("Bad URL Provided");
                }
            });

        _ = RuleFor(v => v.HostUrl)
            .Custom((url, context) =>
            {
                if (!Uri.TryCreate(url, UriKind.Absolute, out _))
                {
                    context.AddFailure("Error Handling Host URL");
                }
            });
    }
}

public class CreateShortUrlCommandHandler : IRequestHandler<CreateShortUrlCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IHashids _hashids;
    private readonly Random _random;

    public CreateShortUrlCommandHandler(IApplicationDbContext context, IHashids hashids)
    {
        _context = context;
        _hashids = hashids;
        _random = new Random();
    }

    public async Task<string> Handle(CreateShortUrlCommand request, CancellationToken cancellationToken)
    {
        string shortCode = CreateShortCode(8);
        if (!await _context.Urls.AnyAsync(s => s.ShortCode == shortCode))
        {
            string shortUrl = request.HostUrl + "/" + shortCode;
            _ = await _context.Urls.AddAsync(new Domain.Entities.Url()
            {
                OriginalUrl = request.Url,
                ShortCode = shortCode,
                ShortUrl = shortUrl
            });
            _ = _context.SaveChangesAsync(cancellationToken);
            return shortUrl;
        }
        else
        {
            return request.Url.Substring(0, 16);
        }
    }

    private string CreateShortCode(int length)
    {
        string urlSafeChars = "abcdefghijklmnopqrstuvwxwzABCDEFGHIJKLMNOPQRSTUVWXWZ0123456789-_.~";
        var result = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            _ = result.Append(urlSafeChars[_random.Next(urlSafeChars.Length) - 1]);
        }
        return result.ToString();
    }
}
