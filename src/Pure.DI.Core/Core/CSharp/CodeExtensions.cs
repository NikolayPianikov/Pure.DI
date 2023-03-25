namespace Pure.DI.Core.CSharp;

internal static class CodeExtensions
{
    public const string CannotResolve = "Cannot resolve composition root";
    public const string MethodImplOptions = "[System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)0x300)]";

    public static IEnumerable<Root> GetActualRoots(this IEnumerable<Root> roots) => 
        roots.Where(i => !i.Injection.Type.IsRefLikeType);

    public static string TagToString(this object? tag) => 
        tag switch
        {
            string => $"\"{tag}\"",
            double => $"{tag}D",
            float => $"{tag}F",
            decimal => $"{tag}M",
            uint => $"{tag}U",
            long => $"{tag}L",
            ulong => $"{tag}UL",
            sbyte => $"(sbyte){tag}",
            byte => $"(byte){tag}",
            short => $"(short){tag}",
            ushort => $"(ushort){tag}",
            nint => $"(nint){tag}",
            nuint => $"(nuint){tag}",
            {} => tag.ToString(),
            _ => "null"
        };
}