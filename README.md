# SM Cheats

Внутриигровая debug-панель (чит-меню) для мобильных игр на Unity. Открывается прямо в билде на устройстве и даёт:

- **Options** — читы из вашего кода: кнопки, переключатели, слайдеры, поля ввода, выпадающие списки. Описываются атрибутами.
- **Console** — лог Unity на устройстве: фильтры Log / Warning / Error, детали со стектрейсом, индикатор новых исключений.
- **Profiler** — графики FPS, CPU и памяти.
- **Info** — сведения об устройстве, билде, экране и т.д.
- **Активация** — горячая клавиша (редактор / десктоп) и тройной тап по невидимой экранной кнопке (мобильные).
- **PIN-код** — опциональная защита входа с ограничением числа попыток и блокировкой.
- **Закрепление (pin)** — элементы опций и консоли можно закрепить поверх игры, не открывая панель.

## Требования

| | Версия |
|---|---|
| Unity | 6000.3+ |
| `com.unity.ugui` | 2.0.0 |
| `com.unity.inputsystem` | 1.20.0 |

Зависимости ставятся автоматически вместе с пакетом. Работает и с новой Input System, и со старым `Input` (Active Input Handling = Both).

## Установка

### Через Git URL (Package Manager)

`Window > Package Manager > + > Add package from git URL…`:

```
https://github.com/alinouski/SMCheats.git
```

Или вручную в `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.alinouski.smcheats": "https://github.com/alinouski/SMCheats.git"
  }
}
```

Конкретная версия — по тегу или коммиту: `https://github.com/alinouski/SMCheats.git#0.1.0`.

### Как embedded-пакет

Склонируйте репозиторий в папку `Packages/` проекта:

```bash
git clone https://github.com/alinouski/SMCheats.git Packages/com.alinouski.smcheats
```

Так пакет можно править прямо из проекта (например, менять префабы).

## Настройка

1. Выполните `Tools > Cheats > Create Settings Asset`.
   Будет создан `Assets/Resources/SMCheats/CheatsSettings.asset`. Если `CheatsSettings` уже лежит в любой папке
   `Assets/**/Resources/SMCheats/`, будет выбран существующий.
2. Настройте ассет в инспекторе:

| Поле | Назначение |
|---|---|
| Enable In Release Builds | Разрешить читы в release-билдах (по умолчанию выключено, см. «Релизные билды») |
| Default Tab Id | Вкладка, открываемая первой: `info`, `console`, `profiler`, `options` или id своей панели |
| Enabled Tabs | Какие встроенные вкладки показывать |
| Activation Trigger Settings → Open Methods | `KeyboardShortcut` и/или `ScreenTrigger` |
| Activation Trigger Settings → Keyboard Shortcut | Клавиша открытия (по умолчанию `P`) |
| Pin Code Settings | Включение PIN-кода, сам код, число попыток, длительность блокировки (сек.) |
| Fps Threshold | Пороги подсветки FPS: `x` — выше «хорошо», `y` — выше «средне», иначе «плохо». По умолчанию `(50, 30)` |
| Profiler … Graph Visual Config | Внешний вид графиков (по умолчанию — конфиги из пакета) |
| Ui | Цвета, иконки вкладок и прочие параметры интерфейса |
| Maximum Console Entries | Размер буфера консоли (не меньше 50) |

> Ассет **обязателен**: без `Resources/SMCheats/CheatsSettings` вызов `SMCheatsUtility.Init()` бросит `FileNotFoundException`.

## Быстрый старт

```csharp
using SMCheats;
using UnityEngine;

public sealed class Bootstrap : MonoBehaviour
{
    private void Start()
    {
        SMCheatsUtility.Init();
        SMCheatsUtility.SetActiveTriggerListener(true); // клавиша / тройной тап
        SMCheatsUtility.RegisterOptions(new GameCheats());
    }
}
```

Теперь панель открывается клавишей `P` в редакторе или тройным тапом по экранной кнопке-триггеру на устройстве.

## Релизные билды

По умолчанию читы работают только в редакторе и в Development Build. В release-билде `Init()` ничего не создаёт,
`SMCheatsUtility.IsEnabled` равен `false`, а остальные методы ничего не делают — вызывать их можно без `#if`.

