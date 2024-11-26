using AutoFixture;
using FluentAssertions;
using LiftManager;
using LiftManager.Core;
using LiftManager.Domain;
using Microsoft.Extensions.Logging;
using Moq;

namespace LiftManagerTests;

public class Operator_Tests : IClassFixture<Operator>
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository> _repositoryMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly Mock<IAppSettings> _appSettingsMock;
    private readonly Operator _operator;

    public Operator_Tests()
    {
        _fixture = new Fixture();

        // Mock the dependencies
        _repositoryMock = _fixture.Freeze<Mock<IRepository>>();
        _loggerMock = _fixture.Freeze<Mock<ILogger>>();
        _appSettingsMock = _fixture.Freeze<Mock<IAppSettings>>();

        // Setup IAppSettings mock to return a valid number of floors
        _appSettingsMock.Setup(a => a.NumberOfFloors).Returns(10);

        // Create the Operator instance
        _operator = new Operator(_repositoryMock.Object, _loggerMock.Object, _appSettingsMock.Object);
    }


    [Fact]
    public async Task LiftToFloor_Should_Log_InvalidDestination_If_InvalidFloor()
    {
        // Arrange
        int invalidDestinationFloor = -1;

        // Act
        var result = await _operator.LiftToFloor(invalidDestinationFloor);

        // Assert
        result.Should().BeFalse();
        _loggerMock.Verify(logger => logger.LogInformation(It.Is<string>(s => s.Contains("Invalid Destinatio Floor"))), Times.Once);
    }

    [Fact]
    public async Task LiftToFloor_Should_Return_True_If_LiftPosition_Is_Already_At_Destination()
    {
        // Arrange
        int destinationFloor = 5;
        var liftPosition = new LiftPosition(Id: 1, 5, 5);
        _repositoryMock.Setup(r => r.GetLiftPosition()).ReturnsAsync(liftPosition);

        // Act
        var result = await _operator.LiftToFloor(destinationFloor);

        // Assert
        result.Should().BeTrue();
        _loggerMock.Verify(logger => logger.LogTrace(It.Is<string>(s => s.Contains("Moving to floor"))), Times.Once);
    }

    [Fact]
    public async Task LiftToFloor_Should_Save_LiftPosition_If_Position_Is_Different()
    {
        // Arrange
        int destinationFloor = 7;
        var liftPosition = new LiftPosition(1, 3, 3);
        _repositoryMock.Setup(r => r.GetLiftPosition()).ReturnsAsync(liftPosition);
        _repositoryMock.Setup(r => r.SaveLiftPosition(It.IsAny<LiftPosition>())).ReturnsAsync(true);

        // Act
        var result = await _operator.LiftToFloor(destinationFloor);

        // Assert
        result.Should().BeTrue();
        _repositoryMock.Verify(r => r.SaveLiftPosition(It.Is<LiftPosition>(lp => lp.Destination == destinationFloor)), Times.Once);
    }

    [Fact]
    public async Task Stop_Should_Return_True()
    {
        // Act
        var result = await _operator.Stop();

        // Assert
        result.Should().BeTrue();
        _loggerMock.Verify(logger => logger.LogTrace("Stopping lift"), Times.Once);
    }

    [Fact]
    public void Dispose_Should_Cleanup_Resources()
    {
        // Act
        _operator.Dispose();

        // Assert
        _repositoryMock.Verify(r => r.Dispose(), Times.Once);
    }

    [Fact]
    public async Task GetLiftPosition_Should_Return_Null_If_Exception_Occurs()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetLiftPosition()).ThrowsAsync(new Exception("DB Error"));

        // Act
        var result = await _operator.LiftToFloor(1);

        // Assert
        result.Should().BeFalse();
        _loggerMock.Verify(logger => logger.LogError(It.IsAny<Exception>(), It.Is<string>(s => s.Contains("Issue while retrieving the saved lift position"))), Times.Once);
    }
}
