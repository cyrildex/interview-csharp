﻿using FluentValidation;
using HashidsNet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UrlShortenerService.Application.Common.Exceptions;
using UrlShortenerService.Application.Common.Interfaces;

namespace UrlShortenerService.Application.Url.Commands;

public record RedirectToUrlCommand : IRequest<string>
{
    public string Id { get; init; } = default!;
}

public class RedirectToUrlCommandValidator : AbstractValidator<RedirectToUrlCommand>
{
    public RedirectToUrlCommandValidator()
    {
        _ = RuleFor(v => v.Id)
          .NotEmpty()
          .WithMessage("Id is required.");
    }
}

public class RedirectToUrlCommandHandler : IRequestHandler<RedirectToUrlCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IHashids _hashids;

    public RedirectToUrlCommandHandler(IApplicationDbContext context, IHashids hashids)
    {
        _context = context;
        _hashids = hashids;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<string> Handle(RedirectToUrlCommand request, CancellationToken cancellationToken)
    {
        var shortCodeDbObject = await _context.Urls.FirstOrDefaultAsync(obj => obj.ShortCode == request.Id);

        if (shortCodeDbObject == null)
        {
            throw new NotFoundException("This entry does not exist!");
        }

        if (!Uri.TryCreate(shortCodeDbObject.OriginalUrl, UriKind.Absolute, out _))
        {
            throw new NotFoundException("Bad Redirect URL!  Please try recreate your short URL!");
        }

        return shortCodeDbObject.OriginalUrl;
    }
}