- **Включить в релизе** (например, для внутреннего тестирования release-сборки) — галочка `Enable In Release Builds` в `CheatsSettings`.
- **Выключить везде**, включая редактор, — scripting define `SMCHEATS_DISABLED`
  (`Project Settings > Player > Scripting Define Symbols`).

Код с `[Cheat]` при этом остаётся в билде. Если он не должен попасть в боевую сборку, оборачивайте его в
`#if DEVELOPMENT_BUILD || UNITY_EDITOR` сами.

## API

| Метод | Описание |
|---|---|
| `SMCheatsUtility.Init(params ICheatsPanelController[] panels)` | Инициализация. Повторные вызовы игнорируются. Можно передать свои вкладки |
| `SMCheatsUtility.IsEnabled` | Включены ли читы в этом билде (см. «Релизные билды») |
| `SMCheatsUtility.SetActiveTriggerListener(bool)` | Включить/выключить открытие по клавише и тройному тапу |
| `SMCheatsUtility.Show()` | Открыть панель (с запросом PIN, если он включён) |
| `SMCheatsUtility.Hide()` | Закрыть панель |
| `SMCheatsUtility.RegisterOptions(object)` | Просканировать объект на `[Cheat]`-члены и добавить их во вкладку Options |
| `SMCheatsUtility.UnregisterOptions(object)` | Убрать опции объекта |
| `SMCheatsUtility.AddAction / AddToggle / AddValue<T>` | Добавить опцию из кода без атрибутов, возвращает `CheatOptionDefinition` |
| `SMCheatsUtility.RemoveOption(CheatOptionDefinition)` | Удалить опцию, добавленную через `Add…` |

Все методы, кроме `Init`, требуют предварительной инициализации — иначе бросается исключение.

Открыть панель можно и из своей кнопки или жеста:

```csharp
debugButton.onClick.AddListener(SMCheatsUtility.Show);
```

## Опции (вкладка Options)

Опции описываются атрибутом `[Cheat]` на **экземплярных** методах и свойствах (public или private) объекта,
переданного в `RegisterOptions`. Тип контрола выбирается по типу члена:

| Член класса | Контрол |
|---|---|
| `void Method()` без параметров | Кнопка |
| `bool` свойство с сеттером | Переключатель |
| `int` / `float` + `[CheatRange]` | Слайдер |
| `int` / `float` + `[CheatStep]` | Селектор `−` / `+` |
| `int` / `float` | Числовое поле ввода |
| `string` | Текстовое поле |
| `enum` или `[CheatDropdown]` | Выпадающий список |
| Свойство без сеттера / другой тип | Значение только для чтения |

### Атрибуты

| Атрибут | Где | Описание |
|---|---|---|
| `[Cheat(category, sortOrder, displayName)]` | метод, свойство | Помечает член как чит. По умолчанию категория `"Default"`, имя — имя члена |
| `[CheatRange(min, max)]` | `int` / `float` | Диапазон → слайдер |
| `[CheatStep(step)]` | `int` / `float` | Шаг → селектор |
| `[CheatDropdown(nameof(Source))]` | свойство | Источник вариантов: поле, свойство или метод без параметров, возвращающий коллекцию |
| `[CheatDropdownItem(nameof(Label), nameof(Value))]` | свойство | Какие члены элемента коллекции брать как подпись и как значение |
| `[CheatVisibleIf(nameof(Flag), expected = true)]` | метод, свойство | Показывать, только когда `bool`-член равен `expected` |

Порядок внутри категории — по возрастанию `sortOrder` (по умолчанию 0), при равных — по порядку объявления в файле.

### Пример

