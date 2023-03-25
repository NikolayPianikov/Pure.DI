namespace Pure.DI.Core.CSharp;

internal class ClassBuilder : IBuilder<CompositionCode, CompositionCode>
{
    private readonly IBuilder<CompositionCode, CompositionCode> _usingDeclarationsBuilder;
    private readonly ImmutableArray<IBuilder<CompositionCode, CompositionCode>> _codeBuilders;
    
    public ClassBuilder(
        [IoC.Tag(WellknownTag.CSharpUsingDeclarationsBuilder)] IBuilder<CompositionCode, CompositionCode> usingDeclarationsBuilder,
        [IoC.Tag(WellknownTag.CSharpSingletonFieldsBuilder)] IBuilder<CompositionCode, CompositionCode> singletonFieldsBuilder,
        [IoC.Tag(WellknownTag.CSharpArgFieldsBuilder)] IBuilder<CompositionCode, CompositionCode> argFieldsBuilder,
        [IoC.Tag(WellknownTag.CSharpPrimaryConstructorBuilder)] IBuilder<CompositionCode, CompositionCode> primaryConstructorBuilder,
        [IoC.Tag(WellknownTag.CSharpDefaultConstructorBuilder)] IBuilder<CompositionCode, CompositionCode> defaultConstructorBuilder,
        [IoC.Tag(WellknownTag.CSharpChildConstructorBuilder)] IBuilder<CompositionCode, CompositionCode> childConstructorBuilder,
        [IoC.Tag(WellknownTag.CSharpRootPropertiesBuilder)] IBuilder<CompositionCode, CompositionCode> rootPropertiesBuilder,
        [IoC.Tag(WellknownTag.CSharpDisposeMethodBuilder)] IBuilder<CompositionCode, CompositionCode> disposeMethodBuilder,
        [IoC.Tag(WellknownTag.CSharpResolversMembersBuilder)] IBuilder<CompositionCode, CompositionCode> resolversMembersBuilder,
        [IoC.Tag(WellknownTag.CSharpResolversFieldsBuilder)] IBuilder<CompositionCode, CompositionCode> resolversFieldsBuilder,
        [IoC.Tag(WellknownTag.CSharpStaticConstructorBuilder)] IBuilder<CompositionCode, CompositionCode> staticConstructorBuilder,
        [IoC.Tag(WellknownTag.CSharpResolverClassBuilder)] IBuilder<CompositionCode, CompositionCode> indexClassBuilder)
    {
        _usingDeclarationsBuilder = usingDeclarationsBuilder;
        _codeBuilders = ImmutableArray.Create(
            singletonFieldsBuilder,
            argFieldsBuilder,
            primaryConstructorBuilder,
            defaultConstructorBuilder,
            childConstructorBuilder,
            rootPropertiesBuilder,
            disposeMethodBuilder,
            resolversMembersBuilder,
            resolversFieldsBuilder,
            staticConstructorBuilder,
            indexClassBuilder);
    }

    public CompositionCode Build(CompositionCode composition, CancellationToken cancellationToken)
    {
        var code = composition.Code;
        code.AppendLine("// <auto-generated/>");
        code.AppendLine();
        var nsIndent = Disposables.Empty;
        if (!string.IsNullOrWhiteSpace(composition.Namespace))
        {
            code.AppendLine($"namespace {composition.Namespace}");
            code.AppendLine("{");
            nsIndent = code.Indent();
        }
        
        composition = _usingDeclarationsBuilder.Build(composition, cancellationToken);

        code.AppendLine("[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        var implementingInterfaces = composition.Singletons.Any() ? $": {CodeConstants.DisposableTypeName}" : "";
        code.AppendLine($"partial class {composition.ClassName}{implementingInterfaces}");
        code.AppendLine("{");

        using (code.Indent())
        {
            // Generate class members
            foreach (var builder in _codeBuilders)
            {
                cancellationToken.ThrowIfCancellationRequested();
                composition = builder.Build(composition, cancellationToken);
            }
        }

        code.AppendLine("}");

        // ReSharper disable once InvertIf
        if (!string.IsNullOrWhiteSpace(composition.Namespace))
        {
            // ReSharper disable once RedundantAssignment
            nsIndent.Dispose();
            code.AppendLine("}");
        }

        return composition;
    }
}