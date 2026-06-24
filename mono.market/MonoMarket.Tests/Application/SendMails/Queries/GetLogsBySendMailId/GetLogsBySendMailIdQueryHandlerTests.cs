using Moq;
using MonoMarket.Application.SendMails.Dtos;
using MonoMarket.Application.SendMails.Queries.GetLogsBySendMailId;
using MonoMarket.Application.SendMails.Repositories;
using MonoMarket.Domain.Entities;

namespace MonoMarket.Tests.Application.SendMails.Queries.GetLogsBySendMailId;

/// <summary>
/// Pruebas del handler de GetLogsBySendMailIdQuery.
/// </summary>
public class GetLogsBySendMailIdQueryHandlerTests
{
    private readonly Mock<ISendMailLogRepository> _repositoryMock = new();

    /// <summary>
    /// Verifica que el handler mapee a DTO todos los intentos devueltos por el repositorio.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsMappedDtoList()
    {
        List<SendMailLog> logs =
        [
            new SendMailLog { SendMailId = "sendmail-1", AttemptNumber = 1 },
            new SendMailLog { SendMailId = "sendmail-1", AttemptNumber = 2 },
        ];
        _repositoryMock
            .Setup(r => r.GetBySendMailIdAsync("sendmail-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(logs);
        GetLogsBySendMailIdQueryHandler handler = new(_repositoryMock.Object);

        List<SendMailLogDto> result = await handler.Handle(new GetLogsBySendMailIdQuery("sendmail-1"), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, dto => dto.AttemptNumber == 1);
        Assert.Contains(result, dto => dto.AttemptNumber == 2);
    }

    /// <summary>
    /// Verifica que el handler reenvíe el Id de la cabecera al repositorio.
    /// </summary>
    [Fact]
    public async Task Handle_ForwardsSendMailIdToRepository()
    {
        _repositoryMock
            .Setup(r => r.GetBySendMailIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        GetLogsBySendMailIdQueryHandler handler = new(_repositoryMock.Object);

        await handler.Handle(new GetLogsBySendMailIdQuery("sendmail-1"), CancellationToken.None);

        _repositoryMock.Verify(r => r.GetBySendMailIdAsync("sendmail-1", It.IsAny<CancellationToken>()), Times.Once);
    }
}
