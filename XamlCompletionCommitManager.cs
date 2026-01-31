// Copyright © 2026 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;

namespace A2v10XamlAutocomplete;

internal class XamlCompletionCommitManager : IAsyncCompletionCommitManager
{
    readonly ImmutableArray<Char> commitChars = [' ', '\'', '"', ',', '.', ';', ':'];

    public IEnumerable<char> PotentialCommitCharacters => commitChars;

    public bool ShouldCommitCompletion(IAsyncCompletionSession session, SnapshotPoint location, char typedChar, CancellationToken token)
    {
        return true;
    }

    public CommitResult TryCommit(IAsyncCompletionSession session, ITextBuffer buffer, CompletionItem item, char typedChar, CancellationToken token)
    {
        if (item.Properties.TryGetProperty<Element>(nameof(Element), out var elem))
        {

            return CommitResult.Unhandled; // use default commit mechanism.
        }
        return CommitResult.Unhandled; // use default commit mechanism.
    }
}
