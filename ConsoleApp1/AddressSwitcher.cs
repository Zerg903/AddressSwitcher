using System;

namespace ConsoleApp1
{
  public class AddressSwitcher
  {
    private readonly string _primaryUrl;
    private readonly string _secondaryUrl;
    private readonly object _sync;
    private readonly ITime _time;
    private readonly TimeSpan _timeframe;

    private ImmutableState _state;

    public AddressSwitcher(string primaryUrl, string secondaryUrl, TimeSpan timeframe, ITime time)
    {
      _primaryUrl = primaryUrl;
      _secondaryUrl = secondaryUrl;
      _timeframe = timeframe;
      _time = time;

      _sync = new object();
      _state = ImmutableState.Primary(this);
    }

    public IState GetState()
    {
      lock (_sync)
      {
        var state = _state;

        if (state.ValidUntil > _time.Now)
          return state;

        state = _state = ImmutableState.Primary(this);

        return state;
      }
    }

    private void Toggle(bool primary)
    {
      lock (_sync)
      {
        _state = primary
          ? ImmutableState.Primary(this)
          : ImmutableState.Secondary(this);
      }
    }

    /// IState
    /// -------------------------------------

    public interface IState
    {
      string Url { get; }
      DateTimeOffset ValidUntil { get; }
      void Toggle();
    }

    /// ImmutableState
    /// -------------------------------------

    protected class ImmutableState : IState
    {
      private readonly bool _primary;
      private readonly AddressSwitcher _switcher;

      public ImmutableState(AddressSwitcher switcher, bool primary, DateTimeOffset validUntil)
      {
        _switcher = switcher;
        _primary = primary;

        ValidUntil = validUntil;
      }

      public DateTimeOffset ValidUntil { get; }
      public string Url => _primary ? _switcher._primaryUrl : _switcher._secondaryUrl;
      public void Toggle() => _switcher.Toggle(!_primary);

      public static ImmutableState Primary(AddressSwitcher switcher) =>
        new ImmutableState(switcher, true, DateTimeOffset.MaxValue);

      public static ImmutableState Secondary(AddressSwitcher switcher) =>
        new ImmutableState(switcher, false, switcher._time.Now + switcher._timeframe);
    }
  }
}