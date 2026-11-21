Project: LifeManagementApp
📌 Updated 3 Nov 2026: what changed?

Using AI is allowed. The assessment focuses on whether you understand your own code (section 4).
Points = work points × understanding coefficient. Each criterion gets a coefficient of 0–1 in the presentation (section 4).
Criterion 9 (Presentation, 5 p) was removed. Its points moved to criteria 1, 3 and 4. The maximum is still 50 + 5 p.
The pass limit for the project is counted from the work points before coefficients. The presentation cannot be retaken (section 4).
Pair work is possible only in the blended implementation 3007 (section 3).
The presentation format changed, and the README gets a short AI section (sections 7 and 8).
Changed parts are marked in yellow. The app requirements (sections 1–2) and the deadline did not change.

From the teacher: I have tried many ready-made apps for managing my everyday life: to-do lists, calendars and weather apps. None of them has been quite what I want. In a situation like this, a programmer usually thinks: "I'll build a better one myself."

Now it's your turn. You will build a .NET MAUI app that makes life management easier. The app combines a to-do list with the weather, and it tells you when it's kitesurfing weather.

The project is 50 % of the course grade. Its purpose is to bring together everything we learn on the course: UI with MAUI and XAML, separating business logic into ViewModels, binding with CommunityToolkit.Mvvm, a database with SQLite and EF Core Code First, a REST API with HttpClient, and dependency injection.

1. What the app must do (core requirements)
Page	Requirements
Home page	
Top 3 to-dos: unfinished items ordered first by priority (highest first) and then by due date (earliest first).
Current weather, e.g. from Open-Meteo: at least temperature and wind speed (m/s).
Kitesurf alert: if the wind is over 5 m/s, show the user a clear notice, e.g. "🪁 Kitesurf weather!".
To-do page	
All to-do items as a list (CollectionView).
The list can be sorted by priority or by time (due date).
A button for adding a new to-do item (on its own page or in a form). An empty title is not accepted.
Minimum fields of a to-do item: Id, Title, Priority (e.g. 1–5), DueDate, IsDone. You may add more (e.g. Notes, Category).

Pitfall: wind speed unit. By default Open-Meteo returns wind speed in km/h. Add wind_speed_unit=ms to the request, otherwise the 5 m/s limit triggers even in a light breeze. Example (Vaasa):
https://api.open-meteo.com/v1/forecast?latitude=63.10&longitude=21.62&current=temperature_2m,wind_speed_10m&wind_speed_unit=ms
Open-Meteo needs no API key. If you use another service, never push keys to Git.
2. Recommended structure
LifeManagementApp/
├── Models/        TodoItem.cs, WeatherInfo.cs
├── ViewModels/    HomeViewModel.cs, TodoListViewModel.cs, AddTodoViewModel.cs
├── Views/         HomePage.xaml, TodoListPage.xaml, AddTodoPage.xaml
├── Services/      IWeatherService.cs, OpenMeteoWeatherService.cs, ITodoRepository.cs, TodoRepository.cs
├── Data/          AppDbContext.cs
├── AppShell.xaml  (navigation)
├── MauiProgram.cs (DI registrations)
└── README.md      (project plan, see the P7 template)
The course code conventions apply: one class per file, .NET 9 / MAUI 9.

3. Alone or in a pair? CLARIFIED
In the daytime implementations (3006, 3008) the project is done alone. Pair work is possible only in the blended implementation (TT00BI73-3007). Pairs sign up in the Group choice activity Project pairs (instructions there). If you work alone, you don't need to do anything. The assessment is the same alone and in a pair.

A pair works in one shared repository, and both members submit its Git link.
Both members must be able to explain the whole app, including the parts their partner wrote. Questions are asked to both in the presentation.
The grade is individual: the work points are the same for the pair, but each member gets their own understanding coefficients. NEW
Both commit with their own accounts. The commit history must show that both have written code.
If the pair splits up, tell the teacher right away. Both continue with their own copy of the shared repository.
4. Assessment: work points × understanding coefficient CHANGED
🤖 Using AI in the project is allowed. You can use e.g. Copilot, ChatGPT or Claude to write code, find bugs and learn new things. Professionals use these tools too.

The assessment does not ask who or what wrote the code. It asks whether you understand it. If you write the code yourself, you usually understand it already, and this model changes nothing for you.

