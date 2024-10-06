# bpqapi

The very start of an OpenAPI-specified API for BPQ.

This will:

- wrap the various BPQ TCP interfaces where needed
- wrap the BPQ API where available
- wrap the BPQ UI where needed

So far just a couple of mail management pages have been made available.

There is a swagger at `/swagger/index.html`

Sample request:

```
curl http://localhost:5177/MailManagement/partners
```

Sample response:

```
{
  "GB7BEX": {
    "queueLength": 0,
    "forwardingConfig": {
      "to": [],
      "at": [
        "OARC",
        "GBR",
        "WW"
      ],
      "times": [],
      "connectScript": [
        "PAUSE 3",
        "INTERLOCK 3",
        "NC 3 !GB7BEX"
      ],
      "hierarchicalRoutes": [],
      "hr": [
        "#38.GBR.EURO"
      ],
      "bbsHa": "GB7BEX.#38.GBR.EURO",
      "enableForwarding": true,
      "forwardingInterval": "00:56:40",
      "requestReverse": false,
      "reverseInterval": "00:56:40",
      "sendNewMessagesWithoutWaiting": true,
      "fbbBlocked": true,
      "maxBlock": 1000,
      "sendPersonalMailOnly": false,
      "allowBinary": true,
      "useB1Protocol": false,
      "useB2Protocol": true,
      "sendCtrlZInsteadOfEx": false,
      "incomingConnectTimeout": "00:02:00"
    }
  },
  "GB7RDG": {
    "queueLength": 0,
    "forwardingConfig": {
      "to": [],
...
      "incomingConnectTimeout": "00:02:00"
    }
  }
}
```

## Running under Docker

Current target: linux-arm

```
docker pull m0lte/bpqapi
docker run -e bpq__uri=http://your-node:8008 -e bpq__sysopUsername=youruser -e bpq__sysopPassword=yourpass -p 8080:8080 m0lte/bpqapi
```

or, `docker-compose.yml`:

```
name: bpqapi
services:
    bpqapi:
        image: m0lte/bpqapi
        restart: unless-stopped
        environment:
            - bpq__uri=http://your-node:8008
            - bpq__sysopUsername=youruser
            - bpq__sysopPassword=yourpass
        ports:
            - 8080:8080
```

and to start: 

```
docker compose up -d
```

to update:
```
docker compose pull
docker compose stop
docker compose up -d
```

Any which way, API available at http://your-node:8080/swagger
