﻿using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;

namespace Qdrant.Grpc.Tests.Container;

public sealed class QdrantBuilder : ContainerBuilder<QdrantBuilder, QdrantContainer, QdrantConfiguration>
{
    public const string QdrantImage = "qdrant/qdrant:v1.1.3";

    public const ushort QdrantHttpPort = 6333;

    public const ushort QdrantGrpcPort = 6334;

    public QdrantBuilder() : this(new QdrantConfiguration()) =>
	    DockerResourceConfiguration = Init().DockerResourceConfiguration;

    private QdrantBuilder(QdrantConfiguration dockerResourceConfiguration) : base(dockerResourceConfiguration) =>
	    DockerResourceConfiguration = dockerResourceConfiguration;

    public override QdrantContainer Build()
    {
        Validate();
        return new QdrantContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    protected override QdrantBuilder Init() =>
        base.Init()
            .WithImage(QdrantImage)
            .WithPortBinding(QdrantHttpPort, true)
            .WithPortBinding(QdrantGrpcPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilMessageIsLogged(".*Actix runtime found; starting in Actix runtime.*"));

    protected override QdrantBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration) =>
        Merge(DockerResourceConfiguration, new QdrantConfiguration(resourceConfiguration));

    protected override QdrantBuilder Merge(QdrantConfiguration oldValue, QdrantConfiguration newValue) =>
        new(new QdrantConfiguration(oldValue, newValue));

    protected override QdrantConfiguration DockerResourceConfiguration { get; }

    protected override QdrantBuilder Clone(IContainerConfiguration resourceConfiguration) =>
        Merge(DockerResourceConfiguration, new QdrantConfiguration(resourceConfiguration));
}
