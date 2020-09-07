﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs.Host.Executors;

namespace Microsoft.Azure.WebJobs.Extensions.SignalRService
{
    internal abstract class SignalRMethodExecutor
    {
        protected IRequestResolver Resolver { get; }
        protected ExecutionContext ExecutionContext { get; }

        protected SignalRMethodExecutor(IRequestResolver resolver, ExecutionContext executionContext)
        {
            Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            ExecutionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
        }

        public abstract Task<HttpResponseMessage> ExecuteAsync(HttpRequestMessage request);

        protected Task<FunctionResult> ExecuteWithAuthAsync(HttpRequestMessage request, ExecutionContext executor,
            InvocationContext context, TaskCompletionSource<object> tcs = null)
        {
            if (!Resolver.ValidateSignature(request, executor.AccessKey))
            {
                throw new SignalRTriggerAuthorizeFailedException();
            }

            return ExecuteAsyncCore(executor.Executor, context, tcs);
        }

        private async Task<FunctionResult> ExecuteAsyncCore(ITriggeredFunctionExecutor executor, InvocationContext context, TaskCompletionSource<object> tcs)
        {
            var signalRTriggerEvent = new SignalRTriggerEvent
            {
                Context = context,
                TaskCompletionSource = tcs,
            };

            var result = await executor.TryExecuteAsync(
                new TriggeredFunctionData
                {
                    TriggerValue = signalRTriggerEvent
                }, CancellationToken.None);

            // If there's exception in invocation, tcs may not be set.
            // And SetException seems not necessary. Exception can be get from FunctionResult.
            if (result.Succeeded == false)
            {
                tcs?.TrySetResult(null);
            }

            return result;
        }
    }
}
