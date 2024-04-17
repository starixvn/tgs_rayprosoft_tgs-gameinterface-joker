namespace Tgs.GameInterface.Joker.Core;

public interface ISystemClock
{
	DateTimeOffset CurrentTime { get; }
}
