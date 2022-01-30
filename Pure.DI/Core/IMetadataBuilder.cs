namespace Pure.DI.Core;

internal interface IMetadataBuilder
{
    MetadataContext Build(Compilation compilation, CancellationToken cancellationToken);
}