# Telegram Бот-планировщик задач.

Добро пожаловать в проект **Имя Вашего Проекта**! Этот проект предназначен для [краткое описание цели или функций проекта].

## Установка

1. Склонировать проект на запускаемую машину.
2. Установить docker на запускаемую машину, или docker-desktop.
3. Скачать environment_variables архив, и поместить .env файл в папку, где находятся \*-compose.yml файлы.
4. Сконфигурировать .env под свои необходимые инфраструктурные зависимости

4.1 Получить токен бота через bot-father и вставить в .env

https://telegram.me/BotFather

4.2 Получито токен time zone db провайдер и вставить в .env

https://timezonedb.com/

5. Запустить приложение с использованием следующей команды:

docker compose -f docker-compose.yml -d --build

## Использование:

Для использования бота в чатах, его необходимо добавить в сами чаты.
Бот ищется по имени, которое ему было задано при создании: https://telegram.me/BotFather

### Бот поддерживает следующие команды:

#### Команды для подписанных чатов:

/change_time_zone - изменить временную зону подписанного чата.
/add_this_chat - подписать чат (или тему чата) к телеграм боту
/bot_chat_time - узнать временную зону и время в подписанном чате
/ping - пингануть бота (проверить, что он запущен)
/tc <текст уведомления> - создать уведомление в подписанном чате (или теме чата)
/remove_this_chat - отписать подписанный чат(или тему) от бота
/start - вызывает отображение справочной информации
/external_chat_info - справка по взаимодействию с ботом из подписанных чатов
/bot_chat_info - справка по взаимодействию с ботом внутри чата бота.

### Команды для общения внутри бота.

/my_tasks - просмотреть список своих созданных задач, задач из тем.

###### Используемые технологии:

C#. .NET 9. Background Worker. (язык и фреймворк)
Docker (инфраструктура).
Postgre SQL (БД).
Seq (логирование) https://serilog.net/
Serilog Seq (логирование) https://docs.datalust.co/docs/using-serilog
Entity Framework Core (ORM библиотека для работы с БД) https://learn.microsoft.com/ru-ru/ef/core/
Dapper (библиотека для работы с SQL) https://www.learndapper.com/
CSharpFunctionalExtensions (библиотека для работы с Result) https://github.com/vkhorikov/CSharpFunctionalExtensions
SharpVector (библиотека для работы с распознаванием времени в тексте) https://github.com/Build5Nines/SharpVector
PR Telegram Bot (библиотека для работы с ботом) https://prethink.gitbook.io/prtelegrambot
