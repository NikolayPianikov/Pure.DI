// ReSharper disable ClassNeverInstantiated.Global
namespace Pure.DI.Core.Code;

internal sealed class ClassBuilder(
    [Tag(WellknownTag.UsingDeclarationsBuilder)] IBuilder<CompositionCode, CompositionCode> usingDeclarationsBuilder,
    [Tag(WellknownTag.FieldsBuilder)] IBuilder<CompositionCode, CompositionCode> fieldsBuilder,
    [Tag(WellknownTag.ArgFieldsBuilder)] IBuilder<CompositionCode, CompositionCode> argFieldsBuilder,
    [Tag(WellknownTag.ParameterizedConstructorBuilder)] IBuilder<CompositionCode, CompositionCode> parameterizedConstructorBuilder,
    [Tag(WellknownTag.DefaultConstructorBuilder)] IBuilder<CompositionCode, CompositionCode> defaultConstructorBuilder,
    [Tag(WellknownTag.ScopeConstructorBuilder)] IBuilder<CompositionCode, CompositionCode> scopeConstructorBuilder,
    [Tag(WellknownTag.RootMethodsBuilder)] IBuilder<CompositionCode, CompositionCode> rootPropertiesBuilder,
    [Tag(WellknownTag.ApiMembersBuilder)] IBuilder<CompositionCode, CompositionCode> apiMembersBuilder,
    [Tag(WellknownTag.DisposeMethodBuilder)] IBuilder<CompositionCode, CompositionCode> disposeMethodBuilder,
    [Tag(WellknownTag.ToStringMethodBuilder)] IBuilder<CompositionCode, CompositionCode> toStringBuilder,
    [Tag(WellknownTag.ResolversFieldsBuilder)] IBuilder<CompositionCode, CompositionCode> resolversFieldsBuilder,
    [Tag(WellknownTag.StaticConstructorBuilder)] IBuilder<CompositionCode, CompositionCode> staticConstructorBuilder,
    [Tag(WellknownTag.ResolverClassesBuilder)] IBuilder<CompositionCode, CompositionCode> resolversClassesBuilder,
    [Tag(typeof(ClassCommenter))] ICommenter<Unit> classCommenter,
    IInformation information,
    CancellationToken cancellationToken)
    : IBuilder<CompositionCode, CompositionCode>
{
    private readonly ImmutableArray<IBuilder<CompositionCode, CompositionCode>> _codeBuilders = ImmutableArray.Create(
        fieldsBuilder,
        argFieldsBuilder,
        parameterizedConstructorBuilder,
        defaultConstructorBuilder,
        scopeConstructorBuilder,
        rootPropertiesBuilder,
        apiMembersBuilder,
        disposeMethodBuilder,
        toStringBuilder,
        resolversFieldsBuilder,
        staticConstructorBuilder,
        resolversClassesBuilder);

    public CompositionCode Build(CompositionCode composition)
    {
        var code = composition.Code;
        code.AppendLine("// <auto-generated/>");
        code.AppendLine($"// by {information.Description}");
        code.AppendLine("#nullable enable");
        code.AppendLine("#pragma warning disable");
        code.AppendLine();

        composition = usingDeclarationsBuilder.Build(composition);
        
        var nsIndent = Disposables.Empty;
        var name = composition.Source.Source.Name;
        if (!string.IsNullOrWhiteSpace(name.Namespace))
        {
            code.AppendLine($"namespace {name.Namespace}");
            code.AppendLine("{");
            nsIndent = code.Indent();
        }
        
        classCommenter.AddComments(composition, Unit.Shared);

        code.AppendLine($"[{Names.SystemNamespace}Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        var implementingInterfaces = composition.DisposablesCount > 0 ? $": {Names.IDisposableInterfaceName}" : "";
        code.AppendLine($"partial class {name.ClassName}{implementingInterfaces}");
        code.AppendLine("{");

        using (code.Indent())
        {
            // Generate class members
            foreach (var builder in _codeBuilders)
            {
                cancellationToken.ThrowIfCancellationRequested();
                composition = builder.Build(composition);
            }
        }

        code.AppendLine("}");

        // ReSharper disable once InvertIf
        if (!string.IsNullOrWhiteSpace(name.Namespace))
        {
            // ReSharper disable once RedundantAssignment
            nsIndent.Dispose();
            code.AppendLine("}");
        }
        
        code.AppendLine("#pragma warning restore");
        return composition;
    }
}