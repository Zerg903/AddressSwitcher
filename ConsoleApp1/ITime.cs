using System;

namespace ConsoleApp1
{
  public interface ITime
  {
    DateTimeOffset Now { get; }
  }

  // Implementations
  // ----------------------------------------------------------------

  public sealed class SystemTime : ITime
  {
    public DateTimeOffset Now => DateTimeOffset.Now;
  }

  public sealed class FixedTime : ITime
  {
    public FixedTime(DateTimeOffset now) => Now = now;

    public DateTimeOffset Now { get; private set; }

    public void Shift(TimeSpan span) => Now += span;
  }
}