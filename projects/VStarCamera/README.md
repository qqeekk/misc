# VStarCamera

The project allows to find and control IR camera [VStarCam C7824](https://vstarcam.ru/product/%d0%ba%d0%b0%d0%bc%d0%b5%d1%80%d0%b0-c7824/).

## Build

The .NET 6 SDK is required.

```bash
dotnet publish ./VStarCameraZone.csproj -r linux-x64 -c Release --self-contained -p:PublishSingleFile=true -p:PublishTrimmed=true -o .
```

## Usage

```bash
ivan@topka /m/d/w/m/p/VStarCamera (master)> ./VStarCameraZone --help
VStarCameraZone

Usage:
  VStarCameraZone [options] [command]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  find         Show the list of cameras in the network
  set-ir <ip>  Set IR state
  show <ip>    Show the info about camera
```

## TODO

1. Implement web service.

## Systemd Example Setup

### `camera-ir-off.timer`

```
[Unit]
Description=Set camera IR off

[Timer]
OnCalendar=*-*-* 02:00:00
OnCalendar=*-*-* 03:00:00
OnCalendar=*-*-* 04:00:00
Persistent=true

[Install]
WantedBy=timers.target
```

### `camera-ir-off.service`

```
[Unit]
Description=Set camera IR off

[Service]
Type=oneshot
ExecStart=/home/ivan/apps/vstar/VStarCameraZone set-ir 192.168.100.24 false
WorkingDirectory=/home/ivan/apps/vstar
User=ivan
```

### `camera-ir-on.timer`

```
[Unit]
Description=Set camera IR on

[Timer]
OnCalendar=*-*-* 20:00:00
OnCalendar=*-*-* 20:05:00
OnCalendar=*-*-* 20:10:00
Persistent=true

[Install]
WantedBy=timers.target
```

### `camera-ir-on.service`

```
[Unit]
Description=Set camera IR on

[Service]
Type=oneshot
ExecStart=/home/ivan/apps/vstar/VStarCameraZone set-ir 192.168.100.24 true
WorkingDirectory=/home/ivan/apps/vstar
User=ivan
```
