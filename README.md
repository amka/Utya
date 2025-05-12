# 🪿 Гуся — сервис сокращения ссылок для РФ

[![.NET](https://img.shields.io/badge/.NET-512BD4?logo=dotnet&logoColor=fff)](https://dot.net/)
[![PostgreSQL](https://img.shields.io/badge/Postgres-%23316192.svg?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**Быстрое сокращение ссылок с аналитикой и адаптацией под российский рынок**

![Логотип сервиса](https://via.placeholder.com/800x200.png?text=Utya+-+Сокращай+ссылку+за+1+клик)

## 🌟 Возможности

- **Мгновенный редирект** с кешированием
- Генерация коротких ссылок **без регистрации**
- Подробная **аналитика переходов** (гео, устройство, источник)
- **API** для разработчиков
- Поддержка **кастомных доменов** (для платных тарифов)
- **Локализация** для РФ: СБП/YandexPay, сервера в России
- Автоматическая **блокировка запрещенных доменов**

## 🛠 Технологический стек

| Компонент       | Технологии                                                                 |
|-----------------|----------------------------------------------------------------------------|
| Бэкенд         | **.Net** (ASP.NET Core) + **PostgreSQL**                 |
| Фронтенд       | HTML5, CSS3, Vanilla JS                                                   |
| Инфраструктура | Docker, Nginx, Redis (кеширование)                                        |
| Аналитика      | Встроенная система + интеграция с Яндекс.Метрикой                        |
| Безопасность   | HTTPS, проверка доменов                             |

## 🚀 Быстрый старт

### Требования
- DotNet 9.0
- PostgreSQL 15
- Docker (опционально)

### Установка

#### 1. Клонировать репозиторий

    git clone https://github.com/tenderowl/utya.git
    cd utya

#### 2. Настроить окружение

    cp appsettings.Development.json appsettings.Production.json
    nano appsettings.Production.json  # Заполнить параметры

#### 3. Установить зависимости

    dotnet restore

#### 4. Запустить миграции

    dotnet ef --project Utya database migrate

#### 5. Запустить сервер

    dotnet run --project Utya