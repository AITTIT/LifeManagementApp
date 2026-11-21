# LifeManagementApp – plan to reach 55 / 55 (work points, before coefficients)

Target: criteria 1–8 = 50 p, plus bonus B1 (2) + B2 (2) + B3 (1) = 55.
Style rule: stay inside what the course taught and what the project already uses
(CommunityToolkit.Mvvm, `Preferences`, typed `HttpClient`, `IDbContextFactory`, `TodoStore`, Shell tabs).
Platform scope: **Windows is the verified target**; Android must not break (no Windows-only APIs outside `#if WINDOWS`, `INTERNET` permission already present) but is not tested in depth. The assignment only requires "Windows or Android".

## 1. Starting state (audit)

| Criterion | Start | Missing for full points |
|---|---|---|
| 1 Functionality (10) | To-do list, 2 sort orders, add with validation | Home page: top 3, weather, kitesurf alert |
| 2 Pages/navigation (5) | 3 tabs = level 3; no data passed between pages (shared `TodoStore`/`Preferences` is shared state, not parameter passing) | Parameter passing: an edit page (level 5) |
| 3 Storage (8) | SQLite + EF Core, async, add/query/update/delete = level 6 | Relation: `Category` 1–N `TodoItem` (level 8) |
| 4 Architecture (9) | Models / ViewModels / Views / Services / Data, `TodoStore` + `ITodoRepository` | Weather service layer; remove duplication (sort logic in 2 places, category list hard-coded in 2 VMs); navigation abstraction |
| 5 Binding (5) | Toolkit, `ObservableCollection`, `x:DataType`, converter | Keep it for all new pages |
| 6 DI (5) | Interfaces, singleton/transient registered | `IWeatherService` typed client; write the *reason* for each lifetime in `MauiProgram` |
| 7 REST (5) | – | Whole feature |
| 8 Git/docs (3) | Regular commits, PR per issue | README (P7 template, **AI use**) |
| B1 (2) | Issue → branch → PR "Closes #N" | Keep doing it |
| B2 (2) | Dark theme picker + default category existed | City/wind-limit settings, offline cache, categories + filter |
| B3 (1) | – | Small xUnit project |

Other findings
- Open-Meteo response (needs `wind_speed_unit=ms`): `{"current":{"time":"…","temperature_2m":0.8,"wind_speed_10m":4.90},"current_units":{…"wind_speed_10m":"m/s"}}`.
- `TodoStore.LoadAsync` is only called from the Todo list page → Home must call it too.
- `TodoItem` fields are plain properties → the edit page needs them observable so the list row refreshes.
- `EnsureCreated()` does not alter an existing DB → after adding the Category table the old `todos.db` must be deleted (documented in README).

## 2. Design decisions

Weather (typed-client pattern, as in the JokeOfTheDay in-class task):
- `Models/WeatherInfo.cs` – app model (Temperature °C, WindSpeed m/s, Time) + `IsKiteWeather(limit)` (strictly `> limit`).
- `Models/OpenMeteoResponse.cs` (+ nested `current`) – JSON DTO with `[JsonPropertyName]`; service maps it to `WeatherInfo`.
- `Services/IWeatherService.cs` – `Task<WeatherInfo> GetCurrentAsync(double lat, double lon)`.
- `Services/OpenMeteoWeatherService.cs` – takes `HttpClient` by ctor; URL built with `CultureInfo.InvariantCulture` and `wind_speed_unit=ms`; `GetAsync` → status check → `ReadFromJsonAsync`; catch `HttpRequestException`, `TaskCanceledException` (timeout), `JsonException` → throw `WeatherServiceException` (user-readable message).
- `Services/WeatherServiceException.cs`.
- `Services/LatestWeatherService.cs` – reads city from `IPreferences`, fetches, saves `CachedWeather {Weather, FetchedAt, City}` JSON in Preferences, on `WeatherServiceException when (cached != null)` returns the cache flagged offline. Cache is only used if the selected city is unchanged.
- `MauiProgram`: `AddHttpClient<IWeatherService, OpenMeteoWeatherService>(BaseAddress "https://api.open-meteo.com/", Timeout 10 s)`; `LatestWeatherService`, `HomeViewModel`, `HomePage` transient (typed client is transient → dependants transient).

