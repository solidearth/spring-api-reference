var express = require('express')
    swig = require('swig'),
    config = require('./config.json'),
    Router = require('reversable-router'),
    request = require('request'),
    util = require('util');

// should the sandbox endpoint be used?
var sandbox = (config.sandbox) ? 'sandbox/' : '';
var app = express();
// use reversable router to get named routes and url generation from routes
var router = new Router();
router.extendExpress(app);
router.registerAppHelpers(app);

// configuration
app.configure(function() {  
    app.set('port', config.port);
    app.use(express.bodyParser());
    app.use(express.errorHandler());
    // NOTE: consider using a database or memory backed session store in a production environment.
    app.use(express.cookieParser(config.secret));
    app.use(express.cookieSession());
    app.engine('html', swig.renderFile);
    app.set('view engine', 'html');
    app.set('views', __dirname + '/views');
    // include request and session in locals for access in views
    app.use(function (req, res, next) {
        res.locals.req = req;
        res.locals.session = req.session;
        next();
    });
    app.use(app.router);

    // NOTE: You should always cache templates in a production environment.
    app.set('view cache', false);
    swig.setDefaults({ cache: false });
});

// routes
app.get('/', 'index', index);
app.get('/search', 'search', search);
app.get('/listing/:listing_id', 'listing', listing);
app.get('/listing_counts/:listing_id', 'listing_counts', listing_counts);
app.get('/login', 'login', login);
app.get('/logout', 'logout', logout);
app.get('/oauth', 'oauth', oauth);

// route handlers
function index(req, res) {
    res.redirect(app._router.build('search'));
}

// Search for listings.
function search(req, res) {
    var url = base_url(req, 'search')();
    var params = [
        'MlsStatusin=Active',
        'class=Residential',
        util.format('page=%s', req.query.page || 0),
        'expand=true'
    ].join('&');

    request(util.format('%s&%s', url, params), function (error, response, body) {
        res.render('search', { 'results': JSON.parse(body), 'mapbox_key': config.mapbox_key });
    });
}

// Get a single listing by its identifier.
function listing(req, res) {
    var url = base_url(req, 'listing')({
        'id': req.params.listing_id
    });
    var params = ['expand=true'].join('&');

    request(util.format('%s&%s', url, params), function (error, response, body) {
        res.render('listing', { 'listing': JSON.parse(body), 'mapbox_key': config.mapbox_key });
    });
}

// Get counts for a single listing by identifier.
function listing_counts(req, res) {
    if (!req.session.token) {
        res.redirect(app._router.build('login'));
    } else {
        var url = base_url(req, 'listing')({
            'id': req.params.listing_id,
            'subendpoint': 'counters',
            'secure': true
        });
        
        request(url, function (error, response, body) {
            res.render('listinganalytics', { 'results': JSON.parse(body) });
        });
    }
}

// Login using oauth.
function login(req, res) {
    if (!req.session.token) {
        res.redirect(
            util.format(
                config.oauth_url,
                config.api_key,
                util.format('http://localhost:5001%s', app._router.build('oauth'))
            )
        );
    } else {
        res.redirect(app._router.build('search'));
    }
}

// logout
function logout(req, res) {
    delete req.session.token;
    res.redirect(app._router.build('search'));
}

// oauth response handler
function oauth(req, res) {
    token = req.query.access_token;
    if (token) {
        req.session.token = token;
    }
    res.redirect(app._router.build('search'));
}

// Returns a base url for the API using the endpoint provided and configured with the provided options.
function base_url(request, endpoint) {
    return function (options) {
        var subendpoint = (options && options.subendpoint)
            ? util.format('/%s', options.subendpoint)
            : '';
        var identifier = (options && options.id)
            ? util.format('/%s', options.id)
            : '';
        var key = (options && options.secure)
            ? util.format('access_token=%s', request.session.token)
            : util.format('api_key=%s', config.api_key);
        var url = util.format(
            'https://%s/%s%s/%s/%s%s%s?format=json&%s',
            config.api_base_url,
            sandbox,
            config.api_version,
            endpoint,
            config.site,
            subendpoint,
            identifier,
            key
        );
        return url;
    }
}

if (config.api_key == '') {
    console.error('Please provide your Spring API key in config.json');
    process.exit(1);
}
if (config.secret == '') {
    console.error('Please provide a secret key in config.json to enable sessions');
    process.exit(1);
}
app.listen(app.get('port'));
console.log('Listening on port', app.get('port'));
