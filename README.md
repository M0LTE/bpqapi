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
  "GB7BDH": {
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
        "PAUSE 2",
        "INTERLOCK 3",
        "NC 3 !M1BFP-1",
        "C 2 GB7BDH-1"
      ],
      "hierarchicalRoutes": [],
      "hr": [
        "#49.GBR.EURO",
        "#53.GBR.EURO"
      ],
      "bbsHa": "",
      "enableForwarding": false,
      "forwardingInterval": "00:58:20",
      "requestReverse": false,
      "reverseInterval": "01:00:00",
      "sendNewMessagesWithoutWaiting": true,
      "fbbBlocked": true,
      "maxBlock": 1000,
      "sendPersonalMailOnly": false,
      "allowBinary": true,
      "useB1Protocol": true,
      "useB2Protocol": false,
      "sendCtrlZInsteadOfEx": false,
      "incomingConnectTimeout": "00:02:00"
    }
  },
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
  "GB7BPQ": {
    "queueLength": 0,
    "forwardingConfig": {
      "to": [],
      "at": [
        "GBR"
      ],
      "times": [],
      "connectScript": [
        "NC 3 !GB7BPQ"
      ],
      "hierarchicalRoutes": [],
      "hr": [
        "#76.GBR.EURO"
      ],
      "bbsHa": "GB7BPQ.#76.GBR.EURO",
      "enableForwarding": true,
      "forwardingInterval": "04:10:00",
      "requestReverse": false,
      "reverseInterval": "00:00:00",
      "sendNewMessagesWithoutWaiting": true,
      "fbbBlocked": true,
      "maxBlock": 10000,
      "sendPersonalMailOnly": false,
      "allowBinary": true,
      "useB1Protocol": false,
      "useB2Protocol": true,
      "sendCtrlZInsteadOfEx": false,
      "incomingConnectTimeout": "00:02:00"
    }
  },
  "GB7BRK": {
    "queueLength": 0,
    "forwardingConfig": {
      "to": [],
      "at": [
        "OARC",
        "GBR"
      ],
      "times": [],
      "connectScript": [
        "c gb7wod",
        "c gb7brk",
        "bbs"
      ],
      "hierarchicalRoutes": [],
      "hr": [
        "GB7BRK.#42.GBR.EURO"
      ],
      "bbsHa": "GB7BRK.#42.GBR.EURO",
      "enableForwarding": true,
      "forwardingInterval": "01:00:00",
      "requestReverse": false,
      "reverseInterval": "00:00:00",
      "sendNewMessagesWithoutWaiting": true,
      "fbbBlocked": true,
      "maxBlock": 10000,
      "sendPersonalMailOnly": false,
      "allowBinary": true,
      "useB1Protocol": false,
      "useB2Protocol": true,
      "sendCtrlZInsteadOfEx": false,
      "incomingConnectTimeout": "00:02:00"
    }
  },
  "GB7BSK": {
    "queueLength": 0,
    "forwardingConfig": {
      "to": [],
      "at": [
        "OARC",
        "GBR",
        "WW",
        "AMSAT"
      ],
      "times": [],
      "connectScript": [
        "NC 1 !GB7BSK-1"
      ],
      "hierarchicalRoutes": [],
      "hr": [
        "#48.GBR.EURO"
      ],
      "bbsHa": "GB7BSK.#48.GBR.EURO",
      "enableForwarding": true,
      "forwardingInterval": "00:55:00",
      "requestReverse": false,
      "reverseInterval": "01:00:00",
      "sendNewMessagesWithoutWaiting": true,
      "fbbBlocked": true,
      "maxBlock": 10000,
      "sendPersonalMailOnly": false,
      "allowBinary": true,
      "useB1Protocol": false,
      "useB2Protocol": true,
      "sendCtrlZInsteadOfEx": false,
      "incomingConnectTimeout": "00:02:00"
    }
  },
  "GB7CIP": {
    "queueLength": 0,
    "forwardingConfig": {
      "to": [],
      "at": [
        "GBR",
        "WW",
        "NOAM"
      ],
      "times": [],
      "connectScript": [
        "c gb7lan",
        "nc 1 !gb7wem-1",
        "c uhf gb7cip",
        "ELSE",
        "PAUSE 4",
        "INTERLOCK 3",
        "NC 3 !GB7WEM-7",
        "C CIPBBS"
      ],
      "hierarchicalRoutes": [],
      "hr": [
        "#32.GBR.EURO",
        "GBR.EURO",
        "GB7CNR.#17.GBR.EURO",
        "GB7ODZ.#18.GBR.EURO"
      ],
      "bbsHa": "GB7CIP.#32.GBR.EURO",
      "enableForwarding": true,
      "forwardingInterval": "01:00:00",
      "requestReverse": true,
      "reverseInterval": "01:00:00",
      "sendNewMessagesWithoutWaiting": true,
      "fbbBlocked": true,
      "maxBlock": 10000,
      "sendPersonalMailOnly": false,
      "allowBinary": true,
      "useB1Protocol": false,
      "useB2Protocol": true,
      "sendCtrlZInsteadOfEx": false,
      "incomingConnectTimeout": "00:02:00"
    }
  },
  "GB7HIB": {
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
        "interlock 3",
        "c 3 gb7hib-1"
      ],
      "hierarchicalRoutes": [],
      "hr": [
        "#75.GBR.EURO"
      ],
      "bbsHa": "GB7HIB.#75.GBR.EURO",
      "enableForwarding": true,
      "forwardingInterval": "01:00:00",
      "requestReverse": true,
      "reverseInterval": "01:00:00",
      "sendNewMessagesWithoutWaiting": true,
      "fbbBlocked": true,
      "maxBlock": 10000,
      "sendPersonalMailOnly": false,
      "allowBinary": true,
      "useB1Protocol": false,
      "useB2Protocol": true,
      "sendCtrlZInsteadOfEx": false,
      "incomingConnectTimeout": "00:02:00"
    }
  },
  "GB7IOW": {
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
        "NC 2 !G7BCS-7",
        "NC 1 !GB7IOW-1"
      ],
      "hierarchicalRoutes": [],
      "hr": [
        "IRL.EURO",
        "#44.GBR.EURO",
        "#45.GBR.EURO",
        "#49.GBR.EURO",
        "#51.GBR.EURO",
        "#52.GBR.EURO",
        "#53.GBR.EURO",
        "#54.GBR.EURO",
        "#55.GBR.EURO",
        "#56.GBR.EURO",
        "#71.GBR.EURO",
        "#72.GBR.EURO",
        "#73.GBR.EURO",
        "#74.GBR.EURO",
        "#75.GBR.EURO",
        "#76.GBR.EURO",
        "#77.GBR.EURO",
        "#78.GBR.EURO",
        "#79.GBR.EURO"
      ],
      "bbsHa": "GB7IOW.#48.GBR.EURO",
      "enableForwarding": true,
      "forwardingInterval": "00:10:00",
      "requestReverse": true,
      "reverseInterval": "01:00:00",
      "sendNewMessagesWithoutWaiting": true,
      "fbbBlocked": true,
      "maxBlock": 10000,
      "sendPersonalMailOnly": false,
      "allowBinary": true,
      "useB1Protocol": false,
      "useB2Protocol": true,
      "sendCtrlZInsteadOfEx": false,
      "incomingConnectTimeout": "00:00:30"
    }
  },
  "GB7LAN": {
    "queueLength": 0,
    "forwardingConfig": {
      "to": [],
      "at": [
        "OARC",
        "GBR",
        "WW",
        "AMSAT"
      ],
      "times": [],
      "connectScript": [
        "C GB7LAN",
        "BBS"
      ],
      "hierarchicalRoutes": [],
      "hr": [
        "#42.GBR.EURO"
      ],
      "bbsHa": "GB7LAN.#42.GBR.EURO",
      "enableForwarding": true,
      "forwardingInterval": "01:00:00",
      "requestReverse": false,
      "reverseInterval": "00:00:00",
      "sendNewMessagesWithoutWaiting": true,
      "fbbBlocked": true,
      "maxBlock": 10000,
      "sendPersonalMailOnly": false,
      "allowBinary": true,
      "useB1Protocol": false,
      "useB2Protocol": true,
      "sendCtrlZInsteadOfEx": false,
      "incomingConnectTimeout": "00:02:00"
    }
  },
  "GB7LOX": {
    "queueLength": 3,
    "forwardingConfig": {
      "to": [],
      "at": [
        "OARC",
        "GBR",
        "WW"
      ],
      "times": [],
      "connectScript": [
        "interlock 3",
        "NC 3 !GB7LOX-1"
      ],
      "hierarchicalRoutes": [],
      "hr": [
        "#19.GBR.EURO"
      ],
      "bbsHa": "GB7LOX.#19.GBR.EURO",
      "enableForwarding": true,
      "forwardingInterval": "02:46:40",
      "requestReverse": false,
      "reverseInterval": "00:00:00",
      "sendNewMessagesWithoutWaiting": true,
      "fbbBlocked": true,
      "maxBlock": 10000,
      "sendPersonalMailOnly": false,
      "allowBinary": true,
      "useB1Protocol": false,
      "useB2Protocol": true,
      "sendCtrlZInsteadOfEx": false,
      "incomingConnectTimeout": "00:02:00"
    }
  },
  "GB7OXF": {
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
        "PAUSE 5",
        "INTERLOCK 3",
        "NC 3 !GB7OXF-2"
      ],
      "hierarchicalRoutes": [],
      "hr": [
        "#49.GBR.EURO"
      ],
      "bbsHa": "GB7OXF.#49.GBR.EURO",
      "enableForwarding": true,
      "forwardingInterval": "00:05:00",
      "requestReverse": false,
      "reverseInterval": "00:51:40",
      "sendNewMessagesWithoutWaiting": true,
      "fbbBlocked": true,
      "maxBlock": 10000,
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
      "at": [],
      "times": [],
      "connectScript": [],
      "hierarchicalRoutes": [],
      "hr": [],
      "bbsHa": "GB7RDG.#42.GBR.EURO",
      "enableForwarding": false,
      "forwardingInterval": "01:00:00",
      "requestReverse": false,
      "reverseInterval": "00:00:00",
      "sendNewMessagesWithoutWaiting": false,
      "fbbBlocked": false,
      "maxBlock": 10000,
      "sendPersonalMailOnly": false,
      "allowBinary": false,
      "useB1Protocol": false,
      "useB2Protocol": false,
      "sendCtrlZInsteadOfEx": false,
      "incomingConnectTimeout": "00:02:00"
    }
  },
  "M5MPC": {
    "queueLength": 2,
    "forwardingConfig": {
      "to": [],
      "at": [
        "OARC",
        "GBR",
        "WW"
      ],
      "times": [],
      "connectScript": [
        "PAUSE 6",
        "INTERLOCK 3",
        "NC 3 !M5MPC-1"
      ],
      "hierarchicalRoutes": [],
      "hr": [
        "#16.GBR.EURO"
      ],
      "bbsHa": "GB7ELE.#16.GBR.EURO",
      "enableForwarding": false,
      "forwardingInterval": "01:00:00",
      "requestReverse": true,
      "reverseInterval": "01:00:00",
      "sendNewMessagesWithoutWaiting": false,
      "fbbBlocked": true,
      "maxBlock": 1000,
      "sendPersonalMailOnly": true,
      "allowBinary": true,
      "useB1Protocol": false,
      "useB2Protocol": true,
      "sendCtrlZInsteadOfEx": false,
      "incomingConnectTimeout": "00:02:00"
    }
  }
}
```
