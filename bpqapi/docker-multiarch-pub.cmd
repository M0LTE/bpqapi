docker login
docker buildx build --pull --platform linux/arm/v7,linux/amd64,linux/arm64/v8 --push -t m0lte/bpqapi:latest .