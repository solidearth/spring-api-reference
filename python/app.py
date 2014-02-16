#!/usr/bin/env python
#Flask application using the Spring API.

from flask import Flask
from flask import jsonify
import requests
import urllib
import config
app = Flask(__name__)
sandbox = 'sandbox/' if config.sandbox else ''

@app.route('/')
def search():
    """Search for listings."""
    params = urllib.urlencode({'MLSStatusIn': 'Active', 'class': 'Residential'})
    r = requests.get(
        'https://%s/%s%s/%s/%s?format=json&api_key=%s&%s' %
        (config.api_base_url, sandbox, config.api_version, 'search', config.site, config.api_key, params))
    return jsonify(r.json())

if __name__ == '__main__':
    if config.api_key == '':
        raise RuntimeError('Please provide your Spring API key in config.py')
    app.run(debug=True)