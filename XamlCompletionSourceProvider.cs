// Copyright © 2026 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace A2v10XamlAutocomplete;

[Export(typeof(IAsyncCompletionSourceProvider))]
[Name("A2v10 XAML completion provider")]
[ContentType("xml")]
internal class XamlCompletionSourceProvider : IAsyncCompletionSourceProvider
{
    private readonly Dictionary<ITextView, IAsyncCompletionSource> cache = new();

    [Import]
    ITextStructureNavigatorSelectorService StructureNavigatorSelector { get; set; }

    public IAsyncCompletionSource GetOrCreate(ITextView textView)
    {
        if (cache.TryGetValue(textView, out var itemSource))
            return itemSource;

        var source = new XamlCompletionSource(StructureNavigatorSelector); // opportunity to pass in MEF parts
        textView.Closed += (o, e) => cache.Remove(textView); // clean up memory as files are closed
        cache.Add(textView, source);
        return source;
    }
}
