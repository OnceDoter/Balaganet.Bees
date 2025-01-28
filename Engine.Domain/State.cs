using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

namespace Engine.Field;

public record State : IStateMachine
{
    public byte Id { get; set; } = 1;
    public bool IsWorking { get; set; }
    public ushort Day { get; set; } = 0;
    public DayStateType DayState { get; set; } = DayStateType.Morning;
    public SeasonStateType SeasonState { get; set; } = SeasonStateType.Spring;
    
    public enum DayStateType : byte
    {
        Morning = 1,
        Afternoon = 2,
        Evening = 3,
        Night = 4,
    }
    
    public enum SeasonStateType : byte
    {
        Spring = 1,
        Summer = 2,
        Autumn = 3,
        Winter = 4,
    }

    public void Stop()
    {
        IsWorking = false;
    }

    public void Start()
    {
        IsWorking = true;
    }

    public void Invoke()
    {
        this.DayState = this.DayState switch
        {
            DayStateType.Morning => DayStateType.Afternoon,
            DayStateType.Afternoon => DayStateType.Evening,
            DayStateType.Evening => DayStateType.Night,
            DayStateType.Night => DayStateType.Morning,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (this.DayState == DayStateType.Morning)
        {
            this.Day++;
        }

        if (this.Day % 50 == 0)
        {
            this.SeasonState = this.SeasonState switch
            {
                SeasonStateType.Spring => SeasonStateType.Summer,
                SeasonStateType.Summer => SeasonStateType.Autumn,
                SeasonStateType.Autumn => SeasonStateType.Winter,
                SeasonStateType.Winter => SeasonStateType.Spring,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}