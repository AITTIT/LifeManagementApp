In-class task 11 – Joke of the day from a REST API (HttpClient)
In task 10 the JSON came from the app's own file. Now the JSON comes from the web, from someone else's server. Task1 gets a fourth tab, Joke, which fetches a joke from the open JokeAPI (v2.jokeapi.dev, no API key). You first write the request the short way, break it on purpose and then write a version that tells the user clearly what went wrong. Finally you make a joke of the day: the joke is fetched once a day and cached, so it is shown even without a network. The weather from Open-Meteo in your project is built exactly the same way.
Goals
You read the API's documentation and its response in a browser before you write any C#.
You write C# classes that match the JSON response (nested object, nullable fields, [JsonPropertyName]).
You fetch data with an HttpClient that the service receives through DI (typed client, AddHttpClient).
You recognise why the same JSON code works for your own file but not for an API response.
You handle network errors – no network, timeout, a 4xx response – and translate them into one exception a user can understand.
You build a cache that saves requests and works without a network.
Before you start
The starting point is Task1 after task 9 (three tabs, IPreferences registered). Task 10 may or may not be done: this task does not touch the storage of the task list.
Do parts 1–3 in order and commit after every part (Task 11 part 1 …). It takes about 80 minutes.
The category is always Programming and ?safe-mode is always on. JokeAPI also has offensive jokes, and the filtering is not perfect.
The whole class looks like one IP address to JokeAPI, and the limit is 120 requests per minute. Never call the API in a loop or from a timer.
Slides: Joke of the day – data from a REST API (httpclient-joke-material).

The files after this task:

Task1/
├── Models/
│   ├── Joke.cs                   <-- NEW (you write it, part 1)
│   ├── JokeFlags.cs              <-- NEW (given, part 1)
│   ├── JokeApiError.cs           <-- NEW (given, part 2)
│   └── DailyJoke.cs              <-- NEW (given, part 3)
├── Services/
│   ├── IJokeService.cs           <-- NEW (given, part 1)
│   ├── JokeService.cs            <-- NEW: short version (part 1) -> long version (part 2)
│   ├── JokeServiceException.cs   <-- NEW (given, part 2)
│   ├── JokeOfTheDayService.cs    <-- NEW (skeleton given, part 3)
│   └── SettingsKeys.cs           <-- + JokeOfTheDay (part 3)
├── ViewModels/
│   └── JokeViewModel.cs          <-- NEW (skeleton given, parts 1–3)
├── Views/
│   ├── JokePage.xaml             <-- NEW (given, part 1; two buttons in part 3)
│   └── JokePage.xaml.cs          <-- NEW (given, part 1; OnAppearing in part 3)
├── AppShell.xaml                 <-- a fourth tab (part 1)
└── MauiProgram.cs                <-- typed client + registrations (parts 1 and 3)
Part 1 – The first request
1.1 Read the response first in a browser. Open the address below and refresh a few times until you have seen both joke types. Also try the parameters &type=single and &type=twopart. Write down: which fields are in both types, which only in one, and which field is a nested object?

https://v2.jokeapi.dev/joke/Programming?safe-mode
1.2 NuGet. Install the package Microsoft.Extensions.Http in the Task1 project. Pick the latest 9.0.x version, not 10.x, because Task1 is a .NET 9 project. The package provides the AddHttpClient method.


1.3 The models. JokeFlags is given. Write the Joke class yourself from the JSON you saw in 1.1 (TODO 1a). Write the field "joke" first as public string? Joke { get; set; } and read the compiler error. Then fix it: [JsonPropertyName("joke")] public string? Text { get; set; }.

// ===== Models/JokeFlags.cs (given) =====
namespace Task1.Models
{
    // "flags": { "nsfw": false, "religious": false, ... } – a nested JSON object = its own class
    public class JokeFlags
    {
        public bool Nsfw { get; set; }
        public bool Religious { get; set; }
        public bool Political { get; set; }
        public bool Racist { get; set; }
        public bool Sexist { get; set; }
        public bool Explicit { get; set; }
    }
}
// ===== Models/Joke.cs (you write the properties) =====
using System.Text.Json.Serialization;

