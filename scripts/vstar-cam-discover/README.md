The python script that allows to get information about VStarCam cameras within network. Example output:

```
➜  vstar-cam-discover git:(master) ✗ $ python vstarcam.py
Broadcast package is sent, waiting...
IP: 192.168.0.36
IP mask: 255.255.255.0
IP gateway: 192.168.0.1
UID: RVSS005850PBPGW
Port: 49623
Name: WATCHER
```

I only tested it with my own camera, not sure that it will work for all others.
