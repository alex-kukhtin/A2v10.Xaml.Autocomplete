// Copyright © 2026 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Operations;

namespace A2v10XamlAutocomplete;

internal class XamlCompletionSource(ITextStructureNavigatorSelectorService _structureNavigatorSelector) : IAsyncCompletionSource
{
    public Task<CompletionContext> GetCompletionContextAsync(IAsyncCompletionSession session, CompletionTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token)
    {
        var builder = ImmutableArray.CreateBuilder<CompletionItem>();

        // Получаем текст до текущей позиции
        var line = triggerLocation.GetContainingLine();
        var spanBeforeCaret = new SnapshotSpan(line.Start, triggerLocation);
        String textBefore = line.Snapshot.GetText(line.Start, triggerLocation.Position - line.Start);

        if (textBefore.EndsWith("<"))
        {
            builder.Add(Element.Create("!--", Element.ElemKind.Comment, this));
            builder.Add(Element.Create("![CDATA[", Element.ElemKind.CData, this));
            builder.Add(Element.Create("Page", Element.ElemKind.Tag, this));
            builder.Add(Element.Create("Dialog", Element.ElemKind.Tag, this));
            builder.Add(Element.Create("Alert", Element.ElemKind.Tag, this));
        }
        else if (textBefore.EndsWith(" "))
        {
            builder.Add(Element.Create("Name", Element.ElemKind.Property, this));
            builder.Add(Element.Create("Title", Element.ElemKind.Property, this));
        }
        else if (textBefore.EndsWith("\""))
        {
            builder.Add(Element.Create("True", Element.ElemKind.EnumValue, this));
            builder.Add(Element.Create("False", Element.ElemKind.EnumValue, this));
        }

        var context = new CompletionContext(builder.ToImmutableArray());
        return Task.FromResult(context);
    }

    public Task<Object> GetDescriptionAsync(IAsyncCompletionSession session, CompletionItem item, CancellationToken token)
    {
        String description = $"XML element or attribute: {item.DisplayText}";
        return Task.FromResult<Object>(description);
    }

    public CompletionStartData InitializeCompletion(CompletionTrigger trigger, SnapshotPoint triggerLocation, CancellationToken token)
    {

        ITextStructureNavigator navigator = _structureNavigatorSelector.GetTextStructureNavigator(triggerLocation.Snapshot.TextBuffer);
        TextExtent extent = navigator.GetExtentOfWord(triggerLocation);
        var ch = navigator.GetSpanOfFirstChild(extent.Span);
        var sibl = navigator.GetSpanOfNextSibling(ch);

        var span = new SnapshotSpan(triggerLocation, 0);
        return new CompletionStartData(CompletionParticipation.ProvidesItems, span);
    }
}