Home:
- `ViewModels/HomeViewModel.cs` – `Top3` from `TodoStore.Items`: `!IsDone`, order by Priority desc then DueDate asc, `Take(3)`; weather props, `IsBusy`, `Status`; `RefreshCommand`; refresh on `OnAppearing` (calls `store.LoadAsync()` first).
- Shared ordering: `Services/TodoOrdering.cs` used by Home **and** `TodoListViewModel` (removes duplicated logic → criterion 4).
- `Views/HomePage.xaml(.cs)`: first tab; Top 3 list, weather card, "🪁 Kitesurf weather!" banner, ActivityIndicator, error label, Refresh button; a converter used on the banner/wind.

Settings (bonus: Preferences): new keys `City` (string) and `WindLimit` (double) (+ defaults "Vaasa" / 5.0); "Weather" section in `SettingsPage`: city `Picker` + wind-limit `Stepper` (1–15 m/s, step 0.5), saved via `OnXChanged` like the existing ones. Reset covers the new values.
Preset cities in one place (`Models/City.cs` + `Services/CityList.cs`): Vaasa 63.10/21.62 (default), Helsinki 60.17/24.94, Tampere 61.50/23.76, Turku 60.45/22.27, Oulu 65.01/25.47. No device location: inaccurate on desktops (Wi-Fi/IP based), needs permissions.

Category relation (criterion 3 = 8):
- `Models/Category.cs` {Id, Name (unique index), `List<TodoItem> Todos` = navigation only, not a column}; `TodoItem.CategoryId` + `Category` navigation; `DbSet<Category>` + seed Study/Home/Health/Work.
- `GetAllAsync` uses `.Include(t => t.Category)`; add `ICategoryRepository`/`EfCategoryRepository` (GetAll, Add). Update uses only `CategoryId` so EF does not re-save the Category graph.
- `AddTaskViewModel` and `SettingsViewModel` read categories from one place. Default category preference stays a name string.

Navigation / edit (criterion 2 = 5):
- Tabs: Home, Todo list, Settings. `AddTaskPage` becomes a pushed page (`Routing.RegisterRoute`) opened from a "+" button on the Todo list; the same page + VM serves **edit** mode (`IQueryAttributable`, parameter `TodoItem`), title "New task"/"Edit task".
- Tiny `INavigationService` (`GoToAsync` wrapper over `Shell.Current`) so VMs have no UI references.
- `TodoItem` fields → `[ObservableProperty]`; `TodoStore.UpdateAsync`. Tap on a row opens the editor.

Todo list filter (bonus): Picker "All + categories"; VM keeps a `VisibleItems` collection rebuilt from `store.Items` (sort + filter), re-applied on collection/IsDone changes.

Dark theme (bonus): theme picker already works; audit all pages (hard-coded `PrimaryColor`, `TextColor="Gray"`, `Gray200` stroke) → `AppThemeBinding`; verify Light and Dark, including new pages.

Tests (B3, deliberately small): new `LifeManagementApp.Tests` xUnit project with one class for `HomeViewModel`:
1. Top 3: unfinished only, priority desc then due date asc, max 3.
2. Kite limit: wind 5.0 → no alert, 5.1 → alert (strictly "over"); a custom limit from preferences is respected.
Fakes: in-memory `ITodoRepository` (→ real `TodoStore`), fake `IPreferences`, fake `IWeatherService`.
Risk: MAUI app TFMs are android/ios/maccatalyst/windows, so the test project must target `net9.0-windows10.0.19041.0` (Windows only). Fallback: link the pure-C# files into a plain `net9.0` test project.

README: description, how to run (Windows/Android, delete old DB note), Mermaid class diagram, DI lifetimes table with reasons, features/bonus list, **AI use** section.

## 3. Work order (one issue at a time: issue → branch → PR "Closes #N")

| Issue | Title | Depends on |
|---|---|---|
| #21 | Housekeeping | – |
| #22 | Weather service (REST) | #21 |
| #23 | Settings: city and wind limit (also removes the redundant "Sort by" picker; sorting stays on the Todo list buttons and is still saved in Preferences) | #21 |
| #24 | Home page: top 3 to-dos | #21 |
| #25 | Home: weather and kitesurf alert | #22, #23, #24 |
| #26 | Extra: cache latest weather | #25 |
| #27 | Category entity and relation | #21 |
| #28 | Edit page with navigation parameters | #27 |
| #29 | Extra: category filter | #27 |
| #30 | Extra: dark theme audit | #25, #28 |
| #31 | xUnit tests for HomeViewModel | #26 |
| #32 | README and final checks | all |