namespace Task1.Models
{
    // One JokeAPI joke. Two shapes: "single" has "joke", "twopart" has "setup" + "delivery".
    public class Joke
    {
        public bool Error { get; set; }

        // TODO 1a: write the rest from the JSON you saw in the browser:
        //          category, type, joke, setup, delivery, flags, safe, id, lang
        //          - C# names (Category, not category) – part 2 shows why this still works
        //          - a field that exists in only one shape -> nullable (string?)
        //          - "flags" -> public JokeFlags Flags { get; set; } = new();
        //          - try "public string? Joke" first and read the compiler error (1.3)
    }
}
1.4 The service. The interface is given. Write the request in the JokeService class the short way (TODO 1b): one GetFromJsonAsync call. The service does not create an HttpClient itself: it asks for one in its constructor. The base address and the timeout are set in 1.6.

// ===== Services/IJokeService.cs (given) =====
using System.Threading.Tasks;
using Task1.Models;

namespace Task1.Services
{
    public interface IJokeService
    {
        // Returns one joke from the given category ("Programming", "Pun", ...).
        // From part 2 on: throws JokeServiceException when anything goes wrong.
        Task<Joke> GetJokeAsync(string category);
    }
}
// ===== Services/JokeService.cs (part 1: short version) =====
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Task1.Models;

namespace Task1.Services
{
    public class JokeService : IJokeService
    {
        private readonly HttpClient http;

        // Asks for an HttpClient – MauiProgram gives one with BaseAddress and Timeout already set.
        public JokeService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<Joke> GetJokeAsync(string category)
        {
            // TODO 1b: GET "joke/{category}?safe-mode" with http.GetFromJsonAsync<Joke>(...)
            //          and return the joke. If the result is null, throw an InvalidOperationException.
            //          The path does NOT start with "/" – the base address already ends with one.
            throw new NotImplementedException();
        }
    }
}
1.5 The ViewModel. The properties and the command skeleton are given. Complete TODO 1c (get a joke and show it) and TODO 1d (a two-shaped joke into two strings). The ViewModel does not expose the Joke object: the page binds only to strings, so it does not need to know the two shapes of a joke.

// ===== ViewModels/JokeViewModel.cs (part 1) =====
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Task1.Models;
using Task1.Services;

namespace Task1.ViewModels
{
    public partial class JokeViewModel : ObservableObject
    {
        private const string Category = "Programming";
        private readonly IJokeService jokeService;

        [ObservableProperty]
        public partial string Heading { get; set; } = "Joke";

        [ObservableProperty]
        public partial string Setup { get; set; } = "";

        [ObservableProperty]
        public partial string Punchline { get; set; } = "";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanShowPunchline))]
        public partial bool HasPunchline { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanShowPunchline))]
        public partial bool IsPunchlineVisible { get; set; }

        [ObservableProperty]
        public partial bool IsBusy { get; set; }

        [ObservableProperty]
        public partial string Status { get; set; } = "";

        // The "Show punchline" button is visible only for a two-part joke whose punchline is hidden.
        public bool CanShowPunchline => HasPunchline && !IsPunchlineVisible;

        public JokeViewModel(IJokeService jokeService)
        {
            this.jokeService = jokeService;
        }

        [RelayCommand]
        private async Task NewJokeAsync()                  // -> NewJokeCommand
        {
            if (IsBusy) return;                            // two quick taps = one request
            IsBusy = true;
            Status = "";
            try
            {
                // TODO 1c: get a joke from jokeService (category: Category),
                //          set Heading = "Random joke" and call Show(joke).
            }
            catch (Exception ex)                           // part 2 replaces this
            {
                Status = ex.Message;
            }
            finally
            {
                IsBusy = false;                            // also after an error
            }
        }

        [RelayCommand]
        private void ShowPunchline()
        {
            IsPunchlineVisible = true;
        }

        private void Show(Joke joke)
        {
            // TODO 1d: "twopart" -> Setup = joke.Setup, Punchline = joke.Delivery, HasPunchline = true
            //          "single"  -> Setup = joke.Text,  Punchline = "",            HasPunchline = false
            //          Always IsPunchlineVisible = false. Use ?? "" with the nullable strings.
        }
    }
}
1.6 The page, the tab and DI. The page is given ready-made. Today's topic is HTTP, not XAML. Add a fourth tab to AppShell, and add the typed client and the registrations of the ViewModel and the page to MauiProgram.

