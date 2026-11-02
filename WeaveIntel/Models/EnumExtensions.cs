using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WeaveIntel.Models;

public static class EnumExtensions
{
    /// <summary>Returns the [Display(Name)] of an enum value, or its identifier when none is set.</summary>
    public static string DisplayName(this Enum value)
    {
        var name = value.ToString();
        var member = value.GetType().GetMember(name).FirstOrDefault();
        var attribute = member?.GetCustomAttribute<DisplayAttribute>();
        return attribute?.GetName() ?? name;
    }

    /// <summary>CSS class used to colour lifecycle badges.</summary>
    public static string CssClass(this LifecycleStatus status) => status switch
    {
        LifecycleStatus.Concept => "wi-status-concept",
        LifecycleStatus.Development => "wi-status-development",
        LifecycleStatus.Pilot => "wi-status-pilot",
        LifecycleStatus.Live => "wi-status-live",
        LifecycleStatus.Retired => "wi-status-retired",
        _ => "wi-status-concept"
    };
}
