// Copyright © 2021-2024 Oleksandr Kukhtin. All rights reserved.

using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Operations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace A2v10XamlAutocomplete;

internal class XamlCompletionSource(ITextStructureNavigatorSelectorService _structureNavigatorSelector) : IAsyncCompletionSource
{

    static Guid ImageLibraryGuid = new Guid("ae27a6b0-e345-4288-96df-5eaf394ee369");

    static ImageElement TagIcon = new ImageElement(new ImageId(ImageLibraryGuid, 1873), "Tag");
    static ImageElement PropertyIcon = new ImageElement(new ImageId(ImageLibraryGuid, 2429), "Property");
    static ImageElement EnumIcon = new ImageElement(new ImageId(ImageLibraryGuid, 1120), "Enum");
    static ImageElement XmlCommentIcon = new ImageElement(new ImageId(ImageLibraryGuid, 3568), "XmlComment");
    static ImageElement XmlCDataIcon = new ImageElement(new ImageId(ImageLibraryGuid, 3567), "XmlCData");


    public Task<CompletionContext> GetCompletionContextAsync(IAsyncCompletionSession session, CompletionTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token)
    {
        var builder = ImmutableArray.CreateBuilder<CompletionItem>();

        // Получаем текст до текущей позиции
        var line = triggerLocation.GetContainingLine();
        string textBefore = line.Snapshot.GetText(line.Start, triggerLocation.Position - line.Start);

        if (textBefore.EndsWith("<"))
        {
            builder.Add(new CompletionItem("Page", this, TagIcon));
            builder.Add(new CompletionItem("Dialog", this, TagIcon));
            builder.Add(new CompletionItem("Alert", this, TagIcon));
        }
        else if (textBefore.EndsWith(" "))
        {
            builder.Add(new CompletionItem("Name", this, PropertyIcon));
            builder.Add(new CompletionItem("Title", this, PropertyIcon));
        }
        else
        {
            // По умолчанию — ничего или можно вернуть все
        }

        var context = new CompletionContext(builder.ToImmutableArray());
        return Task.FromResult(context);
    }

    public Task<object> GetDescriptionAsync(IAsyncCompletionSession session, CompletionItem item, CancellationToken token)
    {
        String description = $"XML element or attribute: {item.DisplayText}";
        return Task.FromResult<object>(description);
    }

    public CompletionStartData InitializeCompletion(CompletionTrigger trigger, SnapshotPoint triggerLocation, CancellationToken token)
    {
        var span = new SnapshotSpan(triggerLocation, 0);
        return new CompletionStartData(CompletionParticipation.ProvidesItems, span);
    }
}
