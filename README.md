# My-Sample-Project
Sample project to add card details and to test Docker
# 💳 Payment Sample Project



A Dockerized .NET 8 Web API for payment processing, using **MySQL** as the database and **Redis** for caching. Managed via **Docker Compose**.

---

## Tech Stack

- .NET 8 Web API
- MySQL 8.0
- Redis (latest)
- Docker & Docker Compose

---

## Quick Start

### Clone the Repo

git clone https://github.com/riteshkeshri98/My-Sample-Project.git
cd My-Sample-Project

SampleProject/
├── Controllers/ # API Controllers
├── Data/ # DB Context & Repositories
├── Migrations/ # EF Core Migrations
├── Models/ # Entity Models
├── wwwroot/ # Static content
├── Program.cs # App entry point
├── Dockerfile # API container definition
├── README.md
└── docker-compose.yml # Multi-container setup

Build and Run with Docker

docker-compose build
docker-compose up

Your API will be live at: http://localhost:5000

Services in docker-compose.yml

| Service  | Description        | Port |
| -------- | ------------------ | ---- |
| `webapi` | .NET 8 Web API     | 5000 |
| `mysql`  | MySQL 8.0 Database | 3306 |
| `redis`  | Redis Cache        | 6379 |

Environment Variables (in docker-compose.yml)

ConnectionStrings__DefaultConnection
Redis__Host, Redis__Port
ASPNETCORE_ENVIRONMENT

Docker Shortcuts:
docker-compose down             # Stop all services
docker ps                       # List running containers
docker logs <container>         # View container logs
docker exec -it <container> sh  # Shell into a container


Push to Docker Hub
1. Tag your Web API image:
docker tag my-sample-project_webapi riteshkeshri98/payment-api:latest

2. Push it:
docker push riteshkeshri98/payment-api:latest
