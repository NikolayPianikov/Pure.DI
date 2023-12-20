﻿// ReSharper disable ClassNeverInstantiated.Global
namespace Pure.DI.Core.Code;

internal class BlockCodeBuilder: ICodeBuilder<Block>
{
    public void Build(BuildContext ctx, in Block block)
    {
        var variable = ctx.Variable;
        if (!IsNewInstanceRequired(variable))
        {
            return;
        }

        var level = ctx.Level;
        var isThreadSafe = ctx.DependencyGraph.Source.Hints.GetHint(Hint.ThreadSafe, SettingState.On) == SettingState.On;
        var lockIsRequired = ctx.LockIsRequired ?? isThreadSafe;
        var toCheckExistence =
            // The "singleton" instance must be created with a check each time
            variable.Node.Lifetime == Lifetime.Singleton
            // The "per resolve" instance should be created without checks if it is the only one in the composition
            || (variable.Node.Lifetime == Lifetime.PerResolve && variable.Info.RefCount > 1);

        var code = ctx.Code; 
        if (toCheckExistence)
        {
            var checkExpression = variable.InstanceType.IsValueType
                ? $"!{variable.VariableName}Created"
                : $"object.ReferenceEquals({variable.VariableName}, null)";

            if (lockIsRequired)
            {
                code.AppendLine($"if ({checkExpression})");
                code.AppendLine("{");
                code.IncIndent();
                code.AppendLine($"lock ({Names.DisposablesFieldName})");
                code.AppendLine("{");
                code.IncIndent();
                ctx = ctx with { LockIsRequired = false };
            }
            
            code.AppendLine($"if ({checkExpression})");
            code.AppendLine("{");
            code.IncIndent();
            ctx = ctx with { Level = level + 1 };
        }

        foreach (var statement in block.Statements)
        {
            ctx.StatementBuilder.Build(ctx with { Variable = statement.Current }, statement);
        }

        if (!toCheckExistence)
        {
            return;
        }

        if (variable.Node.Lifetime == Lifetime.Singleton && variable.Node.IsDisposable())
        {
            code.AppendLine($"{Names.DisposablesFieldName}[{Names.DisposeIndexFieldName}++] = {variable.VariableName};");
        }

        if (variable.InstanceType.IsValueType)
        {
            if (variable.Node.Lifetime is not Lifetime.Transient and not Lifetime.PerBlock && isThreadSafe)
            {
                code.AppendLine($"{Names.SystemNamespace}Threading.Thread.MemoryBarrier();");
            }

            code.AppendLine($"{variable.VariableName}Created = true;");
        }

        code.DecIndent();
        code.AppendLine("}");
        if (!lockIsRequired)
        {
            return;
        }
        
        code.DecIndent();
        code.AppendLine("}");
        code.DecIndent();
        code.AppendLine("}");
        code.AppendLine();
    }
    
    private static bool IsNewInstanceRequired(Variable variable)
    {
        if (variable.Current.HasCycle)
        {
            return false;
        }
        
        var owners = variable.GetPath().TakeWhile(i => !i.Current.IsLazy).OfType<Block>().ToArray();
        if (variable.Info.Owners.Intersect(owners).Any())
        {
            return false;
        }

        variable.Info.Owners.Add(owners.FirstOrDefault());
        return true;
    }
}