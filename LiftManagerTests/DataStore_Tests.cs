using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using LiftManager;
using LiftManager.Data;
using LiftManager.Data.Dto;
using LiftManager.Domain.Enums;
using LiteDB;
using Moq;

namespace LiftManagerTests;

public class DataStore_Tests : IDisposable
{
  private readonly IFixture _fixture;
  private readonly Mock<IAppSettings> _appSettingsMock;
  private readonly Mock<LiftManager.Core.ILogger> _loggerMock;
  private DataStore _dataStore;

  public DataStore_Tests()
  {
    _fixture = new Fixture().Customize(new AutoMoqCustomization());
    _appSettingsMock = _fixture.Freeze<Mock<IAppSettings>>();
    _loggerMock = _fixture.Freeze<Mock<LiftManager.Core.ILogger>>();

    _appSettingsMock.Setup(x => x.FilesForBulk).Returns(10);
  }

  [Fact]
  public async Task GetLatest_ShouldReturnLatestLiftPositionDto()
  {
    // Arrange
    CreateNewDb("1");
    // var data = _fixture.CreateMany<LiftPositionDto>(20).OrderByDescending(x => x.RequestedDate).ToList();
    // Need Id = 0 so LiteDB generates it internally
    var data = _fixture.Build<LiftPositionDto>()
      .With(x => x.Id, 0)
      .With(x => x.RequestedDate, DateTime.Now)
      .With(x => x.OperationType, OperationType.Inside)
      .CreateMany(20);
    var expected = data.Last();

    foreach (var d in data)
    {
      var response = await _dataStore.Save(d);
    }

    // Act
    var actual = await _dataStore.GetLatest();

    // Assert
    actual.Id.Should().NotBe(0);
    actual.SourceFloor.Should().Be(expected.SourceFloor);
    actual.DestinationFloor.Should().Be(expected.DestinationFloor);
    _loggerMock.Verify(x => x.Information(It.IsAny<string>(), It.IsAny<object>()), Times.AtLeast(20));
  }

  [Fact]
  public async Task Save_ShouldEnqueueIfUnderThreshold()
  {
    CreateNewDb("2");
    // Arrange
    var dto = _fixture.Create<LiftPositionDto>();

    // Act
    var result = await _dataStore.Save(dto);

    // Assert
    result.Should().BeTrue();
    _loggerMock.Verify(x => x.Information(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
  }

  [Fact]
  public async Task Save_ShouldBulkSaveWhenThresholdReached()
  {
    // Arrange
    CreateNewDb("3");
    var dtos = _fixture.CreateMany<LiftPositionDto>(10);
    foreach (var dto in dtos)
    {
      await _dataStore.Save(dto);
    }

    // Act
    var result = await _dataStore.Save(_fixture.Create<LiftPositionDto>());

    // Assert
    result.Should().BeTrue();
    _loggerMock.Verify(x => x.Information(It.Is<string>(s => s.Contains("Inserting bulk")), It.IsAny<object>()), Times.AtLeast(1));
  }

  [Fact]
  public void CreateLiteDatabase_ShouldInitializeDatabaseAndEnsureIndices()
  {
    // Arrange
    CreateNewDb("4");

    // Act
    var result = _dataStore.GetType()
        .GetMethod("CreateLiteDatabase", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        ?.Invoke(_dataStore, null) as LiteDatabase;

    // Assert
    result.Should().NotBeNull();
  }

  [Fact]
  public async Task GetLatest_ShouldGetRightLatestRequest_EvenIfNoInDb_ButInCache()
  {
    CreateNewDb("5");
    var actualLiftRequests = _fixture.Build<LiftPositionDto>()
    .With(x => x.Id, 0)
    .With(x => x.RequestedDate, DateTime.Now)
    .CreateMany(88);

    // Act
    foreach (var d in actualLiftRequests)
    {
      await _dataStore.Save(d);
    }

    var result = await _dataStore.GetLatest();

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().Be(actualLiftRequests.LastOrDefault().Id);
    result.SourceFloor.Should().Be(actualLiftRequests.LastOrDefault().SourceFloor);
    result.DestinationFloor.Should().Be(actualLiftRequests.LastOrDefault().DestinationFloor);
  }

  public void Dispose()
  {
    _dataStore.DeleteAll();
    _dataStore.Clear();
    _dataStore.Dispose();
    _dataStore = null;
    // TODO: Clean out db file securely, below throws a IOException because of file being used by another process
    // File.Delete(_appSettingsMock.Object.DatabaseDirectory);
  }

  /// <summary>
  /// Need to create a fresh data base to test avoid test data infection
  /// </summary>
  /// <param name="dbname"></param>
  private void CreateNewDb(string dbname)
  {
    var databaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), $"TestDatabase{dbname}.db");
    _appSettingsMock.Setup(x => x.DatabaseDirectory).Returns(databaseDirectory);

    _dataStore = new DataStore(_appSettingsMock.Object, _loggerMock.Object);
  }
}
