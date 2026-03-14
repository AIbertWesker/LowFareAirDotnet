# LowFareAirDotnet

Projekt na zaliczenie na studia — przepisanie aplikacji z Java do .NET (C#) (Nie lubie dzawy).

Status: **WIP** (work in progress).

## Opis
Celem projektu jest odtworzenie/port istniejącej logiki z wersji javowej do środowiska .NET, z użyciem:
- ASP.NET Core (web)
- Entity Framework Core jako ORM + PostgreSQL (Npgsql)

Na ten moment projekt jest w trakcie rozwoju — część funkcjonalności może być niekompletna lub w trakcie refaktoru.

## Status prac
- [x] Podstawowe modele i konfiguracja EF Core
- [x] Integracja z PostgreSQL
- [ ] Warstwa logiki / use-case’y
- [ ] Autoryzacja (np. JWT)
- [ ] UI / strony

## Uruchomienie (dev)
1. Upewnij się, że masz uruchomionego PostgreSQL.
2. W projekcie Tool.DbUper ustaw connection string do bazy danych (w kodzie bo mi sie nie chcialo konfigurować).
3. Odpal DbUpa, który utworzy tabele i wstawi dane testowe. 
4. Ustaw connection string w `LowFareAirDotnet.Web/appsettings.json` w sekcji `ConnectionStrings:Default`.
5. Uruchom projekt webowy z Visual Studio albo poleceniem `dotnet run`.
6. Powinno działać. 

## Uwagi
- Projekt tworzony głównie pod wymagania zaliczeniowe.

## Licencja
W sumie nie wiem.