<?xml version="1.0" encoding="utf-8" ?>
<!-- ===== Views/JokePage.xaml (given) ===== -->
<ContentPage x:Class="Task1.Views.JokePage"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:Task1.ViewModels"
             x:DataType="vm:JokeViewModel"
             Title="Joke">
    <ScrollView>
        <VerticalStackLayout Padding="20" Spacing="14">

            <Label Text="{Binding Heading}" FontSize="22" FontAttributes="Bold" />

            <Border Padding="16" StrokeShape="RoundRectangle 12">
                <VerticalStackLayout Spacing="12">
                    <Label Text="{Binding Setup}" FontSize="18" />
                    <Label Text="{Binding Punchline}" FontSize="18" FontAttributes="Bold"
                           TextColor="{StaticResource Primary}"
                           IsVisible="{Binding IsPunchlineVisible}" />
                </VerticalStackLayout>
            </Border>

            <Button Text="Show punchline"
                    Command="{Binding ShowPunchlineCommand}"
                    IsVisible="{Binding CanShowPunchline}" />

            <ActivityIndicator IsRunning="{Binding IsBusy}" IsVisible="{Binding IsBusy}" />
            <Label Text="{Binding Status}" TextColor="{StaticResource Gray500}" />

            <!-- Part 3 replaces this button with a Grid of two buttons -->
            <Button Text="Another one" Command="{Binding NewJokeCommand}" />

            <Label Text="Jokes: JokeAPI · v2.jokeapi.dev" FontSize="12"
                   TextColor="{StaticResource Gray500}" />

        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
// ===== Views/JokePage.xaml.cs (given) =====
using Task1.ViewModels;

namespace Task1.Views
{
    public partial class JokePage : ContentPage
    {
        private readonly JokeViewModel vm;

        public JokePage(JokeViewModel vm)
        {
            InitializeComponent();
            this.vm = vm;
            BindingContext = vm;
        }

        // Part 3: OnAppearing loads the joke of the day here.
    }
}
<!-- ===== AppShell.xaml – add a fourth tab inside the TabBar ===== -->
        <ShellContent Title="Joke" Route="JokePage"
                      ContentTemplate="{DataTemplate views:JokePage}" />
// ===== MauiProgram.cs – add to the registrations =====
            builder.Services.AddHttpClient<IJokeService, JokeService>(client =>
            {
                client.BaseAddress = new Uri("https://v2.jokeapi.dev/");   // ends with "/"
                client.Timeout = TimeSpan.FromSeconds(10);
            });
            builder.Services.AddTransient<JokeViewModel>();
            builder.Services.AddTransient<JokePage>();
Typed client: AddHttpClient<IJokeService, JokeService> means: "whoever asks for IJokeService gets a JokeService with an HttpClient set up like this". The factory recycles the connections, so new HttpClient() is not written anywhere. A typed client is registered as transient, so the classes that use it are transient too.
1.7 Run. Open the Joke tab and press Another one a few times. In a two-part joke the punchline appears only after the button. Commit: Task 11 part 1.

