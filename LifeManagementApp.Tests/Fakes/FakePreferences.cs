namespace LifeManagementApp.Tests.Fakes;

// Preferences kept in a dictionary, so the tests need no device storage.
public class FakePreferences : IPreferences
{
    private readonly Dictionary<string, object?> values = new();

    public bool ContainsKey(string key, string? sharedName = null) => values.ContainsKey(key);

    public void Remove(string key, string? sharedName = null) => values.Remove(key);

    public void Clear(string? sharedName = null) => values.Clear();

    public void Set<T>(string key, T value, string? sharedName = null) => values[key] = value;

    public T Get<T>(string key, T defaultValue, string? sharedName = null)
    {
        return values.TryGetValue(key, out object? value) ? (T)value! : defaultValue;
    }
}
