﻿using MediatR;
using UrlShortenerService.Api.Endpoints.Url.Requests;
using UrlShortenerService.Application.Url.Commands;
using IMapper = AutoMapper.IMapper;

namespace UrlShortenerService.Api.Endpoints.Url;

public class CreateShortUrlSummary : Summary<CreateShortUrlEndpoint>
{
    public CreateShortUrlSummary()
    {
        Summary = "Create short url from provided url";
        Description =
            "This endpoint will create a short url from provided original url.";
        Response(500, "Internal server error.");
    }
}

public class CreateShortUrlEndpoint : BaseEndpoint<CreateShortUrlRequest>
{
    public CreateShortUrlEndpoint(ISender mediator, IMapper mapper)
        : base(mediator, mapper) { }

    public override void Configure()
    {
        base.Configure();
        Post("u");
        AllowAnonymous();
        Description(
            d => d.WithTags("Url")
        );
        Summary(new CreateShortUrlSummary());
    }
    //This endpoint will create a short url from provided original url.
    public override async Task HandleAsync(CreateShortUrlRequest req, CancellationToken ct)
    {
        var result = await Mediator.Send(
            new CreateShortUrlCommand
            {
                Url = req.Url,
                HostUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.Path}"
            },
            ct
        );
        await SendOkAsync(result);
    }


}