Part 2 – When the network says no (predict first)
2.1 Predict first: task 10's code on the API's JSON. Add the experiment below to the beginning of GetJokeAsync for a moment. It reads the same response as text and converts it twice: once like in task 10 and once with the web defaults. Before running, write in the table what each line prints in the Output window.

// TEMPORARY experiment (part 2.1) – at the start of GetJokeAsync, remove afterwards.
// usings: System.Diagnostics, System.Text.Json
string raw = await http.GetStringAsync($"joke/{category}?safe-mode");
Debug.WriteLine(raw);

Joke? a = JsonSerializer.Deserialize<Joke>(raw);                       // like task 10
Debug.WriteLine($"A: Type='{a?.Type}' Setup='{a?.Setup}' Text='{a?.Text}' Id={a?.Id}");

var web = new JsonSerializerOptions(JsonSerializerDefaults.Web);
Joke? b = JsonSerializer.Deserialize<Joke>(raw, web);                  // web defaults
Debug.WriteLine($"B: Type='{b?.Type}' Setup='{b?.Setup}' Text='{b?.Text}' Id={b?.Id}");
Line	Prediction	Observed
A	 	 
B	 	 
Explanation (open only after trying)

2.2 Predict first: a typo in the category. Change the ViewModel's constant for a moment to Category = "Programmin" (no g). What does the user read on the Status line after pressing Another one? Write your prediction first. Then open the same wrong address in a browser: what did the server actually answer? Put the category back.


Explanation (open only after trying)

2.3 The long version. JokeApiError and JokeServiceException are given. Rewrite GetJokeAsync in three steps (TODO 2a–2c) and translate the technical exceptions into JokeServiceExceptions (TODO 2d, the first one is given as an example). In the ViewModel, change catch (Exception ex) to catch (JokeServiceException ex) (TODO 2e). Other exceptions are programming errors, and we want to see them.

// ===== Models/JokeApiError.cs (given) =====
namespace Task1.Models
{
    // JokeAPI's error body (HTTP 400), for example:
    // { "error": true, "internalError": false, "code": 106, "message": "No matching joke found",
    //   "causedBy": [ ... ], "additionalInfo": "The specified category/ies is/are invalid. ...", "timestamp": ... }
    public class JokeApiError
    {
        public bool Error { get; set; }
        public bool InternalError { get; set; }
        public int Code { get; set; }
        public string Message { get; set; } = "";
        public string? AdditionalInfo { get; set; }
    }
}
// ===== Services/JokeServiceException.cs (given) =====
using System;

namespace Task1.Services
{
    // The only exception that leaves JokeService. The message is written for the user.
    public class JokeServiceException : Exception
    {
        public JokeServiceException(string message, Exception? inner = null)
            : base(message, inner)
        {
        }
    }
}
// ===== Services/JokeService.cs (part 2: long version) =====
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Task1.Models;

namespace Task1.Services
{
    public class JokeService : IJokeService
    {
        private readonly HttpClient http;

        public JokeService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<Joke> GetJokeAsync(string category)
        {
            string url = $"joke/{Uri.EscapeDataString(category)}?safe-mode";
            try
            {
                using HttpResponseMessage response = await http.GetAsync(url);   // does NOT throw on 400
                Debug.WriteLine($"GET {url} -> {(int)response.StatusCode}");

                // TODO 2a: 429 Too Many Requests (HttpStatusCode.TooManyRequests)
                //          -> throw new JokeServiceException("Too many requests. Wait a minute and try again.");

                // TODO 2b: any other status that is not 2xx (!response.IsSuccessStatusCode):
                //          JokeApiError? error = await TryReadErrorAsync(response);
                //          throw a JokeServiceException with error?.AdditionalInfo,
                //          or $"The joke server answered {(int)response.StatusCode}." if there is no error body.

                // TODO 2c: read the joke: await response.Content.ReadFromJsonAsync<Joke>()
                //          null or joke.Error -> throw new JokeServiceException("The response did not contain a joke.");
                //          otherwise return the joke.
                throw new NotImplementedException();
            }
            catch (HttpRequestException ex)
            {
                throw new JokeServiceException("No connection to the joke server. Check the network.", ex);
            }
            // TODO 2d: two more translations, the original exception goes in as "inner":
            //          TaskCanceledException -> "The joke server did not answer in time."
            //          JsonException         -> "The joke server sent something that is not a joke."
        }

