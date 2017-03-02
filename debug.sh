docker-compose -f docker-compose.dev.yml build
docker-compose -f docker-compose.dev.yml run -p 5000:5000 web bash