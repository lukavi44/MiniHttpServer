# MiniHttpServer

A custom HTTP server built from scratch in C# using raw TCP sockets — without ASP.NET.

## Purpose

This project was built to deeply understand how web frameworks like ASP.NET work internally.

Instead of relying on abstractions, this server manually implements:
- TCP communication
- HTTP request parsing
- Routing
- Middleware pipeline
- JSON deserialization
- Async request handling
- Static file serving

---

## What This Demonstrates

This project shows understanding of:

- How HTTP works over TCP
- How servers process raw network data
- How middleware pipelines are implemented
- The difference between synchronous and asynchronous execution
- How routing and request handling work internally
- How frameworks like ASP.NET abstract these concepts

---

## Architecture

Client → TCP → Raw HTTP → Parser → Middleware → Router → Response → Client

---

## Project Structure

Http/
  Core/
  Server/
  Routing/
  Middleware/

Models/
wwwroot/

---

## Features

- Low-level TCP server
- Manual HTTP parsing
- Routing (static + dynamic)
- Middleware pipeline
- JSON body parsing
- Async/concurrent handling
- Static file serving

---

## Run

dotnet run

Open:
http://localhost:5000

---

## Future Improvements

- Query params
- DI container
- HTTPS
- Better HTTP compliance