        // Returns null if the body is not JokeAPI's error JSON (for example an HTML maintenance page).
        private static async Task<JokeApiError?> TryReadErrorAsync(HttpResponseMessage response)
        {
            try
            {
                return await response.Content.ReadFromJsonAsync<JokeApiError>();
            }
            catch (Exception ex) when (ex is JsonException || ex is NotSupportedException)
            {
                return null;
            }
        }
    }
}
Why is a JokeServiceException thrown inside try not caught by our own catch blocks? They catch only the listed types (HttpRequestException, TaskCanceledException, JsonException), and JokeServiceException is a different type. The original exception is kept as InnerException, so debugging does not suffer.
2.4 Break it in three ways. Do each experiment, write the message the user sees in the table and restore the code before the next one.

#	Experiment	Message to the user
1	Category = "Programmin" again	 
2	MauiProgram: client.Timeout = TimeSpan.FromMilliseconds(1);	 
3	No network: airplane mode or Wi-Fi off on Windows, airplane mode in the Android emulator	 
In every experiment the spinner must stop and the app must keep working. Commit: Task 11 part 2.

Part 3 – The joke of the day
JokeAPI has no "joke of the day" address, so you make one. The first joke of the day is saved in Preferences as a JSON string (tasks 9 and 10) and reused until the date changes. If there is no network, the previously saved joke is shown. The cache fits in Preferences because it is one small value and not user data: if it is lost, a new joke is fetched.


3.1 Add the key to SettingsKeys and create the given DailyJoke.

// ===== Services/SettingsKeys.cs – one new key =====
        public const string JokeOfTheDay = "joke_of_the_day";      // string: a DailyJoke as JSON (a cache)
// ===== Models/DailyJoke.cs (given) =====
namespace Task1.Models
{
    // What is saved in Preferences: the date and the joke of that date.
    public class DailyJoke
    {
        public string Date { get; set; } = "";     // "yyyy-MM-dd"
        public Joke Joke { get; set; } = new();
    }
}
3.2 JokeOfTheDayService. Reading and saving are given. Complete GetTodayAsync (TODO 3a–3c). Note the exception filter catch … when: the exception is caught only if the condition is true. If there is no saved joke, the exception goes on to the ViewModel, which shows the error message.

// ===== Services/JokeOfTheDayService.cs (part 3) =====
using System;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using Task1.Models;

namespace Task1.Services
{
    // JokeAPI has no "joke of the day" – this class makes one:
    // the first joke of each day is saved in Preferences and reused until the date changes.
    public class JokeOfTheDayService
    {
        private const string Category = "Programming";
        private readonly IJokeService jokeService;
        private readonly IPreferences preferences;

        public JokeOfTheDayService(IJokeService jokeService, IPreferences preferences)
        {
            this.jokeService = jokeService;
            this.preferences = preferences;
        }

        // "2026-10-28" – the same text in every culture, so == works.
        // Part 3.4: add .AddDays(1) after DateTime.Today for a moment to simulate tomorrow.
        public static string Today => DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        public async Task<DailyJoke> GetTodayAsync()
        {
            DailyJoke? saved = ReadSaved();

            // TODO 3a: if saved is not null and saved.Date == Today -> return saved (no request at all)

            try
            {
                // TODO 3b: get a joke (Category) from jokeService, create
                //          new DailyJoke { Date = Today, Joke = joke }, Save(...) it and return it.
                throw new NotImplementedException();
            }
            catch (JokeServiceException) when (saved != null)
            {
                // TODO 3c: no network (or another JokeService error), but there is an older joke -> return saved.
                throw;
            }
        }

