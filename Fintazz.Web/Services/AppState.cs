namespace Fintazz.Web.Services;

public class AppState
{
    public Guid? HouseHoldId { get; private set; }
    public string HouseHoldName { get; private set; } = string.Empty;
    public bool HasHouseHold => HouseHoldId.HasValue;

    public event Action? OnChange;

    public void SetHouseHold(Guid id, string name)
    {
        HouseHoldId = id;
        HouseHoldName = name;
        OnChange?.Invoke();
    }

    public void Clear()
    {
        HouseHoldId = null;
        HouseHoldName = string.Empty;
        OnChange?.Invoke();
    }
}
