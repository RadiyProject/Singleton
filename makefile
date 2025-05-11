selenium-up:
	docker run -d -p 4444:4444 -p 7900:7900 --shm-size="2g" --network radx --name selenium-hub selenium/standalone-chromium:latest

selenium-down:
	docker stop selenium-hub && docker rm selenium-hub

up:
	docker compose --env-file ./.env  up -d

down:
	docker compose down

restart:
	make down
	make up
