using HashidsNet;
using UrlShortenerService.Application.Url.Commands;
using Moq;
using UrlShortenerService.Application.Common.Interfaces;
using UrlShortenerService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System;

namespace Tests;

public class TestShortUrlCommands
{
    private readonly Mock<Hashids> _hashIds;
    private readonly Mock<IApplicationDbContext> _context;
    private readonly string endpoint = "http://localhost:7072/u";
    public TestShortUrlCommands()
    {
        _hashIds = new Mock<Hashids>("testsalt123", Hashids.MIN_ALPHABET_LENGTH, Hashids.DEFAULT_ALPHABET + "äöüß", Hashids.DEFAULT_SEPS);
       // _hashIds.Setup(h => h.Encode(It.IsAny<int[]>()))
       //.Returns((int[] numbers) => 123);

       // _hashIds.Setup(h => h.Decode(It.IsAny<string>()))
       //        .Returns((string hash) => "abcd123");
        _context = new Mock<IApplicationDbContext>();
        //_context.Setup(c => c.Urls.Add(It.IsAny<Url>()))
        //        .Returns((Url url) => url);
        _context.Setup(c => c.Urls).Returns(new Mock<DbSet<Url>>().Object);
    }
    /// <summary>
    /// add a test for the <see cref="CreateShortUrlCommandHandler"/> class.
    /// </summary>
    [Fact]
    public void HandleTest()
    {
        // Arrange
        CreateShortUrlCommand shortUrlCommand = new CreateShortUrlCommand()
        {
            Url = "https://www.google.com"
        };

        // Act
        CreateShortUrlCommandHandler handler = new CreateShortUrlCommandHandler(context: _context.Object, hashids: _hashIds.Object);
        string result = handler.Handle(shortUrlCommand, CancellationToken.None).Result;
        Task<Url>? url = _context.Object.Urls.SingleOrDefaultAsync(u => u.OriginalUrl == shortUrlCommand.Url);
        string expectedResult = endpoint + _hashIds.Object.EncodeLong(url?.Result.Id ?? 0);

        //Assert
        Assert.Equal(expectedResult, result);

    }
}
