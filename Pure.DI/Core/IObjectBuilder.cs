﻿namespace Pure.DI.Core
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal interface IObjectBuilder
    {
        ExpressionSyntax TryBuild(
            TypeResolveDescription typeDescription,
            ICollection<BindingMetadata> additionalBindings,
            int level = 0);
    }
}