        // Debug helper (bonus 6): forget the saved joke.
        public void Forget()
        {
            preferences.Remove(SettingsKeys.JokeOfTheDay);
        }

        private DailyJoke? ReadSaved()
        {
            string json = preferences.Get(SettingsKeys.JokeOfTheDay, "");
            if (json == "") return null;
            try
            {
                return JsonSerializer.Deserialize<DailyJoke>(json);
            }
            catch (JsonException)
            {
                return null;                       // a broken cache = no cache (task 10: a broken file)
            }
        }

        private void Save(DailyJoke daily)
        {
            preferences.Set(SettingsKeys.JokeOfTheDay, JsonSerializer.Serialize(daily));
        }
    }
}
3.3 The ViewModel, the page and DI. Add the service and the command LoadTodayAsync to the ViewModel (TODO 3d). Add OnAppearing to the page (TODO 3e) and replace the Another one button with a row of two buttons. Register the service.

// ===== ViewModels/JokeViewModel.cs – part 3 changes =====
        private readonly JokeOfTheDayService jokeOfTheDay;                 // NEW

        public JokeViewModel(IJokeService jokeService, JokeOfTheDayService jokeOfTheDay)
        {
            this.jokeService = jokeService;
            this.jokeOfTheDay = jokeOfTheDay;                              // NEW
        }

        [RelayCommand]
        private async Task LoadTodayAsync()                                // -> LoadTodayCommand
        {
            if (IsBusy) return;
            IsBusy = true;
            Status = "";
            try
            {
                // TODO 3d: DailyJoke daily = await jokeOfTheDay.GetTodayAsync(); Show(daily.Joke);
                //          Heading = "Joke of the day" when daily.Date == JokeOfTheDayService.Today,
                //          otherwise $"Joke of {daily.Date} (offline)".
            }
            catch (JokeServiceException ex)
            {
                Status = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }
// ===== Views/JokePage.xaml.cs – add =====
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // TODO 3e: only when nothing is shown yet (vm.Setup == ""):
            //          await vm.LoadTodayCommand.ExecuteAsync(null);
            //          Shell keeps the page, so without the check every tab switch would load again.
        }
<!-- ===== Views/JokePage.xaml – replace the "Another one" button ===== -->
            <Grid ColumnDefinitions="*,*" ColumnSpacing="10">
                <Button Text="Joke of the day" Command="{Binding LoadTodayCommand}" />
                <Button Grid.Column="1" Text="Another one" Command="{Binding NewJokeCommand}" />
            </Grid>
// ===== MauiProgram.cs – add =====
            builder.Services.AddTransient<JokeOfTheDayService>();   // transient: it uses the typed client
3.4 Test – predict first. Fill in the prediction column before each step. The GET … -> 200 line in the Output window tells whether a request went to the network.

#	Do	Prediction: heading · joke · was there a GET?	Observed
a	Start the app and open the Joke tab.	 	 
b	Press Another one, then Joke of the day.	 	 
c	Close the app, start it again and open Joke.	 	 
d	Add .AddDays(1) to the Today property, cut the network, start and open Joke.	 	 
e	Restore the network and press Joke of the day.	 	 
Explanation (open only after trying)

3.5 Reflection in the commit message. Do you and your neighbour see the same joke of the day today? Why or why not? At most how many requests does your app make per day if the user only opens the Joke tab? Commit: Task 11 part 3.

