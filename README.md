# bpqapi

The very start of an OpenAPI-specified API for BPQ.

This will:

- wrap the various BPQ TCP interfaces where needed
- wrap the BPQ API where available
- wrap the BPQ UI where needed

So far just a couple of mail management pages have been made available.

There is a swagger at `/swagger/index.html`

`500 Internal Server Error` responses are par for the course at this stage, as this is a work in progress, and underlying changes may happen in BPQ. Please contact me to tell me how it broke, supplying the .tmp file it may have given you when it broke.

## Running under Docker

Current targets: linux/arm/v7, linux/amd64

```
docker pull m0lte/bpqapi
docker run -e bpq__uri=http://your-node:8008 -p 8080:8080 m0lte/bpqapi
```

change the port it is exposed at by changing the second part of the `-p` argument.

or, `docker-compose.yml`:

```
name: bpqapi
services:
    bpqapi:
        image: m0lte/bpqapi
        restart: unless-stopped
        environment:
            - bpq__uri=http://your-node:8008
        ports:
            - 8080:8080
```

and to start: 

```
docker compose up -d
```

to update:

```
docker pull m0lte/bpqapi
docker run ...
```

or


```
docker compose pull
docker compose stop
docker compose up -d
```

Any which way, API available at http://your-node:8080/swagger


## InfluxDB Metrics

If you use the TICK stack, you might like to know that you can configure Telegraf's http plugin to read http://your-node:8080/metrics/line-protocol with Basic authentication (sysop username/password) to pull some metrics into Grafana using Telegraf.

Metrics supported:

- packetmail
  - queue_length
