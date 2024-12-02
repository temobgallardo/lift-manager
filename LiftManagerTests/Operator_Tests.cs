using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using LiftManager;
using LiftManager.Core;
using LiftManager.Domain;
using LiftManager.Domain.Data;
using LiftManager.Domain.Enums;
using Moq;

namespace LiftManagerTests;

public class Operator_Tests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository> _repositoryMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly Mock<IAppSettings> _appSettingsMock;
    private readonly Operator _operator;

    public Operator_Tests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization()); ;

        // Mock the dependencies
        _repositoryMock = _fixture.Freeze<Mock<IRepository>>();
        _loggerMock = _fixture.Freeze<Mock<ILogger>>();
        _appSettingsMock = _fixture.Freeze<Mock<IAppSettings>>();

        // Setup IAppSettings mock to return a valid number of floors
        _appSettingsMock.Setup(a => a.NumberOfFloors).Returns(10);
        _appSettingsMock.Setup(a => a.InitialFloor).Returns(0);
        _appSettingsMock.Setup(a => a.TravelSimulationTime).Returns(10);

        // Create the Operator instance
        _operator = new Operator(_repositoryMock.Object, _loggerMock.Object, _appSettingsMock.Object);
    }

    [Theory]
    [InlineData(-2)]
    [InlineData(-1)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task LiftToFloor_Should_Log_InvalidDestination_If_InvalidFloor(int invalidDestinationFloor)
    {
        // Arrange
        _repositoryMock.Setup(a => a.GetCurrentLiftPosition()).ReturnsAsync(_fixture.Create<LiftPosition>());

        // Act
        var result = await _operator.LiftToFloor(invalidDestinationFloor);

        // Assert
        result.Should().BeFalse();
        _loggerMock.Verify(logger => logger.Error(It.IsAny<Exception>(), It.Is<string>(s => s.Contains("Invalid Destinatio Floor"))), Times.Once);
    }

    [Fact]
    public async Task LiftToFloor_Should_Return_False_If_LiftPosition_Is_Already_At_Destination()
    {
        // Arrange
        int destinationFloor = 5;
        var liftPosition = new LiftPosition(DateTime.Now, 5, 5, OperationType.None);
        _repositoryMock.Setup(r => r.GetCurrentLiftPosition()).ReturnsAsync(liftPosition);

        // Act
        var result = await _operator.LiftToFloor(destinationFloor);

        // Assert
        result.Should().BeFalse();
        _loggerMock.Verify(logger => logger.Debug(It.Is<string>(s => s.Contains("Lift stays in same floor"))), Times.Once);
    }

    [Fact]
    public async Task LiftToFloor_Should_Save_LiftPosition_If_Position_Is_Different()
    {
        // Arrange
        int destinationFloor = 7;
        var liftPosition = new LiftPosition(DateTime.Now, 3, 3, OperationType.None);
        _repositoryMock.Setup(r => r.GetCurrentLiftPosition()).ReturnsAsync(liftPosition);
        _repositoryMock.Setup(r => r.SaveLiftPosition(It.IsAny<LiftPosition>())).ReturnsAsync(true);

        // Act
        var result = await _operator.LiftToFloor(destinationFloor);

        // Assert
        result.Should().BeTrue();
        _repositoryMock.Verify(r => r.SaveLiftPosition(It.Is<LiftPosition>(lp => lp.DestinationFloor == destinationFloor)), Times.Once);
    }

    [Fact]
    public async Task Stop_Should_Return_True()
    {
        // Act
        var result = await _operator.Stop();

        // Assert
        result.Should().BeTrue();
        _loggerMock.Verify(logger => logger.Debug("Stopping lift"), Times.Once);
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
        _repositoryMock.Setup(r => r.GetCurrentLiftPosition()).ThrowsAsync(new Exception("DB Error"));

        // Act
        var result = await _operator.LiftToFloor(1);

        // Assert
        result.Should().BeFalse();
        _loggerMock.Verify(logger => logger.Error(It.IsAny<Exception>(), It.Is<string>(s => s.Contains("Issue while retrieving the saved lift position"))), Times.Once);
    }

    [Theory]
    [InlineData(0, 0, 10)]
    [InlineData(0, 1, 2)]
    [InlineData(1, 2, 5)]
    [InlineData(2, 10, 6)]
    [InlineData(2, 10, 1)]
    [InlineData(10, 2, 7)]
    [InlineData(2, 5, 1)]
    [InlineData(2, 5, 5)]
    [InlineData(0, 0, 0)]
    public async Task RequestLiftToFloor_Should_Return_True_If_Lift_Move_To_Valid_Floor(int actualSource, int currentFloor, int floorWhereLiftWasRequested)
    {
        // Arrange
        var numberOfFloorsToMove = Math.Abs(currentFloor - floorWhereLiftWasRequested);
        _repositoryMock.Setup(x => x.GetCurrentLiftPosition())
        .ReturnsAsync(new LiftPosition(DateTime.Now, actualSource, currentFloor, OperationType.Outside));
        _repositoryMock.Setup(x => x.SaveLiftPosition(It.IsAny<LiftPosition>())).ReturnsAsync(true);

        // Act
        var result = await _operator.RequestLiftToFloor(floorWhereLiftWasRequested);

        // Assert
        result.Should().BeTrue();
        _loggerMock.Verify(l => l.Information(It.Is<string>(s => s.Contains("Lift on floor number"))), Times.Exactly(numberOfFloorsToMove));
    }

    [Theory]
    [InlineData(100)]
    [InlineData(-2)]
    [InlineData(-5)]
    [InlineData(60)]
    [InlineData(17)]
    [InlineData(-7)]
    [InlineData(-1)]
    public async Task RequestLiftToFloor_Should_Return_False_If_Lift_Move_To_Invalid_Floor(int floorWhereLiftWasRequested)
    {
        // Arrange

        // Act
        var result = await _operator.RequestLiftToFloor(floorWhereLiftWasRequested);

        // Assert
        result.Should().BeFalse();
        _loggerMock.Verify(l => l.Information(It.Is<string>(s => s.Contains("is out of range"))), Times.Once);
    }
}
