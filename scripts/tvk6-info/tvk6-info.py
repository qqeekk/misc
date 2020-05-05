#!/usr/bin/env python
import argparse
import urllib
import urllib.request
from html.parser import HTMLParser

URL = 'https://tvk6.ru/'


class Tvk6HTMLParser(HTMLParser):
    def __init__(self):
        HTMLParser.__init__(self)
        self.temperature = ''
        self.__parse_weather = False

    def handle_starttag(self, tag, attrs):
        def has_attr(name, value):
            return len(list(filter(lambda attr: attr[0] == name and attr[1] == value, attrs))) > 0
        if tag == 'li' and has_attr('class', 'widget_item widget_weather'):
            self.__parse_weather = True

    def handle_data(self, data):
        if self.__parse_weather:
            self.temperature = data


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Parse head info in tvk6.ru site.')
    parser.add_argument('--type', help='The type of info to get.', required=True)
    args = parser.parse_args()

    response = urllib.request.urlopen(URL)
    parser = Tvk6HTMLParser()
    parser.feed(str(response.read()))
    if args.type == 'temperature':
        print(parser.temperature)
