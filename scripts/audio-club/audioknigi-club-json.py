#!/usr/bin/env python
import argparse
import re
import json
import os
import urllib
import urllib.request
from os.path import splitext

ATTEMPTS_COUNT = 3


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Download books from audioknigi.club from JSON .')
    parser.add_argument('file', help='JSON file to parse.')
    parser.add_argument('--use-numbers', help='Use only number for output files, like 001.mp3 .',
        default=False, required=False, type=bool)
    args = parser.parse_args()

    # parse JSON
    with open(args.file) as f:
        items = json.loads(json.load(f)['aItems'])

    # create directory for it
    dir = os.path.splitext(os.path.basename(args.file))[0]
    if not os.path.exists(dir):
        os.makedirs(dir)

    # download mp3 files
    for item in items:
        filename = item['title'] + '.mp3'
        if args.use_numbers:
            filename = str(index).zfill(3) + splitext(filename)[1]
            index = index + 1
        print('Downloading %s' % (filename))
        parsedurl = urllib.parse.urlsplit(item['mp3'])
        parsedurl = parsedurl._replace(path=urllib.parse.quote(parsedurl.path))
        success = False
        for i in range(1, ATTEMPTS_COUNT+1):
            try:
                urllib.request.urlretrieve(parsedurl.geturl(), os.path.join(dir, filename.lower()))
                success = True
            except (urllib.error.HTTPError, urllib.error.ContentTooShortError):
                print('Failed download %s for %d' % (filename, i))
                continue
            break
        if not success:
            raise Exception('Cannot download file after 3 attempts: %s' % (filename))
