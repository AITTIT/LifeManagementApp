namespace LifeManagementApp.Services;

// All preference keys in one place - never type a key string anywhere else.
public static class SettingsKeys
{
    public const string Theme = "theme";        // string: "System" | "Light" | "Dark"
    public const string DefaultCategory = "default_category"; // string
    public const string DefaultPriority = "default_priority"; // int 1-5
    public const string SortOrder = "sort_order";       // string: "Priority" | "Due Date"
}