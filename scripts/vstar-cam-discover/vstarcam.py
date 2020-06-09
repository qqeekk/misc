#!/usr/bin/env python
# command to watch traffic: sudo tcpdump -s0 -t udp -n 'port 8601' -X

import socket

OUTBOUND_PORT = 8600
INBOUND_PORT = 8601
DISCOVER_MESSAGE = bytes(b'\x44\x48\x01\x01')


def index_subarray(arr: list, subarr: list, start: int=0) -> int:
    """Finds subarr in a arr and return index position. Otherwise returns -1."""
    ind = arr.index(subarr[0], start)
    subarrlen = len(subarr)
    arrlen = len(arr)
    while ind > -1 and arrlen >= ind+subarrlen and arrlen >= start+subarrlen:
        if arr[ind:ind+subarrlen] == subarr:
            return ind
        start = ind + 1
        ind = arr.index(subarr[0], start)
    return -1


if __name__ == '__main__':
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.bind(('', INBOUND_PORT))
    sock.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
    sock.sendto(DISCOVER_MESSAGE, ('255.255.255.255', OUTBOUND_PORT))
    print('Broadcast package is sent, waiting...')

    try:
        while True:
            data, address = sock.recvfrom(1024)
            print('IP: {0}'.format(data[4:19].decode('ascii')))
            print('IP mask: {0}'.format(data[20:35].decode('ascii')))
            print('IP gateway: {0}'.format(data[36:51].decode('ascii')))
            print('UID: {0}'.format(data[92:107].decode('ascii')))
            print('Port: {0}'.format(int.from_bytes(data[90:92], 'little')))
            print('Name: {0}\n'.format(data[124:144].decode('ascii')))
    except KeyboardInterrupt:
        sock.close()
