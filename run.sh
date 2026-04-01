#!/bin/bash

set -e

echo "🚀 Deploy started..."

echo "📥 Pulling latest code..."
git pull origin main

echo "🛑 Stopping containers..."
docker compose -f docker-compose.production.yml down 

echo "🧹 Cleaning old images..."
docker image prune

echo "🔨 Building image (no cache)..."
docker compose -f docker-compose.production.yml build --no-cache

echo "▶️ Starting containers..."
docker compose up -f docker-compose.production.yml -d

echo "⏳ Waiting for services..."
sleep 5

echo "📦 Container status:"
docker compose -f docker-compose.production.yml ps

echo "📜 Last logs:"
docker compose -f docker-compose.production.yml logs --tail=50

echo "✅ Deploy completed!"
