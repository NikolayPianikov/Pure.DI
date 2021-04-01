﻿namespace Pure.DI.Core
{
    using System;

    internal class Binding : IBinding
    {
        private readonly IConfiguration _configuration;

        public Binding(IConfiguration configuration) => _configuration = configuration;

        public IBinding Bind<T>() => this;

        public IBinding As(Lifetime lifetime) => this;

        public IBinding Tag(object tag) => this;

        public IConfiguration To<T>() => _configuration;

        public IConfiguration To<T>(Func<IContext, T> factory) => _configuration;
    }
}