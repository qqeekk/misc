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
    parser = argparse.ArgumentParser(description='Download books from audiokniga.club .')
    parser.add_argument('url', help='Url that contains books player.')
    args = parser.parse_args()

    # get book id
    response = urllib.request.urlopen(args.url)
    book_files = re.findall(r'data-url=\"(.*?)\">', str(response.read()), re.MULTILINE)

    # create directory for it
    dir = re.search(r'\/((?:.(?!\/))+)$', args.url).group(1)
    if not os.path.exists(dir):
        os.makedirs(dir)

    # download mp3 files
    index = 0
    for book_file in book_files:
        filename = re.search(r'\/((?:.(?!\/))+)$', book_file).group(1)
        print('Downloading %s' % (filename))
        parsedurl = urllib.parse.urlsplit(book_file)
        parsedurl = parsedurl._replace(path=urllib.parse.quote(parsedurl.path))
        success = False
        for i in range(1, ATTEMPTS_COUNT+1):
            try:
                urllib.request.urlretrieve(parsedurl.geturl(), os.path.join(dir, filename))
                success = True
            except urllib.error.HTTPError:
                print('Failed download %s for %d' % (filename, i))
                continue
            break
        if not success:
            raise Exception('Cannot download file after 3 attempts: %s' % (filename))
