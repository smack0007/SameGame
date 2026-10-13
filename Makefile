default: build

.PHONY: build
build:
	cd ./src/SameGame && dotnet build

.PHONY: run
run:
	cd ./src/SameGame && dotnet run