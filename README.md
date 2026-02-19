# A2v10.Xaml.Autocomplete

VSIX-расширение для Visual Studio 2022/2026, обеспечивающее IntelliSense (автодополнение) для `.xaml` файлов платформы A2v10.

## Требования

- **Visual Studio 2022** (17.14+) или **Visual Studio 2026**
- Рабочая нагрузка **Visual Studio extension development** (VS SDK)
- **.NET Framework 4.8 Targeting Pack** (для сборки VSIX)
- **.NET 8+ SDK** (для генератора схемы)

## Структура проекта

```
A2v10.Xaml.Autocomplete/          VSIX-расширение (.NET FW 4.8)
  Parser/                         Лексический анализатор XML-контекста
    XmlContext.cs                  Модель контекста (7 типов)
    XmlContextParser.cs           Backward tokenizer
  Schema/                         Модель метаданных и кеширование
    XamlSchema.cs                 Lazy singleton с индексами
    XamlSchemaLoader.cs           Загрузка JSON из EmbeddedResource
    ElementInfo.cs                Модель элемента
    PropertyInfo.cs               Модель свойства
    AttachedPropertyInfo.cs       Модель attached-свойства
  Resources/
    a2v10-xaml-schema.json        Сгенерированная схема (EmbeddedResource)
  XamlCompletionSource.cs         IAsyncCompletionSource (семантическое дополнение)
  XamlCompletionSourceProvider.cs MEF-провайдер
  XamlCompletionCommitManager.cs  IAsyncCompletionCommitManager (контекстный коммит)
  Element.cs                      Модель completion-элемента

A2v10.Xaml.SchemaGenerator/       Генератор JSON-схемы (.NET 8+)
  Program.cs                      CLI точка входа
  SchemaGenerator.cs              Рефлексия сборки A2v10.ViewEngine.Xaml
```

## Сборка

1. Откройте `A2v10.Xaml.Autocomplete.slnx` в Visual Studio.
2. **Build** > **Build Solution** (или `Ctrl+Shift+B`).

Результат сборки: `bin\Debug\A2v10.Xaml.Autocomplete.vsix`.

## Отладка (Experimental Instance)

Проект уже настроен для отладки через Experimental Instance VS:

1. Откройте `A2v10.Xaml.Autocomplete.slnx` в VS.
2. Нажмите **F5** (или **Debug** > **Start Debugging**).
3. Запустится вторая копия VS (Experimental Instance) с установленным расширением.
4. В Experimental Instance откройте папку/проект с `.xaml` файлами A2v10 (например, `USP_2025`).
5. Откройте любой `.xaml` файл и проверьте автодополнение.

> Настройка `/rootsuffix Exp` задана в `.csproj` (свойства `StartProgram` и `StartArguments`).

## Установка вручную

1. Соберите проект в конфигурации **Release**.
2. Файл `bin\Release\A2v10.Xaml.Autocomplete.vsix` — установщик расширения.
3. Закройте все экземпляры Visual Studio.
4. Дважды кликните на `.vsix` файл и следуйте инструкциям.

Для удаления: **Extensions** > **Manage Extensions** > найти **A2v10.Xaml.Autocomplete** > **Uninstall**.

## Обновление JSON-схемы

Схему нужно перегенерировать при изменении сборки `A2v10.ViewEngine.Xaml` (добавление/удаление элементов, свойств, enum-ов).

### Запуск генератора

```bash
cd A2v10.Xaml.SchemaGenerator
dotnet run -- <путь-к-A2v10.ViewEngine.Xaml.dll> <путь-к-output.json>
```

Пример:

```bash
dotnet run -- "D:\Program\PROJECTS\_CPMS\_A2v10CORE\A2v10.Core\ViewEngines\A2v10.ViewEngine.Xaml\bin\Debug\net8.0\A2v10.ViewEngine.Xaml.dll" "D:\Program\PROJECTS\_CPMS\_A2v10CORE\A2v10.Xaml.Autocomplete\Resources\a2v10-xaml-schema.json"
```

Если второй аргумент не указан, JSON будет сохранён в текущую директорию как `a2v10-xaml-schema.json`.

### После обновления

Пересоберите VSIX проект — JSON встраивается как `EmbeddedResource`.

## Возможности IntelliSense

| Контекст | Что предлагается |
|---|---|
| `<` | Все теги A2v10 (с фильтрацией по допустимым дочерним элементам) |
| `<Tag ` (пробел) | Атрибуты элемента (с учётом уже указанных) |
| `<Tag Attr="` | Значения enum, Boolean (`True`/`False`), привязки (`{Bind }`, `{BindCmd }`) |
| `</` | Закрывающий тег ближайшего незакрытого элемента |
| `<Tag.` | Element properties (свойства-элементы) |
| Attached properties | `Grid.Row`, `Grid.Col`, `Toolbar.Align` и др. |
| Комментарии | `<!-- -->`, `<![CDATA[ ]]>` |

## Совместимость

Расширение **активируется** для `.xaml` файлов, содержащих:
- `xmlns="clr-namespace:A2v10.Xaml..."` (default namespace A2v10)
- Корневые элементы `<Page`, `<Dialog`, `<Alert`

Расширение **не активируется** для:
- WPF XAML (`schemas.microsoft.com/winfx/2006/xaml/presentation`)
- .NET MAUI XAML (`schemas.microsoft.com/dotnet/2021/maui`)
- Файлы без расширения `.xaml`
