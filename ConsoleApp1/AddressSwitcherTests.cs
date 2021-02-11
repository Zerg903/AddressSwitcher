using System;
using FluentAssertions;
using Xunit;

namespace ConsoleApp1
{
  public class AddressSwitcherTests
  {
    private const string PrimaryUrl = "http://www.primary.com";
    private const string SecondaryUrl = "http://www.secondary.com";
    private static readonly TimeSpan Timeframe = TimeSpan.FromMinutes(5);

    [Fact]
    public void Test01_После_инициализации_получаем_первичный_адрес()
    {
      // arrange

      var now = new DateTime(2020, 02, 12);
      var time = new FixedTime(now);
      var switcher = new AddressSwitcher(PrimaryUrl, SecondaryUrl, Timeframe, time);

      // act 

      var state = switcher.GetState();

      // assert

      state.Url.Should().BeEquivalentTo(PrimaryUrl);
      state.ValidUntil.Should().Be(DateTimeOffset.MaxValue);
    }

    [Fact]
    public void Test02_Переключение_на_вторичный_адрес_происходит_корректно()
    {
      // arrange

      var now = new DateTime(2020, 02, 12);
      var time = new FixedTime(now);
      var switcher = new AddressSwitcher(PrimaryUrl, SecondaryUrl, Timeframe, time);

      // act 

      var state = switcher.GetState();
      state.Toggle(); // переключили на SecondaryUrl

      state = switcher.GetState();

      // assert

      state.Url.Should().BeEquivalentTo(SecondaryUrl);
      state.ValidUntil.Should().Be(now + Timeframe);
    }

    [Fact]
    public void Test03_Переключение_со_вторичного_адреса_на_первичный_происходит_корректно()
    {
      // arrange

      var now = new DateTime(2020, 02, 12);
      var time = new FixedTime(now);
      var switcher = new AddressSwitcher(PrimaryUrl, SecondaryUrl, Timeframe, time);

      // act 

      var state = switcher.GetState();
      state.Toggle(); // переключили на SecondaryUrl

      state = switcher.GetState();
      state.Toggle(); // переключили на PrimaryUrl

      state = switcher.GetState();

      // assert

      state.Url.Should().BeEquivalentTo(PrimaryUrl);
      state.ValidUntil.Should().Be(DateTimeOffset.MaxValue);
    }

    [Fact]
    public void Test04_Переключение_на_первичный_адрес_через_заданный_интервал_происходит_корректно()
    {
      // arrange

      var now = new DateTime(2020, 02, 12);
      var time = new FixedTime(now);
      var switcher = new AddressSwitcher(PrimaryUrl, SecondaryUrl, Timeframe, time);

      // act 

      var state = switcher.GetState();
      state.Toggle(); // переключили на SecondaryUrl

      time.Shift(Timeframe + TimeSpan.FromSeconds(1));

      state = switcher.GetState();

      // assert

      state.Url.Should().BeEquivalentTo(PrimaryUrl);
      state.ValidUntil.Should().Be(DateTimeOffset.MaxValue);
    }
  }
}