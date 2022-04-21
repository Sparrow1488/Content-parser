# Sparrow.Parsing.Utils

Репозиторий содержит в себе все необходимые (мне) структуры и классы для возможности парсинга HTML-верстки

## Introduction

Весь парсинг построен на принципе конвейерной обработки странички, где каждый обработчик в конвейере независимо обрабатывает исходную HTML-страницу и инициализирует необходимые поля итогового типа. Достоинством, что я хочу отметить, данного подхода является то, что благодаря изолированной модели обработки исходной страницы можно просто переключаться и тестировать отдельные модули данного конвейера. Также, данный способ призван оптимизировать процесс отладки программного кода с целью выявления ошибок.

## Roadmap

* **IParsingSource** - возможность авторизации, если необходимо и возможность работы с Cookies.
  * (Feature) Механизм авторизации и возможности добавления Cookies, если оно необходимо

* **ParsingPipeline** - в этом конвейере собираются все промежуточные обработчики (ParsingMiddleware), которые обрабатывают определённый участок данных, за который они ответственны.
* **ParsingMiddleware** - промежуточный обработчик данных из источника. Принцип работы основан на принципе устройства Middlewares в ASP.NET Core. Отвечает за обработку определенной информации из данных с источника. Для передачи определенных данных в другие ParsingMiddleware использовать можно механизм Dependency Injection.
* **MiddlewareContext** - контекст Middleware-обработчика. В нем хранятся: 
  * Экземпляр источника (IParsingSource), с которого происходит непосредственное получение данных;
  * Экземпляр ParsingMiddleware, являющийся следующим в очереди обработки данных;
  * Экземпляр IServiceCollection, необходимый для передачи в другие Middlewares зарегистрированных зависимостей;
  * Экземпляр IServiceProvider, служащий для доступа к зарегистрированным сервисам.

## .NET Core Dependencies

* Microsoft.Extensions.Hosting → v6.0.1

## Usage

```C#
var source = new MicrosoftSource("https://www.microsoft.com/ru-ru/");
var pipe = new ParsingPipeline<MicrosoftEntity, MicrosoftSource>(source)
            .Use<InitializerMiddleware>()
            .Use<NewsParsingMiddleware>()
            .Use<ProductsParsingMiddleware>();
var resultEntity = await pipe.StartAsync();
Console.WriteLine(resultEntity);
```

Во-первых, создается источник данных, откуда будет происходить получение, извиняюсь, данных. Источник должен реализовывать интерфейс `IParsingSource` или костомный `ITextParsing`.

Во-вторых, регистрируется обрабатываемого типа (в примере: `MicrosoftEntity`). Данный тип будет создан в объекте класса `ParsingPipeline` (для этого он должен иметь хотя бы один пустой конструктор). 

В-третьих создается конвейер `ParsingPipeline`, можно не утруждаться и не писать свой - этого хватит с лихвой. Через метод `Use<T>()` указываются все пользовательские Middlewares, наследуемые от класса `ParsingMiddleware`. Экземпляры указанных типов будут инициализированы внутри `ParsingPipeline`. 

В-четвертых, для обработки данных потребуются, непосредственно, сами обработчики, о которых упоминалось ранее. Обработчики (`ParsingMiddleware`'ы) должны реализовать абстрактный метод `ProcessAsync()`, одним параметром которого является обрабатываемый тип (в примере: `MicrosoftEntity`). В каждом обработчике можно также переопределить метод `Process(TResult)`. По умолчанию он реализуется следующим образом: 

```C#
public virtual void Process(TResult toProcess) => 
		ProcessAsync(toProcess).ConfigureAwait(false);
public abstract Task ProcessAsync(TResult toProcess);
```

## References

* [Проект, используемый в примере](https://github.com/Sparrow1488/Sparrow.Parsing.Utils/tree/master/Examples/Sparrow.Parsing.Example)

