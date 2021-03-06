﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http.Dependencies;
using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;

namespace FsChat.Services.APIs.Public.App_Start.Ioc.Plumbing
{
    [DebuggerStepThrough]
    public class CastleDependencyScope : IDependencyScope
    {
        private readonly IKernel kernel;

        private readonly IDisposable disposable;

        public CastleDependencyScope(IKernel kernel)
        {
            this.kernel = kernel;
            disposable = kernel.BeginScope();
        }

        public object GetService(Type type)
        {
            return kernel.HasComponent(type) ? kernel.Resolve(type) : null;
        }

        public IEnumerable<object> GetServices(Type type)
        {
            return kernel.ResolveAll(type).Cast<object>();
        }

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}