If you use AI, set aside time to understand what it wrote. Ask it "why", try changing the code, and explain it to yourself or your partner. A great app that you cannot explain gives no points.

How the project points are formed:

Work points (0–50 p): the teacher assesses the submitted repository with the table below before the presentation.
Understanding coefficient (0–1) for each criterion: given in the presentation. You get 1–2 questions per criterion.
Project points = Σ (work points of the criterion × its coefficient). The sum is rounded to the nearest whole number.
The coefficient applies to criteria 1–7 and bonuses B2 and B3. Criterion 8 (Git and documentation) and bonus B1 are counted without a coefficient.

Work points: the levels are guidelines, and points between them are possible. A higher level includes the lower one.

#	Criterion	Levels	Max	Coefficient
1	Functionality	Home page top 3 (2 p) · weather (2 p) · kitesurf alert (1 p) · to-do list and two sort orders (3 p) · adding a new to-do with validation (2 p)	10	yes
2	Pages and navigation	1 = everything on one page · 3 = several pages with Shell navigation · 5 = plus passing parameters between pages (e.g. a to-do edit page)	5	yes
3	Storage	1 = in-memory list only · 3 = JSON file · 6 = SQLite + EF Core Code First (DbContext, add, query, update) · 8 = plus a relation (e.g. Category 1–N TodoItem) and async queries	8	yes
4	Architecture (MVVM)	1 = logic in code-behind · 3 = a ViewModel for each page · 6 = Model + ViewModel + Service: data access in services, and the ViewModel does not reference UI controls · 9 = plus a clear folder structure, interfaces and no duplicated logic	9	yes
5	Binding and Community Toolkit	2 = bindings with hand-written INotifyPropertyChanged · 4 = CommunityToolkit.Mvvm: [ObservableProperty], [RelayCommand], ObservableCollection · 5 = plus compiled bindings (x:DataType) and at least one converter	5	yes
6	Dependency injection	0 = no DI · 3 = services registered in MauiProgram and constructor injection · 5 = plus interfaces (IWeatherService, ITodoRepository), pages and ViewModels via DI, and lifetimes (Singleton/Transient) chosen with a reason	5	yes
7	REST API	2 = weather is fetched and shown · 4 = async/await, JSON mapped to a model object, correct unit · 5 = plus error handling (no network → message, UI does not freeze, loading state)	5	yes
8	Git and documentation	1 = code in Git · 2 = regular, descriptive commits throughout the project · 3 = plus a README (P7 template: description, how to run, class diagram, AI use)	3	no
9	Presentation and understanding of own code	Removed. Understanding is now assessed with a coefficient for each criterion.	5	–
Total	50	 
Bonus (up to +5 p, on top of the 50 points):

Bonus	What	Max	Coefficient
B1 Git workflow	New features are defined as issues and merged into main via feature branches (merge or pull request referring to the issue)	2	no
B2 Extra features	1 p per working extra feature from the list below (or your own idea, ask the teacher)	2	yes
B3 Tests	xUnit tests for a ViewModel or a service (e.g. top 3 ordering, kite limit)	1	yes
Extra features for the bonus: marking a to-do done, editing and deleting (e.g. SwipeView) · a settings page for location and wind limit (Preferences) · hourly forecast and "best kite window today" · caching the latest weather for offline use · device location (Geolocation) · dark theme · categories and filtering.

Understanding coefficient NEW

Coefficient	What it means
1	You explain what the code does and why it is done this way. You answer follow-up questions and can change the code.
0.75	You explain what and why, but some detail remains uncertain.
0.5	You explain what the code does, but cannot justify the solution or change it.
0.25	You recognise where the feature is in the code, but the explanation stays general.
0	You cannot explain the criterion.
Example questions in the presentation (the teacher picks spots from your code):

Criterion	Example question
1 Functionality	Show where the top 3 is selected. How is the order decided? Where is an empty title rejected?
2 Pages	How do you move between pages? How does the parameter reach the edit page?
3 Storage	What happens when you add a to-do: how far does the data travel? What does the DbContext do? Why is the query async?
4 Architecture	Why is this logic in a service and not in the ViewModel? What would happen if it were in code-behind?
5 Binding	What does [ObservableProperty] generate? Why ObservableCollection and not List?
6 DI	Where does the ViewModel get the service from? Why is this one Singleton and that one Transient?
7 REST	How does JSON become an object? What happens if there is no network?
B2, B3	Show an extra feature or a test and explain how it works.
Passed project: the app starts, the home page and the to-do page work (at least with an in-memory list), and the work points before coefficients are at least 20 p. So a project can be passed even if few or no points remain after the coefficients. The presentation cannot be retaken: understanding is assessed once.

