// Copyright © 2026 Oleksandr Kukhtin. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace A2v10XamlAutocomplete;

[Export(typeof(IAsyncCompletionCommitManagerProvider))]
[Name("A2v10 XAML element commit manager provider")]
[ContentType("xml")]
internal class XamlCompletionCommitManagerProvider : IAsyncCompletionCommitManagerProvider
{
    IDictionary<ITextView, IAsyncCompletionCommitManager> cache = new Dictionary<ITextView, IAsyncCompletionCommitManager>();

    public IAsyncCompletionCommitManager GetOrCreate(ITextView textView)
    {
        if (cache.TryGetValue(textView, out var itemSource))
            return itemSource;

        var manager = new XamlCompletionCommitManager();
        textView.Closed += (o, e) => cache.Remove(textView); // clean up memory as files are closed
        cache.Add(textView, manager);
        return manager;
    }
}
