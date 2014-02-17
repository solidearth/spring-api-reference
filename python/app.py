#!/usr/bin/env python
#Flask application using the Spring API.

from flask import Flask
from flask import request
from flask import session
from flask import render_template
from flask import jsonify
from flask import redirect
from flask import url_for
import requests
import urllib
import config
app = Flask(__name__)
sandbox = 'sandbox/' if config.sandbox else ''

# TODO: refactor how the base_url is built to deal with differences in end point urls
# TODO: refactor login in templates to use template inheritance
# TODO: add simple menu using template inheritance

def base_url(endpoint, id=None, secure=False):
    """Returns a base url for the API using the endpoint provided and the id provided if available.
       id is used to fetch individual resources, while secure denotes a url requiring an access token.
    """
    identifier = '/%s' % id if id else ''
    key = 'api_key=%s' % config.api_key if not secure else 'access_token=%s' % session['token']
    url = 'https://%s/%s%s/%s/%s%s?format=json&%s' % \
        (config.api_base_url, sandbox, config.api_version, endpoint, config.site, identifier, key)
    return url

@app.route('/search')
def search():
    """Search for listings."""
    params = urllib.urlencode({'MLSStatusIn': 'Active', 'class': 'Residential', 'page': request.args.get('page', '0')})
    r = requests.get('%s&%s' % (base_url('search'), params))
    results = r.json()
    return render_template('search.html', results=results)

@app.route('/listing/<listing_id>')
def listing(listing_id):
    """Get a single listing by its identifier."""
    r = requests.get('%s' % base_url('listing', listing_id))
    results = r.json()
    return render_template('listing.html', listing=results)

@app.route('/listing_counts/<listing_id>')
def listing_counts(listing_id):
    if not 'token' in session:
        return redirect(url_for('login'))
    r = requests.get('%s' % base_url('listing', 'counters/%s' % listing_id, secure=True))
    results = r.json()
    return render_template('listinganalytics.html', results=results)

@app.route('/login')
def login():
    """Login to API using oauth."""
    if not 'token' in session:
        return redirect(config.oauth_url % (config.api_key, 'http://localhost:5000%s' % url_for('oauth')));
    else:
        return redirect(url_for('search'))

@app.route('/logout')
def logout():
    """Logout of API by clearing oauth token."""
    del session['token']
    return redirect(url_for('search'))

@app.route('/oauth')
def oauth():
    """Handles storage of oauth token."""
    token = request.args.get('access_token', None)
    if token:
        session['token'] = token
    return redirect(url_for('search'))

if __name__ == '__main__':
    if config.api_key == '':
        raise RuntimeError('Please provide your Spring API key in config.py')
    if config.secret_key == '':
        raise RuntimeError('Please provide a secret key in config.py to enable sessions')
    print 'Warning: debug should be disabled for production deployments'
    app.secret_key = config.secret_key
    app.run(debug=True)