```csharp
using System.Collections.Generic;
using SMCheats;

public sealed class GameCheats
{
    private readonly PlayerModel _player;
    private readonly IReadOnlyList<WeaponConfig> _weapons;

    public GameCheats(PlayerModel player, IReadOnlyList<WeaponConfig> weapons)
    {
        _player = player;
        _weapons = weapons;
    }

    // Кнопки
    [Cheat("Economy")]
    private void Add1000Coins() => _player.Coins += 1000;

    [Cheat("Economy", displayName: "Reset wallet")]
    private void ResetWallet() => _player.Coins = 0;

    // Значение только для чтения (нет сеттера)
    [Cheat("Economy")]
    private int Coins => _player.Coins;

    // Переключатель
    [Cheat("Player")]
    private bool GodMode
    {
        get => _player.IsInvulnerable;
        set => _player.IsInvulnerable = value;
    }

    // Слайдер
    [Cheat("Player")]
    [CheatRange(0.1, 5)]
    private float SpeedMultiplier
    {
        get => _player.SpeedMultiplier;
        set => _player.SpeedMultiplier = value;
    }

    // Селектор с шагом
    [Cheat("Player")]
    [CheatStep(1)]
    private int Level
    {
        get => _player.Level;
        set => _player.Level = value;
    }

    // Выпадающий список из enum
    [Cheat("Player")]
    private Difficulty Difficulty
    {
        get => _player.Difficulty;
        set => _player.Difficulty = value;
    }

    // Выпадающий список из коллекции: подпись — Title, значение — Id
    [Cheat("Player")]
    [CheatDropdown(nameof(Weapons))]
    [CheatDropdownItem(nameof(WeaponConfig.Title), nameof(WeaponConfig.Id))]
    private string CurrentWeaponId
    {
        get => _player.WeaponId;
        set => _player.WeaponId = value;
    }

    private IReadOnlyList<WeaponConfig> Weapons => _weapons;

    // Видна только при включённом GodMode
    [Cheat("Player")]
    [CheatVisibleIf(nameof(GodMode))]
    private void KillAllEnemies() => _player.World.KillAllEnemies();

    // Текстовое поле
    [Cheat("Debug")]
    private string PromoCode { get; set; } = "";
}
```

Источник для `[CheatDropdown]` может возвращать и готовые `CheatDropdownChoice`:

```csharp
private IEnumerable<CheatDropdownChoice> Levels()
{
    yield return new CheatDropdownChoice("Tutorial", 0);
    yield return new CheatDropdownChoice("Boss", 42);
}
```

Без `[CheatDropdownItem]` подписью элемента станет его `ToString()`, значением — сам элемент.

### Регистрация на время жизни объекта

Опции держат ссылку на объект, поэтому снимайте регистрацию, когда объект уничтожается:

```csharp
public sealed class LevelCheats : MonoBehaviour
{
    private void OnEnable() => SMCheatsUtility.RegisterOptions(this);
    private void OnDisable() => SMCheatsUtility.UnregisterOptions(this);

    [Cheat("Level")]
    private void CompleteLevel() => FindFirstObjectByType<LevelController>().Complete();
}
```

Повторный `RegisterOptions` того же объекта пересоздаёт его опции, а не дублирует их.

### Опции без атрибутов

Если опцию удобнее собрать в рантайме (например, по списку уровней из конфига), используйте `Add…`:

```csharp
SMCheatsUtility.AddAction("Unlock all", () => progress.UnlockAll(), "Progress");

SMCheatsUtility.AddToggle("Show FPS", () => fpsView.activeSelf, fpsView.SetActive, "Debug");

// Слайдер: задан диапазон
SMCheatsUtility.AddValue("Time scale", () => Time.timeScale, v => Time.timeScale = v, "Debug",
    minimumValue: 0, maximumValue: 4);

// Только чтение: без сеттера
SMCheatsUtility.AddValue("Session id", () => analytics.SessionId, category: "Debug");

foreach (var level in levels)
{
    SMCheatsUtility.AddAction($"Load {level.Name}", () => loader.Load(level), "Levels");
}
```

Контрол выбирается по `T` так же, как для свойств с атрибутами. Опции с одинаковыми категорией, именем и типом
заменяют друг друга. Чтобы убрать опцию, сохраните результат и передайте его в `RemoveOption`.

### Ограничения

- Методы — только `void` без параметров; иначе при регистрации будет `InvalidOperationException`.
- Свойства — с геттером и без индексаторов. Поля (`[Cheat]` на field) не поддерживаются.
- `[CheatRange]` / `[CheatStep]` — только для `int` и `float`, и не вместе с `[CheatDropdown]`.
- Статические члены как читы не сканируются (но могут быть источником для `[CheatDropdown]` / `[CheatVisibleIf]`).

## PIN-код

Включите `Require Pin Code` в `CheatsSettings` и задайте код (только цифры, до 12 символов).
При открытии появится окно с цифровой клавиатурой; на десктопе работают цифры, `Enter`, `Backspace`, `Esc`.

