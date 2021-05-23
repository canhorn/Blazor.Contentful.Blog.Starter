﻿namespace Blazor.Contentful_.Blog.Starter.Shared.Components.DynamicComponent
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Rendering;

    /// <summary>
    /// A component that renders another component dynamically according to its
    /// <see cref="Type" /> parameter.
    /// </summary>
    public class DynamicComponent : IComponent
    {
        private RenderHandle _renderHandle;
        private readonly RenderFragment _cachedRenderFragment;

        /// <summary>
        /// Constructs an instance of <see cref="DynamicComponent"/>.
        /// </summary>
        public DynamicComponent()
        {
            _cachedRenderFragment = Render;
        }

        /// <summary>
        /// Gets or sets the type of the component to be rendered. The supplied type must
        /// implement <see cref="IComponent"/>.
        /// </summary>
        [Parameter]
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        public Type Type { get; set; } = default!;

        /// <summary>
        /// Gets or sets a dictionary of parameters to be passed to the component.
        /// </summary>
        // Note that this deliberately does *not* use CaptureUnmatchedValues. Reasons:
        // [1] The primary scenario for DynamicComponent is where the call site doesn't
        //     know which child component it's rendering, so it typically won't know what
        //     set of parameters to pass either, hence the developer most likely wants to
        //     pass a dictionary rather than having a fixed set of parameter names in markup.
        // [2] If we did have CaptureUnmatchedValues here, then it would become a breaking
        //     change to ever add more parameters to DynamicComponent itself in the future,
        //     because they would shadow any coincidentally same-named ones on the target
        //     component. This could lead to application bugs.
        [Parameter]
        public IDictionary<string, object>? Parameters { get; set; }

        /// <inheritdoc />
        public void Attach(RenderHandle renderHandle)
        {
            _renderHandle = renderHandle;
        }

        /// <inheritdoc />
        public Task SetParametersAsync(ParameterView parameters)
        {
            // This manual parameter assignment logic will be marginally faster than calling
            // ComponentProperties.SetProperties.
            foreach (var entry in parameters)
            {
                if (entry.Name.Equals(nameof(Type), StringComparison.OrdinalIgnoreCase))
                {
                    Type = (Type)entry.Value;
                }
                else if (entry.Name.Equals(nameof(Parameters), StringComparison.OrdinalIgnoreCase))
                {
                    Parameters = (IDictionary<string, object>)entry.Value;
                }
                else
                {
                    throw new InvalidOperationException(
                        $"{nameof(DynamicComponent)} does not accept a parameter with the name '{entry.Name}'. To pass parameters to the dynamically-rendered component, use the '{nameof(Parameters)}' parameter.");
                }
            }

            if (Type is null)
            {
                throw new InvalidOperationException($"{nameof(DynamicComponent)} requires a non-null value for the parameter {nameof(Type)}.");
            }

            _renderHandle.Render(_cachedRenderFragment);
            return Task.CompletedTask;
        }

        void Render(RenderTreeBuilder builder)
        {
            builder.OpenComponent(0, Type);

            if (Parameters != null)
            {
                foreach (var entry in Parameters)
                {
                    builder.AddAttribute(1, entry.Key, entry.Value);
                }
            }

            builder.CloseComponent();
        }
    }
}
