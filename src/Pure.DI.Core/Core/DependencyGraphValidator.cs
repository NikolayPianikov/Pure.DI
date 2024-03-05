// ReSharper disable ClassNeverInstantiated.Global
namespace Pure.DI.Core;

internal sealed class DependencyGraphValidator(
    ILogger<DependencyGraphValidator> logger,
    IFilter filter,
    CancellationToken cancellationToken)
    : IValidator<DependencyGraph>
{
    public bool Validate(DependencyGraph dependencyGraph)
    {
        var graph = dependencyGraph.Graph;
        var isResolved = dependencyGraph.IsResolved;
        if (isResolved)
        {
            return true;
        }
        
        var isErrorReported = false;
        foreach (var (_, dependencyNode, unresolvedInjection, target) in graph.Edges.Where(i => !i.IsResolved))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var mdSetup = dependencyGraph.Source;
            if (mdSetup.Hints.IsOnCannotResolveEnabled
                && filter.IsMeetRegularExpression(
                    mdSetup,
                    (Hint.OnCannotResolveContractTypeNameRegularExpression, unresolvedInjection.Type.ToString()),
                    (Hint.OnCannotResolveTagRegularExpression, unresolvedInjection.Tag.ValueToString()),
                    (Hint.OnCannotResolveLifetimeRegularExpression, dependencyNode.Lifetime.ValueToString())))
            {
                continue;
            }

            isResolved = false;
            var errorMessage = $"Unable to resolve \"{unresolvedInjection}\" in {target}.";
            var locationsWalker = new DependencyGraphLocationsWalker(unresolvedInjection);
            locationsWalker.VisitDependencyNode(Unit.Shared, target);
            foreach (var location in locationsWalker.Locations)
            {
                logger.CompileError(errorMessage, location, LogId.ErrorUnableToResolve);
                isErrorReported = true;
            }
        }

        if (!isResolved && !isErrorReported)
        {
            logger.CompileError("Cannot build a dependency graph.", dependencyGraph.Source.Source.GetLocation(), LogId.ErrorUnableToResolve);
        }

        throw HandledException.Shared;
    }
}