- После `Maximum Pin Attempts` неверных попыток ввод блокируется на `Pin Lockout Duration Seconds`.
- После успешного ввода PIN больше не спрашивается до перезапуска приложения.
- Код хранится в ассете в открытом виде — это защита от случайного входа тестировщиков/игроков, а не от взлома.

## Свои вкладки

Унаследуйтесь от `CheatsPanelControllerBase` и передайте экземпляр в `Init`:

```csharp
SMCheatsUtility.Init(new NetworkPanelController());
```

Базовый класс сам создаёт кнопку вкладки, переключает видимость контента и управляет подписками.
Нужно задать `Id` (уникальный), `Order` (позиция вкладки) и построить контент в `BuildMainContent`:

```csharp
using System;
using SMCheats;
using UnityEngine;

public sealed class NetworkPanelController : CheatsPanelControllerBase
{
    public override string Id => "network";
    public override int Order => 100;
    protected override string Title => "Network";
    // protected override Sprite Icon => mySprite;

    protected override (GameObject, IDisposable) BuildMainContent()
    {
        var root = UiFactory.CreateLayoutRoot(
            "NetworkPanel", RuntimeUi.MainContentRoot, true, 8f, new RectOffset(16, 16, 16, 16));

        UiFactory.CreateSectionHeader(root, "Connection");
        var status = UiFactory.CreateText("Status", root, string.Empty, 24, TextAnchor.MiddleLeft);

        // Текст обновляется каждый кадр, пока вкладка открыта
        SelectionState.AddUpdate(status, () => $"Ping: {GameNetwork.Ping} ms");

        UiFactory.CreateSectionButton(root, "Reconnect", true).Button.onClick.AddListener(GameNetwork.Reconnect);

        return root.gameObject.GetDestroyDisposable();
    }
}
```

Что доступно в наследнике:

| Член | Назначение |
|---|---|
| `UiFactory` | Кнопки, тексты, заголовки, лейауты в стиле панели, загрузка своих префабов (`InstantiatePrefabView`) |
| `RuntimeUi` | Корни контента (`MainContentRoot`, `PinnedContentRoot`), заголовок, кнопки в шапке |
| `Services` | Сервисы пакета: `CheatsConsoleService`, `CheatsOptionsService`, `ICheatsCpuProfilerService`, `ICheatsMemoryProfilerService` |
| `SelectionState` | Подписки, активные только пока вкладка выбрана: `AddUpdate`, `AddLifecycle` |
| `AddHeaderButton` | Кнопка в шапке, видимая только на этой вкладке |
| `AddRegisterDisposable` | Освободить ресурс при выгрузке панели |
| `OnSelected` / `OnDeselected` | Переопределяемые хуки выбора вкладки (вызывайте `base`) |

Можно и полностью реализовать `ICheatsPanelController` самостоятельно — кнопку вкладки тогда удобно создать через
`new CheatsPanelTabButton(view, Id, Order)`.

## Структура пакета

```
Runtime/                         сборка SMCheats.Runtime (namespace SMCheats)
  SMCheatsUtility.cs             точка входа
  ActivationTrigger/             клавиша и экранный триггер
  PinCode/                       PIN-код
  Sections/                      вкладки Info, Console, Profiler, Options
  Services/                      консоль, профайлеры FPS/CPU/RAM
  Graphs/                        графики профайлера
  Resources/SMCheats/            префабы, конфиги и иконки (Resources.Load)
Editor/                          сборка SMCheats.Editor: инспектор настроек, генерация и валидация префабов
```

Настройки (`CheatsSettings`) живут в проекте, пакет поставляет только общие префабы и конфиги.

## Частые проблемы

- **`FileNotFoundException: CheatsSettings not found`** — не создан ассет настроек, см. «Настройка».
- **`Initialize cheats before Show`** — `Show` / `RegisterOptions` вызваны до `SMCheatsUtility.Init()`.
- **Панель не открывается по клавише/тапу** — не вызван `SetActiveTriggerListener(true)` или в `Open Methods` выключен нужный способ.
- **В release-билде ничего не происходит** — так задумано, см. «Релизные билды».
- **Опция не появилась** — член статический, это поле, у метода есть параметры, или объект не передан в `RegisterOptions`.

## Лицензия

[MIT](LICENSE.md)