Validation per issue: build, launch on Windows, check the UI (Light + Dark), turn the network off for weather issues.

## 4. Decisions (confirmed)

1. No Joke tab/task.
2. Location = Picker of preset cities (Vaasa default, Helsinki, Tampere, Turku, Oulu). No device location.
3. Delete unused `JsonTodoRepository` (done in #21).
4. Add small `INavigationService`.
5. Plan lives in the repo as `Documents/Plans/ProjectPlan.md`.
6. All four extras: settings (city + wind limit), weather cache, dark theme, categories + filter.
7. Windows is the primary platform; Android must not break.
8. Sorting lives only on the Todo list buttons (no Sort by picker in Settings). The order is still saved in Preferences (`SettingsKeys.SortOrder`).

## 5. Status (update this when an issue is merged)

| Issue | Status |
|---|---|
| #21 Housekeeping | merged (PR #33) |
| #22 Weather service | merged (PR #34) |
| #23 Settings: city and wind limit | PR #35 open; merge it first |
| #24 – #32 | not started; next is #24 |

What exists in code that later issues should reuse:
- Weather: `IWeatherService.GetCurrentAsync(lat, lon)` → `WeatherInfo` (`IsKiteWeather(limit)` is strictly `>`); every failure is a `WeatherServiceException` with a user-readable message; typed client already registered in `MauiProgram`.
- Settings: `SettingsKeys.City` / `WindLimit`, `SettingsDefaults.City` ("Vaasa") / `WindLimit` (5.0), `CityList.Find(name)` → `City(Name, Latitude, Longitude)`, `CityList.Names`.
- `TodoListViewModel.Items` is its own **sorted copy** of `TodoStore.Items`, rebuilt by `ApplySort()` (also when the store changes). This is the start of the `VisibleItems` idea for the category filter (#29): extend `ApplySort`. #24 should extract the ordering rules into `Services/TodoOrdering.cs` and use it from both Home and `TodoListViewModel`.
- `TodoStore.LoadAsync()` loads only once; the Home page must call it too.

## 6. Working agreements and traps (for whoever continues)

Workflow (the user wants this exactly):
- One issue at a time. `git checkout main; git pull`, branch `N-short-slug`, commits `type: Message (#N)` with the trailer `Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>`, push, `gh pr create ... "Closes #N"`. The **user merges** the PRs and checks the UI themselves; ask before starting the next issue if the previous PR is not merged.
- Do not create extra issues for bugs found during an issue; fix them in the same branch. If asked to leave no history, rewrite the branch (`git reset --soft`, recommit, `git push --force-with-lease`). Remember to `git add` every file again after a soft reset.
- Stay in the course style: CommunityToolkit.Mvvm with the partial-property syntax (`LangVersion preview`), `[RelayCommand]`, `ObservableCollection`, `x:DataType` compiled bindings, `IPreferences` injected, interfaces + DI in `MauiProgram`, one class per file, short explanatory comments in the existing tone. No new libraries beyond what is already referenced (xUnit later).

Tooling traps:
- Windows PowerShell 5.1 shows UTF-8 characters such as `·` wrongly; the files are fine. Do not "fix" mojibake. Non-ASCII characters in a PowerShell command line can break parsing; use a small Python script or the edit tool instead.
- Build: `dotnet build -f net9.0-windows10.0.19041.0 --nologo -v q`. Stop the running app (`stopMauiDebugSession`) first or the exe is locked (MSB3027).
- The MauiDevFlow agent does **not** connect to this project, so the UI tree/screenshots are unavailable. Launch with `debugMauiProject` and ask the user to verify visually (list what to check). Do not commit `#if MAUI_DEVFLOW` code.
- On Windows the `CollectionView` did not redraw when `ObservableCollection.Move` was used; rebuild (Clear/Add) a separate collection instead.
- Old `todos.db` must be deleted once when the Category table is introduced (#27); `EnsureCreated()` does not migrate.
- The Bruno collection (`api-tests/`) still contains the unused Joke request; harmless.
- Keep the README (#32) honest about AI use; it is also graded as documentation.
