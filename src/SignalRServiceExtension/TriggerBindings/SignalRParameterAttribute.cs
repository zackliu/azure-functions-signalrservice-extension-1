﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Microsoft.Azure.WebJobs.Extensions.SignalRService
{
    /// <summary>
    /// Mark the parameter as the SignalR parameter that need to bind arguments.
    /// It's mutually exclusive with <see cref="SignalRTriggerAttribute.ParameterNames"/>. That means
    /// you can not set <see cref="SignalRTriggerAttribute.ParameterNames"/> and use <see cref="SignalRParameterAttribute"/>
    /// at the same time.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class SignalRParameterAttribute : Attribute
    {
    }
}
