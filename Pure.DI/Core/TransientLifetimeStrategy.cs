﻿namespace Pure.DI.Core;

// ReSharper disable once ClassNeverInstantiated.Global
internal class TransientLifetimeStrategy : ILifetimeStrategy
{
    private readonly IRaiseOnDisposableExpressionBuilder _raiseOnDisposableExpressionBuilder;
    private readonly IWrapperStrategy _wrapperStrategy;

    public TransientLifetimeStrategy(
        IRaiseOnDisposableExpressionBuilder raiseOnDisposableExpressionBuilder,
        IWrapperStrategy wrapperStrategy)
    {
        _raiseOnDisposableExpressionBuilder = raiseOnDisposableExpressionBuilder;
        _wrapperStrategy = wrapperStrategy;
    }

    public Lifetime Lifetime => Lifetime.Transient;

    public ExpressionSyntax Build(SemanticType resolvingType, Dependency dependency, ExpressionSyntax objectBuildExpression) =>
        _raiseOnDisposableExpressionBuilder.Build(
            dependency.Implementation,
            Lifetime.Transient,
            _wrapperStrategy.Build(resolvingType, dependency, objectBuildExpression));
}