Acceptance criteria
#	Criterion
1	The fourth tab Joke works, and the three old tabs work as before.
2	Another one shows a random Programming joke. In a two-part joke the punchline appears only after Show punchline, and the button then disappears.
3	The spinner runs during the request and stops also after an error.
4	Wrong category → the user sees the server's explanation (additionalInfo), not "400 (Bad Request)".
5	No network → "No connection …", timeout → "… did not answer in time." The app does not crash.
6	The joke of the day is the same after a restart, and no request is sent (Output).
7	A new day without a network → the old joke and "(offline)" in the heading.
8	There is no new HttpClient() in the code. The base address and the timeout are only in MauiProgram.
9	The ViewModel knows neither HttpClient nor IPreferences, and it catches only JokeServiceException.
Hints and common mistakes
AddHttpClient not found: the NuGet package Microsoft.Extensions.Http is missing. Pick 9.0.x, not 10.x.
CS0542 (member names cannot be the same as their enclosing type): a property Joke in the class Joke → [JsonPropertyName("joke")] public string? Text.
All fields empty, no exception: a name is misspelled, or JsonSerializer is used without the web defaults (step 2.1).
Unable to resolve service for type JokeService: the service is registered as an interface. Ask for IJokeService in the constructor, not JokeService.
The address goes wrong: the base address ends with /, and the path does not start with one ("joke/…"). Check the GET line in the Output window.
"429 Too Many Requests": something calls the API too often (a loop, a timer or OnAppearing without the check). Wait a minute.
The spinner never stops: IsBusy = false is not in the finally block.
The app crashes on a network error: the exception is not a JokeServiceException (the translation in 2d is missing), or the ViewModel's catch is missing.
Binding error LoadTodayCommand not found: the XAML button was added before the command of part 3. A compiled binding (x:DataType) checks the names at build time.
The Android emulator gets no connection: check with the emulator's browser whether it has a network. The INTERNET permission is already in the MAUI template's AndroidManifest.xml.
The joke of the day does not change in test d: the app was not restarted, or OnAppearing does not load because Setup is not empty. Press Joke of the day.
AI suggests .Result, a new HttpClient() per call or Newtonsoft.Json: none of these is needed. Use await, the typed client and System.Net.Http.Json.
static and namespace traps: every class in its own file, models in the namespace Task1.Models and services in Task1.Services.
Bonus tasks (optional)
The same joke for everyone. Fetch https://v2.jokeapi.dev/info once and read jokes.idRange.en from it (e.g. [0, N]). Compute an id from the day number: DateOnly.FromDateTime(DateTime.Today).DayNumber % (N + 1). Fetch joke/Any?idRange={id}&safe-mode. If the answer is 400 (the joke is not "safe"), try the next id a few times at most. Now the whole class sees the same joke of the day.
Category in the settings. Add a Picker (Programming, Pun, Misc, Spooky, Christmas) and a key joke_category to the Settings tab. Another one uses the chosen category. What should happen to the joke of the day when the category changes?
FakeJokeService. Write a class that implements IJokeService and returns two fixed jokes in turn (and throws a JokeServiceException every third time). Change one line in MauiProgram: AddTransient<IJokeService, FakeJokeService>(). Write in the commit message which other files had to change.
Show the quota. Read the header RateLimit-Remaining from a successful response (response.Headers.TryGetValues(...)) and write it to the Output window.
Share the joke. Add a button that calls Share.Default.RequestAsync(new ShareTextRequest { Text = …, Title = "Joke of the day" }).
Forget the cache. Add a button to the Settings tab that calls JokeOfTheDayService.Forget(). Test that the next visit fetches a new joke.
For your project
Build your project's weather service right away with the same structure: IWeatherService + a typed client (https://api.open-meteo.com/), classes that match Open-Meteo's response ([JsonPropertyName("wind_speed_10m")]), errors translated into your own exception and the latest weather in a cache. Remember wind_speed_unit=ms and CultureInfo.InvariantCulture in the coordinates.

Submission
Submit the Git link to your OnLecture_MAUI fork in this assignment. The last commit is Task 11 part 3 (or a bonus commit), and the reflection of step 3.5 is in the commit message. The task is not graded.