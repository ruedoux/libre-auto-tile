using Qwaitumin.SimpleTest;
using Qwaitumin.LibreAutoTile.Tiling.Search;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.Tests.Tiling.Search;

[TestClass]
public class TileMaskCacheTest
{
  [TestMethod]
  public void Constructor_ShouldThrow_WhenCapacityIsZeroOrLess()
  {
    // Then
    Assertions.AssertThrows<ArgumentOutOfRangeException>(
      () => new TileMaskCache(0));
    Assertions.AssertThrows<ArgumentOutOfRangeException>(
      () => new TileMaskCache(-1));
  }

  [TestMethod]
  public void SetAndTryGet_ShouldReplaceSingleEntry_WhenCapacityIsOne()
  {
    // Given
    TileMaskCache cache = new(capacity: 1);
    TileMask key1 = new(top: 1);
    TileMask key2 = new(top: 2);
    TileMask value1 = new(top: 3);
    TileMask value2 = new(top: 4);

    // When
    cache.Set(key1, value1);
    bool foundKey1BeforeEviction = cache.TryGet(key1, out var resultValue1BeforeEviction);
    cache.Set(key2, value2);
    bool foundKey1AfterEviction = cache.TryGet(key1, out var resultValue1AfterEviction);
    bool foundKey2AfterEviction = cache.TryGet(key2, out var resultValue2AfterEviction);

    // Then
    Assertions.AssertTrue(foundKey1BeforeEviction);
    Assertions.AssertEqual(value1, resultValue1BeforeEviction);
    Assertions.AssertFalse(foundKey1AfterEviction);
    Assertions.AssertEqual(default, resultValue1AfterEviction);
    Assertions.AssertTrue(foundKey2AfterEviction);
    Assertions.AssertEqual(value2, resultValue2AfterEviction);
  }

  [TestMethod]
  public void Set_ShouldUpdateExistingKeyAndKeepItCached_WhenSameKeyIsStoredTwice()
  {
    // Given
    TileMaskCache cache = new(capacity: 2);
    TileMask key1 = new(top: 1);
    TileMask key2 = new(top: 2);
    TileMask key3 = new(top: 3);
    TileMask value1 = new(top: 4);
    TileMask updatedValue1 = new(top: 5);
    TileMask value2 = new(top: 6);
    TileMask value3 = new(top: 7);

    // When
    cache.Set(key1, value1);
    cache.Set(key2, value2);
    cache.Set(key1, updatedValue1);
    cache.Set(key3, value3);

    bool foundKey1 = cache.TryGet(key1, out var resultValue1);
    bool foundKey2 = cache.TryGet(key2, out var resultValue2);
    bool foundKey3 = cache.TryGet(key3, out var resultValue3);

    // Then
    Assertions.AssertTrue(foundKey1);
    Assertions.AssertEqual(updatedValue1, resultValue1);
    Assertions.AssertFalse(foundKey2);
    Assertions.AssertEqual(default, resultValue2);
    Assertions.AssertTrue(foundKey3);
    Assertions.AssertEqual(value3, resultValue3);
  }

  [TestMethod]
  public void SetAndTryGet_ShouldKeepMostRecentlyUsedItem_WhenCapacityReachedAndExistingKeyUpdated()
  {
    // Given
    TileMaskCache cache = new(capacity: 2);
    TileMask key1 = new(top: 1);
    TileMask key2 = new(top: 2);
    TileMask key3 = new(top: 3);
    TileMask key4 = new(top: 4);
    TileMask value1 = new(top: 5);
    TileMask updatedValue1 = new(top: 6);
    TileMask value2 = new(top: 7);
    TileMask value3 = new(top: 8);
    TileMask value4 = new(top: 9);

    // When
    cache.Set(key1, value1);
    cache.Set(key2, value2);
    bool foundKey1BeforeEviction = cache.TryGet(key1, out var resultValue1BeforeEviction);
    cache.Set(key3, value3);
    bool foundKey1AfterEviction = cache.TryGet(key1, out var resultValue1AfterEviction);
    bool foundKey2AfterEviction = cache.TryGet(key2, out var resultValue2AfterEviction);
    bool foundKey3AfterEviction = cache.TryGet(key3, out var resultValue3AfterEviction);
    cache.Set(key1, updatedValue1);
    cache.Set(key4, value4);
    bool foundKey1AfterUpdate = cache.TryGet(key1, out var resultValue1AfterUpdate);
    bool foundKey3AfterUpdate = cache.TryGet(key3, out var resultValue3AfterUpdate);
    bool foundKey4AfterUpdate = cache.TryGet(key4, out var resultValue4AfterUpdate);

    // Then
    Assertions.AssertTrue(foundKey1BeforeEviction);
    Assertions.AssertEqual(value1, resultValue1BeforeEviction);
    Assertions.AssertTrue(foundKey1AfterEviction);
    Assertions.AssertEqual(value1, resultValue1AfterEviction);
    Assertions.AssertFalse(foundKey2AfterEviction);
    Assertions.AssertEqual(default, resultValue2AfterEviction);
    Assertions.AssertTrue(foundKey3AfterEviction);
    Assertions.AssertEqual(value3, resultValue3AfterEviction);
    Assertions.AssertTrue(foundKey1AfterUpdate);
    Assertions.AssertEqual(updatedValue1, resultValue1AfterUpdate);
    Assertions.AssertFalse(foundKey3AfterUpdate);
    Assertions.AssertEqual(default, resultValue3AfterUpdate);
    Assertions.AssertTrue(foundKey4AfterUpdate);
    Assertions.AssertEqual(value4, resultValue4AfterUpdate);
  }
}