5. Course grade CLARIFIED
Exam 0–50 p + project 0–50 p (+ bonus 0–5 p, weighted with the coefficients) = course points.

To pass the course you need: at least 20 p from the exam, a passed project (see above) and at least 40 course points.

Points	Grade
0–39	fail
40–51	1
52–63	2
64–75	3
76–87	4
88–	5
6. Example: a grade 3 project CHANGED
The project is good and works, but some higher levels are missing. In the presentation the student explains most of the code well:

Criterion	What the project has	Work	In the presentation	Coeff.	p
1 Functionality	Everything works, but adding accepts an empty title and sorting works only by priority	8/10	Shows where the top 3 is computed and explains the ordering	1	8
2 Pages	Three pages with Shell navigation, no parameter passing	3/5	Explains the routes and navigation	1	3
3 Storage	SQLite + EF Core Code First, one table, no relations	6/8	Explains the DbContext and how adding works	1	6
4 Architecture	A ViewModel for each page, but the ViewModel uses DbContext and HttpClient directly (no services)	3/9	Knows what a ViewModel is, but the reasoning is partly uncertain	0.75	2.25
5 Binding	[ObservableProperty], [RelayCommand] and ObservableCollection, no x:DataType	4/5	Cannot say what [ObservableProperty] generates or why ObservableCollection is used	0.5	2
6 DI	DbContext, ViewModels and pages registered and injected, no interfaces	3/5	Explains the registrations and constructor injection	1	3
7 REST	Async fetch and JSON to a model object, but the app crashes without a network	4/5	Explains async/await, JSON mapping remains uncertain	0.75	3
8 Git	Regular commits, no README	2/3	–	no coeff.	2
Total	33/50	 	 	29.25 → 29
If the same student gets 35 p from the exam, the course points are 29 + 35 = 64 → grade 3.

Same code, different presentation:

If the student explains everything (all coefficients 1): project 33 p → 33 + 35 = 68 → grade 3.
If the student cannot explain anything (coefficients 0): the project is passed (work points 33 ≥ 20), but it gives only the 2 Git points → 2 + 35 = 37 → fail.
The quickest improvements for this project: understand your own code (here about +4 p without a single new line of code), a service layer and interfaces (criteria 4 and 6, up to +8 p), plus error handling and a README (+2 p).

7. Submission and presentation CHANGED
Git repository: VAMK Git (add tuo as a collaborator) or GitHub (add tuo-vamk as a collaborator) → the repo can be private. On any other service the repo must be public.
A section "AI use" in the README (a few lines): which tools you used and for what. It gives no points and costs no points. It helps the teacher choose the presentation questions. NEW
Submit the Git link in the Project work section by 10 November 2026 at 23:59. The version at submission time is the one assessed.
Book a presentation slot in the Scheduler activity. Alone: 15 min. Pair (3007): two consecutive slots (30 min).
Presentation (~15 min): demo (~3 min) → the teacher's questions on your code, criterion by criterion (~8 min) → a small change to the code during the presentation, e.g. "add a new field to TodoItem and show it in the list" (~4 min). You don't use AI in the presentation. You may look at your own code and README. NEW
8. Checklist before submitting
☐ The app builds and runs from a clean clone (Windows or Android)
☐ Home page: top 3, weather, kitesurf alert (test the limit too, e.g. by temporarily changing it)
☐ To-do page: list, two sort orders, adding
☐ README is up to date, and the repo has no secrets and no bin/ or obj/ folders
☐ The README has an "AI use" section NEW
☐ You can explain the code of every criterion 1–7: what it does and why. Try making a small change to it without AI. NEW
☐ The teacher is added as a collaborator, and the link is submitted in Moodle
☐ Pair (3007): both members' commits are visible, and both can explain the whole app
Ask questions in the Public discussion or in the Private discussion with the teacher.