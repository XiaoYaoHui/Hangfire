﻿using System;
using System.ComponentModel;
using Hangfire;

namespace Core.Api.Filter
{
    /// <summary>
    /// 
    /// </summary>
    public class ContainerJobActivator : JobActivator
    {
        private readonly IContainer _container;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public ContainerJobActivator(IContainer container)
        {
            _container = container;
        }
    }
}
