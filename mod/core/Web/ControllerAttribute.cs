namespace ValheimWebLink.Web;

[MeansImplicitUse]
[PublicAPI]
[AttributeUsage(AttributeTargets.Class)]
public class ControllerAttribute : Attribute
{
    // public bool NeedsAuth { get; } = false;
    // public ControllerAttribute(bool needsAuth = false) => NeedsAuth = needsAuth;
}