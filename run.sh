#!/bin/bash

set -e

echo "🚀 Deploy started..."

echo "📥 Pulling latest code..."
git pull origin main

echo "🛑 Stopping containers..."
docker compose down

echo "🧹 Cleaning old images..."
docker image prune -f

echo "🔨 Building image (no cache)..."
docker compose build --no-cache

echo "▶️ Starting containers..."
docker compose up -d

echo "⏳ Waiting for services..."
sleep 5

echo "📦 Container status:"
docker compose ps

echo "📜 Last logs:"
docker compose logs --tail=50

echo "✅ Deploy completed!"
