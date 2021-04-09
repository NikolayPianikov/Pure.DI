﻿namespace Pure.DI.Core
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal interface IFallbackStrategy
    {
        StatementSyntax Build(
            ICollection<FallbackMetadata> metadata,
            TypeSyntax? targetType,
            ExpressionSyntax typeExpression,
            ExpressionSyntax tagExpression);
    }
}