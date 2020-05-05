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
    parser = argparse.ArgumentParser(description='Download books from audioknigi.club .')
    parser.add_argument('url', help='Url that contains books player.')
    parser.add_argument('--use-numbers', help='Use only number for output files, like 001.mp3 .',
        default=False, required=False, type=bool)
    args = parser.parse_args()

    # get book id
    response = str(urllib.request.urlopen(args.url).read())
    book_id_match = re.search(r'book-circle-progress-(\d+)', response, re.MULTILINE)
    if not book_id_match:
        raise Exception('Cannot find book id.')
    book_id = book_id_match.group(1)

    # get security variables
    #ls_security_key_match = re.search(r'LIVESTREET_SECURITY_KEY\s*=\s*''([a-f0-9]+)'';', response, re.MULTILINE)
    ls_security_key_match = re.search(r'''([a-f0-9]{32})''', response, re.MULTILINE)
    if not ls_security_key_match:
        raise Exception('Cannot find security key.')
    ls_security_key = ls_security_key_match.group(1)

    # create directory for it
    dir = re.search(r'\/((?:.(?!\/))+)$', args.url).group(1)
    if not os.path.exists(dir):
        os.makedirs(dir)

    # download mp3 files
    data = {
        'bid': book_id,
        'security_ls_key': ls_security_key
    }
    request = urllib.request.Request(url='https://audioknigi.club/ajax/bid/' + book_id, method='POST',
        data=urllib.parse.urlencode(data).encode('utf8'))
    response = urllib.request.urlopen(request).read().decode('utf8')
    print(response)
    exit(1)
    titles = json.loads(response)
    index = 0
    for title in titles:
        filename = re.search(r'\/((?:.(?!\/))+)$', title['mp3']).group(1)
        if args.use_numbers:
            filename = str(index).zfill(3) + splitext(filename)[1]
            index = index + 1
        print('Downloading %s' % (filename))
        parsedurl = urllib.parse.urlsplit(title['mp3'])
        parsedurl = parsedurl._replace(path=urllib.parse.quote(parsedurl.path))
        success = False
        for i in range(1, ATTEMPTS_COUNT+1):
            try:
                urllib.request.urlretrieve(parsedurl.geturl(), os.path.join(dir, filename.lower()))
                success = True
            except urllib.error.HTTPError:
                print('Failed download %s for %d' % (filename, i))
                continue
            break
        if not success:
            raise Exception('Cannot download file after 3 attempts: %s' % (filename))
