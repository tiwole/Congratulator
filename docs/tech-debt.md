# Technical Debt

## TD-001: Упростить GetPersonsResponse — один список вместо двух

**Статус:** Открыт
**Приоритет:** Средний

**Текущее поведение:**
`GetPersonsResponse` содержит два списка: `TodayBirthdays` и `UpcomingBirthdays`. Разделение происходит на сервере в `PersonRepository` (сравнение `BirthDate.Day/Month` с текущей датой).

**Целевое поведение:**
- `GetPersonsResponse` содержит один список `List<PersonModel>`
- Проверка "сегодня ли день рождения" выполняется на клиенте (Blazor WASM), используя `PersonModel.DaysUntilBirthday == 0` или аналогичную логику
- Убрать логику разделения из `PersonRepository`

**Затронутые файлы:**
- `SharedKernel.Contracts/Models/Responses/GetPersonsResponse.cs` — убрать два списка, оставить один
- `Infrastructure/Repositories/PersonRepository.cs` — убрать разделение на TodayBirthdays/UpcomingBirthdays
- `WebAssembly/Pages/Home/Home.razor` — добавить клиентскую фильтрацию