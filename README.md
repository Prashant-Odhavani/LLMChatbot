# 🤖 AI Chatbot with .NET 8, MongoDB Vector Search & Open Source LLMs

A production-ready AI chatbot project that uses **.NET 8**, **MongoDB Atlas Vector Search**, and **open-source LLMs** like **llama3** to generate human-like responses from uploaded content using semantic search and local LLM inference.

---

## 📖 About the Project

This project demonstrates how to build an intelligent chatbot system using:

- MongoDB’s **Vector Search** for fast, semantically relevant document retrieval,
- A **.NET 8** backend for clean and scalable API development,
- And an **open-source LLM** (e.g., llama3) served via Ollama for natural language generation.

It supports full document ingestion, embedding generation, context-based response creation, and modular services ready for extension or production.

---

## 🧰 Tech Stack

| Technology         | Description                                  |
|-------------------|----------------------------------------------|
| .NET 8            | Backend API and logic (C#)                   |
| MongoDB Atlas     | Document storage & vector search             |
| MongoDB Vector DB | Embedding-based search and retrieval         |
| Ollama            | Local LLM runner for open-source models      |
| llama3   | Open-source language models                  |
| Docker            | Containerization and portability             |

---

## 🧱 Architecture Overview

```text
User Query
    ↓
Embed with Model
    ↓
MongoDB Atlas Vector Search
    ↓
Fetch Top K Matched Chunks
    ↓
Build Context + Original Query
    ↓
Send to LLM (Ollama endpoint)
    ↓
Generate & Return Final Response
```
